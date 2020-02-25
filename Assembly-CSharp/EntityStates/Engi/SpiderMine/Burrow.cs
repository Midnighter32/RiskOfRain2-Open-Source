using System;

namespace EntityStates.Engi.SpiderMine
{
	// Token: 0x0200086C RID: 2156
	public class Burrow : BaseSpiderMineState
	{
		// Token: 0x0600309E RID: 12446 RVA: 0x000D183B File Offset: 0x000CFA3B
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = Burrow.baseDuration;
			base.PlayAnimation("Base", "IdleToArmed", "IdleToArmed.playbackRate", this.duration);
		}

		// Token: 0x0600309F RID: 12447 RVA: 0x000D186C File Offset: 0x000CFA6C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				EntityState entityState = null;
				if (!base.projectileStickOnImpact.stuck)
				{
					entityState = new WaitForStick();
				}
				else if (this.duration <= base.fixedAge)
				{
					entityState = new WaitForTarget();
				}
				if (entityState != null)
				{
					this.outer.SetNextState(entityState);
				}
			}
		}

		// Token: 0x17000449 RID: 1097
		// (get) Token: 0x060030A0 RID: 12448 RVA: 0x0000B933 File Offset: 0x00009B33
		protected override bool shouldStick
		{
			get
			{
				return true;
			}
		}

		// Token: 0x04002EF7 RID: 12023
		public static float baseDuration;

		// Token: 0x04002EF8 RID: 12024
		private float duration;
	}
}
