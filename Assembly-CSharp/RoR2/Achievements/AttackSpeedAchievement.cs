using System;

namespace RoR2.Achievements
{
	// Token: 0x02000689 RID: 1673
	[RegisterAchievement("AttackSpeed", "Items.AttackSpeedOnCrit", null, null)]
	public class AttackSpeedAchievement : BaseAchievement
	{
		// Token: 0x0600272C RID: 10028 RVA: 0x000AA261 File Offset: 0x000A8461
		public override void OnInstall()
		{
			base.OnInstall();
			RoR2Application.onUpdate += this.CheckAttackSpeed;
		}

		// Token: 0x0600272D RID: 10029 RVA: 0x000AA27A File Offset: 0x000A847A
		public override void OnUninstall()
		{
			RoR2Application.onUpdate -= this.CheckAttackSpeed;
			base.OnUninstall();
		}

		// Token: 0x0600272E RID: 10030 RVA: 0x000AA293 File Offset: 0x000A8493
		public void CheckAttackSpeed()
		{
			if (base.localUser != null && base.localUser.cachedBody && base.localUser.cachedBody.attackSpeed >= 3f)
			{
				base.Grant();
			}
		}

		// Token: 0x040024DD RID: 9437
		private const float requirement = 3f;
	}
}
