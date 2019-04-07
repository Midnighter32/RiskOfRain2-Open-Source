using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI.SkinControllers
{
	// Token: 0x02000662 RID: 1634
	[RequireComponent(typeof(Image))]
	public class PanelSkinController : BaseSkinController
	{
		// Token: 0x06002479 RID: 9337 RVA: 0x000AAF24 File Offset: 0x000A9124
		protected new void Awake()
		{
			this.image = base.GetComponent<Image>();
			base.Awake();
		}

		// Token: 0x0600247A RID: 9338 RVA: 0x000AAF38 File Offset: 0x000A9138
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

		// Token: 0x04002778 RID: 10104
		public PanelSkinController.PanelType panelType;

		// Token: 0x04002779 RID: 10105
		private Image image;

		// Token: 0x02000663 RID: 1635
		public enum PanelType
		{
			// Token: 0x0400277B RID: 10107
			Default,
			// Token: 0x0400277C RID: 10108
			Header,
			// Token: 0x0400277D RID: 10109
			Detail
		}
	}
}
