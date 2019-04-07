using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005FF RID: 1535
	[RequireComponent(typeof(RectTransform))]
	[ExecuteInEditMode]
	public class MainUIArea : MonoBehaviour
	{
		// Token: 0x06002262 RID: 8802 RVA: 0x000A26A3 File Offset: 0x000A08A3
		private void Awake()
		{
			this.rectTransform = base.GetComponent<RectTransform>();
			this.parentRectTransform = this.rectTransform.parent.GetComponent<RectTransform>();
		}

		// Token: 0x06002263 RID: 8803 RVA: 0x000A26C8 File Offset: 0x000A08C8
		private void Update()
		{
			Rect rect = this.parentRectTransform.rect;
			float num = rect.width * 0.05f;
			float num2 = rect.height * 0.05f;
			this.rectTransform.offsetMin = new Vector2(num, num2);
			this.rectTransform.offsetMax = new Vector2(-num, -num2);
		}

		// Token: 0x0400257A RID: 9594
		private RectTransform rectTransform;

		// Token: 0x0400257B RID: 9595
		private RectTransform parentRectTransform;
	}
}
