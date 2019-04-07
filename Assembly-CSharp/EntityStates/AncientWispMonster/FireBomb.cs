using System;

namespace EntityStates.AncientWispMonster
{
	// Token: 0x020000D5 RID: 213
	public class FireBomb : BaseState
	{
		// Token: 0x06000432 RID: 1074 RVA: 0x00011607 File Offset: 0x0000F807
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireBomb.baseDuration / this.attackSpeedStat;
			base.PlayAnimation("Gesture", "FireBomb", "FireBomb.playbackRate", this.duration);
		}

		// Token: 0x06000433 RID: 1075 RVA: 0x0001163C File Offset: 0x0000F83C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06000434 RID: 1076 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x040003EE RID: 1006
		public static float baseDuration = 4f;

		// Token: 0x040003EF RID: 1007
		private float duration;
	}
}
