using System;

namespace RoR2.Achievements.Mage
{
	// Token: 0x020006E0 RID: 1760
	[RegisterAchievement("MageFastBoss", "Skills.Mage.IceBomb", "FreeMage", typeof(MageFastBossAchievement.MageFastBossServerAchievement))]
	public class MageFastBossAchievement : BaseAchievement
	{
		// Token: 0x060028E2 RID: 10466 RVA: 0x000AD129 File Offset: 0x000AB329
		protected override int LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("MageBody");
		}

		// Token: 0x060028E3 RID: 10467 RVA: 0x000AC8DE File Offset: 0x000AAADE
		protected override void OnBodyRequirementMet()
		{
			base.OnBodyRequirementMet();
			base.SetServerTracked(true);
		}

		// Token: 0x060028E4 RID: 10468 RVA: 0x000AC8ED File Offset: 0x000AAAED
		protected override void OnBodyRequirementBroken()
		{
			base.SetServerTracked(false);
			base.OnBodyRequirementBroken();
		}

		// Token: 0x04002536 RID: 9526
		private static readonly float window = 1f;

		// Token: 0x020006E1 RID: 1761
		private class MageFastBossServerAchievement : BaseServerAchievement
		{
			// Token: 0x060028E7 RID: 10471 RVA: 0x000AD241 File Offset: 0x000AB441
			private void OnBossDamageFirstTaken()
			{
				this.expirationTimeStamp = Run.FixedTimeStamp.now + MageFastBossAchievement.window;
				this.listenForBossDamage.SetActive(false);
				this.listenForBossDefeated.SetActive(true);
			}

			// Token: 0x060028E8 RID: 10472 RVA: 0x000AD270 File Offset: 0x000AB470
			public override void OnInstall()
			{
				base.OnInstall();
				this.listenForBossDamage = new ToggleAction(delegate()
				{
					GlobalEventManager.onServerDamageDealt += this.OnServerDamageDealt;
				}, delegate()
				{
					GlobalEventManager.onServerDamageDealt -= this.OnServerDamageDealt;
				});
				this.listenForBossDefeated = new ToggleAction(delegate()
				{
					BossGroup.onBossGroupDefeatedServer += this.OnBossGroupDefeatedServer;
				}, delegate()
				{
					BossGroup.onBossGroupDefeatedServer -= this.OnBossGroupDefeatedServer;
				});
				BossGroup.onBossGroupStartServer += this.OnBossGroupStartServer;
				Run.onRunStartGlobal += this.OnRunStart;
				this.Reset();
			}

			// Token: 0x060028E9 RID: 10473 RVA: 0x000AD2F1 File Offset: 0x000AB4F1
			public override void OnUninstall()
			{
				BossGroup.onBossGroupStartServer -= this.OnBossGroupStartServer;
				this.listenForBossDefeated.SetActive(false);
				this.listenForBossDamage.SetActive(false);
				base.OnUninstall();
			}

			// Token: 0x060028EA RID: 10474 RVA: 0x000AD322 File Offset: 0x000AB522
			private void OnRunStart(Run run)
			{
				this.Reset();
			}

			// Token: 0x060028EB RID: 10475 RVA: 0x000AD32A File Offset: 0x000AB52A
			private void Reset()
			{
				this.expirationTimeStamp = Run.FixedTimeStamp.negativeInfinity;
				this.listenForBossDefeated.SetActive(false);
				this.listenForBossDamage.SetActive(false);
			}

			// Token: 0x060028EC RID: 10476 RVA: 0x000AD34F File Offset: 0x000AB54F
			private void OnBossGroupStartServer(BossGroup bossGroup)
			{
				this.listenForBossDamage.SetActive(true);
			}

			// Token: 0x060028ED RID: 10477 RVA: 0x000AD35D File Offset: 0x000AB55D
			private void OnServerDamageDealt(DamageReport damageReport)
			{
				if (damageReport.victimMaster && damageReport.victimMaster.isBoss)
				{
					this.OnBossDamageFirstTaken();
				}
			}

			// Token: 0x060028EE RID: 10478 RVA: 0x000AD37F File Offset: 0x000AB57F
			private void OnBossGroupDefeatedServer(BossGroup bossGroup)
			{
				if (!this.expirationTimeStamp.hasPassed)
				{
					base.Grant();
				}
			}

			// Token: 0x04002537 RID: 9527
			private ToggleAction listenForBossDamage;

			// Token: 0x04002538 RID: 9528
			private ToggleAction listenForBossDefeated;

			// Token: 0x04002539 RID: 9529
			private Run.FixedTimeStamp expirationTimeStamp;
		}
	}
}
