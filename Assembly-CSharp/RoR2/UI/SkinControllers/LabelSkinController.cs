using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI.SkinControllers
{
	// Token: 0x02000655 RID: 1621
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class LabelSkinController : BaseSkinController
	{
		// Token: 0x0600261A RID: 9754 RVA: 0x000A59C0 File Offset: 0x000A3BC0
		protected new void Awake()
		{
			this.label = base.GetComponent<TextMeshProUGUI>();
			base.Awake();
		}

		// Token: 0x0600261B RID: 9755 RVA: 0x000A59D4 File Offset: 0x000A3BD4
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

		// Token: 0x040023D8 RID: 9176
		public LabelSkinController.LabelType labelType;

		// Token: 0x040023D9 RID: 9177
		public bool useRecommendedAlignment = true;

		// Token: 0x040023DA RID: 9178
		private TextMeshProUGUI label;

		// Token: 0x02000656 RID: 1622
		public enum LabelType
		{
			// Token: 0x040023DC RID: 9180
			Default,
			// Token: 0x040023DD RID: 9181
			Header,
			// Token: 0x040023DE RID: 9182
			Detail
		}
	}
}
