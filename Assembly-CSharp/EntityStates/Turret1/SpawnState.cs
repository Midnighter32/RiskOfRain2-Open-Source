using System;

namespace EntityStates.Turret1
{
	// Token: 0x02000895 RID: 2197
	public class SpawnState : BaseState
	{
		// Token: 0x0600314D RID: 12621 RVA: 0x000D4429 File Offset: 0x000D2629
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.GetModelAnimator())
			{
				base.PlayAnimation("Body", "Spawn", "Spawn.playbackRate", 1.5f);
			}
		}

		// Token: 0x0600314E RID: 12622 RVA: 0x000D4458 File Offset: 0x000D2658
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x0600314F RID: 12623 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04002FA5 RID: 12197
		public static float duration = 4f;
	}
}
