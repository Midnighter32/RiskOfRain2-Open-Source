using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x020005F0 RID: 1520
	[RequireComponent(typeof(RectTransform))]
	public class ItemInventoryDisplay : UIBehaviour
	{
		// Token: 0x06002211 RID: 8721 RVA: 0x000A1194 File Offset: 0x0009F394
		public void SetSubscribedInventory([CanBeNull] Inventory newInventory)
		{
			if (this.inventory == newInventory && this.inventory == this.inventoryWasValid)
			{
				return;
			}
			if (this.inventory != null)
			{
				this.inventory.onInventoryChanged -= this.OnInventoryChanged;
				this.inventory = null;
			}
			this.inventory = newInventory;
			this.inventoryWasValid = this.inventory;
			if (this.inventory)
			{
				this.inventory.onInventoryChanged += this.OnInventoryChanged;
			}
			this.OnInventoryChanged();
		}

		// Token: 0x06002212 RID: 8722 RVA: 0x000A1228 File Offset: 0x0009F428
		private void OnInventoryChanged()
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (this.inventory)
			{
				this.inventory.WriteItemStacks(this.itemStacks);
				this.inventory.itemAcquisitionOrder.CopyTo(this.itemOrder);
				this.itemOrderCount = this.inventory.itemAcquisitionOrder.Count;
			}
			else
			{
				Array.Clear(this.itemStacks, 0, this.itemStacks.Length);
				this.itemOrderCount = 0;
			}
			this.RequestUpdateDisplay();
		}

		// Token: 0x06002213 RID: 8723 RVA: 0x000A12AC File Offset: 0x0009F4AC
		private static bool ItemIsVisible(ItemIndex itemIndex)
		{
			ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
			return itemDef != null && !itemDef.hidden;
		}

		// Token: 0x06002214 RID: 8724 RVA: 0x000A12D0 File Offset: 0x0009F4D0
		private void AllocateIcons(int desiredItemCount)
		{
			if (desiredItemCount != this.itemIcons.Count)
			{
				while (this.itemIcons.Count > desiredItemCount)
				{
					UnityEngine.Object.Destroy(this.itemIcons[this.itemIcons.Count - 1].gameObject);
					this.itemIcons.RemoveAt(this.itemIcons.Count - 1);
				}
				while (this.itemIcons.Count < desiredItemCount)
				{
					ItemIcon component = UnityEngine.Object.Instantiate<GameObject>(this.itemIconPrefab, this.rectTransform).GetComponent<ItemIcon>();
					this.itemIcons.Add(component);
					this.LayoutIndividualIcon(this.itemIcons.Count - 1);
				}
			}
			this.OnIconCountChanged();
		}

		// Token: 0x06002215 RID: 8725 RVA: 0x000A1384 File Offset: 0x0009F584
		private float CalculateIconScale(int iconCount)
		{
			int num = (int)this.rectTransform.rect.width;
			int num2 = (int)this.maxHeight;
			int num3 = (int)this.iconWidth;
			int num4 = num3;
			int num5 = num3 / 8;
			int num6 = Math.Max(num / num4, 1);
			int num7 = HGMath.IntDivCeil(iconCount, num6);
			while (num4 * num7 > num2)
			{
				num6++;
				num4 = Math.Min(num / num6, num3);
				num7 = HGMath.IntDivCeil(iconCount, num6);
				if (num4 <= num5)
				{
					num4 = num5;
					break;
				}
			}
			return (float)num4 / (float)num3;
		}

		// Token: 0x06002216 RID: 8726 RVA: 0x000A1408 File Offset: 0x0009F608
		private void OnIconCountChanged()
		{
			float num = this.CalculateIconScale(this.itemIcons.Count);
			if (num != this.currentIconScale)
			{
				this.currentIconScale = num;
				this.OnIconScaleChanged();
			}
		}

		// Token: 0x06002217 RID: 8727 RVA: 0x000A143D File Offset: 0x0009F63D
		private void OnIconScaleChanged()
		{
			this.LayoutAllIcons();
		}

		// Token: 0x06002218 RID: 8728 RVA: 0x000A1448 File Offset: 0x0009F648
		private void CalculateLayoutValues(out ItemInventoryDisplay.LayoutValues v)
		{
			Rect rect = this.rectTransform.rect;
			v.width = rect.width;
			v.iconSize = this.iconWidth * this.currentIconScale;
			v.iconsPerRow = Math.Max((int)v.width / (int)v.iconSize, 1);
			v.rowWidth = (float)v.iconsPerRow * v.iconSize;
			float num = (v.width - v.rowWidth) * 0.5f;
			v.rowCount = HGMath.IntDivCeil(this.itemIcons.Count, v.iconsPerRow);
			v.iconLocalScale = new Vector3(this.currentIconScale, this.currentIconScale, 1f);
			v.topLeftCorner = new Vector3(rect.xMin + num, rect.yMax);
		}

		// Token: 0x06002219 RID: 8729 RVA: 0x000A1518 File Offset: 0x0009F718
		private void LayoutAllIcons()
		{
			ItemInventoryDisplay.LayoutValues layoutValues;
			this.CalculateLayoutValues(out layoutValues);
			int num = this.itemIcons.Count - (layoutValues.rowCount - 1) * layoutValues.iconsPerRow;
			int i = 0;
			int num2 = 0;
			while (i < layoutValues.rowCount)
			{
				Vector3 topLeftCorner = layoutValues.topLeftCorner;
				topLeftCorner.y += (float)i * -layoutValues.iconSize;
				int num3 = layoutValues.iconsPerRow;
				if (i == layoutValues.rowCount - 1)
				{
					num3 = num;
				}
				int j = 0;
				while (j < num3)
				{
					RectTransform rectTransform = this.itemIcons[num2].rectTransform;
					rectTransform.localScale = layoutValues.iconLocalScale;
					rectTransform.localPosition = topLeftCorner;
					topLeftCorner.x += layoutValues.iconSize;
					j++;
					num2++;
				}
				i++;
			}
		}

		// Token: 0x0600221A RID: 8730 RVA: 0x000A15E0 File Offset: 0x0009F7E0
		private void LayoutIndividualIcon(int i)
		{
			ItemInventoryDisplay.LayoutValues layoutValues;
			this.CalculateLayoutValues(out layoutValues);
			int num = i / layoutValues.iconsPerRow;
			int num2 = i - num * layoutValues.iconsPerRow;
			Vector3 topLeftCorner = layoutValues.topLeftCorner;
			topLeftCorner.x += (float)num2 * layoutValues.iconSize;
			topLeftCorner.y += (float)num * -layoutValues.iconSize;
			RectTransform rectTransform = this.itemIcons[i].rectTransform;
			rectTransform.localPosition = topLeftCorner;
			rectTransform.localScale = layoutValues.iconLocalScale;
		}

		// Token: 0x0600221B RID: 8731 RVA: 0x000A165C File Offset: 0x0009F85C
		protected override void Awake()
		{
			base.Awake();
			this.rectTransform = (RectTransform)base.transform;
			this.itemStacks = ItemCatalog.RequestItemStackArray();
			this.itemOrder = ItemCatalog.RequestItemOrderBuffer();
		}

		// Token: 0x0600221C RID: 8732 RVA: 0x000A168B File Offset: 0x0009F88B
		protected override void OnDestroy()
		{
			this.SetSubscribedInventory(null);
			base.OnDestroy();
		}

		// Token: 0x0600221D RID: 8733 RVA: 0x000A169A File Offset: 0x0009F89A
		protected override void OnEnable()
		{
			base.OnEnable();
			if (this.inventory)
			{
				this.OnInventoryChanged();
			}
			this.RequestUpdateDisplay();
			this.LayoutAllIcons();
		}

		// Token: 0x0600221E RID: 8734 RVA: 0x000A16C1 File Offset: 0x0009F8C1
		private void RequestUpdateDisplay()
		{
			if (!this.updateRequestPending)
			{
				this.updateRequestPending = true;
				RoR2Application.onNextUpdate += this.UpdateDisplay;
			}
		}

		// Token: 0x0600221F RID: 8735 RVA: 0x000A16E4 File Offset: 0x0009F8E4
		public void UpdateDisplay()
		{
			this.updateRequestPending = false;
			if (!this || !base.isActiveAndEnabled)
			{
				return;
			}
			ItemIndex[] array = ItemCatalog.RequestItemOrderBuffer();
			int num = 0;
			for (int i = 0; i < this.itemOrderCount; i++)
			{
				if (ItemInventoryDisplay.ItemIsVisible(this.itemOrder[i]))
				{
					array[num++] = this.itemOrder[i];
				}
			}
			this.AllocateIcons(num);
			for (int j = 0; j < num; j++)
			{
				ItemIndex itemIndex = array[j];
				this.itemIcons[j].SetItemIndex(itemIndex, this.itemStacks[(int)itemIndex]);
			}
			ItemCatalog.ReturnItemOrderBuffer(array);
		}

		// Token: 0x06002220 RID: 8736 RVA: 0x000A177C File Offset: 0x0009F97C
		public void SetItems(List<ItemIndex> newItemOrder, uint[] newItemStacks)
		{
			this.itemOrderCount = newItemOrder.Count;
			for (int i = 0; i < this.itemOrderCount; i++)
			{
				this.itemOrder[i] = newItemOrder[i];
			}
			Array.Copy(newItemStacks, this.itemStacks, this.itemStacks.Length);
			this.RequestUpdateDisplay();
		}

		// Token: 0x06002221 RID: 8737 RVA: 0x000A17CF File Offset: 0x0009F9CF
		public void SetItems(ItemIndex[] newItemOrder, int newItemOrderCount, int[] newItemStacks)
		{
			this.itemOrderCount = newItemOrderCount;
			Array.Copy(newItemOrder, this.itemOrder, this.itemOrderCount);
			Array.Copy(newItemStacks, this.itemStacks, this.itemStacks.Length);
			this.RequestUpdateDisplay();
		}

		// Token: 0x06002222 RID: 8738 RVA: 0x000A1804 File Offset: 0x0009FA04
		public void ResetItems()
		{
			this.itemOrderCount = 0;
			Array.Clear(this.itemStacks, 0, this.itemStacks.Length);
			this.RequestUpdateDisplay();
		}

		// Token: 0x06002223 RID: 8739 RVA: 0x000A1828 File Offset: 0x0009FA28
		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();
			if (this.rectTransform)
			{
				float width = this.rectTransform.rect.width;
				if (width != this.previousWidth)
				{
					this.previousWidth = width;
					this.LayoutAllIcons();
				}
			}
		}

		// Token: 0x04002519 RID: 9497
		private RectTransform rectTransform;

		// Token: 0x0400251A RID: 9498
		public GameObject itemIconPrefab;

		// Token: 0x0400251B RID: 9499
		public float iconWidth = 64f;

		// Token: 0x0400251C RID: 9500
		public float maxHeight = 128f;

		// Token: 0x0400251D RID: 9501
		[HideInInspector]
		[SerializeField]
		private List<ItemIcon> itemIcons;

		// Token: 0x0400251E RID: 9502
		private ItemIndex[] itemOrder;

		// Token: 0x0400251F RID: 9503
		private int itemOrderCount;

		// Token: 0x04002520 RID: 9504
		private int[] itemStacks;

		// Token: 0x04002521 RID: 9505
		private float currentIconScale = 1f;

		// Token: 0x04002522 RID: 9506
		private float previousWidth;

		// Token: 0x04002523 RID: 9507
		private bool updateRequestPending;

		// Token: 0x04002524 RID: 9508
		private Inventory inventory;

		// Token: 0x04002525 RID: 9509
		private bool inventoryWasValid;

		// Token: 0x020005F1 RID: 1521
		private struct LayoutValues
		{
			// Token: 0x04002526 RID: 9510
			public float width;

			// Token: 0x04002527 RID: 9511
			public float iconSize;

			// Token: 0x04002528 RID: 9512
			public int iconsPerRow;

			// Token: 0x04002529 RID: 9513
			public float rowWidth;

			// Token: 0x0400252A RID: 9514
			public int rowCount;

			// Token: 0x0400252B RID: 9515
			public Vector3 iconLocalScale;

			// Token: 0x0400252C RID: 9516
			public Vector3 topLeftCorner;
		}
	}
}
