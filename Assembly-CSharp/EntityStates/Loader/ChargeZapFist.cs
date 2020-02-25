using System;

namespace EntityStates.Loader
{
	// Token: 0x020007E4 RID: 2020
	public class ChargeZapFist : BaseChargeFist
	{
		// Token: 0x06002DFD RID: 11773 RVA: 0x000C3CF8 File Offset: 0x000C1EF8
		protected override bool ShouldKeepChargingAuthority()
		{
			return base.fixedAge < base.chargeDuration;
		}

		// Token: 0x06002DFE RID: 11774 RVA: 0x000C3D08 File Offset: 0x000C1F08
		protected override EntityState GetNextStateAuthority()
		{
			return new SwingZapFist
			{
				charge = base.charge
			};
		}
	}
}
