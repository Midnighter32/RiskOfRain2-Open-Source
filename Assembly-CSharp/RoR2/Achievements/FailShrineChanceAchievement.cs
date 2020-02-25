using System;

namespace RoR2.Achievements
{
	// Token: 0x020006A3 RID: 1699
	[RegisterAchievement("FailShrineChance", "Items.Hoof", null, typeof(FailShrineChanceAchievement.FailShrineChanceServerAchievement))]
	public class FailShrineChanceAchievement : BaseAchievement
	{
		// Token: 0x060027BB RID: 10171 RVA: 0x000AA636 File Offset: 0x000A8836
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x060027BC RID: 10172 RVA: 0x000AA645 File Offset: 0x000A8845
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006A4 RID: 1700
		private class FailShrineChanceServerAchievement : BaseServerAchievement
		{
			// Token: 0x060027BE RID: 10174 RVA: 0x000AB1CA File Offset: 0x000A93CA
			public override void OnInstall()
			{
				base.OnInstall();
				ShrineChanceBehavior.onShrineChancePurchaseGlobal += this.OnShrineChancePurchase;
				Run.onRunStartGlobal += this.OnRunStartGlobal;
			}

			// Token: 0x060027BF RID: 10175 RVA: 0x000AB1F4 File Offset: 0x000A93F4
			public override void OnUninstall()
			{
				base.OnInstall();
				ShrineChanceBehavior.onShrineChancePurchaseGlobal -= this.OnShrineChancePurchase;
				Run.onRunStartGlobal -= this.OnRunStartGlobal;
			}

			// Token: 0x060027C0 RID: 10176 RVA: 0x000AB21E File Offset: 0x000A941E
			private void OnRunStartGlobal(Run run)
			{
				this.failedInARow = 0;
			}

			// Token: 0x060027C1 RID: 10177 RVA: 0x000AB228 File Offset: 0x000A9428
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

			// Token: 0x040024FC RID: 9468
			private int failedInARow;

			// Token: 0x040024FD RID: 9469
			private const int requirement = 3;
		}
	}
}
