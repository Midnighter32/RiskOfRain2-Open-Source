using System;
using System.Collections.Generic;
using EntityStates;
using Facepunch.Steamworks;
using RoR2.UI.MainMenu;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x0200055B RID: 1371
	public class SteamLobbyFinder : MonoBehaviour
	{
		// Token: 0x17000370 RID: 880
		// (get) Token: 0x060020BB RID: 8379 RVA: 0x0006E17A File Offset: 0x0006C37A
		private static Client steamClient
		{
			get
			{
				return Client.Instance;
			}
		}

		// Token: 0x060020BC RID: 8380 RVA: 0x0008DA91 File Offset: 0x0008BC91
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void OnStartup()
		{
			SteamworksLobbyManager.onLobbiesUpdated += delegate()
			{
				SteamLobbyFinder.awaitingLobbyRefresh = false;
			};
		}

		// Token: 0x060020BD RID: 8381 RVA: 0x0008DAB7 File Offset: 0x0008BCB7
		private void Awake()
		{
			this.stateMachine = base.gameObject.AddComponent<EntityStateMachine>();
			this.stateMachine.initialStateType = new SerializableEntityStateType(typeof(SteamLobbyFinder.LobbyStateStart));
		}

		// Token: 0x060020BE RID: 8382 RVA: 0x0008DAE4 File Offset: 0x0008BCE4
		private void OnDestroy()
		{
			UnityEngine.Object.Destroy(this.stateMachine);
		}

		// Token: 0x060020BF RID: 8383 RVA: 0x0008DAF1 File Offset: 0x0008BCF1
		private void OnEnable()
		{
			SteamworksLobbyManager.onLobbiesUpdated += this.OnLobbiesUpdated;
		}

		// Token: 0x060020C0 RID: 8384 RVA: 0x0008DB04 File Offset: 0x0008BD04
		private void OnDisable()
		{
			SteamworksLobbyManager.onLobbiesUpdated -= this.OnLobbiesUpdated;
		}

		// Token: 0x060020C1 RID: 8385 RVA: 0x0008DB18 File Offset: 0x0008BD18
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

		// Token: 0x060020C2 RID: 8386 RVA: 0x0008DB94 File Offset: 0x0008BD94
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

		// Token: 0x060020C3 RID: 8387 RVA: 0x0008DC28 File Offset: 0x0008BE28
		private void OnLobbiesUpdated()
		{
			SteamLobbyFinder.LobbyStateBase lobbyStateBase = this.stateMachine.state as SteamLobbyFinder.LobbyStateBase;
			if (lobbyStateBase == null)
			{
				return;
			}
			lobbyStateBase.OnLobbiesUpdated();
		}

		// Token: 0x060020C4 RID: 8388 RVA: 0x0008DC44 File Offset: 0x0008BE44
		private static bool CanJoinLobby(int currentLobbySize, LobbyList.Lobby lobby)
		{
			return currentLobbySize + SteamLobbyFinder.GetRealLobbyPlayerCount(lobby) <= lobby.MemberLimit;
		}

		// Token: 0x060020C5 RID: 8389 RVA: 0x0008DC5C File Offset: 0x0008BE5C
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

		// Token: 0x060020C6 RID: 8390 RVA: 0x0008DC93 File Offset: 0x0008BE93
		private static int GetCurrentLobbyRealPlayerCount()
		{
			return SteamworksLobbyManager.newestLobbyData.totalPlayerCount;
		}

		// Token: 0x17000371 RID: 881
		// (get) Token: 0x060020C7 RID: 8391 RVA: 0x0008DC9F File Offset: 0x0008BE9F
		// (set) Token: 0x060020C8 RID: 8392 RVA: 0x0008DCA6 File Offset: 0x0008BEA6
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

		// Token: 0x17000372 RID: 882
		// (get) Token: 0x060020C9 RID: 8393 RVA: 0x0008DCCA File Offset: 0x0008BECA
		// (set) Token: 0x060020CA RID: 8394 RVA: 0x0008DCD1 File Offset: 0x0008BED1
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

		// Token: 0x060020CB RID: 8395 RVA: 0x0008DCF5 File Offset: 0x0008BEF5
		[ConCommand(commandName = "steam_quickplay_start")]
		private static void CCSteamQuickplayStart(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			SteamLobbyFinder.userRequestedQuickplayQueue = true;
			SteamworksLobbyManager.SetLobbyQuickPlayQueuedIfOwner(true);
		}

		// Token: 0x060020CC RID: 8396 RVA: 0x0008DD08 File Offset: 0x0008BF08
		[ConCommand(commandName = "steam_quickplay_stop")]
		private static void CCSteamQuickplayStop(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			SteamLobbyFinder.userRequestedQuickplayQueue = false;
			SteamworksLobbyManager.CreateLobby();
		}

		// Token: 0x060020CD RID: 8397 RVA: 0x0008DD1C File Offset: 0x0008BF1C
		public static void Init()
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

		// Token: 0x060020CE RID: 8398 RVA: 0x0008DDA6 File Offset: 0x0008BFA6
		private static void OnLobbyDataUpdated()
		{
			SteamLobbyFinder.lobbyIsInQuickplayQueue = SteamworksLobbyManager.newestLobbyData.quickplayQueued;
		}

		// Token: 0x060020CF RID: 8399 RVA: 0x0008DDB8 File Offset: 0x0008BFB8
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

		// Token: 0x17000373 RID: 883
		// (get) Token: 0x060020D0 RID: 8400 RVA: 0x0008DFBD File Offset: 0x0008C1BD
		// (set) Token: 0x060020D1 RID: 8401 RVA: 0x0008DFCC File Offset: 0x0008C1CC
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

		// Token: 0x04001DEC RID: 7660
		private float age;

		// Token: 0x04001DED RID: 7661
		public float joinOnlyDuration = 5f;

		// Token: 0x04001DEE RID: 7662
		public float waitForFullDuration = 30f;

		// Token: 0x04001DEF RID: 7663
		public float startDelayDuration = 1f;

		// Token: 0x04001DF0 RID: 7664
		public float refreshInterval = 2f;

		// Token: 0x04001DF1 RID: 7665
		private float refreshTimer;

		// Token: 0x04001DF2 RID: 7666
		private EntityStateMachine stateMachine;

		// Token: 0x04001DF3 RID: 7667
		private static bool awaitingLobbyRefresh;

		// Token: 0x04001DF4 RID: 7668
		private static SteamLobbyFinder instance;

		// Token: 0x04001DF5 RID: 7669
		private static bool _userRequestedQuickplayQueue;

		// Token: 0x04001DF6 RID: 7670
		private static bool _lobbyIsInQuickplayQueue;

		// Token: 0x0200055C RID: 1372
		private class LobbyStateBase : BaseState
		{
			// Token: 0x060020D4 RID: 8404 RVA: 0x0008E067 File Offset: 0x0008C267
			public override void OnEnter()
			{
				base.OnEnter();
				this.lobbyFinder = base.GetComponent<SteamLobbyFinder>();
				SteamworksLobbyManager.onLobbyOwnershipGained += this.OnLobbyOwnershipGained;
				SteamworksLobbyManager.onLobbyOwnershipLost += this.OnLobbyOwnershipLost;
			}

			// Token: 0x060020D5 RID: 8405 RVA: 0x0008E09D File Offset: 0x0008C29D
			public override void OnExit()
			{
				SteamworksLobbyManager.onLobbyOwnershipGained -= this.OnLobbyOwnershipGained;
				SteamworksLobbyManager.onLobbyOwnershipLost -= this.OnLobbyOwnershipLost;
				base.OnExit();
			}

			// Token: 0x060020D6 RID: 8406 RVA: 0x0000409B File Offset: 0x0000229B
			public virtual void OnLobbiesUpdated()
			{
			}

			// Token: 0x060020D7 RID: 8407 RVA: 0x0008E0C7 File Offset: 0x0008C2C7
			private void OnLobbyOwnershipGained()
			{
				this.outer.SetNextState(new SteamLobbyFinder.LobbyStateStart());
			}

			// Token: 0x060020D8 RID: 8408 RVA: 0x0008E0D9 File Offset: 0x0008C2D9
			private void OnLobbyOwnershipLost()
			{
				this.outer.SetNextState(new SteamLobbyFinder.LobbyStateNonLeader());
			}

			// Token: 0x04001DF7 RID: 7671
			protected SteamLobbyFinder lobbyFinder;
		}

		// Token: 0x0200055D RID: 1373
		private class LobbyStateNonLeader : SteamLobbyFinder.LobbyStateBase
		{
			// Token: 0x060020DA RID: 8410 RVA: 0x0008E0EB File Offset: 0x0008C2EB
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

		// Token: 0x0200055E RID: 1374
		private class LobbyStateStart : SteamLobbyFinder.LobbyStateBase
		{
			// Token: 0x060020DC RID: 8412 RVA: 0x0008E12A File Offset: 0x0008C32A
			public override void Update()
			{
				base.Update();
				if (this.lobbyFinder.startDelayDuration <= base.age)
				{
					this.outer.SetNextState(SteamworksLobbyManager.hasMinimumPlayerCount ? new SteamLobbyFinder.LobbyStateMultiSearch() : new SteamLobbyFinder.LobbyStateSingleSearch());
				}
			}
		}

		// Token: 0x0200055F RID: 1375
		private class LobbyStateSingleSearch : SteamLobbyFinder.LobbyStateBase
		{
			// Token: 0x060020DE RID: 8414 RVA: 0x0008E164 File Offset: 0x0008C364
			public override void OnEnter()
			{
				base.OnEnter();
				SteamworksLobbyManager.SetLobbyQuickPlayCutoffTimeIfOwner(null);
			}

			// Token: 0x060020DF RID: 8415 RVA: 0x0008E188 File Offset: 0x0008C388
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

			// Token: 0x060020E0 RID: 8416 RVA: 0x0008E238 File Offset: 0x0008C438
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

		// Token: 0x02000560 RID: 1376
		private class LobbyStateMultiSearch : SteamLobbyFinder.LobbyStateBase
		{
			// Token: 0x060020E2 RID: 8418 RVA: 0x0008E344 File Offset: 0x0008C544
			public override void OnEnter()
			{
				base.OnEnter();
				SteamLobbyFinder.steamClient.Lobby.LobbyType = Lobby.Type.Public;
				SteamworksLobbyManager.SetLobbyQuickPlayCutoffTimeIfOwner(new uint?((uint)(Util.UnixTimeStampToDateTimeUtc(SteamLobbyFinder.steamClient.Utils.GetServerRealTime()) + TimeSpan.FromSeconds((double)this.lobbyFinder.waitForFullDuration) - Util.dateZero).TotalSeconds));
			}

			// Token: 0x060020E3 RID: 8419 RVA: 0x0008E3B0 File Offset: 0x0008C5B0
			public override void OnExit()
			{
				SteamworksLobbyManager.SetLobbyQuickPlayCutoffTimeIfOwner(null);
				base.OnExit();
			}

			// Token: 0x060020E4 RID: 8420 RVA: 0x0008E3D4 File Offset: 0x0008C5D4
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

		// Token: 0x02000561 RID: 1377
		private class LobbyStateBeginGame : SteamLobbyFinder.LobbyStateBase
		{
			// Token: 0x060020E6 RID: 8422 RVA: 0x0008E424 File Offset: 0x0008C624
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
