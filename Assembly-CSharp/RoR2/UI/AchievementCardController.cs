using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005A9 RID: 1449
	public class AchievementCardController : MonoBehaviour
	{
		// Token: 0x0600207B RID: 8315 RVA: 0x00099058 File Offset: 0x00097258
		private static string GetAchievementParentIdentifier(string achievementIdentifier)
		{
			AchievementDef achievementDef = AchievementManager.GetAchievementDef(achievementIdentifier);
			if (achievementDef == null)
			{
				return null;
			}
			return achievementDef.prerequisiteAchievementIdentifier;
		}

		// Token: 0x0600207C RID: 8316 RVA: 0x0009906C File Offset: 0x0009726C
		private static int CalcAchievementTabCount(string achievementIdentifier)
		{
			int num = -1;
			while (!string.IsNullOrEmpty(achievementIdentifier))
			{
				num++;
				achievementIdentifier = AchievementCardController.GetAchievementParentIdentifier(achievementIdentifier);
			}
			return num;
		}

		// Token: 0x0600207D RID: 8317 RVA: 0x00099094 File Offset: 0x00097294
		public void SetAchievement(string achievementIdentifier, UserProfile userProfile)
		{
			AchievementDef achievementDef = AchievementManager.GetAchievementDef(achievementIdentifier);
			if (achievementDef != null)
			{
				bool flag = userProfile.HasAchievement(achievementIdentifier);
				bool flag2 = userProfile.CanSeeAchievement(achievementIdentifier);
				if (this.iconImage)
				{
					this.iconImage.sprite = (flag ? achievementDef.GetAchievedIcon() : achievementDef.GetUnachievedIcon());
				}
				if (this.nameLabel)
				{
					this.nameLabel.token = (userProfile.CanSeeAchievement(achievementIdentifier) ? achievementDef.nameToken : "???");
				}
				if (this.descriptionLabel)
				{
					this.descriptionLabel.token = (userProfile.CanSeeAchievement(achievementIdentifier) ? achievementDef.descriptionToken : "???");
				}
				if (this.unlockedImage)
				{
					this.unlockedImage.gameObject.SetActive(flag);
				}
				if (this.cantBeAchievedImage)
				{
					this.cantBeAchievedImage.gameObject.SetActive(!flag2);
				}
				if (this.tooltipProvider)
				{
					string overrideBodyText = "???";
					if (flag2)
					{
						if (flag)
						{
							UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(achievementDef.unlockableRewardIdentifier);
							if (unlockableDef != null)
							{
								string @string = Language.GetString("ACHIEVEMENT_CARD_REWARD_FORMAT");
								string string2 = Language.GetString(unlockableDef.nameToken);
								overrideBodyText = string.Format(@string, string2);
							}
						}
						else
						{
							string string3 = Language.GetString("ACHIEVEMENT_CARD_REWARD_FORMAT");
							string arg = "???";
							overrideBodyText = string.Format(string3, arg);
						}
					}
					else
					{
						AchievementDef achievementDef2 = AchievementManager.GetAchievementDef(achievementDef.prerequisiteAchievementIdentifier);
						if (achievementDef2 != null)
						{
							string string4 = Language.GetString("ACHIEVEMENT_CARD_PREREQ_FORMAT");
							string string5 = Language.GetString(achievementDef2.nameToken);
							overrideBodyText = string.Format(string4, string5);
						}
					}
					this.tooltipProvider.titleToken = (flag2 ? achievementDef.nameToken : "???");
					this.tooltipProvider.overrideBodyText = overrideBodyText;
				}
				if (this.tabLayoutElement)
				{
					this.tabLayoutElement.preferredWidth = (float)AchievementCardController.CalcAchievementTabCount(achievementIdentifier) * this.tabWidth;
				}
			}
		}

		// Token: 0x0400230B RID: 8971
		public Image iconImage;

		// Token: 0x0400230C RID: 8972
		public LanguageTextMeshController nameLabel;

		// Token: 0x0400230D RID: 8973
		public LanguageTextMeshController descriptionLabel;

		// Token: 0x0400230E RID: 8974
		public LayoutElement tabLayoutElement;

		// Token: 0x0400230F RID: 8975
		public float tabWidth;

		// Token: 0x04002310 RID: 8976
		public GameObject unlockedImage;

		// Token: 0x04002311 RID: 8977
		public GameObject cantBeAchievedImage;

		// Token: 0x04002312 RID: 8978
		public TooltipProvider tooltipProvider;
	}
}
