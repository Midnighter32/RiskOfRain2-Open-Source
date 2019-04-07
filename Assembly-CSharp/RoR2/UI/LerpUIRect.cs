using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005F9 RID: 1529
	[RequireComponent(typeof(RectTransform))]
	public class LerpUIRect : MonoBehaviour
	{
		// Token: 0x06002245 RID: 8773 RVA: 0x000A1FA9 File Offset: 0x000A01A9
		private void Start()
		{
			this.rectTransform = base.GetComponent<RectTransform>();
		}

		// Token: 0x06002246 RID: 8774 RVA: 0x000A1FB7 File Offset: 0x000A01B7
		private void OnDisable()
		{
			this.lerpState = LerpUIRect.LerpState.Entering;
			this.stopwatch = 0f;
			this.UpdateLerp();
		}

		// Token: 0x06002247 RID: 8775 RVA: 0x000A1FD1 File Offset: 0x000A01D1
		private void Update()
		{
			this.stopwatch += Time.deltaTime;
			this.UpdateLerp();
		}

		// Token: 0x06002248 RID: 8776 RVA: 0x000A1FEC File Offset: 0x000A01EC
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

		// Token: 0x04002558 RID: 9560
		public Vector3 startLocalPosition;

		// Token: 0x04002559 RID: 9561
		public Vector3 finalLocalPosition;

		// Token: 0x0400255A RID: 9562
		public LerpUIRect.LerpState lerpState;

		// Token: 0x0400255B RID: 9563
		public AnimationCurve enterCurve;

		// Token: 0x0400255C RID: 9564
		public float enterDuration;

		// Token: 0x0400255D RID: 9565
		public AnimationCurve leavingCurve;

		// Token: 0x0400255E RID: 9566
		public float leaveDuration;

		// Token: 0x0400255F RID: 9567
		private float stopwatch;

		// Token: 0x04002560 RID: 9568
		private RectTransform rectTransform;

		// Token: 0x020005FA RID: 1530
		public enum LerpState
		{
			// Token: 0x04002562 RID: 9570
			Entering,
			// Token: 0x04002563 RID: 9571
			Holding,
			// Token: 0x04002564 RID: 9572
			Leaving
		}
	}
}
