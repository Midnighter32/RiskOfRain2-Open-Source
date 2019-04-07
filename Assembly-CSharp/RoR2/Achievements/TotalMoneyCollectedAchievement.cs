using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006BC RID: 1724
	[RegisterAchievement("TotalMoneyCollected", "Items.GoldGat", null, null)]
	public class TotalMoneyCollectedAchievement : BaseAchievement
	{
		// Token: 0x0600263A RID: 9786 RVA: 0x000B074F File Offset: 0x000AE94F
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onStatsReceived += this.Check;
			this.Check();
		}

		// Token: 0x0600263B RID: 9787 RVA: 0x000B0774 File Offset: 0x000AE974
		public override void OnUninstall()
		{
			this.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x0600263C RID: 9788 RVA: 0x000B0793 File Offset: 0x000AE993
		public override float ProgressForAchievement()
		{
			return this.userProfile.statSheet.GetStatValueULong(StatDef.goldCollected) / 30480f;
		}

		// Token: 0x0600263D RID: 9789 RVA: 0x000B07B2 File Offset: 0x000AE9B2
		private void Check()
		{
			if (this.userProfile.statSheet.GetStatValueULong(StatDef.goldCollected) >= 30480UL)
			{
				base.Grant();
			}
		}

		// Token: 0x04002880 RID: 10368
		private const int requirement = 30480;
	}
}
