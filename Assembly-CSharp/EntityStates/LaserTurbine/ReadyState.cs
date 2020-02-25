using System;

namespace EntityStates.LaserTurbine
{
	// Token: 0x020007FA RID: 2042
	public class ReadyState : LaserTurbineBaseState
	{
		// Token: 0x06002E76 RID: 11894 RVA: 0x000C56E6 File Offset: 0x000C38E6
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.fixedAge >= ReadyState.baseDuration)
			{
				this.outer.SetNextState(new AimState());
			}
		}

		// Token: 0x06002E77 RID: 11895 RVA: 0x000C5713 File Offset: 0x000C3913
		public override float GetChargeFraction()
		{
			return 1f;
		}

		// Token: 0x04002B8D RID: 11149
		public static float baseDuration;
	}
}
