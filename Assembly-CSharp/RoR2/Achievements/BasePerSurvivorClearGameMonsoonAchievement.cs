using System;

namespace RoR2.Achievements
{
	// Token: 0x0200068C RID: 1676
	public class BasePerSurvivorClearGameMonsoonAchievement : BaseAchievement
	{
		// Token: 0x0600274D RID: 10061 RVA: 0x000AA5E6 File Offset: 0x000A87E6
		protected override void OnBodyRequirementMet()
		{
			base.OnBodyRequirementMet();
			Run.onClientGameOverGlobal += this.OnClientGameOverGlobal;
		}

		// Token: 0x0600274E RID: 10062 RVA: 0x000AA5FF File Offset: 0x000A87FF
		protected override void OnBodyRequirementBroken()
		{
			Run.onClientGameOverGlobal -= this.OnClientGameOverGlobal;
			base.OnBodyRequirementBroken();
		}

		// Token: 0x0600274F RID: 10063 RVA: 0x000AA618 File Offset: 0x000A8818
		private void OnClientGameOverGlobal(Run run, RunReport runReport)
		{
			if (runReport.gameResultType != GameResultType.Lost && DifficultyIndex.Hard <= runReport.ruleBook.FindDifficulty())
			{
				base.Grant();
			}
		}
	}
}
