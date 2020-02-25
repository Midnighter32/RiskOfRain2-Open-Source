using System;
using EntityStates.GreaterWispMonster;

namespace EntityStates.ArchWispMonster
{
	// Token: 0x02000728 RID: 1832
	public class ChargeCannons : ChargeCannons
	{
		// Token: 0x06002A9D RID: 10909 RVA: 0x000B3468 File Offset: 0x000B1668
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				FireCannons nextState = new FireCannons();
				this.outer.SetNextState(nextState);
				return;
			}
		}
	}
}
