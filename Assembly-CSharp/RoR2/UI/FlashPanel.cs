using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005DD RID: 1501
	public class FlashPanel : MonoBehaviour
	{
		// Token: 0x0600219D RID: 8605 RVA: 0x0009E24C File Offset: 0x0009C44C
		private void Start()
		{
			this.image = this.flashRectTransform.GetComponent<Image>();
		}

		// Token: 0x0600219E RID: 8606 RVA: 0x0009E260 File Offset: 0x0009C460
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

		// Token: 0x0600219F RID: 8607 RVA: 0x0009E3BD File Offset: 0x0009C5BD
		public void Flash()
		{
			this.theta = 0f;
			this.isFlashing = true;
		}

		// Token: 0x04002456 RID: 9302
		public RectTransform flashRectTransform;

		// Token: 0x04002457 RID: 9303
		public float strength = 1f;

		// Token: 0x04002458 RID: 9304
		public float freq = 1f;

		// Token: 0x04002459 RID: 9305
		public float flashAlpha = 0.7f;

		// Token: 0x0400245A RID: 9306
		public bool alwaysFlash = true;

		// Token: 0x0400245B RID: 9307
		private bool isFlashing;

		// Token: 0x0400245C RID: 9308
		private float theta = 1f;

		// Token: 0x0400245D RID: 9309
		private Image image;
	}
}
