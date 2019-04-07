using System;
using RoR2;

namespace EntityStates.Toolbot
{
	// Token: 0x020000D9 RID: 217
	public class AimStunDrone : AimGrenade
	{
		// Token: 0x06000445 RID: 1093 RVA: 0x00011BF8 File Offset: 0x0000FDF8
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(AimStunDrone.enterSoundString, base.gameObject);
			base.PlayAnimation("Gesture, Additive", "PrepBomb", "PrepBomb.playbackRate", this.minimumDuration);
		}

		// Token: 0x06000446 RID: 1094 RVA: 0x00011C2C File Offset: 0x0000FE2C
		public override void OnExit()
		{
			base.OnExit();
			this.outer.SetNextState(new RecoverAimStunDrone());
			Util.PlaySound(AimStunDrone.exitSoundString, base.gameObject);
		}

		// Token: 0x06000447 RID: 1095 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000405 RID: 1029
		public static string enterSoundString;

		// Token: 0x04000406 RID: 1030
		public static string exitSoundString;
	}
}
