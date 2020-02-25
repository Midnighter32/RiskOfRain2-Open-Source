using System;

namespace RoR2.Achievements.Loader
{
	// Token: 0x020006E7 RID: 1767
	[RegisterAchievement("LoaderSpeedRun", "Skills.Loader.YankHook", "DefeatSuperRoboBallBoss", null)]
	public class LoaderSpeedRunAchievement : BaseAchievement
	{
		// Token: 0x06002909 RID: 10505 RVA: 0x000AD513 File Offset: 0x000AB713
		public override void OnInstall()
		{
			base.OnInstall();
			this.requiredSceneDef = SceneCatalog.GetSceneDefFromSceneName("mysteryspace");
		}

		// Token: 0x0600290A RID: 10506 RVA: 0x000AB9EB File Offset: 0x000A9BEB
		protected override int LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("LoaderBody");
		}

		// Token: 0x0600290B RID: 10507 RVA: 0x000AD52B File Offset: 0x000AB72B
		protected override void OnBodyRequirementMet()
		{
			base.OnBodyRequirementMet();
			SceneCatalog.onMostRecentSceneDefChanged += this.OnMostRecentSceneDefChanged;
		}

		// Token: 0x0600290C RID: 10508 RVA: 0x000AD544 File Offset: 0x000AB744
		private void OnMostRecentSceneDefChanged(SceneDef sceneDef)
		{
			if (sceneDef == this.requiredSceneDef && Run.instance.GetRunStopwatch() <= LoaderSpeedRunAchievement.requirement)
			{
				base.Grant();
			}
		}

		// Token: 0x0600290D RID: 10509 RVA: 0x000AD56B File Offset: 0x000AB76B
		protected override void OnBodyRequirementBroken()
		{
			SceneCatalog.onMostRecentSceneDefChanged -= this.OnMostRecentSceneDefChanged;
			base.OnBodyRequirementBroken();
		}

		// Token: 0x0400253E RID: 9534
		private SceneDef requiredSceneDef;

		// Token: 0x0400253F RID: 9535
		private static readonly float requirement = 1500f;
	}
}
