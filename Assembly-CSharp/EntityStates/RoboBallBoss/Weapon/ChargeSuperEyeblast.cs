using System;

namespace EntityStates.RoboBallBoss.Weapon
{
	// Token: 0x0200079F RID: 1951
	public class ChargeSuperEyeblast : ChargeEyeblast
	{
		// Token: 0x06002CB1 RID: 11441 RVA: 0x000BC969 File Offset: 0x000BAB69
		public override EntityState GetNextState()
		{
			return new FireSuperEyeblast();
		}
	}
}
