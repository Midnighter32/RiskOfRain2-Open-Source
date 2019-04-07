using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000647 RID: 1607
	[RequireComponent(typeof(TMP_InputField))]
	public class SubmitInputFieldOnEnterKey : MonoBehaviour
	{
		// Token: 0x060023F5 RID: 9205 RVA: 0x000A8E6A File Offset: 0x000A706A
		private void Awake()
		{
			this.inputField = base.GetComponent<TMP_InputField>();
		}

		// Token: 0x060023F6 RID: 9206 RVA: 0x000A8E78 File Offset: 0x000A7078
		private void Update()
		{
			if (this.inputField.isFocused && this.inputField.text != "")
			{
				Input.GetKeyDown(KeyCode.Return);
			}
		}

		// Token: 0x040026DF RID: 9951
		private TMP_InputField inputField;
	}
}
