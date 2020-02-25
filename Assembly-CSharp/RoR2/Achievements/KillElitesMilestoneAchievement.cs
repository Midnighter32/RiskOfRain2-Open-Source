using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006B5 RID: 1717
	[RegisterAchievement("KillElitesMilestone", "Items.ExecuteLowHealthElite", null, null)]
	public class KillElitesMilestoneAchievement : BaseAchievement
	{
		// Token: 0x06002804 RID: 10244 RVA: 0x000AB85C File Offset: 0x000A9A5C
		public override void OnInstall()
		{
			base.OnInstall();
			base.userProfile.onStatsReceived += this.Check;
			this.Check();
		}

		// Token: 0x06002805 RID: 10245 RVA: 0x000AB881 File Offset: 0x000A9A81
		public override void OnUninstall()
		{
			base.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x06002806 RID: 10246 RVA: 0x000AB8A0 File Offset: 0x000A9AA0
		public override float ProgressForAchievement()
		{
			return base.userProfile.statSheet.GetStatValueULong(StatDef.totalEliteKills) / 500f;
		}

		// Token: 0x06002807 RID: 10247 RVA: 0x000AB8BF File Offset: 0x000A9ABF
		private void Check()
		{
			if (base.userProfile.statSheet.GetStatValueULong(StatDef.totalEliteKills) >= 500UL)
			{
				base.Grant();
			}
		}

		// Token: 0x04002504 RID: 9476
		private const int requirement = 500;
	}
}
