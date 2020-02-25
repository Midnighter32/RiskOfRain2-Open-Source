using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006CD RID: 1741
	[RegisterAchievement("TotalMoneyCollected", "Items.GoldGat", null, null)]
	public class TotalMoneyCollectedAchievement : BaseAchievement
	{
		// Token: 0x0600286F RID: 10351 RVA: 0x000AC35A File Offset: 0x000AA55A
		public override void OnInstall()
		{
			base.OnInstall();
			base.userProfile.onStatsReceived += this.Check;
			this.Check();
		}

		// Token: 0x06002870 RID: 10352 RVA: 0x000AC37F File Offset: 0x000AA57F
		public override void OnUninstall()
		{
			base.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x06002871 RID: 10353 RVA: 0x000AC39E File Offset: 0x000AA59E
		public override float ProgressForAchievement()
		{
			return base.userProfile.statSheet.GetStatValueULong(StatDef.goldCollected) / 30480f;
		}

		// Token: 0x06002872 RID: 10354 RVA: 0x000AC3BD File Offset: 0x000AA5BD
		private void Check()
		{
			if (base.userProfile.statSheet.GetStatValueULong(StatDef.goldCollected) >= 30480UL)
			{
				base.Grant();
			}
		}

		// Token: 0x04002516 RID: 9494
		private const int requirement = 30480;
	}
}
