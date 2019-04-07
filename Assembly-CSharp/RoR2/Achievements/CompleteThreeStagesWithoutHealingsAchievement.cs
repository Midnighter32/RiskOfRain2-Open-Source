using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x02000695 RID: 1685
	[RegisterAchievement("CompleteThreeStagesWithoutHealing", "Items.IncreaseHealing", null, null)]
	public class CompleteThreeStagesWithoutHealingsAchievement : BaseAchievement
	{
		// Token: 0x06002591 RID: 9617 RVA: 0x000AF62C File Offset: 0x000AD82C
		public override void OnInstall()
		{
			base.OnInstall();
			TeleporterInteraction.onTeleporterFinishGlobal += this.OnTeleporterFinish;
		}

		// Token: 0x06002592 RID: 9618 RVA: 0x000AF645 File Offset: 0x000AD845
		public override void OnUninstall()
		{
			TeleporterInteraction.onTeleporterFinishGlobal -= this.OnTeleporterFinish;
			base.OnUninstall();
		}

		// Token: 0x06002593 RID: 9619 RVA: 0x000AF65E File Offset: 0x000AD85E
		private void OnTeleporterFinish(TeleporterInteraction teleporterInteraction)
		{
			this.Check();
		}

		// Token: 0x06002594 RID: 9620 RVA: 0x000AF668 File Offset: 0x000AD868
		private void Check()
		{
			if (Run.instance && Run.instance.GetType() == typeof(Run) && this.localUser != null && this.localUser.currentNetworkUser != null)
			{
				SceneDef sceneDefForCurrentScene = SceneCatalog.GetSceneDefForCurrentScene();
				StatSheet currentStats = this.localUser.currentNetworkUser.masterPlayerStatsComponent.currentStats;
				if (sceneDefForCurrentScene.stageOrder >= 3 && currentStats.GetStatValueULong(StatDef.totalHealthHealed) <= 0f && this.localUser.cachedBody && this.localUser.cachedBody.healthComponent && this.localUser.cachedBody.healthComponent.alive)
				{
					base.Grant();
				}
			}
		}

		// Token: 0x0400286A RID: 10346
		private const int requirement = 3;
	}
}
