using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000645 RID: 1605
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(EventFunctions))]
	public class UITransitionController : MonoBehaviour
	{
		// Token: 0x060025CF RID: 9679 RVA: 0x000A4ACB File Offset: 0x000A2CCB
		private void Awake()
		{
			this.animator = base.GetComponent<Animator>();
			this.PushMecanimTransitionInParameters();
		}

		// Token: 0x060025D0 RID: 9680 RVA: 0x000A4AE0 File Offset: 0x000A2CE0
		private void PushMecanimTransitionInParameters()
		{
			this.animator.SetFloat("transitionInSpeed", this.transitionInSpeed);
			switch (this.transitionIn)
			{
			case UITransitionController.TransitionStyle.Instant:
				this.animator.SetTrigger("InstantIn");
				return;
			case UITransitionController.TransitionStyle.CanvasGroupAlphaFade:
				this.animator.SetTrigger("CanvasGroupAlphaFadeIn");
				return;
			case UITransitionController.TransitionStyle.SwipeYScale:
				this.animator.SetTrigger("SwipeYScaleIn");
				return;
			case UITransitionController.TransitionStyle.SwipeXScale:
				this.animator.SetTrigger("SwipeXScaleIn");
				return;
			default:
				return;
			}
		}

		// Token: 0x060025D1 RID: 9681 RVA: 0x000A4B64 File Offset: 0x000A2D64
		private void PushMecanimTransitionOutParameters()
		{
			this.animator.SetFloat("transitionOutSpeed", this.transitionOutSpeed);
			switch (this.transitionOut)
			{
			case UITransitionController.TransitionStyle.Instant:
				this.animator.SetTrigger("InstantOut");
				return;
			case UITransitionController.TransitionStyle.CanvasGroupAlphaFade:
				this.animator.SetTrigger("CanvasGroupAlphaFadeOut");
				return;
			case UITransitionController.TransitionStyle.SwipeYScale:
				this.animator.SetTrigger("SwipeYScaleOut");
				return;
			case UITransitionController.TransitionStyle.SwipeXScale:
				this.animator.SetTrigger("SwipeXScaleOut");
				return;
			default:
				return;
			}
		}

		// Token: 0x060025D2 RID: 9682 RVA: 0x000A4BE8 File Offset: 0x000A2DE8
		private void Update()
		{
			if (this.transitionOutAtEndOfLifetime && !this.done)
			{
				this.stopwatch += Time.deltaTime;
				if (this.stopwatch >= this.lifetime)
				{
					this.PushMecanimTransitionOutParameters();
					this.done = true;
				}
			}
		}

		// Token: 0x0400238D RID: 9101
		public UITransitionController.TransitionStyle transitionIn;

		// Token: 0x0400238E RID: 9102
		public UITransitionController.TransitionStyle transitionOut;

		// Token: 0x0400238F RID: 9103
		public float transitionInSpeed = 1f;

		// Token: 0x04002390 RID: 9104
		public float transitionOutSpeed = 1f;

		// Token: 0x04002391 RID: 9105
		public bool transitionOutAtEndOfLifetime;

		// Token: 0x04002392 RID: 9106
		public float lifetime;

		// Token: 0x04002393 RID: 9107
		private float stopwatch;

		// Token: 0x04002394 RID: 9108
		private Animator animator;

		// Token: 0x04002395 RID: 9109
		private bool done;

		// Token: 0x02000646 RID: 1606
		public enum TransitionStyle
		{
			// Token: 0x04002397 RID: 9111
			Instant,
			// Token: 0x04002398 RID: 9112
			CanvasGroupAlphaFade,
			// Token: 0x04002399 RID: 9113
			SwipeYScale,
			// Token: 0x0400239A RID: 9114
			SwipeXScale
		}
	}
}
