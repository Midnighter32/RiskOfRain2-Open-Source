using System;

namespace EntityStates.TitanMonster
{
	// Token: 0x02000169 RID: 361
	internal class ChargeGoldMegaLaser : ChargeMegaLaser
	{
		// Token: 0x06000700 RID: 1792 RVA: 0x00021754 File Offset: 0x0001F954
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
