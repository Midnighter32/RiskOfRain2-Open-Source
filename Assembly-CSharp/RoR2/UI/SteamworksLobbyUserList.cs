using System;
using System.Collections.Generic;
using System.Globalization;
using Facepunch.Steamworks;
using RoR2.Networking;
using TMPro;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000645 RID: 1605
	public class SteamworksLobbyUserList : MonoBehaviour
	{
		// Token: 0x1700031F RID: 799
		// (get) Token: 0x060023E6 RID: 9190 RVA: 0x000A88E7 File Offset: 0x000A6AE7
		private bool ValidLobbyExists
		{
			get
			{
				return Client.Instance.Lobby.LobbyType != Lobby.Type.Error;
			}
		}

		// Token: 0x060023E7 RID: 9191 RVA: 0x000A8900 File Offset: 0x000A6B00
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

		// Token: 0x060023E8 RID: 9192 RVA: 0x000A8940 File Offset: 0x000A6B40
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

		// Token: 0x060023E9 RID: 9193 RVA: 0x000A8AB4 File Offset: 0x000A6CB4
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

		// Token: 0x060023EA RID: 9194 RVA: 0x000A8B48 File Offset: 0x000A6D48
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

		// Token: 0x060023EB RID: 9195 RVA: 0x000A8B90 File Offset: 0x000A6D90
		private void OnEnable()
		{
			SteamworksLobbyManager.onLobbyDataUpdated += this.Rebuild;
			SteamworksLobbyManager.onLobbyStateChanged += this.OnLobbyStateChanged;
			SteamworksLobbyManager.onLobbyMemberDataUpdated += this.OnLobbyMemberDataUpdated;
			SteamworksLobbyManager.onPlayerCountUpdated += this.Rebuild;
			this.Rebuild();
		}

		// Token: 0x060023EC RID: 9196 RVA: 0x000A8BE8 File Offset: 0x000A6DE8
		private void OnDisable()
		{
			SteamworksLobbyManager.onLobbyDataUpdated -= this.Rebuild;
			SteamworksLobbyManager.onLobbyStateChanged -= this.OnLobbyStateChanged;
			SteamworksLobbyManager.onLobbyMemberDataUpdated -= this.OnLobbyMemberDataUpdated;
			SteamworksLobbyManager.onPlayerCountUpdated -= this.Rebuild;
		}

		// Token: 0x060023ED RID: 9197 RVA: 0x000A8C39 File Offset: 0x000A6E39
		private void OnLobbyStateChanged(Lobby.MemberStateChange memberStateChange, ulong initiatorUserId, ulong affectedUserId)
		{
			this.Rebuild();
		}

		// Token: 0x060023EE RID: 9198 RVA: 0x000A8C41 File Offset: 0x000A6E41
		private void OnLobbyMemberDataUpdated(ulong steamId)
		{
			this.UpdateUser(steamId);
		}

		// Token: 0x040026D6 RID: 9942
		public TextMeshProUGUI lobbyStateText;

		// Token: 0x040026D7 RID: 9943
		public GameObject copyLobbyIDToClipboardButton;

		// Token: 0x040026D8 RID: 9944
		private List<SteamworksLobbyUserList.Element> elements = new List<SteamworksLobbyUserList.Element>();

		// Token: 0x02000646 RID: 1606
		private class Element
		{
			// Token: 0x060023F0 RID: 9200 RVA: 0x000A8C5D File Offset: 0x000A6E5D
			public void SetUser(ulong steamId, int subPlayerIndex)
			{
				this.steamId = steamId;
				this.userIcon.SetFromSteamId(steamId);
				this.usernameLabel.userSteamId = steamId;
				this.usernameLabel.subPlayerIndex = subPlayerIndex;
				this.Refresh();
			}

			// Token: 0x060023F1 RID: 9201 RVA: 0x000A8C90 File Offset: 0x000A6E90
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

			// Token: 0x060023F2 RID: 9202 RVA: 0x000A8D30 File Offset: 0x000A6F30
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

			// Token: 0x040026D9 RID: 9945
			public ulong steamId;

			// Token: 0x040026DA RID: 9946
			public GameObject gameObject;

			// Token: 0x040026DB RID: 9947
			public SocialUserIcon userIcon;

			// Token: 0x040026DC RID: 9948
			public SteamUsernameLabel usernameLabel;

			// Token: 0x040026DD RID: 9949
			public GameObject lobbyLeaderCrown;

			// Token: 0x040026DE RID: 9950
			public ChildLocator elementChildLocator;
		}
	}
}
