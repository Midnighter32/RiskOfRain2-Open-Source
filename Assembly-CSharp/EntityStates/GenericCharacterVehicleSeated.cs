using System;

namespace EntityStates
{
	// Token: 0x0200070F RID: 1807
	public class GenericCharacterVehicleSeated : BaseState
	{
		// Token: 0x06002A30 RID: 10800 RVA: 0x0000C88C File Offset: 0x0000AA8C
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Vehicle;
		}
	}
}
