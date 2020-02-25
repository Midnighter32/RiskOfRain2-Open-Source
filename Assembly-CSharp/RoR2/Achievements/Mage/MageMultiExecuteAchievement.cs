using System;

namespace RoR2.Achievements.Mage
{
	// Token: 0x020006E2 RID: 1762
	public class MageMultiExecuteAchievement : BaseAchievement
	{
		// Token: 0x060028F4 RID: 10484 RVA: 0x000AD129 File Offset: 0x000AB329
		protected override int LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("MageBody");
		}

		// Token: 0x060028F5 RID: 10485 RVA: 0x000AC8DE File Offset: 0x000AAADE
		protected override void OnBodyRequirementMet()
		{
			base.OnBodyRequirementMet();
			base.SetServerTracked(true);
		}

		// Token: 0x060028F6 RID: 10486 RVA: 0x000AC8ED File Offset: 0x000AAAED
		protected override void OnBodyRequirementBroken()
		{
			base.SetServerTracked(false);
			base.OnBodyRequirementBroken();
		}

		// Token: 0x0400253A RID: 9530
		private static int requirement = 10;

		// Token: 0x0400253B RID: 9531
		private static float window = 10f;

		// Token: 0x020006E3 RID: 1763
		private class MageMultiExecuteServerAchievement : BaseServerAchievement
		{
			// Token: 0x060028F9 RID: 10489 RVA: 0x000AD3F3 File Offset: 0x000AB5F3
			public override void OnInstall()
			{
				base.OnInstall();
				this.tracker = new DoXInYSecondsTracker(MageMultiExecuteAchievement.requirement, MageMultiExecuteAchievement.window);
				Run.onRunStartGlobal += this.OnRunStart;
				GlobalEventManager.onServerCharacterExecuted += this.OnServerCharacterExecuted;
			}

			// Token: 0x060028FA RID: 10490 RVA: 0x000AD432 File Offset: 0x000AB632
			public override void OnUninstall()
			{
				Run.onRunStartGlobal -= this.OnRunStart;
				GlobalEventManager.onServerCharacterExecuted -= this.OnServerCharacterExecuted;
				base.OnUninstall();
			}

			// Token: 0x060028FB RID: 10491 RVA: 0x000AD45C File Offset: 0x000AB65C
			private void OnRunStart(Run run)
			{
				this.tracker.Clear();
			}

			// Token: 0x060028FC RID: 10492 RVA: 0x000AD469 File Offset: 0x000AB669
			private void OnServerCharacterExecuted(DamageReport damageReport, float executionHealthLost)
			{
				if (damageReport.attackerMaster == base.networkUser.master && base.networkUser.master != null && this.tracker.Push(Run.FixedTimeStamp.now.t))
				{
					base.Grant();
				}
			}

			// Token: 0x0400253C RID: 9532
			private DoXInYSecondsTracker tracker;
		}
	}
}
