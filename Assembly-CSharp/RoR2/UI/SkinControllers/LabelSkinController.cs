using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI.SkinControllers
{
	// Token: 0x02000660 RID: 1632
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class LabelSkinController : BaseSkinController
	{
		// Token: 0x06002476 RID: 9334 RVA: 0x000AAE84 File Offset: 0x000A9084
		protected new void Awake()
		{
			this.label = base.GetComponent<TextMeshProUGUI>();
			base.Awake();
		}

		// Token: 0x06002477 RID: 9335 RVA: 0x000AAE98 File Offset: 0x000A9098
		protected override void OnSkinUI()
		{
			switch (this.labelType)
			{
			case LabelSkinController.LabelType.Default:
				this.skinData.bodyTextStyle.Apply(this.label, this.useRecommendedAlignment);
				return;
			case LabelSkinController.LabelType.Header:
				this.skinData.headerTextStyle.Apply(this.label, this.useRecommendedAlignment);
				return;
			case LabelSkinController.LabelType.Detail:
				this.skinData.detailTextStyle.Apply(this.label, this.useRecommendedAlignment);
				return;
			default:
				return;
			}
		}

		// Token: 0x04002771 RID: 10097
		public LabelSkinController.LabelType labelType;

		// Token: 0x04002772 RID: 10098
		public bool useRecommendedAlignment = true;

		// Token: 0x04002773 RID: 10099
		private TextMeshProUGUI label;

		// Token: 0x02000661 RID: 1633
		public enum LabelType
		{
			// Token: 0x04002775 RID: 10101
			Default,
			// Token: 0x04002776 RID: 10102
			Header,
			// Token: 0x04002777 RID: 10103
			Detail
		}
	}
}
