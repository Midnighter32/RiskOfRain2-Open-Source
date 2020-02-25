using System;
using Rewired;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoR2.UI
{
	// Token: 0x020005EA RID: 1514
	public class LocalUserSignInCardController : MonoBehaviour
	{
		// Token: 0x060023BA RID: 9146 RVA: 0x0009C0BC File Offset: 0x0009A2BC
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

		// Token: 0x170003B5 RID: 949
		// (get) Token: 0x060023BB RID: 9147 RVA: 0x0009C237 File Offset: 0x0009A437
		// (set) Token: 0x060023BC RID: 9148 RVA: 0x0009C23F File Offset: 0x0009A43F
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

		// Token: 0x170003B6 RID: 950
		// (get) Token: 0x060023BD RID: 9149 RVA: 0x0009C261 File Offset: 0x0009A461
		// (set) Token: 0x060023BE RID: 9150 RVA: 0x0009C269 File Offset: 0x0009A469
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

		// Token: 0x060023BF RID: 9151 RVA: 0x0009C2A4 File Offset: 0x0009A4A4
		private void OnUserSelectedUserProfile(UserProfile userProfile)
		{
			this.requestedUserProfile = userProfile;
		}

		// Token: 0x040021AF RID: 8623
		public TextMeshProUGUI nameLabel;

		// Token: 0x040021B0 RID: 8624
		public TextMeshProUGUI promptLabel;

		// Token: 0x040021B1 RID: 8625
		public Image cardImage;

		// Token: 0x040021B2 RID: 8626
		public Sprite playerCardNone;

		// Token: 0x040021B3 RID: 8627
		public Sprite playerCardKBM;

		// Token: 0x040021B4 RID: 8628
		public Sprite playerCardController;

		// Token: 0x040021B5 RID: 8629
		public Color unselectedColor;

		// Token: 0x040021B6 RID: 8630
		public Color selectedColor;

		// Token: 0x040021B7 RID: 8631
		private UserProfileListController userProfileSelectionList;

		// Token: 0x040021B8 RID: 8632
		public GameObject userProfileSelectionListPrefab;

		// Token: 0x040021B9 RID: 8633
		private Player _rewiredPlayer;

		// Token: 0x040021BA RID: 8634
		private UserProfile _requestedUserProfile;
	}
}
