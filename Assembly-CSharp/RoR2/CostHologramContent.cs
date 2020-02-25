using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001CD RID: 461
	public class CostHologramContent : MonoBehaviour
	{
		// Token: 0x060009DE RID: 2526 RVA: 0x0002B010 File Offset: 0x00029210
		private void FixedUpdate()
		{
			if (this.targetTextMesh)
			{
				CostHologramContent.sharedStringBuilder.Clear();
				Color color = Color.white;
				CostTypeDef costTypeDef = CostTypeCatalog.GetCostTypeDef(this.costType);
				if (costTypeDef != null)
				{
					costTypeDef.BuildCostStringStyled(this.displayValue, CostHologramContent.sharedStringBuilder, true, false);
					color = costTypeDef.GetCostColor(true);
				}
				this.targetTextMesh.SetText(CostHologramContent.sharedStringBuilder);
				this.targetTextMesh.color = color;
			}
		}

		// Token: 0x04000A0A RID: 2570
		public int displayValue;

		// Token: 0x04000A0B RID: 2571
		public TextMeshPro targetTextMesh;

		// Token: 0x04000A0C RID: 2572
		public CostTypeIndex costType;

		// Token: 0x04000A0D RID: 2573
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();
	}
}
