using System;

namespace EntityStates.AncientWispMonster
{
	// Token: 0x02000734 RID: 1844
	public class FireBomb : BaseState
	{
		// Token: 0x06002ADB RID: 10971 RVA: 0x000B4650 File Offset: 0x000B2850
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireBomb.baseDuration / this.attackSpeedStat;
			base.PlayAnimation("Gesture", "FireBomb", "FireBomb.playbackRate", this.duration);
		}

		// Token: 0x06002ADC RID: 10972 RVA: 0x000B4685 File Offset: 0x000B2885
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002ADD RID: 10973 RVA: 0x0000C68F File Offset: 0x0000A88F
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Frozen;
		}

		// Token: 0x040026B4 RID: 9908
		public static float baseDuration = 4f;

		// Token: 0x040026B5 RID: 9909
		private float duration;
	}
}
