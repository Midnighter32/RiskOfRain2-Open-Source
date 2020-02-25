using System;
using RoR2;

namespace EntityStates.Treebot.Weapon
{
	// Token: 0x0200074B RID: 1867
	public class AimMortar : AimThrowableBase
	{
		// Token: 0x06002B47 RID: 11079 RVA: 0x000B6706 File Offset: 0x000B4906
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(AimMortar.enterSoundString, base.gameObject);
			base.PlayAnimation("Gesture, Additive", "PrepBomb", "PrepBomb.playbackRate", this.minimumDuration);
		}

		// Token: 0x06002B48 RID: 11080 RVA: 0x000B673A File Offset: 0x000B493A
		public override void OnExit()
		{
			base.OnExit();
			this.outer.SetNextState(new FireMortar());
			if (!this.outer.destroying)
			{
				Util.PlaySound(AimMortar.exitSoundString, base.gameObject);
			}
		}

		// Token: 0x06002B49 RID: 11081 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002738 RID: 10040
		public static string enterSoundString;

		// Token: 0x04002739 RID: 10041
		public static string exitSoundString;
	}
}
