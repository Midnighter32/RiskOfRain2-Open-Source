using System;

namespace RoR2.Achievements
{
	// Token: 0x02000690 RID: 1680
	[RegisterAchievement("CompletePrismaticTrial", "Items.HealOnCrit", null, typeof(CompletePrismaticTrialAchievement.CompletePrismaticTrialServerAchievement))]
	public class CompletePrismaticTrialAchievement : BaseAchievement
	{
		// Token: 0x06002579 RID: 9593 RVA: 0x000AF2C0 File Offset: 0x000AD4C0
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x0600257A RID: 9594 RVA: 0x000AF2CF File Offset: 0x000AD4CF
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x02000691 RID: 1681
		private class CompletePrismaticTrialServerAchievement : BaseServerAchievement
		{
			// Token: 0x0600257C RID: 9596 RVA: 0x000AF38D File Offset: 0x000AD58D
			public override void OnInstall()
			{
				base.OnInstall();
				Run.OnServerGameOver += this.OnServerGameOver;
			}

			// Token: 0x0600257D RID: 9597 RVA: 0x000AF3A6 File Offset: 0x000AD5A6
			public override void OnUninstall()
			{
				base.OnInstall();
				Run.OnServerGameOver -= this.OnServerGameOver;
			}

			// Token: 0x0600257E RID: 9598 RVA: 0x000AF3BF File Offset: 0x000AD5BF
			private void OnServerGameOver(Run run, GameResultType result)
			{
				if (run is WeeklyRun && result == GameResultType.Won)
				{
					base.Grant();
				}
			}
		}
	}
}
