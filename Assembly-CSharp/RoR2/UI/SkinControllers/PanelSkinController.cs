using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI.SkinControllers
{
	// Token: 0x02000657 RID: 1623
	[RequireComponent(typeof(Image))]
	public class PanelSkinController : BaseSkinController
	{
		// Token: 0x0600261D RID: 9757 RVA: 0x000A5A60 File Offset: 0x000A3C60
		protected new void Awake()
		{
			this.image = base.GetComponent<Image>();
			base.Awake();
		}

		// Token: 0x0600261E RID: 9758 RVA: 0x000A5A74 File Offset: 0x000A3C74
		protected override void OnSkinUI()
		{
			switch (this.panelType)
			{
			case PanelSkinController.PanelType.Default:
				this.skinData.mainPanelStyle.Apply(this.image);
				return;
			case PanelSkinController.PanelType.Header:
				this.skinData.headerPanelStyle.Apply(this.image);
				return;
			case PanelSkinController.PanelType.Detail:
				this.skinData.detailPanelStyle.Apply(this.image);
				return;
			default:
				return;
			}
		}

		// Token: 0x040023DF RID: 9183
		public PanelSkinController.PanelType panelType;

		// Token: 0x040023E0 RID: 9184
		private Image image;

		// Token: 0x02000658 RID: 1624
		public enum PanelType
		{
			// Token: 0x040023E2 RID: 9186
			Default,
			// Token: 0x040023E3 RID: 9187
			Header,
			// Token: 0x040023E4 RID: 9188
			Detail
		}
	}
}
