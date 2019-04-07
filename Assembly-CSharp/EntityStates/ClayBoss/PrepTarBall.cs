using System;
using RoR2;
using UnityEngine;

namespace EntityStates.ClayBoss
{
	// Token: 0x020001B9 RID: 441
	internal class PrepTarBall : BaseState
	{
		// Token: 0x060008A3 RID: 2211 RVA: 0x0002B4F0 File Offset: 0x000296F0
		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			this.duration = PrepTarBall.baseDuration / this.attackSpeedStat;
			this.modelAnimator = base.GetModelAnimator();
			if (this.modelAnimator)
			{
				base.PlayCrossfade("Body", "PrepTarBall", "PrepTarBall.playbackRate", this.duration, 0.5f);
			}
			if (!string.IsNullOrEmpty(PrepTarBall.prepTarBallSoundString))
			{
				Util.PlayScaledSound(PrepTarBall.prepTarBallSoundString, base.gameObject, this.attackSpeedStat);
			}
		}

		// Token: 0x060008A4 RID: 2212 RVA: 0x0002B57C File Offset: 0x0002977C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.duration && base.isAuthority)
			{
				this.outer.SetNextState(new FireTarball());
				return;
			}
		}

		// Token: 0x060008A5 RID: 2213 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000B90 RID: 2960
		public static float baseDuration = 3f;

		// Token: 0x04000B91 RID: 2961
		public static string prepTarBallSoundString;

		// Token: 0x04000B92 RID: 2962
		private float duration;

		// Token: 0x04000B93 RID: 2963
		private float stopwatch;

		// Token: 0x04000B94 RID: 2964
		private Animator modelAnimator;
	}
}
