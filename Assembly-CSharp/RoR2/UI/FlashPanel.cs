using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005BC RID: 1468
	public class FlashPanel : MonoBehaviour
	{
		// Token: 0x060022CD RID: 8909 RVA: 0x000971F0 File Offset: 0x000953F0
		private void Start()
		{
			this.image = this.flashRectTransform.GetComponent<Image>();
		}

		// Token: 0x060022CE RID: 8910 RVA: 0x00097204 File Offset: 0x00095404
		private void Update()
		{
			this.flashRectTransform.anchorMin = new Vector2(0f, 0f);
			this.flashRectTransform.anchorMax = new Vector2(1f, 1f);
			if (this.alwaysFlash)
			{
				this.isFlashing = true;
			}
			if (this.isFlashing)
			{
				this.theta += Time.deltaTime * this.freq;
			}
			if (this.theta > 1f)
			{
				if (this.alwaysFlash)
				{
					this.theta -= this.theta - this.theta % 1f;
				}
				else
				{
					this.theta = 1f;
				}
				this.isFlashing = false;
			}
			float num = 1f - (1f + Mathf.Cos(this.theta * 3.1415927f * 0.5f + 1.5707964f));
			this.flashRectTransform.sizeDelta = new Vector2(1f + num * 20f * this.strength, 1f + num * 20f * this.strength);
			if (this.image)
			{
				Color color = this.image.color;
				color.a = (1f - num) * this.strength * this.flashAlpha;
				this.image.color = color;
			}
		}

		// Token: 0x060022CF RID: 8911 RVA: 0x00097361 File Offset: 0x00095561
		public void Flash()
		{
			this.theta = 0f;
			this.isFlashing = true;
		}

		// Token: 0x04002065 RID: 8293
		public RectTransform flashRectTransform;

		// Token: 0x04002066 RID: 8294
		public float strength = 1f;

		// Token: 0x04002067 RID: 8295
		public float freq = 1f;

		// Token: 0x04002068 RID: 8296
		public float flashAlpha = 0.7f;

		// Token: 0x04002069 RID: 8297
		public bool alwaysFlash = true;

		// Token: 0x0400206A RID: 8298
		private bool isFlashing;

		// Token: 0x0400206B RID: 8299
		private float theta = 1f;

		// Token: 0x0400206C RID: 8300
		private Image image;
	}
}
