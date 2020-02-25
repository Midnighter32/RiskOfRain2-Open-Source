using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2.UI.MainMenu
{
	// Token: 0x0200065A RID: 1626
	[RequireComponent(typeof(RectTransform))]
	public class BaseMainMenuScreen : MonoBehaviour
	{
		// Token: 0x06002624 RID: 9764 RVA: 0x0000B933 File Offset: 0x00009B33
		public virtual bool IsReadyToLeave()
		{
			return true;
		}

		// Token: 0x06002625 RID: 9765 RVA: 0x000A5BD2 File Offset: 0x000A3DD2
		public virtual void OnEnter(MainMenuController mainMenuController)
		{
			this.myMainMenuController = mainMenuController;
			this.onEnter.Invoke();
		}

		// Token: 0x06002626 RID: 9766 RVA: 0x000A5BE6 File Offset: 0x000A3DE6
		public virtual void OnExit(MainMenuController mainMenuController)
		{
			if (this.myMainMenuController == mainMenuController)
			{
				this.myMainMenuController = null;
			}
			this.onExit.Invoke();
		}

		// Token: 0x040023E6 RID: 9190
		public Transform desiredCameraTransform;

		// Token: 0x040023E7 RID: 9191
		[HideInInspector]
		public bool shouldDisplay;

		// Token: 0x040023E8 RID: 9192
		protected MainMenuController myMainMenuController;

		// Token: 0x040023E9 RID: 9193
		public UnityEvent onEnter;

		// Token: 0x040023EA RID: 9194
		public UnityEvent onExit;
	}
}
