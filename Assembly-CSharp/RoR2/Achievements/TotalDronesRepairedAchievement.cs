using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006CC RID: 1740
	[RegisterAchievement("TotalDronesRepaired", "Items.DroneBackup", null, null)]
	public class TotalDronesRepairedAchievement : BaseAchievement
	{
		// Token: 0x06002869 RID: 10345 RVA: 0x000AC2DD File Offset: 0x000AA4DD
		public override void OnInstall()
		{
			base.OnInstall();
			base.userProfile.onStatsReceived += this.Check;
			this.Check();
		}

		// Token: 0x0600286A RID: 10346 RVA: 0x000AC302 File Offset: 0x000AA502
		public override void OnUninstall()
		{
			base.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x0600286B RID: 10347 RVA: 0x000AC321 File Offset: 0x000AA521
		public override float ProgressForAchievement()
		{
			return (float)this.TotalDronesPurchased() / 30f;
		}

		// Token: 0x0600286C RID: 10348 RVA: 0x000AC330 File Offset: 0x000AA530
		private int TotalDronesPurchased()
		{
			return (int)base.userProfile.statSheet.GetStatValueULong(StatDef.totalDronesPurchased);
		}

		// Token: 0x0600286D RID: 10349 RVA: 0x000AC348 File Offset: 0x000AA548
		private void Check()
		{
			if (this.TotalDronesPurchased() >= 30)
			{
				base.Grant();
			}
		}

		// Token: 0x04002515 RID: 9493
		private const int requirement = 30;
	}
}
