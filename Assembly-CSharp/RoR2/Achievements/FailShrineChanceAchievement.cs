using System;

namespace RoR2.Achievements
{
	// Token: 0x0200069B RID: 1691
	[RegisterAchievement("FailShrineChance", "Items.Hoof", null, typeof(FailShrineChanceAchievement.FailShrineChanceServerAchievement))]
	public class FailShrineChanceAchievement : BaseAchievement
	{
		// Token: 0x060025B0 RID: 9648 RVA: 0x000AF2C0 File Offset: 0x000AD4C0
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x060025B1 RID: 9649 RVA: 0x000AF2CF File Offset: 0x000AD4CF
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x0200069C RID: 1692
		private class FailShrineChanceServerAchievement : BaseServerAchievement
		{
			// Token: 0x060025B3 RID: 9651 RVA: 0x000AF97D File Offset: 0x000ADB7D
			public override void OnInstall()
			{
				base.OnInstall();
				ShrineChanceBehavior.onShrineChancePurchaseGlobal += this.OnShrineChancePurchase;
				Run.onRunStartGlobal += this.OnRunStartGlobal;
			}

			// Token: 0x060025B4 RID: 9652 RVA: 0x000AF9A7 File Offset: 0x000ADBA7
			public override void OnUninstall()
			{
				base.OnInstall();
				ShrineChanceBehavior.onShrineChancePurchaseGlobal -= this.OnShrineChancePurchase;
				Run.onRunStartGlobal -= this.OnRunStartGlobal;
			}

			// Token: 0x060025B5 RID: 9653 RVA: 0x000AF9D1 File Offset: 0x000ADBD1
			private void OnRunStartGlobal(Run run)
			{
				this.failedInARow = 0;
			}

			// Token: 0x060025B6 RID: 9654 RVA: 0x000AF9DC File Offset: 0x000ADBDC
			private void OnShrineChancePurchase(bool failed, Interactor interactor)
			{
				CharacterBody currentBody = this.serverAchievementTracker.networkUser.GetCurrentBody();
				if (currentBody && currentBody.GetComponent<Interactor>() == interactor)
				{
					if (failed)
					{
						this.failedInARow++;
						if (this.failedInARow >= 3)
						{
							base.Grant();
							return;
						}
					}
					else
					{
						this.failedInARow = 0;
					}
				}
			}

			// Token: 0x0400286E RID: 10350
			private int failedInARow;

			// Token: 0x0400286F RID: 10351
			private const int requirement = 3;
		}
	}
}
