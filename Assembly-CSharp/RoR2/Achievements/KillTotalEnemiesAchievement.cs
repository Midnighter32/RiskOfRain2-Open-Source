using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006B8 RID: 1720
	[RegisterAchievement("KillTotalEnemies", "Items.Infusion", null, null)]
	public class KillTotalEnemiesAchievement : BaseAchievement
	{
		// Token: 0x06002810 RID: 10256 RVA: 0x000AB963 File Offset: 0x000A9B63
		public override void OnInstall()
		{
			base.OnInstall();
			base.userProfile.onStatsReceived += this.Check;
			this.Check();
		}

		// Token: 0x06002811 RID: 10257 RVA: 0x000AB988 File Offset: 0x000A9B88
		public override void OnUninstall()
		{
			base.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x06002812 RID: 10258 RVA: 0x000AB9A7 File Offset: 0x000A9BA7
		public override float ProgressForAchievement()
		{
			return base.userProfile.statSheet.GetStatValueULong(StatDef.totalKills) / 3000f;
		}

		// Token: 0x06002813 RID: 10259 RVA: 0x000AB9C6 File Offset: 0x000A9BC6
		private void Check()
		{
			if (base.userProfile.statSheet.GetStatValueULong(StatDef.totalKills) >= 3000UL)
			{
				base.Grant();
			}
		}

		// Token: 0x04002506 RID: 9478
		private const int requirement = 3000;
	}
}
