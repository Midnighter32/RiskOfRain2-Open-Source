using System;

namespace RoR2.Achievements.Merc
{
	// Token: 0x020006DA RID: 1754
	[RegisterAchievement("MercCompleteTrialWithFullHealth", "Skills.Merc.EvisProjectile", "CompleteUnknownEnding", typeof(MercCompleteTrialWithFullHealthAchievement.MercCompleteTrialWithFullHealthServerAchievement))]
	public class MercCompleteTrialWithFullHealthAchievement : BaseAchievement
	{
		// Token: 0x060028BB RID: 10427 RVA: 0x000ACDE8 File Offset: 0x000AAFE8
		protected override int LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("MercBody");
		}

		// Token: 0x060028BC RID: 10428 RVA: 0x000AC8DE File Offset: 0x000AAADE
		protected override void OnBodyRequirementMet()
		{
			base.OnBodyRequirementMet();
			base.SetServerTracked(true);
		}

		// Token: 0x060028BD RID: 10429 RVA: 0x000AC8ED File Offset: 0x000AAAED
		protected override void OnBodyRequirementBroken()
		{
			base.SetServerTracked(false);
			base.OnBodyRequirementBroken();
		}

		// Token: 0x020006DB RID: 1755
		private class MercCompleteTrialWithFullHealthServerAchievement : BaseServerAchievement
		{
			// Token: 0x060028BF RID: 10431 RVA: 0x000ACDF4 File Offset: 0x000AAFF4
			public override void OnInstall()
			{
				base.OnInstall();
				this.listenForDamage = new ToggleAction(delegate()
				{
					RoR2Application.onFixedUpdate += this.OnFixedUpdate;
				}, delegate()
				{
					RoR2Application.onFixedUpdate -= this.OnFixedUpdate;
				});
				this.listenForGameOver = new ToggleAction(delegate()
				{
					Run.OnServerGameOver += this.OnServerGameOver;
				}, delegate()
				{
					Run.OnServerGameOver -= this.OnServerGameOver;
				});
				Run.onRunStartGlobal += this.OnRunStart;
				Run.onRunDestroyGlobal += this.OnRunDestroy;
				if (Run.instance)
				{
					this.OnRunDiscovered(Run.instance);
				}
			}

			// Token: 0x060028C0 RID: 10432 RVA: 0x000ACE88 File Offset: 0x000AB088
			public override void OnUninstall()
			{
				Run.onRunDestroyGlobal -= this.OnRunDestroy;
				Run.onRunStartGlobal -= this.OnRunStart;
				this.listenForGameOver.SetActive(false);
				this.listenForDamage.SetActive(false);
				base.OnUninstall();
			}

			// Token: 0x060028C1 RID: 10433 RVA: 0x000ACED8 File Offset: 0x000AB0D8
			private bool CharacterIsAtFullHealthOrNull()
			{
				CharacterBody currentBody = base.GetCurrentBody();
				return !currentBody || currentBody.healthComponent.fullCombinedHealth <= currentBody.healthComponent.combinedHealth;
			}

			// Token: 0x060028C2 RID: 10434 RVA: 0x000ACF11 File Offset: 0x000AB111
			private void OnFixedUpdate()
			{
				if (!this.CharacterIsAtFullHealthOrNull())
				{
					this.Fail();
				}
			}

			// Token: 0x060028C3 RID: 10435 RVA: 0x000ACF21 File Offset: 0x000AB121
			private void Fail()
			{
				this.failed = true;
				this.listenForDamage.SetActive(false);
				this.listenForGameOver.SetActive(false);
			}

			// Token: 0x060028C4 RID: 10436 RVA: 0x000ACF42 File Offset: 0x000AB142
			private void OnServerGameOver(Run run, GameResultType gameResultType)
			{
				if (gameResultType == GameResultType.Won)
				{
					if (this.runOk && !this.failed)
					{
						base.Grant();
					}
					this.runOk = false;
				}
			}

			// Token: 0x060028C5 RID: 10437 RVA: 0x000ACF65 File Offset: 0x000AB165
			private void OnRunStart(Run run)
			{
				this.OnRunDiscovered(run);
			}

			// Token: 0x060028C6 RID: 10438 RVA: 0x000ACF6E File Offset: 0x000AB16E
			private void OnRunDiscovered(Run run)
			{
				this.runOk = (run is WeeklyRun);
				if (this.runOk)
				{
					this.listenForGameOver.SetActive(true);
					this.listenForDamage.SetActive(true);
					this.failed = false;
				}
			}

			// Token: 0x060028C7 RID: 10439 RVA: 0x000ACFA6 File Offset: 0x000AB1A6
			private void OnRunDestroy(Run run)
			{
				this.OnRunLost(run);
			}

			// Token: 0x060028C8 RID: 10440 RVA: 0x000ACFAF File Offset: 0x000AB1AF
			private void OnRunLost(Run run)
			{
				this.listenForGameOver.SetActive(false);
				this.listenForDamage.SetActive(false);
			}

			// Token: 0x0400252C RID: 9516
			private ToggleAction listenForDamage;

			// Token: 0x0400252D RID: 9517
			private ToggleAction listenForGameOver;

			// Token: 0x0400252E RID: 9518
			private bool failed;

			// Token: 0x0400252F RID: 9519
			private bool runOk;
		}
	}
}
