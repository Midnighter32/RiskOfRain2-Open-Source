using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005B8 RID: 1464
	public class ButtonSelectionController : MonoBehaviour
	{
		// Token: 0x060020CD RID: 8397 RVA: 0x0009A118 File Offset: 0x00098318
		public void SelectThisButton(MPButton selectedButton)
		{
			for (int i = 0; i < this.buttons.Length; i++)
			{
				this.buttons[i] == selectedButton;
			}
		}

		// Token: 0x04002354 RID: 9044
		public MPButton[] buttons;
	}
}
