using System;

namespace EntityStates.Huntress
{
	// Token: 0x02000829 RID: 2089
	public class BeginArrowSnipe : BaseBeginArrowBarrage
	{
		// Token: 0x06002F51 RID: 12113 RVA: 0x000C9FEA File Offset: 0x000C81EA
		protected override EntityState InstantiateNextState()
		{
			return new AimArrowSnipe();
		}
	}
}
