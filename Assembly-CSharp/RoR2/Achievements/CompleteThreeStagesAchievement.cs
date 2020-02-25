using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x0200069A RID: 1690
	[RegisterAchievement("CompleteThreeStages", "Characters.Huntress", null, null)]
	public class CompleteThreeStagesAchievement : BaseAchievement
	{
		// Token: 0x0600278F RID: 10127 RVA: 0x000AAD53 File Offset: 0x000A8F53
		public override void OnInstall()
		{
			base.OnInstall();
			TeleporterInteraction.onTeleporterChargedGlobal += this.OnTeleporterCharged;
		}

		// Token: 0x06002790 RID: 10128 RVA: 0x000AAD6C File Offset: 0x000A8F6C
		public override void OnUninstall()
		{
			TeleporterInteraction.onTeleporterChargedGlobal -= this.OnTeleporterCharged;
			base.OnUninstall();
		}

		// Token: 0x06002791 RID: 10129 RVA: 0x000AAD85 File Offset: 0x000A8F85
		private void OnTeleporterCharged(TeleporterInteraction teleporterInteraction)
		{
			this.Check();
		}

		// Token: 0x06002792 RID: 10130 RVA: 0x000AAD90 File Offset: 0x000A8F90
		private void Check()
		{
			if (Run.instance && Run.instance.GetType() == typeof(Run))
			{
				SceneDef sceneDefForCurrentScene = SceneCatalog.GetSceneDefForCurrentScene();
				if (sceneDefForCurrentScene == null)
				{
					return;
				}
				if (base.localUser.currentNetworkUser.masterPlayerStatsComponent.currentStats.GetStatValueULong(StatDef.totalDeaths) == 0UL && sceneDefForCurrentScene.stageOrder == 3)
				{
					base.Grant();
				}
			}
		}

		// Token: 0x040024F5 RID: 9461
		private const int requirement = 3;
	}
}
