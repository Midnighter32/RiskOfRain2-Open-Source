using System;

namespace EntityStates.Merc
{
	// Token: 0x020007CA RID: 1994
	public class WhirlwindEntry : BaseState
	{
		// Token: 0x06002D7F RID: 11647 RVA: 0x000C0F98 File Offset: 0x000BF198
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
