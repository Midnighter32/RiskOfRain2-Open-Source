using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2
{
	// Token: 0x02000256 RID: 598
	public class AnimateImageAlpha : MonoBehaviour
	{
		// Token: 0x06000B23 RID: 2851 RVA: 0x000374FD File Offset: 0x000356FD
		private void OnEnable()
		{
			this.stopwatch = 0f;
		}

		// Token: 0x06000B24 RID: 2852 RVA: 0x000374FD File Offset: 0x000356FD
		public void ResetStopwatch()
		{
			this.stopwatch = 0f;
		}

		// Token: 0x06000B25 RID: 2853 RVA: 0x0003750C File Offset: 0x0003570C
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

		// Token: 0x04000F2D RID: 3885
		public AnimationCurve alphaCurve;

		// Token: 0x04000F2E RID: 3886
		public Image[] images;

		// Token: 0x04000F2F RID: 3887
		public float timeMax = 5f;

		// Token: 0x04000F30 RID: 3888
		public float delayBetweenElements;

		// Token: 0x04000F31 RID: 3889
		[HideInInspector]
		public float stopwatch;
	}
}
