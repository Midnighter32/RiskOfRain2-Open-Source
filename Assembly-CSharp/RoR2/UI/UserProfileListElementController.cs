using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace RoR2.UI
{
	// Token: 0x02000649 RID: 1609
	[RequireComponent(typeof(MPButton))]
	public class UserProfileListElementController : MonoBehaviour
	{
		// Token: 0x060025E4 RID: 9700 RVA: 0x000A4EEF File Offset: 0x000A30EF
		private void Awake()
		{
			this.button = base.GetComponent<MPButton>();
			this.button.onClick.AddListener(new UnityAction(this.InformListControllerOfSelection));
		}

		// Token: 0x060025E5 RID: 9701 RVA: 0x000A4F19 File Offset: 0x000A3119
		private void InformListControllerOfSelection()
		{
			if (!this.userProfile.isCorrupted)
			{
				this.listController.SendProfileSelection(this.userProfile);
			}
		}

		// Token: 0x170003E3 RID: 995
		// (get) Token: 0x060025E6 RID: 9702 RVA: 0x000A4F39 File Offset: 0x000A3139
		// (set) Token: 0x060025E7 RID: 9703 RVA: 0x000A4F44 File Offset: 0x000A3144
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

		// Token: 0x040023A3 RID: 9123
		public TextMeshProUGUI nameLabel;

		// Token: 0x040023A4 RID: 9124
		private MPButton button;

		// Token: 0x040023A5 RID: 9125
		public TextMeshProUGUI playTimeLabel;

		// Token: 0x040023A6 RID: 9126
		[NonSerialized]
		public UserProfileListController listController;

		// Token: 0x040023A7 RID: 9127
		private UserProfile _userProfile;
	}
}
