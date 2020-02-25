using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI.SkinControllers
{
	// Token: 0x02000659 RID: 1625
	[RequireComponent(typeof(ScrollRect))]
	public class ScrollRectSkinController : BaseSkinController
	{
		// Token: 0x06002620 RID: 9760 RVA: 0x000A5AE7 File Offset: 0x000A3CE7
		protected new void Awake()
		{
			this.scrollRect = base.GetComponent<ScrollRect>();
			base.Awake();
		}

		// Token: 0x06002621 RID: 9761 RVA: 0x000A5AFC File Offset: 0x000A3CFC
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

		// Token: 0x06002622 RID: 9762 RVA: 0x000A5B74 File Offset: 0x000A3D74
		private void SkinScrollbar(Scrollbar scrollbar)
		{
			this.skinData.scrollRectStyle.scrollbarBackgroundStyle.Apply(scrollbar.GetComponent<Image>());
			scrollbar.colors = this.skinData.scrollRectStyle.scrollbarHandleColors;
			scrollbar.handleRect.GetComponent<Image>().sprite = this.skinData.scrollRectStyle.scrollbarHandleImage;
		}

		// Token: 0x040023E5 RID: 9189
		private ScrollRect scrollRect;
	}
}
