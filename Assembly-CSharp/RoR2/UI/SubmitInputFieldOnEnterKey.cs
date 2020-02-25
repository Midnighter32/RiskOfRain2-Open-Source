using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200063C RID: 1596
	[RequireComponent(typeof(TMP_InputField))]
	public class SubmitInputFieldOnEnterKey : MonoBehaviour
	{
		// Token: 0x06002594 RID: 9620 RVA: 0x000A387A File Offset: 0x000A1A7A
		private void Awake()
		{
			this.inputField = base.GetComponent<TMP_InputField>();
		}

		// Token: 0x06002595 RID: 9621 RVA: 0x000A3888 File Offset: 0x000A1A88
		private void Update()
		{
			if (this.inputField.isFocused && this.inputField.text != "")
			{
				Input.GetKeyDown(KeyCode.Return);
			}
		}

		// Token: 0x04002342 RID: 9026
		private TMP_InputField inputField;
	}
}
