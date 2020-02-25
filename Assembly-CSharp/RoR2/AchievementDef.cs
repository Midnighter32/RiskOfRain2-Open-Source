using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020000B8 RID: 184
	public class AchievementDef
	{
		// Token: 0x060003AA RID: 938 RVA: 0x0000E268 File Offset: 0x0000C468
		public Sprite GetAchievedIcon()
		{
			if (!this.achievedIcon)
			{
				this.achievedIcon = Resources.Load<Sprite>(this.iconPath);
				if (!this.achievedIcon)
				{
					this.achievedIcon = Resources.Load<Sprite>("Textures/AchievementIcons/texPlaceholderAchievement");
				}
			}
			return this.achievedIcon;
		}

		// Token: 0x060003AB RID: 939 RVA: 0x0000E2B6 File Offset: 0x0000C4B6
		public Sprite GetUnachievedIcon()
		{
			return Resources.Load<Sprite>("Textures/MiscIcons/texUnlockIcon");
		}

		// Token: 0x060003AC RID: 940 RVA: 0x0000E2C4 File Offset: 0x0000C4C4
		public string GetAchievementSoundString()
		{
			if (this.unlockableRewardIdentifier.Contains("Characters."))
			{
				return "Play_UI_achievementUnlock_enhanced";
			}
			if (this.unlockableRewardIdentifier.Contains("Skills.") || this.unlockableRewardIdentifier.Contains("Skins."))
			{
				return "Play_UI_skill_unlock";
			}
			return "Play_UI_achievementUnlock";
		}

		// Token: 0x04000321 RID: 801
		public AchievementIndex index;

		// Token: 0x04000322 RID: 802
		public ServerAchievementIndex serverIndex = new ServerAchievementIndex
		{
			intValue = -1
		};

		// Token: 0x04000323 RID: 803
		public string identifier;

		// Token: 0x04000324 RID: 804
		public string unlockableRewardIdentifier;

		// Token: 0x04000325 RID: 805
		public string prerequisiteAchievementIdentifier;

		// Token: 0x04000326 RID: 806
		public string nameToken;

		// Token: 0x04000327 RID: 807
		public string descriptionToken;

		// Token: 0x04000328 RID: 808
		public string iconPath;

		// Token: 0x04000329 RID: 809
		public Type type;

		// Token: 0x0400032A RID: 810
		public Type serverTrackerType;

		// Token: 0x0400032B RID: 811
		private static readonly string[] emptyStringArray = Array.Empty<string>();

		// Token: 0x0400032C RID: 812
		public string[] childAchievementIdentifiers = AchievementDef.emptyStringArray;

		// Token: 0x0400032D RID: 813
		private Sprite achievedIcon;

		// Token: 0x0400032E RID: 814
		private Sprite unachievedIcon;
	}
}
