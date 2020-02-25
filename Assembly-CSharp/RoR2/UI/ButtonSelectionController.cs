using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000592 RID: 1426
	public class ButtonSelectionController : MonoBehaviour
	{
		// Token: 0x060021EF RID: 8687 RVA: 0x00092904 File Offset: 0x00090B04
		public void SelectThisButton(MPButton selectedButton)
		{
			for (int i = 0; i < this.buttons.Length; i++)
			{
				this.buttons[i] == selectedButton;
			}
		}

		// Token: 0x04001F4A RID: 8010
		public MPButton[] buttons;
	}
}
