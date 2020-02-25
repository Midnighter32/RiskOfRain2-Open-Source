using System;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020008B7 RID: 2231
	public class ThrowGrenade : FireFMJ
	{
		// Token: 0x06003209 RID: 12809 RVA: 0x000D81C4 File Offset: 0x000D63C4
		protected override void PlayAnimation(float duration)
		{
			if (base.GetModelAnimator())
			{
				base.PlayAnimation("Gesture, Additive", "ThrowGrenade", "FireFMJ.playbackRate", duration * 2f);
				base.PlayAnimation("Gesture, Override", "ThrowGrenade", "FireFMJ.playbackRate", duration * 2f);
			}
		}

		// Token: 0x0600320A RID: 12810 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}
	}
}
