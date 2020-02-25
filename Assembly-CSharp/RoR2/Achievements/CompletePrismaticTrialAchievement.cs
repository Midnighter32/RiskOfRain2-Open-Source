using System;

namespace RoR2.Achievements
{
	// Token: 0x02000696 RID: 1686
	[RegisterAchievement("CompletePrismaticTrial", "Items.HealOnCrit", null, typeof(CompletePrismaticTrialAchievement.CompletePrismaticTrialServerAchievement))]
	public class CompletePrismaticTrialAchievement : BaseAchievement
	{
		// Token: 0x0600277C RID: 10108 RVA: 0x000AA636 File Offset: 0x000A8836
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x0600277D RID: 10109 RVA: 0x000AA645 File Offset: 0x000A8845
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x02000697 RID: 1687
		private class CompletePrismaticTrialServerAchievement : BaseServerAchievement
		{
			// Token: 0x0600277F RID: 10111 RVA: 0x000AAB66 File Offset: 0x000A8D66
			public override void OnInstall()
			{
				base.OnInstall();
				Run.OnServerGameOver += this.OnServerGameOver;
			}

			// Token: 0x06002780 RID: 10112 RVA: 0x000AAB7F File Offset: 0x000A8D7F
			public override void OnUninstall()
			{
				base.OnInstall();
				Run.OnServerGameOver -= this.OnServerGameOver;
			}

			// Token: 0x06002781 RID: 10113 RVA: 0x000AAB98 File Offset: 0x000A8D98
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
