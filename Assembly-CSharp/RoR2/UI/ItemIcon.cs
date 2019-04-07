using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005EF RID: 1519
	[RequireComponent(typeof(RectTransform))]
	public class ItemIcon : MonoBehaviour
	{
		// Token: 0x170002FB RID: 763
		// (get) Token: 0x0600220B RID: 8715 RVA: 0x000A1004 File Offset: 0x0009F204
		// (set) Token: 0x0600220C RID: 8716 RVA: 0x000A100C File Offset: 0x0009F20C
		public RectTransform rectTransform { get; private set; }

		// Token: 0x0600220D RID: 8717 RVA: 0x000A1015 File Offset: 0x0009F215
		private void Awake()
		{
			this.CacheRectTransform();
		}

		// Token: 0x0600220E RID: 8718 RVA: 0x000A101D File Offset: 0x0009F21D
		public void CacheRectTransform()
		{
			if (this.rectTransform == null)
			{
				this.rectTransform = (RectTransform)base.transform;
			}
		}

		// Token: 0x0600220F RID: 8719 RVA: 0x000A1038 File Offset: 0x0009F238
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

		// Token: 0x04002513 RID: 9491
		public RawImage glowImage;

		// Token: 0x04002514 RID: 9492
		public RawImage image;

		// Token: 0x04002515 RID: 9493
		public TextMeshProUGUI stackText;

		// Token: 0x04002516 RID: 9494
		public TooltipProvider tooltipProvider;

		// Token: 0x04002517 RID: 9495
		private ItemIndex itemIndex;

		// Token: 0x04002518 RID: 9496
		private int itemCount;
	}
}
