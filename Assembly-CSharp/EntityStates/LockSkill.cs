using System;

namespace EntityStates
{
	// Token: 0x020000BC RID: 188
	public class LockSkill : BaseState
	{
		// Token: 0x060003AF RID: 943 RVA: 0x0000BBE7 File Offset: 0x00009DE7
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}
	}
}
