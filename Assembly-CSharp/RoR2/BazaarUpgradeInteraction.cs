using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200015B RID: 347
	public sealed class BazaarUpgradeInteraction : NetworkBehaviour, IInteractable, IHologramContentProvider, IDisplayNameProvider
	{
		// Token: 0x0600064F RID: 1615 RVA: 0x0001A114 File Offset: 0x00018314
		private void Awake()
		{
			this.unlockableProgressionDefs = new UnlockableDef[this.unlockableProgression.Length];
			for (int i = 0; i < this.unlockableProgressionDefs.Length; i++)
			{
				this.unlockableProgressionDefs[i] = UnlockableCatalog.GetUnlockableDef(this.unlockableProgression[i]);
			}
		}

		// Token: 0x06000650 RID: 1616 RVA: 0x0001A15C File Offset: 0x0001835C
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

		// Token: 0x06000651 RID: 1617 RVA: 0x0000AC89 File Offset: 0x00008E89
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x0001A194 File Offset: 0x00018394
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

		// Token: 0x06000653 RID: 1619 RVA: 0x0001A220 File Offset: 0x00018420
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

		// Token: 0x06000654 RID: 1620 RVA: 0x0001A26B File Offset: 0x0001846B
		public string GetDisplayName()
		{
			return Language.GetString(this.displayNameToken);
		}

		// Token: 0x06000655 RID: 1621 RVA: 0x0001A278 File Offset: 0x00018478
		private string GetCostString()
		{
			return string.Format(" (<color=#{1}>{0}</color>)", this.cost, BazaarUpgradeInteraction.lunarCoinColorString);
		}

		// Token: 0x06000656 RID: 1622 RVA: 0x0001A294 File Offset: 0x00018494
		public string GetContextString(Interactor activator)
		{
			if (!this.CanBeAffordedByInteractor(activator))
			{
				return null;
			}
			return Language.GetString(this.contextToken) + this.GetCostString();
		}

		// Token: 0x06000657 RID: 1623 RVA: 0x0001A2B7 File Offset: 0x000184B7
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

		// Token: 0x06000658 RID: 1624 RVA: 0x0000409B File Offset: 0x0000229B
		public void OnInteractionBegin(Interactor activator)
		{
		}

		// Token: 0x06000659 RID: 1625 RVA: 0x0001A2DD File Offset: 0x000184DD
		private int GetCostForInteractor(Interactor activator)
		{
			return this.cost;
		}

		// Token: 0x0600065A RID: 1626 RVA: 0x0001A2E8 File Offset: 0x000184E8
		public bool CanBeAffordedByInteractor(Interactor activator)
		{
			NetworkUser networkUser = Util.LookUpBodyNetworkUser(activator.gameObject);
			return networkUser && (ulong)networkUser.lunarCoins >= (ulong)((long)this.GetCostForInteractor(activator));
		}

		// Token: 0x0600065B RID: 1627 RVA: 0x0001A31F File Offset: 0x0001851F
		public bool ShouldDisplayHologram(GameObject viewer)
		{
			return this.GetInteractorNextUnlockable(viewer) != null;
		}

		// Token: 0x0600065C RID: 1628 RVA: 0x0001A32B File Offset: 0x0001852B
		public GameObject GetHologramContentPrefab()
		{
			return Resources.Load<GameObject>("Prefabs/CostHologramContent");
		}

		// Token: 0x0600065D RID: 1629 RVA: 0x0001A338 File Offset: 0x00018538
		public void UpdateHologramContent(GameObject hologramContentObject)
		{
			CostHologramContent component = hologramContentObject.GetComponent<CostHologramContent>();
			if (component)
			{
				component.displayValue = this.cost;
				component.costType = CostTypeIndex.LunarCoin;
			}
		}

		// Token: 0x0600065E RID: 1630 RVA: 0x0001A367 File Offset: 0x00018567
		private void OnEnable()
		{
			InstanceTracker.Add<BazaarUpgradeInteraction>(this);
		}

		// Token: 0x0600065F RID: 1631 RVA: 0x0001A36F File Offset: 0x0001856F
		private void OnDisable()
		{
			InstanceTracker.Remove<BazaarUpgradeInteraction>(this);
		}

		// Token: 0x06000660 RID: 1632 RVA: 0x0001A377 File Offset: 0x00018577
		public bool ShouldShowOnScanner()
		{
			return this.available;
		}

		// Token: 0x06000663 RID: 1635 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000664 RID: 1636 RVA: 0x0001A3C8 File Offset: 0x000185C8
		// (set) Token: 0x06000665 RID: 1637 RVA: 0x0001A3DB File Offset: 0x000185DB
		public bool Networkavailable
		{
			get
			{
				return this.available;
			}
			[param: In]
			set
			{
				base.SetSyncVar<bool>(value, ref this.available, 1U);
			}
		}

		// Token: 0x06000666 RID: 1638 RVA: 0x0001A3F0 File Offset: 0x000185F0
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.available);
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
				writer.Write(this.available);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000667 RID: 1639 RVA: 0x0001A45C File Offset: 0x0001865C
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

		// Token: 0x040006B2 RID: 1714
		[SyncVar]
		public bool available = true;

		// Token: 0x040006B3 RID: 1715
		public string displayNameToken;

		// Token: 0x040006B4 RID: 1716
		public int cost;

		// Token: 0x040006B5 RID: 1717
		public string contextToken;

		// Token: 0x040006B6 RID: 1718
		public string[] unlockableProgression;

		// Token: 0x040006B7 RID: 1719
		private UnlockableDef[] unlockableProgressionDefs;

		// Token: 0x040006B8 RID: 1720
		public float activationCooldownDuration = 1f;

		// Token: 0x040006B9 RID: 1721
		private float activationTimer;

		// Token: 0x040006BA RID: 1722
		public GameObject purchaseEffect;

		// Token: 0x040006BB RID: 1723
		private static readonly Color32 lunarCoinColor = new Color32(198, 173, 250, byte.MaxValue);

		// Token: 0x040006BC RID: 1724
		private static readonly string lunarCoinColorString = Util.RGBToHex(BazaarUpgradeInteraction.lunarCoinColor);
	}
}
