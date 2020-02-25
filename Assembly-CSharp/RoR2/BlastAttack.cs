using System;
using System.Runtime.CompilerServices;
using RoR2.Networking;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020000C3 RID: 195
	public class BlastAttack
	{
		// Token: 0x060003CE RID: 974 RVA: 0x0000EB04 File Offset: 0x0000CD04
		public BlastAttack.Result Fire()
		{
			BlastAttack.HitPoint[] array = this.CollectHits();
			this.HandleHits(array);
			return new BlastAttack.Result
			{
				hitCount = array.Length
			};
		}

		// Token: 0x060003CF RID: 975 RVA: 0x0000EB34 File Offset: 0x0000CD34
		[NetworkMessageHandler(msgType = 75, client = false, server = true)]
		private static void HandleReportBlastAttackDamage(NetworkMessage netMsg)
		{
			NetworkReader reader = netMsg.reader;
			BlastAttack.BlastAttackDamageInfo blastAttackDamageInfo = default(BlastAttack.BlastAttackDamageInfo);
			blastAttackDamageInfo.Read(reader);
			BlastAttack.PerformDamageServer(blastAttackDamageInfo);
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x0000EB60 File Offset: 0x0000CD60
		private BlastAttack.HitPoint[] CollectHits()
		{
			Vector3 vector = this.position;
			Collider[] array = Physics.OverlapSphere(vector, this.radius, LayerIndex.entityPrecise.mask);
			int num = array.Length;
			int num2 = 0;
			BlastAttack.<>c__DisplayClass27_0 CS$<>8__locals1;
			CS$<>8__locals1.encounteredHealthComponentsLength = 0;
			CS$<>8__locals1.hitOrderBufferLength = 0;
			HGArrayUtilities.EnsureCapacity<BlastAttack.HitPoint>(ref BlastAttack.hitPointsBuffer, num);
			HGArrayUtilities.EnsureCapacity<int>(ref BlastAttack.hitOrderBuffer, num);
			HGArrayUtilities.EnsureCapacity<HealthComponent>(ref BlastAttack.encounteredHealthComponentsBuffer, num);
			for (int i = 0; i < num; i++)
			{
				Collider collider = array[i];
				HurtBox component = collider.GetComponent<HurtBox>();
				if (component)
				{
					HealthComponent healthComponent = component.healthComponent;
					if (healthComponent && ((this.canHurtAttacker && healthComponent.gameObject == this.attacker) || healthComponent.GetComponent<TeamComponent>().teamIndex != this.teamIndex))
					{
						Vector3 vector2 = collider.transform.position;
						Vector3 hitNormal = vector2 - vector;
						float sqrMagnitude = hitNormal.sqrMagnitude;
						BlastAttack.hitPointsBuffer[num2++] = new BlastAttack.HitPoint
						{
							hurtBox = component,
							hitPosition = vector2,
							hitNormal = hitNormal,
							distanceSqr = sqrMagnitude
						};
					}
				}
			}
			if (true)
			{
				for (int j = 0; j < num2; j++)
				{
					ref BlastAttack.HitPoint ptr = ref BlastAttack.hitPointsBuffer[j];
					RaycastHit raycastHit;
					if (ptr.hurtBox != null && ptr.distanceSqr > 0f && ptr.hurtBox.collider.Raycast(new Ray(vector, -ptr.hitNormal), out raycastHit, this.radius))
					{
						ptr.hitPosition = raycastHit.point;
						ptr.hitNormal = raycastHit.normal;
					}
				}
			}
			CS$<>8__locals1.hitOrderBufferLength = num2;
			for (int k = 0; k < num2; k++)
			{
				BlastAttack.hitOrderBuffer[k] = k;
			}
			bool flag = true;
			while (flag)
			{
				flag = false;
				for (int l = 1; l < CS$<>8__locals1.hitOrderBufferLength; l++)
				{
					int num3 = l - 1;
					if (BlastAttack.hitPointsBuffer[BlastAttack.hitOrderBuffer[num3]].distanceSqr > BlastAttack.hitPointsBuffer[BlastAttack.hitOrderBuffer[l]].distanceSqr)
					{
						Util.Swap<int>(ref BlastAttack.hitOrderBuffer[num3], ref BlastAttack.hitOrderBuffer[l]);
						flag = true;
					}
				}
			}
			bool flag2 = this.losType == BlastAttack.LoSType.None || this.losType == BlastAttack.LoSType.NearestHit;
			for (int m = 0; m < CS$<>8__locals1.hitOrderBufferLength; m++)
			{
				int num4 = BlastAttack.hitOrderBuffer[m];
				ref BlastAttack.HitPoint ptr2 = ref BlastAttack.hitPointsBuffer[num4];
				HealthComponent healthComponent2 = ptr2.hurtBox.healthComponent;
				if (!BlastAttack.<CollectHits>g__EntityIsMarkedEncountered|27_1(healthComponent2, ref CS$<>8__locals1))
				{
					BlastAttack.<CollectHits>g__MarkEntityAsEncountered|27_2(healthComponent2, ref CS$<>8__locals1);
				}
				else if (flag2)
				{
					ptr2.hurtBox = null;
				}
			}
			BlastAttack.<CollectHits>g__ClearEncounteredEntities|27_3(ref CS$<>8__locals1);
			BlastAttack.<CollectHits>g__CondenseHitOrderBuffer|27_0(ref CS$<>8__locals1);
			BlastAttack.LoSType loSType = this.losType;
			if (loSType != BlastAttack.LoSType.None && loSType == BlastAttack.LoSType.NearestHit)
			{
				NativeArray<RaycastCommand> commands = new NativeArray<RaycastCommand>(CS$<>8__locals1.hitOrderBufferLength, Allocator.TempJob, NativeArrayOptions.ClearMemory);
				NativeArray<RaycastHit> results = new NativeArray<RaycastHit>(CS$<>8__locals1.hitOrderBufferLength, Allocator.TempJob, NativeArrayOptions.ClearMemory);
				int n = 0;
				int num5 = 0;
				while (n < CS$<>8__locals1.hitOrderBufferLength)
				{
					int num6 = BlastAttack.hitOrderBuffer[n];
					ref BlastAttack.HitPoint ptr3 = ref BlastAttack.hitPointsBuffer[num6];
					commands[num5++] = new RaycastCommand(vector, ptr3.hitPosition - vector, Mathf.Sqrt(ptr3.distanceSqr), LayerIndex.world.mask, 1);
					n++;
				}
				bool queriesHitTriggers = Physics.queriesHitTriggers;
				Physics.queriesHitTriggers = true;
				RaycastCommand.ScheduleBatch(commands, results, 1, default(JobHandle)).Complete();
				Physics.queriesHitTriggers = queriesHitTriggers;
				int num7 = 0;
				int num8 = 0;
				while (num7 < CS$<>8__locals1.hitOrderBufferLength)
				{
					int num9 = BlastAttack.hitOrderBuffer[num7];
					ref BlastAttack.HitPoint ptr4 = ref BlastAttack.hitPointsBuffer[num9];
					if (ptr4.hurtBox != null && results[num8++].collider)
					{
						ptr4.hurtBox = null;
					}
					num7++;
				}
				results.Dispose();
				commands.Dispose();
				BlastAttack.<CollectHits>g__CondenseHitOrderBuffer|27_0(ref CS$<>8__locals1);
			}
			BlastAttack.HitPoint[] array2 = new BlastAttack.HitPoint[CS$<>8__locals1.hitOrderBufferLength];
			for (int num10 = 0; num10 < CS$<>8__locals1.hitOrderBufferLength; num10++)
			{
				int num11 = BlastAttack.hitOrderBuffer[num10];
				array2[num10] = BlastAttack.hitPointsBuffer[num11];
			}
			HGArrayUtilities.Clear<BlastAttack.HitPoint>(BlastAttack.hitPointsBuffer, ref num2);
			BlastAttack.<CollectHits>g__ClearEncounteredEntities|27_3(ref CS$<>8__locals1);
			return array2;
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x0000EFE0 File Offset: 0x0000D1E0
		private void HandleHits(BlastAttack.HitPoint[] hitPoints)
		{
			Vector3 b = this.position;
			foreach (BlastAttack.HitPoint hitPoint in hitPoints)
			{
				float num = Mathf.Sqrt(hitPoint.distanceSqr);
				float num2 = 0f;
				Vector3 a = (num > 0f) ? ((hitPoint.hitPosition - b) / num) : Vector3.zero;
				HealthComponent healthComponent = hitPoint.hurtBox ? hitPoint.hurtBox.healthComponent : null;
				if (healthComponent)
				{
					switch (this.falloffModel)
					{
					case BlastAttack.FalloffModel.None:
						num2 = 1f;
						break;
					case BlastAttack.FalloffModel.Linear:
						num2 = 1f - Mathf.Clamp01(num / this.radius);
						break;
					case BlastAttack.FalloffModel.SweetSpot:
						num2 = 1f - ((num > this.radius / 2f) ? 0.75f : 0f);
						break;
					}
					BlastAttack.BlastAttackDamageInfo blastAttackDamageInfo = new BlastAttack.BlastAttackDamageInfo
					{
						attacker = this.attacker,
						inflictor = this.inflictor,
						crit = this.crit,
						damage = this.baseDamage * num2,
						damageColorIndex = this.damageColorIndex,
						damageModifier = hitPoint.hurtBox.damageModifier,
						damageType = (this.damageType | DamageType.AOE),
						force = this.bonusForce * num2 + this.baseForce * num2 * a,
						position = hitPoint.hitPosition,
						procChainMask = this.procChainMask,
						procCoefficient = this.procCoefficient,
						hitHealthComponent = healthComponent
					};
					if (NetworkServer.active)
					{
						BlastAttack.PerformDamageServer(blastAttackDamageInfo);
					}
					else
					{
						BlastAttack.ClientReportDamage(blastAttackDamageInfo);
					}
					if (this.impactEffect != EffectIndex.Invalid)
					{
						EffectData effectData = new EffectData();
						effectData.origin = hitPoint.hitPosition;
						effectData.rotation = Quaternion.LookRotation(-a);
						EffectManager.SpawnEffect(this.impactEffect, effectData, true);
					}
				}
			}
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x0000F1F4 File Offset: 0x0000D3F4
		private static void ClientReportDamage(in BlastAttack.BlastAttackDamageInfo blastAttackDamageInfo)
		{
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.StartMessage(75);
			BlastAttack.BlastAttackDamageInfo blastAttackDamageInfo2 = blastAttackDamageInfo;
			blastAttackDamageInfo2.Write(networkWriter);
			networkWriter.FinishMessage();
			GameNetworkManager.singleton.client.connection.SendWriter(networkWriter, QosChannelIndex.defaultReliable.intVal);
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x0000F244 File Offset: 0x0000D444
		private static void PerformDamageServer(in BlastAttack.BlastAttackDamageInfo blastAttackDamageInfo)
		{
			if (!blastAttackDamageInfo.hitHealthComponent)
			{
				return;
			}
			DamageInfo damageInfo = new DamageInfo();
			damageInfo.attacker = blastAttackDamageInfo.attacker;
			damageInfo.inflictor = blastAttackDamageInfo.inflictor;
			damageInfo.damage = blastAttackDamageInfo.damage;
			damageInfo.crit = blastAttackDamageInfo.crit;
			damageInfo.force = blastAttackDamageInfo.force;
			damageInfo.procChainMask = blastAttackDamageInfo.procChainMask;
			damageInfo.procCoefficient = blastAttackDamageInfo.procCoefficient;
			damageInfo.damageType = blastAttackDamageInfo.damageType;
			damageInfo.damageColorIndex = blastAttackDamageInfo.damageColorIndex;
			damageInfo.position = blastAttackDamageInfo.position;
			damageInfo.ModifyDamageInfo(blastAttackDamageInfo.damageModifier);
			blastAttackDamageInfo.hitHealthComponent.TakeDamage(damageInfo);
			GlobalEventManager.instance.OnHitEnemy(damageInfo, blastAttackDamageInfo.hitHealthComponent.gameObject);
			GlobalEventManager.instance.OnHitAll(damageInfo, blastAttackDamageInfo.hitHealthComponent.gameObject);
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x0000F37C File Offset: 0x0000D57C
		[CompilerGenerated]
		internal static void <CollectHits>g__CondenseHitOrderBuffer|27_0(ref BlastAttack.<>c__DisplayClass27_0 A_0)
		{
			for (int i = 0; i < A_0.hitOrderBufferLength; i++)
			{
				int num = 0;
				for (int j = i; j < A_0.hitOrderBufferLength; j++)
				{
					int num2 = BlastAttack.hitOrderBuffer[j];
					if (BlastAttack.hitPointsBuffer[num2].hurtBox != null)
					{
						break;
					}
					num++;
				}
				if (num > 0)
				{
					HGArrayUtilities.ArrayRemoveAt<int>(ref BlastAttack.hitOrderBuffer, ref A_0.hitOrderBufferLength, i, num);
				}
			}
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x0000F3E4 File Offset: 0x0000D5E4
		[CompilerGenerated]
		internal static bool <CollectHits>g__EntityIsMarkedEncountered|27_1(HealthComponent healthComponent, ref BlastAttack.<>c__DisplayClass27_0 A_1)
		{
			for (int i = 0; i < A_1.encounteredHealthComponentsLength; i++)
			{
				if (BlastAttack.encounteredHealthComponentsBuffer[i] == healthComponent)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x0000F410 File Offset: 0x0000D610
		[CompilerGenerated]
		internal static void <CollectHits>g__MarkEntityAsEncountered|27_2(HealthComponent healthComponent, ref BlastAttack.<>c__DisplayClass27_0 A_1)
		{
			HealthComponent[] array = BlastAttack.encounteredHealthComponentsBuffer;
			int encounteredHealthComponentsLength = A_1.encounteredHealthComponentsLength;
			A_1.encounteredHealthComponentsLength = encounteredHealthComponentsLength + 1;
			array[encounteredHealthComponentsLength] = healthComponent;
		}

		// Token: 0x060003D9 RID: 985 RVA: 0x0000F435 File Offset: 0x0000D635
		[CompilerGenerated]
		internal static void <CollectHits>g__ClearEncounteredEntities|27_3(ref BlastAttack.<>c__DisplayClass27_0 A_0)
		{
			Array.Clear(BlastAttack.encounteredHealthComponentsBuffer, 0, A_0.encounteredHealthComponentsLength);
			A_0.encounteredHealthComponentsLength = 0;
		}

		// Token: 0x0400035C RID: 860
		public GameObject attacker;

		// Token: 0x0400035D RID: 861
		public GameObject inflictor;

		// Token: 0x0400035E RID: 862
		public TeamIndex teamIndex;

		// Token: 0x0400035F RID: 863
		public bool canHurtAttacker;

		// Token: 0x04000360 RID: 864
		public Vector3 position;

		// Token: 0x04000361 RID: 865
		public float radius;

		// Token: 0x04000362 RID: 866
		public BlastAttack.FalloffModel falloffModel = BlastAttack.FalloffModel.Linear;

		// Token: 0x04000363 RID: 867
		public float baseDamage;

		// Token: 0x04000364 RID: 868
		public float baseForce;

		// Token: 0x04000365 RID: 869
		public Vector3 bonusForce;

		// Token: 0x04000366 RID: 870
		public bool crit;

		// Token: 0x04000367 RID: 871
		public DamageType damageType;

		// Token: 0x04000368 RID: 872
		public DamageColorIndex damageColorIndex;

		// Token: 0x04000369 RID: 873
		public BlastAttack.LoSType losType;

		// Token: 0x0400036A RID: 874
		public EffectIndex impactEffect = EffectIndex.Invalid;

		// Token: 0x0400036B RID: 875
		public ProcChainMask procChainMask;

		// Token: 0x0400036C RID: 876
		public float procCoefficient = 1f;

		// Token: 0x0400036D RID: 877
		private static readonly int initialBufferSize = 256;

		// Token: 0x0400036E RID: 878
		private static BlastAttack.HitPoint[] hitPointsBuffer = new BlastAttack.HitPoint[BlastAttack.initialBufferSize];

		// Token: 0x0400036F RID: 879
		private static int[] hitOrderBuffer = new int[BlastAttack.initialBufferSize];

		// Token: 0x04000370 RID: 880
		private static HealthComponent[] encounteredHealthComponentsBuffer = new HealthComponent[BlastAttack.initialBufferSize];

		// Token: 0x020000C4 RID: 196
		public enum FalloffModel
		{
			// Token: 0x04000372 RID: 882
			None,
			// Token: 0x04000373 RID: 883
			Linear,
			// Token: 0x04000374 RID: 884
			SweetSpot
		}

		// Token: 0x020000C5 RID: 197
		public enum LoSType
		{
			// Token: 0x04000376 RID: 886
			None,
			// Token: 0x04000377 RID: 887
			NearestHit
		}

		// Token: 0x020000C6 RID: 198
		private struct HitPoint
		{
			// Token: 0x04000378 RID: 888
			public HurtBox hurtBox;

			// Token: 0x04000379 RID: 889
			public Vector3 hitPosition;

			// Token: 0x0400037A RID: 890
			public Vector3 hitNormal;

			// Token: 0x0400037B RID: 891
			public float distanceSqr;
		}

		// Token: 0x020000C7 RID: 199
		public struct Result
		{
			// Token: 0x0400037C RID: 892
			public int hitCount;
		}

		// Token: 0x020000C8 RID: 200
		private struct BlastAttackDamageInfo
		{
			// Token: 0x060003DA RID: 986 RVA: 0x0000F450 File Offset: 0x0000D650
			public void Write(NetworkWriter writer)
			{
				writer.Write(this.attacker);
				writer.Write(this.inflictor);
				writer.Write(this.crit);
				writer.Write(this.damage);
				writer.Write(this.damageColorIndex);
				writer.Write((byte)this.damageModifier);
				writer.Write(this.damageType);
				writer.Write(this.force);
				writer.Write(this.position);
				writer.Write(this.procChainMask);
				writer.Write(this.procCoefficient);
				writer.Write(this.hitHealthComponent.netId);
			}

			// Token: 0x060003DB RID: 987 RVA: 0x0000F4F8 File Offset: 0x0000D6F8
			public void Read(NetworkReader reader)
			{
				this.attacker = reader.ReadGameObject();
				this.inflictor = reader.ReadGameObject();
				this.crit = reader.ReadBoolean();
				this.damage = reader.ReadSingle();
				this.damageColorIndex = reader.ReadDamageColorIndex();
				this.damageModifier = (HurtBox.DamageModifier)reader.ReadByte();
				this.damageType = reader.ReadDamageType();
				this.force = reader.ReadVector3();
				this.position = reader.ReadVector3();
				this.procChainMask = reader.ReadProcChainMask();
				this.procCoefficient = reader.ReadSingle();
				GameObject gameObject = reader.ReadGameObject();
				this.hitHealthComponent = (gameObject ? gameObject.GetComponent<HealthComponent>() : null);
			}

			// Token: 0x0400037D RID: 893
			public GameObject attacker;

			// Token: 0x0400037E RID: 894
			public GameObject inflictor;

			// Token: 0x0400037F RID: 895
			public bool crit;

			// Token: 0x04000380 RID: 896
			public float damage;

			// Token: 0x04000381 RID: 897
			public DamageColorIndex damageColorIndex;

			// Token: 0x04000382 RID: 898
			public HurtBox.DamageModifier damageModifier;

			// Token: 0x04000383 RID: 899
			public DamageType damageType;

			// Token: 0x04000384 RID: 900
			public Vector3 force;

			// Token: 0x04000385 RID: 901
			public Vector3 position;

			// Token: 0x04000386 RID: 902
			public ProcChainMask procChainMask;

			// Token: 0x04000387 RID: 903
			public float procCoefficient;

			// Token: 0x04000388 RID: 904
			public HealthComponent hitHealthComponent;
		}
	}
}
