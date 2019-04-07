using System;

namespace EntityStates.Toolbot
{
	// Token: 0x020000E1 RID: 225
	public class DroneProjectilePrepHover : BaseState
	{
		// Token: 0x06000468 RID: 1128 RVA: 0x00012892 File Offset: 0x00010A92
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.age >= DroneProjectilePrepHover.duration)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x04000435 RID: 1077
		public static float duration;
	}
}
