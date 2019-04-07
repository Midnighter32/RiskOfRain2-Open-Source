using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006B6 RID: 1718
	[RegisterAchievement("RepeatFirstTeleporter", "Characters.Toolbot", null, typeof(RepeatFirstTeleporterAchievement.RepeatFirstTeleporterServerAchievement))]
	public class RepeatFirstTeleporterAchievement : BaseAchievement
	{
		// Token: 0x0600261D RID: 9757 RVA: 0x000B03E0 File Offset: 0x000AE5E0
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onStatsReceived += this.Check;
			base.SetServerTracked(true);
		}

		// Token: 0x0600261E RID: 9758 RVA: 0x000B0406 File Offset: 0x000AE606
		public override void OnUninstall()
		{
			this.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x0600261F RID: 9759 RVA: 0x000B0425 File Offset: 0x000AE625
		private void Check()
		{
			if (this.userProfile.statSheet.GetStatValueULong(StatDef.firstTeleporterCompleted) >= 5UL)
			{
				base.Grant();
			}
		}

		// Token: 0x06002620 RID: 9760 RVA: 0x000B0446 File Offset: 0x000AE646
		public override float ProgressForAchievement()
		{
			return this.userProfile.statSheet.GetStatValueULong(StatDef.firstTeleporterCompleted) / 5f;
		}

		// Token: 0x0400287C RID: 10364
		private const int requirement = 5;

		// Token: 0x020006B7 RID: 1719
		private class RepeatFirstTeleporterServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002622 RID: 9762 RVA: 0x000B0466 File Offset: 0x000AE666
			public override void OnInstall()
			{
				base.OnInstall();
				TeleporterInteraction.onTeleporterChargedGlobal += this.OnTeleporterCharged;
			}

			// Token: 0x06002623 RID: 9763 RVA: 0x000B047F File Offset: 0x000AE67F
			public override void OnUninstall()
			{
				TeleporterInteraction.onTeleporterChargedGlobal -= this.OnTeleporterCharged;
				base.OnUninstall();
			}

			// Token: 0x06002624 RID: 9764 RVA: 0x000B0498 File Offset: 0x000AE698
			private void OnTeleporterCharged(TeleporterInteraction teleporterInteraction)
			{
				SceneCatalog.GetSceneDefForCurrentScene();
				StatSheet currentStats = base.networkUser.masterPlayerStatsComponent.currentStats;
				if (Run.instance && Run.instance.stageClearCount == 0)
				{
					PlayerStatsComponent masterPlayerStatsComponent = base.networkUser.masterPlayerStatsComponent;
					if (masterPlayerStatsComponent)
					{
						masterPlayerStatsComponent.currentStats.PushStatValue(StatDef.firstTeleporterCompleted, 1UL);
					}
				}
			}
		}
	}
}
