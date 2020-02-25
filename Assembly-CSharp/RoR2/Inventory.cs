using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000260 RID: 608
	public class Inventory : NetworkBehaviour
	{
		// Token: 0x1400001E RID: 30
		// (add) Token: 0x06000D4F RID: 3407 RVA: 0x0003BD58 File Offset: 0x00039F58
		// (remove) Token: 0x06000D50 RID: 3408 RVA: 0x0003BD90 File Offset: 0x00039F90
		public event Action onInventoryChanged;

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x06000D51 RID: 3409 RVA: 0x0003BDC5 File Offset: 0x00039FC5
		public EquipmentIndex currentEquipmentIndex
		{
			get
			{
				return this.currentEquipmentState.equipmentIndex;
			}
		}

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x06000D52 RID: 3410 RVA: 0x0003BDD2 File Offset: 0x00039FD2
		public EquipmentState currentEquipmentState
		{
			get
			{
				return this.GetEquipment((uint)this.activeEquipmentSlot);
			}
		}

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x06000D53 RID: 3411 RVA: 0x0003BDE0 File Offset: 0x00039FE0
		public EquipmentIndex alternateEquipmentIndex
		{
			get
			{
				return this.alternateEquipmentState.equipmentIndex;
			}
		}

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x06000D54 RID: 3412 RVA: 0x0003BDF0 File Offset: 0x00039FF0
		public EquipmentState alternateEquipmentState
		{
			get
			{
				uint num = 0U;
				while ((ulong)num < (ulong)((long)this.GetEquipmentSlotCount()))
				{
					if (num != (uint)this.activeEquipmentSlot)
					{
						return this.GetEquipment(num);
					}
					num += 1U;
				}
				return EquipmentState.empty;
			}
		}

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x06000D55 RID: 3413 RVA: 0x0003BE26 File Offset: 0x0003A026
		// (set) Token: 0x06000D56 RID: 3414 RVA: 0x0003BE2E File Offset: 0x0003A02E
		public byte activeEquipmentSlot { get; private set; }

		// Token: 0x06000D57 RID: 3415 RVA: 0x0003BE38 File Offset: 0x0003A038
		private bool SetEquipmentInternal(EquipmentState equipmentState, uint slot)
		{
			if ((long)this.equipmentStateSlots.Length <= (long)((ulong)slot))
			{
				int num = this.equipmentStateSlots.Length;
				Array.Resize<EquipmentState>(ref this.equipmentStateSlots, (int)(slot + 1U));
				for (int i = num; i < this.equipmentStateSlots.Length; i++)
				{
					this.equipmentStateSlots[i] = EquipmentState.empty;
				}
			}
			if (this.equipmentStateSlots[(int)slot].Equals(equipmentState))
			{
				return false;
			}
			this.equipmentStateSlots[(int)slot] = equipmentState;
			return true;
		}

		// Token: 0x06000D58 RID: 3416 RVA: 0x0003BEAF File Offset: 0x0003A0AF
		[Server]
		public void SetEquipment(EquipmentState equipmentState, uint slot)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::SetEquipment(RoR2.EquipmentState,System.UInt32)' called on client");
				return;
			}
			if (this.SetEquipmentInternal(equipmentState, slot))
			{
				if (NetworkServer.active)
				{
					base.SetDirtyBit(16U);
				}
				Action action = this.onInventoryChanged;
				if (action == null)
				{
					return;
				}
				action();
			}
		}

		// Token: 0x06000D59 RID: 3417 RVA: 0x0003BEEF File Offset: 0x0003A0EF
		public EquipmentState GetEquipment(uint slot)
		{
			if ((ulong)slot >= (ulong)((long)this.equipmentStateSlots.Length))
			{
				return EquipmentState.empty;
			}
			return this.equipmentStateSlots[(int)slot];
		}

		// Token: 0x06000D5A RID: 3418 RVA: 0x0003BF10 File Offset: 0x0003A110
		[Server]
		public void SetActiveEquipmentSlot(byte slotIndex)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::SetActiveEquipmentSlot(System.Byte)' called on client");
				return;
			}
			this.activeEquipmentSlot = slotIndex;
			base.SetDirtyBit(16U);
			Action action = this.onInventoryChanged;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06000D5B RID: 3419 RVA: 0x0003BF46 File Offset: 0x0003A146
		public int GetEquipmentSlotCount()
		{
			return this.equipmentStateSlots.Length;
		}

		// Token: 0x06000D5C RID: 3420 RVA: 0x0003BF50 File Offset: 0x0003A150
		[Server]
		public void SetEquipmentIndex(EquipmentIndex newEquipmentIndex)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::SetEquipmentIndex(RoR2.EquipmentIndex)' called on client");
				return;
			}
			if (this.currentEquipmentIndex != newEquipmentIndex)
			{
				EquipmentState equipment = this.GetEquipment((uint)this.activeEquipmentSlot);
				byte charges = equipment.charges;
				Run.FixedTimeStamp chargeFinishTime = equipment.chargeFinishTime;
				if (equipment.equipmentIndex == EquipmentIndex.None && chargeFinishTime.isNegativeInfinity)
				{
					charges = 1;
					chargeFinishTime = Run.FixedTimeStamp.now;
				}
				EquipmentState equipmentState = new EquipmentState(newEquipmentIndex, chargeFinishTime, charges);
				this.SetEquipment(equipmentState, (uint)this.activeEquipmentSlot);
			}
		}

		// Token: 0x06000D5D RID: 3421 RVA: 0x0003BFC5 File Offset: 0x0003A1C5
		public EquipmentIndex GetEquipmentIndex()
		{
			return this.currentEquipmentIndex;
		}

		// Token: 0x06000D5E RID: 3422 RVA: 0x0003BFD0 File Offset: 0x0003A1D0
		[Server]
		public void DeductEquipmentCharges(byte slot, int deduction)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::DeductEquipmentCharges(System.Byte,System.Int32)' called on client");
				return;
			}
			EquipmentState equipment = this.GetEquipment((uint)slot);
			byte b = equipment.charges;
			if ((int)b < deduction)
			{
				b = 0;
			}
			else
			{
				b -= (byte)deduction;
			}
			this.SetEquipment(new EquipmentState(equipment.equipmentIndex, equipment.chargeFinishTime, b), (uint)slot);
		}

		// Token: 0x06000D5F RID: 3423 RVA: 0x0003C028 File Offset: 0x0003A228
		[Server]
		public void DeductActiveEquipmentCooldown(float seconds)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::DeductActiveEquipmentCooldown(System.Single)' called on client");
				return;
			}
			EquipmentState equipment = this.GetEquipment((uint)this.activeEquipmentSlot);
			this.SetEquipment(new EquipmentState(equipment.equipmentIndex, equipment.chargeFinishTime - seconds, equipment.charges), (uint)this.activeEquipmentSlot);
		}

		// Token: 0x06000D60 RID: 3424 RVA: 0x0003C080 File Offset: 0x0003A280
		public int GetActiveEquipmentMaxCharges()
		{
			return 1 + this.GetItemCount(ItemIndex.EquipmentMagazine);
		}

		// Token: 0x06000D61 RID: 3425 RVA: 0x0003C08C File Offset: 0x0003A28C
		[Server]
		private void UpdateEquipment()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::UpdateEquipment()' called on client");
				return;
			}
			Run.FixedTimeStamp now = Run.FixedTimeStamp.now;
			int itemCount = this.GetItemCount(ItemIndex.EquipmentMagazine);
			byte b = (byte)Mathf.Min(1 + this.GetItemCount(ItemIndex.EquipmentMagazine), 255);
			uint num = 0U;
			while ((ulong)num < (ulong)((long)this.equipmentStateSlots.Length))
			{
				EquipmentState equipmentState = this.equipmentStateSlots[(int)num];
				if (equipmentState.equipmentIndex != EquipmentIndex.None)
				{
					if (equipmentState.charges < b)
					{
						Run.FixedTimeStamp a = equipmentState.chargeFinishTime;
						byte b2 = equipmentState.charges;
						if (!a.isPositiveInfinity)
						{
							b2 += 1;
						}
						if (a.isInfinity)
						{
							a = now;
						}
						if (a.hasPassed)
						{
							int itemCount2 = this.GetItemCount(ItemIndex.AutoCastEquipment);
							float num2 = Mathf.Pow(0.85f, (float)itemCount);
							if (itemCount2 > 0)
							{
								num2 *= 0.5f * Mathf.Pow(0.85f, (float)(itemCount2 - 1));
							}
							float b3 = equipmentState.equipmentDef.cooldown * num2;
							this.SetEquipment(new EquipmentState(equipmentState.equipmentIndex, a + b3, b2), num);
						}
					}
					if (equipmentState.charges >= b && !equipmentState.chargeFinishTime.isPositiveInfinity)
					{
						this.SetEquipment(new EquipmentState(equipmentState.equipmentIndex, Run.FixedTimeStamp.positiveInfinity, b), num);
					}
				}
				num += 1U;
			}
		}

		// Token: 0x06000D62 RID: 3426 RVA: 0x0003C1E8 File Offset: 0x0003A3E8
		private void Start()
		{
			if (NetworkServer.active && Run.instance.enabledArtifacts.HasArtifact(ArtifactIndex.Enigma))
			{
				this.SetEquipmentIndex(EquipmentIndex.Enigma);
			}
		}

		// Token: 0x06000D63 RID: 3427 RVA: 0x0003C20B File Offset: 0x0003A40B
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.UpdateEquipment();
			}
		}

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x06000D64 RID: 3428 RVA: 0x0003C21A File Offset: 0x0003A41A
		// (set) Token: 0x06000D65 RID: 3429 RVA: 0x0003C222 File Offset: 0x0003A422
		public uint infusionBonus { get; private set; }

		// Token: 0x06000D66 RID: 3430 RVA: 0x0003C22B File Offset: 0x0003A42B
		[Server]
		public void AddInfusionBonus(uint value)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::AddInfusionBonus(System.UInt32)' called on client");
				return;
			}
			if (value != 0U)
			{
				this.infusionBonus += value;
				base.SetDirtyBit(4U);
				Action action = this.onInventoryChanged;
				if (action == null)
				{
					return;
				}
				action();
			}
		}

		// Token: 0x06000D67 RID: 3431 RVA: 0x0003C26A File Offset: 0x0003A46A
		[Server]
		public void GiveItemString(string itemString)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::GiveItemString(System.String)' called on client");
				return;
			}
			this.GiveItem(ItemCatalog.FindItemIndex(itemString), 1);
		}

		// Token: 0x06000D68 RID: 3432 RVA: 0x0003C28E File Offset: 0x0003A48E
		[Server]
		public void GiveEquipmentString(string equipmentString)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::GiveEquipmentString(System.String)' called on client");
				return;
			}
			this.SetEquipmentIndex(EquipmentCatalog.FindEquipmentIndex(equipmentString));
		}

		// Token: 0x06000D69 RID: 3433 RVA: 0x0003C2B4 File Offset: 0x0003A4B4
		[Server]
		public void GiveRandomItems(int count)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::GiveRandomItems(System.Int32)' called on client");
				return;
			}
			try
			{
				if (count > 0)
				{
					WeightedSelection<List<PickupIndex>> weightedSelection = new WeightedSelection<List<PickupIndex>>(8);
					weightedSelection.AddChoice(Run.instance.availableTier1DropList, 80f);
					weightedSelection.AddChoice(Run.instance.availableTier2DropList, 19f);
					weightedSelection.AddChoice(Run.instance.availableTier3DropList, 1f);
					for (int i = 0; i < count; i++)
					{
						List<PickupIndex> list = weightedSelection.Evaluate(UnityEngine.Random.value);
						this.GiveItem(list[UnityEngine.Random.Range(0, list.Count)].itemIndex, 1);
					}
				}
			}
			catch (ArgumentException)
			{
			}
		}

		// Token: 0x06000D6A RID: 3434 RVA: 0x0003C370 File Offset: 0x0003A570
		[Server]
		public void GiveRandomEquipment()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::GiveRandomEquipment()' called on client");
				return;
			}
			this.SetEquipmentIndex(Run.instance.availableEquipmentDropList[UnityEngine.Random.Range(0, Run.instance.availableEquipmentDropList.Count)].equipmentIndex);
		}

		// Token: 0x06000D6B RID: 3435 RVA: 0x0003C3C4 File Offset: 0x0003A5C4
		[Server]
		public void GiveItem(ItemIndex itemIndex, int count = 1)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::GiveItem(RoR2.ItemIndex,System.Int32)' called on client");
				return;
			}
			if (count <= 0)
			{
				if (count < 0)
				{
					this.RemoveItem(itemIndex, -count);
				}
				return;
			}
			base.SetDirtyBit(1U);
			if ((this.itemStacks[(int)itemIndex] += count) == count)
			{
				this.itemAcquisitionOrder.Add(itemIndex);
				base.SetDirtyBit(8U);
			}
			Action action = this.onInventoryChanged;
			if (action != null)
			{
				action();
			}
			Action<Inventory, ItemIndex, int> action2 = Inventory.onServerItemGiven;
			if (action2 != null)
			{
				action2(this, itemIndex, count);
			}
			if (this.spawnedOverNetwork)
			{
				this.CallRpcItemAdded(itemIndex);
			}
		}

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x06000D6C RID: 3436 RVA: 0x0003C45C File Offset: 0x0003A65C
		private bool spawnedOverNetwork
		{
			get
			{
				return base.isServer;
			}
		}

		// Token: 0x1400001F RID: 31
		// (add) Token: 0x06000D6D RID: 3437 RVA: 0x0003C464 File Offset: 0x0003A664
		// (remove) Token: 0x06000D6E RID: 3438 RVA: 0x0003C498 File Offset: 0x0003A698
		public static event Action<Inventory, ItemIndex, int> onServerItemGiven;

		// Token: 0x06000D6F RID: 3439 RVA: 0x0003C4CB File Offset: 0x0003A6CB
		private IEnumerator HighlightNewItem(ItemIndex itemIndex)
		{
			yield return new WaitForSeconds(0.05f);
			CharacterMaster component = base.GetComponent<CharacterMaster>();
			if (component)
			{
				GameObject bodyObject = component.GetBodyObject();
				if (bodyObject)
				{
					ModelLocator component2 = bodyObject.GetComponent<ModelLocator>();
					if (component2)
					{
						Transform modelTransform = component2.modelTransform;
						if (modelTransform)
						{
							CharacterModel component3 = modelTransform.GetComponent<CharacterModel>();
							if (component3)
							{
								component3.HighlightItemDisplay(itemIndex);
							}
						}
					}
				}
			}
			yield break;
		}

		// Token: 0x06000D70 RID: 3440 RVA: 0x0003C4E1 File Offset: 0x0003A6E1
		[ClientRpc]
		private void RpcItemAdded(ItemIndex itemIndex)
		{
			base.StartCoroutine(this.HighlightNewItem(itemIndex));
		}

		// Token: 0x06000D71 RID: 3441 RVA: 0x0003C4F4 File Offset: 0x0003A6F4
		[Server]
		public void RemoveItem(ItemIndex itemIndex, int count = 1)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::RemoveItem(RoR2.ItemIndex,System.Int32)' called on client");
				return;
			}
			if (count <= 0)
			{
				if (count < 0)
				{
					this.GiveItem(itemIndex, -count);
				}
				return;
			}
			int num = this.itemStacks[(int)itemIndex];
			count = Math.Min(count, num);
			if (count == 0)
			{
				return;
			}
			if ((this.itemStacks[(int)itemIndex] = num - count) == 0)
			{
				this.itemAcquisitionOrder.Remove(itemIndex);
				base.SetDirtyBit(8U);
			}
			base.SetDirtyBit(1U);
			Action action = this.onInventoryChanged;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06000D72 RID: 3442 RVA: 0x0003C57C File Offset: 0x0003A77C
		[Server]
		public void ResetItem(ItemIndex itemIndex)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::ResetItem(RoR2.ItemIndex)' called on client");
				return;
			}
			if (this.itemStacks[(int)itemIndex] <= 0)
			{
				return;
			}
			this.itemStacks[(int)itemIndex] = 0;
			base.SetDirtyBit(1U);
			base.SetDirtyBit(8U);
			Action action = this.onInventoryChanged;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06000D73 RID: 3443 RVA: 0x0003C5D4 File Offset: 0x0003A7D4
		[Server]
		public void CopyEquipmentFrom(Inventory other)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::CopyEquipmentFrom(RoR2.Inventory)' called on client");
				return;
			}
			for (int i = 0; i < other.equipmentStateSlots.Length; i++)
			{
				this.SetEquipment(new EquipmentState(other.equipmentStateSlots[i].equipmentIndex, Run.FixedTimeStamp.negativeInfinity, 1), (uint)i);
			}
		}

		// Token: 0x06000D74 RID: 3444 RVA: 0x0003C62C File Offset: 0x0003A82C
		[Server]
		public void CopyItemsFrom(Inventory other)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::CopyItemsFrom(RoR2.Inventory)' called on client");
				return;
			}
			other.itemStacks.CopyTo(this.itemStacks, 0);
			this.itemAcquisitionOrder.Clear();
			this.itemAcquisitionOrder.AddRange(other.itemAcquisitionOrder);
			base.SetDirtyBit(1U);
			base.SetDirtyBit(8U);
			Action action = this.onInventoryChanged;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06000D75 RID: 3445 RVA: 0x0003C69C File Offset: 0x0003A89C
		[Server]
		public void ShrineRestackInventory([NotNull] Xoroshiro128Plus rng)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::ShrineRestackInventory(Xoroshiro128Plus)' called on client");
				return;
			}
			List<ItemIndex> list = new List<ItemIndex>();
			List<ItemIndex> list2 = new List<ItemIndex>();
			List<ItemIndex> list3 = new List<ItemIndex>();
			List<ItemIndex> list4 = new List<ItemIndex>();
			List<ItemIndex> list5 = new List<ItemIndex>();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			for (int i = 0; i < this.itemStacks.Length; i++)
			{
				ItemIndex itemIndex = (ItemIndex)i;
				if (this.itemStacks[i] > 0)
				{
					switch (ItemCatalog.GetItemDef(itemIndex).tier)
					{
					case ItemTier.Tier1:
						num += this.itemStacks[i];
						list.Add(itemIndex);
						break;
					case ItemTier.Tier2:
						num2 += this.itemStacks[i];
						list2.Add(itemIndex);
						break;
					case ItemTier.Tier3:
						num3 += this.itemStacks[i];
						list3.Add(itemIndex);
						break;
					case ItemTier.Lunar:
						num4 += this.itemStacks[i];
						list4.Add(itemIndex);
						break;
					case ItemTier.Boss:
						num5 += this.itemStacks[i];
						list5.Add(itemIndex);
						break;
					default:
						goto IL_12C;
					}
					this.itemAcquisitionOrder.Remove(itemIndex);
					this.ResetItem(itemIndex);
				}
				IL_12C:;
			}
			ItemIndex itemIndex2 = (list.Count == 0) ? ItemIndex.None : rng.NextElementUniform<ItemIndex>(list);
			ItemIndex itemIndex3 = (list2.Count == 0) ? ItemIndex.None : rng.NextElementUniform<ItemIndex>(list2);
			ItemIndex itemIndex4 = (list3.Count == 0) ? ItemIndex.None : rng.NextElementUniform<ItemIndex>(list3);
			ItemIndex itemIndex5 = (list4.Count == 0) ? ItemIndex.None : rng.NextElementUniform<ItemIndex>(list4);
			ItemIndex itemIndex6 = (list5.Count == 0) ? ItemIndex.None : rng.NextElementUniform<ItemIndex>(list5);
			this.GiveItem(itemIndex2, num);
			this.GiveItem(itemIndex3, num2);
			this.GiveItem(itemIndex4, num3);
			this.GiveItem(itemIndex5, num4);
			this.GiveItem(itemIndex6, num5);
			base.SetDirtyBit(8U);
		}

		// Token: 0x06000D76 RID: 3446 RVA: 0x0003C889 File Offset: 0x0003AA89
		public int GetItemCount(ItemIndex itemIndex)
		{
			return this.itemStacks[(int)itemIndex];
		}

		// Token: 0x06000D77 RID: 3447 RVA: 0x0003C894 File Offset: 0x0003AA94
		public bool HasAtLeastXTotalItemsOfTier(ItemTier itemTier, int x)
		{
			int num = 0;
			ItemIndex itemIndex = ItemIndex.Syringe;
			ItemIndex itemCount = (ItemIndex)ItemCatalog.itemCount;
			while (itemIndex < itemCount)
			{
				if (ItemCatalog.GetItemDef(itemIndex).tier == itemTier)
				{
					num += this.GetItemCount(itemIndex);
					if (num >= x)
					{
						return true;
					}
				}
				itemIndex++;
			}
			return false;
		}

		// Token: 0x06000D78 RID: 3448 RVA: 0x0003C8D4 File Offset: 0x0003AAD4
		public int GetTotalItemCountOfTier(ItemTier itemTier)
		{
			int num = 0;
			ItemIndex itemIndex = ItemIndex.Syringe;
			ItemIndex itemCount = (ItemIndex)ItemCatalog.itemCount;
			while (itemIndex < itemCount)
			{
				if (ItemCatalog.GetItemDef(itemIndex).tier == itemTier)
				{
					num += this.GetItemCount(itemIndex);
				}
				itemIndex++;
			}
			return num;
		}

		// Token: 0x06000D79 RID: 3449 RVA: 0x0003C90E File Offset: 0x0003AB0E
		public void WriteItemStacks(int[] output)
		{
			Array.Copy(this.itemStacks, output, output.Length);
		}

		// Token: 0x06000D7A RID: 3450 RVA: 0x00019B5A File Offset: 0x00017D5A
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x06000D7B RID: 3451 RVA: 0x0003C920 File Offset: 0x0003AB20
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			byte b = reader.ReadByte();
			bool flag = (b & 1) > 0;
			bool flag2 = (b & 4) > 0;
			bool flag3 = (b & 8) > 0;
			bool flag4 = (b & 16) > 0;
			if (flag)
			{
				reader.ReadItemStacks(this.itemStacks);
			}
			if (flag2)
			{
				this.infusionBonus = reader.ReadPackedUInt32();
			}
			if (flag3)
			{
				byte b2 = reader.ReadByte();
				this.itemAcquisitionOrder.Clear();
				this.itemAcquisitionOrder.Capacity = (int)b2;
				for (byte b3 = 0; b3 < b2; b3 += 1)
				{
					ItemIndex item = (ItemIndex)reader.ReadByte();
					this.itemAcquisitionOrder.Add(item);
				}
			}
			if (flag4)
			{
				uint num = (uint)reader.ReadByte();
				for (uint num2 = 0U; num2 < num; num2 += 1U)
				{
					this.SetEquipmentInternal(EquipmentState.Deserialize(reader), num2);
				}
				this.activeEquipmentSlot = reader.ReadByte();
			}
			if (flag || flag4 || flag2)
			{
				Action action = this.onInventoryChanged;
				if (action == null)
				{
					return;
				}
				action();
			}
		}

		// Token: 0x06000D7C RID: 3452 RVA: 0x0003CA08 File Offset: 0x0003AC08
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint num = base.syncVarDirtyBits;
			if (initialState)
			{
				num = 29U;
			}
			for (int i = 0; i < this.equipmentStateSlots.Length; i++)
			{
				if (this.equipmentStateSlots[i].dirty)
				{
					num |= 16U;
					break;
				}
			}
			bool flag = (num & 1U) > 0U;
			bool flag2 = (num & 4U) > 0U;
			bool flag3 = (num & 8U) > 0U;
			bool flag4 = (num & 16U) > 0U;
			writer.Write((byte)num);
			if (flag)
			{
				writer.WriteItemStacks(this.itemStacks);
			}
			if (flag2)
			{
				writer.WritePackedUInt32(this.infusionBonus);
			}
			if (flag3)
			{
				byte b = (byte)this.itemAcquisitionOrder.Count;
				writer.Write(b);
				for (byte b2 = 0; b2 < b; b2 += 1)
				{
					writer.Write((byte)this.itemAcquisitionOrder[(int)b2]);
				}
			}
			if (flag4)
			{
				writer.Write((byte)this.equipmentStateSlots.Length);
				for (int j = 0; j < this.equipmentStateSlots.Length; j++)
				{
					EquipmentState.Serialize(writer, this.equipmentStateSlots[j]);
				}
				writer.Write(this.activeEquipmentSlot);
			}
			if (!initialState)
			{
				for (int k = 0; k < this.equipmentStateSlots.Length; k++)
				{
					this.equipmentStateSlots[k].dirty = false;
				}
			}
			return !initialState && num > 0U;
		}

		// Token: 0x06000D7E RID: 3454 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x06000D7F RID: 3455 RVA: 0x0003CB7B File Offset: 0x0003AD7B
		protected static void InvokeRpcRpcItemAdded(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcItemAdded called on server.");
				return;
			}
			((Inventory)obj).RpcItemAdded((ItemIndex)reader.ReadInt32());
		}

		// Token: 0x06000D80 RID: 3456 RVA: 0x0003CBA4 File Offset: 0x0003ADA4
		public void CallRpcItemAdded(ItemIndex itemIndex)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcItemAdded called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)Inventory.kRpcRpcItemAdded);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write((int)itemIndex);
			this.SendRPCInternal(networkWriter, 0, "RpcItemAdded");
		}

		// Token: 0x06000D81 RID: 3457 RVA: 0x0003CC17 File Offset: 0x0003AE17
		static Inventory()
		{
			NetworkBehaviour.RegisterRpcDelegate(typeof(Inventory), Inventory.kRpcRpcItemAdded, new NetworkBehaviour.CmdDelegate(Inventory.InvokeRpcRpcItemAdded));
			NetworkCRC.RegisterBehaviour("Inventory", 0);
		}

		// Token: 0x04000D81 RID: 3457
		private int[] itemStacks = ItemCatalog.RequestItemStackArray();

		// Token: 0x04000D82 RID: 3458
		public readonly List<ItemIndex> itemAcquisitionOrder = new List<ItemIndex>();

		// Token: 0x04000D83 RID: 3459
		private const uint itemListDirtyBit = 1U;

		// Token: 0x04000D84 RID: 3460
		private const uint infusionBonusDirtyBit = 4U;

		// Token: 0x04000D85 RID: 3461
		private const uint itemAcquisitionOrderDirtyBit = 8U;

		// Token: 0x04000D86 RID: 3462
		private const uint equipmentDirtyBit = 16U;

		// Token: 0x04000D87 RID: 3463
		private const uint allDirtyBits = 29U;

		// Token: 0x04000D8A RID: 3466
		private EquipmentState[] equipmentStateSlots = Array.Empty<EquipmentState>();

		// Token: 0x04000D8D RID: 3469
		private static int kRpcRpcItemAdded = 1978705787;
	}
}
