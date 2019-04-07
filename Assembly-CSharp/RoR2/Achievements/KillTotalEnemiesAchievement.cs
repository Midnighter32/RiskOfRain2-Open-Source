using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006AC RID: 1708
	[RegisterAchievement("KillTotalEnemies", "Items.Infusion", null, null)]
	public class KillTotalEnemiesAchievement : BaseAchievement
	{
		// Token: 0x060025F2 RID: 9714 RVA: 0x000AFF44 File Offset: 0x000AE144
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onStatsReceived += this.Check;
			this.Check();
		}

		// Token: 0x060025F3 RID: 9715 RVA: 0x000AFF69 File Offset: 0x000AE169
		public override void OnUninstall()
		{
			this.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x060025F4 RID: 9716 RVA: 0x000AFF88 File Offset: 0x000AE188
		public override float ProgressForAchievement()
		{
			return this.userProfile.statSheet.GetStatValueULong(StatDef.totalKills) / 3000f;
		}

		// Token: 0x060025F5 RID: 9717 RVA: 0x000AFFA7 File Offset: 0x000AE1A7
		private void Check()
		{
			if (this.userProfile.statSheet.GetStatValueULong(StatDef.totalKills) >= 3000UL)
			{
				base.Grant();
			}
		}

		// Token: 0x04002874 RID: 10356
		private const int requirement = 3000;
	}
}
