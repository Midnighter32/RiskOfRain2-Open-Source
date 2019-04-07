using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2.UI
{
	// Token: 0x02000654 RID: 1620
	[RequireComponent(typeof(MPButton))]
	public class UserProfileListElementController : MonoBehaviour
	{
		// Token: 0x06002445 RID: 9285 RVA: 0x000AA427 File Offset: 0x000A8627
		private void Awake()
		{
			this.button = base.GetComponent<MPButton>();
			this.button.onClick.AddListener(new UnityAction(this.InformListControllerOfSelection));
		}

		// Token: 0x06002446 RID: 9286 RVA: 0x000AA451 File Offset: 0x000A8651
		private void InformListControllerOfSelection()
		{
			if (!this.userProfile.isCorrupted)
			{
				this.listController.SendProfileSelection(this.userProfile);
			}
		}

		// Token: 0x17000324 RID: 804
		// (get) Token: 0x06002447 RID: 9287 RVA: 0x000AA471 File Offset: 0x000A8671
		// (set) Token: 0x06002448 RID: 9288 RVA: 0x000AA47C File Offset: 0x000A867C
		public UserProfile userProfile
		{
			get
			{
				return this._userProfile;
			}
			set
			{
				this._userProfile = value;
				this.nameLabel.text = ((this._userProfile == null) ? "???" : this._userProfile.name);
				if (this.playTimeLabel)
				{
					TimeSpan timeSpan = TimeSpan.FromSeconds(this._userProfile.totalLoginSeconds);
					this.playTimeLabel.text = string.Format("{0}:{1:D2}", (uint)timeSpan.TotalHours, (uint)timeSpan.Minutes);
				}
			}
		}

		// Token: 0x0400273E RID: 10046
		public TextMeshProUGUI nameLabel;

		// Token: 0x0400273F RID: 10047
		private MPButton button;

		// Token: 0x04002740 RID: 10048
		public TextMeshProUGUI playTimeLabel;

		// Token: 0x04002741 RID: 10049
		[NonSerialized]
		public UserProfileListController listController;

		// Token: 0x04002742 RID: 10050
		private UserProfile _userProfile;
	}
}
