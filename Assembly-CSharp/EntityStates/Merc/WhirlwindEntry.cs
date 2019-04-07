using System;

namespace EntityStates.Merc
{
	// Token: 0x0200010C RID: 268
	public class WhirlwindEntry : BaseState
	{
		// Token: 0x0600052A RID: 1322 RVA: 0x00016C4C File Offset: 0x00014E4C
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.isAuthority)
			{
				EntityState nextState = (base.characterMotor && base.characterMotor.isGrounded) ? new WhirlwindGround() : new WhirlwindAir();
				this.outer.SetNextState(nextState);
				return;
			}
		}
	}
}
