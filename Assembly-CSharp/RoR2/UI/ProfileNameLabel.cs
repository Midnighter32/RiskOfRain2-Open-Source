using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000614 RID: 1556
	[RequireComponent(typeof(TextMeshProUGUI))]
	[RequireComponent(typeof(MPEventSystemLocator))]
	public class ProfileNameLabel : MonoBehaviour
	{
		// Token: 0x060024D0 RID: 9424 RVA: 0x000A091E File Offset: 0x0009EB1E
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			this.label = base.GetComponent<TextMeshProUGUI>();
		}

		// Token: 0x060024D1 RID: 9425 RVA: 0x000A0938 File Offset: 0x0009EB38
		private void LateUpdate()
		{
			MPEventSystem eventSystem = this.eventSystemLocator.eventSystem;
			string text;
			if (eventSystem == null)
			{
				text = null;
			}
			else
			{
				LocalUser localUser = eventSystem.localUser;
				text = ((localUser != null) ? localUser.userProfile.name : null);
			}
			string a = text ?? string.Empty;
			if (a != this.currentUserName)
			{
				this.currentUserName = a;
				this.label.text = Language.GetStringFormatted(this.token, new object[]
				{
					this.currentUserName
				});
			}
		}

		// Token: 0x04002297 RID: 8855
		public string token;

		// Token: 0x04002298 RID: 8856
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04002299 RID: 8857
		private TextMeshProUGUI label;

		// Token: 0x0400229A RID: 8858
		private string currentUserName;
	}
}
