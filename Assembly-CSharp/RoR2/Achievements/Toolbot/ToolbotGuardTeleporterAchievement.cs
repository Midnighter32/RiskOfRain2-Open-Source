using System;

namespace RoR2.Achievements.Toolbot
{
	// Token: 0x020006D5 RID: 1749
	[RegisterAchievement("ToolbotGuardTeleporter", "Skills.Toolbot.Grenade", "RepeatFirstTeleporter", typeof(ToolbotGuardTeleporterAchievement.ToolbotGuardTeleporterServerAchievement))]
	public class ToolbotGuardTeleporterAchievement : BaseAchievement
	{
		// Token: 0x060028A4 RID: 10404 RVA: 0x000ACB56 File Offset: 0x000AAD56
		protected override int LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("ToolbotBody");
		}

		// Token: 0x060028A5 RID: 10405 RVA: 0x000AC8DE File Offset: 0x000AAADE
		protected override void OnBodyRequirementMet()
		{
			base.OnBodyRequirementMet();
			base.SetServerTracked(true);
		}

		// Token: 0x060028A6 RID: 10406 RVA: 0x000AC8ED File Offset: 0x000AAAED
		protected override void OnBodyRequirementBroken()
		{
			base.SetServerTracked(false);
			base.OnBodyRequirementBroken();
		}

		// Token: 0x020006D6 RID: 1750
		public class ToolbotGuardTeleporterServerAchievement : BaseServerAchievement
		{
			// Token: 0x060028A8 RID: 10408 RVA: 0x000ACB64 File Offset: 0x000AAD64
			public override void OnInstall()
			{
				base.OnInstall();
				TeleporterInteraction.onTeleporterBeginChargingGlobal += this.OnTeleporterBeginCharging;
				TeleporterInteraction.onTeleporterChargedGlobal += this.OnTeleporterCharged;
				this.killCount = 0;
				this.beetleQueenBodyIndex = BodyCatalog.FindBodyIndex("BeetleQueen2Body");
			}

			// Token: 0x060028A9 RID: 10409 RVA: 0x000ACBB0 File Offset: 0x000AADB0
			public override void OnUninstall()
			{
				TeleporterInteraction.onTeleporterChargedGlobal -= this.OnTeleporterCharged;
				TeleporterInteraction.onTeleporterBeginChargingGlobal -= this.OnTeleporterBeginCharging;
				this.SetStayedInZone(false);
				base.OnUninstall();
			}

			// Token: 0x060028AA RID: 10410 RVA: 0x000ACBE1 File Offset: 0x000AADE1
			private void OnTeleporterBeginCharging(TeleporterInteraction teleporterInteraction)
			{
				this.SetStayedInZone(true);
			}

			// Token: 0x060028AB RID: 10411 RVA: 0x000ACBEA File Offset: 0x000AADEA
			private void OnTeleporterCharged(TeleporterInteraction teleporterInteraction)
			{
				this.SetStayedInZone(false);
			}

			// Token: 0x060028AC RID: 10412 RVA: 0x000ACBF4 File Offset: 0x000AADF4
			private void SetStayedInZone(bool newStayedInZone)
			{
				if (this.stayedInZone == newStayedInZone)
				{
					return;
				}
				this.stayedInZone = newStayedInZone;
				if (this.stayedInZone)
				{
					RoR2Application.onFixedUpdate += this.FixedUpdateTeleporterCharging;
					GlobalEventManager.onCharacterDeathGlobal += this.OnCharacterDeath;
					this.UpdateStayedInZone();
					return;
				}
				this.killCount = 0;
				GlobalEventManager.onCharacterDeathGlobal -= this.OnCharacterDeath;
				RoR2Application.onFixedUpdate -= this.FixedUpdateTeleporterCharging;
			}

			// Token: 0x060028AD RID: 10413 RVA: 0x000ACC6C File Offset: 0x000AAE6C
			private void FixedUpdateTeleporterCharging()
			{
				this.UpdateStayedInZone();
			}

			// Token: 0x060028AE RID: 10414 RVA: 0x000ACC74 File Offset: 0x000AAE74
			private void UpdateStayedInZone()
			{
				if (this.stayedInZone)
				{
					TeleporterInteraction instance = TeleporterInteraction.instance;
					CharacterBody currentBody = base.GetCurrentBody();
					if (!instance || !currentBody || !instance.IsInChargingRange(currentBody))
					{
						this.SetStayedInZone(false);
					}
				}
			}

			// Token: 0x060028AF RID: 10415 RVA: 0x000ACCB8 File Offset: 0x000AAEB8
			private void OnCharacterDeath(DamageReport damageReport)
			{
				if (damageReport.victimBody && damageReport.victimBody.bodyIndex == this.beetleQueenBodyIndex)
				{
					this.killCount++;
					if (this.killCount >= this.killRequirement)
					{
						base.Grant();
					}
				}
			}

			// Token: 0x04002526 RID: 9510
			private bool stayedInZone;

			// Token: 0x04002527 RID: 9511
			private int killCount;

			// Token: 0x04002528 RID: 9512
			private int killRequirement = 2;

			// Token: 0x04002529 RID: 9513
			private int beetleQueenBodyIndex = -1;
		}
	}
}
