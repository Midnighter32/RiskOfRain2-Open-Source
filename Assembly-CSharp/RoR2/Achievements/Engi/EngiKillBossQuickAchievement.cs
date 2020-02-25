using System;

namespace RoR2.Achievements.Engi
{
	// Token: 0x020006ED RID: 1773
	[RegisterAchievement("EngiKillBossQuick", "Skills.Engi.SpiderMine", "Complete30StagesCareer", typeof(EngiKillBossQuickAchievement.EngiKillBossQuickServerAchievement))]
	public class EngiKillBossQuickAchievement : BaseAchievement
	{
		// Token: 0x06002937 RID: 10551 RVA: 0x000AD916 File Offset: 0x000ABB16
		protected override int LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("EngiBody");
		}

		// Token: 0x06002938 RID: 10552 RVA: 0x000AC8DE File Offset: 0x000AAADE
		protected override void OnBodyRequirementMet()
		{
			base.OnBodyRequirementMet();
			base.SetServerTracked(true);
		}

		// Token: 0x06002939 RID: 10553 RVA: 0x000AC8ED File Offset: 0x000AAAED
		protected override void OnBodyRequirementBroken()
		{
			base.SetServerTracked(false);
			base.OnBodyRequirementBroken();
		}

		// Token: 0x020006EE RID: 1774
		private class EngiKillBossQuickServerAchievement : BaseServerAchievement
		{
			// Token: 0x0600293B RID: 10555 RVA: 0x000ADA04 File Offset: 0x000ABC04
			public override void OnInstall()
			{
				base.OnInstall();
				BossGroup.onBossGroupDefeatedServer += this.OnBossGroupDefeatedServer;
			}

			// Token: 0x0600293C RID: 10556 RVA: 0x000ADA1D File Offset: 0x000ABC1D
			public override void OnUninstall()
			{
				BossGroup.onBossGroupDefeatedServer -= this.OnBossGroupDefeatedServer;
				base.OnUninstall();
			}

			// Token: 0x0600293D RID: 10557 RVA: 0x000ADA36 File Offset: 0x000ABC36
			private void OnBossGroupDefeatedServer(BossGroup bossGroup)
			{
				if (bossGroup.fixedTimeSinceEnabled <= 5f)
				{
					base.Grant();
				}
			}

			// Token: 0x0400254B RID: 9547
			private const float requirement = 5f;
		}
	}
}
