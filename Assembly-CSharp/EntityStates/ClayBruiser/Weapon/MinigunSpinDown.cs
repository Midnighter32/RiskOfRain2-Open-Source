using System;
using RoR2;

namespace EntityStates.ClayBruiser.Weapon
{
	// Token: 0x020008CA RID: 2250
	public class MinigunSpinDown : MinigunState
	{
		// Token: 0x06003273 RID: 12915 RVA: 0x000DA284 File Offset: 0x000D8484
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = MinigunSpinDown.baseDuration / this.attackSpeedStat;
			Util.PlayScaledSound(MinigunSpinDown.sound, base.gameObject, this.attackSpeedStat);
			base.GetModelAnimator().SetBool("WeaponIsReady", false);
		}

		// Token: 0x06003274 RID: 12916 RVA: 0x000DA2D1 File Offset: 0x000D84D1
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x04003169 RID: 12649
		public static float baseDuration;

		// Token: 0x0400316A RID: 12650
		public static string sound;

		// Token: 0x0400316B RID: 12651
		private float duration;
	}
}
