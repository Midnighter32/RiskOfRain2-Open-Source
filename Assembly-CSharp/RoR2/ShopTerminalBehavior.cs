using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200032C RID: 812
	public class ShopTerminalBehavior : NetworkBehaviour
	{
		// Token: 0x1700024F RID: 591
		// (get) Token: 0x06001324 RID: 4900 RVA: 0x00052091 File Offset: 0x00050291
		public bool pickupIndexIsHidden
		{
			get
			{
				return this.hidden;
			}
		}

		// Token: 0x06001325 RID: 4901 RVA: 0x0005209C File Offset: 0x0005029C
		private void SetHasBeenPurchased(bool newHasBeenPurchased)
		{
			if (this.hasBeenPurchased != newHasBeenPurchased)
			{
				this.NetworkhasBeenPurchased = newHasBeenPurchased;
				if (newHasBeenPurchased && this.animator)
				{
					int layerIndex = this.animator.GetLayerIndex("Body");
					this.animator.PlayInFixedTime("Opening", layerIndex);
				}
			}
		}

		// Token: 0x06001326 RID: 4902 RVA: 0x000520EB File Offset: 0x000502EB
		private void OnSyncHidden(bool newHidden)
		{
			this.SetPickupIndex(this.pickupIndex, newHidden);
		}

		// Token: 0x06001327 RID: 4903 RVA: 0x000520FA File Offset: 0x000502FA
		private void OnSyncPickupIndex(PickupIndex newPickupIndex)
		{
			this.SetPickupIndex(newPickupIndex, this.hidden);
		}

		// Token: 0x06001328 RID: 4904 RVA: 0x00052109 File Offset: 0x00050309
		public void Start()
		{
			if (NetworkServer.active && this.selfGeneratePickup)
			{
				this.GenerateNewPickupServer();
			}
			if (NetworkClient.active)
			{
				this.UpdatePickupDisplayAndAnimations();
			}
		}

		// Token: 0x06001329 RID: 4905 RVA: 0x00052130 File Offset: 0x00050330
		[Server]
		public void GenerateNewPickupServer()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ShopTerminalBehavior::GenerateNewPickupServer()' called on client");
				return;
			}
			PickupIndex newPickupIndex = PickupIndex.none;
			if (this.dropTable)
			{
				newPickupIndex = this.dropTable.GenerateDrop(Run.instance.treasureRng);
			}
			else
			{
				List<PickupIndex> list;
				switch (this.itemTier)
				{
				case ItemTier.Tier1:
					list = Run.instance.availableTier1DropList;
					break;
				case ItemTier.Tier2:
					list = Run.instance.availableTier2DropList;
					break;
				case ItemTier.Tier3:
					list = Run.instance.availableTier3DropList;
					break;
				case ItemTier.Lunar:
					list = Run.instance.availableLunarDropList;
					break;
				case ItemTier.Boss:
					list = Run.instance.availableBossDropList;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				newPickupIndex = this.<GenerateNewPickupServer>g__Pick|17_1(list);
			}
			this.SetPickupIndex(newPickupIndex, false);
		}

		// Token: 0x0600132A RID: 4906 RVA: 0x000521F9 File Offset: 0x000503F9
		public void SetPickupIndex(PickupIndex newPickupIndex, bool newHidden = false)
		{
			if (this.pickupIndex != newPickupIndex || this.hidden != newHidden)
			{
				this.NetworkpickupIndex = newPickupIndex;
				this.Networkhidden = newHidden;
				this.UpdatePickupDisplayAndAnimations();
			}
		}

		// Token: 0x0600132B RID: 4907 RVA: 0x00052228 File Offset: 0x00050428
		private void UpdatePickupDisplayAndAnimations()
		{
			if (this.pickupDisplay)
			{
				this.pickupDisplay.SetPickupIndex(this.pickupIndex, this.hidden);
			}
			if (this.pickupIndex == PickupIndex.none)
			{
				Util.PlaySound("Play_UI_tripleChestShutter", base.gameObject);
				if (this.animator)
				{
					int layerIndex = this.animator.GetLayerIndex("Body");
					this.animator.PlayInFixedTime(this.hasBeenPurchased ? "Open" : "Closing", layerIndex);
				}
			}
		}

		// Token: 0x0600132C RID: 4908 RVA: 0x000522BA File Offset: 0x000504BA
		public PickupIndex CurrentPickupIndex()
		{
			return this.pickupIndex;
		}

		// Token: 0x0600132D RID: 4909 RVA: 0x000522C2 File Offset: 0x000504C2
		[Server]
		public void SetNoPickup()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ShopTerminalBehavior::SetNoPickup()' called on client");
				return;
			}
			this.SetPickupIndex(PickupIndex.none, false);
		}

		// Token: 0x0600132E RID: 4910 RVA: 0x000522E8 File Offset: 0x000504E8
		[Server]
		public void DropPickup()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ShopTerminalBehavior::DropPickup()' called on client");
				return;
			}
			this.SetHasBeenPurchased(true);
			PickupDropletController.CreatePickupDroplet(this.pickupIndex, (this.dropTransform ? this.dropTransform : base.transform).position, base.transform.TransformVector(this.dropVelocity));
		}

		// Token: 0x06001330 RID: 4912 RVA: 0x00052360 File Offset: 0x00050560
		[CompilerGenerated]
		private bool <GenerateNewPickupServer>g__PassesFilter|17_0(PickupIndex pickupIndex)
		{
			if (this.bannedItemTag == ItemTag.Any)
			{
				return true;
			}
			PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
			return pickupDef.itemIndex == ItemIndex.None || !ItemCatalog.GetItemDef(pickupDef.itemIndex).ContainsTag(this.bannedItemTag);
		}

		// Token: 0x06001331 RID: 4913 RVA: 0x000523A2 File Offset: 0x000505A2
		[CompilerGenerated]
		private PickupIndex <GenerateNewPickupServer>g__Pick|17_1(List<PickupIndex> list)
		{
			return Run.instance.treasureRng.NextElementUniform<PickupIndex>(list.Where(new Func<PickupIndex, bool>(this.<GenerateNewPickupServer>g__PassesFilter|17_0)).ToList<PickupIndex>());
		}

		// Token: 0x06001332 RID: 4914 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x17000250 RID: 592
		// (get) Token: 0x06001333 RID: 4915 RVA: 0x000523CC File Offset: 0x000505CC
		// (set) Token: 0x06001334 RID: 4916 RVA: 0x000523DF File Offset: 0x000505DF
		public PickupIndex NetworkpickupIndex
		{
			get
			{
				return this.pickupIndex;
			}
			[param: In]
			set
			{
				uint dirtyBit = 1U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncPickupIndex(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<PickupIndex>(value, ref this.pickupIndex, dirtyBit);
			}
		}

		// Token: 0x17000251 RID: 593
		// (get) Token: 0x06001335 RID: 4917 RVA: 0x00052420 File Offset: 0x00050620
		// (set) Token: 0x06001336 RID: 4918 RVA: 0x00052433 File Offset: 0x00050633
		public bool Networkhidden
		{
			get
			{
				return this.hidden;
			}
			[param: In]
			set
			{
				uint dirtyBit = 2U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncHidden(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<bool>(value, ref this.hidden, dirtyBit);
			}
		}

		// Token: 0x17000252 RID: 594
		// (get) Token: 0x06001337 RID: 4919 RVA: 0x00052474 File Offset: 0x00050674
		// (set) Token: 0x06001338 RID: 4920 RVA: 0x00052487 File Offset: 0x00050687
		public bool NetworkhasBeenPurchased
		{
			get
			{
				return this.hasBeenPurchased;
			}
			[param: In]
			set
			{
				uint dirtyBit = 4U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.SetHasBeenPurchased(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<bool>(value, ref this.hasBeenPurchased, dirtyBit);
			}
		}

		// Token: 0x06001339 RID: 4921 RVA: 0x000524C8 File Offset: 0x000506C8
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				GeneratedNetworkCode._WritePickupIndex_None(writer, this.pickupIndex);
				writer.Write(this.hidden);
				writer.Write(this.hasBeenPurchased);
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
				GeneratedNetworkCode._WritePickupIndex_None(writer, this.pickupIndex);
			}
			if ((base.syncVarDirtyBits & 2U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.hidden);
			}
			if ((base.syncVarDirtyBits & 4U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.hasBeenPurchased);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x0600133A RID: 4922 RVA: 0x000525B4 File Offset: 0x000507B4
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.pickupIndex = GeneratedNetworkCode._ReadPickupIndex_None(reader);
				this.hidden = reader.ReadBoolean();
				this.hasBeenPurchased = reader.ReadBoolean();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.OnSyncPickupIndex(GeneratedNetworkCode._ReadPickupIndex_None(reader));
			}
			if ((num & 2) != 0)
			{
				this.OnSyncHidden(reader.ReadBoolean());
			}
			if ((num & 4) != 0)
			{
				this.SetHasBeenPurchased(reader.ReadBoolean());
			}
		}

		// Token: 0x040011F1 RID: 4593
		[SyncVar(hook = "OnSyncPickupIndex")]
		private PickupIndex pickupIndex = PickupIndex.none;

		// Token: 0x040011F2 RID: 4594
		[SyncVar(hook = "OnSyncHidden")]
		private bool hidden;

		// Token: 0x040011F3 RID: 4595
		[SyncVar(hook = "SetHasBeenPurchased")]
		private bool hasBeenPurchased;

		// Token: 0x040011F4 RID: 4596
		[Tooltip("The PickupDisplay component that should show which item this shop terminal is offering.")]
		public PickupDisplay pickupDisplay;

		// Token: 0x040011F5 RID: 4597
		[Tooltip("The position from which the drop will be emitted")]
		public Transform dropTransform;

		// Token: 0x040011F6 RID: 4598
		[Tooltip("Whether or not the shop terminal should drive itself")]
		public bool selfGeneratePickup;

		// Token: 0x040011F7 RID: 4599
		[Tooltip("The drop table to select a pickup index from - only works if the pickup generates itself")]
		public PickupDropTable dropTable;

		// Token: 0x040011F8 RID: 4600
		[Tooltip("The tier of items to drop - only works if the pickup generates itself and the dropTable field is empty.")]
		public ItemTier itemTier;

		// Token: 0x040011F9 RID: 4601
		public ItemTag bannedItemTag;

		// Token: 0x040011FA RID: 4602
		[Tooltip("The velocity with which the drop will be emitted. Rotates with this object.")]
		public Vector3 dropVelocity;

		// Token: 0x040011FB RID: 4603
		public Animator animator;
	}
}
