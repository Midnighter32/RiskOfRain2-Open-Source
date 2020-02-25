using System;

namespace RoR2.Achievements
{
	// Token: 0x020006AF RID: 1711
	[RegisterAchievement("KillBossQuick", "Items.TreasureCache", null, typeof(KillBossQuickAchievement.KillBossQuickServerAchievement))]
	public class KillBossQuickAchievement : BaseAchievement
	{
		// Token: 0x060027F3 RID: 10227 RVA: 0x000AA636 File Offset: 0x000A8836
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x060027F4 RID: 10228 RVA: 0x000AA645 File Offset: 0x000A8845
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006B0 RID: 1712
		private class KillBossQuickServerAchievement : BaseServerAchievement
		{
			// Token: 0x060027F6 RID: 10230 RVA: 0x000AB763 File Offset: 0x000A9963
			public override void OnInstall()
			{
				base.OnInstall();
				BossGroup.onBossGroupDefeatedServer += this.OnBossGroupDefeatedServer;
			}

			// Token: 0x060027F7 RID: 10231 RVA: 0x000AB77C File Offset: 0x000A997C
			public override void OnUninstall()
			{
				BossGroup.onBossGroupDefeatedServer -= this.OnBossGroupDefeatedServer;
				base.OnUninstall();
			}

			// Token: 0x060027F8 RID: 10232 RVA: 0x000AB795 File Offset: 0x000A9995
			private void OnBossGroupDefeatedServer(BossGroup bossGroup)
			{
				if (bossGroup.fixedTimeSinceEnabled <= 15f)
				{
					base.Grant();
				}
			}

			// Token: 0x04002503 RID: 9475
			private const float requirement = 15f;
		}
	}
}
