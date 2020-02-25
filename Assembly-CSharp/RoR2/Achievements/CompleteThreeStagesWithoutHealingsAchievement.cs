using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x0200069B RID: 1691
	[RegisterAchievement("CompleteThreeStagesWithoutHealing", "Items.IncreaseHealing", null, null)]
	public class CompleteThreeStagesWithoutHealingsAchievement : BaseAchievement
	{
		// Token: 0x06002794 RID: 10132 RVA: 0x000AAE04 File Offset: 0x000A9004
		public override void OnInstall()
		{
			base.OnInstall();
			TeleporterInteraction.onTeleporterFinishGlobal += this.OnTeleporterFinish;
		}

		// Token: 0x06002795 RID: 10133 RVA: 0x000AAE1D File Offset: 0x000A901D
		public override void OnUninstall()
		{
			TeleporterInteraction.onTeleporterFinishGlobal -= this.OnTeleporterFinish;
			base.OnUninstall();
		}

		// Token: 0x06002796 RID: 10134 RVA: 0x000AAE36 File Offset: 0x000A9036
		private void OnTeleporterFinish(TeleporterInteraction teleporterInteraction)
		{
			this.Check();
		}

		// Token: 0x06002797 RID: 10135 RVA: 0x000AAE40 File Offset: 0x000A9040
		private void Check()
		{
			if (Run.instance && Run.instance.GetType() == typeof(Run) && base.localUser != null && base.localUser.currentNetworkUser != null)
			{
				SceneDef sceneDefForCurrentScene = SceneCatalog.GetSceneDefForCurrentScene();
				StatSheet currentStats = base.localUser.currentNetworkUser.masterPlayerStatsComponent.currentStats;
				if (sceneDefForCurrentScene.stageOrder >= 3 && currentStats.GetStatValueULong(StatDef.totalHealthHealed) <= 0f && base.localUser.cachedBody && base.localUser.cachedBody.healthComponent && base.localUser.cachedBody.healthComponent.alive)
				{
					base.Grant();
				}
			}
		}

		// Token: 0x040024F6 RID: 9462
		private const int requirement = 3;
	}
}
