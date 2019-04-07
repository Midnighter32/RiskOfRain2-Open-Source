using System;
using UnityEngine;

namespace RoR2.UI.SkinControllers
{
	// Token: 0x0200065E RID: 1630
	[ExecuteAlways]
	public abstract class BaseSkinController : MonoBehaviour
	{
		// Token: 0x06002468 RID: 9320
		protected abstract void OnSkinUI();

		// Token: 0x06002469 RID: 9321 RVA: 0x000AAC24 File Offset: 0x000A8E24
		protected void Awake()
		{
			if (this.skinData)
			{
				this.DoSkinUI();
			}
		}

		// Token: 0x0600246A RID: 9322 RVA: 0x000AAC39 File Offset: 0x000A8E39
		private void DoSkinUI()
		{
			if (this.skinData)
			{
				this.OnSkinUI();
			}
		}

		// Token: 0x04002767 RID: 10087
		public UISkinData skinData;
	}
}
