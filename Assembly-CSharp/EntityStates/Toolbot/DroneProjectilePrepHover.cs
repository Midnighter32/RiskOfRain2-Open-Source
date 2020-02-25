using System;

namespace EntityStates.Toolbot
{
	// Token: 0x02000768 RID: 1896
	public class DroneProjectilePrepHover : BaseState
	{
		// Token: 0x06002BC3 RID: 11203 RVA: 0x000B918A File Offset: 0x000B738A
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.age >= DroneProjectilePrepHover.duration)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x040027F0 RID: 10224
		public static float duration;
	}
}
