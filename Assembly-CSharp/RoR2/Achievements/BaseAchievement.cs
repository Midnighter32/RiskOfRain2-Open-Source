using System;

namespace RoR2.Achievements
{
	// Token: 0x0200068A RID: 1674
	public abstract class BaseAchievement
	{
		// Token: 0x170003F2 RID: 1010
		// (get) Token: 0x06002730 RID: 10032 RVA: 0x000AA2D4 File Offset: 0x000A84D4
		// (set) Token: 0x06002731 RID: 10033 RVA: 0x000AA2DC File Offset: 0x000A84DC
		private protected LocalUser localUser { protected get; private set; }

		// Token: 0x170003F3 RID: 1011
		// (get) Token: 0x06002732 RID: 10034 RVA: 0x000AA2E5 File Offset: 0x000A84E5
		// (set) Token: 0x06002733 RID: 10035 RVA: 0x000AA2ED File Offset: 0x000A84ED
		private protected UserProfile userProfile { protected get; private set; }

		// Token: 0x06002734 RID: 10036 RVA: 0x000AA2F8 File Offset: 0x000A84F8
		public virtual void OnInstall()
		{
			this.localUser = this.owner.localUser;
			this.userProfile = this.owner.userProfile;
			this.requiredBodyIndex = this.LookUpRequiredBodyIndex();
			if (this.requiredBodyIndex != -1)
			{
				this.meetsBodyRequirement = false;
				this.localUser.onBodyChanged += this.HandleBodyChangedForBodyRequirement;
			}
		}

		// Token: 0x06002735 RID: 10037 RVA: 0x000AA35A File Offset: 0x000A855A
		public virtual float ProgressForAchievement()
		{
			return 0f;
		}

		// Token: 0x06002736 RID: 10038 RVA: 0x000AA364 File Offset: 0x000A8564
		public virtual void OnUninstall()
		{
			if (this.achievementDef.serverTrackerType != null)
			{
				this.SetServerTracked(false);
			}
			if (this.requiredBodyIndex != -1)
			{
				this.localUser.onBodyChanged -= this.HandleBodyChangedForBodyRequirement;
				if (this.meetsBodyRequirement)
				{
					this.meetsBodyRequirement = false;
					this.OnBodyRequirementBroken();
				}
			}
			this.owner = null;
			this.localUser = null;
			this.userProfile = null;
		}

		// Token: 0x06002737 RID: 10039 RVA: 0x000AA3D5 File Offset: 0x000A85D5
		public void Grant()
		{
			if (this.shouldGrant)
			{
				return;
			}
			this.shouldGrant = true;
			this.owner.dirtyGrantsCount++;
		}

		// Token: 0x06002738 RID: 10040 RVA: 0x000AA3FC File Offset: 0x000A85FC
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

		// Token: 0x06002739 RID: 10041 RVA: 0x000AA46A File Offset: 0x000A866A
		public void SetServerTracked(bool shouldTrack)
		{
			this.owner.SetServerAchievementTracked(this.achievementDef.serverIndex, shouldTrack);
		}

		// Token: 0x170003F4 RID: 1012
		// (get) Token: 0x0600273A RID: 10042 RVA: 0x000AA483 File Offset: 0x000A8683
		// (set) Token: 0x0600273B RID: 10043 RVA: 0x000AA48B File Offset: 0x000A868B
		private protected int requiredBodyIndex { protected get; private set; } = -1;

		// Token: 0x170003F5 RID: 1013
		// (get) Token: 0x0600273C RID: 10044 RVA: 0x000AA494 File Offset: 0x000A8694
		// (set) Token: 0x0600273D RID: 10045 RVA: 0x000AA49C File Offset: 0x000A869C
		private protected bool meetsBodyRequirement { protected get; private set; } = true;

		// Token: 0x170003F6 RID: 1014
		// (get) Token: 0x0600273E RID: 10046 RVA: 0x000AA4A5 File Offset: 0x000A86A5
		protected virtual bool wantsBodyCallbacks { get; }

		// Token: 0x0600273F RID: 10047 RVA: 0x000AA4AD File Offset: 0x000A86AD
		protected virtual int LookUpRequiredBodyIndex()
		{
			return -1;
		}

		// Token: 0x06002740 RID: 10048 RVA: 0x000AA4B0 File Offset: 0x000A86B0
		private void HandleBodyChangedForBodyRequirement()
		{
			bool flag = this.localUser.cachedBody;
			bool flag2 = this.meetsBodyRequirement;
			if (flag)
			{
				flag2 = (this.localUser.cachedBody.bodyIndex == this.requiredBodyIndex);
			}
			if (this.meetsBodyRequirement != flag2)
			{
				this.meetsBodyRequirement = flag2;
				if (this.meetsBodyRequirement)
				{
					this.OnBodyRequirementMet();
					return;
				}
				this.OnBodyRequirementBroken();
			}
		}

		// Token: 0x06002741 RID: 10049 RVA: 0x0000409B File Offset: 0x0000229B
		protected virtual void OnBodyRequirementMet()
		{
		}

		// Token: 0x06002742 RID: 10050 RVA: 0x0000409B File Offset: 0x0000229B
		protected virtual void OnBodyRequirementBroken()
		{
		}

		// Token: 0x040024DE RID: 9438
		public UserAchievementManager owner;

		// Token: 0x040024E1 RID: 9441
		public bool shouldGrant;

		// Token: 0x040024E2 RID: 9442
		public AchievementDef achievementDef;
	}
}
