using System;
using RoR2;

namespace EntityStates.Mage
{
	// Token: 0x020007CD RID: 1997
	public class MageCharacterMain : GenericCharacterMain
	{
		// Token: 0x06002D8E RID: 11662 RVA: 0x000C1310 File Offset: 0x000BF510
		public override void OnEnter()
		{
			base.OnEnter();
			this.jetpackStateMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Jet");
		}

		// Token: 0x06002D8F RID: 11663 RVA: 0x000C1330 File Offset: 0x000BF530
		public override void ProcessJump()
		{
			base.ProcessJump();
			if (this.hasCharacterMotor && this.hasInputBank && base.isAuthority)
			{
				object obj = base.inputBank.jump.down && base.characterMotor.velocity.y < 0f && !base.characterMotor.isGrounded;
				bool flag = this.jetpackStateMachine.state.GetType() == typeof(JetpackOn);
				object obj2 = obj;
				if (obj2 != null && !flag)
				{
					this.jetpackStateMachine.SetNextState(new JetpackOn());
				}
				if (obj2 == 0 && flag)
				{
					this.jetpackStateMachine.SetNextState(new Idle());
				}
			}
		}

		// Token: 0x04002A2A RID: 10794
		private EntityStateMachine jetpackStateMachine;
	}
}
