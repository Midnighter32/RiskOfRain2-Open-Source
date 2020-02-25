using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using RoR2.Stats;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020001EB RID: 491
	public class DotController : NetworkBehaviour
	{
		// Token: 0x06000A40 RID: 2624 RVA: 0x0002CAD3 File Offset: 0x0002ACD3
		private DotController.DotDef GetDotDef(DotController.DotIndex dotIndex)
		{
			if (dotIndex < DotController.DotIndex.Bleed || dotIndex >= DotController.DotIndex.Count)
			{
				return null;
			}
			return DotController.dotDefs[(int)dotIndex];
		}

		// Token: 0x06000A41 RID: 2625 RVA: 0x0002CAE8 File Offset: 0x0002ACE8
		private static void InitDotCatalog()
		{
			DotController.dotDefs = new DotController.DotDef[5];
			DotController.dotDefs[0] = new DotController.DotDef
			{
				interval = 0.25f,
				damageCoefficient = 0.2f,
				damageColorIndex = DamageColorIndex.Bleed,
				associatedBuff = BuffIndex.Bleeding
			};
			DotController.dotDefs[1] = new DotController.DotDef
			{
				interval = 0.5f,
				damageCoefficient = 0.25f,
				damageColorIndex = DamageColorIndex.Item,
				associatedBuff = BuffIndex.OnFire
			};
			DotController.dotDefs[3] = new DotController.DotDef
			{
				interval = 0.2f,
				damageCoefficient = 0.1f,
				damageColorIndex = DamageColorIndex.Item,
				associatedBuff = BuffIndex.OnFire
			};
			DotController.dotDefs[2] = new DotController.DotDef
			{
				interval = 0.2f,
				damageCoefficient = 1f,
				damageColorIndex = DamageColorIndex.Item
			};
			DotController.dotDefs[4] = new DotController.DotDef
			{
				interval = 0.333f,
				damageCoefficient = 0.333f,
				damageColorIndex = DamageColorIndex.Poison,
				associatedBuff = BuffIndex.Poisoned
			};
		}

		// Token: 0x06000A42 RID: 2626 RVA: 0x0002CBEB File Offset: 0x0002ADEB
		static DotController()
		{
			DotController.InitDotCatalog();
		}

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x06000A43 RID: 2627 RVA: 0x0002CC18 File Offset: 0x0002AE18
		// (set) Token: 0x06000A44 RID: 2628 RVA: 0x0002CC6A File Offset: 0x0002AE6A
		public GameObject victimObject
		{
			get
			{
				if (!this._victimObject)
				{
					if (NetworkServer.active)
					{
						this._victimObject = NetworkServer.FindLocalObject(this.victimObjectId);
					}
					else if (NetworkClient.active)
					{
						this._victimObject = ClientScene.FindLocalObject(this.victimObjectId);
					}
				}
				return this._victimObject;
			}
			set
			{
				this.NetworkvictimObjectId = value.GetComponent<NetworkIdentity>().netId;
			}
		}

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x06000A45 RID: 2629 RVA: 0x0002CC7D File Offset: 0x0002AE7D
		private CharacterBody victimBody
		{
			get
			{
				if (!this._victimBody && this.victimObject)
				{
					this._victimBody = this.victimObject.GetComponent<CharacterBody>();
				}
				return this._victimBody;
			}
		}

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x06000A46 RID: 2630 RVA: 0x0002CCB0 File Offset: 0x0002AEB0
		private HealthComponent victimHealthComponent
		{
			get
			{
				CharacterBody victimBody = this.victimBody;
				if (victimBody == null)
				{
					return null;
				}
				return victimBody.healthComponent;
			}
		}

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x06000A47 RID: 2631 RVA: 0x0002CCC3 File Offset: 0x0002AEC3
		private TeamIndex victimTeam
		{
			get
			{
				if (!this.victimBody)
				{
					return TeamIndex.None;
				}
				return this.victimBody.teamComponent.teamIndex;
			}
		}

		// Token: 0x06000A48 RID: 2632 RVA: 0x0002CCE4 File Offset: 0x0002AEE4
		public bool HasDotActive(DotController.DotIndex dotIndex)
		{
			return ((int)this.activeDotFlags & 1 << (int)dotIndex) != 0;
		}

		// Token: 0x06000A49 RID: 2633 RVA: 0x0002CCF6 File Offset: 0x0002AEF6
		private void Awake()
		{
			if (NetworkServer.active)
			{
				this.dotStackList = new List<DotController.DotStack>();
				this.dotTimers = new float[5];
			}
			DotController.instancesList.Add(this);
		}

		// Token: 0x06000A4A RID: 2634 RVA: 0x0002CD24 File Offset: 0x0002AF24
		private void OnDestroy()
		{
			if (NetworkServer.active)
			{
				for (int i = this.dotStackList.Count - 1; i >= 0; i--)
				{
					this.RemoveDotStackAtServer(i);
				}
			}
			DotController.instancesList.Remove(this);
			if (this.recordedVictimInstanceId != -1)
			{
				DotController.dotControllerLocator.Remove(this.recordedVictimInstanceId);
			}
		}

		// Token: 0x06000A4B RID: 2635 RVA: 0x0002CD80 File Offset: 0x0002AF80
		private void FixedUpdate()
		{
			GameObject victimObject = this.victimObject;
			if (!victimObject)
			{
				if (NetworkServer.active)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
				return;
			}
			if (NetworkServer.active)
			{
				for (DotController.DotIndex dotIndex = DotController.DotIndex.Bleed; dotIndex < DotController.DotIndex.Count; dotIndex++)
				{
					DotController.DotDef dotDef = this.GetDotDef(dotIndex);
					float num = this.dotTimers[(int)dotIndex] - Time.fixedDeltaTime;
					if (num <= 0f)
					{
						num += dotDef.interval;
						int num2 = 0;
						this.EvaluateDotStacksForType(dotIndex, dotDef.interval, out num2);
						byte b = (byte)(1 << (int)dotIndex);
						this.NetworkactiveDotFlags = (this.activeDotFlags & ~b);
						if (num2 != 0)
						{
							this.NetworkactiveDotFlags = (this.activeDotFlags | b);
						}
					}
					this.dotTimers[(int)dotIndex] = num;
				}
				if (this.dotStackList.Count == 0)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
			if ((this.activeDotFlags & 1) != 0)
			{
				if (!this.bleedEffect)
				{
					this.bleedEffect = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/BleedEffect"), base.transform);
				}
			}
			else if (this.bleedEffect)
			{
				UnityEngine.Object.Destroy(this.bleedEffect);
				this.bleedEffect = null;
			}
			if ((this.activeDotFlags & 2) != 0 || (this.activeDotFlags & 8) != 0)
			{
				if (!this.burnEffectController)
				{
					ModelLocator component = victimObject.GetComponent<ModelLocator>();
					if (component && component.modelTransform)
					{
						this.burnEffectController = base.gameObject.AddComponent<BurnEffectController>();
						this.burnEffectController.effectType = BurnEffectController.normalEffect;
						this.burnEffectController.target = component.modelTransform.gameObject;
					}
				}
			}
			else if (this.burnEffectController)
			{
				UnityEngine.Object.Destroy(this.burnEffectController);
				this.burnEffectController = null;
			}
			if ((this.activeDotFlags & 4) != 0)
			{
				if (!this.helfireEffectController)
				{
					ModelLocator component2 = victimObject.GetComponent<ModelLocator>();
					if (component2 && component2.modelTransform)
					{
						this.helfireEffectController = base.gameObject.AddComponent<BurnEffectController>();
						this.helfireEffectController.effectType = BurnEffectController.helfireEffect;
						this.helfireEffectController.target = component2.modelTransform.gameObject;
					}
				}
			}
			else if (this.helfireEffectController)
			{
				UnityEngine.Object.Destroy(this.helfireEffectController);
				this.helfireEffectController = null;
			}
			if ((this.activeDotFlags & 16) != 0)
			{
				if (!this.poisonEffect)
				{
					this.poisonEffect = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/PoisonEffect"), base.transform);
					return;
				}
			}
			else if (this.poisonEffect)
			{
				UnityEngine.Object.Destroy(this.poisonEffect);
				this.poisonEffect = null;
			}
		}

		// Token: 0x06000A4C RID: 2636 RVA: 0x0002D01D File Offset: 0x0002B21D
		private void LateUpdate()
		{
			if (this.victimObject)
			{
				base.transform.position = this.victimObject.transform.position;
			}
		}

		// Token: 0x06000A4D RID: 2637 RVA: 0x0002D048 File Offset: 0x0002B248
		private static void AddPendingDamageEntry(List<DotController.PendingDamage> pendingDamages, GameObject attackerObject, float damage, DamageType damageType)
		{
			for (int i = 0; i < pendingDamages.Count; i++)
			{
				if (pendingDamages[i].attackerObject == attackerObject)
				{
					pendingDamages[i].totalDamage += damage;
					return;
				}
			}
			pendingDamages.Add(new DotController.PendingDamage
			{
				attackerObject = attackerObject,
				totalDamage = damage,
				damageType = damageType
			});
		}

		// Token: 0x06000A4E RID: 2638 RVA: 0x0002D0B0 File Offset: 0x0002B2B0
		private void OnDotStackAddedServer(DotController.DotStack dotStack)
		{
			DotController.DotDef dotDef = dotStack.dotDef;
			if (dotDef.associatedBuff != BuffIndex.None && this.victimBody)
			{
				this.victimBody.AddBuff(dotDef.associatedBuff);
			}
		}

		// Token: 0x06000A4F RID: 2639 RVA: 0x0002D0EC File Offset: 0x0002B2EC
		private void OnDotStackRemovedServer(DotController.DotStack dotStack)
		{
			DotController.DotDef dotDef = dotStack.dotDef;
			if (dotDef.associatedBuff != BuffIndex.None && this.victimBody)
			{
				this.victimBody.RemoveBuff(dotDef.associatedBuff);
			}
		}

		// Token: 0x06000A50 RID: 2640 RVA: 0x0002D128 File Offset: 0x0002B328
		private void RemoveDotStackAtServer(int i)
		{
			DotController.DotStack dotStack = this.dotStackList[i];
			this.dotStackList.RemoveAt(i);
			this.OnDotStackRemovedServer(dotStack);
		}

		// Token: 0x06000A51 RID: 2641 RVA: 0x0002D158 File Offset: 0x0002B358
		private void EvaluateDotStacksForType(DotController.DotIndex dotIndex, float dt, out int remainingActive)
		{
			List<DotController.PendingDamage> list = new List<DotController.PendingDamage>();
			remainingActive = 0;
			DotController.DotDef dotDef = this.GetDotDef(dotIndex);
			for (int i = this.dotStackList.Count - 1; i >= 0; i--)
			{
				DotController.DotStack dotStack = this.dotStackList[i];
				if (dotStack.dotIndex == dotIndex)
				{
					dotStack.timer -= dt;
					DotController.AddPendingDamageEntry(list, dotStack.attackerObject, dotStack.damage, dotStack.damageType);
					if (dotStack.timer <= 0f)
					{
						this.RemoveDotStackAtServer(i);
					}
					else
					{
						remainingActive++;
					}
				}
			}
			if (this.victimObject && this.victimHealthComponent)
			{
				Vector3 corePosition = this.victimBody.corePosition;
				for (int j = 0; j < list.Count; j++)
				{
					DamageInfo damageInfo = new DamageInfo();
					damageInfo.attacker = list[j].attackerObject;
					damageInfo.crit = false;
					damageInfo.damage = list[j].totalDamage;
					damageInfo.force = Vector3.zero;
					damageInfo.inflictor = base.gameObject;
					damageInfo.position = corePosition;
					damageInfo.procCoefficient = 0f;
					damageInfo.damageColorIndex = dotDef.damageColorIndex;
					damageInfo.damageType = list[j].damageType;
					damageInfo.dotIndex = dotIndex;
					this.victimHealthComponent.TakeDamage(damageInfo);
				}
			}
		}

		// Token: 0x06000A52 RID: 2642 RVA: 0x0002D2CC File Offset: 0x0002B4CC
		[Server]
		private void AddDot(GameObject attackerObject, float duration, DotController.DotIndex dotIndex, float damageMultiplier)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.DotController::AddDot(UnityEngine.GameObject,System.Single,RoR2.DotController/DotIndex,System.Single)' called on client");
				return;
			}
			if (dotIndex < DotController.DotIndex.Bleed || dotIndex >= DotController.DotIndex.Count)
			{
				return;
			}
			TeamIndex teamIndex = TeamIndex.Neutral;
			float num = 0f;
			TeamComponent component = attackerObject.GetComponent<TeamComponent>();
			if (component)
			{
				teamIndex = component.teamIndex;
			}
			CharacterBody component2 = attackerObject.GetComponent<CharacterBody>();
			if (component2)
			{
				num = component2.damage;
			}
			DotController.DotDef dotDef = DotController.dotDefs[(int)dotIndex];
			DotController.DotStack dotStack = new DotController.DotStack
			{
				dotIndex = dotIndex,
				dotDef = dotDef,
				attackerObject = attackerObject,
				attackerTeam = teamIndex,
				timer = duration,
				damage = dotDef.damageCoefficient * num * damageMultiplier,
				damageType = DamageType.Generic
			};
			switch (dotIndex)
			{
			case DotController.DotIndex.Helfire:
			{
				if (!component2)
				{
					return;
				}
				HealthComponent healthComponent = component2.healthComponent;
				if (!healthComponent)
				{
					return;
				}
				dotStack.damage = healthComponent.fullHealth * 0.01f * damageMultiplier;
				if (this.victimObject == attackerObject)
				{
					dotStack.damageType |= (DamageType.NonLethal | DamageType.Silent);
				}
				else if (this.victimTeam == teamIndex)
				{
					dotStack.damage *= 0.5f;
				}
				else
				{
					dotStack.damage *= 24f;
				}
				int i = 0;
				int count = this.dotStackList.Count;
				while (i < count)
				{
					if (this.dotStackList[i].dotIndex == DotController.DotIndex.Helfire && this.dotStackList[i].attackerObject == attackerObject)
					{
						this.dotStackList[i].timer = Mathf.Max(this.dotStackList[i].timer, duration);
						this.dotStackList[i].damage = dotStack.damage;
						return;
					}
					i++;
				}
				if (this.victimBody)
				{
					EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/HelfireIgniteEffect"), new EffectData
					{
						origin = this.victimBody.corePosition
					}, true);
				}
				break;
			}
			case DotController.DotIndex.PercentBurn:
				dotStack.damage = Mathf.Min(dotStack.damage, this.victimBody.healthComponent.fullCombinedHealth * 0.01f);
				break;
			case DotController.DotIndex.Poison:
			{
				float a = this.victimHealthComponent.fullCombinedHealth / 100f * 1f * dotDef.interval;
				dotStack.damage = Mathf.Min(Mathf.Max(a, dotStack.damage), dotStack.damage * 50f);
				dotStack.damageType = DamageType.NonLethal;
				int j = 0;
				int count2 = this.dotStackList.Count;
				while (j < count2)
				{
					if (this.dotStackList[j].dotIndex == DotController.DotIndex.Poison)
					{
						this.dotStackList[j].timer = Mathf.Max(this.dotStackList[j].timer, duration);
						this.dotStackList[j].damage = dotStack.damage;
						return;
					}
					j++;
				}
				bool flag = false;
				for (int k = 0; k < this.dotStackList.Count; k++)
				{
					if (this.dotStackList[k].dotIndex == DotController.DotIndex.Poison)
					{
						flag = true;
						break;
					}
				}
				if (!flag && component2 != null)
				{
					CharacterMaster master = component2.master;
					if (master != null)
					{
						PlayerStatsComponent playerStatsComponent = master.playerStatsComponent;
						if (playerStatsComponent != null)
						{
							playerStatsComponent.currentStats.PushStatValue(StatDef.totalCrocoInfectionsInflicted, 1UL);
						}
					}
				}
				break;
			}
			}
			this.dotStackList.Add(dotStack);
			this.OnDotStackAddedServer(dotStack);
		}

		// Token: 0x06000A53 RID: 2643 RVA: 0x0002D654 File Offset: 0x0002B854
		[Server]
		public static void InflictDot(GameObject victimObject, GameObject attackerObject, DotController.DotIndex dotIndex, float duration = 8f, float damageMultiplier = 1f)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.DotController::InflictDot(UnityEngine.GameObject,UnityEngine.GameObject,RoR2.DotController/DotIndex,System.Single,System.Single)' called on client");
				return;
			}
			if (victimObject && attackerObject)
			{
				DotController component;
				if (!DotController.dotControllerLocator.TryGetValue(victimObject.GetInstanceID(), out component))
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/DotController"));
					component = gameObject.GetComponent<DotController>();
					component.victimObject = victimObject;
					component.recordedVictimInstanceId = victimObject.GetInstanceID();
					DotController.dotControllerLocator.Add(component.recordedVictimInstanceId, component);
					NetworkServer.Spawn(gameObject);
				}
				component.AddDot(attackerObject, duration, dotIndex, damageMultiplier);
			}
		}

		// Token: 0x06000A54 RID: 2644 RVA: 0x0002D6E4 File Offset: 0x0002B8E4
		[Server]
		public static void RemoveAllDots(GameObject victimObject)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.DotController::RemoveAllDots(UnityEngine.GameObject)' called on client");
				return;
			}
			DotController dotController;
			if (DotController.dotControllerLocator.TryGetValue(victimObject.GetInstanceID(), out dotController))
			{
				UnityEngine.Object.Destroy(dotController.gameObject);
			}
		}

		// Token: 0x06000A55 RID: 2645 RVA: 0x0002D728 File Offset: 0x0002B928
		[Server]
		public static DotController FindDotController(GameObject victimObject)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'RoR2.DotController RoR2.DotController::FindDotController(UnityEngine.GameObject)' called on client");
				return null;
			}
			int i = 0;
			int count = DotController.instancesList.Count;
			while (i < count)
			{
				if (victimObject == DotController.instancesList[i]._victimObject)
				{
					return DotController.instancesList[i];
				}
				i++;
			}
			return null;
		}

		// Token: 0x06000A57 RID: 2647 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x06000A58 RID: 2648 RVA: 0x0002D79C File Offset: 0x0002B99C
		// (set) Token: 0x06000A59 RID: 2649 RVA: 0x0002D7AF File Offset: 0x0002B9AF
		public NetworkInstanceId NetworkvictimObjectId
		{
			get
			{
				return this.victimObjectId;
			}
			[param: In]
			set
			{
				base.SetSyncVar<NetworkInstanceId>(value, ref this.victimObjectId, 1U);
			}
		}

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x06000A5A RID: 2650 RVA: 0x0002D7C4 File Offset: 0x0002B9C4
		// (set) Token: 0x06000A5B RID: 2651 RVA: 0x0002D7D7 File Offset: 0x0002B9D7
		public byte NetworkactiveDotFlags
		{
			get
			{
				return this.activeDotFlags;
			}
			[param: In]
			set
			{
				base.SetSyncVar<byte>(value, ref this.activeDotFlags, 2U);
			}
		}

		// Token: 0x06000A5C RID: 2652 RVA: 0x0002D7EC File Offset: 0x0002B9EC
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.victimObjectId);
				writer.WritePackedUInt32((uint)this.activeDotFlags);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.victimObjectId);
			}
			if ((base.syncVarDirtyBits & 2U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32((uint)this.activeDotFlags);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000A5D RID: 2653 RVA: 0x0002D898 File Offset: 0x0002BA98
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.victimObjectId = reader.ReadNetworkId();
				this.activeDotFlags = (byte)reader.ReadPackedUInt32();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.victimObjectId = reader.ReadNetworkId();
			}
			if ((num & 2) != 0)
			{
				this.activeDotFlags = (byte)reader.ReadPackedUInt32();
			}
		}

		// Token: 0x04000A9A RID: 2714
		private static DotController.DotDef[] dotDefs;

		// Token: 0x04000A9B RID: 2715
		private static readonly Dictionary<int, DotController> dotControllerLocator = new Dictionary<int, DotController>();

		// Token: 0x04000A9C RID: 2716
		private static readonly List<DotController> instancesList = new List<DotController>();

		// Token: 0x04000A9D RID: 2717
		public static readonly ReadOnlyCollection<DotController> readOnlyInstancesList = DotController.instancesList.AsReadOnly();

		// Token: 0x04000A9E RID: 2718
		[SyncVar]
		private NetworkInstanceId victimObjectId;

		// Token: 0x04000A9F RID: 2719
		private GameObject _victimObject;

		// Token: 0x04000AA0 RID: 2720
		private CharacterBody _victimBody;

		// Token: 0x04000AA1 RID: 2721
		private BurnEffectController burnEffectController;

		// Token: 0x04000AA2 RID: 2722
		private BurnEffectController helfireEffectController;

		// Token: 0x04000AA3 RID: 2723
		private GameObject bleedEffect;

		// Token: 0x04000AA4 RID: 2724
		private GameObject poisonEffect;

		// Token: 0x04000AA5 RID: 2725
		[SyncVar]
		private byte activeDotFlags;

		// Token: 0x04000AA6 RID: 2726
		private List<DotController.DotStack> dotStackList;

		// Token: 0x04000AA7 RID: 2727
		private float[] dotTimers;

		// Token: 0x04000AA8 RID: 2728
		private int recordedVictimInstanceId = -1;

		// Token: 0x020001EC RID: 492
		public enum DotIndex
		{
			// Token: 0x04000AAA RID: 2730
			None = -1,
			// Token: 0x04000AAB RID: 2731
			Bleed,
			// Token: 0x04000AAC RID: 2732
			Burn,
			// Token: 0x04000AAD RID: 2733
			Helfire,
			// Token: 0x04000AAE RID: 2734
			PercentBurn,
			// Token: 0x04000AAF RID: 2735
			Poison,
			// Token: 0x04000AB0 RID: 2736
			Count
		}

		// Token: 0x020001ED RID: 493
		private class DotDef
		{
			// Token: 0x04000AB1 RID: 2737
			public float interval;

			// Token: 0x04000AB2 RID: 2738
			public float damageCoefficient;

			// Token: 0x04000AB3 RID: 2739
			public DamageColorIndex damageColorIndex;

			// Token: 0x04000AB4 RID: 2740
			public BuffIndex associatedBuff = BuffIndex.None;
		}

		// Token: 0x020001EE RID: 494
		private class DotStack
		{
			// Token: 0x04000AB5 RID: 2741
			public DotController.DotIndex dotIndex;

			// Token: 0x04000AB6 RID: 2742
			public DotController.DotDef dotDef;

			// Token: 0x04000AB7 RID: 2743
			public GameObject attackerObject;

			// Token: 0x04000AB8 RID: 2744
			public TeamIndex attackerTeam;

			// Token: 0x04000AB9 RID: 2745
			public float timer;

			// Token: 0x04000ABA RID: 2746
			public float damage;

			// Token: 0x04000ABB RID: 2747
			public DamageType damageType;
		}

		// Token: 0x020001EF RID: 495
		private class PendingDamage
		{
			// Token: 0x04000ABC RID: 2748
			public GameObject attackerObject;

			// Token: 0x04000ABD RID: 2749
			public float totalDamage;

			// Token: 0x04000ABE RID: 2750
			public DamageType damageType;
		}
	}
}
