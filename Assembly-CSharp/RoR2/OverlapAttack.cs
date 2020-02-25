using System;
using System.Collections.Generic;
using RoR2.Audio;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003D5 RID: 981
	public class OverlapAttack
	{
		// Token: 0x170002C4 RID: 708
		// (get) Token: 0x060017D2 RID: 6098 RVA: 0x00067441 File Offset: 0x00065641
		// (set) Token: 0x060017D3 RID: 6099 RVA: 0x00067449 File Offset: 0x00065649
		public Vector3 lastFireAverageHitPosition { get; private set; }

		// Token: 0x060017D4 RID: 6100 RVA: 0x00067454 File Offset: 0x00065654
		private bool HurtBoxPassesFilter(HurtBox hurtBox)
		{
			if (!hurtBox.healthComponent)
			{
				return true;
			}
			if (this.hitBoxGroup.transform.IsChildOf(hurtBox.healthComponent.transform))
			{
				return false;
			}
			if (this.ignoredHealthComponentList.Contains(hurtBox.healthComponent))
			{
				return false;
			}
			TeamComponent component = hurtBox.healthComponent.GetComponent<TeamComponent>();
			return !component || component.teamIndex != this.teamIndex;
		}

		// Token: 0x060017D5 RID: 6101 RVA: 0x000674CC File Offset: 0x000656CC
		public bool Fire(List<HealthComponent> hitResults = null)
		{
			if (!this.hitBoxGroup)
			{
				return false;
			}
			HitBox[] hitBoxes = this.hitBoxGroup.hitBoxes;
			for (int i = 0; i < hitBoxes.Length; i++)
			{
				Transform transform = hitBoxes[i].transform;
				Vector3 position = transform.position;
				Vector3 vector = transform.lossyScale * 0.5f;
				Quaternion rotation = transform.rotation;
				if (float.IsInfinity(vector.x) || float.IsInfinity(vector.y) || float.IsInfinity(vector.z))
				{
					Chat.AddMessage("Aborting OverlapAttack.Fire: hitBoxHalfExtents are infinite.");
				}
				else if (float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z))
				{
					Chat.AddMessage("Aborting OverlapAttack.Fire: hitBoxHalfExtents are NaN.");
				}
				else if (float.IsInfinity(position.x) || float.IsInfinity(position.y) || float.IsInfinity(position.z))
				{
					Chat.AddMessage("Aborting OverlapAttack.Fire: hitBoxCenter is infinite.");
				}
				else if (float.IsNaN(position.x) || float.IsNaN(position.y) || float.IsNaN(position.z))
				{
					Chat.AddMessage("Aborting OverlapAttack.Fire: hitBoxCenter is NaN.");
				}
				else if (float.IsInfinity(rotation.x) || float.IsInfinity(rotation.y) || float.IsInfinity(rotation.z) || float.IsInfinity(rotation.w))
				{
					Chat.AddMessage("Aborting OverlapAttack.Fire: hitBoxRotation is infinite.");
				}
				else if (float.IsNaN(rotation.x) || float.IsNaN(rotation.y) || float.IsNaN(rotation.z) || float.IsNaN(rotation.w))
				{
					Chat.AddMessage("Aborting OverlapAttack.Fire: hitBoxRotation is NaN.");
				}
				else
				{
					Collider[] array = Physics.OverlapBox(position, vector, rotation, LayerIndex.entityPrecise.mask);
					int num = array.Length;
					int num2 = 0;
					for (int j = 0; j < num; j++)
					{
						HurtBox component = array[j].GetComponent<HurtBox>();
						if (component && this.HurtBoxPassesFilter(component))
						{
							Vector3 position2 = component.transform.position;
							this.overlapList.Add(new OverlapAttack.OverlapInfo
							{
								hurtBox = component,
								hitPosition = position2,
								pushDirection = (position2 - position).normalized
							});
							this.ignoredHealthComponentList.Add(component.healthComponent);
							if (hitResults != null)
							{
								hitResults.Add(component.healthComponent);
							}
							num2++;
						}
						if (num2 >= this.maximumOverlapTargets)
						{
							break;
						}
					}
				}
			}
			this.ProcessHits(this.overlapList);
			bool result = this.overlapList.Count > 0;
			this.overlapList.Clear();
			return result;
		}

		// Token: 0x060017D6 RID: 6102 RVA: 0x00067798 File Offset: 0x00065998
		[NetworkMessageHandler(msgType = 71, client = false, server = true)]
		public static void HandleOverlapAttackHits(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<OverlapAttack.OverlapAttackMessage>(OverlapAttack.incomingMessage);
			OverlapAttack.PerformDamage(OverlapAttack.incomingMessage.attacker, OverlapAttack.incomingMessage.inflictor, OverlapAttack.incomingMessage.damage, OverlapAttack.incomingMessage.isCrit, OverlapAttack.incomingMessage.procChainMask, OverlapAttack.incomingMessage.procCoefficient, OverlapAttack.incomingMessage.damageColorIndex, OverlapAttack.incomingMessage.damageType, OverlapAttack.incomingMessage.forceVector, OverlapAttack.incomingMessage.pushAwayForce, OverlapAttack.incomingMessage.overlapInfoList);
		}

		// Token: 0x060017D7 RID: 6103 RVA: 0x00067824 File Offset: 0x00065A24
		private void ProcessHits(List<OverlapAttack.OverlapInfo> hitList)
		{
			if (hitList.Count == 0)
			{
				return;
			}
			Vector3 vector = Vector3.zero;
			float d = 1f / (float)hitList.Count;
			for (int i = 0; i < hitList.Count; i++)
			{
				OverlapAttack.OverlapInfo overlapInfo = hitList[i];
				if (this.hitEffectPrefab)
				{
					Vector3 forward = -hitList[i].pushDirection;
					EffectManager.SpawnEffect(this.hitEffectPrefab, new EffectData
					{
						origin = overlapInfo.hitPosition,
						rotation = Util.QuaternionSafeLookRotation(forward),
						networkSoundEventIndex = this.impactSound
					}, true);
				}
				vector += overlapInfo.hitPosition * d;
				SurfaceDefProvider component = hitList[i].hurtBox.GetComponent<SurfaceDefProvider>();
				if (component && component.surfaceDef)
				{
					SurfaceDef objectSurfaceDef = SurfaceDefProvider.GetObjectSurfaceDef(hitList[i].hurtBox.collider, hitList[i].hitPosition);
					if (objectSurfaceDef)
					{
						if (objectSurfaceDef.impactEffectPrefab)
						{
							EffectManager.SpawnEffect(objectSurfaceDef.impactEffectPrefab, new EffectData
							{
								origin = overlapInfo.hitPosition,
								rotation = ((overlapInfo.pushDirection == Vector3.zero) ? Quaternion.identity : Util.QuaternionSafeLookRotation(overlapInfo.pushDirection)),
								color = objectSurfaceDef.approximateColor,
								scale = 2f
							}, true);
						}
						if (objectSurfaceDef.impactSoundString != null && objectSurfaceDef.impactSoundString.Length != 0)
						{
							Util.PlaySound(objectSurfaceDef.impactSoundString, hitList[i].hurtBox.gameObject);
						}
					}
				}
			}
			this.lastFireAverageHitPosition = vector;
			if (NetworkServer.active)
			{
				OverlapAttack.PerformDamage(this.attacker, this.inflictor, this.damage, this.isCrit, this.procChainMask, this.procCoefficient, this.damageColorIndex, this.damageType, this.forceVector, this.pushAwayForce, hitList);
				return;
			}
			OverlapAttack.outgoingMessage.attacker = this.attacker;
			OverlapAttack.outgoingMessage.inflictor = this.inflictor;
			OverlapAttack.outgoingMessage.damage = this.damage;
			OverlapAttack.outgoingMessage.isCrit = this.isCrit;
			OverlapAttack.outgoingMessage.procChainMask = this.procChainMask;
			OverlapAttack.outgoingMessage.procCoefficient = this.procCoefficient;
			OverlapAttack.outgoingMessage.damageColorIndex = this.damageColorIndex;
			OverlapAttack.outgoingMessage.damageType = this.damageType;
			OverlapAttack.outgoingMessage.forceVector = this.forceVector;
			OverlapAttack.outgoingMessage.pushAwayForce = this.pushAwayForce;
			Util.CopyList<OverlapAttack.OverlapInfo>(hitList, OverlapAttack.outgoingMessage.overlapInfoList);
			GameNetworkManager.singleton.client.connection.SendByChannel(71, OverlapAttack.outgoingMessage, QosChannelIndex.defaultReliable.intVal);
		}

		// Token: 0x060017D8 RID: 6104 RVA: 0x00067B08 File Offset: 0x00065D08
		private static void PerformDamage(GameObject attacker, GameObject inflictor, float damage, bool isCrit, ProcChainMask procChainMask, float procCoefficient, DamageColorIndex damageColorIndex, DamageType damageType, Vector3 forceVector, float pushAwayForce, List<OverlapAttack.OverlapInfo> hitList)
		{
			for (int i = 0; i < hitList.Count; i++)
			{
				OverlapAttack.OverlapInfo overlapInfo = hitList[i];
				if (overlapInfo.hurtBox)
				{
					HealthComponent healthComponent = overlapInfo.hurtBox.healthComponent;
					if (healthComponent)
					{
						DamageInfo damageInfo = new DamageInfo();
						damageInfo.attacker = attacker;
						damageInfo.inflictor = inflictor;
						damageInfo.force = forceVector + pushAwayForce * overlapInfo.pushDirection;
						damageInfo.damage = damage;
						damageInfo.crit = isCrit;
						damageInfo.position = overlapInfo.hitPosition;
						damageInfo.procChainMask = procChainMask;
						damageInfo.procCoefficient = procCoefficient;
						damageInfo.damageColorIndex = damageColorIndex;
						damageInfo.damageType = damageType;
						damageInfo.ModifyDamageInfo(overlapInfo.hurtBox.damageModifier);
						healthComponent.TakeDamage(damageInfo);
						GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
						GlobalEventManager.instance.OnHitAll(damageInfo, healthComponent.gameObject);
					}
				}
			}
		}

		// Token: 0x060017D9 RID: 6105 RVA: 0x00067BFF File Offset: 0x00065DFF
		public void ResetIgnoredHealthComponents()
		{
			this.ignoredHealthComponentList.Clear();
		}

		// Token: 0x0400166C RID: 5740
		public GameObject attacker;

		// Token: 0x0400166D RID: 5741
		public GameObject inflictor;

		// Token: 0x0400166E RID: 5742
		public TeamIndex teamIndex;

		// Token: 0x0400166F RID: 5743
		public Vector3 forceVector = Vector3.zero;

		// Token: 0x04001670 RID: 5744
		public float pushAwayForce;

		// Token: 0x04001671 RID: 5745
		public float damage = 1f;

		// Token: 0x04001672 RID: 5746
		public bool isCrit;

		// Token: 0x04001673 RID: 5747
		public ProcChainMask procChainMask;

		// Token: 0x04001674 RID: 5748
		public float procCoefficient = 1f;

		// Token: 0x04001675 RID: 5749
		public HitBoxGroup hitBoxGroup;

		// Token: 0x04001676 RID: 5750
		public GameObject hitEffectPrefab;

		// Token: 0x04001677 RID: 5751
		public NetworkSoundEventIndex impactSound = NetworkSoundEventIndex.Invalid;

		// Token: 0x04001678 RID: 5752
		public DamageColorIndex damageColorIndex;

		// Token: 0x04001679 RID: 5753
		public DamageType damageType;

		// Token: 0x0400167A RID: 5754
		public int maximumOverlapTargets = 100;

		// Token: 0x0400167B RID: 5755
		private readonly List<HealthComponent> ignoredHealthComponentList = new List<HealthComponent>();

		// Token: 0x0400167D RID: 5757
		private readonly List<OverlapAttack.OverlapInfo> overlapList = new List<OverlapAttack.OverlapInfo>();

		// Token: 0x0400167E RID: 5758
		private static readonly OverlapAttack.OverlapAttackMessage incomingMessage = new OverlapAttack.OverlapAttackMessage();

		// Token: 0x0400167F RID: 5759
		private static readonly OverlapAttack.OverlapAttackMessage outgoingMessage = new OverlapAttack.OverlapAttackMessage();

		// Token: 0x020003D6 RID: 982
		private struct OverlapInfo
		{
			// Token: 0x04001680 RID: 5760
			public HurtBox hurtBox;

			// Token: 0x04001681 RID: 5761
			public Vector3 hitPosition;

			// Token: 0x04001682 RID: 5762
			public Vector3 pushDirection;
		}

		// Token: 0x020003D7 RID: 983
		public struct AttackInfo
		{
			// Token: 0x04001683 RID: 5763
			public GameObject attacker;

			// Token: 0x04001684 RID: 5764
			public GameObject inflictor;

			// Token: 0x04001685 RID: 5765
			public float damage;

			// Token: 0x04001686 RID: 5766
			public bool isCrit;

			// Token: 0x04001687 RID: 5767
			public float procCoefficient;

			// Token: 0x04001688 RID: 5768
			public DamageColorIndex damageColorIndex;

			// Token: 0x04001689 RID: 5769
			public DamageType damageType;

			// Token: 0x0400168A RID: 5770
			public Vector3 forceVector;
		}

		// Token: 0x020003D8 RID: 984
		private class OverlapAttackMessage : MessageBase
		{
			// Token: 0x060017DC RID: 6108 RVA: 0x00067C7C File Offset: 0x00065E7C
			public override void Serialize(NetworkWriter writer)
			{
				base.Serialize(writer);
				writer.Write(this.attacker);
				writer.Write(this.inflictor);
				writer.Write(this.damage);
				writer.Write(this.isCrit);
				writer.Write(this.procChainMask);
				writer.Write(this.procCoefficient);
				writer.Write(this.damageColorIndex);
				writer.Write(this.damageType);
				writer.Write(this.forceVector);
				writer.Write(this.pushAwayForce);
				writer.WritePackedUInt32((uint)this.overlapInfoList.Count);
				foreach (OverlapAttack.OverlapInfo overlapInfo in this.overlapInfoList)
				{
					writer.Write(HurtBoxReference.FromHurtBox(overlapInfo.hurtBox));
					writer.Write(overlapInfo.hitPosition);
					writer.Write(overlapInfo.pushDirection);
				}
			}

			// Token: 0x060017DD RID: 6109 RVA: 0x00067D84 File Offset: 0x00065F84
			public override void Deserialize(NetworkReader reader)
			{
				base.Deserialize(reader);
				this.attacker = reader.ReadGameObject();
				this.inflictor = reader.ReadGameObject();
				this.damage = reader.ReadSingle();
				this.isCrit = reader.ReadBoolean();
				this.procChainMask = reader.ReadProcChainMask();
				this.procCoefficient = reader.ReadSingle();
				this.damageColorIndex = reader.ReadDamageColorIndex();
				this.damageType = reader.ReadDamageType();
				this.forceVector = reader.ReadVector3();
				this.pushAwayForce = reader.ReadSingle();
				this.overlapInfoList.Clear();
				int i = 0;
				int num = (int)reader.ReadPackedUInt32();
				while (i < num)
				{
					OverlapAttack.OverlapInfo item = default(OverlapAttack.OverlapInfo);
					GameObject gameObject = reader.ReadHurtBoxReference().ResolveGameObject();
					item.hurtBox = ((gameObject != null) ? gameObject.GetComponent<HurtBox>() : null);
					item.hitPosition = reader.ReadVector3();
					item.pushDirection = reader.ReadVector3();
					this.overlapInfoList.Add(item);
					i++;
				}
			}

			// Token: 0x0400168B RID: 5771
			public GameObject attacker;

			// Token: 0x0400168C RID: 5772
			public GameObject inflictor;

			// Token: 0x0400168D RID: 5773
			public float damage;

			// Token: 0x0400168E RID: 5774
			public bool isCrit;

			// Token: 0x0400168F RID: 5775
			public ProcChainMask procChainMask;

			// Token: 0x04001690 RID: 5776
			public float procCoefficient;

			// Token: 0x04001691 RID: 5777
			public DamageColorIndex damageColorIndex;

			// Token: 0x04001692 RID: 5778
			public DamageType damageType;

			// Token: 0x04001693 RID: 5779
			public Vector3 forceVector;

			// Token: 0x04001694 RID: 5780
			public float pushAwayForce;

			// Token: 0x04001695 RID: 5781
			public readonly List<OverlapAttack.OverlapInfo> overlapInfoList = new List<OverlapAttack.OverlapInfo>();
		}
	}
}
