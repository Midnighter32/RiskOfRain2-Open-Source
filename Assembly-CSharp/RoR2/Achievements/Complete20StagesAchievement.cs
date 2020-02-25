using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x02000692 RID: 1682
	[RegisterAchievement("Complete20Stages", "Items.Clover", null, null)]
	public class Complete20StagesAchievement : BaseAchievement
	{
		// Token: 0x06002769 RID: 10089 RVA: 0x000AA933 File Offset: 0x000A8B33
		public override void OnInstall()
		{
			base.OnInstall();
			Run.onRunStartGlobal += this.OnRunStart;
		}

		// Token: 0x0600276A RID: 10090 RVA: 0x000AA94C File Offset: 0x000A8B4C
		public override void OnUninstall()
		{
			Run.onRunStartGlobal -= this.OnRunStart;
			this.SetListeningForStats(false);
			base.OnUninstall();
		}

		// Token: 0x0600276B RID: 10091 RVA: 0x000AA96C File Offset: 0x000A8B6C
		private void OnRunStart(Run run)
		{
			this.SetListeningForStats(true);
		}

		// Token: 0x0600276C RID: 10092 RVA: 0x000AA978 File Offset: 0x000A8B78
		private void SetListeningForStats(bool shouldListen)
		{
			if (this.listeningForStats == shouldListen)
			{
				return;
			}
			this.listeningForStats = shouldListen;
			if (this.listeningForStats)
			{
				this.subscribedProfile = base.localUser.userProfile;
				this.subscribedProfile.onStatsReceived += this.OnStatsReceived;
				return;
			}
			this.subscribedProfile.onStatsReceived -= this.OnStatsReceived;
			this.subscribedProfile = null;
		}

		// Token: 0x0600276D RID: 10093 RVA: 0x000AA9E8 File Offset: 0x000A8BE8
		private void OnStatsReceived()
		{
			PlayerStatsComponent playerStatsComponent = this.playerStatsComponentGetter.Get(base.localUser.cachedMasterObject);
			if (playerStatsComponent && playerStatsComponent.currentStats.GetStatValueULong(StatDef.highestStagesCompleted) >= 20UL)
			{
				base.Grant();
			}
		}

		// Token: 0x040024EE RID: 9454
		private const int requirement = 20;

		// Token: 0x040024EF RID: 9455
		private bool listeningForStats;

		// Token: 0x040024F0 RID: 9456
		private UserProfile subscribedProfile;

		// Token: 0x040024F1 RID: 9457
		private MemoizedGetComponent<PlayerStatsComponent> playerStatsComponentGetter;
	}
}
