using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI.SkinControllers
{
	// Token: 0x02000664 RID: 1636
	[RequireComponent(typeof(ScrollRect))]
	public class ScrollRectSkinController : BaseSkinController
	{
		// Token: 0x0600247C RID: 9340 RVA: 0x000AAFAB File Offset: 0x000A91AB
		protected new void Awake()
		{
			this.scrollRect = base.GetComponent<ScrollRect>();
			base.Awake();
		}

		// Token: 0x0600247D RID: 9341 RVA: 0x000AAFC0 File Offset: 0x000A91C0
		protected override void OnSkinUI()
		{
			Image component = base.GetComponent<Image>();
			if (component)
			{
				this.skinData.scrollRectStyle.backgroundPanelStyle.Apply(component);
			}
			if (this.scrollRect.verticalScrollbar)
			{
				this.SkinScrollbar(this.scrollRect.verticalScrollbar);
			}
			if (this.scrollRect.horizontalScrollbar)
			{
				this.SkinScrollbar(this.scrollRect.horizontalScrollbar);
			}
		}

		// Token: 0x0600247E RID: 9342 RVA: 0x000AB038 File Offset: 0x000A9238
		private void SkinScrollbar(Scrollbar scrollbar)
		{
			this.skinData.scrollRectStyle.scrollbarBackgroundStyle.Apply(scrollbar.GetComponent<Image>());
			scrollbar.colors = this.skinData.scrollRectStyle.scrollbarHandleColors;
			scrollbar.handleRect.GetComponent<Image>().sprite = this.skinData.scrollRectStyle.scrollbarHandleImage;
		}

		// Token: 0x0400277E RID: 10110
		private ScrollRect scrollRect;
	}
}
