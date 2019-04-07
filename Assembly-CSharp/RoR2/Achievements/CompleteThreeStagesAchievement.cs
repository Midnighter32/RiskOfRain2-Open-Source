using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x02000694 RID: 1684
	[RegisterAchievement("CompleteThreeStages", "Characters.Huntress", null, null)]
	public class CompleteThreeStagesAchievement : BaseAchievement
	{
		// Token: 0x0600258C RID: 9612 RVA: 0x000AF57B File Offset: 0x000AD77B
		public override void OnInstall()
		{
			base.OnInstall();
			TeleporterInteraction.onTeleporterChargedGlobal += this.OnTeleporterCharged;
		}

		// Token: 0x0600258D RID: 9613 RVA: 0x000AF594 File Offset: 0x000AD794
		public override void OnUninstall()
		{
			TeleporterInteraction.onTeleporterChargedGlobal -= this.OnTeleporterCharged;
			base.OnUninstall();
		}

		// Token: 0x0600258E RID: 9614 RVA: 0x000AF5AD File Offset: 0x000AD7AD
		private void OnTeleporterCharged(TeleporterInteraction teleporterInteraction)
		{
			this.Check();
		}

		// Token: 0x0600258F RID: 9615 RVA: 0x000AF5B8 File Offset: 0x000AD7B8
		private void Check()
		{
			if (Run.instance && Run.instance.GetType() == typeof(Run))
			{
				SceneDef sceneDefForCurrentScene = SceneCatalog.GetSceneDefForCurrentScene();
				if (sceneDefForCurrentScene == null)
				{
					return;
				}
				if (this.localUser.currentNetworkUser.masterPlayerStatsComponent.currentStats.GetStatValueULong(StatDef.totalDeaths) == 0UL && sceneDefForCurrentScene.stageOrder == 3)
				{
					base.Grant();
				}
			}
		}

		// Token: 0x04002869 RID: 10345
		private const int requirement = 3;
	}
}
