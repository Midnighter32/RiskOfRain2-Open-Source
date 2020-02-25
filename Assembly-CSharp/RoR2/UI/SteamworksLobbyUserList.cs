using System;
using System.Collections.Generic;
using System.Globalization;
using Facepunch.Steamworks;
using RoR2.Networking;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200063A RID: 1594
	public class SteamworksLobbyUserList : MonoBehaviour
	{
		// Token: 0x170003DE RID: 990
		// (get) Token: 0x06002585 RID: 9605 RVA: 0x000A32F7 File Offset: 0x000A14F7
		private bool ValidLobbyExists
		{
			get
			{
				return Client.Instance.Lobby.LobbyType != Lobby.Type.Error;
			}
		}

		// Token: 0x06002586 RID: 9606 RVA: 0x000A3310 File Offset: 0x000A1510
		private void Update()
		{
			Client instance = Client.Instance;
			if (instance == null)
			{
				return;
			}
			if (!instance.Lobby.IsValid && this.elements.Count > 0)
			{
				this.Rebuild();
			}
			this.UpdateLobbyString();
		}

		// Token: 0x06002587 RID: 9607 RVA: 0x000A3350 File Offset: 0x000A1550
		private void Rebuild()
		{
			Client instance = Client.Instance;
			if (instance == null)
			{
				return;
			}
			bool validLobbyExists = this.ValidLobbyExists;
			ulong currentLobby = instance.Lobby.CurrentLobby;
			ulong[] memberIDs = instance.Lobby.GetMemberIDs();
			int num = validLobbyExists ? RoR2Application.maxPlayers : 0;
			this.copyLobbyIDToClipboardButton.SetActive(validLobbyExists);
			while (this.elements.Count > num)
			{
				int index = this.elements.Count - 1;
				UnityEngine.Object.Destroy(this.elements[index].gameObject);
				this.elements.RemoveAt(index);
			}
			while (this.elements.Count < num)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/SteamLobbyUserListElement"), base.transform);
				SocialUserIcon componentInChildren = gameObject.GetComponentInChildren<SocialUserIcon>();
				SteamUsernameLabel componentInChildren2 = gameObject.GetComponentInChildren<SteamUsernameLabel>();
				ChildLocator component = gameObject.GetComponent<ChildLocator>();
				this.elements.Add(new SteamworksLobbyUserList.Element
				{
					gameObject = gameObject,
					userIcon = componentInChildren,
					usernameLabel = componentInChildren2,
					elementChildLocator = component
				});
			}
			int i = 0;
			for (int j = 0; j < memberIDs.Length; j++)
			{
				int lobbyMemberPlayerCountByIndex = SteamworksLobbyManager.GetLobbyMemberPlayerCountByIndex(j);
				for (int k = 0; k < lobbyMemberPlayerCountByIndex; k++)
				{
					this.elements[i++].SetUser(memberIDs[j], k);
				}
			}
			while (i < num)
			{
				this.elements[i].SetUser(0UL, 0);
				i++;
			}
		}

		// Token: 0x06002588 RID: 9608 RVA: 0x000A34C4 File Offset: 0x000A16C4
		private void UpdateLobbyString()
		{
			if (this.lobbyStateText)
			{
				string text = "";
				switch (Client.Instance.Lobby.LobbyType)
				{
				case Lobby.Type.Private:
					text = Language.GetString("STEAM_LOBBY_PRIVATE");
					break;
				case Lobby.Type.FriendsOnly:
					text = Language.GetString("STEAM_LOBBY_FRIENDSONLY");
					break;
				case Lobby.Type.Public:
					text = Language.GetString("STEAM_LOBBY_PUBLIC");
					break;
				case Lobby.Type.Invisible:
					text = Language.GetString("STEAM_LOBBY_INVISIBLE");
					break;
				case Lobby.Type.Error:
					text = "";
					break;
				}
				this.lobbyStateText.text = text;
			}
		}

		// Token: 0x06002589 RID: 9609 RVA: 0x000A3558 File Offset: 0x000A1758
		private void UpdateUser(ulong userId)
		{
			for (int i = 0; i < this.elements.Count; i++)
			{
				if (this.elements[i].steamId == userId)
				{
					this.elements[i].Refresh();
				}
			}
		}

		// Token: 0x0600258A RID: 9610 RVA: 0x000A35A0 File Offset: 0x000A17A0
		private void OnEnable()
		{
			SteamworksLobbyManager.onLobbyDataUpdated += this.Rebuild;
			SteamworksLobbyManager.onLobbyStateChanged += this.OnLobbyStateChanged;
			SteamworksLobbyManager.onLobbyMemberDataUpdated += this.OnLobbyMemberDataUpdated;
			SteamworksLobbyManager.onPlayerCountUpdated += this.Rebuild;
			this.Rebuild();
		}

		// Token: 0x0600258B RID: 9611 RVA: 0x000A35F8 File Offset: 0x000A17F8
		private void OnDisable()
		{
			SteamworksLobbyManager.onLobbyDataUpdated -= this.Rebuild;
			SteamworksLobbyManager.onLobbyStateChanged -= this.OnLobbyStateChanged;
			SteamworksLobbyManager.onLobbyMemberDataUpdated -= this.OnLobbyMemberDataUpdated;
			SteamworksLobbyManager.onPlayerCountUpdated -= this.Rebuild;
		}

		// Token: 0x0600258C RID: 9612 RVA: 0x000A3649 File Offset: 0x000A1849
		private void OnLobbyStateChanged(Lobby.MemberStateChange memberStateChange, ulong initiatorUserId, ulong affectedUserId)
		{
			this.Rebuild();
		}

		// Token: 0x0600258D RID: 9613 RVA: 0x000A3651 File Offset: 0x000A1851
		private void OnLobbyMemberDataUpdated(ulong steamId)
		{
			this.UpdateUser(steamId);
		}

		// Token: 0x04002339 RID: 9017
		public TextMeshProUGUI lobbyStateText;

		// Token: 0x0400233A RID: 9018
		public GameObject copyLobbyIDToClipboardButton;

		// Token: 0x0400233B RID: 9019
		private List<SteamworksLobbyUserList.Element> elements = new List<SteamworksLobbyUserList.Element>();

		// Token: 0x0200063B RID: 1595
		private class Element
		{
			// Token: 0x0600258F RID: 9615 RVA: 0x000A366D File Offset: 0x000A186D
			public void SetUser(ulong steamId, int subPlayerIndex)
			{
				this.steamId = steamId;
				this.userIcon.SetFromSteamId(steamId);
				this.usernameLabel.userSteamId = steamId;
				this.usernameLabel.subPlayerIndex = subPlayerIndex;
				this.Refresh();
			}

			// Token: 0x06002590 RID: 9616 RVA: 0x000A36A0 File Offset: 0x000A18A0
			public void Refresh()
			{
				if (this.steamId == 0UL)
				{
					this.elementChildLocator.FindChild("UserIcon").gameObject.SetActive(false);
					this.elementChildLocator.FindChild("InviteButton").gameObject.SetActive(true);
				}
				else
				{
					this.elementChildLocator.FindChild("UserIcon").gameObject.SetActive(true);
					this.elementChildLocator.FindChild("InviteButton").gameObject.SetActive(false);
				}
				this.userIcon.Refresh();
				this.usernameLabel.Refresh();
				this.RefreshCrownAndPromoteButton();
			}

			// Token: 0x06002591 RID: 9617 RVA: 0x000A3740 File Offset: 0x000A1940
			private void RefreshCrownAndPromoteButton()
			{
				if (Client.Instance == null)
				{
					return;
				}
				bool flag = Client.Instance.Lobby.Owner == this.steamId && this.steamId > 0UL;
				if (this.lobbyLeaderCrown != flag)
				{
					if (flag)
					{
						this.lobbyLeaderCrown = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/LobbyLeaderCrown"), this.gameObject.transform);
					}
					else
					{
						UnityEngine.Object.Destroy(this.lobbyLeaderCrown);
						this.lobbyLeaderCrown = null;
					}
				}
				if (this.elementChildLocator)
				{
					bool flag2 = !flag && SteamworksLobbyManager.ownsLobby && this.steamId != 0UL && !SteamLobbyFinder.running && !NetworkSession.instance;
					GameObject gameObject = this.elementChildLocator.FindChild("PromoteButton").gameObject;
					if (gameObject)
					{
						gameObject.SetActive(flag2);
						if (flag2)
						{
							MPButton component = gameObject.GetComponent<MPButton>();
							if (component)
							{
								component.onClick.RemoveAllListeners();
								component.onClick.AddListener(delegate()
								{
									Console.instance.SubmitCmd(null, string.Format(CultureInfo.InvariantCulture, "steam_lobby_assign_owner {0}", TextSerialization.ToStringInvariant(this.steamId)), false);
								});
							}
						}
					}
				}
			}

			// Token: 0x0400233C RID: 9020
			public ulong steamId;

			// Token: 0x0400233D RID: 9021
			public GameObject gameObject;

			// Token: 0x0400233E RID: 9022
			public SocialUserIcon userIcon;

			// Token: 0x0400233F RID: 9023
			public SteamUsernameLabel usernameLabel;

			// Token: 0x04002340 RID: 9024
			public GameObject lobbyLeaderCrown;

			// Token: 0x04002341 RID: 9025
			public ChildLocator elementChildLocator;
		}
	}
}
