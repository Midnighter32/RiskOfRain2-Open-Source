using System;
using System.Collections.Generic;
using EntityStates;
using Facepunch.Steamworks;
using RoR2.UI.MainMenu;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x0200058A RID: 1418
	public class SteamLobbyFinder : MonoBehaviour
	{
		// Token: 0x170002CD RID: 717
		// (get) Token: 0x06001FF2 RID: 8178 RVA: 0x0007C779 File Offset: 0x0007A979
		private static Client steamClient
		{
			get
			{
				return Client.Instance;
			}
		}

		// Token: 0x06001FF3 RID: 8179 RVA: 0x000964FD File Offset: 0x000946FD
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void OnStartup()
		{
			SteamworksLobbyManager.onLobbiesUpdated += delegate()
			{
				SteamLobbyFinder.awaitingLobbyRefresh = false;
			};
		}

		// Token: 0x06001FF4 RID: 8180 RVA: 0x00096523 File Offset: 0x00094723
		private void Awake()
		{
			this.stateMachine = base.gameObject.AddComponent<EntityStateMachine>();
			this.stateMachine.initialStateType = new SerializableEntityStateType(typeof(SteamLobbyFinder.LobbyStateStart));
		}

		// Token: 0x06001FF5 RID: 8181 RVA: 0x00096550 File Offset: 0x00094750
		private void OnDestroy()
		{
			UnityEngine.Object.Destroy(this.stateMachine);
		}

		// Token: 0x06001FF6 RID: 8182 RVA: 0x0009655D File Offset: 0x0009475D
		private void OnEnable()
		{
			SteamworksLobbyManager.onLobbiesUpdated += this.OnLobbiesUpdated;
		}

		// Token: 0x06001FF7 RID: 8183 RVA: 0x00096570 File Offset: 0x00094770
		private void OnDisable()
		{
			SteamworksLobbyManager.onLobbiesUpdated -= this.OnLobbiesUpdated;
		}

		// Token: 0x06001FF8 RID: 8184 RVA: 0x00096584 File Offset: 0x00094784
		private void Update()
		{
			if (SteamLobbyFinder.steamClient.Lobby.IsValid && !SteamworksLobbyManager.ownsLobby)
			{
				if (this.stateMachine.state.GetType() != typeof(SteamLobbyFinder.LobbyStateNonLeader))
				{
					this.stateMachine.SetNextState(new SteamLobbyFinder.LobbyStateNonLeader());
				}
				return;
			}
			this.refreshTimer -= Time.unscaledDeltaTime;
			this.age += Time.unscaledDeltaTime;
		}

		// Token: 0x06001FF9 RID: 8185 RVA: 0x00096600 File Offset: 0x00094800
		private static void RequestLobbyListRefresh()
		{
			if (SteamLobbyFinder.awaitingLobbyRefresh)
			{
				return;
			}
			SteamLobbyFinder.awaitingLobbyRefresh = true;
			LobbyList.Filter filter = new LobbyList.Filter();
			filter.StringFilters["appid"] = TextSerialization.ToStringInvariant(SteamLobbyFinder.steamClient.AppId);
			filter.StringFilters["build_id"] = RoR2Application.GetBuildId();
			filter.StringFilters["qp"] = "1";
			filter.StringFilters["total_max_players"] = TextSerialization.ToStringInvariant(RoR2Application.maxPlayers);
			LobbyList.Filter filter2 = filter;
			SteamLobbyFinder.steamClient.LobbyList.Refresh(filter2);
		}

		// Token: 0x06001FFA RID: 8186 RVA: 0x00096694 File Offset: 0x00094894
		private void OnLobbiesUpdated()
		{
			SteamLobbyFinder.LobbyStateBase lobbyStateBase = this.stateMachine.state as SteamLobbyFinder.LobbyStateBase;
			if (lobbyStateBase == null)
			{
				return;
			}
			lobbyStateBase.OnLobbiesUpdated();
		}

		// Token: 0x06001FFB RID: 8187 RVA: 0x000966B0 File Offset: 0x000948B0
		private static bool CanJoinLobby(int currentLobbySize, LobbyList.Lobby lobby)
		{
			return currentLobbySize + SteamLobbyFinder.GetRealLobbyPlayerCount(lobby) < lobby.MemberLimit;
		}

		// Token: 0x06001FFC RID: 8188 RVA: 0x000966C4 File Offset: 0x000948C4
		private static int GetRealLobbyPlayerCount(LobbyList.Lobby lobby)
		{
			string data = lobby.GetData("player_count");
			int result;
			if (data != null && TextSerialization.TryParseInvariant(data, out result))
			{
				return result;
			}
			return SteamLobbyFinder.steamClient.Lobby.MaxMembers;
		}

		// Token: 0x06001FFD RID: 8189 RVA: 0x000966FB File Offset: 0x000948FB
		private static int GetCurrentLobbyRealPlayerCount()
		{
			return SteamworksLobbyManager.newestLobbyData.totalPlayerCount;
		}

		// Token: 0x170002CE RID: 718
		// (get) Token: 0x06001FFE RID: 8190 RVA: 0x00096707 File Offset: 0x00094907
		// (set) Token: 0x06001FFF RID: 8191 RVA: 0x0009670E File Offset: 0x0009490E
		public static bool userRequestedQuickplayQueue
		{
			get
			{
				return SteamLobbyFinder._userRequestedQuickplayQueue;
			}
			set
			{
				if (SteamLobbyFinder._userRequestedQuickplayQueue != value)
				{
					SteamLobbyFinder._userRequestedQuickplayQueue = value;
					SteamLobbyFinder.running = (SteamLobbyFinder._lobbyIsInQuickplayQueue || SteamLobbyFinder._userRequestedQuickplayQueue);
				}
			}
		}

		// Token: 0x170002CF RID: 719
		// (get) Token: 0x06002000 RID: 8192 RVA: 0x00096732 File Offset: 0x00094932
		// (set) Token: 0x06002001 RID: 8193 RVA: 0x00096739 File Offset: 0x00094939
		private static bool lobbyIsInQuickplayQueue
		{
			get
			{
				return SteamLobbyFinder._lobbyIsInQuickplayQueue;
			}
			set
			{
				if (SteamLobbyFinder._lobbyIsInQuickplayQueue != value)
				{
					SteamLobbyFinder._lobbyIsInQuickplayQueue = value;
					SteamLobbyFinder.running = (SteamLobbyFinder._lobbyIsInQuickplayQueue || SteamLobbyFinder._userRequestedQuickplayQueue);
				}
			}
		}

		// Token: 0x06002002 RID: 8194 RVA: 0x0009675D File Offset: 0x0009495D
		[ConCommand(commandName = "steam_quickplay_start")]
		private static void CCSteamQuickplayStart(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			SteamLobbyFinder.userRequestedQuickplayQueue = true;
			SteamworksLobbyManager.SetLobbyQuickPlayQueuedIfOwner(true);
		}

		// Token: 0x06002003 RID: 8195 RVA: 0x00096770 File Offset: 0x00094970
		[ConCommand(commandName = "steam_quickplay_stop")]
		private static void CCSteamQuickplayStop(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			SteamLobbyFinder.userRequestedQuickplayQueue = false;
			SteamworksLobbyManager.CreateLobby();
		}

		// Token: 0x06002004 RID: 8196 RVA: 0x00096784 File Offset: 0x00094984
		static SteamLobbyFinder()
		{
			SteamworksLobbyManager.onLobbyDataUpdated += SteamLobbyFinder.OnLobbyDataUpdated;
			GameNetworkManager.onStartClientGlobal += delegate(NetworkClient client)
			{
				SteamworksLobbyManager.SetLobbyQuickPlayQueuedIfOwner(false);
				SteamLobbyFinder.userRequestedQuickplayQueue = false;
			};
			SteamworksLobbyManager.onLobbyOwnershipGained += delegate()
			{
				if (SteamworksLobbyManager.newestLobbyData.quickplayQueued)
				{
					SteamLobbyFinder.userRequestedQuickplayQueue = true;
				}
			};
			SteamworksLobbyManager.onLobbyOwnershipLost += delegate()
			{
				SteamLobbyFinder.userRequestedQuickplayQueue = false;
			};
		}

		// Token: 0x06002005 RID: 8197 RVA: 0x000967E1 File Offset: 0x000949E1
		private static void OnLobbyDataUpdated()
		{
			SteamLobbyFinder.lobbyIsInQuickplayQueue = SteamworksLobbyManager.newestLobbyData.quickplayQueued;
		}

		// Token: 0x06002006 RID: 8198 RVA: 0x000967F4 File Offset: 0x000949F4
		public static string GetResolvedStateString()
		{
			if (!SteamLobbyFinder.steamClient.Lobby.IsValid)
			{
				return Language.GetString("STEAM_LOBBY_STATUS_NOT_IN_LOBBY");
			}
			bool flag = SteamLobbyFinder.steamClient.Lobby.LobbyType == Lobby.Type.Public;
			if (SteamLobbyFinder.instance)
			{
				bool flag2 = SteamLobbyFinder.instance.stateMachine.state is SteamLobbyFinder.LobbyStateSingleSearch;
			}
			int totalPlayerCount = SteamworksLobbyManager.newestLobbyData.totalPlayerCount;
			int totalMaxPlayers = SteamworksLobbyManager.newestLobbyData.totalMaxPlayers;
			bool isFull = SteamworksLobbyManager.isFull;
			string token = string.Empty;
			object[] args = Array.Empty<object>();
			if (GameNetworkManager.singleton.isHost || (MultiplayerMenuController.instance && MultiplayerMenuController.instance.isInHostingState))
			{
				token = "STEAM_LOBBY_STATUS_STARTING_SERVER";
			}
			else if (SteamworksLobbyManager.newestLobbyData.starting)
			{
				token = "STEAM_LOBBY_STATUS_GAME_STARTING";
			}
			else if (SteamworksLobbyManager.newestLobbyData.shouldConnect)
			{
				token = "STEAM_LOBBY_STATUS_CONNECTING_TO_HOST";
			}
			else if (SteamLobbyFinder.instance && SteamLobbyFinder.instance.stateMachine.state is SteamLobbyFinder.LobbyStateStart)
			{
				token = "STEAM_LOBBY_STATUS_LAUNCHING_QUICKPLAY";
			}
			else if (SteamworksLobbyManager.isInLobby)
			{
				if (SteamworksLobbyManager.newestLobbyData.quickplayQueued)
				{
					if (!flag)
					{
						token = "STEAM_LOBBY_STATUS_QUICKPLAY_SEARCHING_FOR_EXISTING_LOBBY";
					}
					else
					{
						DateTime d = Util.UnixTimeStampToDateTimeUtc(SteamLobbyFinder.steamClient.Utils.GetServerRealTime());
						DateTime? quickplayCutoffTime = SteamworksLobbyManager.newestLobbyData.quickplayCutoffTime;
						if (quickplayCutoffTime == null)
						{
							token = "STEAM_LOBBY_STATUS_QUICKPLAY_WAITING_BELOW_MINIMUM_PLAYERS";
							args = new object[]
							{
								SteamworksLobbyManager.newestLobbyData.totalPlayerCount,
								SteamworksLobbyManager.newestLobbyData.totalMaxPlayers
							};
						}
						else
						{
							TimeSpan timeSpan = quickplayCutoffTime.Value - d;
							token = "STEAM_LOBBY_STATUS_QUICKPLAY_WAITING_ABOVE_MINIMUM_PLAYERS";
							args = new object[]
							{
								SteamworksLobbyManager.newestLobbyData.totalPlayerCount,
								SteamworksLobbyManager.newestLobbyData.totalMaxPlayers,
								Math.Max(0.0, timeSpan.TotalSeconds)
							};
						}
					}
				}
				else
				{
					token = "STEAM_LOBBY_STATUS_OUT_OF_QUICKPLAY";
				}
			}
			return Language.GetStringFormatted(token, args);
		}

		// Token: 0x170002D0 RID: 720
		// (get) Token: 0x06002007 RID: 8199 RVA: 0x000969F9 File Offset: 0x00094BF9
		// (set) Token: 0x06002008 RID: 8200 RVA: 0x00096A08 File Offset: 0x00094C08
		public static bool running
		{
			get
			{
				return SteamLobbyFinder.instance;
			}
			private set
			{
				if (SteamLobbyFinder.instance != value)
				{
					if (value)
					{
						SteamLobbyFinder.instance = new GameObject("SteamLobbyFinder", new Type[]
						{
							typeof(SteamLobbyFinder),
							typeof(SetDontDestroyOnLoad)
						}).GetComponent<SteamLobbyFinder>();
						return;
					}
					UnityEngine.Object.Destroy(SteamLobbyFinder.instance.gameObject);
					SteamLobbyFinder.instance = null;
				}
			}
		}

		// Token: 0x04002238 RID: 8760
		private float age;

		// Token: 0x04002239 RID: 8761
		public float joinOnlyDuration = 5f;

		// Token: 0x0400223A RID: 8762
		public float waitForFullDuration = 20f;

		// Token: 0x0400223B RID: 8763
		public float startDelayDuration = 1f;

		// Token: 0x0400223C RID: 8764
		public float refreshInterval = 2f;

		// Token: 0x0400223D RID: 8765
		private float refreshTimer;

		// Token: 0x0400223E RID: 8766
		private EntityStateMachine stateMachine;

		// Token: 0x0400223F RID: 8767
		private static bool awaitingLobbyRefresh;

		// Token: 0x04002240 RID: 8768
		private static SteamLobbyFinder instance;

		// Token: 0x04002241 RID: 8769
		private static bool _userRequestedQuickplayQueue;

		// Token: 0x04002242 RID: 8770
		private static bool _lobbyIsInQuickplayQueue;

		// Token: 0x0200058B RID: 1419
		private class LobbyStateBase : BaseState
		{
			// Token: 0x0600200A RID: 8202 RVA: 0x00096AA3 File Offset: 0x00094CA3
			public override void OnEnter()
			{
				base.OnEnter();
				this.lobbyFinder = base.GetComponent<SteamLobbyFinder>();
				SteamworksLobbyManager.onLobbyOwnershipGained += this.OnLobbyOwnershipGained;
				SteamworksLobbyManager.onLobbyOwnershipLost += this.OnLobbyOwnershipLost;
			}

			// Token: 0x0600200B RID: 8203 RVA: 0x00096AD9 File Offset: 0x00094CD9
			public override void OnExit()
			{
				SteamworksLobbyManager.onLobbyOwnershipGained -= this.OnLobbyOwnershipGained;
				SteamworksLobbyManager.onLobbyOwnershipLost -= this.OnLobbyOwnershipLost;
				base.OnExit();
			}

			// Token: 0x0600200C RID: 8204 RVA: 0x00004507 File Offset: 0x00002707
			public virtual void OnLobbiesUpdated()
			{
			}

			// Token: 0x0600200D RID: 8205 RVA: 0x00096B03 File Offset: 0x00094D03
			private void OnLobbyOwnershipGained()
			{
				this.outer.SetNextState(new SteamLobbyFinder.LobbyStateStart());
			}

			// Token: 0x0600200E RID: 8206 RVA: 0x00096B15 File Offset: 0x00094D15
			private void OnLobbyOwnershipLost()
			{
				this.outer.SetNextState(new SteamLobbyFinder.LobbyStateNonLeader());
			}

			// Token: 0x04002243 RID: 8771
			protected SteamLobbyFinder lobbyFinder;
		}

		// Token: 0x0200058C RID: 1420
		private class LobbyStateNonLeader : SteamLobbyFinder.LobbyStateBase
		{
			// Token: 0x06002010 RID: 8208 RVA: 0x00096B27 File Offset: 0x00094D27
			public override void Update()
			{
				base.Update();
				if (SteamworksLobbyManager.ownsLobby)
				{
					if (SteamworksLobbyManager.hasMinimumPlayerCount)
					{
						this.outer.SetNextState(new SteamLobbyFinder.LobbyStateMultiSearch());
						return;
					}
					this.outer.SetNextState(new SteamLobbyFinder.LobbyStateSingleSearch());
				}
			}
		}

		// Token: 0x0200058D RID: 1421
		private class LobbyStateStart : SteamLobbyFinder.LobbyStateBase
		{
			// Token: 0x06002012 RID: 8210 RVA: 0x00096B66 File Offset: 0x00094D66
			public override void Update()
			{
				base.Update();
				if (this.lobbyFinder.startDelayDuration <= base.age)
				{
					this.outer.SetNextState(SteamworksLobbyManager.hasMinimumPlayerCount ? new SteamLobbyFinder.LobbyStateMultiSearch() : new SteamLobbyFinder.LobbyStateSingleSearch());
				}
			}
		}

		// Token: 0x0200058E RID: 1422
		private class LobbyStateSingleSearch : SteamLobbyFinder.LobbyStateBase
		{
			// Token: 0x06002014 RID: 8212 RVA: 0x00096BA0 File Offset: 0x00094DA0
			public override void OnEnter()
			{
				base.OnEnter();
				SteamworksLobbyManager.SetLobbyQuickPlayCutoffTimeIfOwner(null);
			}

			// Token: 0x06002015 RID: 8213 RVA: 0x00096BC4 File Offset: 0x00094DC4
			public override void Update()
			{
				base.Update();
				if (SteamworksLobbyManager.hasMinimumPlayerCount)
				{
					this.outer.SetNextState(new SteamLobbyFinder.LobbyStateMultiSearch());
					return;
				}
				if (this.lobbyFinder.refreshTimer <= 0f)
				{
					if (base.age >= this.lobbyFinder.joinOnlyDuration && SteamLobbyFinder.steamClient.Lobby.LobbyType != Lobby.Type.Public)
					{
						Debug.LogFormat("Unable to find joinable lobby after {0} seconds. Setting lobby to public.", new object[]
						{
							this.lobbyFinder.age
						});
						SteamLobbyFinder.steamClient.Lobby.LobbyType = Lobby.Type.Public;
					}
					this.lobbyFinder.refreshTimer = this.lobbyFinder.refreshInterval;
					SteamLobbyFinder.RequestLobbyListRefresh();
				}
			}

			// Token: 0x06002016 RID: 8214 RVA: 0x00096C74 File Offset: 0x00094E74
			public override void OnLobbiesUpdated()
			{
				base.OnLobbiesUpdated();
				if (SteamLobbyFinder.steamClient.IsValid)
				{
					List<LobbyList.Lobby> lobbies = SteamLobbyFinder.steamClient.LobbyList.Lobbies;
					List<LobbyList.Lobby> list = new List<LobbyList.Lobby>();
					ulong currentLobby = SteamLobbyFinder.steamClient.Lobby.CurrentLobby;
					bool isValid = SteamLobbyFinder.steamClient.Lobby.IsValid;
					int currentLobbySize = isValid ? SteamLobbyFinder.GetCurrentLobbyRealPlayerCount() : LocalUserManager.readOnlyLocalUsersList.Count;
					if (SteamworksLobbyManager.ownsLobby || !isValid)
					{
						for (int i = 0; i < lobbies.Count; i++)
						{
							if ((!isValid || lobbies[i].LobbyID < currentLobby) && SteamLobbyFinder.CanJoinLobby(currentLobbySize, lobbies[i]))
							{
								list.Add(lobbies[i]);
							}
						}
						if (list.Count > 0)
						{
							SteamworksLobbyManager.JoinLobby(new CSteamID(list[0].LobbyID));
						}
					}
					Debug.LogFormat("Found {0} lobbies, {1} joinable.", new object[]
					{
						lobbies.Count,
						list.Count
					});
				}
			}
		}

		// Token: 0x0200058F RID: 1423
		private class LobbyStateMultiSearch : SteamLobbyFinder.LobbyStateBase
		{
			// Token: 0x06002018 RID: 8216 RVA: 0x00096D80 File Offset: 0x00094F80
			public override void OnEnter()
			{
				base.OnEnter();
				SteamLobbyFinder.steamClient.Lobby.LobbyType = Lobby.Type.Public;
				SteamworksLobbyManager.SetLobbyQuickPlayCutoffTimeIfOwner(new uint?((uint)(Util.UnixTimeStampToDateTimeUtc(SteamLobbyFinder.steamClient.Utils.GetServerRealTime()) + TimeSpan.FromSeconds((double)this.lobbyFinder.waitForFullDuration) - Util.dateZero).TotalSeconds));
			}

			// Token: 0x06002019 RID: 8217 RVA: 0x00096DEC File Offset: 0x00094FEC
			public override void OnExit()
			{
				SteamworksLobbyManager.SetLobbyQuickPlayCutoffTimeIfOwner(null);
				base.OnExit();
			}

			// Token: 0x0600201A RID: 8218 RVA: 0x00096E10 File Offset: 0x00095010
			public override void Update()
			{
				base.Update();
				if (!SteamworksLobbyManager.hasMinimumPlayerCount)
				{
					this.outer.SetNextState(new SteamLobbyFinder.LobbyStateSingleSearch());
					return;
				}
				if (this.lobbyFinder.waitForFullDuration <= base.age)
				{
					this.outer.SetNextState(new SteamLobbyFinder.LobbyStateBeginGame());
				}
			}
		}

		// Token: 0x02000590 RID: 1424
		private class LobbyStateBeginGame : SteamLobbyFinder.LobbyStateBase
		{
			// Token: 0x0600201C RID: 8220 RVA: 0x00096E60 File Offset: 0x00095060
			public override void OnEnter()
			{
				base.OnEnter();
				SteamLobbyFinder.steamClient.Lobby.LobbyType = SteamworksLobbyManager.preferredLobbyType;
				SteamworksLobbyManager.SetStartingIfOwner(true);
				string arg = "ClassicRun";
				Console.instance.SubmitCmd(null, string.Format("transition_command \"gamemode {0}; host 1;\"", arg), false);
			}
		}
	}
}
