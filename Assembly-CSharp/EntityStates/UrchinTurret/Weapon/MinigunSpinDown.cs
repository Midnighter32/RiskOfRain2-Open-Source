using System;
using RoR2;

namespace EntityStates.UrchinTurret.Weapon
{
	// Token: 0x02000907 RID: 2311
	public class MinigunSpinDown : MinigunState
	{
		// Token: 0x06003391 RID: 13201 RVA: 0x000DFD3A File Offset: 0x000DDF3A
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = MinigunSpinDown.baseDuration / this.attackSpeedStat;
			Util.PlayScaledSound(MinigunSpinDown.sound, base.gameObject, this.attackSpeedStat);
		}

		// Token: 0x06003392 RID: 13202 RVA: 0x000DFD6B File Offset: 0x000DDF6B
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x0400330E RID: 13070
		public static float baseDuration;

		// Token: 0x0400330F RID: 13071
		public static string sound;

		// Token: 0x04003310 RID: 13072
		private float duration;
	}
}
