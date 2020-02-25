using System;

namespace EntityStates.TitanMonster
{
	// Token: 0x0200084F RID: 2127
	public class ChargeGoldMegaLaser : ChargeMegaLaser
	{
		// Token: 0x0600301B RID: 12315 RVA: 0x000CE54C File Offset: 0x000CC74C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				FireGoldMegaLaser nextState = new FireGoldMegaLaser();
				this.outer.SetNextState(nextState);
				return;
			}
		}
	}
}
