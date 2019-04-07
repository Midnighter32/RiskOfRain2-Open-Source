using System;

namespace EntityStates.Turret1
{
	// Token: 0x02000192 RID: 402
	public class SpawnState : BaseState
	{
		// Token: 0x060007BC RID: 1980 RVA: 0x00026586 File Offset: 0x00024786
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.GetModelAnimator())
			{
				base.PlayAnimation("Body", "Spawn", "Spawn.playbackRate", 1.5f);
			}
		}

		// Token: 0x060007BD RID: 1981 RVA: 0x000265B5 File Offset: 0x000247B5
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= SpawnState.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060007BE RID: 1982 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}

		// Token: 0x04000A09 RID: 2569
		public static float duration = 4f;
	}
}
