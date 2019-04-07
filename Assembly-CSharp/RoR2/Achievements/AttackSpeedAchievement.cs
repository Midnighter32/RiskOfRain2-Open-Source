using System;

namespace RoR2.Achievements
{
	// Token: 0x02000687 RID: 1671
	[RegisterAchievement("AttackSpeed", "Items.AttackSpeedOnCrit", null, null)]
	public class AttackSpeedAchievement : BaseAchievement
	{
		// Token: 0x06002549 RID: 9545 RVA: 0x000AED71 File Offset: 0x000ACF71
		public override void OnInstall()
		{
			base.OnInstall();
			RoR2Application.onUpdate += this.CheckAttackSpeed;
		}

		// Token: 0x0600254A RID: 9546 RVA: 0x000AED8A File Offset: 0x000ACF8A
		public override void OnUninstall()
		{
			base.OnUninstall();
			RoR2Application.onUpdate -= this.CheckAttackSpeed;
		}

		// Token: 0x0600254B RID: 9547 RVA: 0x000AEDA3 File Offset: 0x000ACFA3
		public void CheckAttackSpeed()
		{
			if (this.localUser.cachedBody && this.localUser.cachedBody.attackSpeed >= 3f)
			{
				base.Grant();
			}
		}

		// Token: 0x04002856 RID: 10326
		private const float requirement = 3f;
	}
}
