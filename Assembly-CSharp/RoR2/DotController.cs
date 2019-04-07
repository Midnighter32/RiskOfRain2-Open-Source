using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002D9 RID: 729
	public class DotController : NetworkBehaviour
	{
		// Token: 0x06000E94 RID: 3732 RVA: 0x00047B9F File Offset: 0x00045D9F
		private DotController.DotDef GetDotDef(DotController.DotIndex dotIndex)
		{
			if (dotIndex < DotController.DotIndex.Bleed || dotIndex >= DotController.DotIndex.Count)
			{
				return null;
			}
			return DotController.dotDefs[(int)dotIndex];
		}

		// Token: 0x06000E95 RID: 3733 RVA: 0x00047BB4 File Offset: 0x00045DB4
		private static void InitDotCatalog()
		{
			DotController.dotDefs = new DotController.DotDef[3];
			DotController.dotDefs[0] = new DotController.DotDef
			{
				interval = 0.25f,
				damageCoefficient = 0.2f,
				damageColorIndex = DamageColorIndex.Bleed
			};
			DotController.dotDefs[1] = new DotController.DotDef
			{
				interval = 0.5f,
				damageCoefficient = 0.25f,
				damageColorIndex = DamageColorIndex.Item
			};
			DotController.dotDefs[2] = new DotController.DotDef
			{
				interval = 0.2f,
				damageCoefficient = 1f,
				damageColorIndex = DamageColorIndex.Item
			};
		}

		// Token: 0x06000E96 RID: 3734 RVA: 0x00047C47 File Offset: 0x00045E47
		static DotController()
		{
			DotController.InitDotCatalog();
		}

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x06000E97 RID: 3735 RVA: 0x00047C74 File Offset: 0x00045E74
		// (set) Token: 0x06000E98 RID: 3736 RVA: 0x00047CC6 File Offset: 0x00045EC6
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

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x06000E99 RID: 3737 RVA: 0x00047CD9 File Offset: 0x00045ED9
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

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x06000E9A RID: 3738 RVA: 0x00047D0C File Offset: 0x00045F0C
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

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x06000E9B RID: 3739 RVA: 0x00047D1F File Offset: 0x00045F1F
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

		// Token: 0x06000E9C RID: 3740 RVA: 0x00047D40 File Offset: 0x00045F40
		private void Awake()
		{
			if (NetworkServer.active)
			{
				this.dotStackList = new List<DotController.DotStack>();
				this.dotTimers = new float[3];
			}
			DotController.instancesList.Add(this);
		}

		// Token: 0x06000E9D RID: 3741 RVA: 0x00047D6B File Offset: 0x00045F6B
		private void OnDestroy()
		{
			DotController.instancesList.Remove(this);
			if (this.recordedVictimInstanceId != -1)
			{
				DotController.dotControllerLocator.Remove(this.recordedVictimInstanceId);
			}
		}

		// Token: 0x06000E9E RID: 3742 RVA: 0x00047D94 File Offset: 0x00045F94
		private void FixedUpdate()
		{
			GameObject victimObject = this.victimObject;
			if (victimObject)
			{
				if (base.transform.parent != victimObject.transform)
				{
					base.transform.SetParent(victimObject.transform, false);
					base.transform.localPosition = Vector3.zero;
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
				if ((this.activeDotFlags & 2) != 0)
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
							return;
						}
					}
				}
				else if (this.helfireEffectController)
				{
					UnityEngine.Object.Destroy(this.helfireEffectController);
					this.helfireEffectController = null;
				}
				return;
			}
			if (NetworkServer.active)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06000E9F RID: 3743 RVA: 0x00048010 File Offset: 0x00046210
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

		// Token: 0x06000EA0 RID: 3744 RVA: 0x00048078 File Offset: 0x00046278
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
						this.dotStackList.RemoveAt(i);
					}
					else
					{
						remainingActive++;
					}
				}
			}
			if (this.victimObject && this.victimHealthComponent)
			{
				for (int j = 0; j < list.Count; j++)
				{
					DamageInfo damageInfo = new DamageInfo();
					damageInfo.attacker = list[j].attackerObject;
					damageInfo.crit = false;
					damageInfo.damage = list[j].totalDamage;
					damageInfo.force = Vector3.zero;
					damageInfo.inflictor = base.gameObject;
					damageInfo.position = Util.GetCorePosition(this.victimObject);
					damageInfo.procCoefficient = 0f;
					damageInfo.damageColorIndex = dotDef.damageColorIndex;
					damageInfo.damageType = list[j].damageType;
					this.victimHealthComponent.TakeDamage(damageInfo);
				}
			}
		}

		// Token: 0x06000EA1 RID: 3745 RVA: 0x000481E4 File Offset: 0x000463E4
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
			if (dotIndex == DotController.DotIndex.Helfire)
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
					EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/HelfireIgniteEffect"), new EffectData
					{
						origin = this.victimBody.corePosition
					}, true);
				}
			}
			this.dotStackList.Add(dotStack);
		}

		// Token: 0x06000EA2 RID: 3746 RVA: 0x000483F4 File Offset: 0x000465F4
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

		// Token: 0x06000EA4 RID: 3748 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x06000EA5 RID: 3749 RVA: 0x00048494 File Offset: 0x00046694
		// (set) Token: 0x06000EA6 RID: 3750 RVA: 0x000484A7 File Offset: 0x000466A7
		public NetworkInstanceId NetworkvictimObjectId
		{
			get
			{
				return this.victimObjectId;
			}
			set
			{
				base.SetSyncVar<NetworkInstanceId>(value, ref this.victimObjectId, 1u);
			}
		}

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x06000EA7 RID: 3751 RVA: 0x000484BC File Offset: 0x000466BC
		// (set) Token: 0x06000EA8 RID: 3752 RVA: 0x000484CF File Offset: 0x000466CF
		public byte NetworkactiveDotFlags
		{
			get
			{
				return this.activeDotFlags;
			}
			set
			{
				base.SetSyncVar<byte>(value, ref this.activeDotFlags, 2u);
			}
		}

		// Token: 0x06000EA9 RID: 3753 RVA: 0x000484E4 File Offset: 0x000466E4
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.victimObjectId);
				writer.WritePackedUInt32((uint)this.activeDotFlags);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.victimObjectId);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
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

		// Token: 0x06000EAA RID: 3754 RVA: 0x00048590 File Offset: 0x00046790
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

		// Token: 0x040012A5 RID: 4773
		private static DotController.DotDef[] dotDefs;

		// Token: 0x040012A6 RID: 4774
		private static readonly Dictionary<int, DotController> dotControllerLocator = new Dictionary<int, DotController>();

		// Token: 0x040012A7 RID: 4775
		private static readonly List<DotController> instancesList = new List<DotController>();

		// Token: 0x040012A8 RID: 4776
		public static readonly ReadOnlyCollection<DotController> readOnlyInstancesList = DotController.instancesList.AsReadOnly();

		// Token: 0x040012A9 RID: 4777
		[SyncVar]
		private NetworkInstanceId victimObjectId;

		// Token: 0x040012AA RID: 4778
		private GameObject _victimObject;

		// Token: 0x040012AB RID: 4779
		private CharacterBody _victimBody;

		// Token: 0x040012AC RID: 4780
		private BurnEffectController burnEffectController;

		// Token: 0x040012AD RID: 4781
		private BurnEffectController helfireEffectController;

		// Token: 0x040012AE RID: 4782
		private GameObject bleedEffect;

		// Token: 0x040012AF RID: 4783
		[SyncVar]
		private byte activeDotFlags;

		// Token: 0x040012B0 RID: 4784
		private List<DotController.DotStack> dotStackList;

		// Token: 0x040012B1 RID: 4785
		private float[] dotTimers;

		// Token: 0x040012B2 RID: 4786
		private int recordedVictimInstanceId = -1;

		// Token: 0x020002DA RID: 730
		public enum DotIndex
		{
			// Token: 0x040012B4 RID: 4788
			Bleed,
			// Token: 0x040012B5 RID: 4789
			Burn,
			// Token: 0x040012B6 RID: 4790
			Helfire,
			// Token: 0x040012B7 RID: 4791
			Count
		}

		// Token: 0x020002DB RID: 731
		private class DotDef
		{
			// Token: 0x040012B8 RID: 4792
			public float interval;

			// Token: 0x040012B9 RID: 4793
			public float damageCoefficient;

			// Token: 0x040012BA RID: 4794
			public DamageColorIndex damageColorIndex;
		}

		// Token: 0x020002DC RID: 732
		private class DotStack
		{
			// Token: 0x040012BB RID: 4795
			public DotController.DotIndex dotIndex;

			// Token: 0x040012BC RID: 4796
			public DotController.DotDef dotDef;

			// Token: 0x040012BD RID: 4797
			public GameObject attackerObject;

			// Token: 0x040012BE RID: 4798
			public TeamIndex attackerTeam;

			// Token: 0x040012BF RID: 4799
			public float timer;

			// Token: 0x040012C0 RID: 4800
			public float damage;

			// Token: 0x040012C1 RID: 4801
			public DamageType damageType;
		}

		// Token: 0x020002DD RID: 733
		private class PendingDamage
		{
			// Token: 0x040012C2 RID: 4802
			public GameObject attackerObject;

			// Token: 0x040012C3 RID: 4803
			public float totalDamage;

			// Token: 0x040012C4 RID: 4804
			public DamageType damageType;
		}
	}
}
