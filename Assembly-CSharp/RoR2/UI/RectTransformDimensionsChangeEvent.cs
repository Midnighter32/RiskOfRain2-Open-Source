using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000627 RID: 1575
	public class RectTransformDimensionsChangeEvent : MonoBehaviour
	{
		// Token: 0x1400005A RID: 90
		// (add) Token: 0x06002358 RID: 9048 RVA: 0x000A66BC File Offset: 0x000A48BC
		// (remove) Token: 0x06002359 RID: 9049 RVA: 0x000A66F4 File Offset: 0x000A48F4
		public event Action onRectTransformDimensionsChange;

		// Token: 0x0600235A RID: 9050 RVA: 0x000A6729 File Offset: 0x000A4929
		private void OnRectTransformDimensionsChange()
		{
			if (this.onRectTransformDimensionsChange != null)
			{
				this.onRectTransformDimensionsChange();
			}
		}
	}
}
