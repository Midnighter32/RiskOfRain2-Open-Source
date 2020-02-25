using System;

namespace RoR2.Achievements.Treebot
{
	// Token: 0x020006D1 RID: 1745
	[RegisterAchievement("TreebotDunkClayBoss", "Skills.Treebot.PlantSonicBoom", "RescueTreebot", typeof(TreebotDunkClayBossAchievement.TreebotDunkClayBossServerAchievement))]
	public class TreebotDunkClayBossAchievement : BaseAchievement
	{
		// Token: 0x0600288E RID: 10382 RVA: 0x000AC8CA File Offset: 0x000AAACA
		protected override int LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("TreebotBody");
		}

		// Token: 0x0600288F RID: 10383 RVA: 0x000AC8DE File Offset: 0x000AAADE
		protected override void OnBodyRequirementMet()
		{
			base.OnBodyRequirementMet();
			base.SetServerTracked(true);
		}

		// Token: 0x06002890 RID: 10384 RVA: 0x000AC8ED File Offset: 0x000AAAED
		protected override void OnBodyRequirementBroken()
		{
			base.SetServerTracked(false);
			base.OnBodyRequirementBroken();
		}

		// Token: 0x020006D2 RID: 1746
		private class TreebotDunkClayBossServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002892 RID: 10386 RVA: 0x000AC8FC File Offset: 0x000AAAFC
			public override void OnInstall()
			{
				base.OnInstall();
				this.clayBossBodyIndex = BodyCatalog.FindBodyIndex("ClayBossBody");
				this.requiredSceneDef = SceneCatalog.GetSceneDefFromSceneName("goolake");
				GlobalEventManager.onCharacterDeathGlobal += this.OnCharacterDeathGlobal;
			}

			// Token: 0x06002893 RID: 10387 RVA: 0x000AC938 File Offset: 0x000AAB38
			private void OnCharacterDeathGlobal(DamageReport damageReport)
			{
				if (damageReport.victimBodyIndex == this.clayBossBodyIndex && base.IsCurrentBody(damageReport.attackerBody) && damageReport.damageInfo.inflictor && damageReport.damageInfo.inflictor.GetComponent<MapZone>() && SceneCatalog.mostRecentSceneDef == this.requiredSceneDef)
				{
					base.Grant();
				}
			}

			// Token: 0x06002894 RID: 10388 RVA: 0x000AC9A2 File Offset: 0x000AABA2
			public override void OnUninstall()
			{
				GlobalEventManager.onCharacterDeathGlobal -= this.OnCharacterDeathGlobal;
				base.OnUninstall();
			}

			// Token: 0x0400251F RID: 9503
			private int clayBossBodyIndex;

			// Token: 0x04002520 RID: 9504
			private SceneDef requiredSceneDef;
		}
	}
}
