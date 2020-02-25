using System;
using UnityEngine;

namespace RoR2.UI.SkinControllers
{
	// Token: 0x02000653 RID: 1619
	[ExecuteAlways]
	public abstract class BaseSkinController : MonoBehaviour
	{
		// Token: 0x0600260C RID: 9740
		protected abstract void OnSkinUI();

		// Token: 0x0600260D RID: 9741 RVA: 0x000A5760 File Offset: 0x000A3960
		protected void Awake()
		{
			if (this.skinData)
			{
				this.DoSkinUI();
			}
		}

		// Token: 0x0600260E RID: 9742 RVA: 0x000A5775 File Offset: 0x000A3975
		private void DoSkinUI()
		{
			if (this.skinData)
			{
				this.OnSkinUI();
			}
		}

		// Token: 0x040023CE RID: 9166
		public UISkinData skinData;
	}
}
