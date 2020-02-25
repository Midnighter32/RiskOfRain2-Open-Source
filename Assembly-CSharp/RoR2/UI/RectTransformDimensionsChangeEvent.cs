using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000616 RID: 1558
	public class RectTransformDimensionsChangeEvent : MonoBehaviour
	{
		// Token: 0x14000087 RID: 135
		// (add) Token: 0x060024D7 RID: 9431 RVA: 0x000A0A9C File Offset: 0x0009EC9C
		// (remove) Token: 0x060024D8 RID: 9432 RVA: 0x000A0AD4 File Offset: 0x0009ECD4
		public event Action onRectTransformDimensionsChange;

		// Token: 0x060024D9 RID: 9433 RVA: 0x000A0B09 File Offset: 0x0009ED09
		private void OnRectTransformDimensionsChange()
		{
			if (this.onRectTransformDimensionsChange != null)
			{
				this.onRectTransformDimensionsChange();
			}
		}
	}
}
