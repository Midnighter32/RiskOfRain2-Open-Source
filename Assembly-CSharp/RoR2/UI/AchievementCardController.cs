using System;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x0200057C RID: 1404
	public class AchievementCardController : MonoBehaviour
	{
		// Token: 0x0600216B RID: 8555 RVA: 0x00090C10 File Offset: 0x0008EE10
		private static string GetAchievementParentIdentifier(string achievementIdentifier)
		{
			AchievementDef achievementDef = AchievementManager.GetAchievementDef(achievementIdentifier);
			if (achievementDef == null)
			{
				return null;
			}
			return achievementDef.prerequisiteAchievementIdentifier;
		}

		// Token: 0x0600216C RID: 8556 RVA: 0x00090C24 File Offset: 0x0008EE24
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

		// Token: 0x0600216D RID: 8557 RVA: 0x00090C4C File Offset: 0x0008EE4C
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

		// Token: 0x04001EDE RID: 7902
		public Image iconImage;

		// Token: 0x04001EDF RID: 7903
		public LanguageTextMeshController nameLabel;

		// Token: 0x04001EE0 RID: 7904
		public LanguageTextMeshController descriptionLabel;

		// Token: 0x04001EE1 RID: 7905
		public LayoutElement tabLayoutElement;

		// Token: 0x04001EE2 RID: 7906
		public float tabWidth;

		// Token: 0x04001EE3 RID: 7907
		public GameObject unlockedImage;

		// Token: 0x04001EE4 RID: 7908
		public GameObject cantBeAchievedImage;

		// Token: 0x04001EE5 RID: 7909
		public TooltipProvider tooltipProvider;
	}
}
