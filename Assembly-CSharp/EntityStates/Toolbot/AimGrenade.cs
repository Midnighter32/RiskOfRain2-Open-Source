using System;

namespace EntityStates.Toolbot
{
	// Token: 0x020000D8 RID: 216
	public abstract class AimGrenade : AimThrowableBase
	{
		// Token: 0x06000443 RID: 1091 RVA: 0x00011BDD File Offset: 0x0000FDDD
		public override void OnEnter()
		{
			base.OnEnter();
			this.detonationRadius = DroneProjectileHover.pulseRadius;
		}
	}
}
