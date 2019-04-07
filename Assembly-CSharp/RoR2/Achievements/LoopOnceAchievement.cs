using System;

namespace RoR2.Achievements
{
	// Token: 0x020006AE RID: 1710
	[RegisterAchievement("LoopOnce", "Items.BounceNearby", null, null)]
	public class LoopOnceAchievement : BaseAchievement
	{
		// Token: 0x060025FF RID: 9727 RVA: 0x000B00AB File Offset: 0x000AE2AB
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onStatsReceived += this.Check;
			this.Check();
		}

		// Token: 0x06002600 RID: 9728 RVA: 0x000B00D0 File Offset: 0x000AE2D0
		public override void OnUninstall()
		{
			this.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x06002601 RID: 9729 RVA: 0x000B00F0 File Offset: 0x000AE2F0
		private void Check()
		{
			if (Run.instance && Run.instance.GetType() == typeof(Run))
			{
				SceneDef sceneDefForCurrentScene = SceneCatalog.GetSceneDefForCurrentScene();
				if (sceneDefForCurrentScene && sceneDefForCurrentScene.stageOrder < Run.instance.stageClearCount)
				{
					base.Grant();
				}
			}
		}
	}
}
