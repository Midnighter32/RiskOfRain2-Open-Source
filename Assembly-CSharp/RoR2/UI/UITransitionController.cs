using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000650 RID: 1616
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(EventFunctions))]
	public class UITransitionController : MonoBehaviour
	{
		// Token: 0x06002430 RID: 9264 RVA: 0x000AA003 File Offset: 0x000A8203
		private void Awake()
		{
			this.animator = base.GetComponent<Animator>();
			this.PushMecanimTransitionInParameters();
		}

		// Token: 0x06002431 RID: 9265 RVA: 0x000AA018 File Offset: 0x000A8218
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

		// Token: 0x06002432 RID: 9266 RVA: 0x000AA09C File Offset: 0x000A829C
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

		// Token: 0x06002433 RID: 9267 RVA: 0x000AA120 File Offset: 0x000A8320
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

		// Token: 0x04002728 RID: 10024
		public UITransitionController.TransitionStyle transitionIn;

		// Token: 0x04002729 RID: 10025
		public UITransitionController.TransitionStyle transitionOut;

		// Token: 0x0400272A RID: 10026
		public float transitionInSpeed = 1f;

		// Token: 0x0400272B RID: 10027
		public float transitionOutSpeed = 1f;

		// Token: 0x0400272C RID: 10028
		public bool transitionOutAtEndOfLifetime;

		// Token: 0x0400272D RID: 10029
		public float lifetime;

		// Token: 0x0400272E RID: 10030
		private float stopwatch;

		// Token: 0x0400272F RID: 10031
		private Animator animator;

		// Token: 0x04002730 RID: 10032
		private bool done;

		// Token: 0x02000651 RID: 1617
		public enum TransitionStyle
		{
			// Token: 0x04002732 RID: 10034
			Instant,
			// Token: 0x04002733 RID: 10035
			CanvasGroupAlphaFade,
			// Token: 0x04002734 RID: 10036
			SwipeYScale,
			// Token: 0x04002735 RID: 10037
			SwipeXScale
		}
	}
}
