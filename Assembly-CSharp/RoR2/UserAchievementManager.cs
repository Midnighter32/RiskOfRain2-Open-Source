using System;
using System.Collections.Generic;
using System.Linq;
using RoR2.Achievements;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001F9 RID: 505
	public class UserAchievementManager
	{
		// Token: 0x060009EB RID: 2539 RVA: 0x000316BE File Offset: 0x0002F8BE
		public void SetServerAchievementTracked(ServerAchievementIndex serverAchievementIndex, bool shouldTrack)
		{
			if (this.serverAchievementTrackingMask[serverAchievementIndex.intValue] == shouldTrack)
			{
				return;
			}
			this.serverAchievementTrackingMask[serverAchievementIndex.intValue] = shouldTrack;
			this.serverAchievementTrackingMaskDirty = true;
		}

		// Token: 0x060009EC RID: 2540 RVA: 0x000316E6 File Offset: 0x0002F8E6
		public void TransmitAchievementRequestsToServer()
		{
			if (this.localUser.currentNetworkUser)
			{
				this.localUser.currentNetworkUser.GetComponent<ServerAchievementTracker>().SendAchievementTrackerRequestsMaskToServer(this.serverAchievementTrackingMask);
			}
		}

		// Token: 0x060009ED RID: 2541 RVA: 0x00031718 File Offset: 0x0002F918
		public void Update()
		{
			if (this.serverAchievementTrackingMaskDirty)
			{
				this.serverAchievementTrackingMaskDirty = false;
				this.TransmitAchievementRequestsToServer();
			}
			int num = this.achievementsList.Count - 1;
			while (num >= 0 && this.dirtyGrantsCount > 0)
			{
				BaseAchievement baseAchievement = this.achievementsList[num];
				if (baseAchievement.shouldGrant)
				{
					this.dirtyGrantsCount--;
					this.achievementsList.RemoveAt(num);
					this.userProfile.AddAchievement(baseAchievement.achievementDef.identifier, true);
					baseAchievement.OnGranted();
					baseAchievement.OnUninstall();
					NetworkUser currentNetworkUser = this.localUser.currentNetworkUser;
					if (currentNetworkUser != null)
					{
						currentNetworkUser.CallCmdReportAchievement(baseAchievement.achievementDef.nameToken);
					}
				}
				num--;
			}
		}

		// Token: 0x060009EE RID: 2542 RVA: 0x000317D8 File Offset: 0x0002F9D8
		public void GrantAchievement(AchievementDef achievementDef)
		{
			for (int i = 0; i < this.achievementsList.Count; i++)
			{
				if (this.achievementsList[i].achievementDef == achievementDef)
				{
					this.achievementsList[i].Grant();
				}
			}
		}

		// Token: 0x060009EF RID: 2543 RVA: 0x00031820 File Offset: 0x0002FA20
		public void HandleServerAchievementCompleted(ServerAchievementIndex serverAchievementIndex)
		{
			BaseAchievement baseAchievement = this.achievementsList.FirstOrDefault((BaseAchievement a) => a.achievementDef.serverIndex == serverAchievementIndex);
			if (baseAchievement == null)
			{
				return;
			}
			baseAchievement.Grant();
		}

		// Token: 0x060009F0 RID: 2544 RVA: 0x0003185C File Offset: 0x0002FA5C
		public float GetAchievementProgress(AchievementDef achievementDef)
		{
			BaseAchievement baseAchievement = this.achievementsList.FirstOrDefault((BaseAchievement a) => a.achievementDef == achievementDef);
			if (baseAchievement == null)
			{
				return 1f;
			}
			return baseAchievement.ProgressForAchievement();
		}

		// Token: 0x060009F1 RID: 2545 RVA: 0x0003189C File Offset: 0x0002FA9C
		public void OnInstall(LocalUser localUser)
		{
			this.localUser = localUser;
			this.userProfile = localUser.userProfile;
			foreach (string text in AchievementManager.readOnlyAchievementIdentifiers)
			{
				AchievementDef achievementDef = AchievementManager.GetAchievementDef(text);
				if (this.userProfile.HasAchievement(text))
				{
					if (!this.userProfile.HasUnlockable(achievementDef.unlockableRewardIdentifier))
					{
						Debug.LogFormat("UserProfile {0} has achievement {1} but not its unlockable {2}. Granting.", new object[]
						{
							this.userProfile.name,
							achievementDef.nameToken,
							achievementDef.unlockableRewardIdentifier
						});
						this.userProfile.AddUnlockToken(achievementDef.unlockableRewardIdentifier);
					}
				}
				else
				{
					BaseAchievement baseAchievement = (BaseAchievement)Activator.CreateInstance(achievementDef.type);
					baseAchievement.achievementDef = achievementDef;
					baseAchievement.owner = this;
					this.achievementsList.Add(baseAchievement);
					baseAchievement.OnInstall();
				}
			}
		}

		// Token: 0x060009F2 RID: 2546 RVA: 0x00031998 File Offset: 0x0002FB98
		public void OnUninstall()
		{
			for (int i = this.achievementsList.Count - 1; i >= 0; i--)
			{
				this.achievementsList[i].OnUninstall();
			}
			this.achievementsList.Clear();
			this.localUser = null;
			this.userProfile = null;
		}

		// Token: 0x04000D24 RID: 3364
		private readonly List<BaseAchievement> achievementsList = new List<BaseAchievement>();

		// Token: 0x04000D25 RID: 3365
		public LocalUser localUser;

		// Token: 0x04000D26 RID: 3366
		public UserProfile userProfile;

		// Token: 0x04000D27 RID: 3367
		public int dirtyGrantsCount;

		// Token: 0x04000D28 RID: 3368
		private readonly bool[] serverAchievementTrackingMask = new bool[AchievementManager.serverAchievementCount];

		// Token: 0x04000D29 RID: 3369
		private bool serverAchievementTrackingMaskDirty;
	}
}
