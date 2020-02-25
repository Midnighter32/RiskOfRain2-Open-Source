using System;

namespace EntityStates
{
	// Token: 0x02000716 RID: 1814
	public class LockSkill : BaseState
	{
		// Token: 0x06002A4D RID: 10829 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Death;
		}
	}
}
