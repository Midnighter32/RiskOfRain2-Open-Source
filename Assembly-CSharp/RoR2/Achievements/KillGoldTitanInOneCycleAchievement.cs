using System;
using EntityStates.Missions.Goldshores;

namespace RoR2.Achievements
{
	// Token: 0x020006B6 RID: 1718
	[RegisterAchievement("KillGoldTitanInOneCycle", "Items.Gateway", null, typeof(KillGoldTitanInOneCycleAchievement.KillGoldTitanInOnePhaseServerAchievement))]
	public class KillGoldTitanInOneCycleAchievement : BaseAchievement
	{
		// Token: 0x06002809 RID: 10249 RVA: 0x000AA636 File Offset: 0x000A8836
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x0600280A RID: 10250 RVA: 0x000AB8E4 File Offset: 0x000A9AE4
		public override void OnUninstall()
		{
			base.SetServerTracked(false);
			base.OnUninstall();
		}

		// Token: 0x020006B7 RID: 1719
		public class KillGoldTitanInOnePhaseServerAchievement : BaseServerAchievement
		{
			// Token: 0x0600280C RID: 10252 RVA: 0x000AB8F3 File Offset: 0x000A9AF3
			public override void OnInstall()
			{
				base.OnInstall();
				this.goldTitanBodyIndex = BodyCatalog.FindBodyIndex("TitanGoldBody");
				GoldshoresBossfight.onOneCycleGoldTitanKill += this.OnOneCycleGoldTitanKill;
			}

			// Token: 0x0600280D RID: 10253 RVA: 0x000AB91C File Offset: 0x000A9B1C
			private void OnOneCycleGoldTitanKill()
			{
				if (this.serverAchievementTracker.networkUser.GetCurrentBody())
				{
					base.Grant();
				}
			}

			// Token: 0x0600280E RID: 10254 RVA: 0x000AB93B File Offset: 0x000A9B3B
			public override void OnUninstall()
			{
				GoldshoresBossfight.onOneCycleGoldTitanKill -= this.OnOneCycleGoldTitanKill;
				base.OnUninstall();
			}

			// Token: 0x04002505 RID: 9477
			private int goldTitanBodyIndex = -1;
		}
	}
}
