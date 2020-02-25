using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2
{
	// Token: 0x02000147 RID: 327
	public class AnimateImageAlpha : MonoBehaviour
	{
		// Token: 0x060005CB RID: 1483 RVA: 0x00018021 File Offset: 0x00016221
		private void OnEnable()
		{
			this.stopwatch = 0f;
		}

		// Token: 0x060005CC RID: 1484 RVA: 0x00018021 File Offset: 0x00016221
		public void ResetStopwatch()
		{
			this.stopwatch = 0f;
		}

		// Token: 0x060005CD RID: 1485 RVA: 0x00018030 File Offset: 0x00016230
		private void LateUpdate()
		{
			this.stopwatch += Time.unscaledDeltaTime;
			int num = 0;
			foreach (Image image in this.images)
			{
				num++;
				float a = this.alphaCurve.Evaluate((this.stopwatch + this.delayBetweenElements * (float)num) / this.timeMax);
				Color color = image.color;
				image.color = new Color(color.r, color.g, color.b, a);
			}
		}

		// Token: 0x0400064A RID: 1610
		public AnimationCurve alphaCurve;

		// Token: 0x0400064B RID: 1611
		public Image[] images;

		// Token: 0x0400064C RID: 1612
		public float timeMax = 5f;

		// Token: 0x0400064D RID: 1613
		public float delayBetweenElements;

		// Token: 0x0400064E RID: 1614
		[HideInInspector]
		public float stopwatch;
	}
}
