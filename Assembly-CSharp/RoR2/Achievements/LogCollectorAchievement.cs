using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006AD RID: 1709
	[RegisterAchievement("LogCollector", "Items.Scanner", null, null)]
	public class LogCollectorAchievement : BaseAchievement
	{
		// Token: 0x060025F7 RID: 9719 RVA: 0x000AFFCC File Offset: 0x000AE1CC
		public override void OnInstall()
		{
			base.OnInstall();
			this.Check();
			UserProfile.onUnlockableGranted += this.OnUnlockCheck;
		}

		// Token: 0x060025F8 RID: 9720 RVA: 0x000AFFEB File Offset: 0x000AE1EB
		public override void OnUninstall()
		{
			UserProfile.onUnlockableGranted -= this.OnUnlockCheck;
			base.OnUninstall();
		}

		// Token: 0x060025F9 RID: 9721 RVA: 0x000B0004 File Offset: 0x000AE204
		public override float ProgressForAchievement()
		{
			return (float)this.MonsterLogCount() / 10f;
		}

		// Token: 0x060025FA RID: 9722 RVA: 0x000B0013 File Offset: 0x000AE213
		private static bool IsUnlockableMonsterLog(UnlockableDef unlockableDef)
		{
			return unlockableDef.name.StartsWith("Logs.");
		}

		// Token: 0x060025FB RID: 9723 RVA: 0x000B0028 File Offset: 0x000AE228
		private int MonsterLogCount()
		{
			StatSheet statSheet = this.userProfile.statSheet;
			int num = 0;
			int i = 0;
			int unlockableCount = statSheet.GetUnlockableCount();
			while (i < unlockableCount)
			{
				if (LogCollectorAchievement.IsUnlockableMonsterLog(statSheet.GetUnlockable(i)))
				{
					num++;
				}
				i++;
			}
			return num;
		}

		// Token: 0x060025FC RID: 9724 RVA: 0x000B0069 File Offset: 0x000AE269
		private void Check()
		{
			if (this.MonsterLogCount() >= 10)
			{
				base.Grant();
			}
		}

		// Token: 0x060025FD RID: 9725 RVA: 0x000B007C File Offset: 0x000AE27C
		private void OnUnlockCheck(UserProfile userProfile, string unlockableToken)
		{
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(unlockableToken);
			if (unlockableDef == null)
			{
				return;
			}
			if (userProfile == this.userProfile && LogCollectorAchievement.IsUnlockableMonsterLog(unlockableDef))
			{
				this.Check();
			}
		}

		// Token: 0x04002875 RID: 10357
		private const int requirement = 10;
	}
}
