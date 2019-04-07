using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000625 RID: 1573
	[RequireComponent(typeof(MPEventSystemLocator))]
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class ProfileNameLabel : MonoBehaviour
	{
		// Token: 0x06002351 RID: 9041 RVA: 0x000A653E File Offset: 0x000A473E
		private void Awake()
		{
			this.eventSystemLocator = base.GetComponent<MPEventSystemLocator>();
			this.label = base.GetComponent<TextMeshProUGUI>();
		}

		// Token: 0x06002352 RID: 9042 RVA: 0x000A6558 File Offset: 0x000A4758
		private void Update()
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

		// Token: 0x0400264E RID: 9806
		public string token;

		// Token: 0x0400264F RID: 9807
		private MPEventSystemLocator eventSystemLocator;

		// Token: 0x04002650 RID: 9808
		private TextMeshProUGUI label;

		// Token: 0x04002651 RID: 9809
		private string currentUserName;
	}
}
