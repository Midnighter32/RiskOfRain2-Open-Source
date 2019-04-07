using System;

namespace RoR2.Achievements
{
	// Token: 0x02000688 RID: 1672
	public abstract class BaseAchievement
	{
		// Token: 0x0600254D RID: 9549 RVA: 0x000AEDDC File Offset: 0x000ACFDC
		public virtual void OnInstall()
		{
			this.localUser = this.owner.localUser;
			this.userProfile = this.owner.userProfile;
		}

		// Token: 0x0600254E RID: 9550 RVA: 0x000AEE00 File Offset: 0x000AD000
		public virtual float ProgressForAchievement()
		{
			return 0f;
		}

		// Token: 0x0600254F RID: 9551 RVA: 0x000AEE07 File Offset: 0x000AD007
		public virtual void OnUninstall()
		{
			if (this.achievementDef.serverTrackerType != null)
			{
				this.SetServerTracked(false);
			}
			this.owner = null;
			this.localUser = null;
			this.userProfile = null;
		}

		// Token: 0x06002550 RID: 9552 RVA: 0x000AEE38 File Offset: 0x000AD038
		public void Grant()
		{
			if (this.shouldGrant)
			{
				return;
			}
			this.shouldGrant = true;
			this.owner.dirtyGrantsCount++;
		}

		// Token: 0x06002551 RID: 9553 RVA: 0x000AEE60 File Offset: 0x000AD060
		public virtual void OnGranted()
		{
			if (!string.IsNullOrEmpty(this.achievementDef.unlockableRewardIdentifier))
			{
				if (this.localUser.currentNetworkUser)
				{
					UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(this.achievementDef.unlockableRewardIdentifier);
					this.localUser.currentNetworkUser.CallCmdReportUnlock(unlockableDef.index);
				}
				this.userProfile.AddUnlockToken(this.achievementDef.unlockableRewardIdentifier);
			}
		}

		// Token: 0x06002552 RID: 9554 RVA: 0x000AEECE File Offset: 0x000AD0CE
		public void SetServerTracked(bool shouldTrack)
		{
			this.owner.SetServerAchievementTracked(this.achievementDef.serverIndex, shouldTrack);
		}

		// Token: 0x04002857 RID: 10327
		public UserAchievementManager owner;

		// Token: 0x04002858 RID: 10328
		protected LocalUser localUser;

		// Token: 0x04002859 RID: 10329
		protected UserProfile userProfile;

		// Token: 0x0400285A RID: 10330
		public bool shouldGrant;

		// Token: 0x0400285B RID: 10331
		public AchievementDef achievementDef;
	}
}
