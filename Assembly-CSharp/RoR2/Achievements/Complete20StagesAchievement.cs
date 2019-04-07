using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x0200068C RID: 1676
	[RegisterAchievement("Complete20Stages", "Items.Clover", null, null)]
	public class Complete20StagesAchievement : BaseAchievement
	{
		// Token: 0x06002566 RID: 9574 RVA: 0x000AF13F File Offset: 0x000AD33F
		public override void OnInstall()
		{
			base.OnInstall();
			Run.onRunStartGlobal += this.OnRunStart;
		}

		// Token: 0x06002567 RID: 9575 RVA: 0x000AF158 File Offset: 0x000AD358
		public override void OnUninstall()
		{
			Run.onRunStartGlobal -= this.OnRunStart;
			this.SetListeningForStats(false);
			base.OnUninstall();
		}

		// Token: 0x06002568 RID: 9576 RVA: 0x000AF178 File Offset: 0x000AD378
		private void OnRunStart(Run run)
		{
			this.SetListeningForStats(true);
		}

		// Token: 0x06002569 RID: 9577 RVA: 0x000AF184 File Offset: 0x000AD384
		private void SetListeningForStats(bool shouldListen)
		{
			if (this.listeningForStats == shouldListen)
			{
				return;
			}
			this.listeningForStats = shouldListen;
			if (this.listeningForStats)
			{
				this.subscribedProfile = this.localUser.userProfile;
				this.subscribedProfile.onStatsReceived += this.OnStatsReceived;
				return;
			}
			this.subscribedProfile.onStatsReceived -= this.OnStatsReceived;
			this.subscribedProfile = null;
		}

		// Token: 0x0600256A RID: 9578 RVA: 0x000AF1F4 File Offset: 0x000AD3F4
		private void OnStatsReceived()
		{
			PlayerStatsComponent playerStatsComponent = this.playerStatsComponentGetter.Get(this.localUser.cachedMasterObject);
			if (playerStatsComponent && playerStatsComponent.currentStats.GetStatValueULong(StatDef.highestStagesCompleted) >= 20UL)
			{
				base.Grant();
			}
		}

		// Token: 0x04002862 RID: 10338
		private const int requirement = 20;

		// Token: 0x04002863 RID: 10339
		private bool listeningForStats;

		// Token: 0x04002864 RID: 10340
		private UserProfile subscribedProfile;

		// Token: 0x04002865 RID: 10341
		private MemoizedGetComponent<PlayerStatsComponent> playerStatsComponentGetter;
	}
}
