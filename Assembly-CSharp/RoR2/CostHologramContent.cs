using System;
using TMPro;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002BE RID: 702
	public class CostHologramContent : MonoBehaviour
	{
		// Token: 0x06000E3F RID: 3647 RVA: 0x00046090 File Offset: 0x00044290
		private void FixedUpdate()
		{
			if (this.targetTextMesh)
			{
				switch (this.costType)
				{
				case CostType.Money:
					this.targetTextMesh.text = string.Format("${0}", this.displayValue);
					this.targetTextMesh.color = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Money);
					return;
				case CostType.PercentHealth:
					this.targetTextMesh.text = string.Format("{0}% HP", this.displayValue);
					this.targetTextMesh.color = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Teleporter);
					return;
				case CostType.Lunar:
					this.targetTextMesh.text = string.Format("{0} Lunar", this.displayValue);
					this.targetTextMesh.color = ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarCoin);
					return;
				case CostType.WhiteItem:
					this.targetTextMesh.text = string.Format("{0} Items", this.displayValue);
					this.targetTextMesh.color = ColorCatalog.GetColor(PurchaseInteraction.CostTypeToColorIndex(this.costType));
					return;
				case CostType.GreenItem:
					this.targetTextMesh.text = string.Format("{0} Items", this.displayValue);
					this.targetTextMesh.color = ColorCatalog.GetColor(PurchaseInteraction.CostTypeToColorIndex(this.costType));
					return;
				case CostType.RedItem:
					this.targetTextMesh.text = string.Format("{0} Items", this.displayValue);
					this.targetTextMesh.color = ColorCatalog.GetColor(PurchaseInteraction.CostTypeToColorIndex(this.costType));
					return;
				default:
					this.targetTextMesh.text = string.Format("${0}", this.displayValue);
					this.targetTextMesh.color = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Error);
					break;
				}
			}
		}

		// Token: 0x04001222 RID: 4642
		public int displayValue;

		// Token: 0x04001223 RID: 4643
		public TextMeshPro targetTextMesh;

		// Token: 0x04001224 RID: 4644
		public CostType costType;
	}
}
