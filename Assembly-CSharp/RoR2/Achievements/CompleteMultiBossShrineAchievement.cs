using System;

namespace RoR2.Achievements
{
	// Token: 0x0200068E RID: 1678
	[RegisterAchievement("CompleteMultiBossShrine", "Items.Lightning", null, typeof(CompleteMultiBossShrineAchievement.CompleteMultiBossShrineServerAchievement))]
	public class CompleteMultiBossShrineAchievement : BaseAchievement
	{
		// Token: 0x06002571 RID: 9585 RVA: 0x000AF2C0 File Offset: 0x000AD4C0
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x06002572 RID: 9586 RVA: 0x000AF2CF File Offset: 0x000AD4CF
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x0200068F RID: 1679
		private class CompleteMultiBossShrineServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002574 RID: 9588 RVA: 0x000AF2D7 File Offset: 0x000AD4D7
			public override void OnInstall()
			{
				base.OnInstall();
				BossGroup.onBossGroupDefeatedServer += this.OnBossGroupDefeatedServer;
			}

			// Token: 0x06002575 RID: 9589 RVA: 0x000AF2F0 File Offset: 0x000AD4F0
			public override void OnUninstall()
			{
				BossGroup.onBossGroupDefeatedServer -= this.OnBossGroupDefeatedServer;
				base.OnUninstall();
			}

			// Token: 0x06002576 RID: 9590 RVA: 0x000AF30C File Offset: 0x000AD50C
			private void OnBossGroupDefeatedServer(BossGroup bossGroup)
			{
				CharacterBody currentBody = base.GetCurrentBody();
				if (currentBody && currentBody.healthComponent && currentBody.healthComponent.alive && TeleporterInteraction.instance && this.CheckTeleporter(bossGroup, TeleporterInteraction.instance))
				{
					base.Grant();
				}
			}

			// Token: 0x06002577 RID: 9591 RVA: 0x000AF362 File Offset: 0x000AD562
			private bool CheckTeleporter(BossGroup bossGroup, TeleporterInteraction teleporterInteraction)
			{
				return teleporterInteraction.bossDirector.bossGroup == bossGroup && teleporterInteraction.shrineBonusStacks >= 2;
			}

			// Token: 0x04002867 RID: 10343
			private const int requirement = 2;
		}
	}
}
