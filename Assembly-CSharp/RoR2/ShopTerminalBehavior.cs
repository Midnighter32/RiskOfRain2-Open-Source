using System;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003DB RID: 987
	public class ShopTerminalBehavior : NetworkBehaviour
	{
		// Token: 0x170001ED RID: 493
		// (get) Token: 0x06001561 RID: 5473 RVA: 0x000668E9 File Offset: 0x00064AE9
		public bool pickupIndexIsHidden
		{
			get
			{
				return this.hidden;
			}
		}

		// Token: 0x06001562 RID: 5474 RVA: 0x000668F4 File Offset: 0x00064AF4
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

		// Token: 0x06001563 RID: 5475 RVA: 0x00066943 File Offset: 0x00064B43
		private void OnSyncHidden(bool newHidden)
		{
			this.SetPickupIndex(this.pickupIndex, newHidden);
		}

		// Token: 0x06001564 RID: 5476 RVA: 0x00066952 File Offset: 0x00064B52
		private void OnSyncPickupIndex(PickupIndex newPickupIndex)
		{
			this.SetPickupIndex(newPickupIndex, this.hidden);
		}

		// Token: 0x06001565 RID: 5477 RVA: 0x00066964 File Offset: 0x00064B64
		public void Start()
		{
			if (NetworkServer.active && this.selfGeneratePickup)
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
				this.SetPickupIndex(newPickupIndex, false);
			}
			if (NetworkClient.active)
			{
				this.UpdatePickupDisplayAndAnimations();
			}
		}

		// Token: 0x06001566 RID: 5478 RVA: 0x00066A88 File Offset: 0x00064C88
		public void SetPickupIndex(PickupIndex newPickupIndex, bool newHidden = false)
		{
			if (this.pickupIndex != newPickupIndex || this.hidden != newHidden)
			{
				this.NetworkpickupIndex = newPickupIndex;
				this.Networkhidden = newHidden;
				this.UpdatePickupDisplayAndAnimations();
			}
		}

		// Token: 0x06001567 RID: 5479 RVA: 0x00066AB8 File Offset: 0x00064CB8
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

		// Token: 0x06001568 RID: 5480 RVA: 0x00066B4A File Offset: 0x00064D4A
		public PickupIndex CurrentPickupIndex()
		{
			return this.pickupIndex;
		}

		// Token: 0x06001569 RID: 5481 RVA: 0x00066B52 File Offset: 0x00064D52
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

		// Token: 0x0600156A RID: 5482 RVA: 0x00066B78 File Offset: 0x00064D78
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

		// Token: 0x0600156C RID: 5484 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x0600156D RID: 5485 RVA: 0x00066BF0 File Offset: 0x00064DF0
		// (set) Token: 0x0600156E RID: 5486 RVA: 0x00066C03 File Offset: 0x00064E03
		public PickupIndex NetworkpickupIndex
		{
			get
			{
				return this.pickupIndex;
			}
			set
			{
				uint dirtyBit = 1u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncPickupIndex(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<PickupIndex>(value, ref this.pickupIndex, dirtyBit);
			}
		}

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x0600156F RID: 5487 RVA: 0x00066C44 File Offset: 0x00064E44
		// (set) Token: 0x06001570 RID: 5488 RVA: 0x00066C57 File Offset: 0x00064E57
		public bool Networkhidden
		{
			get
			{
				return this.hidden;
			}
			set
			{
				uint dirtyBit = 2u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncHidden(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<bool>(value, ref this.hidden, dirtyBit);
			}
		}

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x06001571 RID: 5489 RVA: 0x00066C98 File Offset: 0x00064E98
		// (set) Token: 0x06001572 RID: 5490 RVA: 0x00066CAB File Offset: 0x00064EAB
		public bool NetworkhasBeenPurchased
		{
			get
			{
				return this.hasBeenPurchased;
			}
			set
			{
				uint dirtyBit = 4u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.SetHasBeenPurchased(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<bool>(value, ref this.hasBeenPurchased, dirtyBit);
			}
		}

		// Token: 0x06001573 RID: 5491 RVA: 0x00066CEC File Offset: 0x00064EEC
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
			if ((base.syncVarDirtyBits & 1u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				GeneratedNetworkCode._WritePickupIndex_None(writer, this.pickupIndex);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.hidden);
			}
			if ((base.syncVarDirtyBits & 4u) != 0u)
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

		// Token: 0x06001574 RID: 5492 RVA: 0x00066DD8 File Offset: 0x00064FD8
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

		// Token: 0x040018B5 RID: 6325
		[SyncVar(hook = "OnSyncPickupIndex")]
		private PickupIndex pickupIndex = PickupIndex.none;

		// Token: 0x040018B6 RID: 6326
		[SyncVar(hook = "OnSyncHidden")]
		private bool hidden;

		// Token: 0x040018B7 RID: 6327
		[SyncVar(hook = "SetHasBeenPurchased")]
		private bool hasBeenPurchased;

		// Token: 0x040018B8 RID: 6328
		[Tooltip("The PickupDisplay component that should show which item this shop terminal is offering.")]
		public PickupDisplay pickupDisplay;

		// Token: 0x040018B9 RID: 6329
		[Tooltip("The position from which the drop will be emitted")]
		public Transform dropTransform;

		// Token: 0x040018BA RID: 6330
		[Tooltip("Whether or not the shop terminal shouldd drive itself")]
		public bool selfGeneratePickup;

		// Token: 0x040018BB RID: 6331
		[Tooltip("The tier of items to drop - only works if the pickup generates itself")]
		public ItemTier itemTier;

		// Token: 0x040018BC RID: 6332
		[Tooltip("The velocity with which the drop will be emitted. Rotates with this object.")]
		public Vector3 dropVelocity;

		// Token: 0x040018BD RID: 6333
		public Animator animator;
	}
}
