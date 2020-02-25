using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005D4 RID: 1492
	[RequireComponent(typeof(RectTransform))]
	public class ItemIcon : MonoBehaviour
	{
		// Token: 0x170003B0 RID: 944
		// (get) Token: 0x0600234E RID: 9038 RVA: 0x0009A45C File Offset: 0x0009865C
		// (set) Token: 0x0600234F RID: 9039 RVA: 0x0009A464 File Offset: 0x00098664
		public RectTransform rectTransform { get; private set; }

		// Token: 0x06002350 RID: 9040 RVA: 0x0009A46D File Offset: 0x0009866D
		private void Awake()
		{
			this.CacheRectTransform();
		}

		// Token: 0x06002351 RID: 9041 RVA: 0x0009A475 File Offset: 0x00098675
		public void CacheRectTransform()
		{
			if (this.rectTransform == null)
			{
				this.rectTransform = (RectTransform)base.transform;
			}
		}

		// Token: 0x06002352 RID: 9042 RVA: 0x0009A490 File Offset: 0x00098690
		public void SetItemIndex(ItemIndex newItemIndex, int newItemCount)
		{
			if (this.itemIndex == newItemIndex && this.itemCount == newItemCount)
			{
				return;
			}
			this.itemIndex = newItemIndex;
			this.itemCount = newItemCount;
			string titleToken = "";
			string bodyToken = "";
			Color color = Color.white;
			Color bodyColor = new Color(0.6f, 0.6f, 0.6f, 1f);
			ItemDef itemDef = ItemCatalog.GetItemDef(this.itemIndex);
			if (itemDef != null)
			{
				this.image.texture = Resources.Load<Texture>(itemDef.pickupIconPath);
				if (this.itemCount > 1)
				{
					this.stackText.enabled = true;
					this.stackText.text = string.Format("x{0}", this.itemCount);
				}
				else
				{
					this.stackText.enabled = false;
				}
				titleToken = itemDef.nameToken;
				bodyToken = itemDef.pickupToken;
				color = ColorCatalog.GetColor(itemDef.darkColorIndex);
			}
			if (this.glowImage)
			{
				this.glowImage.color = new Color(color.r, color.g, color.b, 0.75f);
			}
			if (this.tooltipProvider)
			{
				this.tooltipProvider.titleToken = titleToken;
				this.tooltipProvider.bodyToken = bodyToken;
				this.tooltipProvider.titleColor = color;
				this.tooltipProvider.bodyColor = bodyColor;
			}
		}

		// Token: 0x04002135 RID: 8501
		public RawImage glowImage;

		// Token: 0x04002136 RID: 8502
		public RawImage image;

		// Token: 0x04002137 RID: 8503
		public TextMeshProUGUI stackText;

		// Token: 0x04002138 RID: 8504
		public TooltipProvider tooltipProvider;

		// Token: 0x04002139 RID: 8505
		private ItemIndex itemIndex;

		// Token: 0x0400213A RID: 8506
		private int itemCount;
	}
}
