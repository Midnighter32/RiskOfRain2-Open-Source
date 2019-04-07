using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000362 RID: 866
	public class MultiShopController : NetworkBehaviour, IHologramContentProvider
	{
		// Token: 0x060011C5 RID: 4549 RVA: 0x00057F26 File Offset: 0x00056126
		private void Awake()
		{
			if (NetworkServer.active)
			{
				this.CreateTerminals();
			}
		}

		// Token: 0x060011C6 RID: 4550 RVA: 0x00057F38 File Offset: 0x00056138
		private void Start()
		{
			if (Run.instance && NetworkServer.active)
			{
				this.Networkcost = Run.instance.GetDifficultyScaledCost(this.baseCost);
				if (this.terminalGameObjects != null)
				{
					GameObject[] array = this.terminalGameObjects;
					for (int i = 0; i < array.Length; i++)
					{
						PurchaseInteraction component = array[i].GetComponent<PurchaseInteraction>();
						component.Networkcost = this.cost;
						component.costType = this.costType;
					}
				}
			}
		}

		// Token: 0x060011C7 RID: 4551 RVA: 0x00057FAC File Offset: 0x000561AC
		private void OnDestroy()
		{
			if (this.terminalGameObjects != null)
			{
				for (int i = this.terminalGameObjects.Length - 1; i >= 0; i--)
				{
					UnityEngine.Object.Destroy(this.terminalGameObjects[i]);
				}
				this.terminalGameObjects = null;
			}
		}

		// Token: 0x060011C8 RID: 4552 RVA: 0x00057FEC File Offset: 0x000561EC
		private void CreateTerminals()
		{
			this.terminalGameObjects = new GameObject[this.terminalPositions.Length];
			for (int i = 0; i < this.terminalPositions.Length; i++)
			{
				PickupIndex newPickupIndex = PickupIndex.none;
				switch (this.itemTier)
				{
				case ItemTier.Tier1:
					newPickupIndex = Run.instance.availableTier1DropList[Run.instance.treasureRng.RangeInt(0, Run.instance.availableTier1DropList.Count)];
					break;
				case ItemTier.Tier2:
					newPickupIndex = Run.instance.availableTier2DropList[Run.instance.treasureRng.RangeInt(0, Run.instance.availableTier2DropList.Count)];
					break;
				case ItemTier.Tier3:
					newPickupIndex = Run.instance.availableTier3DropList[Run.instance.treasureRng.RangeInt(0, Run.instance.availableTier3DropList.Count)];
					break;
				case ItemTier.Lunar:
					newPickupIndex = Run.instance.availableLunarDropList[Run.instance.treasureRng.RangeInt(0, Run.instance.availableLunarDropList.Count)];
					break;
				}
				bool newHidden = this.hideDisplayContent && Run.instance.treasureRng.nextNormalizedFloat < 0.2f;
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.terminalPrefab, this.terminalPositions[i].position, this.terminalPositions[i].rotation);
				this.terminalGameObjects[i] = gameObject;
				gameObject.GetComponent<ShopTerminalBehavior>().SetPickupIndex(newPickupIndex, newHidden);
				NetworkServer.Spawn(gameObject);
			}
			GameObject[] array = this.terminalGameObjects;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].GetComponent<PurchaseInteraction>().onPurchase.AddListener(new UnityAction<Interactor>(this.DisableAllTerminals));
			}
		}

		// Token: 0x060011C9 RID: 4553 RVA: 0x000581B4 File Offset: 0x000563B4
		private void DisableAllTerminals(Interactor interactor)
		{
			foreach (GameObject gameObject in this.terminalGameObjects)
			{
				gameObject.GetComponent<PurchaseInteraction>().Networkavailable = false;
				gameObject.GetComponent<ShopTerminalBehavior>().SetNoPickup();
			}
			this.Networkavailable = false;
		}

		// Token: 0x060011CA RID: 4554 RVA: 0x000581F6 File Offset: 0x000563F6
		public bool ShouldDisplayHologram(GameObject viewer)
		{
			return this.available;
		}

		// Token: 0x060011CB RID: 4555 RVA: 0x0003863B File Offset: 0x0003683B
		public GameObject GetHologramContentPrefab()
		{
			return Resources.Load<GameObject>("Prefabs/CostHologramContent");
		}

		// Token: 0x060011CC RID: 4556 RVA: 0x00058200 File Offset: 0x00056400
		public void UpdateHologramContent(GameObject hologramContentObject)
		{
			CostHologramContent component = hologramContentObject.GetComponent<CostHologramContent>();
			if (component)
			{
				component.displayValue = this.cost;
				component.costType = this.costType;
			}
		}

		// Token: 0x060011CE RID: 4558 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x060011CF RID: 4559 RVA: 0x0005824C File Offset: 0x0005644C
		// (set) Token: 0x060011D0 RID: 4560 RVA: 0x0005825F File Offset: 0x0005645F
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

		// Token: 0x17000189 RID: 393
		// (get) Token: 0x060011D1 RID: 4561 RVA: 0x00058274 File Offset: 0x00056474
		// (set) Token: 0x060011D2 RID: 4562 RVA: 0x00058287 File Offset: 0x00056487
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

		// Token: 0x060011D3 RID: 4563 RVA: 0x0005829C File Offset: 0x0005649C
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.available);
				writer.WritePackedUInt32((uint)this.cost);
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
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x060011D4 RID: 4564 RVA: 0x00058348 File Offset: 0x00056548
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.available = reader.ReadBoolean();
				this.cost = (int)reader.ReadPackedUInt32();
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
		}

		// Token: 0x040015E2 RID: 5602
		[Tooltip("The shop terminal prefab to instantiate.")]
		public GameObject terminalPrefab;

		// Token: 0x040015E3 RID: 5603
		[Tooltip("The positions at which to instantiate shop terminals.")]
		public Transform[] terminalPositions;

		// Token: 0x040015E4 RID: 5604
		[Tooltip("The tier of items to drop")]
		public ItemTier itemTier;

		// Token: 0x040015E5 RID: 5605
		[Tooltip("Whether or not there's a chance the item contents are replaced with a '?'")]
		private bool hideDisplayContent = true;

		// Token: 0x040015E6 RID: 5606
		private GameObject[] terminalGameObjects;

		// Token: 0x040015E7 RID: 5607
		[SyncVar]
		private bool available = true;

		// Token: 0x040015E8 RID: 5608
		public int baseCost;

		// Token: 0x040015E9 RID: 5609
		public CostType costType;

		// Token: 0x040015EA RID: 5610
		[SyncVar]
		private int cost;
	}
}
