using System;

namespace EntityStates
{
	// Token: 0x0200070D RID: 1805
	public class GenericCharacterPod : BaseState
	{
		// Token: 0x06002A29 RID: 10793 RVA: 0x000B1746 File Offset: 0x000AF946
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.characterMotor)
			{
				base.characterMotor.enabled = false;
			}
			if (base.rigidbodyMotor)
			{
				base.rigidbodyMotor.enabled = false;
			}
		}

		// Token: 0x06002A2A RID: 10794 RVA: 0x000B1780 File Offset: 0x000AF980
		public override void OnExit()
		{
			if (base.characterMotor)
			{
				base.characterMotor.enabled = true;
			}
			if (base.rigidbodyMotor)
			{
				base.rigidbodyMotor.enabled = true;
			}
			base.OnExit();
		}
	}
}
