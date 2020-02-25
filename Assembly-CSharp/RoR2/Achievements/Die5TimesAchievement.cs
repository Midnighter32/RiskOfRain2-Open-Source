using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006A0 RID: 1696
	[RegisterAchievement("Die5Times", "Items.Bear", null, null)]
	public class Die5TimesAchievement : BaseAchievement
	{
		// Token: 0x060027A8 RID: 10152 RVA: 0x000AAFBD File Offset: 0x000A91BD
		public override void OnInstall()
		{
			base.OnInstall();
			base.userProfile.onStatsReceived += this.Check;
			this.Check();
		}

		// Token: 0x060027A9 RID: 10153 RVA: 0x000AAFE2 File Offset: 0x000A91E2
		public override void OnUninstall()
		{
			base.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x060027AA RID: 10154 RVA: 0x000AB001 File Offset: 0x000A9201
		public override float ProgressForAchievement()
		{
			return base.userProfile.statSheet.GetStatValueULong(StatDef.totalDeaths) / 5f;
		}

		// Token: 0x060027AB RID: 10155 RVA: 0x000AB020 File Offset: 0x000A9220
		private void Check()
		{
			if (base.userProfile.statSheet.GetStatValueULong(StatDef.totalDeaths) >= 5UL)
			{
				base.Grant();
			}
		}

		// Token: 0x040024F9 RID: 9465
		private const int requirement = 5;
	}
}
