using System;
using RoR2;

namespace EntityStates.RoboBallBoss.Weapon
{
	// Token: 0x0200079C RID: 1948
	public class EnableEyebeams : BaseState
	{
		// Token: 0x06002CA1 RID: 11425 RVA: 0x000BC524 File Offset: 0x000BA724
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = EnableEyebeams.baseDuration / this.attackSpeedStat;
			Util.PlaySound(EnableEyebeams.soundString, base.gameObject);
			foreach (EntityStateMachine entityStateMachine in base.gameObject.GetComponents<EntityStateMachine>())
			{
				if (entityStateMachine.customName.Contains("EyeBeam"))
				{
					entityStateMachine.SetNextState(new FireSpinningEyeBeam());
				}
			}
		}

		// Token: 0x040028C0 RID: 10432
		public static float baseDuration;

		// Token: 0x040028C1 RID: 10433
		public static string soundString;

		// Token: 0x040028C2 RID: 10434
		private float duration;
	}
}
