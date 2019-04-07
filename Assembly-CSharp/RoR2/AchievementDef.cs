using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001F7 RID: 503
	public class AchievementDef
	{
		// Token: 0x060009E5 RID: 2533 RVA: 0x000315DC File Offset: 0x0002F7DC
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

		// Token: 0x060009E6 RID: 2534 RVA: 0x0003162A File Offset: 0x0002F82A
		public Sprite GetUnachievedIcon()
		{
			return Resources.Load<Sprite>("Textures/MiscIcons/texUnlockIcon");
		}

		// Token: 0x060009E7 RID: 2535 RVA: 0x00031636 File Offset: 0x0002F836
		public string GetAchievementSoundString()
		{
			if (this.unlockableRewardIdentifier.Contains("Characters"))
			{
				return "Play_UI_achievementUnlock_enhanced";
			}
			return "Play_UI_achievementUnlock";
		}

		// Token: 0x04000D12 RID: 3346
		public AchievementIndex index;

		// Token: 0x04000D13 RID: 3347
		public ServerAchievementIndex serverIndex = new ServerAchievementIndex
		{
			intValue = -1
		};

		// Token: 0x04000D14 RID: 3348
		public string identifier;

		// Token: 0x04000D15 RID: 3349
		public string unlockableRewardIdentifier;

		// Token: 0x04000D16 RID: 3350
		public string prerequisiteAchievementIdentifier;

		// Token: 0x04000D17 RID: 3351
		public string nameToken;

		// Token: 0x04000D18 RID: 3352
		public string descriptionToken;

		// Token: 0x04000D19 RID: 3353
		public string iconPath;

		// Token: 0x04000D1A RID: 3354
		public Type type;

		// Token: 0x04000D1B RID: 3355
		public Type serverTrackerType;

		// Token: 0x04000D1C RID: 3356
		private static readonly string[] emptyStringArray = Array.Empty<string>();

		// Token: 0x04000D1D RID: 3357
		public string[] childAchievementIdentifiers = AchievementDef.emptyStringArray;

		// Token: 0x04000D1E RID: 3358
		private Sprite achievedIcon;

		// Token: 0x04000D1F RID: 3359
		private Sprite unachievedIcon;
	}
}
