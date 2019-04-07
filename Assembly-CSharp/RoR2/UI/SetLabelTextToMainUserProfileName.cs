using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000635 RID: 1589
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class SetLabelTextToMainUserProfileName : MonoBehaviour
	{
		// Token: 0x060023A6 RID: 9126 RVA: 0x000A7AF9 File Offset: 0x000A5CF9
		private void Awake()
		{
			this.label = base.GetComponent<TextMeshProUGUI>();
		}

		// Token: 0x060023A7 RID: 9127 RVA: 0x000A7B08 File Offset: 0x000A5D08
		private void OnEnable()
		{
			LocalUser localUser = LocalUserManager.FindLocalUser(0);
			if (localUser != null)
			{
				string name = localUser.userProfile.name;
				this.label.text = string.Format(Language.GetString("TITLE_PROFILE"), name);
				return;
			}
			this.label.text = "NO USER";
		}

		// Token: 0x0400269A RID: 9882
		private TextMeshProUGUI label;
	}
}
