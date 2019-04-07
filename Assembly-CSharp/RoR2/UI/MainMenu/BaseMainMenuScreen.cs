using System;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2.UI.MainMenu
{
	// Token: 0x02000665 RID: 1637
	[RequireComponent(typeof(RectTransform))]
	public class BaseMainMenuScreen : MonoBehaviour
	{
		// Token: 0x06002480 RID: 9344 RVA: 0x0000AE8B File Offset: 0x0000908B
		public virtual bool IsReadyToLeave()
		{
			return true;
		}

		// Token: 0x06002481 RID: 9345 RVA: 0x000AB096 File Offset: 0x000A9296
		public virtual void OnEnter(MainMenuController mainMenuController)
		{
			this.myMainMenuController = mainMenuController;
			this.onEnter.Invoke();
		}

		// Token: 0x06002482 RID: 9346 RVA: 0x000AB0AA File Offset: 0x000A92AA
		public virtual void OnExit(MainMenuController mainMenuController)
		{
			if (this.myMainMenuController == mainMenuController)
			{
				this.myMainMenuController = null;
			}
			this.onExit.Invoke();
		}

		// Token: 0x0400277F RID: 10111
		public Transform desiredCameraTransform;

		// Token: 0x04002780 RID: 10112
		[HideInInspector]
		public bool shouldDisplay;

		// Token: 0x04002781 RID: 10113
		protected MainMenuController myMainMenuController;

		// Token: 0x04002782 RID: 10114
		public UnityEvent onEnter;

		// Token: 0x04002783 RID: 10115
		public UnityEvent onExit;
	}
}
