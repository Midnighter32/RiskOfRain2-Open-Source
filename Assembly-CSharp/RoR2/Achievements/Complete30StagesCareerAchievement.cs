using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x0200068D RID: 1677
	[RegisterAchievement("Complete30StagesCareer", "Characters.Engineer", null, null)]
	public class Complete30StagesCareerAchievement : BaseAchievement
	{
		// Token: 0x0600256C RID: 9580 RVA: 0x000AF23B File Offset: 0x000AD43B
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onStatsReceived += this.Check;
			this.Check();
		}

		// Token: 0x0600256D RID: 9581 RVA: 0x000AF260 File Offset: 0x000AD460
		public override void OnUninstall()
		{
			this.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x0600256E RID: 9582 RVA: 0x000AF27F File Offset: 0x000AD47F
		public override float ProgressForAchievement()
		{
			return this.userProfile.statSheet.GetStatValueULong(StatDef.totalStagesCompleted) / 30f;
		}

		// Token: 0x0600256F RID: 9583 RVA: 0x000AF29E File Offset: 0x000AD49E
		private void Check()
		{
			if (this.userProfile.statSheet.GetStatValueULong(StatDef.totalStagesCompleted) >= 30UL)
			{
				base.Grant();
			}
		}

		// Token: 0x04002866 RID: 10342
		private const int requirement = 30;
	}
}
