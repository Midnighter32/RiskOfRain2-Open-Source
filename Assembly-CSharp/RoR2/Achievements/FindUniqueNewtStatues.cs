using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006A8 RID: 1704
	[RegisterAchievement("FindUniqueNewtStatues", "Items.Talisman", null, null)]
	public class FindUniqueNewtStatues : BaseAchievement
	{
		// Token: 0x060027CE RID: 10190 RVA: 0x000AB30C File Offset: 0x000A950C
		public override void OnInstall()
		{
			base.OnInstall();
			this.Check();
			UserProfile.onUnlockableGranted += this.OnUnlockCheck;
		}

		// Token: 0x060027CF RID: 10191 RVA: 0x000AB32B File Offset: 0x000A952B
		public override void OnUninstall()
		{
			UserProfile.onUnlockableGranted -= this.OnUnlockCheck;
			base.OnUninstall();
		}

		// Token: 0x060027D0 RID: 10192 RVA: 0x000AB344 File Offset: 0x000A9544
		public override float ProgressForAchievement()
		{
			return (float)this.UniqueNewtStatueCount() / 8f;
		}

		// Token: 0x060027D1 RID: 10193 RVA: 0x000AB353 File Offset: 0x000A9553
		private static bool IsUnlockableNewtStatue(UnlockableDef unlockableDef)
		{
			return unlockableDef.name.StartsWith("NewtStatue.");
		}

		// Token: 0x060027D2 RID: 10194 RVA: 0x000AB368 File Offset: 0x000A9568
		private int UniqueNewtStatueCount()
		{
			StatSheet statSheet = base.userProfile.statSheet;
			int num = 0;
			int i = 0;
			int unlockableCount = statSheet.GetUnlockableCount();
			while (i < unlockableCount)
			{
				if (FindUniqueNewtStatues.IsUnlockableNewtStatue(statSheet.GetUnlockable(i)))
				{
					num++;
				}
				i++;
			}
			return num;
		}

		// Token: 0x060027D3 RID: 10195 RVA: 0x000AB3A9 File Offset: 0x000A95A9
		private void Check()
		{
			if (this.UniqueNewtStatueCount() >= 8)
			{
				base.Grant();
			}
		}

		// Token: 0x060027D4 RID: 10196 RVA: 0x000AB3BC File Offset: 0x000A95BC
		private void OnUnlockCheck(UserProfile userProfile, string unlockableToken)
		{
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(unlockableToken);
			if (unlockableDef == null)
			{
				return;
			}
			if (userProfile == base.userProfile && FindUniqueNewtStatues.IsUnlockableNewtStatue(unlockableDef))
			{
				this.Check();
			}
		}

		// Token: 0x040024FE RID: 9470
		private const int requirement = 8;
	}
}
