using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006BB RID: 1723
	[RegisterAchievement("TotalDronesRepaired", "Items.DroneBackup", null, null)]
	public class TotalDronesRepairedAchievement : BaseAchievement
	{
		// Token: 0x06002634 RID: 9780 RVA: 0x000B06D2 File Offset: 0x000AE8D2
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onStatsReceived += this.Check;
			this.Check();
		}

		// Token: 0x06002635 RID: 9781 RVA: 0x000B06F7 File Offset: 0x000AE8F7
		public override void OnUninstall()
		{
			this.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x06002636 RID: 9782 RVA: 0x000B0716 File Offset: 0x000AE916
		public override float ProgressForAchievement()
		{
			return (float)this.TotalDronesPurchased() / 30f;
		}

		// Token: 0x06002637 RID: 9783 RVA: 0x000B0725 File Offset: 0x000AE925
		private int TotalDronesPurchased()
		{
			return (int)this.userProfile.statSheet.GetStatValueULong(StatDef.totalDronesPurchased);
		}

		// Token: 0x06002638 RID: 9784 RVA: 0x000B073D File Offset: 0x000AE93D
		private void Check()
		{
			if (this.TotalDronesPurchased() >= 30)
			{
				base.Grant();
			}
		}

		// Token: 0x0400287F RID: 10367
		private const int requirement = 30;
	}
}
