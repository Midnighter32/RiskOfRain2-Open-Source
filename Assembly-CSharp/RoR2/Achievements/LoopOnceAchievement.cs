using System;

namespace RoR2.Achievements
{
	// Token: 0x020006BB RID: 1723
	[RegisterAchievement("LoopOnce", "Items.BounceNearby", null, null)]
	public class LoopOnceAchievement : BaseAchievement
	{
		// Token: 0x06002823 RID: 10275 RVA: 0x000ABB97 File Offset: 0x000A9D97
		public override void OnInstall()
		{
			base.OnInstall();
			base.userProfile.onStatsReceived += this.Check;
			this.Check();
		}

		// Token: 0x06002824 RID: 10276 RVA: 0x000ABBBC File Offset: 0x000A9DBC
		public override void OnUninstall()
		{
			base.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x06002825 RID: 10277 RVA: 0x000ABBDC File Offset: 0x000A9DDC
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
