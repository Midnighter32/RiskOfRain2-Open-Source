using System;

namespace EntityStates
{
	// Token: 0x020006FF RID: 1791
	public class BigCharacterMain : GenericCharacterMain
	{
		// Token: 0x060029A9 RID: 10665 RVA: 0x000AF86C File Offset: 0x000ADA6C
		public override void ProcessJump()
		{
			if (base.characterMotor.jumpCount > base.characterBody.baseJumpCount)
			{
				base.ProcessJump();
				return;
			}
			if (this.jumpInputReceived && base.characterMotor.isGrounded)
			{
				this.outer.SetNextState(new AnimatedJump());
			}
		}
	}
}
