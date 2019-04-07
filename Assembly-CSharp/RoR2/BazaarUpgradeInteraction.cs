using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000265 RID: 613
	public class BazaarUpgradeInteraction : NetworkBehaviour, IInteractable, IHologramContentProvider, IDisplayNameProvider
	{
		// Token: 0x06000B74 RID: 2932 RVA: 0x00038424 File Offset: 0x00036624
		private void Awake()
		{
			this.unlockableProgressionDefs = new UnlockableDef[this.unlockableProgression.Length];
			for (int i = 0; i < this.unlockableProgressionDefs.Length; i++)
			{
				this.unlockableProgressionDefs[i] = UnlockableCatalog.GetUnlockableDef(this.unlockableProgression[i]);
			}
		}

		// Token: 0x06000B75 RID: 2933 RVA: 0x0003846C File Offset: 0x0003666C
		private void FixedUpdate()
		{
			if (NetworkServer.active && !this.available)
			{
				this.activationTimer -= Time.fixedDeltaTime;
				if (this.activationTimer <= 0f)
				{
					this.Networkavailable = true;
				}
			}
		}

		// Token: 0x06000B76 RID: 2934 RVA: 0x0000A1ED File Offset: 0x000083ED
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x06000B77 RID: 2935 RVA: 0x000384A4 File Offset: 0x000366A4
		private UnlockableDef GetInteractorNextUnlockable(GameObject activatorGameObject)
		{
			NetworkUser networkUser = Util.LookUpBodyNetworkUser(activatorGameObject);
			if (networkUser)
			{
				LocalUser localUser = networkUser.localUser;
				if (localUser != null)
				{
					for (int i = 0; i < this.unlockableProgressionDefs.Length; i++)
					{
						UnlockableDef unlockableDef = this.unlockableProgressionDefs[i];
						if (!localUser.userProfile.HasUnlockable(unlockableDef))
						{
							return unlockableDef;
						}
					}
				}
				else
				{
					for (int j = 0; j < this.unlockableProgressionDefs.Length; j++)
					{
						UnlockableDef unlockableDef2 = this.unlockableProgressionDefs[j];
						if (!networkUser.unlockables.Contains(unlockableDef2))
						{
							return unlockableDef2;
						}
					}
				}
			}
			return null;
		}

		// Token: 0x06000B78 RID: 2936 RVA: 0x00038530 File Offset: 0x00036730
		private static bool ActivatorHasUnlockable(Interactor activator, string unlockableName)
		{
			NetworkUser networkUser = Util.LookUpBodyNetworkUser(activator.gameObject);
			if (!networkUser)
			{
				return true;
			}
			LocalUser localUser = networkUser.localUser;
			if (localUser != null)
			{
				return localUser.userProfile.HasUnlockable(unlockableName);
			}
			return networkUser.unlockables.Contains(UnlockableCatalog.GetUnlockableDef(unlockableName));
		}

		// Token: 0x06000B79 RID: 2937 RVA: 0x0003857B File Offset: 0x0003677B
		public string GetDisplayName()
		{
			return Language.GetString(this.displayNameToken);
		}

		// Token: 0x06000B7A RID: 2938 RVA: 0x00038588 File Offset: 0x00036788
		private string GetCostString()
		{
			return string.Format(" (<color=#{1}>{0}</color>)", this.cost, BazaarUpgradeInteraction.lunarCoinColorString);
		}

		// Token: 0x06000B7B RID: 2939 RVA: 0x000385A4 File Offset: 0x000367A4
		public string GetContextString(Interactor activator)
		{
			if (!this.CanBeAffordedByInteractor(activator))
			{
				return null;
			}
			return Language.GetString(this.contextToken) + this.GetCostString();
		}

		// Token: 0x06000B7C RID: 2940 RVA: 0x000385C7 File Offset: 0x000367C7
		public Interactability GetInteractability(Interactor activator)
		{
			if (this.GetInteractorNextUnlockable(activator.gameObject) == null || !this.available)
			{
				return Interactability.Disabled;
			}
			if (!this.CanBeAffordedByInteractor(activator))
			{
				return Interactability.ConditionsNotMet;
			}
			return Interactability.Available;
		}

		// Token: 0x06000B7D RID: 2941 RVA: 0x00004507 File Offset: 0x00002707
		public void OnInteractionBegin(Interactor activator)
		{
		}

		// Token: 0x06000B7E RID: 2942 RVA: 0x000385ED File Offset: 0x000367ED
		private int GetCostForInteractor(Interactor activator)
		{
			return this.cost;
		}

		// Token: 0x06000B7F RID: 2943 RVA: 0x000385F8 File Offset: 0x000367F8
		public bool CanBeAffordedByInteractor(Interactor activator)
		{
			NetworkUser networkUser = Util.LookUpBodyNetworkUser(activator.gameObject);
			return networkUser && (ulong)networkUser.lunarCoins >= (ulong)((long)this.GetCostForInteractor(activator));
		}

		// Token: 0x06000B80 RID: 2944 RVA: 0x0003862F File Offset: 0x0003682F
		public bool ShouldDisplayHologram(GameObject viewer)
		{
			return this.GetInteractorNextUnlockable(viewer) != null;
		}

		// Token: 0x06000B81 RID: 2945 RVA: 0x0003863B File Offset: 0x0003683B
		public GameObject GetHologramContentPrefab()
		{
			return Resources.Load<GameObject>("Prefabs/CostHologramContent");
		}

		// Token: 0x06000B82 RID: 2946 RVA: 0x00038648 File Offset: 0x00036848
		public void UpdateHologramContent(GameObject hologramContentObject)
		{
			CostHologramContent component = hologramContentObject.GetComponent<CostHologramContent>();
			if (component)
			{
				component.displayValue = this.cost;
				component.costType = CostType.Lunar;
			}
		}

		// Token: 0x06000B85 RID: 2949 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x06000B86 RID: 2950 RVA: 0x000386C0 File Offset: 0x000368C0
		// (set) Token: 0x06000B87 RID: 2951 RVA: 0x000386D3 File Offset: 0x000368D3
		public bool Networkavailable
		{
			get
			{
				return this.available;
			}
			set
			{
				base.SetSyncVar<bool>(value, ref this.available, 1u);
			}
		}

		// Token: 0x06000B88 RID: 2952 RVA: 0x000386E8 File Offset: 0x000368E8
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.available);
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
				writer.Write(this.available);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000B89 RID: 2953 RVA: 0x00038754 File Offset: 0x00036954
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.available = reader.ReadBoolean();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.available = reader.ReadBoolean();
			}
		}

		// Token: 0x04000F6A RID: 3946
		[SyncVar]
		public bool available = true;

		// Token: 0x04000F6B RID: 3947
		public string displayNameToken;

		// Token: 0x04000F6C RID: 3948
		public int cost;

		// Token: 0x04000F6D RID: 3949
		public string contextToken;

		// Token: 0x04000F6E RID: 3950
		public string[] unlockableProgression;

		// Token: 0x04000F6F RID: 3951
		private UnlockableDef[] unlockableProgressionDefs;

		// Token: 0x04000F70 RID: 3952
		public float activationCooldownDuration = 1f;

		// Token: 0x04000F71 RID: 3953
		private float activationTimer;

		// Token: 0x04000F72 RID: 3954
		public GameObject purchaseEffect;

		// Token: 0x04000F73 RID: 3955
		private static readonly Color32 lunarCoinColor = new Color32(198, 173, 250, byte.MaxValue);

		// Token: 0x04000F74 RID: 3956
		private static readonly string lunarCoinColorString = Util.RGBToHex(BazaarUpgradeInteraction.lunarCoinColor);
	}
}
