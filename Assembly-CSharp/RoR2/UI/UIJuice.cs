using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200064F RID: 1615
	public class UIJuice : MonoBehaviour
	{
		// Token: 0x0600241E RID: 9246 RVA: 0x000A9A61 File Offset: 0x000A7C61
		private void Awake()
		{
			this.InitializeFirstTimeInfo();
		}

		// Token: 0x0600241F RID: 9247 RVA: 0x000A9A69 File Offset: 0x000A7C69
		private void Update()
		{
			this.transitionStopwatch = Mathf.Min(this.transitionStopwatch + Time.unscaledDeltaTime, this.transitionDuration);
			this.ProcessTransition();
		}

		// Token: 0x06002420 RID: 9248 RVA: 0x000A9A90 File Offset: 0x000A7C90
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

		// Token: 0x06002421 RID: 9249 RVA: 0x000A9C20 File Offset: 0x000A7E20
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

		// Token: 0x06002422 RID: 9250 RVA: 0x000A9C78 File Offset: 0x000A7E78
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

		// Token: 0x06002423 RID: 9251 RVA: 0x000A9CCC File Offset: 0x000A7ECC
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

		// Token: 0x06002424 RID: 9252 RVA: 0x000A9D20 File Offset: 0x000A7F20
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

		// Token: 0x06002425 RID: 9253 RVA: 0x000A9D74 File Offset: 0x000A7F74
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

		// Token: 0x06002426 RID: 9254 RVA: 0x000A9DC8 File Offset: 0x000A7FC8
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

		// Token: 0x06002427 RID: 9255 RVA: 0x000A9E1C File Offset: 0x000A801C
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

		// Token: 0x06002428 RID: 9256 RVA: 0x000A9E70 File Offset: 0x000A8070
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

		// Token: 0x06002429 RID: 9257 RVA: 0x000A9EC4 File Offset: 0x000A80C4
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

		// Token: 0x0600242A RID: 9258 RVA: 0x000A9F16 File Offset: 0x000A8116
		public void TransitionAlphaFadeIn()
		{
			this.InitializeFirstTimeInfo();
			this.transitionStartAlpha = 0f;
			this.transitionEndAlpha = this.originalAlpha;
			this.BeginTransition();
		}

		// Token: 0x0600242B RID: 9259 RVA: 0x000A9F3B File Offset: 0x000A813B
		public void TransitionAlphaFadeOut()
		{
			this.InitializeFirstTimeInfo();
			this.transitionStartAlpha = this.originalAlpha;
			this.transitionEndAlpha = 0f;
			this.BeginTransition();
		}

		// Token: 0x0600242C RID: 9260 RVA: 0x000A9F60 File Offset: 0x000A8160
		public void DestroyOnEndOfTransition(bool set)
		{
			this.destroyOnEndOfTransition = set;
		}

		// Token: 0x0600242D RID: 9261 RVA: 0x000A9F69 File Offset: 0x000A8169
		private void BeginTransition()
		{
			this.transitionStopwatch = 0f;
			this.ProcessTransition();
		}

		// Token: 0x0600242E RID: 9262 RVA: 0x000A9F7C File Offset: 0x000A817C
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

		// Token: 0x04002718 RID: 10008
		[Header("Transition Settings")]
		public CanvasGroup canvasGroup;

		// Token: 0x04002719 RID: 10009
		public RectTransform panningRect;

		// Token: 0x0400271A RID: 10010
		public float transitionDuration;

		// Token: 0x0400271B RID: 10011
		public float panningMagnitude;

		// Token: 0x0400271C RID: 10012
		public bool destroyOnEndOfTransition;

		// Token: 0x0400271D RID: 10013
		private float transitionStopwatch;

		// Token: 0x0400271E RID: 10014
		private float transitionEndAlpha;

		// Token: 0x0400271F RID: 10015
		private float transitionStartAlpha;

		// Token: 0x04002720 RID: 10016
		private float originalAlpha;

		// Token: 0x04002721 RID: 10017
		private Vector2 transitionStartPosition;

		// Token: 0x04002722 RID: 10018
		private Vector2 transitionEndPosition;

		// Token: 0x04002723 RID: 10019
		private Vector2 originalPosition;

		// Token: 0x04002724 RID: 10020
		private Vector2 transitionStartSize;

		// Token: 0x04002725 RID: 10021
		private Vector2 transitionEndSize;

		// Token: 0x04002726 RID: 10022
		private Vector3 originalSize;

		// Token: 0x04002727 RID: 10023
		private bool hasInitialized;
	}
}
