using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2.UI.MainMenu
{
	// Token: 0x02000669 RID: 1641
	public class SubmenuMainMenuScreen : BaseMainMenuScreen
	{
		// Token: 0x0600249B RID: 9371 RVA: 0x000AB7AC File Offset: 0x000A99AC
		public override void OnEnter(MainMenuController mainMenuController)
		{
			base.OnEnter(mainMenuController);
			this.submenuPanelInstance = UnityEngine.Object.Instantiate<GameObject>(this.submenuPanelPrefab, base.transform);
		}

		// Token: 0x0600249C RID: 9372 RVA: 0x000AB7CC File Offset: 0x000A99CC
		public override void OnExit(MainMenuController mainMenuController)
		{
			UnityEngine.Object.Destroy(this.submenuPanelInstance);
			base.OnExit(mainMenuController);
		}

		// Token: 0x0600249D RID: 9373 RVA: 0x000AB7E0 File Offset: 0x000A99E0
		public void Update()
		{
			if (!this.submenuPanelInstance && this.myMainMenuController)
			{
				this.myMainMenuController.SetDesiredMenuScreen(this.myMainMenuController.titleMenuScreen);
			}
		}

		// Token: 0x040027A2 RID: 10146
		[FormerlySerializedAs("settingsPanelPrefab")]
		public GameObject submenuPanelPrefab;

		// Token: 0x040027A3 RID: 10147
		private GameObject submenuPanelInstance;
	}
}
