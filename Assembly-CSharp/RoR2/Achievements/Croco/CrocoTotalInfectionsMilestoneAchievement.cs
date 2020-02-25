using System;
using RoR2.Stats;

namespace RoR2.Achievements.Croco
{
	// Token: 0x020006EF RID: 1775
	[RegisterAchievement("CrocoTotalInfectionsMilestone", "Skills.Croco.ChainableLeap", "BeatArena", null)]
	public class CrocoTotalInfectionsMilestoneAchievement : BaseAchievement
	{
		// Token: 0x0600293F RID: 10559 RVA: 0x000ADA4B File Offset: 0x000ABC4B
		public override void OnInstall()
		{
			base.OnInstall();
			base.userProfile.onStatsReceived += this.Check;
		}

		// Token: 0x06002940 RID: 10560 RVA: 0x000ADA6A File Offset: 0x000ABC6A
		public override void OnUninstall()
		{
			base.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x06002941 RID: 10561 RVA: 0x000ADA89 File Offset: 0x000ABC89
		private void Check()
		{
			if (base.userProfile.statSheet.GetStatValueULong(CrocoTotalInfectionsMilestoneAchievement.statDef) >= CrocoTotalInfectionsMilestoneAchievement.requirement)
			{
				base.Grant();
			}
		}

		// Token: 0x06002942 RID: 10562 RVA: 0x000ADAAD File Offset: 0x000ABCAD
		public override float ProgressForAchievement()
		{
			return base.userProfile.statSheet.GetStatValueULong(CrocoTotalInfectionsMilestoneAchievement.statDef) / CrocoTotalInfectionsMilestoneAchievement.requirement;
		}

		// Token: 0x0400254C RID: 9548
		public static readonly ulong requirement = 1000UL;

		// Token: 0x0400254D RID: 9549
		private static StatDef statDef = StatDef.totalCrocoInfectionsInflicted;
	}
}
