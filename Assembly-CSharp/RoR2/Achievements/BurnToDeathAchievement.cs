using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x0200068F RID: 1679
	[RegisterAchievement("BurnToDeath", "Items.Cleanse", null, null)]
	public class BurnToDeathAchievement : BaseAchievement
	{
		// Token: 0x06002758 RID: 10072 RVA: 0x000AA68F File Offset: 0x000A888F
		public override void OnInstall()
		{
			base.OnInstall();
			base.userProfile.onStatsReceived += this.Check;
			this.Check();
		}

		// Token: 0x06002759 RID: 10073 RVA: 0x000AA6B4 File Offset: 0x000A88B4
		public override void OnUninstall()
		{
			base.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x0600275A RID: 10074 RVA: 0x000AA6D3 File Offset: 0x000A88D3
		public override float ProgressForAchievement()
		{
			return base.userProfile.statSheet.GetStatValueULong(BurnToDeathAchievement.trackedStat) / BurnToDeathAchievement.requirement;
		}

		// Token: 0x0600275B RID: 10075 RVA: 0x000AA6F4 File Offset: 0x000A88F4
		private void Check()
		{
			if (base.userProfile.statSheet.GetStatValueULong(BurnToDeathAchievement.trackedStat) >= BurnToDeathAchievement.requirement)
			{
				base.Grant();
			}
		}

		// Token: 0x040024E8 RID: 9448
		private static readonly ulong requirement = 3UL;

		// Token: 0x040024E9 RID: 9449
		private static StatDef trackedStat = StatDef.totalDeathsWhileBurning;
	}
}
