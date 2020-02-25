using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000644 RID: 1604
	public class UIJuice : MonoBehaviour
	{
		// Token: 0x060025BD RID: 9661 RVA: 0x000A4529 File Offset: 0x000A2729
		private void Awake()
		{
			this.InitializeFirstTimeInfo();
		}

		// Token: 0x060025BE RID: 9662 RVA: 0x000A4531 File Offset: 0x000A2731
		private void Update()
		{
			this.transitionStopwatch = Mathf.Min(this.transitionStopwatch + Time.unscaledDeltaTime, this.transitionDuration);
			this.ProcessTransition();
		}

		// Token: 0x060025BF RID: 9663 RVA: 0x000A4558 File Offset: 0x000A2758
		private void ProcessTransition()
		{
			this.InitializeFirstTimeInfo();
			if (this.transitionStopwatch < this.transitionDuration)
			{
				AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, this.transitionStartAlpha, 1f, this.transitionEndAlpha);
				if (this.canvasGroup)
				{
					this.canvasGroup.alpha = animationCurve.Evaluate(this.transitionStopwatch / this.transitionDuration);
				}
				AnimationCurve animationCurve2 = new AnimationCurve();
				Keyframe key = new Keyframe(0f, 0f, 3f, 3f);
				Keyframe key2 = new Keyframe(1f, 1f, 0f, 0f);
				animationCurve2.AddKey(key);
				animationCurve2.AddKey(key2);
				Vector2 anchoredPosition = Vector2.Lerp(this.transitionStartPosition, this.transitionEndPosition, animationCurve2.Evaluate(this.transitionStopwatch / this.transitionDuration));
				Vector2 sizeDelta = Vector2.Lerp(this.transitionStartSize, this.transitionEndSize, animationCurve2.Evaluate(this.transitionStopwatch / this.transitionDuration));
				if (this.panningRect)
				{
					this.panningRect.anchoredPosition = anchoredPosition;
					this.panningRect.sizeDelta = sizeDelta;
					return;
				}
			}
			else
			{
				if (this.canvasGroup)
				{
					this.canvasGroup.alpha = this.transitionEndAlpha;
				}
				if (this.panningRect)
				{
					this.panningRect.anchoredPosition = this.transitionEndPosition;
					this.panningRect.sizeDelta = this.transitionEndSize;
				}
				if (this.destroyOnEndOfTransition)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x060025C0 RID: 9664 RVA: 0x000A46E8 File Offset: 0x000A28E8
		public void TransitionScaleUpWidth()
		{
			this.InitializeFirstTimeInfo();
			if (this.panningRect)
			{
				this.transitionStartSize = new Vector2(0f, this.transitionEndSize.y * 0.8f);
				this.transitionEndSize = this.originalSize;
			}
			this.BeginTransition();
		}

		// Token: 0x060025C1 RID: 9665 RVA: 0x000A4740 File Offset: 0x000A2940
		public void TransitionPanFromLeft()
		{
			this.InitializeFirstTimeInfo();
			if (this.panningRect)
			{
				this.transitionStartPosition = new Vector2(-1f, 0f) * this.panningMagnitude;
				this.transitionEndPosition = this.originalPosition;
			}
			this.BeginTransition();
		}

		// Token: 0x060025C2 RID: 9666 RVA: 0x000A4794 File Offset: 0x000A2994
		public void TransitionPanToLeft()
		{
			this.InitializeFirstTimeInfo();
			if (this.panningRect)
			{
				this.transitionStartPosition = this.originalPosition;
				this.transitionEndPosition = new Vector2(-1f, 0f) * this.panningMagnitude;
			}
			this.BeginTransition();
		}

		// Token: 0x060025C3 RID: 9667 RVA: 0x000A47E8 File Offset: 0x000A29E8
		public void TransitionPanFromRight()
		{
			this.InitializeFirstTimeInfo();
			if (this.panningRect)
			{
				this.transitionStartPosition = new Vector2(1f, 0f) * this.panningMagnitude;
				this.transitionEndPosition = this.originalPosition;
			}
			this.BeginTransition();
		}

		// Token: 0x060025C4 RID: 9668 RVA: 0x000A483C File Offset: 0x000A2A3C
		public void TransitionPanToRight()
		{
			this.InitializeFirstTimeInfo();
			if (this.panningRect)
			{
				this.transitionStartPosition = this.originalPosition;
				this.transitionEndPosition = new Vector2(1f, 0f) * this.panningMagnitude;
			}
			this.BeginTransition();
		}

		// Token: 0x060025C5 RID: 9669 RVA: 0x000A4890 File Offset: 0x000A2A90
		public void TransitionPanFromTop()
		{
			this.InitializeFirstTimeInfo();
			if (this.panningRect)
			{
				this.transitionStartPosition = new Vector2(0f, 1f) * this.panningMagnitude;
				this.transitionEndPosition = this.originalPosition;
			}
			this.BeginTransition();
		}

		// Token: 0x060025C6 RID: 9670 RVA: 0x000A48E4 File Offset: 0x000A2AE4
		public void TransitionPanToTop()
		{
			this.InitializeFirstTimeInfo();
			if (this.panningRect)
			{
				this.transitionStartPosition = this.originalPosition;
				this.transitionEndPosition = new Vector2(0f, 1f) * this.panningMagnitude;
			}
			this.BeginTransition();
		}

		// Token: 0x060025C7 RID: 9671 RVA: 0x000A4938 File Offset: 0x000A2B38
		public void TransitionPanFromBottom()
		{
			this.InitializeFirstTimeInfo();
			if (this.panningRect)
			{
				this.transitionStartPosition = new Vector2(0f, -1f) * this.panningMagnitude;
				this.transitionEndPosition = this.originalPosition;
			}
			this.BeginTransition();
		}

		// Token: 0x060025C8 RID: 9672 RVA: 0x000A498C File Offset: 0x000A2B8C
		public void TransitionPanToBottom()
		{
			this.InitializeFirstTimeInfo();
			if (this.panningRect)
			{
				this.transitionStartPosition = this.originalPosition;
				this.transitionEndPosition = new Vector2(0f, -1f) * this.panningMagnitude;
			}
			this.BeginTransition();
		}

		// Token: 0x060025C9 RID: 9673 RVA: 0x000A49DE File Offset: 0x000A2BDE
		public void TransitionAlphaFadeIn()
		{
			this.InitializeFirstTimeInfo();
			this.transitionStartAlpha = 0f;
			this.transitionEndAlpha = this.originalAlpha;
			this.BeginTransition();
		}

		// Token: 0x060025CA RID: 9674 RVA: 0x000A4A03 File Offset: 0x000A2C03
		public void TransitionAlphaFadeOut()
		{
			this.InitializeFirstTimeInfo();
			this.transitionStartAlpha = this.originalAlpha;
			this.transitionEndAlpha = 0f;
			this.BeginTransition();
		}

		// Token: 0x060025CB RID: 9675 RVA: 0x000A4A28 File Offset: 0x000A2C28
		public void DestroyOnEndOfTransition(bool set)
		{
			this.destroyOnEndOfTransition = set;
		}

		// Token: 0x060025CC RID: 9676 RVA: 0x000A4A31 File Offset: 0x000A2C31
		private void BeginTransition()
		{
			this.transitionStopwatch = 0f;
			this.ProcessTransition();
		}

		// Token: 0x060025CD RID: 9677 RVA: 0x000A4A44 File Offset: 0x000A2C44
		private void InitializeFirstTimeInfo()
		{
			if (this.hasInitialized)
			{
				return;
			}
			if (this.panningRect)
			{
				this.originalPosition = this.panningRect.anchoredPosition;
				this.originalSize = this.panningRect.sizeDelta;
			}
			if (this.canvasGroup)
			{
				this.originalAlpha = this.canvasGroup.alpha;
				this.transitionEndAlpha = this.originalAlpha;
				this.transitionStartAlpha = this.originalAlpha;
			}
			this.hasInitialized = true;
		}

		// Token: 0x0400237D RID: 9085
		[Header("Transition Settings")]
		public CanvasGroup canvasGroup;

		// Token: 0x0400237E RID: 9086
		public RectTransform panningRect;

		// Token: 0x0400237F RID: 9087
		public float transitionDuration;

		// Token: 0x04002380 RID: 9088
		public float panningMagnitude;

		// Token: 0x04002381 RID: 9089
		public bool destroyOnEndOfTransition;

		// Token: 0x04002382 RID: 9090
		private float transitionStopwatch;

		// Token: 0x04002383 RID: 9091
		private float transitionEndAlpha;

		// Token: 0x04002384 RID: 9092
		private float transitionStartAlpha;

		// Token: 0x04002385 RID: 9093
		private float originalAlpha;

		// Token: 0x04002386 RID: 9094
		private Vector2 transitionStartPosition;

		// Token: 0x04002387 RID: 9095
		private Vector2 transitionEndPosition;

		// Token: 0x04002388 RID: 9096
		private Vector2 originalPosition;

		// Token: 0x04002389 RID: 9097
		private Vector2 transitionStartSize;

		// Token: 0x0400238A RID: 9098
		private Vector2 transitionEndSize;

		// Token: 0x0400238B RID: 9099
		private Vector3 originalSize;

		// Token: 0x0400238C RID: 9100
		private bool hasInitialized;
	}
}
