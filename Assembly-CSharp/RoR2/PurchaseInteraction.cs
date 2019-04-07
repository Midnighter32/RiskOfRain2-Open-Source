using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RoR2.Stats;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200039D RID: 925
	[RequireComponent(typeof(Highlight))]
	public class PurchaseInteraction : NetworkBehaviour, IInteractable, IHologramContentProvider, IDisplayNameProvider
	{
		// Token: 0x06001381 RID: 4993 RVA: 0x0005F3DC File Offset: 0x0005D5DC
		private void Awake()
		{
			if (this.automaticallyScaleCostWithDifficulty)
			{
				this.Networkcost = Run.instance.GetDifficultyScaledCost(this.cost);
			}
			if (NetworkServer.active)
			{
				this.rng = new Xoroshiro128Plus(Run.instance.treasureRng.nextUlong);
			}
		}

		// Token: 0x06001382 RID: 4994 RVA: 0x0005F428 File Offset: 0x0005D628
		[Server]
		public void SetAvailable(bool newAvailable)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.PurchaseInteraction::SetAvailable(System.Boolean)' called on client");
				return;
			}
			this.Networkavailable = newAvailable;
		}

		// Token: 0x06001383 RID: 4995 RVA: 0x0005F446 File Offset: 0x0005D646
		[Server]
		public void SetUnavailableTemporarily(float time)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.PurchaseInteraction::SetUnavailableTemporarily(System.Single)' called on client");
				return;
			}
			this.Networkavailable = false;
			base.Invoke("SetAvailableTrue", time);
		}

		// Token: 0x06001384 RID: 4996 RVA: 0x0005F470 File Offset: 0x0005D670
		private void SetAvailableTrue()
		{
			this.Networkavailable = true;
		}

		// Token: 0x06001385 RID: 4997 RVA: 0x0005F479 File Offset: 0x0005D679
		public string GetDisplayName()
		{
			return Language.GetString(this.displayNameToken);
		}

		// Token: 0x06001386 RID: 4998 RVA: 0x0005F488 File Offset: 0x0005D688
		private string GetCostString()
		{
			switch (this.costType)
			{
			case CostType.None:
				return "";
			case CostType.Money:
				return string.Format(" (<nobr><style=cShrine>${0}</style></nobr>)", this.cost);
			case CostType.PercentHealth:
				return string.Format(" (<nobr><style=cDeath>{0}% HP</style></nobr>)", this.cost);
			case CostType.Lunar:
				return string.Format(" (<nobr><color=#{1}>{0}</color></nobr>)", this.cost, ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.LunarCoin));
			case CostType.WhiteItem:
				return string.Format(" <nobr>(<nobr><color=#{1}>{0} Items</color></nobr>)", this.cost, ColorCatalog.GetColorHexString(PurchaseInteraction.CostTypeToColorIndex(this.costType)));
			case CostType.GreenItem:
				return string.Format(" <nobr>(<nobr><color=#{1}>{0} Items</color></nobr>)", this.cost, ColorCatalog.GetColorHexString(PurchaseInteraction.CostTypeToColorIndex(this.costType)));
			case CostType.RedItem:
				return string.Format(" <nobr>(<nobr><color=#{1}>{0} Items</color></nobr>)", this.cost, ColorCatalog.GetColorHexString(PurchaseInteraction.CostTypeToColorIndex(this.costType)));
			default:
				return "";
			}
		}

		// Token: 0x06001387 RID: 4999 RVA: 0x0005F58C File Offset: 0x0005D78C
		private static bool ActivatorHasUnlockable(Interactor activator, string unlockableName)
		{
			NetworkUser networkUser = Util.LookUpBodyNetworkUser(activator.gameObject);
			if (networkUser)
			{
				LocalUser localUser = networkUser.localUser;
				if (localUser != null)
				{
					return localUser.userProfile.HasUnlockable(unlockableName);
				}
			}
			return true;
		}

		// Token: 0x06001388 RID: 5000 RVA: 0x0005F5C5 File Offset: 0x0005D7C5
		public string GetContextString(Interactor activator)
		{
			return Language.GetString(this.contextToken) + this.GetCostString();
		}

		// Token: 0x06001389 RID: 5001 RVA: 0x0005F5E0 File Offset: 0x0005D7E0
		public Interactability GetInteractability(Interactor activator)
		{
			if (!string.IsNullOrEmpty(this.requiredUnlockable) && !PurchaseInteraction.ActivatorHasUnlockable(activator, this.requiredUnlockable))
			{
				return Interactability.Disabled;
			}
			if (!this.available || this.lockGameObject)
			{
				return Interactability.Disabled;
			}
			if (!this.CanBeAffordedByInteractor(activator))
			{
				return Interactability.ConditionsNotMet;
			}
			return Interactability.Available;
		}

		// Token: 0x0600138A RID: 5002 RVA: 0x0005F62D File Offset: 0x0005D82D
		public static ItemTier CostTypeToItemTier(CostType costType)
		{
			switch (costType)
			{
			case CostType.WhiteItem:
				return ItemTier.Tier1;
			case CostType.GreenItem:
				return ItemTier.Tier2;
			case CostType.RedItem:
				return ItemTier.Tier3;
			default:
				return ItemTier.NoTier;
			}
		}

		// Token: 0x0600138B RID: 5003 RVA: 0x0005F64C File Offset: 0x0005D84C
		public static ColorCatalog.ColorIndex CostTypeToColorIndex(CostType costType)
		{
			switch (costType)
			{
			case CostType.WhiteItem:
				return ColorCatalog.ColorIndex.Tier1Item;
			case CostType.GreenItem:
				return ColorCatalog.ColorIndex.Tier2Item;
			case CostType.RedItem:
				return ColorCatalog.ColorIndex.Tier3Item;
			default:
				return ColorCatalog.ColorIndex.Error;
			}
		}

		// Token: 0x0600138C RID: 5004 RVA: 0x0005F66C File Offset: 0x0005D86C
		public bool CanBeAffordedByInteractor(Interactor activator)
		{
			switch (this.costType)
			{
			case CostType.None:
				return true;
			case CostType.Money:
			{
				CharacterBody component = activator.GetComponent<CharacterBody>();
				if (component)
				{
					CharacterMaster master = component.master;
					if (master)
					{
						return (ulong)master.money >= (ulong)((long)this.cost);
					}
				}
				return false;
			}
			case CostType.PercentHealth:
			{
				HealthComponent component2 = activator.GetComponent<HealthComponent>();
				return component2 && component2.health / component2.fullHealth * 100f >= (float)this.cost;
			}
			case CostType.Lunar:
			{
				NetworkUser networkUser = Util.LookUpBodyNetworkUser(activator.gameObject);
				return networkUser && (ulong)networkUser.lunarCoins >= (ulong)((long)this.cost);
			}
			case CostType.WhiteItem:
			case CostType.GreenItem:
			case CostType.RedItem:
			{
				ItemTier itemTier = PurchaseInteraction.CostTypeToItemTier(this.costType);
				CharacterBody component3 = activator.gameObject.GetComponent<CharacterBody>();
				if (component3)
				{
					Inventory inventory = component3.inventory;
					if (inventory)
					{
						return inventory.HasAtLeastXTotalItemsOfTier(itemTier, this.cost);
					}
				}
				return false;
			}
			default:
				return false;
			}
		}

		// Token: 0x1400001D RID: 29
		// (add) Token: 0x0600138D RID: 5005 RVA: 0x0005F784 File Offset: 0x0005D984
		// (remove) Token: 0x0600138E RID: 5006 RVA: 0x0005F7B8 File Offset: 0x0005D9B8
		public static event Action<PurchaseInteraction, Interactor> onItemSpentOnPurchase;

		// Token: 0x0600138F RID: 5007 RVA: 0x0005F7EC File Offset: 0x0005D9EC
		public void OnInteractionBegin(Interactor activator)
		{
			if (!this.CanBeAffordedByInteractor(activator))
			{
				return;
			}
			CharacterBody component = activator.GetComponent<CharacterBody>();
			switch (this.costType)
			{
			case CostType.Money:
				if (component)
				{
					CharacterMaster master = component.master;
					if (master)
					{
						master.money -= (uint)this.cost;
					}
				}
				break;
			case CostType.PercentHealth:
			{
				HealthComponent component2 = activator.GetComponent<HealthComponent>();
				if (component2)
				{
					float health = component2.health;
					float num = component2.fullHealth * (float)this.cost / 100f;
					if (health > num)
					{
						component2.TakeDamage(new DamageInfo
						{
							damage = num,
							attacker = base.gameObject,
							position = base.transform.position,
							damageType = DamageType.BypassArmor
						});
					}
				}
				break;
			}
			case CostType.Lunar:
			{
				NetworkUser networkUser = Util.LookUpBodyNetworkUser(activator.gameObject);
				if (networkUser)
				{
					networkUser.DeductLunarCoins((uint)this.cost);
				}
				break;
			}
			case CostType.WhiteItem:
			case CostType.GreenItem:
			case CostType.RedItem:
			{
				ItemTier itemTier = PurchaseInteraction.CostTypeToItemTier(this.costType);
				if (component)
				{
					Inventory inventory = component.inventory;
					if (inventory)
					{
						ItemIndex itemIndex = ItemIndex.None;
						ShopTerminalBehavior component3 = base.GetComponent<ShopTerminalBehavior>();
						if (component3)
						{
							itemIndex = component3.CurrentPickupIndex().itemIndex;
						}
						WeightedSelection<ItemIndex> weightedSelection = new WeightedSelection<ItemIndex>(8);
						foreach (ItemIndex itemIndex2 in ItemCatalog.allItems)
						{
							if (itemIndex2 != itemIndex)
							{
								int itemCount = inventory.GetItemCount(itemIndex2);
								if (itemCount > 0 && ItemCatalog.GetItemDef(itemIndex2).tier == itemTier)
								{
									weightedSelection.AddChoice(itemIndex2, (float)itemCount);
								}
							}
						}
						List<ItemIndex> list = new List<ItemIndex>();
						int num2 = 0;
						while (weightedSelection.Count > 0 && num2 < this.cost)
						{
							int num3 = weightedSelection.EvaluteToChoiceIndex(this.rng.nextNormalizedFloat);
							WeightedSelection<ItemIndex>.ChoiceInfo choice = weightedSelection.GetChoice(num3);
							ItemIndex value = choice.value;
							int num4 = (int)choice.weight;
							num4--;
							if (num4 <= 0)
							{
								weightedSelection.RemoveChoice(num3);
							}
							else
							{
								weightedSelection.ModifyChoiceWeight(num3, (float)num4);
							}
							list.Add(value);
							num2++;
						}
						for (int i = num2; i < this.cost; i++)
						{
							list.Add(itemIndex);
						}
						for (int j = 0; j < list.Count; j++)
						{
							ItemIndex itemIndex3 = list[j];
							PurchaseInteraction.CreateItemTakenOrb(component.corePosition, base.gameObject, itemIndex3);
							inventory.RemoveItem(itemIndex3, 1);
							if (itemIndex3 != itemIndex)
							{
								Action<PurchaseInteraction, Interactor> action = PurchaseInteraction.onItemSpentOnPurchase;
								if (action != null)
								{
									action(this, activator);
								}
							}
						}
					}
				}
				break;
			}
			}
			IEnumerable<StatDef> statDefsToIncrement = this.purchaseStatNames.Select(new Func<string, StatDef>(StatDef.Find));
			StatManager.OnPurchase<IEnumerable<StatDef>>(component, this.costType, statDefsToIncrement);
			this.onPurchase.Invoke(activator);
			this.lastActivator = activator;
		}

		// Token: 0x06001390 RID: 5008 RVA: 0x0005FB0C File Offset: 0x0005DD0C
		[Server]
		private static void CreateItemTakenOrb(Vector3 effectOrigin, GameObject targetObject, ItemIndex itemIndex)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.PurchaseInteraction::CreateItemTakenOrb(UnityEngine.Vector3,UnityEngine.GameObject,RoR2.ItemIndex)' called on client");
				return;
			}
			GameObject effectPrefab = Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/ItemTakenOrbEffect");
			EffectData effectData = new EffectData
			{
				origin = effectOrigin,
				genericFloat = 1.5f,
				genericUInt = (uint)(itemIndex + 1)
			};
			effectData.SetNetworkedObjectReference(targetObject);
			EffectManager.instance.SpawnEffect(effectPrefab, effectData, true);
		}

		// Token: 0x06001391 RID: 5009 RVA: 0x0005FB6E File Offset: 0x0005DD6E
		public bool ShouldDisplayHologram(GameObject viewer)
		{
			return this.available;
		}

		// Token: 0x06001392 RID: 5010 RVA: 0x0003863B File Offset: 0x0003683B
		public GameObject GetHologramContentPrefab()
		{
			return Resources.Load<GameObject>("Prefabs/CostHologramContent");
		}

		// Token: 0x06001393 RID: 5011 RVA: 0x0005FB78 File Offset: 0x0005DD78
		public void UpdateHologramContent(GameObject hologramContentObject)
		{
			CostHologramContent component = hologramContentObject.GetComponent<CostHologramContent>();
			if (component)
			{
				component.displayValue = this.cost;
				component.costType = this.costType;
			}
		}

		// Token: 0x06001394 RID: 5012 RVA: 0x0000A1ED File Offset: 0x000083ED
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x06001395 RID: 5013 RVA: 0x0005FBAC File Offset: 0x0005DDAC
		private void OnEnable()
		{
			PurchaseInteraction.instancesList.Add(this);
		}

		// Token: 0x06001396 RID: 5014 RVA: 0x0005FBB9 File Offset: 0x0005DDB9
		private void OnDisable()
		{
			PurchaseInteraction.instancesList.Remove(this);
		}

		// Token: 0x06001399 RID: 5017 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x0600139A RID: 5018 RVA: 0x0005FBFC File Offset: 0x0005DDFC
		// (set) Token: 0x0600139B RID: 5019 RVA: 0x0005FC0F File Offset: 0x0005DE0F
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

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x0600139C RID: 5020 RVA: 0x0005FC24 File Offset: 0x0005DE24
		// (set) Token: 0x0600139D RID: 5021 RVA: 0x0005FC37 File Offset: 0x0005DE37
		public int Networkcost
		{
			get
			{
				return this.cost;
			}
			set
			{
				base.SetSyncVar<int>(value, ref this.cost, 2u);
			}
		}

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x0600139E RID: 5022 RVA: 0x0005FC4C File Offset: 0x0005DE4C
		// (set) Token: 0x0600139F RID: 5023 RVA: 0x0005FC5F File Offset: 0x0005DE5F
		public GameObject NetworklockGameObject
		{
			get
			{
				return this.lockGameObject;
			}
			set
			{
				base.SetSyncVarGameObject(value, ref this.lockGameObject, 4u, ref this.___lockGameObjectNetId);
			}
		}

		// Token: 0x060013A0 RID: 5024 RVA: 0x0005FC7C File Offset: 0x0005DE7C
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.available);
				writer.WritePackedUInt32((uint)this.cost);
				writer.Write(this.lockGameObject);
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
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32((uint)this.cost);
			}
			if ((base.syncVarDirtyBits & 4u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.lockGameObject);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x060013A1 RID: 5025 RVA: 0x0005FD68 File Offset: 0x0005DF68
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.available = reader.ReadBoolean();
				this.cost = (int)reader.ReadPackedUInt32();
				this.___lockGameObjectNetId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.available = reader.ReadBoolean();
			}
			if ((num & 2) != 0)
			{
				this.cost = (int)reader.ReadPackedUInt32();
			}
			if ((num & 4) != 0)
			{
				this.lockGameObject = reader.ReadGameObject();
			}
		}

		// Token: 0x060013A2 RID: 5026 RVA: 0x0005FDF3 File Offset: 0x0005DFF3
		public override void PreStartClient()
		{
			if (!this.___lockGameObjectNetId.IsEmpty())
			{
				this.NetworklockGameObject = ClientScene.FindLocalObject(this.___lockGameObjectNetId);
			}
		}

		// Token: 0x0400173A RID: 5946
		private static readonly List<PurchaseInteraction> instancesList = new List<PurchaseInteraction>();

		// Token: 0x0400173B RID: 5947
		public static readonly ReadOnlyCollection<PurchaseInteraction> readOnlyInstancesList = PurchaseInteraction.instancesList.AsReadOnly();

		// Token: 0x0400173C RID: 5948
		public string displayNameToken;

		// Token: 0x0400173D RID: 5949
		public string contextToken;

		// Token: 0x0400173E RID: 5950
		public CostType costType;

		// Token: 0x0400173F RID: 5951
		[SyncVar]
		public bool available = true;

		// Token: 0x04001740 RID: 5952
		[SyncVar]
		public int cost;

		// Token: 0x04001741 RID: 5953
		public bool automaticallyScaleCostWithDifficulty;

		// Token: 0x04001742 RID: 5954
		[Tooltip("The unlockable that a player must have to be able to interact with this terminal.")]
		public string requiredUnlockable = "";

		// Token: 0x04001743 RID: 5955
		public bool ignoreSpherecastForInteractability;

		// Token: 0x04001744 RID: 5956
		public string[] purchaseStatNames;

		// Token: 0x04001745 RID: 5957
		[HideInInspector]
		public Interactor lastActivator;

		// Token: 0x04001746 RID: 5958
		[SyncVar]
		public GameObject lockGameObject;

		// Token: 0x04001747 RID: 5959
		private Xoroshiro128Plus rng;

		// Token: 0x04001749 RID: 5961
		public PurchaseEvent onPurchase;

		// Token: 0x0400174A RID: 5962
		private NetworkInstanceId ___lockGameObjectNetId;
	}
}
