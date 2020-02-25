using System;
using RoR2;

namespace EntityStates.Toolbot
{
	// Token: 0x0200075D RID: 1885
	public class AimStunDrone : AimGrenade
	{
		// Token: 0x06002B93 RID: 11155 RVA: 0x000B8014 File Offset: 0x000B6214
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(AimStunDrone.enterSoundString, base.gameObject);
			base.PlayAnimation("Gesture, Additive", "PrepBomb", "PrepBomb.playbackRate", this.minimumDuration);
			base.PlayAnimation("Stance, Override", "PutAwayGun");
		}

		// Token: 0x06002B94 RID: 11156 RVA: 0x000B8063 File Offset: 0x000B6263
		public override void OnExit()
		{
			base.OnExit();
			this.outer.SetNextState(new RecoverAimStunDrone());
			Util.PlaySound(AimStunDrone.exitSoundString, base.gameObject);
		}

		// Token: 0x06002B95 RID: 11157 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x040027AB RID: 10155
		public static string enterSoundString;

		// Token: 0x040027AC RID: 10156
		public static string exitSoundString;
	}
}
