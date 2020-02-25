using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002A7 RID: 679
	public class ObjectScaleCurve : MonoBehaviour
	{
		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x06000F7C RID: 3964 RVA: 0x000442ED File Offset: 0x000424ED
		// (set) Token: 0x06000F7D RID: 3965 RVA: 0x000442F5 File Offset: 0x000424F5
		public float time { get; set; }

		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x06000F7E RID: 3966 RVA: 0x000442FE File Offset: 0x000424FE
		// (set) Token: 0x06000F7F RID: 3967 RVA: 0x00044306 File Offset: 0x00042506
		public Vector3 baseScale { get; set; }

		// Token: 0x06000F80 RID: 3968 RVA: 0x0004430F File Offset: 0x0004250F
		private void Awake()
		{
			this.baseScale = base.transform.localScale;
			this.Reset();
		}

		// Token: 0x06000F81 RID: 3969 RVA: 0x00044328 File Offset: 0x00042528
		private void OnEnable()
		{
			this.Reset();
		}

		// Token: 0x06000F82 RID: 3970 RVA: 0x00044330 File Offset: 0x00042530
		public void Reset()
		{
			this.time = 0f;
			this.UpdateScale(0f);
		}

		// Token: 0x06000F83 RID: 3971 RVA: 0x00044348 File Offset: 0x00042548
		private void Update()
		{
			this.time += Time.deltaTime;
			this.UpdateScale(this.time);
		}

		// Token: 0x06000F84 RID: 3972 RVA: 0x00044368 File Offset: 0x00042568
		private void UpdateScale(float time)
		{
			float time2 = Mathf.Clamp01(time / this.timeMax);
			float d = 1f;
			if (this.overallCurve != null)
			{
				d = this.overallCurve.Evaluate(time2);
			}
			Vector3 a = new Vector3(this.curveX.Evaluate(time2) * this.baseScale.x, this.curveY.Evaluate(time2) * this.baseScale.y, this.curveZ.Evaluate(time2) * this.baseScale.z);
			base.transform.localScale = a * d;
		}

		// Token: 0x04000EED RID: 3821
		public AnimationCurve curveX;

		// Token: 0x04000EEE RID: 3822
		public AnimationCurve curveY;

		// Token: 0x04000EEF RID: 3823
		public AnimationCurve curveZ;

		// Token: 0x04000EF0 RID: 3824
		public AnimationCurve overallCurve;

		// Token: 0x04000EF1 RID: 3825
		public float timeMax = 5f;
	}
}
