using System;

namespace EntityStates
{
	// Token: 0x020000B6 RID: 182
	public class GenericCharacterPod : BaseState
	{
		// Token: 0x0600039D RID: 925 RVA: 0x0000EFB3 File Offset: 0x0000D1B3
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

		// Token: 0x0600039E RID: 926 RVA: 0x0000EFED File Offset: 0x0000D1ED
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
