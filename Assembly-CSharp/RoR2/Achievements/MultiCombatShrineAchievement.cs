using System;

namespace RoR2.Achievements
{
	// Token: 0x020006C1 RID: 1729
	[RegisterAchievement("MultiCombatShrine", "Items.EnergizedOnEquipmentUse", null, typeof(MultiCombatShrineAchievement.MultiCombatShrineServerAchievement))]
	public class MultiCombatShrineAchievement : BaseAchievement
	{
		// Token: 0x06002839 RID: 10297 RVA: 0x000AA636 File Offset: 0x000A8836
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x0600283A RID: 10298 RVA: 0x000AB8E4 File Offset: 0x000A9AE4
		public override void OnUninstall()
		{
			base.SetServerTracked(false);
			base.OnUninstall();
		}

		// Token: 0x020006C2 RID: 1730
		private class MultiCombatShrineServerAchievement : BaseServerAchievement
		{
			// Token: 0x0600283C RID: 10300 RVA: 0x000ABDC9 File Offset: 0x000A9FC9
			public override void OnInstall()
			{
				base.OnInstall();
				ShrineCombatBehavior.onDefeatedServerGlobal += this.OnShrineDefeated;
				Stage.onServerStageBegin += this.OnServerStageBegin;
			}

			// Token: 0x0600283D RID: 10301 RVA: 0x000ABDF3 File Offset: 0x000A9FF3
			public override void OnUninstall()
			{
				Stage.onServerStageBegin -= this.OnServerStageBegin;
				ShrineCombatBehavior.onDefeatedServerGlobal -= this.OnShrineDefeated;
				base.OnUninstall();
			}

			// Token: 0x0600283E RID: 10302 RVA: 0x000ABE1D File Offset: 0x000AA01D
			private void OnServerStageBegin(Stage stage)
			{
				this.counter = 0;
			}

			// Token: 0x0600283F RID: 10303 RVA: 0x000ABE26 File Offset: 0x000AA026
			private void OnShrineDefeated(ShrineCombatBehavior instance)
			{
				this.counter++;
				this.Check();
			}

			// Token: 0x06002840 RID: 10304 RVA: 0x000ABE3C File Offset: 0x000AA03C
			private void Check()
			{
				if (this.counter >= MultiCombatShrineAchievement.MultiCombatShrineServerAchievement.requirement)
				{
					base.Grant();
				}
			}

			// Token: 0x0400250C RID: 9484
			private int counter;

			// Token: 0x0400250D RID: 9485
			private static readonly int requirement = 3;
		}
	}
}
