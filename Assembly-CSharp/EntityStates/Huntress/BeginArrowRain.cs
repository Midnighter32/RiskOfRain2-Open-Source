using System;

namespace EntityStates.Huntress
{
	// Token: 0x0200082E RID: 2094
	public class BeginArrowRain : BaseBeginArrowBarrage
	{
		// Token: 0x06002F6B RID: 12139 RVA: 0x000CA940 File Offset: 0x000C8B40
		protected override EntityState InstantiateNextState()
		{
			return new ArrowRain();
		}
	}
}
