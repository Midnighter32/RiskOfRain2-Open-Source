using System;

namespace RoR2.Achievements
{
	// Token: 0x020006A6 RID: 1702
	[RegisterAchievement("KillBossQuick", "Items.TreasureCache", null, typeof(KillBossQuickAchievement.KillBossQuickServerAchievement))]
	public class KillBossQuickAchievement : BaseAchievement
	{
		// Token: 0x060025E1 RID: 9697 RVA: 0x000AF2C0 File Offset: 0x000AD4C0
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x060025E2 RID: 9698 RVA: 0x000AF2CF File Offset: 0x000AD4CF
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006A7 RID: 1703
		private class KillBossQuickServerAchievement : BaseServerAchievement
		{
			// Token: 0x060025E4 RID: 9700 RVA: 0x000AFE48 File Offset: 0x000AE048
			public override void OnInstall()
			{
				base.OnInstall();
				BossGroup.onBossGroupDefeatedServer += this.OnBossGroupDefeatedServer;
			}

			// Token: 0x060025E5 RID: 9701 RVA: 0x000AFE61 File Offset: 0x000AE061
			public override void OnUninstall()
			{
				BossGroup.onBossGroupDefeatedServer -= this.OnBossGroupDefeatedServer;
				base.OnUninstall();
			}

			// Token: 0x060025E6 RID: 9702 RVA: 0x000AFE7A File Offset: 0x000AE07A
			private void OnBossGroupDefeatedServer(BossGroup bossGroup)
			{
				if (bossGroup.fixedAge <= 15f)
				{
					base.Grant();
				}
			}

			// Token: 0x04002873 RID: 10355
			private const float requirement = 15f;
		}
	}
}
