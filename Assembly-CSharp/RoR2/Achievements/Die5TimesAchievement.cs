using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x02000698 RID: 1688
	[RegisterAchievement("Die5Times", "Items.Bear", null, null)]
	public class Die5TimesAchievement : BaseAchievement
	{
		// Token: 0x0600259D RID: 9629 RVA: 0x000AF77B File Offset: 0x000AD97B
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onStatsReceived += this.Check;
			this.Check();
		}

		// Token: 0x0600259E RID: 9630 RVA: 0x000AF7A0 File Offset: 0x000AD9A0
		public override void OnUninstall()
		{
			this.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x0600259F RID: 9631 RVA: 0x000AF7BF File Offset: 0x000AD9BF
		public override float ProgressForAchievement()
		{
			return this.userProfile.statSheet.GetStatValueULong(StatDef.totalDeaths) / 5f;
		}

		// Token: 0x060025A0 RID: 9632 RVA: 0x000AF7DE File Offset: 0x000AD9DE
		private void Check()
		{
			if (this.userProfile.statSheet.GetStatValueULong(StatDef.totalDeaths) >= 5UL)
			{
				base.Grant();
			}
		}

		// Token: 0x0400286B RID: 10347
		private const int requirement = 5;
	}
}
