using System;
using System.Collections.Generic;
using System.Reflection;
using Facepunch.Steamworks;
using RoR2.Networking;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200049F RID: 1183
	public class SteamworksLobbyManager
	{
		// Token: 0x1700026E RID: 622
		// (get) Token: 0x06001A46 RID: 6726 RVA: 0x0007C779 File Offset: 0x0007A979
		private static Client client
		{
			get
			{
				return Client.Instance;
			}
		}

		// Token: 0x1700026F RID: 623
		// (get) Token: 0x06001A47 RID: 6727 RVA: 0x0007C780 File Offset: 0x0007A980
		// (set) Token: 0x06001A48 RID: 6728 RVA: 0x0007C787 File Offset: 0x0007A987
		public static bool isInLobby { get; private set; }

		// Token: 0x17000270 RID: 624
		// (get) Token: 0x06001A49 RID: 6729 RVA: 0x0007C78F File Offset: 0x0007A98F
		// (set) Token: 0x06001A4A RID: 6730 RVA: 0x0007C796 File Offset: 0x0007A996
		public static bool ownsLobby
		{
			get
			{
				return SteamworksLobbyManager._ownsLobby;
			}
			private set
			{
				if (value != SteamworksLobbyManager._ownsLobby)
				{
					SteamworksLobbyManager._ownsLobby = value;
					if (SteamworksLobbyManager._ownsLobby)
					{
						SteamworksLobbyManager.OnLobbyOwnershipGained();
						SteamworksLobbyManager.UpdatePlayerCount();
						return;
					}
					SteamworksLobbyManager.OnLobbyOwnershipLost();
				}
			}
		}

		// Token: 0x06001A4B RID: 6731 RVA: 0x0007C7BD File Offset: 0x0007A9BD
		private static void UpdateOwnsLobby()
		{
			SteamworksLobbyManager.ownsLobby = SteamworksLobbyManager.client.Lobby.IsOwner;
		}

		// Token: 0x17000271 RID: 625
		// (get) Token: 0x06001A4C RID: 6732 RVA: 0x0007C7D3 File Offset: 0x0007A9D3
		public static bool hasMinimumPlayerCount
		{
			get
			{
				return SteamworksLobbyManager.newestLobbyData.totalPlayerCount >= SteamworksLobbyManager.minimumPlayerCount;
			}
		}

		// Token: 0x17000272 RID: 626
		// (get) Token: 0x06001A4D RID: 6733 RVA: 0x0007C7E9 File Offset: 0x0007A9E9
		// (set) Token: 0x06001A4E RID: 6734 RVA: 0x0007C7F0 File Offset: 0x0007A9F0
		public static bool isFull { get; private set; }

		// Token: 0x17000273 RID: 627
		// (get) Token: 0x06001A4F RID: 6735 RVA: 0x0007C7F8 File Offset: 0x0007A9F8
		public static CSteamID serverId
		{
			get
			{
				return SteamworksLobbyManager.newestLobbyData.serverId;
			}
		}

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x06001A50 RID: 6736 RVA: 0x0007C804 File Offset: 0x0007AA04
		// (set) Token: 0x06001A51 RID: 6737 RVA: 0x0007C80B File Offset: 0x0007AA0B
		public static SteamworksLobbyManager.LobbyData newestLobbyData { get; private set; }

		// Token: 0x06001A52 RID: 6738 RVA: 0x0007C814 File Offset: 0x0007AA14
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			SteamworksLobbyManager.newestLobbyData = new SteamworksLobbyManager.LobbyData();
			LocalUserManager.onLocalUsersUpdated += SteamworksLobbyManager.UpdatePlayerCount;
			GameNetworkManager.onStopClientGlobal += delegate()
			{
				NetworkConnection connection = GameNetworkManager.singleton.client.connection;
				bool flag = Util.ConnectionIsLocal(connection);
				bool flag2;
				if (connection is SteamNetworkConnection)
				{
					flag2 = (((SteamNetworkConnection)connection).steamId == SteamworksLobbyManager.newestLobbyData.serverId);
				}
				else
				{
					flag2 = (connection.address == SteamworksLobbyManager.newestLobbyData.serverAddressPortPair.address);
				}
				if (flag && SteamworksLobbyManager.ownsLobby)
				{
					SteamworksLobbyManager.client.Lobby.CurrentLobbyData.RemoveData("server_id");
				}
				if (!flag && flag2)
				{
					SteamworksLobbyManager.client.Lobby.Leave();
				}
			};
			GameNetworkManager.onStartClientGlobal += delegate(NetworkClient networkClient)
			{
				if (SteamworksLobbyManager.ownsLobby)
				{
					SteamworksLobbyManager.client.Lobby.LobbyType = SteamworksLobbyManager.preferredLobbyType;
				}
			};
			SteamworksLobbyManager.onLobbyOwnershipGained += delegate()
			{
				SteamworksLobbyManager.SetStartingIfOwner(false);
			};
			GameNetworkManager.onStopClientGlobal += delegate()
			{
				SteamworksLobbyManager.SetStartingIfOwner(false);
			};
		}

		// Token: 0x06001A53 RID: 6739 RVA: 0x0007C8CC File Offset: 0x0007AACC
		public static void SetupCallbacks(Client client)
		{
			client.Lobby.OnChatMessageRecieved = new Action<ulong, byte[], int>(SteamworksLobbyManager.OnChatMessageReceived);
			client.Lobby.OnLobbyCreated = new Action<bool>(SteamworksLobbyManager.OnLobbyCreated);
			client.Lobby.OnLobbyDataUpdated = new Action(SteamworksLobbyManager.OnLobbyDataUpdated);
			client.Lobby.OnLobbyJoined = new Action<bool>(SteamworksLobbyManager.OnLobbyJoined);
			client.Lobby.OnLobbyMemberDataUpdated = new Action<ulong>(SteamworksLobbyManager.OnLobbyMemberDataUpdated);
			client.Lobby.OnLobbyStateChanged = new Action<Lobby.MemberStateChange, ulong, ulong>(SteamworksLobbyManager.OnLobbyStateChanged);
			client.Lobby.OnLobbyKicked = new Action<bool, ulong, ulong>(SteamworksLobbyManager.OnLobbyKicked);
			client.Lobby.OnLobbyLeave = new Action<ulong>(SteamworksLobbyManager.OnLobbyLeave);
			client.Lobby.OnUserInvitedToLobby = new Action<ulong, ulong>(SteamworksLobbyManager.OnUserInvitedToLobby);
			client.Lobby.OnLobbyJoinRequested = new Action<ulong>(SteamworksLobbyManager.OnLobbyJoinRequested);
			client.LobbyList.OnLobbiesUpdated = new Action(SteamworksLobbyManager.OnLobbiesUpdated);
			RoR2Application.onUpdate += SteamworksLobbyManager.StaticUpdate;
			SteamworksLobbyManager.SetStartingIfOwner(false);
		}

		// Token: 0x06001A54 RID: 6740 RVA: 0x0007C9ED File Offset: 0x0007ABED
		public static int GetLobbyMemberPlayerCountByIndex(int memberIndex)
		{
			if (memberIndex >= SteamworksLobbyManager.playerCountsList.Count)
			{
				return 0;
			}
			return SteamworksLobbyManager.playerCountsList[memberIndex];
		}

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x06001A55 RID: 6741 RVA: 0x0007CA09 File Offset: 0x0007AC09
		// (set) Token: 0x06001A56 RID: 6742 RVA: 0x0007CA10 File Offset: 0x0007AC10
		public static int calculatedTotalPlayerCount { get; private set; }

		// Token: 0x06001A57 RID: 6743 RVA: 0x0007CA18 File Offset: 0x0007AC18
		private static void UpdatePlayerCount()
		{
			if (SteamworksLobbyManager.client != null && SteamworksLobbyManager.client.Lobby.IsValid)
			{
				int count = LocalUserManager.readOnlyLocalUsersList.Count;
				string memberData = SteamworksLobbyManager.client.Lobby.GetMemberData(SteamworksLobbyManager.client.SteamId, "player_count");
				string text = TextSerialization.ToStringInvariant(Math.Max(1, count));
				if (memberData != text)
				{
					SteamworksLobbyManager.client.Lobby.SetMemberData("player_count", text);
				}
				SteamworksLobbyManager.playerCountsList.Clear();
				SteamworksLobbyManager.calculatedTotalPlayerCount = 0;
				ulong steamId = SteamworksLobbyManager.client.SteamId;
				int num = 0;
				foreach (ulong num2 in SteamworksLobbyManager.client.Lobby.GetMemberIDs())
				{
					int num3 = TextSerialization.TryParseInvariant(SteamworksLobbyManager.client.Lobby.GetMemberData(num2, "player_count"), out num3) ? Math.Min(Math.Max(1, num3), RoR2Application.maxLocalPlayers) : 1;
					if (num2 == steamId)
					{
						num3 = Math.Max(1, count);
					}
					SteamworksLobbyManager.playerCountsList.Add(num3);
					SteamworksLobbyManager.calculatedTotalPlayerCount += num3;
					if (num3 > 1)
					{
						num += num3 - 1;
					}
				}
				if (SteamworksLobbyManager.ownsLobby)
				{
					string data = SteamworksLobbyManager.client.Lobby.CurrentLobbyData.GetData("player_count");
					string b = TextSerialization.ToStringInvariant(SteamworksLobbyManager.calculatedTotalPlayerCount);
					if (data != b)
					{
						SteamworksLobbyManager.client.Lobby.CurrentLobbyData.SetData("player_count", b);
					}
					int maxMembers = SteamworksLobbyManager.client.Lobby.MaxMembers;
					int num4 = SteamworksLobbyManager.newestLobbyData.totalMaxPlayers - num;
					if (maxMembers != num4)
					{
						SteamworksLobbyManager.client.Lobby.MaxMembers = num4;
					}
				}
			}
			Action action = SteamworksLobbyManager.onPlayerCountUpdated;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06001A58 RID: 6744 RVA: 0x0007CBD9 File Offset: 0x0007ADD9
		[ConCommand(commandName = "steam_lobby_update_player_count", flags = ConVarFlags.None, helpText = "Forces a refresh of the steam lobby player count.")]
		private static void CCSteamLobbyUpdatePlayerCount(ConCommandArgs args)
		{
			SteamworksLobbyManager.UpdatePlayerCount();
		}

		// Token: 0x1400003C RID: 60
		// (add) Token: 0x06001A59 RID: 6745 RVA: 0x0007CBE0 File Offset: 0x0007ADE0
		// (remove) Token: 0x06001A5A RID: 6746 RVA: 0x0007CC14 File Offset: 0x0007AE14
		public static event Action onPlayerCountUpdated;

		// Token: 0x1400003D RID: 61
		// (add) Token: 0x06001A5B RID: 6747 RVA: 0x0007CC48 File Offset: 0x0007AE48
		// (remove) Token: 0x06001A5C RID: 6748 RVA: 0x0007CC7C File Offset: 0x0007AE7C
		public static event Action onLobbyChanged;

		// Token: 0x06001A5D RID: 6749 RVA: 0x0007CCB0 File Offset: 0x0007AEB0
		private static void OnLobbyChanged()
		{
			SteamworksLobbyManager.isInLobby = SteamworksLobbyManager.client.Lobby.IsValid;
			SteamworksLobbyManager.UpdateOwnsLobby();
			if (SteamworksLobbyManager.client.Lobby.CurrentLobbyData != null)
			{
				SteamworksLobbyManager.client.Lobby.CurrentLobbyData.SetData("total_max_players", TextSerialization.ToStringInvariant(RoR2Application.maxPlayers));
				SteamworksLobbyManager.client.Lobby.CurrentLobbyData.SetData("build_id", RoR2Application.GetBuildId());
			}
			SteamworksLobbyManager.UpdatePlayerCount();
			Action action = SteamworksLobbyManager.onLobbyChanged;
			if (action != null)
			{
				action();
			}
			SteamworksLobbyManager.OnLobbyDataUpdated();
		}

		// Token: 0x06001A5E RID: 6750 RVA: 0x0007CD44 File Offset: 0x0007AF44
		public static void CreateLobby()
		{
			if (SteamworksLobbyManager.client == null)
			{
				return;
			}
			SteamworksLobbyManager.client.Lobby.Leave();
			SteamworksLobbyManager.client.Lobby.Create(SteamworksLobbyManager.preferredLobbyType, RoR2Application.maxPlayers);
		}

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x06001A5F RID: 6751 RVA: 0x0007CD76 File Offset: 0x0007AF76
		// (set) Token: 0x06001A60 RID: 6752 RVA: 0x0007CD7D File Offset: 0x0007AF7D
		public static bool awaitingJoin { get; private set; }

		// Token: 0x06001A61 RID: 6753 RVA: 0x0007CD85 File Offset: 0x0007AF85
		public static void JoinLobby(CSteamID newLobbyId)
		{
			if (SteamworksLobbyManager.client == null)
			{
				return;
			}
			SteamworksLobbyManager.awaitingJoin = true;
			SteamworksLobbyManager.client.Lobby.Join(newLobbyId.value);
		}

		// Token: 0x06001A62 RID: 6754 RVA: 0x0007CDAA File Offset: 0x0007AFAA
		public static void LeaveLobby()
		{
			Client client = SteamworksLobbyManager.client;
			if (client == null)
			{
				return;
			}
			client.Lobby.Leave();
		}

		// Token: 0x06001A63 RID: 6755 RVA: 0x0007CDC0 File Offset: 0x0007AFC0
		private static void Update()
		{
			if (SteamworksLobbyManager.client == null)
			{
				return;
			}
			if (SteamworksLobbyManager.startingFadeSet != (SteamworksLobbyManager.newestLobbyData.starting && !ClientScene.ready))
			{
				if (SteamworksLobbyManager.startingFadeSet)
				{
					FadeToBlackManager.fadeCount--;
				}
				else
				{
					FadeToBlackManager.fadeCount++;
				}
				SteamworksLobbyManager.startingFadeSet = !SteamworksLobbyManager.startingFadeSet;
			}
		}

		// Token: 0x06001A64 RID: 6756 RVA: 0x0007CE21 File Offset: 0x0007B021
		private static void StaticUpdate()
		{
			if (SteamworksLobbyManager.client == null)
			{
				return;
			}
			SteamworksLobbyManager.UpdateOwnsLobby();
		}

		// Token: 0x06001A65 RID: 6757 RVA: 0x0007CE30 File Offset: 0x0007B030
		[ConCommand(commandName = "steam_lobby_create")]
		private static void CCSteamLobbyCreate(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			SteamworksLobbyManager.CreateLobby();
		}

		// Token: 0x06001A66 RID: 6758 RVA: 0x0007CE3C File Offset: 0x0007B03C
		[ConCommand(commandName = "steam_lobby_create_if_none")]
		private static void CCSteamLobbyCreateIfNone(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			if (!SteamworksLobbyManager.client.Lobby.IsValid)
			{
				SteamworksLobbyManager.CreateLobby();
			}
		}

		// Token: 0x06001A67 RID: 6759 RVA: 0x0007CE59 File Offset: 0x0007B059
		[ConCommand(commandName = "steam_lobby_find")]
		private static void CCSteamLobbyFind(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			SteamworksLobbyManager.awaitingLobbyListUpdate = true;
		}

		// Token: 0x06001A68 RID: 6760 RVA: 0x0007CE68 File Offset: 0x0007B068
		[ConCommand(commandName = "steam_lobby_join")]
		private static void CCSteamLobbyJoin(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			args.CheckArgumentCount(1);
			Debug.LogFormat("Joining lobby {0}...", new object[]
			{
				args[0]
			});
			CSteamID newLobbyId;
			if (CSteamID.TryParse(args[0], out newLobbyId))
			{
				SteamworksLobbyManager.JoinLobby(newLobbyId);
				return;
			}
			throw new ConCommandException("Could not parse lobby id.");
		}

		// Token: 0x06001A69 RID: 6761 RVA: 0x0007CEBF File Offset: 0x0007B0BF
		[ConCommand(commandName = "steam_lobby_leave")]
		private static void CCSteamLobbyLeave(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			SteamworksLobbyManager.LeaveLobby();
		}

		// Token: 0x06001A6A RID: 6762 RVA: 0x0007CECC File Offset: 0x0007B0CC
		[ConCommand(commandName = "steam_lobby_assign_owner")]
		private static void CCSteamLobbyAssignOwner(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			args.CheckArgumentCount(1);
			Debug.LogFormat("Promoting {0} to lobby leader...", new object[]
			{
				args[0]
			});
			CSteamID csteamID;
			if (CSteamID.TryParse(args[0], out csteamID))
			{
				SteamworksLobbyManager.client.Lobby.Owner = csteamID.value;
				return;
			}
			throw new ConCommandException("Could not parse steamID.");
		}

		// Token: 0x06001A6B RID: 6763 RVA: 0x0007CF34 File Offset: 0x0007B134
		[ConCommand(commandName = "steam_lobby_invite", flags = ConVarFlags.None, helpText = "Invites the player with the specified steam id to the current lobby.")]
		private static void CCSteamLobbyInvite(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			args.CheckArgumentCount(1);
			CSteamID csteamID;
			if (CSteamID.TryParse(args[0], out csteamID))
			{
				SteamworksLobbyManager.client.Lobby.InviteUserToLobby(csteamID.value);
				return;
			}
			throw new ConCommandException("Could not parse user id.");
		}

		// Token: 0x06001A6C RID: 6764 RVA: 0x0007CF80 File Offset: 0x0007B180
		[ConCommand(commandName = "steam_lobby_open_invite_overlay", flags = ConVarFlags.None, helpText = "Opens the steam overlay to the friend invite dialog.")]
		private static void CCSteamLobbyOpenInviteOverlay(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			SteamworksLobbyManager.client.Overlay.OpenInviteDialog(SteamworksLobbyManager.client.Lobby.CurrentLobby);
		}

		// Token: 0x06001A6D RID: 6765 RVA: 0x0007CFA5 File Offset: 0x0007B1A5
		[ConCommand(commandName = "steam_lobby_copy_to_clipboard", flags = ConVarFlags.None, helpText = "Copies the currently active lobby to the clipboard if applicable.")]
		private static void CCSteamLobbyCopyToClipboard(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			GUIUtility.systemCopyBuffer = TextSerialization.ToStringInvariant(SteamworksLobbyManager.client.Lobby.CurrentLobby);
			Chat.AddMessage(Language.GetString("STEAM_COPY_LOBBY_TO_CLIPBOARD_MESSAGE"));
		}

		// Token: 0x06001A6E RID: 6766 RVA: 0x0007CFD4 File Offset: 0x0007B1D4
		[ConCommand(commandName = "steam_lobby_print_data", flags = ConVarFlags.None, helpText = "Prints all data about the current steam lobby.")]
		private static void CCSteamLobbyPrintData(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			if (SteamworksLobbyManager.client.Lobby.IsValid)
			{
				List<string> list = new List<string>();
				foreach (KeyValuePair<string, string> keyValuePair in SteamworksLobbyManager.client.Lobby.CurrentLobbyData.GetAllData())
				{
					list.Add(string.Format("\"{0}\" = \"{1}\"", keyValuePair.Key, keyValuePair.Value));
				}
				Debug.Log(string.Join("\n", list.ToArray()));
			}
		}

		// Token: 0x06001A6F RID: 6767 RVA: 0x0007D080 File Offset: 0x0007B280
		[ConCommand(commandName = "steam_id", flags = ConVarFlags.None, helpText = "Displays your steam id.")]
		private static void CCSteamId(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			Debug.LogFormat("Steam id = {0}", new object[]
			{
				SteamworksLobbyManager.client.SteamId
			});
		}

		// Token: 0x06001A70 RID: 6768 RVA: 0x0007D0A9 File Offset: 0x0007B2A9
		[ConCommand(commandName = "steam_lobby_id", flags = ConVarFlags.None, helpText = "Displays the steam id of the current lobby.")]
		private static void CCSteamLobbyId(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			Debug.LogFormat("Lobby id = {0}", new object[]
			{
				SteamworksLobbyManager.client.Lobby.CurrentLobby
			});
		}

		// Token: 0x06001A71 RID: 6769 RVA: 0x0007D0D8 File Offset: 0x0007B2D8
		[ConCommand(commandName = "steam_lobby_print_members", flags = ConVarFlags.None, helpText = "Displays the members current lobby.")]
		private static void CCSteamLobbyPrintMembers(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			ulong[] memberIDs = SteamworksLobbyManager.client.Lobby.GetMemberIDs();
			string[] array = new string[memberIDs.Length];
			for (int i = 0; i < memberIDs.Length; i++)
			{
				array[i] = string.Format("[{0}]{1} id={2} name={3}", new object[]
				{
					i,
					(SteamworksLobbyManager.client.Lobby.Owner == memberIDs[i]) ? "*" : " ",
					memberIDs[i],
					SteamworksLobbyManager.client.Friends.GetName(memberIDs[i])
				});
			}
			Debug.Log(string.Join("\n", array));
		}

		// Token: 0x06001A72 RID: 6770 RVA: 0x0007D180 File Offset: 0x0007B380
		[ConCommand(commandName = "steam_lobby_print_list", flags = ConVarFlags.None, helpText = "Displays a list of lobbies from the last search.")]
		private static void CCSteamLobbyPrintList(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			List<LobbyList.Lobby> lobbies = SteamworksLobbyManager.client.LobbyList.Lobbies;
			string[] array = new string[lobbies.Count];
			for (int i = 0; i < lobbies.Count; i++)
			{
				array[i] = string.Format("[{0}] id={1}\n      players={2}/{3},\n      owner=\"{4}\"", new object[]
				{
					i,
					lobbies[i].LobbyID,
					lobbies[i].NumMembers,
					lobbies[i].MemberLimit,
					SteamworksLobbyManager.client.Friends.GetName(lobbies[i].Owner)
				});
			}
			Debug.Log(string.Join("\n", array));
		}

		// Token: 0x06001A73 RID: 6771 RVA: 0x0007D248 File Offset: 0x0007B448
		public static void ForceLobbyDataUpdate()
		{
			Client client = SteamworksLobbyManager.client;
			Lobby lobby = (client != null) ? client.Lobby : null;
			if (lobby != null)
			{
				SteamworksLobbyManager.updateLobbyDataMethodInfo.Invoke(lobby, Array.Empty<object>());
			}
		}

		// Token: 0x06001A74 RID: 6772 RVA: 0x0007D27C File Offset: 0x0007B47C
		private static void OnChatMessageReceived(ulong senderId, byte[] buffer, int byteCount)
		{
			NetworkReader networkReader = new NetworkReader(buffer);
			if (byteCount >= 1 && networkReader.ReadByte() == 0)
			{
				Chat.AddMessage(string.Format("{0}: {1}", Client.Instance.Friends.Get(senderId), networkReader.ReadString()));
			}
		}

		// Token: 0x06001A75 RID: 6773 RVA: 0x0007D2C3 File Offset: 0x0007B4C3
		public static void JoinOrStartMigrate(CSteamID newLobbyId)
		{
			if (SteamworksLobbyManager.ownsLobby)
			{
				SteamworksLobbyManager.StartMigrateLobby(newLobbyId);
				return;
			}
			SteamworksLobbyManager.client.Lobby.Leave();
			SteamworksLobbyManager.JoinLobby(newLobbyId);
		}

		// Token: 0x06001A76 RID: 6774 RVA: 0x0007D2E8 File Offset: 0x0007B4E8
		public static void StartMigrateLobby(CSteamID newLobbyId)
		{
			SteamworksLobbyManager.client.Lobby.Joinable = false;
			SteamworksLobbyManager.client.Lobby.CurrentLobbyData.SetData("migration_id", TextSerialization.ToStringInvariant(newLobbyId.value));
		}

		// Token: 0x06001A77 RID: 6775 RVA: 0x0007D31F File Offset: 0x0007B51F
		private static void OnLobbyCreated(bool success)
		{
			if (success)
			{
				Debug.LogFormat("Steamworks lobby creation succeeded. Lobby id = {0}", new object[]
				{
					SteamworksLobbyManager.client.Lobby.CurrentLobby
				});
				SteamworksLobbyManager.OnLobbyChanged();
				return;
			}
			Debug.Log("Steamworks lobby creation failed.");
		}

		// Token: 0x1400003E RID: 62
		// (add) Token: 0x06001A78 RID: 6776 RVA: 0x0007D35C File Offset: 0x0007B55C
		// (remove) Token: 0x06001A79 RID: 6777 RVA: 0x0007D390 File Offset: 0x0007B590
		public static event Action onLobbyDataUpdated;

		// Token: 0x06001A7A RID: 6778 RVA: 0x0007D3C4 File Offset: 0x0007B5C4
		private static void OnLobbyDataUpdated()
		{
			Lobby lobby = SteamworksLobbyManager.client.Lobby;
			SteamworksLobbyManager.newestLobbyData = (lobby.IsValid ? new SteamworksLobbyManager.LobbyData(lobby.CurrentLobbyData) : new SteamworksLobbyManager.LobbyData());
			SteamworksLobbyManager.UpdateOwnsLobby();
			SteamworksLobbyManager.UpdatePlayerCount();
			if (lobby.IsValid && !SteamworksLobbyManager.ownsLobby)
			{
				if (SteamworksLobbyManager.newestLobbyData.serverId.isValid)
				{
					if (!GameNetworkManager.singleton.IsConnectedToServer(SteamworksLobbyManager.newestLobbyData.serverId) && RoR2Application.GetBuildId() == SteamworksLobbyManager.newestLobbyData.buildId)
					{
						GameNetworkManager.singleton.desiredHost = new GameNetworkManager.HostDescription(SteamworksLobbyManager.newestLobbyData.serverId);
					}
				}
				else
				{
					GameNetworkManager.singleton.desiredHost = GameNetworkManager.HostDescription.none;
				}
			}
			Action action = SteamworksLobbyManager.onLobbyDataUpdated;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x1400003F RID: 63
		// (add) Token: 0x06001A7B RID: 6779 RVA: 0x0007D490 File Offset: 0x0007B690
		// (remove) Token: 0x06001A7C RID: 6780 RVA: 0x0007D4C4 File Offset: 0x0007B6C4
		public static event Action<bool> onLobbyJoined;

		// Token: 0x06001A7D RID: 6781 RVA: 0x0007D4F8 File Offset: 0x0007B6F8
		private static void OnLobbyJoined(bool success)
		{
			SteamworksLobbyManager.awaitingJoin = false;
			if (success)
			{
				if (SteamworksLobbyManager.client.Lobby.CurrentLobbyData != null)
				{
					string buildId = RoR2Application.GetBuildId();
					string data = SteamworksLobbyManager.client.Lobby.CurrentLobbyData.GetData("build_id");
					if (buildId != data)
					{
						Debug.LogFormat("Lobby build_id mismatch, leaving lobby. Ours=\"{0}\" Theirs=\"{1}\"", new object[]
						{
							buildId,
							data
						});
						SimpleDialogBox simpleDialogBox = SimpleDialogBox.Create(null);
						simpleDialogBox.AddCancelButton(CommonLanguageTokens.ok, Array.Empty<object>());
						simpleDialogBox.headerToken = new SimpleDialogBox.TokenParamsPair
						{
							token = "STEAM_LOBBY_VERSION_MISMATCH_DIALOG_TITLE",
							formatParams = Array.Empty<object>()
						};
						SimpleDialogBox.TokenParamsPair descriptionToken = default(SimpleDialogBox.TokenParamsPair);
						descriptionToken.token = "STEAM_LOBBY_VERSION_MISMATCH_DIALOG_DESCRIPTION";
						object[] formatParams = new string[]
						{
							buildId,
							data
						};
						descriptionToken.formatParams = formatParams;
						simpleDialogBox.descriptionToken = descriptionToken;
						SteamworksLobbyManager.client.Lobby.Leave();
						return;
					}
				}
				Debug.LogFormat("Steamworks lobby join succeeded. Lobby id = {0}", new object[]
				{
					SteamworksLobbyManager.client.Lobby.CurrentLobby
				});
				SteamworksLobbyManager.OnLobbyChanged();
			}
			else
			{
				Debug.Log("Steamworks lobby join failed.");
				Console.instance.SubmitCmd(null, "steam_lobby_create_if_none", true);
			}
			Action<bool> action = SteamworksLobbyManager.onLobbyJoined;
			if (action == null)
			{
				return;
			}
			action(success);
		}

		// Token: 0x14000040 RID: 64
		// (add) Token: 0x06001A7E RID: 6782 RVA: 0x0007D644 File Offset: 0x0007B844
		// (remove) Token: 0x06001A7F RID: 6783 RVA: 0x0007D678 File Offset: 0x0007B878
		public static event Action<ulong> onLobbyLeave;

		// Token: 0x06001A80 RID: 6784 RVA: 0x0007D6AB File Offset: 0x0007B8AB
		private static void OnLobbyLeave(ulong lobbyId)
		{
			Debug.LogFormat("Left lobby {0}.", new object[]
			{
				lobbyId
			});
			Action<ulong> action = SteamworksLobbyManager.onLobbyLeave;
			if (action != null)
			{
				action(lobbyId);
			}
			SteamworksLobbyManager.OnLobbyChanged();
		}

		// Token: 0x06001A81 RID: 6785 RVA: 0x0007D6DC File Offset: 0x0007B8DC
		private static void OnLobbyKicked(bool kickedDueToDisconnect, ulong lobbyId, ulong adminId)
		{
			Debug.LogFormat("Kicked from lobby. kickedDueToDisconnect={0} lobbyId={1} adminId={2}", new object[]
			{
				kickedDueToDisconnect,
				lobbyId,
				adminId
			});
			SteamworksLobbyManager.OnLobbyChanged();
		}

		// Token: 0x14000041 RID: 65
		// (add) Token: 0x06001A82 RID: 6786 RVA: 0x0007D710 File Offset: 0x0007B910
		// (remove) Token: 0x06001A83 RID: 6787 RVA: 0x0007D744 File Offset: 0x0007B944
		public static event Action<ulong> onLobbyMemberDataUpdated;

		// Token: 0x06001A84 RID: 6788 RVA: 0x0007D777 File Offset: 0x0007B977
		private static void OnLobbyMemberDataUpdated(ulong memberId)
		{
			SteamworksLobbyManager.UpdateOwnsLobby();
			Action<ulong> action = SteamworksLobbyManager.onLobbyMemberDataUpdated;
			if (action == null)
			{
				return;
			}
			action(memberId);
		}

		// Token: 0x14000042 RID: 66
		// (add) Token: 0x06001A85 RID: 6789 RVA: 0x0007D790 File Offset: 0x0007B990
		// (remove) Token: 0x06001A86 RID: 6790 RVA: 0x0007D7C4 File Offset: 0x0007B9C4
		public static event Action<Lobby.MemberStateChange, ulong, ulong> onLobbyStateChanged;

		// Token: 0x06001A87 RID: 6791 RVA: 0x0007D7F8 File Offset: 0x0007B9F8
		private static void OnLobbyStateChanged(Lobby.MemberStateChange memberStateChange, ulong initiatorUserId, ulong affectedUserId)
		{
			Debug.LogFormat("OnLobbyStateChanged memberStateChange={0} initiatorUserId={1} affectedUserId={2}", new object[]
			{
				memberStateChange,
				initiatorUserId,
				affectedUserId
			});
			SteamworksLobbyManager.OnLobbyChanged();
			Action<Lobby.MemberStateChange, ulong, ulong> action = SteamworksLobbyManager.onLobbyStateChanged;
			if (action == null)
			{
				return;
			}
			action(memberStateChange, initiatorUserId, affectedUserId);
		}

		// Token: 0x06001A88 RID: 6792 RVA: 0x0007D847 File Offset: 0x0007BA47
		private static void OnLobbyJoinRequested(ulong lobbyId)
		{
			Debug.LogFormat("Request to join lobby {0} received. Attempting to join lobby.", new object[]
			{
				lobbyId
			});
			SteamworksLobbyManager.JoinLobby(new CSteamID(lobbyId));
		}

		// Token: 0x06001A89 RID: 6793 RVA: 0x0007D86D File Offset: 0x0007BA6D
		private static void OnUserInvitedToLobby(ulong lobbyId, ulong senderId)
		{
			Debug.LogFormat("Received invitation to lobby {0} from sender {1}.", new object[]
			{
				lobbyId,
				senderId
			});
		}

		// Token: 0x06001A8A RID: 6794 RVA: 0x0007D894 File Offset: 0x0007BA94
		[ConCommand(commandName = "dump_lobbies", flags = ConVarFlags.None, helpText = "")]
		private static void DumpLobbies(ConCommandArgs args)
		{
			LobbyList.Filter filter = new LobbyList.Filter();
			filter.MaxResults = new int?(50);
			filter.DistanceFilter = LobbyList.Filter.Distance.Worldwide;
			SteamworksLobbyManager.client.LobbyList.Refresh(filter);
		}

		// Token: 0x06001A8B RID: 6795 RVA: 0x0007D8CC File Offset: 0x0007BACC
		private static void OnLobbiesUpdated()
		{
			if (SteamworksLobbyManager.awaitingLobbyListUpdate)
			{
				SteamworksLobbyManager.awaitingLobbyListUpdate = false;
				List<LobbyList.Lobby> lobbies = SteamworksLobbyManager.client.LobbyList.Lobbies;
				Debug.LogFormat("Found {0} lobbies.", new object[]
				{
					lobbies.Count
				});
				if (lobbies.Count > 0)
				{
					Console.instance.SubmitCmd(null, string.Format("steam_lobby_join {0}", lobbies[0].LobbyID), false);
				}
			}
			Action action = SteamworksLobbyManager.onLobbiesUpdated;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06001A8C RID: 6796 RVA: 0x0007D954 File Offset: 0x0007BB54
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void OnStartup()
		{
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for (int i = 0; i < commandLineArgs.Length - 1; i++)
			{
				ulong num;
				if (commandLineArgs[i].ToLower() == "+connect_lobby" && TextSerialization.TryParseInvariant(commandLineArgs[i + 1], out num))
				{
					SteamworksLobbyManager.pendingSteamworksLobbyId = num;
				}
			}
			RoR2Application.onStart = (Action)Delegate.Combine(RoR2Application.onStart, new Action(SteamworksLobbyManager.CheckStartupLobby));
		}

		// Token: 0x06001A8D RID: 6797 RVA: 0x0007D9BF File Offset: 0x0007BBBF
		private static void CheckStartupLobby()
		{
			if (SteamworksLobbyManager.pendingSteamworksLobbyId != 0UL)
			{
				Console.instance.SubmitCmd(null, string.Format("steam_lobby_join {0}", SteamworksLobbyManager.pendingSteamworksLobbyId), false);
				SteamworksLobbyManager.pendingSteamworksLobbyId = 0UL;
			}
		}

		// Token: 0x06001A8E RID: 6798 RVA: 0x0007D9EF File Offset: 0x0007BBEF
		public static void OnServerIPDiscovered(string address, ushort port)
		{
			if (SteamworksLobbyManager.client == null)
			{
				return;
			}
			if (SteamworksLobbyManager.client.Lobby.IsValid)
			{
				ulong owner = SteamworksLobbyManager.client.Lobby.Owner;
				ulong steamId = SteamworksLobbyManager.client.SteamId;
			}
		}

		// Token: 0x06001A8F RID: 6799 RVA: 0x0007DA28 File Offset: 0x0007BC28
		public static void OnServerSteamIDDiscovered(CSteamID serverId)
		{
			if (SteamworksLobbyManager.client == null)
			{
				return;
			}
			if (SteamworksLobbyManager.client.Lobby.IsValid && SteamworksLobbyManager.client.Lobby.Owner == SteamworksLobbyManager.client.SteamId)
			{
				SteamworksLobbyManager.client.Lobby.CurrentLobbyData.SetData("server_id", serverId.ToString());
			}
		}

		// Token: 0x06001A90 RID: 6800 RVA: 0x0007DA90 File Offset: 0x0007BC90
		public static void SendMigrationMessage(CSteamID newLobbyId)
		{
			if (SteamworksLobbyManager.client != null)
			{
				SteamworksLobbyManager.client.Lobby.SendChatMessage("");
				SteamworksLobbyManager.JoinLobby(newLobbyId);
			}
		}

		// Token: 0x14000043 RID: 67
		// (add) Token: 0x06001A91 RID: 6801 RVA: 0x0007DAB4 File Offset: 0x0007BCB4
		// (remove) Token: 0x06001A92 RID: 6802 RVA: 0x0007DAE8 File Offset: 0x0007BCE8
		public static event Action onLobbiesUpdated;

		// Token: 0x06001A93 RID: 6803 RVA: 0x0007DB1C File Offset: 0x0007BD1C
		public static void SetLobbyQuickPlayQueuedIfOwner(bool quickplayQueuedState)
		{
			Lobby lobby = SteamworksLobbyManager.client.Lobby;
			if (((lobby != null) ? lobby.CurrentLobbyData : null) == null)
			{
				return;
			}
			lobby.CurrentLobbyData.SetData("qp", quickplayQueuedState ? "1" : "0");
			lobby.CurrentLobbyData.SetData("v", TextSerialization.ToStringInvariant(SteamworksLobbyManager.v++));
			if (!quickplayQueuedState)
			{
				lobby.LobbyType = SteamworksLobbyManager.preferredLobbyType;
			}
		}

		// Token: 0x06001A94 RID: 6804 RVA: 0x0007DB94 File Offset: 0x0007BD94
		public static void SetLobbyQuickPlayCutoffTimeIfOwner(uint? timestamp)
		{
			Lobby lobby = SteamworksLobbyManager.client.Lobby;
			if (((lobby != null) ? lobby.CurrentLobbyData : null) == null)
			{
				return;
			}
			if (timestamp == null)
			{
				lobby.CurrentLobbyData.RemoveData("qp_cutoff_time");
				return;
			}
			string text = TextSerialization.ToStringInvariant(timestamp.Value);
			lobby.CurrentLobbyData.SetData("qp_cutoff_time", text);
		}

		// Token: 0x06001A95 RID: 6805 RVA: 0x0007DBF8 File Offset: 0x0007BDF8
		public static void SetStartingIfOwner(bool startingState)
		{
			Lobby lobby = SteamworksLobbyManager.client.Lobby;
			if (((lobby != null) ? lobby.CurrentLobbyData : null) == null)
			{
				return;
			}
			Lobby.LobbyData currentLobbyData = lobby.CurrentLobbyData;
			if (currentLobbyData == null)
			{
				return;
			}
			currentLobbyData.SetData("starting", startingState ? "1" : "0");
		}

		// Token: 0x06001A96 RID: 6806 RVA: 0x0007DC44 File Offset: 0x0007BE44
		private static void OnLobbyOwnershipGained()
		{
			Action action = SteamworksLobbyManager.onLobbyOwnershipGained;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06001A97 RID: 6807 RVA: 0x0007DC55 File Offset: 0x0007BE55
		private static void OnLobbyOwnershipLost()
		{
			Action action = SteamworksLobbyManager.onLobbyOwnershipLost;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x14000044 RID: 68
		// (add) Token: 0x06001A98 RID: 6808 RVA: 0x0007DC68 File Offset: 0x0007BE68
		// (remove) Token: 0x06001A99 RID: 6809 RVA: 0x0007DC9C File Offset: 0x0007BE9C
		public static event Action onLobbyOwnershipGained;

		// Token: 0x14000045 RID: 69
		// (add) Token: 0x06001A9A RID: 6810 RVA: 0x0007DCD0 File Offset: 0x0007BED0
		// (remove) Token: 0x06001A9B RID: 6811 RVA: 0x0007DD04 File Offset: 0x0007BF04
		public static event Action onLobbyOwnershipLost;

		// Token: 0x04001D6A RID: 7530
		public static Lobby.Type preferredLobbyType = Lobby.Type.FriendsOnly;

		// Token: 0x04001D6B RID: 7531
		public static ulong pendingSteamworksLobbyId;

		// Token: 0x04001D6D RID: 7533
		private static bool _ownsLobby;

		// Token: 0x04001D6E RID: 7534
		private static int minimumPlayerCount = 2;

		// Token: 0x04001D70 RID: 7536
		public const string mdV = "v";

		// Token: 0x04001D71 RID: 7537
		public const string mdAppId = "appid";

		// Token: 0x04001D72 RID: 7538
		public const string mdTotalMaxPlayers = "total_max_players";

		// Token: 0x04001D73 RID: 7539
		public const string mdPlayerCount = "player_count";

		// Token: 0x04001D74 RID: 7540
		public const string mdQuickplayQueued = "qp";

		// Token: 0x04001D75 RID: 7541
		public const string mdQuickplayCutoffTime = "qp_cutoff_time";

		// Token: 0x04001D76 RID: 7542
		public const string mdServerId = "server_id";

		// Token: 0x04001D77 RID: 7543
		public const string mdServerAddress = "server_address";

		// Token: 0x04001D78 RID: 7544
		public const string mdMigrationId = "migration_id";

		// Token: 0x04001D79 RID: 7545
		public const string mdStarting = "starting";

		// Token: 0x04001D7A RID: 7546
		public const string mdBuildId = "build_id";

		// Token: 0x04001D7C RID: 7548
		private static readonly List<int> playerCountsList = new List<int>();

		// Token: 0x04001D81 RID: 7553
		private static bool startingFadeSet = false;

		// Token: 0x04001D82 RID: 7554
		private static readonly MethodInfo updateLobbyDataMethodInfo = typeof(Lobby).GetMethod("UpdateLobbyData", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04001D88 RID: 7560
		private static bool awaitingLobbyListUpdate = false;

		// Token: 0x04001D8A RID: 7562
		private static int v = 0;

		// Token: 0x020004A0 RID: 1184
		public class LobbyData
		{
			// Token: 0x06001A9E RID: 6814 RVA: 0x0007DD88 File Offset: 0x0007BF88
			public LobbyData()
			{
			}

			// Token: 0x06001A9F RID: 6815 RVA: 0x0007DDA8 File Offset: 0x0007BFA8
			public LobbyData(Lobby.LobbyData lobbyData)
			{
				SteamworksLobbyManager.LobbyData.<>c__DisplayClass11_0 CS$<>8__locals1;
				CS$<>8__locals1.lobbyDataDictionary = lobbyData.GetAllData();
				SteamworksLobbyManager.LobbyData.<.ctor>g__ReadInt|11_2("total_max_players", ref this.totalMaxPlayers, ref CS$<>8__locals1);
				SteamworksLobbyManager.LobbyData.<.ctor>g__ReadInt|11_2("player_count", ref this.totalPlayerCount, ref CS$<>8__locals1);
				SteamworksLobbyManager.LobbyData.<.ctor>g__ReadBool|11_1("qp", ref this.quickplayQueued, ref CS$<>8__locals1);
				SteamworksLobbyManager.LobbyData.<.ctor>g__ReadCSteamID|11_3("server_id", ref this.serverId, ref CS$<>8__locals1);
				SteamworksLobbyManager.LobbyData.<.ctor>g__ReadAddressPortPair|11_4("server_address", ref this.serverAddressPortPair, ref CS$<>8__locals1);
				SteamworksLobbyManager.LobbyData.<.ctor>g__ReadCSteamID|11_3("migration_id", ref this.migrationId, ref CS$<>8__locals1);
				SteamworksLobbyManager.LobbyData.<.ctor>g__ReadBool|11_1("starting", ref this.starting, ref CS$<>8__locals1);
				SteamworksLobbyManager.LobbyData.<.ctor>g__ReadString|11_0("build_id", ref this.buildId, ref CS$<>8__locals1);
				SteamworksLobbyManager.LobbyData.<.ctor>g__ReadNullableDate|11_5("qp_cutoff_time", out this.quickplayCutoffTime, ref CS$<>8__locals1);
				this.shouldConnect = (this.serverId.isValid || this.serverAddressPortPair.isValid);
			}

			// Token: 0x04001D8D RID: 7565
			public readonly int totalMaxPlayers = RoR2Application.maxPlayers;

			// Token: 0x04001D8E RID: 7566
			public readonly int totalPlayerCount;

			// Token: 0x04001D8F RID: 7567
			public readonly bool quickplayQueued;

			// Token: 0x04001D90 RID: 7568
			public readonly CSteamID serverId;

			// Token: 0x04001D91 RID: 7569
			public readonly AddressPortPair serverAddressPortPair;

			// Token: 0x04001D92 RID: 7570
			public readonly CSteamID migrationId;

			// Token: 0x04001D93 RID: 7571
			public readonly bool starting;

			// Token: 0x04001D94 RID: 7572
			public readonly string buildId = "0";

			// Token: 0x04001D95 RID: 7573
			public readonly DateTime? quickplayCutoffTime;

			// Token: 0x04001D96 RID: 7574
			public readonly bool shouldConnect;
		}

		// Token: 0x020004A2 RID: 1186
		private enum LobbyMessageType : byte
		{
			// Token: 0x04001D99 RID: 7577
			Chat
		}
	}
}
