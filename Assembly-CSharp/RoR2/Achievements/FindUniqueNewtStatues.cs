using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006A0 RID: 1696
	[RegisterAchievement("FindUniqueNewtStatues", "Items.Talisman", null, null)]
	public class FindUniqueNewtStatues : BaseAchievement
	{
		// Token: 0x060025C3 RID: 9667 RVA: 0x000AFAC0 File Offset: 0x000ADCC0
		public override void OnInstall()
		{
			base.OnInstall();
			this.Check();
			UserProfile.onUnlockableGranted += this.OnUnlockCheck;
		}

		// Token: 0x060025C4 RID: 9668 RVA: 0x000AFADF File Offset: 0x000ADCDF
		public override void OnUninstall()
		{
			UserProfile.onUnlockableGranted -= this.OnUnlockCheck;
			base.OnUninstall();
		}

		// Token: 0x060025C5 RID: 9669 RVA: 0x000AFAF8 File Offset: 0x000ADCF8
		public override float ProgressForAchievement()
		{
			return (float)this.UniqueNewtStatueCount() / 8f;
		}

		// Token: 0x060025C6 RID: 9670 RVA: 0x000AFB07 File Offset: 0x000ADD07
		private static bool IsUnlockableNewtStatue(UnlockableDef unlockableDef)
		{
			return unlockableDef.name.StartsWith("NewtStatue.");
		}

		// Token: 0x060025C7 RID: 9671 RVA: 0x000AFB1C File Offset: 0x000ADD1C
		private int UniqueNewtStatueCount()
		{
			StatSheet statSheet = this.userProfile.statSheet;
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

		// Token: 0x060025C8 RID: 9672 RVA: 0x000AFB5D File Offset: 0x000ADD5D
		private void Check()
		{
			if (this.UniqueNewtStatueCount() >= 8)
			{
				base.Grant();
			}
		}

		// Token: 0x060025C9 RID: 9673 RVA: 0x000AFB70 File Offset: 0x000ADD70
		private void OnUnlockCheck(UserProfile userProfile, string unlockableToken)
		{
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(unlockableToken);
			if (unlockableDef == null)
			{
				return;
			}
			if (userProfile == this.userProfile && FindUniqueNewtStatues.IsUnlockableNewtStatue(unlockableDef))
			{
				this.Check();
			}
		}

		// Token: 0x04002870 RID: 10352
		private const int requirement = 8;
	}
}
