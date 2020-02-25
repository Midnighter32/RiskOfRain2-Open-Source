using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000293 RID: 659
	public class MultiShopController : NetworkBehaviour, IHologramContentProvider
	{
		// Token: 0x06000E9F RID: 3743 RVA: 0x00040DAA File Offset: 0x0003EFAA
		private void Awake()
		{
			if (NetworkServer.active)
			{
				this.rng = new Xoroshiro128Plus(Run.instance.treasureRng.nextUlong);
				this.CreateTerminals();
			}
		}

		// Token: 0x06000EA0 RID: 3744 RVA: 0x00040DD4 File Offset: 0x0003EFD4
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

		// Token: 0x06000EA1 RID: 3745 RVA: 0x00040E48 File Offset: 0x0003F048
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

		// Token: 0x06000EA2 RID: 3746 RVA: 0x00040E88 File Offset: 0x0003F088
		private void CreateTerminals()
		{
			this.terminalGameObjects = new GameObject[this.terminalPositions.Length];
			for (int i = 0; i < this.terminalPositions.Length; i++)
			{
				PickupIndex newPickupIndex = PickupIndex.none;
				switch (this.itemTier)
				{
				case ItemTier.Tier1:
					newPickupIndex = this.rng.NextElementUniform<PickupIndex>(Run.instance.availableTier1DropList);
					break;
				case ItemTier.Tier2:
					newPickupIndex = this.rng.NextElementUniform<PickupIndex>(Run.instance.availableTier2DropList);
					break;
				case ItemTier.Tier3:
					newPickupIndex = this.rng.NextElementUniform<PickupIndex>(Run.instance.availableTier3DropList);
					break;
				case ItemTier.Lunar:
					newPickupIndex = this.rng.NextElementUniform<PickupIndex>(Run.instance.availableLunarDropList);
					break;
				}
				bool newHidden = this.hideDisplayContent && i != 0 && this.rng.nextNormalizedFloat < 0.2f;
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

		// Token: 0x06000EA3 RID: 3747 RVA: 0x00040FE4 File Offset: 0x0003F1E4
		private void DisableAllTerminals(Interactor interactor)
		{
			foreach (GameObject gameObject in this.terminalGameObjects)
			{
				gameObject.GetComponent<PurchaseInteraction>().Networkavailable = false;
				gameObject.GetComponent<ShopTerminalBehavior>().SetNoPickup();
			}
			this.Networkavailable = false;
		}

		// Token: 0x06000EA4 RID: 3748 RVA: 0x00041026 File Offset: 0x0003F226
		public bool ShouldDisplayHologram(GameObject viewer)
		{
			return this.available;
		}

		// Token: 0x06000EA5 RID: 3749 RVA: 0x0001A32B File Offset: 0x0001852B
		public GameObject GetHologramContentPrefab()
		{
			return Resources.Load<GameObject>("Prefabs/CostHologramContent");
		}

		// Token: 0x06000EA6 RID: 3750 RVA: 0x00041030 File Offset: 0x0003F230
		public void UpdateHologramContent(GameObject hologramContentObject)
		{
			CostHologramContent component = hologramContentObject.GetComponent<CostHologramContent>();
			if (component)
			{
				component.displayValue = this.cost;
				component.costType = this.costType;
			}
		}

		// Token: 0x06000EA8 RID: 3752 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x06000EA9 RID: 3753 RVA: 0x0004107C File Offset: 0x0003F27C
		// (set) Token: 0x06000EAA RID: 3754 RVA: 0x0004108F File Offset: 0x0003F28F
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

		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x06000EAB RID: 3755 RVA: 0x000410A4 File Offset: 0x0003F2A4
		// (set) Token: 0x06000EAC RID: 3756 RVA: 0x000410B7 File Offset: 0x0003F2B7
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

		// Token: 0x06000EAD RID: 3757 RVA: 0x000410CC File Offset: 0x0003F2CC
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.available);
				writer.WritePackedUInt32((uint)this.cost);
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
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000EAE RID: 3758 RVA: 0x00041178 File Offset: 0x0003F378
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

		// Token: 0x04000E89 RID: 3721
		[Tooltip("The shop terminal prefab to instantiate.")]
		public GameObject terminalPrefab;

		// Token: 0x04000E8A RID: 3722
		[Tooltip("The positions at which to instantiate shop terminals.")]
		public Transform[] terminalPositions;

		// Token: 0x04000E8B RID: 3723
		[Tooltip("The tier of items to drop")]
		public ItemTier itemTier;

		// Token: 0x04000E8C RID: 3724
		[Tooltip("Whether or not there's a chance the item contents are replaced with a '?'")]
		private bool hideDisplayContent = true;

		// Token: 0x04000E8D RID: 3725
		private GameObject[] terminalGameObjects;

		// Token: 0x04000E8E RID: 3726
		[SyncVar]
		private bool available = true;

		// Token: 0x04000E8F RID: 3727
		public int baseCost;

		// Token: 0x04000E90 RID: 3728
		public CostTypeIndex costType;

		// Token: 0x04000E91 RID: 3729
		[SyncVar]
		private int cost;

		// Token: 0x04000E92 RID: 3730
		private Xoroshiro128Plus rng;
	}
}
