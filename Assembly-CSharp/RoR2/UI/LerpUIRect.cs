using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005DE RID: 1502
	[RequireComponent(typeof(RectTransform))]
	public class LerpUIRect : MonoBehaviour
	{
		// Token: 0x0600238A RID: 9098 RVA: 0x0009B3BD File Offset: 0x000995BD
		private void Start()
		{
			this.rectTransform = base.GetComponent<RectTransform>();
		}

		// Token: 0x0600238B RID: 9099 RVA: 0x0009B3CB File Offset: 0x000995CB
		private void OnDisable()
		{
			this.lerpState = LerpUIRect.LerpState.Entering;
			this.stopwatch = 0f;
			this.UpdateLerp();
		}

		// Token: 0x0600238C RID: 9100 RVA: 0x0009B3E5 File Offset: 0x000995E5
		private void Update()
		{
			this.stopwatch += Time.deltaTime;
			this.UpdateLerp();
		}

		// Token: 0x0600238D RID: 9101 RVA: 0x0009B400 File Offset: 0x00099600
		private void UpdateLerp()
		{
			LerpUIRect.LerpState lerpState = this.lerpState;
			if (lerpState != LerpUIRect.LerpState.Entering)
			{
				if (lerpState != LerpUIRect.LerpState.Leaving)
				{
					return;
				}
				float num = this.stopwatch / this.enterDuration;
				float t = this.leavingCurve.Evaluate(num);
				this.rectTransform.anchoredPosition = Vector3.LerpUnclamped(this.finalLocalPosition, this.startLocalPosition, t);
				if (num >= 1f)
				{
					this.lerpState = LerpUIRect.LerpState.Holding;
					this.stopwatch = 0f;
				}
			}
			else
			{
				float num = this.stopwatch / this.enterDuration;
				float t = this.enterCurve.Evaluate(num);
				this.rectTransform.anchoredPosition = Vector3.LerpUnclamped(this.startLocalPosition, this.finalLocalPosition, t);
				if (num >= 1f)
				{
					this.lerpState = LerpUIRect.LerpState.Holding;
					this.stopwatch = 0f;
					return;
				}
			}
		}

		// Token: 0x04002177 RID: 8567
		public Vector3 startLocalPosition;

		// Token: 0x04002178 RID: 8568
		public Vector3 finalLocalPosition;

		// Token: 0x04002179 RID: 8569
		public LerpUIRect.LerpState lerpState;

		// Token: 0x0400217A RID: 8570
		public AnimationCurve enterCurve;

		// Token: 0x0400217B RID: 8571
		public float enterDuration;

		// Token: 0x0400217C RID: 8572
		public AnimationCurve leavingCurve;

		// Token: 0x0400217D RID: 8573
		public float leaveDuration;

		// Token: 0x0400217E RID: 8574
		private float stopwatch;

		// Token: 0x0400217F RID: 8575
		private RectTransform rectTransform;

		// Token: 0x020005DF RID: 1503
		public enum LerpState
		{
			// Token: 0x04002181 RID: 8577
			Entering,
			// Token: 0x04002182 RID: 8578
			Holding,
			// Token: 0x04002183 RID: 8579
			Leaving
		}
	}
}
