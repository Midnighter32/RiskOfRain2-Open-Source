using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace RoR2
{
	// Token: 0x0200038D RID: 909
	public class PostProcessDuration : MonoBehaviour
	{
		// Token: 0x0600130E RID: 4878 RVA: 0x0005D685 File Offset: 0x0005B885
		private void Update()
		{
			this.stopwatch += Time.deltaTime;
			this.UpdatePostProccess();
		}

		// Token: 0x0600130F RID: 4879 RVA: 0x0005D69F File Offset: 0x0005B89F
		private void Awake()
		{
			this.UpdatePostProccess();
		}

		// Token: 0x06001310 RID: 4880 RVA: 0x0005D6A7 File Offset: 0x0005B8A7
		private void OnEnable()
		{
			this.stopwatch = 0f;
		}

		// Token: 0x06001311 RID: 4881 RVA: 0x0005D6B4 File Offset: 0x0005B8B4
		private void UpdatePostProccess()
		{
			float num = Mathf.Clamp01(this.stopwatch / this.maxDuration);
			this.ppVolume.weight = this.ppWeightCurve.Evaluate(num);
			if (num == 1f && this.destroyOnEnd)
			{
				UnityEngine.Object.Destroy(this.ppVolume.gameObject);
			}
		}

		// Token: 0x06001312 RID: 4882 RVA: 0x0005D70B File Offset: 0x0005B90B
		private void OnValidate()
		{
			if (this.maxDuration <= Mathf.Epsilon)
			{
				Debug.LogErrorFormat("{0} has PP of time zero!", new object[]
				{
					base.gameObject
				});
			}
		}

		// Token: 0x040016D5 RID: 5845
		public PostProcessVolume ppVolume;

		// Token: 0x040016D6 RID: 5846
		public AnimationCurve ppWeightCurve;

		// Token: 0x040016D7 RID: 5847
		public float maxDuration;

		// Token: 0x040016D8 RID: 5848
		public bool destroyOnEnd;

		// Token: 0x040016D9 RID: 5849
		private float stopwatch;
	}
}
