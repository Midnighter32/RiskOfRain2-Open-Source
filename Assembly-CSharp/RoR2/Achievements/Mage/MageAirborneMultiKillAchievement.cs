using System;

namespace RoR2.Achievements.Mage
{
	// Token: 0x020006DD RID: 1757
	[RegisterAchievement("MageAirborneMultiKill", "Skills.Mage.FlyUp", "FreeMage", typeof(MageAirborneMultiKillAchievement.MageAirborneMultiKillServerAchievement))]
	public class MageAirborneMultiKillAchievement : BaseAchievement
	{
		// Token: 0x060028D5 RID: 10453 RVA: 0x000AD129 File Offset: 0x000AB329
		protected override int LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("MageBody");
		}

		// Token: 0x060028D6 RID: 10454 RVA: 0x000AC8DE File Offset: 0x000AAADE
		protected override void OnBodyRequirementMet()
		{
			base.OnBodyRequirementMet();
			base.SetServerTracked(true);
		}

		// Token: 0x060028D7 RID: 10455 RVA: 0x000AC8ED File Offset: 0x000AAAED
		protected override void OnBodyRequirementBroken()
		{
			base.SetServerTracked(false);
			base.OnBodyRequirementBroken();
		}

		// Token: 0x04002534 RID: 9524
		private static readonly int requirement = 15;

		// Token: 0x020006DE RID: 1758
		private class MageAirborneMultiKillServerAchievement : BaseServerAchievement
		{
			// Token: 0x060028DA RID: 10458 RVA: 0x000AD13E File Offset: 0x000AB33E
			public override void OnInstall()
			{
				base.OnInstall();
				RoR2Application.onFixedUpdate += this.OnFixedUpdate;
				GlobalEventManager.onCharacterDeathGlobal += this.OnCharacterDeath;
			}

			// Token: 0x060028DB RID: 10459 RVA: 0x000AD168 File Offset: 0x000AB368
			public override void OnUninstall()
			{
				GlobalEventManager.onCharacterDeathGlobal -= this.OnCharacterDeath;
				RoR2Application.onFixedUpdate -= this.OnFixedUpdate;
				base.OnUninstall();
			}

			// Token: 0x060028DC RID: 10460 RVA: 0x000AD194 File Offset: 0x000AB394
			private bool CharacterIsInAir()
			{
				CharacterBody currentBody = base.networkUser.GetCurrentBody();
				return currentBody && currentBody.characterMotor && !currentBody.characterMotor.isGrounded;
			}

			// Token: 0x060028DD RID: 10461 RVA: 0x000AD1D2 File Offset: 0x000AB3D2
			private void OnFixedUpdate()
			{
				if (!this.CharacterIsInAir())
				{
					this.killCount = 0;
				}
			}

			// Token: 0x060028DE RID: 10462 RVA: 0x000AD1E4 File Offset: 0x000AB3E4
			private void OnCharacterDeath(DamageReport damageReport)
			{
				if (damageReport.attackerMaster == base.networkUser.master && damageReport.attackerMaster != null && this.CharacterIsInAir())
				{
					this.killCount++;
					if (MageAirborneMultiKillAchievement.requirement <= this.killCount)
					{
						base.Grant();
					}
				}
			}

			// Token: 0x04002535 RID: 9525
			private int killCount;
		}
	}
}
