using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using RoR2.Stats;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002DA RID: 730
	[RequireComponent(typeof(Highlight))]
	public sealed class PurchaseInteraction : NetworkBehaviour, IInteractable, IHologramContentProvider, IDisplayNameProvider
	{
		// Token: 0x060010A6 RID: 4262 RVA: 0x00049090 File Offset: 0x00047290
		private void Awake()
		{
			if (NetworkServer.active)
			{
				if (this.automaticallyScaleCostWithDifficulty)
				{
					this.Networkcost = Run.instance.GetDifficultyScaledCost(this.cost);
				}
				this.rng = new Xoroshiro128Plus(Run.instance.treasureRng.nextUlong);
			}
		}

		// Token: 0x060010A7 RID: 4263 RVA: 0x000490DC File Offset: 0x000472DC
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

		// Token: 0x060010A8 RID: 4264 RVA: 0x000490FA File Offset: 0x000472FA
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

		// Token: 0x060010A9 RID: 4265 RVA: 0x00049124 File Offset: 0x00047324
		private void SetAvailableTrue()
		{
			this.Networkavailable = true;
		}

		// Token: 0x060010AA RID: 4266 RVA: 0x0004912D File Offset: 0x0004732D
		public string GetDisplayName()
		{
			return Language.GetString(this.displayNameToken);
		}

		// Token: 0x060010AB RID: 4267 RVA: 0x0004913C File Offset: 0x0004733C
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

		// Token: 0x060010AC RID: 4268 RVA: 0x00049178 File Offset: 0x00047378
		public string GetContextString(Interactor activator)
		{
			PurchaseInteraction.sharedStringBuilder.Clear();
			PurchaseInteraction.sharedStringBuilder.Append(Language.GetString(this.contextToken));
			if (this.costType != CostTypeIndex.None)
			{
				PurchaseInteraction.sharedStringBuilder.Append(" <nobr>(");
				CostTypeCatalog.GetCostTypeDef(this.costType).BuildCostStringStyled(this.cost, PurchaseInteraction.sharedStringBuilder, false, true);
				PurchaseInteraction.sharedStringBuilder.Append(")</nobr>");
			}
			return PurchaseInteraction.sharedStringBuilder.ToString();
		}

		// Token: 0x060010AD RID: 4269 RVA: 0x000491F8 File Offset: 0x000473F8
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

		// Token: 0x060010AE RID: 4270 RVA: 0x00049245 File Offset: 0x00047445
		public bool CanBeAffordedByInteractor(Interactor activator)
		{
			return CostTypeCatalog.GetCostTypeDef(this.costType).IsAffordable(this.cost, activator);
		}

		// Token: 0x1400002E RID: 46
		// (add) Token: 0x060010AF RID: 4271 RVA: 0x00049260 File Offset: 0x00047460
		// (remove) Token: 0x060010B0 RID: 4272 RVA: 0x00049294 File Offset: 0x00047494
		public static event Action<PurchaseInteraction, Interactor> onItemSpentOnPurchase;

		// Token: 0x1400002F RID: 47
		// (add) Token: 0x060010B1 RID: 4273 RVA: 0x000492C8 File Offset: 0x000474C8
		// (remove) Token: 0x060010B2 RID: 4274 RVA: 0x000492FC File Offset: 0x000474FC
		public static event Action<PurchaseInteraction, Interactor, EquipmentIndex> onEquipmentSpentOnPurchase;

		// Token: 0x060010B3 RID: 4275 RVA: 0x00049330 File Offset: 0x00047530
		public void OnInteractionBegin(Interactor activator)
		{
			if (!this.CanBeAffordedByInteractor(activator))
			{
				return;
			}
			CharacterBody component = activator.GetComponent<CharacterBody>();
			CostTypeDef costTypeDef = CostTypeCatalog.GetCostTypeDef(this.costType);
			ItemIndex itemIndex = ItemIndex.None;
			ShopTerminalBehavior component2 = base.GetComponent<ShopTerminalBehavior>();
			if (component2)
			{
				itemIndex = component2.CurrentPickupIndex().itemIndex;
			}
			CostTypeDef.PayCostResults payCostResults = costTypeDef.PayCost(this.cost, activator, base.gameObject, this.rng, itemIndex);
			foreach (ItemIndex itemIndex2 in payCostResults.itemsTaken)
			{
				PurchaseInteraction.CreateItemTakenOrb(component.corePosition, base.gameObject, itemIndex2);
				if (itemIndex2 != itemIndex)
				{
					Action<PurchaseInteraction, Interactor> action = PurchaseInteraction.onItemSpentOnPurchase;
					if (action != null)
					{
						action(this, activator);
					}
				}
			}
			foreach (EquipmentIndex arg in payCostResults.equipmentTaken)
			{
				Action<PurchaseInteraction, Interactor, EquipmentIndex> action2 = PurchaseInteraction.onEquipmentSpentOnPurchase;
				if (action2 != null)
				{
					action2(this, activator, arg);
				}
			}
			IEnumerable<StatDef> statDefsToIncrement = this.purchaseStatNames.Select(new Func<string, StatDef>(StatDef.Find));
			StatManager.OnPurchase<IEnumerable<StatDef>>(component, this.costType, statDefsToIncrement);
			this.onPurchase.Invoke(activator);
			this.lastActivator = activator;
		}

		// Token: 0x060010B4 RID: 4276 RVA: 0x0004948C File Offset: 0x0004768C
		[Server]
		public static void CreateItemTakenOrb(Vector3 effectOrigin, GameObject targetObject, ItemIndex itemIndex)
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
			EffectManager.SpawnEffect(effectPrefab, effectData, true);
		}

		// Token: 0x060010B5 RID: 4277 RVA: 0x000494E7 File Offset: 0x000476E7
		public bool ShouldDisplayHologram(GameObject viewer)
		{
			return this.available;
		}

		// Token: 0x060010B6 RID: 4278 RVA: 0x0001A32B File Offset: 0x0001852B
		public GameObject GetHologramContentPrefab()
		{
			return Resources.Load<GameObject>("Prefabs/CostHologramContent");
		}

		// Token: 0x060010B7 RID: 4279 RVA: 0x000494F0 File Offset: 0x000476F0
		public void UpdateHologramContent(GameObject hologramContentObject)
		{
			CostHologramContent component = hologramContentObject.GetComponent<CostHologramContent>();
			if (component)
			{
				component.displayValue = this.cost;
				component.costType = this.costType;
			}
		}

		// Token: 0x060010B8 RID: 4280 RVA: 0x0000AC89 File Offset: 0x00008E89
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x060010B9 RID: 4281 RVA: 0x000494E7 File Offset: 0x000476E7
		public bool ShouldShowOnScanner()
		{
			return this.available;
		}

		// Token: 0x060010BA RID: 4282 RVA: 0x00049524 File Offset: 0x00047724
		private void OnEnable()
		{
			InstanceTracker.Add<PurchaseInteraction>(this);
		}

		// Token: 0x060010BB RID: 4283 RVA: 0x0004952C File Offset: 0x0004772C
		private void OnDisable()
		{
			InstanceTracker.Remove<PurchaseInteraction>(this);
		}

		// Token: 0x060010BC RID: 4284 RVA: 0x00049534 File Offset: 0x00047734
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			TeleporterInteraction.onTeleporterBeginChargingGlobal += PurchaseInteraction.OnTeleporterBeginCharging;
		}

		// Token: 0x060010BD RID: 4285 RVA: 0x00049548 File Offset: 0x00047748
		private static void OnTeleporterBeginCharging(TeleporterInteraction teleporterInteraction)
		{
			if (NetworkServer.active)
			{
				foreach (PurchaseInteraction purchaseInteraction in InstanceTracker.GetInstancesList<PurchaseInteraction>())
				{
					if (purchaseInteraction.setUnavailableOnTeleporterActivated)
					{
						purchaseInteraction.SetAvailable(false);
						purchaseInteraction.CancelInvoke("SetUnavailableTemporarily");
					}
				}
			}
		}

		// Token: 0x060010C0 RID: 4288 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x1700020B RID: 523
		// (get) Token: 0x060010C1 RID: 4289 RVA: 0x000495DC File Offset: 0x000477DC
		// (set) Token: 0x060010C2 RID: 4290 RVA: 0x000495EF File Offset: 0x000477EF
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

		// Token: 0x1700020C RID: 524
		// (get) Token: 0x060010C3 RID: 4291 RVA: 0x00049604 File Offset: 0x00047804
		// (set) Token: 0x060010C4 RID: 4292 RVA: 0x00049617 File Offset: 0x00047817
		public int Networkcost
		{
			get
			{
				return this.cost;
			}
			[param: In]
			set
			{
				base.SetSyncVar<int>(value, ref this.cost, 2U);
			}
		}

		// Token: 0x1700020D RID: 525
		// (get) Token: 0x060010C5 RID: 4293 RVA: 0x0004962C File Offset: 0x0004782C
		// (set) Token: 0x060010C6 RID: 4294 RVA: 0x0004963F File Offset: 0x0004783F
		public GameObject NetworklockGameObject
		{
			get
			{
				return this.lockGameObject;
			}
			[param: In]
			set
			{
				base.SetSyncVarGameObject(value, ref this.lockGameObject, 4U, ref this.___lockGameObjectNetId);
			}
		}

		// Token: 0x060010C7 RID: 4295 RVA: 0x0004965C File Offset: 0x0004785C
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
			if ((base.syncVarDirtyBits & 1U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.available);
			}
			if ((base.syncVarDirtyBits & 2U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32((uint)this.cost);
			}
			if ((base.syncVarDirtyBits & 4U) != 0U)
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

		// Token: 0x060010C8 RID: 4296 RVA: 0x00049748 File Offset: 0x00047948
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

		// Token: 0x060010C9 RID: 4297 RVA: 0x000497D3 File Offset: 0x000479D3
		public override void PreStartClient()
		{
			if (!this.___lockGameObjectNetId.IsEmpty())
			{
				this.NetworklockGameObject = ClientScene.FindLocalObject(this.___lockGameObjectNetId);
			}
		}

		// Token: 0x04001008 RID: 4104
		public string displayNameToken;

		// Token: 0x04001009 RID: 4105
		public string contextToken;

		// Token: 0x0400100A RID: 4106
		public CostTypeIndex costType;

		// Token: 0x0400100B RID: 4107
		[SyncVar]
		public bool available = true;

		// Token: 0x0400100C RID: 4108
		[SyncVar]
		public int cost;

		// Token: 0x0400100D RID: 4109
		public bool automaticallyScaleCostWithDifficulty;

		// Token: 0x0400100E RID: 4110
		[Tooltip("The unlockable that a player must have to be able to interact with this terminal.")]
		public string requiredUnlockable = "";

		// Token: 0x0400100F RID: 4111
		public bool ignoreSpherecastForInteractability;

		// Token: 0x04001010 RID: 4112
		public string[] purchaseStatNames;

		// Token: 0x04001011 RID: 4113
		public bool setUnavailableOnTeleporterActivated;

		// Token: 0x04001012 RID: 4114
		[HideInInspector]
		public Interactor lastActivator;

		// Token: 0x04001013 RID: 4115
		[SyncVar]
		public GameObject lockGameObject;

		// Token: 0x04001014 RID: 4116
		private Xoroshiro128Plus rng;

		// Token: 0x04001015 RID: 4117
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();

		// Token: 0x04001018 RID: 4120
		public PurchaseEvent onPurchase;

		// Token: 0x04001019 RID: 4121
		private NetworkInstanceId ___lockGameObjectNetId;
	}
}
