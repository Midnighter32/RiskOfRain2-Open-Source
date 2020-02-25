using System;

namespace RoR2.Achievements
{
	// Token: 0x02000694 RID: 1684
	[RegisterAchievement("CompleteMultiBossShrine", "Items.Lightning", null, typeof(CompleteMultiBossShrineAchievement.CompleteMultiBossShrineServerAchievement))]
	public class CompleteMultiBossShrineAchievement : BaseAchievement
	{
		// Token: 0x06002774 RID: 10100 RVA: 0x000AA636 File Offset: 0x000A8836
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x06002775 RID: 10101 RVA: 0x000AA645 File Offset: 0x000A8845
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x02000695 RID: 1685
		private class CompleteMultiBossShrineServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002777 RID: 10103 RVA: 0x000AAAB4 File Offset: 0x000A8CB4
			public override void OnInstall()
			{
				base.OnInstall();
				BossGroup.onBossGroupDefeatedServer += this.OnBossGroupDefeatedServer;
			}

			// Token: 0x06002778 RID: 10104 RVA: 0x000AAACD File Offset: 0x000A8CCD
			public override void OnUninstall()
			{
				BossGroup.onBossGroupDefeatedServer -= this.OnBossGroupDefeatedServer;
				base.OnUninstall();
			}

			// Token: 0x06002779 RID: 10105 RVA: 0x000AAAE8 File Offset: 0x000A8CE8
			private void OnBossGroupDefeatedServer(BossGroup bossGroup)
			{
				CharacterBody currentBody = base.GetCurrentBody();
				if (currentBody && currentBody.healthComponent && currentBody.healthComponent.alive && TeleporterInteraction.instance && this.CheckTeleporter(bossGroup, TeleporterInteraction.instance))
				{
					base.Grant();
				}
			}

			// Token: 0x0600277A RID: 10106 RVA: 0x000AAB3E File Offset: 0x000A8D3E
			private bool CheckTeleporter(BossGroup bossGroup, TeleporterInteraction teleporterInteraction)
			{
				return teleporterInteraction.bossDirector.combatSquad == bossGroup.combatSquad && teleporterInteraction.shrineBonusStacks >= 2;
			}

			// Token: 0x040024F3 RID: 9459
			private const int requirement = 2;
		}
	}
}
