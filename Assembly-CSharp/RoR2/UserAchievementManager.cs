using System;
using System.Collections.Generic;
using System.Linq;
using RoR2.Achievements;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020000BA RID: 186
	public class UserAchievementManager
	{
		// Token: 0x060003B0 RID: 944 RVA: 0x0000E37E File Offset: 0x0000C57E
		public void SetServerAchievementTracked(ServerAchievementIndex serverAchievementIndex, bool shouldTrack)
		{
			if (this.serverAchievementTrackingMask[serverAchievementIndex.intValue] == shouldTrack)
			{
				return;
			}
			this.serverAchievementTrackingMask[serverAchievementIndex.intValue] = shouldTrack;
			this.serverAchievementTrackingMaskDirty = true;
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x0000E3A6 File Offset: 0x0000C5A6
		public void TransmitAchievementRequestsToServer()
		{
			if (this.localUser.currentNetworkUser)
			{
				this.localUser.currentNetworkUser.GetComponent<ServerAchievementTracker>().SendAchievementTrackerRequestsMaskToServer(this.serverAchievementTrackingMask);
			}
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x0000E3D8 File Offset: 0x0000C5D8
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

		// Token: 0x060003B3 RID: 947 RVA: 0x0000E498 File Offset: 0x0000C698
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

		// Token: 0x060003B4 RID: 948 RVA: 0x0000E4E0 File Offset: 0x0000C6E0
		public void HandleServerAchievementCompleted(ServerAchievementIndex serverAchievementIndex)
		{
			BaseAchievement baseAchievement = this.achievementsList.FirstOrDefault((BaseAchievement a) => a.achievementDef.serverIndex == serverAchievementIndex);
			if (baseAchievement == null)
			{
				return;
			}
			baseAchievement.Grant();
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x0000E51C File Offset: 0x0000C71C
		public float GetAchievementProgress(AchievementDef achievementDef)
		{
			BaseAchievement baseAchievement = this.achievementsList.FirstOrDefault((BaseAchievement a) => a.achievementDef == achievementDef);
			if (baseAchievement == null)
			{
				return 1f;
			}
			return baseAchievement.ProgressForAchievement();
		}

		// Token: 0x060003B6 RID: 950 RVA: 0x0000E55C File Offset: 0x0000C75C
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

		// Token: 0x060003B7 RID: 951 RVA: 0x0000E658 File Offset: 0x0000C858
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

		// Token: 0x04000333 RID: 819
		private readonly List<BaseAchievement> achievementsList = new List<BaseAchievement>();

		// Token: 0x04000334 RID: 820
		public LocalUser localUser;

		// Token: 0x04000335 RID: 821
		public UserProfile userProfile;

		// Token: 0x04000336 RID: 822
		public int dirtyGrantsCount;

		// Token: 0x04000337 RID: 823
		private readonly bool[] serverAchievementTrackingMask = new bool[AchievementManager.serverAchievementCount];

		// Token: 0x04000338 RID: 824
		private bool serverAchievementTrackingMaskDirty;
	}
}
