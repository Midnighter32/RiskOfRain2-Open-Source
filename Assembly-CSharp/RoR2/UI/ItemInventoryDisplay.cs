using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoR2.UI
{
	// Token: 0x020005D5 RID: 1493
	[RequireComponent(typeof(RectTransform))]
	public class ItemInventoryDisplay : UIBehaviour
	{
		// Token: 0x06002354 RID: 9044 RVA: 0x0009A5EC File Offset: 0x000987EC
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

		// Token: 0x06002355 RID: 9045 RVA: 0x0009A680 File Offset: 0x00098880
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

		// Token: 0x06002356 RID: 9046 RVA: 0x0009A704 File Offset: 0x00098904
		private static bool ItemIsVisible(ItemIndex itemIndex)
		{
			ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
			return itemDef != null && !itemDef.hidden;
		}

		// Token: 0x06002357 RID: 9047 RVA: 0x0009A728 File Offset: 0x00098928
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

		// Token: 0x06002358 RID: 9048 RVA: 0x0009A7DC File Offset: 0x000989DC
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

		// Token: 0x06002359 RID: 9049 RVA: 0x0009A860 File Offset: 0x00098A60
		private void OnIconCountChanged()
		{
			float num = this.CalculateIconScale(this.itemIcons.Count);
			if (num != this.currentIconScale)
			{
				this.currentIconScale = num;
				this.OnIconScaleChanged();
			}
		}

		// Token: 0x0600235A RID: 9050 RVA: 0x0009A895 File Offset: 0x00098A95
		private void OnIconScaleChanged()
		{
			this.LayoutAllIcons();
		}

		// Token: 0x0600235B RID: 9051 RVA: 0x0009A8A0 File Offset: 0x00098AA0
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

		// Token: 0x0600235C RID: 9052 RVA: 0x0009A970 File Offset: 0x00098B70
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

		// Token: 0x0600235D RID: 9053 RVA: 0x0009AA38 File Offset: 0x00098C38
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

		// Token: 0x0600235E RID: 9054 RVA: 0x0009AAB4 File Offset: 0x00098CB4
		protected override void Awake()
		{
			base.Awake();
			this.rectTransform = (RectTransform)base.transform;
			this.itemStacks = ItemCatalog.RequestItemStackArray();
			this.itemOrder = ItemCatalog.RequestItemOrderBuffer();
		}

		// Token: 0x0600235F RID: 9055 RVA: 0x0009AAE3 File Offset: 0x00098CE3
		protected override void OnDestroy()
		{
			this.SetSubscribedInventory(null);
			base.OnDestroy();
		}

		// Token: 0x06002360 RID: 9056 RVA: 0x0009AAF2 File Offset: 0x00098CF2
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

		// Token: 0x06002361 RID: 9057 RVA: 0x0009AB19 File Offset: 0x00098D19
		private void RequestUpdateDisplay()
		{
			if (!this.updateRequestPending)
			{
				this.updateRequestPending = true;
				RoR2Application.onNextUpdate += this.UpdateDisplay;
			}
		}

		// Token: 0x06002362 RID: 9058 RVA: 0x0009AB3C File Offset: 0x00098D3C
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

		// Token: 0x06002363 RID: 9059 RVA: 0x0009ABD4 File Offset: 0x00098DD4
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

		// Token: 0x06002364 RID: 9060 RVA: 0x0009AC27 File Offset: 0x00098E27
		public void SetItems(ItemIndex[] newItemOrder, int newItemOrderCount, int[] newItemStacks)
		{
			this.itemOrderCount = newItemOrderCount;
			Array.Copy(newItemOrder, this.itemOrder, this.itemOrderCount);
			Array.Copy(newItemStacks, this.itemStacks, this.itemStacks.Length);
			this.RequestUpdateDisplay();
		}

		// Token: 0x06002365 RID: 9061 RVA: 0x0009AC5C File Offset: 0x00098E5C
		public void ResetItems()
		{
			this.itemOrderCount = 0;
			Array.Clear(this.itemStacks, 0, this.itemStacks.Length);
			this.RequestUpdateDisplay();
		}

		// Token: 0x06002366 RID: 9062 RVA: 0x0009AC80 File Offset: 0x00098E80
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

		// Token: 0x0400213B RID: 8507
		private RectTransform rectTransform;

		// Token: 0x0400213C RID: 8508
		public GameObject itemIconPrefab;

		// Token: 0x0400213D RID: 8509
		public float iconWidth = 64f;

		// Token: 0x0400213E RID: 8510
		public float maxHeight = 128f;

		// Token: 0x0400213F RID: 8511
		[SerializeField]
		[HideInInspector]
		private List<ItemIcon> itemIcons;

		// Token: 0x04002140 RID: 8512
		private ItemIndex[] itemOrder;

		// Token: 0x04002141 RID: 8513
		private int itemOrderCount;

		// Token: 0x04002142 RID: 8514
		private int[] itemStacks;

		// Token: 0x04002143 RID: 8515
		private float currentIconScale = 1f;

		// Token: 0x04002144 RID: 8516
		private float previousWidth;

		// Token: 0x04002145 RID: 8517
		private bool updateRequestPending;

		// Token: 0x04002146 RID: 8518
		private Inventory inventory;

		// Token: 0x04002147 RID: 8519
		private bool inventoryWasValid;

		// Token: 0x020005D6 RID: 1494
		private struct LayoutValues
		{
			// Token: 0x04002148 RID: 8520
			public float width;

			// Token: 0x04002149 RID: 8521
			public float iconSize;

			// Token: 0x0400214A RID: 8522
			public int iconsPerRow;

			// Token: 0x0400214B RID: 8523
			public float rowWidth;

			// Token: 0x0400214C RID: 8524
			public int rowCount;

			// Token: 0x0400214D RID: 8525
			public Vector3 iconLocalScale;

			// Token: 0x0400214E RID: 8526
			public Vector3 topLeftCorner;
		}
	}
}
