using System;
using Rewired;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005FC RID: 1532
	public class LocalUserSignInCardController : MonoBehaviour
	{
		// Token: 0x0600224F RID: 8783 RVA: 0x000A2160 File Offset: 0x000A0360
		private void Update()
		{
			if (this.requestedUserProfile != null != this.userProfileSelectionList)
			{
				if (!this.userProfileSelectionList)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.userProfileSelectionListPrefab, base.transform);
					this.userProfileSelectionList = gameObject.GetComponent<UserProfileListController>();
					this.userProfileSelectionList.GetComponent<MPEventSystemProvider>().eventSystem = MPEventSystemManager.FindEventSystem(this.rewiredPlayer);
					this.userProfileSelectionList.onProfileSelected += this.OnUserSelectedUserProfile;
				}
				else
				{
					UnityEngine.Object.Destroy(this.userProfileSelectionList.gameObject);
					this.userProfileSelectionList = null;
				}
			}
			if (this.rewiredPlayer == null)
			{
				this.nameLabel.gameObject.SetActive(false);
				this.promptLabel.text = "Press 'Start'";
				this.cardImage.color = this.unselectedColor;
				this.cardImage.sprite = this.playerCardNone;
				return;
			}
			this.cardImage.color = this.selectedColor;
			this.nameLabel.gameObject.SetActive(true);
			if (this.requestedUserProfile == null)
			{
				this.cardImage.sprite = this.playerCardNone;
				this.nameLabel.text = "";
				this.promptLabel.text = "...";
				return;
			}
			this.cardImage.sprite = this.playerCardKBM;
			this.nameLabel.text = this.requestedUserProfile.name;
			this.promptLabel.text = "";
		}

		// Token: 0x170002FF RID: 767
		// (get) Token: 0x06002250 RID: 8784 RVA: 0x000A22DB File Offset: 0x000A04DB
		// (set) Token: 0x06002251 RID: 8785 RVA: 0x000A22E3 File Offset: 0x000A04E3
		public Player rewiredPlayer
		{
			get
			{
				return this._rewiredPlayer;
			}
			set
			{
				if (this._rewiredPlayer == value)
				{
					return;
				}
				this._rewiredPlayer = value;
				if (this._rewiredPlayer == null)
				{
					this.requestedUserProfile = null;
				}
			}
		}

		// Token: 0x17000300 RID: 768
		// (get) Token: 0x06002252 RID: 8786 RVA: 0x000A2305 File Offset: 0x000A0505
		// (set) Token: 0x06002253 RID: 8787 RVA: 0x000A230D File Offset: 0x000A050D
		public UserProfile requestedUserProfile
		{
			get
			{
				return this._requestedUserProfile;
			}
			private set
			{
				if (this._requestedUserProfile == value)
				{
					return;
				}
				if (this._requestedUserProfile != null)
				{
					this._requestedUserProfile.isClaimed = false;
				}
				this._requestedUserProfile = value;
				if (this._requestedUserProfile != null)
				{
					this._requestedUserProfile.isClaimed = true;
				}
			}
		}

		// Token: 0x06002254 RID: 8788 RVA: 0x000A2348 File Offset: 0x000A0548
		private void OnUserSelectedUserProfile(UserProfile userProfile)
		{
			this.requestedUserProfile = userProfile;
		}

		// Token: 0x04002569 RID: 9577
		public TextMeshProUGUI nameLabel;

		// Token: 0x0400256A RID: 9578
		public TextMeshProUGUI promptLabel;

		// Token: 0x0400256B RID: 9579
		public Image cardImage;

		// Token: 0x0400256C RID: 9580
		public Sprite playerCardNone;

		// Token: 0x0400256D RID: 9581
		public Sprite playerCardKBM;

		// Token: 0x0400256E RID: 9582
		public Sprite playerCardController;

		// Token: 0x0400256F RID: 9583
		public Color unselectedColor;

		// Token: 0x04002570 RID: 9584
		public Color selectedColor;

		// Token: 0x04002571 RID: 9585
		private UserProfileListController userProfileSelectionList;

		// Token: 0x04002572 RID: 9586
		public GameObject userProfileSelectionListPrefab;

		// Token: 0x04002573 RID: 9587
		private Player _rewiredPlayer;

		// Token: 0x04002574 RID: 9588
		private UserProfile _requestedUserProfile;
	}
}
