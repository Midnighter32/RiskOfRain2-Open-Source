using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Croco
{
	// Token: 0x020008AE RID: 2222
	public class WakeUp : BaseState
	{
		// Token: 0x060031D0 RID: 12752 RVA: 0x000D699C File Offset: 0x000D4B9C
		public override void OnEnter()
		{
			base.OnEnter();
			base.modelLocator.normalizeToFloor = true;
			this.modelAnimator = base.GetModelAnimator();
			if (this.modelAnimator)
			{
				this.modelAnimator.SetFloat(AnimationParameters.aimWeight, 0f);
			}
			base.PlayAnimation("Body", "SleepToIdle", "SleepToIdle.playbackRate", WakeUp.duration);
		}

		// Token: 0x060031D1 RID: 12753 RVA: 0x000D6A03 File Offset: 0x000D4C03
		public override void Update()
		{
			base.Update();
			if (this.modelAnimator)
			{
				this.modelAnimator.SetFloat(AnimationParameters.aimWeight, Mathf.Clamp01((base.age - WakeUp.delayBeforeAimAnimatorWeight) / WakeUp.duration));
			}
		}

		// Token: 0x060031D2 RID: 12754 RVA: 0x000D6A3F File Offset: 0x000D4C3F
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= WakeUp.duration)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x060031D3 RID: 12755 RVA: 0x000D6A5F File Offset: 0x000D4C5F
		public override void OnExit()
		{
			if (this.modelAnimator)
			{
				this.modelAnimator.SetFloat(AnimationParameters.aimWeight, 1f);
			}
			base.OnExit();
		}

		// Token: 0x0400305E RID: 12382
		public static float duration;

		// Token: 0x0400305F RID: 12383
		public static float delayBeforeAimAnimatorWeight;

		// Token: 0x04003060 RID: 12384
		private Animator modelAnimator;
	}
}
