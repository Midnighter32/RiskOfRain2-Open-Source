using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005ED RID: 1517
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	public class MainUIArea : MonoBehaviour
	{
		// Token: 0x060023CD RID: 9165 RVA: 0x0009C5FF File Offset: 0x0009A7FF
		private void Awake()
		{
			this.rectTransform = base.GetComponent<RectTransform>();
			this.parentRectTransform = this.rectTransform.parent.GetComponent<RectTransform>();
		}

		// Token: 0x060023CE RID: 9166 RVA: 0x0009C624 File Offset: 0x0009A824
		private void Update()
		{
			Rect rect = this.parentRectTransform.rect;
			float num = rect.width * 0.05f;
			float num2 = rect.height * 0.05f;
			this.rectTransform.offsetMin = new Vector2(num, num2);
			this.rectTransform.offsetMax = new Vector2(-num, -num2);
		}

		// Token: 0x040021C0 RID: 8640
		private RectTransform rectTransform;

		// Token: 0x040021C1 RID: 8641
		private RectTransform parentRectTransform;
	}
}
