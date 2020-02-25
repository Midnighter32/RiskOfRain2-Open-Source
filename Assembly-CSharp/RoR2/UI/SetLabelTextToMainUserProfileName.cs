using System;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000628 RID: 1576
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class SetLabelTextToMainUserProfileName : MonoBehaviour
	{
		// Token: 0x06002540 RID: 9536 RVA: 0x000A2381 File Offset: 0x000A0581
		private void Awake()
		{
			this.label = base.GetComponent<TextMeshProUGUI>();
		}

		// Token: 0x06002541 RID: 9537 RVA: 0x000A2390 File Offset: 0x000A0590
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

		// Token: 0x040022F7 RID: 8951
		private TextMeshProUGUI label;
	}
}
