using System;

namespace RoR2.Achievements
{
	// Token: 0x02000689 RID: 1673
	public class BaseServerAchievement
	{
		// Token: 0x1700032E RID: 814
		// (get) Token: 0x06002554 RID: 9556 RVA: 0x000AEEE7 File Offset: 0x000AD0E7
		public NetworkUser networkUser
		{
			get
			{
				return this.serverAchievementTracker.networkUser;
			}
		}

		// Token: 0x06002555 RID: 9557 RVA: 0x000AEEF4 File Offset: 0x000AD0F4
		protected CharacterBody GetCurrentBody()
		{
			return this.networkUser.GetCurrentBody();
		}

		// Token: 0x06002556 RID: 9558 RVA: 0x00004507 File Offset: 0x00002707
		public virtual void OnInstall()
		{
		}

		// Token: 0x06002557 RID: 9559 RVA: 0x00004507 File Offset: 0x00002707
		public virtual void OnUninstall()
		{
		}

		// Token: 0x06002558 RID: 9560 RVA: 0x000AEF01 File Offset: 0x000AD101
		protected void Grant()
		{
			this.serverAchievementTracker.CallRpcGrantAchievement(this.achievementDef.serverIndex);
		}

		// Token: 0x06002559 RID: 9561 RVA: 0x000AEF1C File Offset: 0x000AD11C
		public static BaseServerAchievement Instantiate(ServerAchievementIndex serverAchievementIndex)
		{
			AchievementDef achievementDef = AchievementManager.GetAchievementDef(serverAchievementIndex);
			if (achievementDef == null || achievementDef.serverTrackerType == null)
			{
				return null;
			}
			BaseServerAchievement baseServerAchievement = (BaseServerAchievement)Activator.CreateInstance(achievementDef.serverTrackerType);
			baseServerAchievement.achievementDef = achievementDef;
			return baseServerAchievement;
		}

		// Token: 0x0400285C RID: 10332
		public ServerAchievementTracker serverAchievementTracker;

		// Token: 0x0400285D RID: 10333
		public AchievementDef achievementDef;
	}
}
