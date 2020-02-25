using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x02000693 RID: 1683
	[RegisterAchievement("Complete30StagesCareer", "Characters.Engineer", null, null)]
	public class Complete30StagesCareerAchievement : BaseAchievement
	{
		// Token: 0x0600276F RID: 10095 RVA: 0x000AAA2F File Offset: 0x000A8C2F
		public override void OnInstall()
		{
			base.OnInstall();
			base.userProfile.onStatsReceived += this.Check;
			this.Check();
		}

		// Token: 0x06002770 RID: 10096 RVA: 0x000AAA54 File Offset: 0x000A8C54
		public override void OnUninstall()
		{
			base.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x06002771 RID: 10097 RVA: 0x000AAA73 File Offset: 0x000A8C73
		public override float ProgressForAchievement()
		{
			return base.userProfile.statSheet.GetStatValueULong(StatDef.totalStagesCompleted) / 30f;
		}

		// Token: 0x06002772 RID: 10098 RVA: 0x000AAA92 File Offset: 0x000A8C92
		private void Check()
		{
			if (base.userProfile.statSheet.GetStatValueULong(StatDef.totalStagesCompleted) >= 30UL)
			{
				base.Grant();
			}
		}

		// Token: 0x040024F2 RID: 9458
		private const int requirement = 30;
	}
}
