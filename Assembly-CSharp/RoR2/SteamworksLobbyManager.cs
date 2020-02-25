using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using Facepunch.Steamworks;
using RoR2.Networking;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000430 RID: 1072
	public class SteamworksLobbyManager
	{
		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x060019A9 RID: 6569 RVA: 0x0006E17A File Offset: 0x0006C37A
		private static Client client
		{
			get
			{
				return Client.Instance;
			}
		}

		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x060019AA RID: 6570 RVA: 0x0006E181 File Offset: 0x0006C381
		// (set) Token: 0x060019AB RID: 6571 RVA: 0x0006E188 File Offset: 0x0006C388
		public static bool isInLobby { get; private set; }

		// Token: 0x170002F6 RID: 758
		// (get) Token: 0x060019AC RID: 6572 RVA: 0x0006E190 File Offset: 0x0006C390
		// (set) Token: 0x060019AD RID: 6573 RVA: 0x0006E197 File Offset: 0x0006C397
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

		// Token: 0x060019AE RID: 6574 RVA: 0x0006E1BE File Offset: 0x0006C3BE
		private static void UpdateOwnsLobby()
		{
			SteamworksLobbyManager.ownsLobby = SteamworksLobbyManager.client.Lobby.IsOwner;
		}

		// Token: 0x170002F7 RID: 759
		// (get) Token: 0x060019AF RID: 6575 RVA: 0x0006E1D4 File Offset: 0x0006C3D4
		public static bool hasMinimumPlayerCount
		{
			get
			{
				return SteamworksLobbyManager.newestLobbyData.totalPlayerCount >= SteamworksLobbyManager.minimumPlayerCount;
			}
		}

		// Token: 0x170002F8 RID: 760
		// (get) Token: 0x060019B0 RID: 6576 RVA: 0x0006E1EA File Offset: 0x0006C3EA
		// (set) Token: 0x060019B1 RID: 6577 RVA: 0x0006E1F1 File Offset: 0x0006C3F1
		public static int remoteMachineCount { get; private set; }

		// Token: 0x170002F9 RID: 761
		// (get) Token: 0x060019B2 RID: 6578 RVA: 0x0006E1F9 File Offset: 0x0006C3F9
		// (set) Token: 0x060019B3 RID: 6579 RVA: 0x0006E200 File Offset: 0x0006C400
		public static bool isFull { get; private set; }

		// Token: 0x170002FA RID: 762
		// (get) Token: 0x060019B4 RID: 6580 RVA: 0x0006E208 File Offset: 0x0006C408
		public static CSteamID serverId
		{
			get
			{
				return SteamworksLobbyManager.newestLobbyData.serverId;
			}
		}

		// Token: 0x170002FB RID: 763
		// (get) Token: 0x060019B5 RID: 6581 RVA: 0x0006E214 File Offset: 0x0006C414
		// (set) Token: 0x060019B6 RID: 6582 RVA: 0x0006E21B File Offset: 0x0006C41B
		public static SteamworksLobbyManager.LobbyData newestLobbyData { get; private set; }

		// Token: 0x060019B7 RID: 6583 RVA: 0x0006E224 File Offset: 0x0006C424
		public static void Init()
		{
			Client instance = Client.Instance;
			instance.Lobby.OnChatMessageRecieved = new Action<ulong, byte[], int>(SteamworksLobbyManager.OnChatMessageReceived);
			instance.Lobby.OnLobbyCreated = new Action<bool>(SteamworksLobbyManager.OnLobbyCreated);
			instance.Lobby.OnLobbyDataUpdated = new Action(SteamworksLobbyManager.OnLobbyDataUpdated);
			instance.Lobby.OnLobbyJoined = new Action<bool>(SteamworksLobbyManager.OnLobbyJoined);
			instance.Lobby.OnLobbyMemberDataUpdated = new Action<ulong>(SteamworksLobbyManager.OnLobbyMemberDataUpdated);
			instance.Lobby.OnLobbyStateChanged = new Action<Lobby.MemberStateChange, ulong, ulong>(SteamworksLobbyManager.OnLobbyStateChanged);
			instance.Lobby.OnLobbyKicked = new Action<bool, ulong, ulong>(SteamworksLobbyManager.OnLobbyKicked);
			instance.Lobby.OnLobbyLeave = new Action<ulong>(SteamworksLobbyManager.OnLobbyLeave);
			instance.Lobby.OnUserInvitedToLobby = new Action<ulong, ulong>(SteamworksLobbyManager.OnUserInvitedToLobby);
			instance.Lobby.OnLobbyJoinRequested = new Action<ulong>(SteamworksLobbyManager.OnLobbyJoinRequested);
			instance.LobbyList.OnLobbiesUpdated = new Action(SteamworksLobbyManager.OnLobbiesUpdated);
			RoR2Application.onUpdate += SteamworksLobbyManager.StaticUpdate;
			RoR2Application.onStart = (Action)Delegate.Combine(RoR2Application.onStart, new Action(SteamworksLobbyManager.CheckStartupLobby));
			GameNetworkManager.onStartServerGlobal += SteamworksLobbyManager.OnStartHostingServer;
			GameNetworkManager.onClientConnectGlobal += SteamworksLobbyManager.OnClientConnect;
			SteamworksLobbyManager.newestLobbyData = new SteamworksLobbyManager.LobbyData();
			LocalUserManager.onLocalUsersUpdated += SteamworksLobbyManager.UpdatePlayerCount;
			GameNetworkManager.onStartClientGlobal += SteamworksLobbyManager.OnStartClient;
			GameNetworkManager.onStopClientGlobal += SteamworksLobbyManager.OnStopClient;
			SteamworksLobbyManager.onLobbyOwnershipGained += delegate()
			{
				SteamworksLobbyManager.SetStartingIfOwner(false);
			};
			GameNetworkManager.onStopClientGlobal += delegate()
			{
				SteamworksLobbyManager.SetStartingIfOwner(false);
			};
			SteamworksLobbyManager.SetStartingIfOwner(false);
			SteamworksLobbyManager.pendingSteamworksLobbyId = SteamworksLobbyManager.GetLaunchParamsLobbyId();
		}

		// Token: 0x060019B8 RID: 6584 RVA: 0x0006E41A File Offset: 0x0006C61A
		public static int GetLobbyMemberPlayerCountByIndex(int memberIndex)
		{
			if (memberIndex >= SteamworksLobbyManager.playerCountsList.Count)
			{
				return 0;
			}
			return SteamworksLobbyManager.playerCountsList[memberIndex];
		}

		// Token: 0x170002FC RID: 764
		// (get) Token: 0x060019B9 RID: 6585 RVA: 0x0006E436 File Offset: 0x0006C636
		// (set) Token: 0x060019BA RID: 6586 RVA: 0x0006E43D File Offset: 0x0006C63D
		public static int calculatedTotalPlayerCount { get; private set; }

		// Token: 0x060019BB RID: 6587 RVA: 0x0006E448 File Offset: 0x0006C648
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
				SteamworksLobbyManager.remoteMachineCount = 0;
				ulong steamId = SteamworksLobbyManager.client.SteamId;
				int num = 0;
				foreach (ulong num2 in SteamworksLobbyManager.client.Lobby.GetMemberIDs())
				{
					int num3 = TextSerialization.TryParseInvariant(SteamworksLobbyManager.client.Lobby.GetMemberData(num2, "player_count"), out num3) ? Math.Min(Math.Max(1, num3), RoR2Application.maxLocalPlayers) : 1;
					if (num2 == steamId)
					{
						num3 = Math.Max(1, count);
					}
					else
					{
						SteamworksLobbyManager.remoteMachineCount++;
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

		// Token: 0x060019BC RID: 6588 RVA: 0x0006E623 File Offset: 0x0006C823
		[ConCommand(commandName = "steam_lobby_update_player_count", flags = ConVarFlags.None, helpText = "Forces a refresh of the steam lobby player count.")]
		private static void CCSteamLobbyUpdatePlayerCount(ConCommandArgs args)
		{
			SteamworksLobbyManager.UpdatePlayerCount();
		}

		// Token: 0x1400005B RID: 91
		// (add) Token: 0x060019BD RID: 6589 RVA: 0x0006E62C File Offset: 0x0006C82C
		// (remove) Token: 0x060019BE RID: 6590 RVA: 0x0006E660 File Offset: 0x0006C860
		public static event Action onPlayerCountUpdated;

		// Token: 0x1400005C RID: 92
		// (add) Token: 0x060019BF RID: 6591 RVA: 0x0006E694 File Offset: 0x0006C894
		// (remove) Token: 0x060019C0 RID: 6592 RVA: 0x0006E6C8 File Offset: 0x0006C8C8
		public static event Action onLobbyChanged;

		// Token: 0x060019C1 RID: 6593 RVA: 0x0006E6FC File Offset: 0x0006C8FC
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

		// Token: 0x060019C2 RID: 6594 RVA: 0x0006E790 File Offset: 0x0006C990
		public static void CreateLobby()
		{
			if (SteamworksLobbyManager.client == null)
			{
				return;
			}
			SteamworksLobbyManager.client.Lobby.Leave();
			SteamworksLobbyManager.client.Lobby.Create(SteamworksLobbyManager.preferredLobbyType, RoR2Application.maxPlayers);
		}

		// Token: 0x170002FD RID: 765
		// (get) Token: 0x060019C3 RID: 6595 RVA: 0x0006E7C2 File Offset: 0x0006C9C2
		// (set) Token: 0x060019C4 RID: 6596 RVA: 0x0006E7C9 File Offset: 0x0006C9C9
		public static bool awaitingJoin { get; private set; }

		// Token: 0x060019C5 RID: 6597 RVA: 0x0006E7D1 File Offset: 0x0006C9D1
		public static void JoinLobby(CSteamID newLobbyId)
		{
			if (SteamworksLobbyManager.client == null)
			{
				return;
			}
			SteamworksLobbyManager.awaitingJoin = true;
			SteamworksLobbyManager.client.Lobby.Join(newLobbyId.value);
		}

		// Token: 0x060019C6 RID: 6598 RVA: 0x0006E7F6 File Offset: 0x0006C9F6
		public static void LeaveLobby()
		{
			Client client = SteamworksLobbyManager.client;
			if (client == null)
			{
				return;
			}
			client.Lobby.Leave();
		}

		// Token: 0x060019C7 RID: 6599 RVA: 0x0006E80C File Offset: 0x0006CA0C
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

		// Token: 0x060019C8 RID: 6600 RVA: 0x0006E86D File Offset: 0x0006CA6D
		private static void StaticUpdate()
		{
			if (SteamworksLobbyManager.client == null)
			{
				return;
			}
			SteamworksLobbyManager.UpdateOwnsLobby();
		}

		// Token: 0x060019C9 RID: 6601 RVA: 0x0006E87C File Offset: 0x0006CA7C
		private static ulong GetLaunchParamsLobbyId()
		{
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for (int i = 0; i < commandLineArgs.Length - 1; i++)
			{
				ulong result;
				if (commandLineArgs[i].ToLower(CultureInfo.InvariantCulture) == "+connect_lobby" && TextSerialization.TryParseInvariant(commandLineArgs[i + 1], out result))
				{
					return result;
				}
			}
			return CSteamID.nil.value;
		}

		// Token: 0x060019CA RID: 6602 RVA: 0x0006E8D2 File Offset: 0x0006CAD2
		[ConCommand(commandName = "steam_lobby_create")]
		private static void CCSteamLobbyCreate(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			SteamworksLobbyManager.CreateLobby();
		}

		// Token: 0x060019CB RID: 6603 RVA: 0x0006E8DE File Offset: 0x0006CADE
		[ConCommand(commandName = "steam_lobby_create_if_none")]
		private static void CCSteamLobbyCreateIfNone(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			if (!SteamworksLobbyManager.client.Lobby.IsValid)
			{
				SteamworksLobbyManager.CreateLobby();
			}
		}

		// Token: 0x060019CC RID: 6604 RVA: 0x0006E8FB File Offset: 0x0006CAFB
		[ConCommand(commandName = "steam_lobby_find")]
		private static void CCSteamLobbyFind(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			SteamworksLobbyManager.awaitingLobbyListUpdate = true;
		}

		// Token: 0x060019CD RID: 6605 RVA: 0x0006E908 File Offset: 0x0006CB08
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

		// Token: 0x060019CE RID: 6606 RVA: 0x0006E95F File Offset: 0x0006CB5F
		[ConCommand(commandName = "steam_lobby_leave")]
		private static void CCSteamLobbyLeave(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			SteamworksLobbyManager.LeaveLobby();
		}

		// Token: 0x060019CF RID: 6607 RVA: 0x0006E96C File Offset: 0x0006CB6C
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

		// Token: 0x060019D0 RID: 6608 RVA: 0x0006E9D4 File Offset: 0x0006CBD4
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

		// Token: 0x060019D1 RID: 6609 RVA: 0x0006EA20 File Offset: 0x0006CC20
		[ConCommand(commandName = "steam_lobby_open_invite_overlay", flags = ConVarFlags.None, helpText = "Opens the steam overlay to the friend invite dialog.")]
		private static void CCSteamLobbyOpenInviteOverlay(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			SteamworksLobbyManager.client.Overlay.OpenInviteDialog(SteamworksLobbyManager.client.Lobby.CurrentLobby);
		}

		// Token: 0x060019D2 RID: 6610 RVA: 0x0006EA45 File Offset: 0x0006CC45
		[ConCommand(commandName = "steam_lobby_copy_to_clipboard", flags = ConVarFlags.None, helpText = "Copies the currently active lobby to the clipboard if applicable.")]
		private static void CCSteamLobbyCopyToClipboard(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			GUIUtility.systemCopyBuffer = TextSerialization.ToStringInvariant(SteamworksLobbyManager.client.Lobby.CurrentLobby);
			Chat.AddMessage(Language.GetString("STEAM_COPY_LOBBY_TO_CLIPBOARD_MESSAGE"));
		}

		// Token: 0x060019D3 RID: 6611 RVA: 0x0006EA74 File Offset: 0x0006CC74
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

		// Token: 0x060019D4 RID: 6612 RVA: 0x0006EB20 File Offset: 0x0006CD20
		[ConCommand(commandName = "steam_id", flags = ConVarFlags.None, helpText = "Displays your steam id.")]
		private static void CCSteamId(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			Debug.LogFormat("Steam id = {0}", new object[]
			{
				SteamworksLobbyManager.client.SteamId
			});
		}

		// Token: 0x060019D5 RID: 6613 RVA: 0x0006EB49 File Offset: 0x0006CD49
		[ConCommand(commandName = "steam_lobby_id", flags = ConVarFlags.None, helpText = "Displays the steam id of the current lobby.")]
		private static void CCSteamLobbyId(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			Debug.LogFormat("Lobby id = {0}", new object[]
			{
				SteamworksLobbyManager.client.Lobby.CurrentLobby
			});
		}

		// Token: 0x060019D6 RID: 6614 RVA: 0x0006EB78 File Offset: 0x0006CD78
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

		// Token: 0x060019D7 RID: 6615 RVA: 0x0006EC20 File Offset: 0x0006CE20
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

		// Token: 0x060019D8 RID: 6616 RVA: 0x0006ECE8 File Offset: 0x0006CEE8
		public static void ForceLobbyDataUpdate()
		{
			Client client = SteamworksLobbyManager.client;
			Lobby lobby = (client != null) ? client.Lobby : null;
			if (lobby != null)
			{
				SteamworksLobbyManager.updateLobbyDataMethodInfo.Invoke(lobby, Array.Empty<object>());
			}
		}

		// Token: 0x060019D9 RID: 6617 RVA: 0x0006ED1C File Offset: 0x0006CF1C
		private static void OnChatMessageReceived(ulong senderId, byte[] buffer, int byteCount)
		{
			NetworkReader networkReader = new NetworkReader(buffer);
			if (byteCount >= 1 && networkReader.ReadByte() == 0)
			{
				Chat.AddMessage(string.Format("{0}: {1}", Client.Instance.Friends.Get(senderId), networkReader.ReadString()));
			}
		}

		// Token: 0x060019DA RID: 6618 RVA: 0x0006ED63 File Offset: 0x0006CF63
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

		// Token: 0x060019DB RID: 6619 RVA: 0x0006ED88 File Offset: 0x0006CF88
		public static void StartMigrateLobby(CSteamID newLobbyId)
		{
			SteamworksLobbyManager.client.Lobby.Joinable = false;
			SteamworksLobbyManager.client.Lobby.CurrentLobbyData.SetData("migration_id", TextSerialization.ToStringInvariant(newLobbyId.value));
		}

		// Token: 0x060019DC RID: 6620 RVA: 0x0006EDBF File Offset: 0x0006CFBF
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

		// Token: 0x1400005D RID: 93
		// (add) Token: 0x060019DD RID: 6621 RVA: 0x0006EDFC File Offset: 0x0006CFFC
		// (remove) Token: 0x060019DE RID: 6622 RVA: 0x0006EE30 File Offset: 0x0006D030
		public static event Action onLobbyDataUpdated;

		// Token: 0x060019DF RID: 6623 RVA: 0x0006EE64 File Offset: 0x0006D064
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

		// Token: 0x1400005E RID: 94
		// (add) Token: 0x060019E0 RID: 6624 RVA: 0x0006EF30 File Offset: 0x0006D130
		// (remove) Token: 0x060019E1 RID: 6625 RVA: 0x0006EF64 File Offset: 0x0006D164
		public static event Action<bool> onLobbyJoined;

		// Token: 0x060019E2 RID: 6626 RVA: 0x0006EF98 File Offset: 0x0006D198
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

		// Token: 0x1400005F RID: 95
		// (add) Token: 0x060019E3 RID: 6627 RVA: 0x0006F0E4 File Offset: 0x0006D2E4
		// (remove) Token: 0x060019E4 RID: 6628 RVA: 0x0006F118 File Offset: 0x0006D318
		public static event Action<ulong> onLobbyLeave;

		// Token: 0x060019E5 RID: 6629 RVA: 0x0006F14B File Offset: 0x0006D34B
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

		// Token: 0x060019E6 RID: 6630 RVA: 0x0006F17C File Offset: 0x0006D37C
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

		// Token: 0x14000060 RID: 96
		// (add) Token: 0x060019E7 RID: 6631 RVA: 0x0006F1B0 File Offset: 0x0006D3B0
		// (remove) Token: 0x060019E8 RID: 6632 RVA: 0x0006F1E4 File Offset: 0x0006D3E4
		public static event Action<ulong> onLobbyMemberDataUpdated;

		// Token: 0x060019E9 RID: 6633 RVA: 0x0006F217 File Offset: 0x0006D417
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

		// Token: 0x14000061 RID: 97
		// (add) Token: 0x060019EA RID: 6634 RVA: 0x0006F230 File Offset: 0x0006D430
		// (remove) Token: 0x060019EB RID: 6635 RVA: 0x0006F264 File Offset: 0x0006D464
		public static event Action<Lobby.MemberStateChange, ulong, ulong> onLobbyStateChanged;

		// Token: 0x060019EC RID: 6636 RVA: 0x0006F298 File Offset: 0x0006D498
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

		// Token: 0x060019ED RID: 6637 RVA: 0x0006F2E7 File Offset: 0x0006D4E7
		private static void OnLobbyJoinRequested(ulong lobbyId)
		{
			Debug.LogFormat("Request to join lobby {0} received. Attempting to join lobby.", new object[]
			{
				lobbyId
			});
			SteamworksLobbyManager.JoinLobby(new CSteamID(lobbyId));
		}

		// Token: 0x060019EE RID: 6638 RVA: 0x0006F30D File Offset: 0x0006D50D
		private static void OnUserInvitedToLobby(ulong lobbyId, ulong senderId)
		{
			Debug.LogFormat("Received invitation to lobby {0} from sender {1}.", new object[]
			{
				lobbyId,
				senderId
			});
		}

		// Token: 0x060019EF RID: 6639 RVA: 0x0006F334 File Offset: 0x0006D534
		[ConCommand(commandName = "dump_lobbies", flags = ConVarFlags.None, helpText = "")]
		private static void DumpLobbies(ConCommandArgs args)
		{
			LobbyList.Filter filter = new LobbyList.Filter();
			filter.MaxResults = new int?(50);
			filter.DistanceFilter = LobbyList.Filter.Distance.Worldwide;
			SteamworksLobbyManager.client.LobbyList.Refresh(filter);
		}

		// Token: 0x060019F0 RID: 6640 RVA: 0x0006F36C File Offset: 0x0006D56C
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

		// Token: 0x060019F1 RID: 6641 RVA: 0x0006F3F3 File Offset: 0x0006D5F3
		private static void CheckStartupLobby()
		{
			if (SteamworksLobbyManager.pendingSteamworksLobbyId != 0UL)
			{
				Console.instance.SubmitCmd(null, string.Format("steam_lobby_join {0}", SteamworksLobbyManager.pendingSteamworksLobbyId), false);
				SteamworksLobbyManager.pendingSteamworksLobbyId = 0UL;
			}
		}

		// Token: 0x060019F2 RID: 6642 RVA: 0x0006F424 File Offset: 0x0006D624
		public static void OnServerIPDiscovered(string address, ushort port)
		{
			if (SteamworksLobbyManager.client == null)
			{
				return;
			}
			if (SteamworksLobbyManager.client.Lobby.IsValid && SteamworksLobbyManager.client.Lobby.Owner == SteamworksLobbyManager.client.SteamId)
			{
				SteamworksLobbyManager.client.Lobby.CurrentLobbyData.SetData("server_address", address + ":" + port);
			}
		}

		// Token: 0x060019F3 RID: 6643 RVA: 0x0000409B File Offset: 0x0000229B
		private static void OnClientConnect(NetworkConnection conn)
		{
		}

		// Token: 0x060019F4 RID: 6644 RVA: 0x0006F490 File Offset: 0x0006D690
		private static void OnStartHostingServer()
		{
			SteamworksLobbyManager.OnServerSteamIDDiscovered(new CSteamID(Client.Instance.SteamId));
		}

		// Token: 0x060019F5 RID: 6645 RVA: 0x0006F4A8 File Offset: 0x0006D6A8
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

		// Token: 0x060019F6 RID: 6646 RVA: 0x0006F510 File Offset: 0x0006D710
		private static void OnStartClient(NetworkClient networkClient)
		{
			if (SteamworksLobbyManager.ownsLobby)
			{
				SteamworksLobbyManager.client.Lobby.LobbyType = SteamworksLobbyManager.preferredLobbyType;
			}
		}

		// Token: 0x060019F7 RID: 6647 RVA: 0x0006F530 File Offset: 0x0006D730
		private static void OnStopClient()
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
		}

		// Token: 0x060019F8 RID: 6648 RVA: 0x0006F5D1 File Offset: 0x0006D7D1
		public static void SendMigrationMessage(CSteamID newLobbyId)
		{
			if (SteamworksLobbyManager.client != null)
			{
				SteamworksLobbyManager.client.Lobby.SendChatMessage("");
				SteamworksLobbyManager.JoinLobby(newLobbyId);
			}
		}

		// Token: 0x14000062 RID: 98
		// (add) Token: 0x060019F9 RID: 6649 RVA: 0x0006F5F8 File Offset: 0x0006D7F8
		// (remove) Token: 0x060019FA RID: 6650 RVA: 0x0006F62C File Offset: 0x0006D82C
		public static event Action onLobbiesUpdated;

		// Token: 0x060019FB RID: 6651 RVA: 0x0006F660 File Offset: 0x0006D860
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

		// Token: 0x060019FC RID: 6652 RVA: 0x0006F6D8 File Offset: 0x0006D8D8
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

		// Token: 0x060019FD RID: 6653 RVA: 0x0006F73C File Offset: 0x0006D93C
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

		// Token: 0x060019FE RID: 6654 RVA: 0x0006F788 File Offset: 0x0006D988
		private static void OnLobbyOwnershipGained()
		{
			Action action = SteamworksLobbyManager.onLobbyOwnershipGained;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x060019FF RID: 6655 RVA: 0x0006F799 File Offset: 0x0006D999
		private static void OnLobbyOwnershipLost()
		{
			Action action = SteamworksLobbyManager.onLobbyOwnershipLost;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x14000063 RID: 99
		// (add) Token: 0x06001A00 RID: 6656 RVA: 0x0006F7AC File Offset: 0x0006D9AC
		// (remove) Token: 0x06001A01 RID: 6657 RVA: 0x0006F7E0 File Offset: 0x0006D9E0
		public static event Action onLobbyOwnershipGained;

		// Token: 0x14000064 RID: 100
		// (add) Token: 0x06001A02 RID: 6658 RVA: 0x0006F814 File Offset: 0x0006DA14
		// (remove) Token: 0x06001A03 RID: 6659 RVA: 0x0006F848 File Offset: 0x0006DA48
		public static event Action onLobbyOwnershipLost;

		// Token: 0x040017D1 RID: 6097
		public static Lobby.Type preferredLobbyType = Lobby.Type.FriendsOnly;

		// Token: 0x040017D2 RID: 6098
		public static ulong pendingSteamworksLobbyId;

		// Token: 0x040017D4 RID: 6100
		private static bool _ownsLobby;

		// Token: 0x040017D5 RID: 6101
		private static int minimumPlayerCount = 2;

		// Token: 0x040017D8 RID: 6104
		public const string mdV = "v";

		// Token: 0x040017D9 RID: 6105
		public const string mdAppId = "appid";

		// Token: 0x040017DA RID: 6106
		public const string mdTotalMaxPlayers = "total_max_players";

		// Token: 0x040017DB RID: 6107
		public const string mdPlayerCount = "player_count";

		// Token: 0x040017DC RID: 6108
		public const string mdQuickplayQueued = "qp";

		// Token: 0x040017DD RID: 6109
		public const string mdQuickplayCutoffTime = "qp_cutoff_time";

		// Token: 0x040017DE RID: 6110
		public const string mdServerId = "server_id";

		// Token: 0x040017DF RID: 6111
		public const string mdServerAddress = "server_address";

		// Token: 0x040017E0 RID: 6112
		public const string mdMigrationId = "migration_id";

		// Token: 0x040017E1 RID: 6113
		public const string mdStarting = "starting";

		// Token: 0x040017E2 RID: 6114
		public const string mdBuildId = "build_id";

		// Token: 0x040017E4 RID: 6116
		private static readonly List<int> playerCountsList = new List<int>();

		// Token: 0x040017E9 RID: 6121
		private static bool startingFadeSet = false;

		// Token: 0x040017EA RID: 6122
		private static readonly MethodInfo updateLobbyDataMethodInfo = typeof(Lobby).GetMethod("UpdateLobbyData", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x040017F0 RID: 6128
		private static bool awaitingLobbyListUpdate = false;

		// Token: 0x040017F2 RID: 6130
		private static int v = 0;

		// Token: 0x02000431 RID: 1073
		public class LobbyData
		{
			// Token: 0x06001A06 RID: 6662 RVA: 0x0006F8CC File Offset: 0x0006DACC
			public LobbyData()
			{
			}

			// Token: 0x06001A07 RID: 6663 RVA: 0x0006F8EC File Offset: 0x0006DAEC
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

			// Token: 0x06001A08 RID: 6664 RVA: 0x0006F9F0 File Offset: 0x0006DBF0
			[CompilerGenerated]
			internal static bool <.ctor>g__ReadString|11_0(string metaDataName, ref string field, ref SteamworksLobbyManager.LobbyData.<>c__DisplayClass11_0 A_2)
			{
				string text;
				if (A_2.lobbyDataDictionary.TryGetValue(metaDataName, out text))
				{
					field = text;
					return true;
				}
				return false;
			}

			// Token: 0x06001A09 RID: 6665 RVA: 0x0006FA14 File Offset: 0x0006DC14
			[CompilerGenerated]
			internal static bool <.ctor>g__ReadBool|11_1(string metaDataName, ref bool field, ref SteamworksLobbyManager.LobbyData.<>c__DisplayClass11_0 A_2)
			{
				string s;
				int num;
				if (A_2.lobbyDataDictionary.TryGetValue(metaDataName, out s) && TextSerialization.TryParseInvariant(s, out num))
				{
					field = (num != 0);
					return true;
				}
				return false;
			}

			// Token: 0x06001A0A RID: 6666 RVA: 0x0006FA44 File Offset: 0x0006DC44
			[CompilerGenerated]
			internal static bool <.ctor>g__ReadInt|11_2(string metaDataName, ref int field, ref SteamworksLobbyManager.LobbyData.<>c__DisplayClass11_0 A_2)
			{
				string s;
				int num;
				if (A_2.lobbyDataDictionary.TryGetValue(metaDataName, out s) && TextSerialization.TryParseInvariant(s, out num))
				{
					field = num;
					return true;
				}
				return false;
			}

			// Token: 0x06001A0B RID: 6667 RVA: 0x0006FA74 File Offset: 0x0006DC74
			[CompilerGenerated]
			internal static bool <.ctor>g__ReadCSteamID|11_3(string metaDataName, ref CSteamID field, ref SteamworksLobbyManager.LobbyData.<>c__DisplayClass11_0 A_2)
			{
				string str;
				CSteamID csteamID;
				if (A_2.lobbyDataDictionary.TryGetValue(metaDataName, out str) && CSteamID.TryParse(str, out csteamID))
				{
					field = csteamID;
					return true;
				}
				return false;
			}

			// Token: 0x06001A0C RID: 6668 RVA: 0x0006FAA8 File Offset: 0x0006DCA8
			[CompilerGenerated]
			internal static bool <.ctor>g__ReadAddressPortPair|11_4(string metaDataName, ref AddressPortPair field, ref SteamworksLobbyManager.LobbyData.<>c__DisplayClass11_0 A_2)
			{
				string str;
				AddressPortPair addressPortPair;
				if (A_2.lobbyDataDictionary.TryGetValue(metaDataName, out str) && AddressPortPair.TryParse(str, out addressPortPair))
				{
					field = addressPortPair;
					return true;
				}
				return false;
			}

			// Token: 0x06001A0D RID: 6669 RVA: 0x0006FADC File Offset: 0x0006DCDC
			[CompilerGenerated]
			internal static bool <.ctor>g__ReadNullableDate|11_5(string metaDataName, out DateTime? field, ref SteamworksLobbyManager.LobbyData.<>c__DisplayClass11_0 A_2)
			{
				string s;
				uint num;
				if (A_2.lobbyDataDictionary.TryGetValue(metaDataName, out s) && TextSerialization.TryParseInvariant(s, out num))
				{
					field = new DateTime?(Util.dateZero + TimeSpan.FromSeconds(num));
					return true;
				}
				field = null;
				return false;
			}

			// Token: 0x040017F5 RID: 6133
			public readonly int totalMaxPlayers = RoR2Application.maxPlayers;

			// Token: 0x040017F6 RID: 6134
			public readonly int totalPlayerCount;

			// Token: 0x040017F7 RID: 6135
			public readonly bool quickplayQueued;

			// Token: 0x040017F8 RID: 6136
			public readonly CSteamID serverId;

			// Token: 0x040017F9 RID: 6137
			public readonly AddressPortPair serverAddressPortPair;

			// Token: 0x040017FA RID: 6138
			public readonly CSteamID migrationId;

			// Token: 0x040017FB RID: 6139
			public readonly bool starting;

			// Token: 0x040017FC RID: 6140
			public readonly string buildId = "0";

			// Token: 0x040017FD RID: 6141
			public readonly DateTime? quickplayCutoffTime;

			// Token: 0x040017FE RID: 6142
			public readonly bool shouldConnect;
		}

		// Token: 0x02000433 RID: 1075
		private enum LobbyMessageType : byte
		{
			// Token: 0x04001801 RID: 6145
			Chat
		}
	}
}
