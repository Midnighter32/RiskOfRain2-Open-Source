using System;

namespace RoR2.Achievements.Toolbot
{
	// Token: 0x020006D7 RID: 1751
	[RegisterAchievement("ToolbotKillImpBossWithBfg", "Skills.Toolbot.Buzzsaw", "RepeatFirstTeleporter", typeof(ToolbotKillImpBossWithBfgAchievement.ToolbotKillImpBossWithBfgServerAchievement))]
	public class ToolbotKillImpBossWithBfgAchievement : BaseAchievement
	{
		// Token: 0x060028B1 RID: 10417 RVA: 0x000ACB56 File Offset: 0x000AAD56
		protected override int LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("ToolbotBody");
		}

		// Token: 0x060028B2 RID: 10418 RVA: 0x000AC8DE File Offset: 0x000AAADE
		protected override void OnBodyRequirementMet()
		{
			base.OnBodyRequirementMet();
			base.SetServerTracked(true);
		}

		// Token: 0x060028B3 RID: 10419 RVA: 0x000AC8ED File Offset: 0x000AAAED
		protected override void OnBodyRequirementBroken()
		{
			base.SetServerTracked(false);
			base.OnBodyRequirementBroken();
		}

		// Token: 0x020006D8 RID: 1752
		private class ToolbotKillImpBossWithBfgServerAchievement : BaseServerAchievement
		{
			// Token: 0x060028B5 RID: 10421 RVA: 0x000ACD1D File Offset: 0x000AAF1D
			public override void OnInstall()
			{
				base.OnInstall();
				this.impBossBodyIndex = BodyCatalog.FindBodyIndex("ImpBossBody");
				this.bfgProjectileIndex = ProjectileCatalog.FindProjectileIndex("BeamSphere");
				GlobalEventManager.onCharacterDeathGlobal += this.OnCharacterDeath;
			}

			// Token: 0x060028B6 RID: 10422 RVA: 0x000ACD56 File Offset: 0x000AAF56
			public override void OnUninstall()
			{
				GlobalEventManager.onCharacterDeathGlobal -= this.OnCharacterDeath;
				base.OnUninstall();
			}

			// Token: 0x060028B7 RID: 10423 RVA: 0x000ACD70 File Offset: 0x000AAF70
			private void OnCharacterDeath(DamageReport damageReport)
			{
				if (!damageReport.victimBody)
				{
					return;
				}
				if (damageReport.victimBody.bodyIndex != this.impBossBodyIndex)
				{
					return;
				}
				if (!base.IsCurrentBody(damageReport.damageInfo.attacker))
				{
					return;
				}
				if (ProjectileCatalog.GetProjectileIndex(damageReport.damageInfo.inflictor) != this.bfgProjectileIndex)
				{
					return;
				}
				base.Grant();
			}

			// Token: 0x0400252A RID: 9514
			private int impBossBodyIndex = -1;

			// Token: 0x0400252B RID: 9515
			private int bfgProjectileIndex = -1;
		}
	}
}
