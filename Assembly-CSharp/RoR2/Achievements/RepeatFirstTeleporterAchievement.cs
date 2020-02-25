using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006C5 RID: 1733
	[RegisterAchievement("RepeatFirstTeleporter", "Characters.Toolbot", null, typeof(RepeatFirstTeleporterAchievement.RepeatFirstTeleporterServerAchievement))]
	public class RepeatFirstTeleporterAchievement : BaseAchievement
	{
		// Token: 0x0600284B RID: 10315 RVA: 0x000ABF64 File Offset: 0x000AA164
		public override void OnInstall()
		{
			base.OnInstall();
			base.userProfile.onStatsReceived += this.Check;
			base.SetServerTracked(true);
		}

		// Token: 0x0600284C RID: 10316 RVA: 0x000ABF8A File Offset: 0x000AA18A
		public override void OnUninstall()
		{
			base.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x0600284D RID: 10317 RVA: 0x000ABFA9 File Offset: 0x000AA1A9
		private void Check()
		{
			if (base.userProfile.statSheet.GetStatValueULong(StatDef.firstTeleporterCompleted) >= 5UL)
			{
				base.Grant();
			}
		}

		// Token: 0x0600284E RID: 10318 RVA: 0x000ABFCA File Offset: 0x000AA1CA
		public override float ProgressForAchievement()
		{
			return base.userProfile.statSheet.GetStatValueULong(StatDef.firstTeleporterCompleted) / 5f;
		}

		// Token: 0x04002511 RID: 9489
		private const int requirement = 5;

		// Token: 0x020006C6 RID: 1734
		private class RepeatFirstTeleporterServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002850 RID: 10320 RVA: 0x000ABFEA File Offset: 0x000AA1EA
			public override void OnInstall()
			{
				base.OnInstall();
				TeleporterInteraction.onTeleporterChargedGlobal += this.OnTeleporterCharged;
			}

			// Token: 0x06002851 RID: 10321 RVA: 0x000AC003 File Offset: 0x000AA203
			public override void OnUninstall()
			{
				TeleporterInteraction.onTeleporterChargedGlobal -= this.OnTeleporterCharged;
				base.OnUninstall();
			}

			// Token: 0x06002852 RID: 10322 RVA: 0x000AC01C File Offset: 0x000AA21C
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
