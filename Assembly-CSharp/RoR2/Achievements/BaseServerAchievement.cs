using System;
using UnityEngine;

namespace RoR2.Achievements
{
	// Token: 0x0200068B RID: 1675
	public class BaseServerAchievement
	{
		// Token: 0x170003F7 RID: 1015
		// (get) Token: 0x06002744 RID: 10052 RVA: 0x000AA52A File Offset: 0x000A872A
		public NetworkUser networkUser
		{
			get
			{
				return this.serverAchievementTracker.networkUser;
			}
		}

		// Token: 0x06002745 RID: 10053 RVA: 0x000AA537 File Offset: 0x000A8737
		protected CharacterBody GetCurrentBody()
		{
			return this.networkUser.GetCurrentBody();
		}

		// Token: 0x06002746 RID: 10054 RVA: 0x000AA544 File Offset: 0x000A8744
		protected bool IsCurrentBody(GameObject gameObject)
		{
			CharacterBody currentBody = this.GetCurrentBody();
			return currentBody && currentBody.gameObject == gameObject;
		}

		// Token: 0x06002747 RID: 10055 RVA: 0x000AA56C File Offset: 0x000A876C
		protected bool IsCurrentBody(CharacterBody characterBody)
		{
			CharacterBody currentBody = this.GetCurrentBody();
			return currentBody && currentBody == characterBody;
		}

		// Token: 0x06002748 RID: 10056 RVA: 0x0000409B File Offset: 0x0000229B
		public virtual void OnInstall()
		{
		}

		// Token: 0x06002749 RID: 10057 RVA: 0x0000409B File Offset: 0x0000229B
		public virtual void OnUninstall()
		{
		}

		// Token: 0x0600274A RID: 10058 RVA: 0x000AA58E File Offset: 0x000A878E
		protected void Grant()
		{
			this.serverAchievementTracker.CallRpcGrantAchievement(this.achievementDef.serverIndex);
		}

		// Token: 0x0600274B RID: 10059 RVA: 0x000AA5A8 File Offset: 0x000A87A8
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

		// Token: 0x040024E6 RID: 9446
		public ServerAchievementTracker serverAchievementTracker;

		// Token: 0x040024E7 RID: 9447
		public AchievementDef achievementDef;
	}
}
