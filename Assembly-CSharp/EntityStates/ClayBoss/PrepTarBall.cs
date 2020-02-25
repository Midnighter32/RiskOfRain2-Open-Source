using System;
using RoR2;
using UnityEngine;

namespace EntityStates.ClayBoss
{
	// Token: 0x020008D4 RID: 2260
	public class PrepTarBall : BaseState
	{
		// Token: 0x060032A2 RID: 12962 RVA: 0x000DB108 File Offset: 0x000D9308
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

		// Token: 0x060032A3 RID: 12963 RVA: 0x000DB194 File Offset: 0x000D9394
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

		// Token: 0x060032A4 RID: 12964 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040031B8 RID: 12728
		public static float baseDuration = 3f;

		// Token: 0x040031B9 RID: 12729
		public static string prepTarBallSoundString;

		// Token: 0x040031BA RID: 12730
		private float duration;

		// Token: 0x040031BB RID: 12731
		private float stopwatch;

		// Token: 0x040031BC RID: 12732
		private Animator modelAnimator;
	}
}
