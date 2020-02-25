using System;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020008C0 RID: 2240
	public class FireShotgunBlast : GenericBulletBaseState
	{
		// Token: 0x0600323D RID: 12861 RVA: 0x000D9210 File Offset: 0x000D7410
		public override void OnEnter()
		{
			this.muzzleName = "MuzzleLeft";
			base.OnEnter();
			base.PlayAnimation("Gesture Additive, Left", "FirePistol, Left");
			base.PlayAnimation("Gesture Override, Left", "FirePistol, Left");
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(2f);
			}
		}

		// Token: 0x0600323E RID: 12862 RVA: 0x000D926C File Offset: 0x000D746C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (!this.hasFiredSecondBlast && FireShotgunBlast.delayBetweenShotgunBlasts / this.attackSpeedStat < base.fixedAge)
			{
				this.hasFiredSecondBlast = true;
				base.PlayAnimation("Gesture Additive, Right", "FirePistol, Right");
				base.PlayAnimation("Gesture Override, Right", "FirePistol, Right");
				this.muzzleName = "MuzzleRight";
				this.FireBullet(base.GetAimRay());
			}
		}

		// Token: 0x0600323F RID: 12863 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04003118 RID: 12568
		public static float delayBetweenShotgunBlasts;

		// Token: 0x04003119 RID: 12569
		private bool hasFiredSecondBlast;
	}
}
