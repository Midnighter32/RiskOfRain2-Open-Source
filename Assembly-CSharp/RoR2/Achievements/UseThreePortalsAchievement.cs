using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006CE RID: 1742
	[RegisterAchievement("UseThreePortals", "Items.Tonic", null, null)]
	public class UseThreePortalsAchievement : BaseAchievement
	{
		// Token: 0x06002874 RID: 10356 RVA: 0x000AC3E4 File Offset: 0x000AA5E4
		public override void OnInstall()
		{
			base.OnInstall();
			this.statsToCheck = new StatDef[]
			{
				PerStageStatDef.totalTimesVisited.FindStatDef("bazaar"),
				PerStageStatDef.totalTimesVisited.FindStatDef("mysteryspace"),
				PerStageStatDef.totalTimesVisited.FindStatDef("goldshores")
			};
			base.userProfile.onStatsReceived += this.Check;
			this.Check();
		}

		// Token: 0x06002875 RID: 10357 RVA: 0x000AC456 File Offset: 0x000AA656
		public override void OnUninstall()
		{
			base.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x06002876 RID: 10358 RVA: 0x000AC475 File Offset: 0x000AA675
		public override float ProgressForAchievement()
		{
			return (float)this.GetUniquePortalsUsedCount() / 3f;
		}

		// Token: 0x06002877 RID: 10359 RVA: 0x000AC484 File Offset: 0x000AA684
		private int GetUniquePortalsUsedCount()
		{
			StatSheet statSheet = base.userProfile.statSheet;
			int num = 0;
			foreach (StatDef statDef in this.statsToCheck)
			{
				if (statSheet.GetStatValueULong(statDef) > 0UL)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06002878 RID: 10360 RVA: 0x000AC4CA File Offset: 0x000AA6CA
		private void Check()
		{
			if (this.GetUniquePortalsUsedCount() >= 3)
			{
				base.Grant();
			}
		}

		// Token: 0x04002517 RID: 9495
		private StatDef[] statsToCheck;

		// Token: 0x04002518 RID: 9496
		private const int requirement = 3;
	}
}
