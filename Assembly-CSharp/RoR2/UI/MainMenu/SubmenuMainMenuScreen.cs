using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2.UI.MainMenu
{
	// Token: 0x0200065E RID: 1630
	public class SubmenuMainMenuScreen : BaseMainMenuScreen
	{
		// Token: 0x0600263F RID: 9791 RVA: 0x000A62E8 File Offset: 0x000A44E8
		public override void OnEnter(MainMenuController mainMenuController)
		{
			base.OnEnter(mainMenuController);
			this.submenuPanelInstance = UnityEngine.Object.Instantiate<GameObject>(this.submenuPanelPrefab, base.transform);
		}

		// Token: 0x06002640 RID: 9792 RVA: 0x000A6308 File Offset: 0x000A4508
		public override void OnExit(MainMenuController mainMenuController)
		{
			UnityEngine.Object.Destroy(this.submenuPanelInstance);
			base.OnExit(mainMenuController);
		}

		// Token: 0x06002641 RID: 9793 RVA: 0x000A631C File Offset: 0x000A451C
		public void Update()
		{
			if (!this.submenuPanelInstance && this.myMainMenuController)
			{
				this.myMainMenuController.SetDesiredMenuScreen(this.myMainMenuController.titleMenuScreen);
			}
		}

		// Token: 0x04002409 RID: 9225
		[FormerlySerializedAs("settingsPanelPrefab")]
		public GameObject submenuPanelPrefab;

		// Token: 0x0400240A RID: 9226
		private GameObject submenuPanelInstance;
	}
}
