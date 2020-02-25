using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006BA RID: 1722
	[RegisterAchievement("LogCollector", "Items.Scanner", null, null)]
	public class LogCollectorAchievement : BaseAchievement
	{
		// Token: 0x0600281B RID: 10267 RVA: 0x000ABABB File Offset: 0x000A9CBB
		public override void OnInstall()
		{
			base.OnInstall();
			this.Check();
			UserProfile.onUnlockableGranted += this.OnUnlockCheck;
		}

		// Token: 0x0600281C RID: 10268 RVA: 0x000ABADA File Offset: 0x000A9CDA
		public override void OnUninstall()
		{
			UserProfile.onUnlockableGranted -= this.OnUnlockCheck;
			base.OnUninstall();
		}

		// Token: 0x0600281D RID: 10269 RVA: 0x000ABAF3 File Offset: 0x000A9CF3
		public override float ProgressForAchievement()
		{
			return (float)this.MonsterLogCount() / 10f;
		}

		// Token: 0x0600281E RID: 10270 RVA: 0x000ABB02 File Offset: 0x000A9D02
		private static bool IsUnlockableMonsterLog(UnlockableDef unlockableDef)
		{
			return unlockableDef.name.StartsWith("Logs.");
		}

		// Token: 0x0600281F RID: 10271 RVA: 0x000ABB14 File Offset: 0x000A9D14
		private int MonsterLogCount()
		{
			StatSheet statSheet = base.userProfile.statSheet;
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

		// Token: 0x06002820 RID: 10272 RVA: 0x000ABB55 File Offset: 0x000A9D55
		private void Check()
		{
			if (this.MonsterLogCount() >= 10)
			{
				base.Grant();
			}
		}

		// Token: 0x06002821 RID: 10273 RVA: 0x000ABB68 File Offset: 0x000A9D68
		private void OnUnlockCheck(UserProfile userProfile, string unlockableToken)
		{
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(unlockableToken);
			if (unlockableDef == null)
			{
				return;
			}
			if (userProfile == base.userProfile && LogCollectorAchievement.IsUnlockableMonsterLog(unlockableDef))
			{
				this.Check();
			}
		}

		// Token: 0x04002508 RID: 9480
		private const int requirement = 10;
	}
}
