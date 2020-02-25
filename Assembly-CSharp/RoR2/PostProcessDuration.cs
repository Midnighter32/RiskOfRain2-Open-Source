using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace RoR2
{
	// Token: 0x020002C9 RID: 713
	public class PostProcessDuration : MonoBehaviour
	{
		// Token: 0x06001024 RID: 4132 RVA: 0x0004705C File Offset: 0x0004525C
		private void Update()
		{
			this.stopwatch += Time.deltaTime;
			this.UpdatePostProccess();
		}

		// Token: 0x06001025 RID: 4133 RVA: 0x00047076 File Offset: 0x00045276
		private void Awake()
		{
			this.UpdatePostProccess();
		}

		// Token: 0x06001026 RID: 4134 RVA: 0x0004707E File Offset: 0x0004527E
		private void OnEnable()
		{
			this.stopwatch = 0f;
		}

		// Token: 0x06001027 RID: 4135 RVA: 0x0004708C File Offset: 0x0004528C
		private void UpdatePostProccess()
		{
			float num = Mathf.Clamp01(this.stopwatch / this.maxDuration);
			this.ppVolume.weight = this.ppWeightCurve.Evaluate(num);
			if (num == 1f && this.destroyOnEnd)
			{
				UnityEngine.Object.Destroy(this.ppVolume.gameObject);
			}
		}

		// Token: 0x06001028 RID: 4136 RVA: 0x000470E3 File Offset: 0x000452E3
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

		// Token: 0x04000FA3 RID: 4003
		public PostProcessVolume ppVolume;

		// Token: 0x04000FA4 RID: 4004
		public AnimationCurve ppWeightCurve;

		// Token: 0x04000FA5 RID: 4005
		public float maxDuration;

		// Token: 0x04000FA6 RID: 4006
		public bool destroyOnEnd;

		// Token: 0x04000FA7 RID: 4007
		private float stopwatch;
	}
}
