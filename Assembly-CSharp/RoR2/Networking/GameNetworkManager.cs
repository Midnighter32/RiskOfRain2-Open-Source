using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Reflection;
using System.Text;
using Facepunch.Steamworks;
using RoR2.ConVar;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace RoR2.Networking
{
	// Token: 0x02000578 RID: 1400
	public class GameNetworkManager : NetworkManager
	{
		// Token: 0x06001F2C RID: 7980 RVA: 0x00092F68 File Offset: 0x00091168
		static GameNetworkManager()
		{
			GameNetworkManager.loadingSceneAsyncFieldInfo = typeof(NetworkManager).GetField("s_LoadingSceneAsync", BindingFlags.Static | BindingFlags.NonPublic);
			if (GameNetworkManager.loadingSceneAsyncFieldInfo == null)
			{
				Debug.LogError("NetworkManager.s_LoadingSceneAsync field could not be found! Make sure to provide a proper implementation for this version of Unity.");
			}
			GameNetworkManager.StaticInit();
		}

		// Token: 0x06001F2D RID: 7981 RVA: 0x00093008 File Offset: 0x00091208
		private static void StaticInit()
		{
			GameNetworkManager.onStartServerGlobal += delegate()
			{
				if (!NetworkServer.dontListen)
				{
					GameNetworkManager.singleton.StartSteamworksServer();
				}
			};
			GameNetworkManager.onStopServerGlobal += delegate()
			{
				GameNetworkManager.singleton.StopSteamworksServer();
			};
		}

		// Token: 0x170002BB RID: 699
		// (get) Token: 0x06001F2E RID: 7982 RVA: 0x00093060 File Offset: 0x00091260
		private static bool isLoadingScene
		{
			get
			{
				AsyncOperation asyncOperation = (AsyncOperation)GameNetworkManager.loadingSceneAsyncFieldInfo.GetValue(null);
				return asyncOperation != null && !asyncOperation.isDone;
			}
		}

		// Token: 0x170002BC RID: 700
		// (get) Token: 0x06001F2F RID: 7983 RVA: 0x0009308C File Offset: 0x0009128C
		public new static GameNetworkManager singleton
		{
			get
			{
				return (GameNetworkManager)NetworkManager.singleton;
			}
		}

		// Token: 0x170002BD RID: 701
		// (get) Token: 0x06001F30 RID: 7984 RVA: 0x00093098 File Offset: 0x00091298
		// (set) Token: 0x06001F31 RID: 7985 RVA: 0x000930A0 File Offset: 0x000912A0
		public float unpredictedServerFixedTime { get; private set; }

		// Token: 0x170002BE RID: 702
		// (get) Token: 0x06001F32 RID: 7986 RVA: 0x000930A9 File Offset: 0x000912A9
		public float serverFixedTime
		{
			get
			{
				return this.unpredictedServerFixedTime + this.filteredClientRTT;
			}
		}

		// Token: 0x06001F33 RID: 7987 RVA: 0x000930B8 File Offset: 0x000912B8
		private static NetworkUser[] GetConnectionNetworkUsers(NetworkConnection conn)
		{
			List<PlayerController> playerControllers = conn.playerControllers;
			NetworkUser[] array = new NetworkUser[playerControllers.Count];
			for (int i = 0; i < playerControllers.Count; i++)
			{
				array[i] = playerControllers[i].gameObject.GetComponent<NetworkUser>();
			}
			return array;
		}

		// Token: 0x06001F34 RID: 7988 RVA: 0x000930FE File Offset: 0x000912FE
		private void Ping(NetworkConnection conn, int channelId)
		{
			conn.SendByChannel(65, new GameNetworkManager.PingMessage
			{
				timeStampMs = (uint)RoR2Application.instance.stopwatch.ElapsedMilliseconds
			}, channelId);
		}

		// Token: 0x06001F35 RID: 7989 RVA: 0x00093128 File Offset: 0x00091328
		protected void Start()
		{
			foreach (QosType value in base.channels)
			{
				base.connectionConfig.AddChannel(value);
			}
			base.connectionConfig.PacketSize = 1200;
			Client instance = Client.Instance;
			if (instance != null)
			{
				instance.Networking.OnP2PData = new Networking.OnRecievedP2PData(this.OnP2PData);
				for (int i = 0; i < base.connectionConfig.ChannelCount; i++)
				{
					instance.Networking.SetListenChannel(i, true);
				}
				instance.Networking.OnIncomingConnection = new Func<ulong, bool>(this.OnIncomingP2PConnection);
				instance.Networking.OnConnectionFailed = new Action<ulong, Networking.SessionError>(this.OnClientP2PConnectionFailed);
			}
			Action action = GameNetworkManager.onStartGlobal;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x1400004B RID: 75
		// (add) Token: 0x06001F36 RID: 7990 RVA: 0x00093214 File Offset: 0x00091414
		// (remove) Token: 0x06001F37 RID: 7991 RVA: 0x00093248 File Offset: 0x00091448
		public static event Action onStartGlobal;

		// Token: 0x06001F38 RID: 7992 RVA: 0x0009327B File Offset: 0x0009147B
		private void OnDestroy()
		{
			typeof(NetworkManager).GetMethod("OnDestroy", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(this, null);
		}

		// Token: 0x06001F39 RID: 7993 RVA: 0x0009329C File Offset: 0x0009149C
		protected void FixedUpdate()
		{
			if (NetworkServer.active || NetworkClient.active)
			{
				this.unpredictedServerFixedTime += Time.fixedDeltaTime;
			}
			this.FixedUpdateServer();
			this.FixedUpdateClient();
			this.debugServerTime = this.unpredictedServerFixedTime;
			this.debugRTT = this.clientRTT;
		}

		// Token: 0x06001F3A RID: 7994 RVA: 0x000932ED File Offset: 0x000914ED
		protected void Update()
		{
			this.EnsureDesiredHost();
			this.UpdateServer();
			this.UpdateClient();
		}

		// Token: 0x06001F3B RID: 7995 RVA: 0x00093304 File Offset: 0x00091504
		private void EnsureDesiredHost()
		{
			if (false | this.serverShuttingDown | this.clientIsConnecting | (NetworkServer.active && GameNetworkManager.isLoadingScene) | (!NetworkClient.active && GameNetworkManager.isLoadingScene))
			{
				return;
			}
			if (this.isNetworkActive && !this.actedUponDesiredHost && !this.desiredHost.DescribesCurrentHost())
			{
				this.Disconnect();
				return;
			}
			if (!this.actedUponDesiredHost)
			{
				if (this.desiredHost.hostType == GameNetworkManager.HostDescription.HostType.Self)
				{
					if (NetworkServer.active)
					{
						return;
					}
					this.actedUponDesiredHost = true;
					base.maxConnections = this.desiredHost.hostingParameters.maxPlayers;
					NetworkServer.dontListen = !this.desiredHost.hostingParameters.listen;
					this.StartHost();
				}
				if (this.desiredHost.hostType == GameNetworkManager.HostDescription.HostType.Steam && Time.unscaledTime - this.lastDesiredHostSetTime >= 1f)
				{
					this.actedUponDesiredHost = true;
					this.StartClientSteam(this.desiredHost.steamId);
				}
				if (this.desiredHost.hostType == GameNetworkManager.HostDescription.HostType.IPv4 && Time.unscaledTime - this.lastDesiredHostSetTime >= 1f)
				{
					this.actedUponDesiredHost = true;
					Debug.LogFormat("Attempting connection. ip={0} port={1}", new object[]
					{
						this.desiredHost.addressPortPair.address,
						this.desiredHost.addressPortPair.port
					});
					GameNetworkManager.singleton.networkAddress = this.desiredHost.addressPortPair.address;
					GameNetworkManager.singleton.networkPort = (int)this.desiredHost.addressPortPair.port;
					GameNetworkManager.singleton.StartClient();
				}
			}
		}

		// Token: 0x170002BF RID: 703
		// (get) Token: 0x06001F3C RID: 7996 RVA: 0x000934A8 File Offset: 0x000916A8
		// (set) Token: 0x06001F3D RID: 7997 RVA: 0x000934B0 File Offset: 0x000916B0
		public GameNetworkManager.HostDescription desiredHost
		{
			get
			{
				return this._desiredHost;
			}
			set
			{
				if (this._desiredHost.Equals(value))
				{
					return;
				}
				this._desiredHost = value;
				this.actedUponDesiredHost = false;
				this.lastDesiredHostSetTime = Time.unscaledTime;
				Debug.LogFormat("GameNetworkManager.desiredHost={0}", new object[]
				{
					this._desiredHost.ToString()
				});
			}
		}

		// Token: 0x06001F3E RID: 7998 RVA: 0x0009350C File Offset: 0x0009170C
		public void ForceCloseAllConnections()
		{
			Client instance = Client.Instance;
			Networking networking = (instance != null) ? instance.Networking : null;
			if (networking == null)
			{
				return;
			}
			using (IEnumerator<NetworkConnection> enumerator = NetworkServer.connections.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SteamNetworkConnection steamNetworkConnection;
					if ((steamNetworkConnection = (enumerator.Current as SteamNetworkConnection)) != null)
					{
						networking.CloseSession(steamNetworkConnection.steamId.value);
					}
				}
			}
			NetworkClient client = this.client;
			SteamNetworkConnection steamNetworkConnection2;
			if ((steamNetworkConnection2 = (((client != null) ? client.connection : null) as SteamNetworkConnection)) != null)
			{
				networking.CloseSession(steamNetworkConnection2.steamId.value);
			}
		}

		// Token: 0x170002C0 RID: 704
		// (get) Token: 0x06001F3F RID: 7999 RVA: 0x000935B0 File Offset: 0x000917B0
		private bool clientIsConnecting
		{
			get
			{
				NetworkClient client = this.client;
				return ((client != null) ? client.connection : null) != null && !this.client.isConnected;
			}
		}

		// Token: 0x1400004C RID: 76
		// (add) Token: 0x06001F40 RID: 8000 RVA: 0x000935D8 File Offset: 0x000917D8
		// (remove) Token: 0x06001F41 RID: 8001 RVA: 0x0009360C File Offset: 0x0009180C
		public static event Action<NetworkClient> onStartClientGlobal;

		// Token: 0x1400004D RID: 77
		// (add) Token: 0x06001F42 RID: 8002 RVA: 0x00093640 File Offset: 0x00091840
		// (remove) Token: 0x06001F43 RID: 8003 RVA: 0x00093674 File Offset: 0x00091874
		public static event Action onStopClientGlobal;

		// Token: 0x1400004E RID: 78
		// (add) Token: 0x06001F44 RID: 8004 RVA: 0x000936A8 File Offset: 0x000918A8
		// (remove) Token: 0x06001F45 RID: 8005 RVA: 0x000936DC File Offset: 0x000918DC
		public static event Action<NetworkConnection> onClientConnectGlobal;

		// Token: 0x1400004F RID: 79
		// (add) Token: 0x06001F46 RID: 8006 RVA: 0x00093710 File Offset: 0x00091910
		// (remove) Token: 0x06001F47 RID: 8007 RVA: 0x00093744 File Offset: 0x00091944
		public static event Action<NetworkConnection> onClientDisconnectGlobal;

		// Token: 0x06001F48 RID: 8008 RVA: 0x00093778 File Offset: 0x00091978
		public override void OnStartClient(NetworkClient newClient)
		{
			base.OnStartClient(newClient);
			foreach (string str in GameNetworkManager.spawnableFolders)
			{
				GameObject[] array2 = Resources.LoadAll<GameObject>("Prefabs/" + str + "/");
				for (int j = 0; j < array2.Length; j++)
				{
					ClientScene.RegisterPrefab(array2[j]);
				}
			}
			ClientScene.RegisterPrefab(Resources.Load<GameObject>("Prefabs/NetworkSession"));
			ClientScene.RegisterPrefab(Resources.Load<GameObject>("Prefabs/Stage"));
			NetworkMessageHandlerAttribute.RegisterClientMessages(newClient);
			Action<NetworkClient> action = GameNetworkManager.onStartClientGlobal;
			if (action == null)
			{
				return;
			}
			action(newClient);
		}

		// Token: 0x06001F49 RID: 8009 RVA: 0x0009380C File Offset: 0x00091A0C
		public override void OnStopClient()
		{
			this.ForceCloseAllConnections();
			if (this.actedUponDesiredHost)
			{
				GameNetworkManager.singleton.desiredHost = default(GameNetworkManager.HostDescription);
			}
			Action action = GameNetworkManager.onStopClientGlobal;
			if (action != null)
			{
				action();
			}
			base.OnStopClient();
		}

		// Token: 0x06001F4A RID: 8010 RVA: 0x00093850 File Offset: 0x00091A50
		public override void OnClientConnect(NetworkConnection conn)
		{
			base.OnClientConnect(conn);
			this.clientRTT = 0f;
			this.filteredClientRTT = 0f;
			this.ClientSetPlayers(conn);
			Action<NetworkConnection> action = GameNetworkManager.onClientConnectGlobal;
			if (action == null)
			{
				return;
			}
			action(conn);
		}

		// Token: 0x06001F4B RID: 8011 RVA: 0x00093888 File Offset: 0x00091A88
		public override void OnClientDisconnect(NetworkConnection conn)
		{
			SteamNetworkConnection steamNetworkConnection;
			if ((steamNetworkConnection = (conn as SteamNetworkConnection)) != null)
			{
				Debug.LogFormat("Closing connection with steamId {0}", new object[]
				{
					steamNetworkConnection.steamId
				});
			}
			base.OnClientDisconnect(conn);
			Action<NetworkConnection> action = GameNetworkManager.onClientDisconnectGlobal;
			if (action == null)
			{
				return;
			}
			action(conn);
		}

		// Token: 0x06001F4C RID: 8012 RVA: 0x000938D4 File Offset: 0x00091AD4
		public void ClientAddPlayer(short playerControllerId, NetworkConnection connection = null)
		{
			foreach (PlayerController playerController in ClientScene.localPlayers)
			{
				if (playerController.playerControllerId == playerControllerId && playerController.IsValid && playerController.gameObject)
				{
					Debug.LogFormat("Player {0} already added, aborting.", new object[]
					{
						playerControllerId
					});
					return;
				}
			}
			Debug.LogFormat("Adding local player controller {0} on connection {1}", new object[]
			{
				playerControllerId,
				connection
			});
			GameNetworkManager.AddPlayerMessage extraMessage;
			if (RoR2Application.instance.steamworksClient != null)
			{
				extraMessage = new GameNetworkManager.AddPlayerMessage
				{
					steamId = RoR2Application.instance.steamworksClient.SteamId,
					steamAuthTicketData = RoR2Application.instance.steamworksAuthTicket.Data
				};
			}
			else
			{
				extraMessage = new GameNetworkManager.AddPlayerMessage
				{
					steamId = 0UL,
					steamAuthTicketData = Array.Empty<byte>()
				};
			}
			ClientScene.AddPlayer(connection, playerControllerId, extraMessage);
		}

		// Token: 0x06001F4D RID: 8013 RVA: 0x000939D8 File Offset: 0x00091BD8
		private void UpdateClient()
		{
			NetworkClient client = this.client;
			if (((client != null) ? client.connection : null) is SteamNetworkConnection)
			{
				Networking.P2PSessionState p2PSessionState = default(Networking.P2PSessionState);
				if (Client.Instance.Networking.GetP2PSessionState(((SteamNetworkConnection)this.client.connection).steamId.value, ref p2PSessionState) && p2PSessionState.Connecting == 0 && p2PSessionState.ConnectionActive == 0)
				{
					base.StopClient();
				}
			}
			bool flag = (this.client != null && !ClientScene.ready) || GameNetworkManager.isLoadingScene;
			if (GameNetworkManager.wasFading != flag)
			{
				if (flag)
				{
					FadeToBlackManager.fadeCount++;
				}
				else
				{
					FadeToBlackManager.fadeCount--;
				}
				GameNetworkManager.wasFading = flag;
			}
		}

		// Token: 0x170002C1 RID: 705
		// (get) Token: 0x06001F4E RID: 8014 RVA: 0x00093A8C File Offset: 0x00091C8C
		// (set) Token: 0x06001F4F RID: 8015 RVA: 0x00093A94 File Offset: 0x00091C94
		public float clientRTT { get; private set; }

		// Token: 0x170002C2 RID: 706
		// (get) Token: 0x06001F50 RID: 8016 RVA: 0x00093A9D File Offset: 0x00091C9D
		// (set) Token: 0x06001F51 RID: 8017 RVA: 0x00093AA5 File Offset: 0x00091CA5
		public float filteredClientRTT { get; private set; }

		// Token: 0x06001F52 RID: 8018 RVA: 0x00093AB0 File Offset: 0x00091CB0
		private void FixedUpdateClient()
		{
			if (!NetworkClient.active || this.client == null)
			{
				return;
			}
			SteamNetworkConnection steamNetworkConnection;
			if ((steamNetworkConnection = (this.client.connection as SteamNetworkConnection)) != null)
			{
				this.clientRTT = steamNetworkConnection.rtt * 0.001f;
			}
			else
			{
				this.clientRTT = (float)this.client.GetRTT() * 0.001f;
			}
			if (this.filteredClientRTT == 0f)
			{
				this.filteredClientRTT = this.clientRTT;
			}
			else
			{
				this.filteredClientRTT = Mathf.SmoothDamp(this.filteredClientRTT, this.clientRTT, ref this.rttVelocity, this.filteredRTTSmoothDuration, 100f, Time.fixedDeltaTime);
			}
			int i = 0;
			int count = NetworkClient.allClients.Count;
			while (i < count)
			{
				NetworkConnection connection = NetworkClient.allClients[i].connection;
				if (!Util.ConnectionIsLocal(connection))
				{
					this.Ping(connection, QosChannelIndex.ping.intVal);
				}
				i++;
			}
		}

		// Token: 0x06001F53 RID: 8019 RVA: 0x00093B98 File Offset: 0x00091D98
		public override void OnClientSceneChanged(NetworkConnection conn)
		{
			base.autoCreatePlayer = false;
			base.OnClientSceneChanged(conn);
			this.ClientSetPlayers(conn);
		}

		// Token: 0x06001F54 RID: 8020 RVA: 0x00093BB0 File Offset: 0x00091DB0
		private void ClientSetPlayers(NetworkConnection conn)
		{
			ReadOnlyCollection<LocalUser> readOnlyLocalUsersList = LocalUserManager.readOnlyLocalUsersList;
			for (int i = 0; i < readOnlyLocalUsersList.Count; i++)
			{
				this.ClientAddPlayer((short)readOnlyLocalUsersList[i].id, conn);
			}
		}

		// Token: 0x06001F55 RID: 8021 RVA: 0x00093BE8 File Offset: 0x00091DE8
		private void StartClientSteam(CSteamID serverId)
		{
			if (!NetworkServer.active)
			{
				NetworkManager.networkSceneName = "";
			}
			string text = "";
			if (this.isNetworkActive)
			{
				text += "isNetworkActive ";
			}
			if (NetworkClient.active)
			{
				text += "NetworkClient.active ";
			}
			if (NetworkServer.active)
			{
				text += "NetworkClient.active ";
			}
			if (GameNetworkManager.isLoadingScene)
			{
				text += "isLoadingScene ";
			}
			if (text != "")
			{
				Debug.Log(text);
				RoR2Application.onNextUpdate += delegate()
				{
				};
			}
			SteamNetworkClient steamNetworkClient = new SteamNetworkClient(new SteamNetworkConnection(serverId));
			steamNetworkClient.Configure(base.connectionConfig, 1);
			base.UseExternalClient(steamNetworkClient);
			steamNetworkClient.Connect();
			Debug.LogFormat("Initiating connection to server {0}...", new object[]
			{
				serverId.value
			});
			if (!Client.Instance.Networking.SendP2PPacket(serverId.value, null, 0, Networking.SendType.Reliable, 0))
			{
				Debug.LogFormat("Failed to send connection request to server {0}.", new object[]
				{
					serverId.value
				});
			}
		}

		// Token: 0x06001F56 RID: 8022 RVA: 0x00093D10 File Offset: 0x00091F10
		public bool IsConnectedToServer(CSteamID serverSteamId)
		{
			if (this.client == null || !this.client.connection.isConnected || Client.Instance == null)
			{
				return false;
			}
			SteamNetworkConnection steamNetworkConnection;
			if ((steamNetworkConnection = (this.client.connection as SteamNetworkConnection)) != null)
			{
				return steamNetworkConnection.steamId == serverSteamId;
			}
			return this.client.connection.address == "localServer" && serverSteamId.value == Client.Instance.SteamId;
		}

		// Token: 0x06001F57 RID: 8023 RVA: 0x00093D91 File Offset: 0x00091F91
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void ClientInit()
		{
			SceneCatalog.onMostRecentSceneDefChanged += GameNetworkManager.ClientUpdateOfflineScene;
		}

		// Token: 0x06001F58 RID: 8024 RVA: 0x00093DA4 File Offset: 0x00091FA4
		private static void ClientUpdateOfflineScene(SceneDef sceneDef)
		{
			if (GameNetworkManager.singleton && sceneDef.isOfflineScene)
			{
				GameNetworkManager.singleton.offlineScene = sceneDef.sceneName;
			}
		}

		// Token: 0x06001F59 RID: 8025 RVA: 0x00093DCA File Offset: 0x00091FCA
		private static void EnsureNetworkManagerNotBusy()
		{
			if (!GameNetworkManager.singleton)
			{
				return;
			}
			if (GameNetworkManager.singleton.serverShuttingDown || GameNetworkManager.isLoadingScene)
			{
				throw new ConCommandException("NetworkManager is busy and cannot receive commands.");
			}
		}

		// Token: 0x06001F5A RID: 8026 RVA: 0x00093DF8 File Offset: 0x00091FF8
		[ConCommand(commandName = "client_set_players", flags = ConVarFlags.None, helpText = "Adds network players for all local players. Debug only.")]
		private static void CCClientSetPlayers(ConCommandArgs args)
		{
			if (GameNetworkManager.singleton && GameNetworkManager.singleton.client != null && GameNetworkManager.singleton.client.connection != null)
			{
				GameNetworkManager.singleton.ClientSetPlayers(GameNetworkManager.singleton.client.connection);
			}
		}

		// Token: 0x06001F5B RID: 8027 RVA: 0x00093E48 File Offset: 0x00092048
		[ConCommand(commandName = "ping", flags = ConVarFlags.None, helpText = "Prints the current round trip time from this client to the server and back.")]
		private static void CCPing(ConCommandArgs args)
		{
			if (GameNetworkManager.singleton && GameNetworkManager.singleton.client != null && GameNetworkManager.singleton.client.connection != null)
			{
				uint rtt;
				if (GameNetworkManager.singleton.client.connection is SteamNetworkConnection)
				{
					rtt = ((SteamNetworkConnection)GameNetworkManager.singleton.client.connection).rtt;
				}
				else
				{
					rtt = (uint)GameNetworkManager.singleton.client.GetRTT();
				}
				Debug.LogFormat("rtt={0}ms", new object[]
				{
					rtt
				});
			}
		}

		// Token: 0x06001F5C RID: 8028 RVA: 0x00093EDC File Offset: 0x000920DC
		[ConCommand(commandName = "set_scene", flags = ConVarFlags.None, helpText = "Changes to the named scene.")]
		private static void CCSetScene(ConCommandArgs args)
		{
			args.CheckArgumentCount(1);
			int boolValue = RoR2Application.cvCheats.boolValue ? 1 : 0;
			bool flag = !GameNetworkManager.singleton || GameNetworkManager.singleton.isNetworkActive;
			if (boolValue == 0 && flag && Array.IndexOf<string>(GameNetworkManager.sceneWhiteList, args[0]) == -1)
			{
				return;
			}
			if (!GameNetworkManager.singleton)
			{
				throw new ConCommandException("set_scene failed: GameNetworkManager is not available.");
			}
			if (NetworkServer.active)
			{
				Debug.LogFormat("Setting server scene to {0}", new object[]
				{
					args[0]
				});
				GameNetworkManager.singleton.ServerChangeScene(args[0]);
				return;
			}
			if (!NetworkClient.active)
			{
				Debug.LogFormat("Setting offline scene to {0}", new object[]
				{
					args[0]
				});
				SceneManager.LoadScene(args[0], LoadSceneMode.Single);
				return;
			}
			throw new ConCommandException("Cannot change scene while connected to a remote server.");
		}

		// Token: 0x06001F5D RID: 8029 RVA: 0x00093FBC File Offset: 0x000921BC
		[ConCommand(commandName = "scene_list", flags = ConVarFlags.None, helpText = "Prints a list of all available scene names.")]
		private static void CCSceneList(ConCommandArgs args)
		{
			string[] array = new string[SceneManager.sceneCountInBuildSettings];
			for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
			{
				array[i] = string.Format("[{0}]={1}", i, SceneUtility.GetScenePathByBuildIndex(i));
			}
			Debug.Log(string.Join("\n", array));
		}

		// Token: 0x06001F5E RID: 8030 RVA: 0x00094010 File Offset: 0x00092210
		[ConCommand(commandName = "dump_network_ids", flags = ConVarFlags.None, helpText = "Lists the network ids of all currently networked game objects.")]
		private static void CCDumpNetworkIDs(ConCommandArgs args)
		{
			NetworkIdentity[] array = UnityEngine.Object.FindObjectsOfType<NetworkIdentity>();
			for (int i = 0; i < array.Length; i++)
			{
				Debug.LogFormat("{0}={1}", new object[]
				{
					array[i].netId.Value,
					array[i].gameObject.name
				});
			}
		}

		// Token: 0x06001F5F RID: 8031 RVA: 0x0009406C File Offset: 0x0009226C
		[ConCommand(commandName = "disconnect", flags = ConVarFlags.None, helpText = "Disconnect from a server or shut down the current server.")]
		private static void CCDisconnect(ConCommandArgs args)
		{
			GameNetworkManager.singleton.desiredHost = default(GameNetworkManager.HostDescription);
		}

		// Token: 0x06001F60 RID: 8032 RVA: 0x0009408C File Offset: 0x0009228C
		private void Disconnect()
		{
			if (this.serverShuttingDown)
			{
				return;
			}
			if (GameNetworkManager.singleton.isNetworkActive)
			{
				Debug.Log("Network shutting down...");
				if (NetworkServer.active)
				{
					GameNetworkManager.singleton.RequestServerShutdown();
					return;
				}
				GameNetworkManager.singleton.StopClient();
			}
		}

		// Token: 0x06001F61 RID: 8033 RVA: 0x000940CC File Offset: 0x000922CC
		[ConCommand(commandName = "connect", flags = ConVarFlags.None, helpText = "Connect to a server.")]
		private static void CCConnect(ConCommandArgs args)
		{
			args.CheckArgumentCount(1);
			if (!GameNetworkManager.singleton)
			{
				return;
			}
			GameNetworkManager.EnsureNetworkManagerNotBusy();
			string[] array = args[0].Split(new char[]
			{
				':'
			});
			string address = array[0];
			ushort port = 7777;
			if (array.Length > 1)
			{
				TextSerialization.TryParseInvariant(array[1], out port);
			}
			GameNetworkManager.singleton.desiredHost = new GameNetworkManager.HostDescription(new AddressPortPair(address, port));
		}

		// Token: 0x06001F62 RID: 8034 RVA: 0x00094140 File Offset: 0x00092340
		[ConCommand(commandName = "connect_steamworks_p2p", flags = ConVarFlags.None, helpText = "Connect to a server using Steamworks P2P. Argument is the 64-bit Steam ID of the server to connect to.")]
		private static void CCConnectSteamworksP2P(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			args.CheckArgumentCount(1);
			CSteamID csteamID;
			if (!CSteamID.TryParse(args[0], out csteamID))
			{
				throw new ConCommandException("Could not parse server id.");
			}
			if (Client.Instance.Lobby.IsValid && !SteamworksLobbyManager.ownsLobby && csteamID != SteamworksLobbyManager.serverId)
			{
				Debug.LogFormat("Cannot connect to server {0}: Server is not the one specified by the current steam lobby.", new object[]
				{
					csteamID
				});
				return;
			}
			if (Client.Instance.SteamId == csteamID.value)
			{
				return;
			}
			GameNetworkManager.singleton.desiredHost = new GameNetworkManager.HostDescription(csteamID);
		}

		// Token: 0x06001F63 RID: 8035 RVA: 0x000941D8 File Offset: 0x000923D8
		[ConCommand(commandName = "host", flags = ConVarFlags.None, helpText = "Host a server. First argument is whether or not to listen for incoming connections.")]
		private static void CCHost(ConCommandArgs args)
		{
			if (!GameNetworkManager.singleton)
			{
				return;
			}
			args.CheckArgumentCount(1);
			if (SteamworksLobbyManager.isInLobby && !SteamworksLobbyManager.ownsLobby)
			{
				return;
			}
			bool flag = false;
			if (NetworkServer.active)
			{
				Debug.Log("Server already running.");
				flag = true;
			}
			if (!flag)
			{
				int maxPlayers = GameNetworkManager.SvMaxPlayersConVar.instance.intValue;
				if (SteamworksLobbyManager.isInLobby)
				{
					maxPlayers = SteamworksLobbyManager.newestLobbyData.totalMaxPlayers;
				}
				GameNetworkManager.singleton.desiredHost = new GameNetworkManager.HostDescription(new GameNetworkManager.HostDescription.HostingParameters
				{
					listen = args.GetArgBool(0),
					maxPlayers = maxPlayers
				});
			}
		}

		// Token: 0x06001F64 RID: 8036 RVA: 0x00094270 File Offset: 0x00092470
		[ConCommand(commandName = "steam_get_p2p_session_state")]
		private static void CCSteamGetP2PSessionState(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			args.CheckArgumentCount(1);
			if (!GameNetworkManager.singleton)
			{
				return;
			}
			CSteamID csteamID;
			if (!CSteamID.TryParse(args[0], out csteamID))
			{
				throw new ConCommandException("Could not parse steam id.");
			}
			Networking.P2PSessionState p2PSessionState = default(Networking.P2PSessionState);
			if (Client.Instance.Networking.GetP2PSessionState(csteamID.value, ref p2PSessionState))
			{
				Debug.LogFormat("ConnectionActive={0}\nConnecting={1}\nP2PSessionError={2}\nUsingRelay={3}\nBytesQueuedForSend={4}\nPacketsQueuedForSend={5}\nRemoteIP={6}\nRemotePort={7}", new object[]
				{
					p2PSessionState.ConnectionActive,
					p2PSessionState.Connecting,
					p2PSessionState.P2PSessionError,
					p2PSessionState.UsingRelay,
					p2PSessionState.BytesQueuedForSend,
					p2PSessionState.PacketsQueuedForSend,
					p2PSessionState.RemoteIP,
					p2PSessionState.RemotePort
				});
				return;
			}
			Debug.LogFormat("Could not get p2p session info for steamId={0}", new object[]
			{
				csteamID
			});
		}

		// Token: 0x06001F65 RID: 8037 RVA: 0x00094374 File Offset: 0x00092574
		[ConCommand(commandName = "steam_server_print_info")]
		private static void CCSteamServerPrintInfo(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			if (!GameNetworkManager.singleton)
			{
				return;
			}
			if (GameNetworkManager.singleton.steamworksServer == null)
			{
				throw new ConCommandException("No steamworks server.");
			}
			Debug.Log("" + string.Format("IsValid={0}\n", GameNetworkManager.singleton.steamworksServer.IsValid) + string.Format("Product={0}\n", GameNetworkManager.singleton.steamworksServer.Product) + string.Format("ModDir={0}\n", GameNetworkManager.singleton.steamworksServer.ModDir) + string.Format("SteamId={0}\n", GameNetworkManager.singleton.steamworksServer.SteamId) + string.Format("DedicatedServer={0}\n", GameNetworkManager.singleton.steamworksServer.DedicatedServer) + string.Format("LoggedOn={0}\n", GameNetworkManager.singleton.steamworksServer.LoggedOn) + string.Format("ServerName={0}\n", GameNetworkManager.singleton.steamworksServer.ServerName) + string.Format("PublicIp={0}\n", GameNetworkManager.singleton.steamworksServer.PublicIp) + string.Format("Passworded={0}\n", GameNetworkManager.singleton.steamworksServer.Passworded) + string.Format("MaxPlayers={0}\n", GameNetworkManager.singleton.steamworksServer.MaxPlayers) + string.Format("BotCount={0}\n", GameNetworkManager.singleton.steamworksServer.BotCount) + string.Format("MapName={0}\n", GameNetworkManager.singleton.steamworksServer.MapName) + string.Format("GameDescription={0}\n", GameNetworkManager.singleton.steamworksServer.GameDescription) + string.Format("GameTags={0}\n", GameNetworkManager.singleton.steamworksServer.GameTags));
		}

		// Token: 0x06001F66 RID: 8038 RVA: 0x0009457C File Offset: 0x0009277C
		[ConCommand(commandName = "kick_steam", flags = ConVarFlags.SenderMustBeServer, helpText = "Kicks the user with the specified steam id from the server.")]
		private static void CCKickSteam(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			args.CheckArgumentCount(1);
			CSteamID steamId;
			if (CSteamID.TryParse(args[0], out steamId))
			{
				NetworkConnection client = GameNetworkManager.singleton.GetClient(steamId);
				if (client != null)
				{
					GameNetworkManager.singleton.ServerKickClient(client, GameNetworkManager.KickReason.Kick);
				}
			}
		}

		// Token: 0x06001F67 RID: 8039 RVA: 0x000945C4 File Offset: 0x000927C4
		[ConCommand(commandName = "ban_steam", flags = ConVarFlags.SenderMustBeServer, helpText = "Bans the user with the specified steam id from the server.")]
		private static void CCBanSteam(ConCommandArgs args)
		{
			ConCommandException.CheckSteamworks();
			args.CheckArgumentCount(1);
			CSteamID steamId;
			if (CSteamID.TryParse(args[0], out steamId))
			{
				NetworkConnection client = GameNetworkManager.singleton.GetClient(steamId);
				if (client != null)
				{
					GameNetworkManager.singleton.ServerKickClient(client, GameNetworkManager.KickReason.Ban);
				}
			}
		}

		// Token: 0x170002C3 RID: 707
		// (get) Token: 0x06001F68 RID: 8040 RVA: 0x0009460A File Offset: 0x0009280A
		// (set) Token: 0x06001F69 RID: 8041 RVA: 0x00094612 File Offset: 0x00092812
		public bool isHost { get; private set; }

		// Token: 0x14000050 RID: 80
		// (add) Token: 0x06001F6A RID: 8042 RVA: 0x0009461C File Offset: 0x0009281C
		// (remove) Token: 0x06001F6B RID: 8043 RVA: 0x00094650 File Offset: 0x00092850
		public static event Action onStartHostGlobal;

		// Token: 0x14000051 RID: 81
		// (add) Token: 0x06001F6C RID: 8044 RVA: 0x00094684 File Offset: 0x00092884
		// (remove) Token: 0x06001F6D RID: 8045 RVA: 0x000946B8 File Offset: 0x000928B8
		public static event Action onStopHostGlobal;

		// Token: 0x06001F6E RID: 8046 RVA: 0x000946EB File Offset: 0x000928EB
		public override void OnStartHost()
		{
			base.OnStartHost();
			this.isHost = true;
			Action action = GameNetworkManager.onStartHostGlobal;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06001F6F RID: 8047 RVA: 0x00094709 File Offset: 0x00092909
		public override void OnStopHost()
		{
			Action action = GameNetworkManager.onStopHostGlobal;
			if (action != null)
			{
				action();
			}
			this.isHost = false;
			base.OnStopHost();
		}

		// Token: 0x06001F70 RID: 8048 RVA: 0x00094728 File Offset: 0x00092928
		[NetworkMessageHandler(client = true, server = false, msgType = 67)]
		private static void HandleKick(NetworkMessage netMsg)
		{
			GameNetworkManager.KickMessage kickMessage = netMsg.ReadMessage<GameNetworkManager.KickMessage>();
			Debug.LogFormat("Received kick message. Reason={0}", new object[]
			{
				kickMessage.reason
			});
			GameNetworkManager.singleton.StopClient();
			string displayToken = kickMessage.GetDisplayToken();
			SimpleDialogBox simpleDialogBox = SimpleDialogBox.Create(null);
			simpleDialogBox.headerToken = new SimpleDialogBox.TokenParamsPair("DISCONNECTED", Array.Empty<object>());
			simpleDialogBox.descriptionToken = new SimpleDialogBox.TokenParamsPair(displayToken, Array.Empty<object>());
			simpleDialogBox.AddCancelButton(CommonLanguageTokens.ok, Array.Empty<object>());
			simpleDialogBox.rootObject.transform.SetParent(RoR2Application.instance.mainCanvas.transform);
		}

		// Token: 0x06001F71 RID: 8049 RVA: 0x000947C8 File Offset: 0x000929C8
		[NetworkMessageHandler(msgType = 54, client = true)]
		private static void HandleUpdateTime(NetworkMessage netMsg)
		{
			float unpredictedServerFixedTime = netMsg.reader.ReadSingle();
			GameNetworkManager.singleton.unpredictedServerFixedTime = unpredictedServerFixedTime;
		}

		// Token: 0x06001F72 RID: 8050 RVA: 0x000947EC File Offset: 0x000929EC
		[NetworkMessageHandler(msgType = 64, client = true, server = true)]
		private static void HandleTest(NetworkMessage netMsg)
		{
			int num = netMsg.reader.ReadInt32();
			Debug.LogFormat("Received test packet. value={0}", new object[]
			{
				num
			});
		}

		// Token: 0x06001F73 RID: 8051 RVA: 0x00094820 File Offset: 0x00092A20
		[NetworkMessageHandler(msgType = 65, client = true, server = true)]
		private static void HandlePing(NetworkMessage netMsg)
		{
			NetworkReader reader = netMsg.reader;
			netMsg.conn.SendByChannel(66, reader.ReadMessage<GameNetworkManager.PingMessage>(), netMsg.channelId);
		}

		// Token: 0x06001F74 RID: 8052 RVA: 0x00094850 File Offset: 0x00092A50
		[NetworkMessageHandler(msgType = 66, client = true, server = true)]
		private static void HandlePingResponse(NetworkMessage netMsg)
		{
			uint timeStampMs = netMsg.reader.ReadMessage<GameNetworkManager.PingMessage>().timeStampMs;
			uint rtt = (uint)RoR2Application.instance.stopwatch.ElapsedMilliseconds - timeStampMs;
			SteamNetworkConnection steamNetworkConnection;
			if ((steamNetworkConnection = (netMsg.conn as SteamNetworkConnection)) != null)
			{
				steamNetworkConnection.rtt = rtt;
			}
		}

		// Token: 0x06001F75 RID: 8053 RVA: 0x00094897 File Offset: 0x00092A97
		public static bool IsMemberInSteamLobby(CSteamID steamId)
		{
			return Client.Instance.Lobby.UserIsInCurrentLobby(steamId.value);
		}

		// Token: 0x06001F76 RID: 8054 RVA: 0x000948B0 File Offset: 0x00092AB0
		private void OnP2PData(ulong steamId, byte[] data, int dataLength, int channel)
		{
			CSteamID steamId2 = new CSteamID(steamId);
			if (SteamNetworkConnection.cvNetP2PDebugTransport.value)
			{
				Debug.LogFormat("Received packet from {0} dataLength={1} channel={2}", new object[]
				{
					steamId,
					dataLength,
					channel
				});
			}
			NetworkConnection networkConnection;
			if (NetworkServer.active)
			{
				networkConnection = this.GetClient(steamId2);
			}
			else
			{
				NetworkClient client = this.client;
				networkConnection = ((client != null) ? client.connection : null);
			}
			if (networkConnection != null)
			{
				networkConnection.TransportReceive(data, dataLength, 0);
			}
		}

		// Token: 0x06001F77 RID: 8055 RVA: 0x00094930 File Offset: 0x00092B30
		public CSteamID GetSteamIDForConnection(NetworkConnection conn)
		{
			if (this.client != null && conn == this.client.connection)
			{
				return new CSteamID(Client.Instance.SteamId);
			}
			ReadOnlyCollection<NetworkConnection> connections = NetworkServer.connections;
			for (int i = 0; i < connections.Count; i++)
			{
				if (connections[i] == conn)
				{
					SteamNetworkConnection steamNetworkConnection;
					if ((steamNetworkConnection = (connections[i] as SteamNetworkConnection)) != null)
					{
						return steamNetworkConnection.steamId;
					}
					Debug.LogError("Client is not a SteamNetworkConnection");
				}
			}
			Debug.LogError("Could not find Steam ID");
			return CSteamID.nil;
		}

		// Token: 0x06001F78 RID: 8056 RVA: 0x000949B4 File Offset: 0x00092BB4
		private bool OnIncomingP2PConnection(ulong steamId)
		{
			bool flag = false;
			if (NetworkServer.active)
			{
				flag = (!NetworkServer.dontListen && !this.steamIdBanList.Contains(steamId) && !this.IsServerAtMaxConnections());
			}
			else if (this.client is SteamNetworkClient && ((SteamNetworkClient)this.client).steamConnection.steamId.value == steamId)
			{
				flag = true;
			}
			Debug.LogFormat("Incoming Steamworks connection from Steam ID {0}: {1}", new object[]
			{
				steamId,
				flag ? "accepted" : "rejected"
			});
			if (flag)
			{
				this.CreateP2PConnectionWithPeer(new CSteamID(steamId));
			}
			return flag;
		}

		// Token: 0x06001F79 RID: 8057 RVA: 0x00094A58 File Offset: 0x00092C58
		private void OnServerP2PConnectionFailed(ulong steamId, Networking.SessionError sessionError)
		{
			Debug.LogFormat("GameNetworkManager.OnServerP2PConnectionFailed steamId={0} sessionError={1}", new object[]
			{
				steamId,
				sessionError
			});
			ReadOnlyCollection<NetworkConnection> connections = NetworkServer.connections;
			for (int i = connections.Count - 1; i >= 0; i--)
			{
				SteamNetworkConnection steamNetworkConnection;
				if ((steamNetworkConnection = (connections[i] as SteamNetworkConnection)) != null && steamNetworkConnection.steamId.value == steamId)
				{
					this.ServerHandleClientDisconnect(steamNetworkConnection);
				}
			}
		}

		// Token: 0x06001F7A RID: 8058 RVA: 0x00094AC8 File Offset: 0x00092CC8
		private void OnClientP2PConnectionFailed(ulong steamId, Networking.SessionError sessionError)
		{
			Debug.LogFormat("GameNetworkManager.OnClientP2PConnectionFailed steamId={0} sessionError={1}", new object[]
			{
				steamId,
				sessionError
			});
			SteamNetworkConnection steamNetworkConnection;
			if (this.client != null && (steamNetworkConnection = (this.client.connection as SteamNetworkConnection)) != null && steamNetworkConnection.steamId.value == steamId)
			{
				steamNetworkConnection.InvokeHandlerNoData(33);
				steamNetworkConnection.Disconnect();
				steamNetworkConnection.Dispose();
			}
			if (NetworkServer.active)
			{
				ReadOnlyCollection<NetworkConnection> connections = NetworkServer.connections;
				for (int i = connections.Count - 1; i >= 0; i--)
				{
					SteamNetworkConnection steamNetworkConnection2;
					if ((steamNetworkConnection2 = (connections[i] as SteamNetworkConnection)) != null && steamNetworkConnection2.steamId.value == steamId)
					{
						steamNetworkConnection2.InvokeHandlerNoData(33);
						steamNetworkConnection2.Disconnect();
						steamNetworkConnection2.Dispose();
					}
				}
			}
		}

		// Token: 0x06001F7B RID: 8059 RVA: 0x00094B8C File Offset: 0x00092D8C
		public void CreateP2PConnectionWithPeer(CSteamID peer)
		{
			SteamNetworkConnection steamNetworkConnection = new SteamNetworkConnection(peer);
			steamNetworkConnection.ForceInitialize(NetworkServer.hostTopology);
			int num = -1;
			ReadOnlyCollection<NetworkConnection> connections = NetworkServer.connections;
			for (int i = 1; i < connections.Count; i++)
			{
				if (connections[i] == null)
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				num = connections.Count;
			}
			steamNetworkConnection.connectionId = num;
			NetworkServer.AddExternalConnection(steamNetworkConnection);
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.StartMessage(32);
			networkWriter.FinishMessage();
			steamNetworkConnection.SendWriter(networkWriter, QosChannelIndex.defaultReliable.intVal);
		}

		// Token: 0x14000052 RID: 82
		// (add) Token: 0x06001F7C RID: 8060 RVA: 0x00094C18 File Offset: 0x00092E18
		// (remove) Token: 0x06001F7D RID: 8061 RVA: 0x00094C4C File Offset: 0x00092E4C
		public static event Action onStartServerGlobal;

		// Token: 0x14000053 RID: 83
		// (add) Token: 0x06001F7E RID: 8062 RVA: 0x00094C80 File Offset: 0x00092E80
		// (remove) Token: 0x06001F7F RID: 8063 RVA: 0x00094CB4 File Offset: 0x00092EB4
		public static event Action onStopServerGlobal;

		// Token: 0x14000054 RID: 84
		// (add) Token: 0x06001F80 RID: 8064 RVA: 0x00094CE8 File Offset: 0x00092EE8
		// (remove) Token: 0x06001F81 RID: 8065 RVA: 0x00094D1C File Offset: 0x00092F1C
		public static event Action<NetworkConnection> onServerConnectGlobal;

		// Token: 0x14000055 RID: 85
		// (add) Token: 0x06001F82 RID: 8066 RVA: 0x00094D50 File Offset: 0x00092F50
		// (remove) Token: 0x06001F83 RID: 8067 RVA: 0x00094D84 File Offset: 0x00092F84
		public static event Action<NetworkConnection> onServerDisconnectGlobal;

		// Token: 0x14000056 RID: 86
		// (add) Token: 0x06001F84 RID: 8068 RVA: 0x00094DB8 File Offset: 0x00092FB8
		// (remove) Token: 0x06001F85 RID: 8069 RVA: 0x00094DEC File Offset: 0x00092FEC
		public static event Action<string> onServerSceneChangedGlobal;

		// Token: 0x06001F86 RID: 8070 RVA: 0x00094E20 File Offset: 0x00093020
		private void StartSteamworksServer()
		{
			string modDir = "Risk of Rain 2";
			string gameDesc = "Risk of Rain 2";
			this.steamworksServer = new Server(632360u, new ServerInit(modDir, gameDesc)
			{
				VersionString = "0.0.0.0",
				Secure = true,
				IpAddress = IPAddress.Any,
				GamePort = 7777
			});
			Debug.LogFormat("steamworksServer.IsValid={0}", new object[]
			{
				this.steamworksServer.IsValid
			});
			if (!this.steamworksServer.IsValid)
			{
				this.steamworksServer.Dispose();
				this.steamworksServer = null;
			}
			if (this.steamworksServer != null)
			{
				this.steamworksServer.MaxPlayers = base.maxConnections;
				this.steamworksServer.ServerName = "NAME";
				this.steamworksServer.DedicatedServer = false;
				this.steamworksServer.AutomaticHeartbeats = true;
				this.steamworksServer.MapName = SceneManager.GetActiveScene().name;
				this.steamworksServer.LogOnAnonymous();
				Debug.LogFormat("steamworksServer.LoggedOn={0}", new object[]
				{
					this.steamworksServer.LoggedOn
				});
				base.StartCoroutine("CheckIPUntilAvailable");
				base.StartCoroutine("UpdateSteamServerPlayers");
				SteamworksLobbyManager.OnServerSteamIDDiscovered(new CSteamID(Client.Instance.SteamId));
				GameNetworkManager.onServerSceneChangedGlobal += this.UpdateSteamMapName;
			}
		}

		// Token: 0x06001F87 RID: 8071 RVA: 0x00094F81 File Offset: 0x00093181
		private void UpdateSteamMapName(string sceneName)
		{
			if (this.steamworksServer != null)
			{
				this.steamworksServer.MapName = sceneName;
			}
		}

		// Token: 0x06001F88 RID: 8072 RVA: 0x00094F97 File Offset: 0x00093197
		private void StopSteamworksServer()
		{
			GameNetworkManager.onServerSceneChangedGlobal -= this.UpdateSteamMapName;
			if (this.steamworksServer != null)
			{
				this.steamworksServer.Dispose();
				this.steamworksServer = null;
			}
			base.StopCoroutine("CheckIPUntilAvailable");
		}

		// Token: 0x06001F89 RID: 8073 RVA: 0x00094FCF File Offset: 0x000931CF
		private IEnumerator CheckIPUntilAvailable()
		{
			IPAddress address = null;
			while (this.steamworksServer != null && (address = this.steamworksServer.PublicIp) == null)
			{
				yield return new WaitForSecondsRealtime(0.1f);
			}
			if (address != null)
			{
				SteamworksLobbyManager.OnServerIPDiscovered(address.ToString(), (ushort)NetworkServer.listenPort);
			}
			else
			{
				Debug.Log("Failed to find Steamworks server IP.");
			}
			yield break;
		}

		// Token: 0x06001F8A RID: 8074 RVA: 0x00094FDE File Offset: 0x000931DE
		private IEnumerator UpdateSteamServerPlayers()
		{
			while (this.steamworksServer != null)
			{
				foreach (NetworkUser networkUser in NetworkUser.readOnlyInstancesList)
				{
					SteamNetworkConnection steamNetworkConnection;
					if ((steamNetworkConnection = (networkUser.connectionToClient as SteamNetworkConnection)) != null)
					{
						this.steamworksServer.UpdatePlayer(steamNetworkConnection.steamId.value, networkUser.userName, 0);
					}
				}
				yield return new WaitForSecondsRealtime(1f);
			}
			yield break;
		}

		// Token: 0x06001F8B RID: 8075 RVA: 0x00094FF0 File Offset: 0x000931F0
		public NetworkConnection GetClient(CSteamID steamId)
		{
			if (!NetworkServer.active)
			{
				return null;
			}
			if (steamId.value == Client.Instance.SteamId && NetworkServer.connections.Count > 0)
			{
				return NetworkServer.connections[0];
			}
			using (IEnumerator<NetworkConnection> enumerator = NetworkServer.connections.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SteamNetworkConnection steamNetworkConnection;
					if ((steamNetworkConnection = (enumerator.Current as SteamNetworkConnection)) != null && steamNetworkConnection.steamId.value == steamId.value)
					{
						return steamNetworkConnection;
					}
				}
			}
			Debug.LogError("Client not found");
			return null;
		}

		// Token: 0x06001F8C RID: 8076 RVA: 0x00095098 File Offset: 0x00093298
		public override void OnStartServer()
		{
			base.OnStartServer();
			NetworkMessageHandlerAttribute.RegisterServerMessages();
			this.unpredictedServerFixedTime = 0f;
			UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkSession"));
			Action action = GameNetworkManager.onStartServerGlobal;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06001F8D RID: 8077 RVA: 0x000950CF File Offset: 0x000932CF
		public override void OnStopServer()
		{
			Action action = GameNetworkManager.onStopServerGlobal;
			if (action != null)
			{
				action();
			}
			base.OnStopServer();
		}

		// Token: 0x06001F8E RID: 8078 RVA: 0x000950E7 File Offset: 0x000932E7
		public override void OnServerConnect(NetworkConnection conn)
		{
			base.OnServerConnect(conn);
			if (NetworkUser.readOnlyInstancesList.Count >= base.maxConnections)
			{
				this.ServerKickClient(conn, GameNetworkManager.KickReason.ServerFull);
				return;
			}
			Action<NetworkConnection> action = GameNetworkManager.onServerConnectGlobal;
			if (action == null)
			{
				return;
			}
			action(conn);
		}

		// Token: 0x06001F8F RID: 8079 RVA: 0x0009511C File Offset: 0x0009331C
		public override void OnServerDisconnect(NetworkConnection conn)
		{
			Action<NetworkConnection> action = GameNetworkManager.onServerDisconnectGlobal;
			if (action != null)
			{
				action(conn);
			}
			if (conn.clientOwnedObjects != null)
			{
				foreach (NetworkInstanceId netId in new HashSet<NetworkInstanceId>(conn.clientOwnedObjects))
				{
					GameObject gameObject = NetworkServer.FindLocalObject(netId);
					if (gameObject != null && gameObject.GetComponent<CharacterMaster>())
					{
						NetworkIdentity component = gameObject.GetComponent<NetworkIdentity>();
						if (component && component.clientAuthorityOwner == conn)
						{
							component.RemoveClientAuthority(conn);
						}
					}
				}
			}
			List<PlayerController> playerControllers = conn.playerControllers;
			for (int i = 0; i < playerControllers.Count; i++)
			{
				NetworkUser component2 = playerControllers[i].gameObject.GetComponent<NetworkUser>();
				if (component2)
				{
					Chat.SendPlayerDisconnectedMessage(component2);
				}
			}
			if (conn is SteamNetworkConnection)
			{
				Debug.LogFormat("Closing connection with steamId {0}", new object[]
				{
					((SteamNetworkConnection)conn).steamId.value
				});
			}
			base.OnServerDisconnect(conn);
		}

		// Token: 0x06001F90 RID: 8080 RVA: 0x00095238 File Offset: 0x00093438
		private void ServerHandleClientDisconnect(NetworkConnection conn)
		{
			conn.InvokeHandlerNoData(33);
			conn.Disconnect();
			conn.Dispose();
			if (conn is SteamNetworkConnection)
			{
				NetworkServer.RemoveExternalConnection(conn.connectionId);
			}
		}

		// Token: 0x06001F91 RID: 8081 RVA: 0x00095264 File Offset: 0x00093464
		private void ServerKickClient(NetworkConnection conn, GameNetworkManager.KickReason reason)
		{
			Debug.LogFormat("Kicking client on connection {0}: Reason {1}", new object[]
			{
				conn.connectionId,
				reason
			});
			conn.SendByChannel(67, new GameNetworkManager.KickMessage
			{
				reason = reason
			}, QosChannelIndex.defaultReliable.intVal);
			conn.FlushChannels();
			SteamNetworkConnection steamNetworkConnection;
			if ((steamNetworkConnection = (conn as SteamNetworkConnection)) != null)
			{
				if (reason == GameNetworkManager.KickReason.Ban)
				{
					this.steamIdBanList.Add(steamNetworkConnection.steamId.value);
				}
				steamNetworkConnection.ignore = true;
			}
		}

		// Token: 0x06001F92 RID: 8082 RVA: 0x000952E8 File Offset: 0x000934E8
		private void ServerDisconnectClient(NetworkConnection conn)
		{
			Debug.LogFormat("Disconnecting client on connection {0}", new object[]
			{
				conn.connectionId
			});
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.StartMessage(33);
			networkWriter.FinishMessage();
			conn.SendWriter(networkWriter, QosChannelIndex.defaultReliable.intVal);
			this.ServerHandleClientDisconnect(conn);
		}

		// Token: 0x06001F93 RID: 8083 RVA: 0x00095340 File Offset: 0x00093540
		public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
		{
			this.OnServerAddPlayer(conn, playerControllerId, null);
		}

		// Token: 0x06001F94 RID: 8084 RVA: 0x0009534B File Offset: 0x0009354B
		public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
		{
			this.OnServerAddPlayerInternal(conn, playerControllerId, extraMessageReader);
		}

		// Token: 0x06001F95 RID: 8085 RVA: 0x00095358 File Offset: 0x00093558
		private void OnServerAddPlayerInternal(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
		{
			if (base.playerPrefab == null)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("The PlayerPrefab is empty on the NetworkManager. Please setup a PlayerPrefab object.");
				}
				return;
			}
			if (base.playerPrefab.GetComponent<NetworkIdentity>() == null)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("The PlayerPrefab does not have a NetworkIdentity. Please add a NetworkIdentity to the player prefab.");
				}
				return;
			}
			if ((int)playerControllerId < conn.playerControllers.Count && conn.playerControllers[(int)playerControllerId].IsValid && conn.playerControllers[(int)playerControllerId].gameObject != null)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("There is already a player at that playerControllerId for this connections.");
				}
				return;
			}
			if (NetworkUser.readOnlyInstancesList.Count >= base.maxConnections)
			{
				if (LogFilter.logError)
				{
					Debug.LogError("Cannot add any more players.)");
				}
				return;
			}
			GameNetworkManager.AddPlayerMessage addPlayerMessage = extraMessageReader.ReadMessage<GameNetworkManager.AddPlayerMessage>();
			Transform startPosition = base.GetStartPosition();
			GameObject gameObject;
			if (startPosition != null)
			{
				gameObject = UnityEngine.Object.Instantiate<GameObject>(base.playerPrefab, startPosition.position, startPosition.rotation);
			}
			else
			{
				gameObject = UnityEngine.Object.Instantiate<GameObject>(base.playerPrefab, Vector3.zero, Quaternion.identity);
			}
			Debug.LogFormat("GameNetworkManager.AddPlayerInternal(conn={0}, playerControllerId={1}, extraMessageReader={2}", new object[]
			{
				conn,
				playerControllerId,
				extraMessageReader
			});
			NetworkUser component = gameObject.GetComponent<NetworkUser>();
			bool flag = Util.ConnectionIsLocal(conn);
			NetworkUserId id = NetworkUserId.FromIp(conn.address, (byte)playerControllerId);
			CSteamID csteamID = CSteamID.nil;
			SteamNetworkConnection steamNetworkConnection;
			if ((steamNetworkConnection = (conn as SteamNetworkConnection)) != null)
			{
				csteamID = steamNetworkConnection.steamId;
			}
			else if (flag && Client.Instance != null)
			{
				csteamID = new CSteamID(Client.Instance.SteamId);
			}
			if (csteamID != CSteamID.nil)
			{
				id = NetworkUserId.FromSteamId(csteamID.value, (byte)playerControllerId);
			}
			if (this.steamworksServer != null && csteamID != CSteamID.nil && this.steamworksServer.Auth.StartSession(addPlayerMessage.steamAuthTicketData, csteamID.value))
			{
				this.steamworksServer.UpdatePlayer(csteamID.value, RoR2Application.instance.steamworksClient.Friends.GetName(csteamID.value), 0);
			}
			component.id = id;
			Chat.SendPlayerConnectedMessage(component);
			NetworkServer.AddPlayerForConnection(conn, gameObject, playerControllerId);
		}

		// Token: 0x06001F96 RID: 8086 RVA: 0x00095570 File Offset: 0x00093770
		private void UpdateServer()
		{
			Server server = this.steamworksServer;
			if (server != null)
			{
				server.Update();
			}
			if (NetworkServer.active)
			{
				ReadOnlyCollection<NetworkConnection> connections = NetworkServer.connections;
				for (int i = connections.Count - 1; i >= 0; i--)
				{
					SteamNetworkConnection steamNetworkConnection;
					if ((steamNetworkConnection = (connections[i] as SteamNetworkConnection)) != null)
					{
						Networking.P2PSessionState p2PSessionState = default(Networking.P2PSessionState);
						if (Client.Instance.Networking.GetP2PSessionState(steamNetworkConnection.steamId.value, ref p2PSessionState) && p2PSessionState.Connecting == 0 && p2PSessionState.ConnectionActive == 0)
						{
							this.ServerHandleClientDisconnect(steamNetworkConnection);
						}
					}
				}
			}
		}

		// Token: 0x06001F97 RID: 8087 RVA: 0x000955FC File Offset: 0x000937FC
		private void FixedUpdateServer()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			this.timeTransmitTimer -= Time.fixedDeltaTime;
			if (this.timeTransmitTimer <= 0f)
			{
				NetworkWriter networkWriter = new NetworkWriter();
				networkWriter.StartMessage(54);
				networkWriter.Write(this.unpredictedServerFixedTime);
				networkWriter.FinishMessage();
				NetworkServer.SendWriterToReady(null, networkWriter, QosChannelIndex.time.intVal);
				this.timeTransmitTimer += this.timeTransmitInterval;
			}
			foreach (NetworkConnection networkConnection in NetworkServer.connections)
			{
				if (networkConnection != null && !Util.ConnectionIsLocal(networkConnection))
				{
					this.Ping(networkConnection, QosChannelIndex.ping.intVal);
				}
			}
		}

		// Token: 0x06001F98 RID: 8088 RVA: 0x000956CC File Offset: 0x000938CC
		public override void OnServerSceneChanged(string sceneName)
		{
			base.OnServerSceneChanged(sceneName);
			if (Run.instance)
			{
				Run.instance.OnServerSceneChanged(sceneName);
			}
			Action<string> action = GameNetworkManager.onServerSceneChangedGlobal;
			if (action == null)
			{
				return;
			}
			action(sceneName);
		}

		// Token: 0x06001F99 RID: 8089 RVA: 0x000956FC File Offset: 0x000938FC
		private bool IsServerAtMaxConnections()
		{
			ReadOnlyCollection<NetworkConnection> connections = NetworkServer.connections;
			if (connections.Count >= base.maxConnections)
			{
				int num = 0;
				using (IEnumerator<NetworkConnection> enumerator = connections.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current != null)
						{
							num++;
						}
					}
				}
				return num >= base.maxConnections;
			}
			return false;
		}

		// Token: 0x06001F9A RID: 8090 RVA: 0x00095768 File Offset: 0x00093968
		public int GetConnectingClientCount()
		{
			int num = 0;
			ReadOnlyCollection<NetworkConnection> connections = NetworkServer.connections;
			int count = connections.Count;
			ReadOnlyCollection<NetworkUser> readOnlyInstancesList = NetworkUser.readOnlyInstancesList;
			int count2 = readOnlyInstancesList.Count;
			for (int i = 0; i < count; i++)
			{
				NetworkConnection networkConnection = connections[i];
				if (networkConnection != null)
				{
					bool flag = false;
					for (int j = 0; j < count2; j++)
					{
						if (readOnlyInstancesList[j].connectionToClient == networkConnection)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x06001F9B RID: 8091 RVA: 0x000957E3 File Offset: 0x000939E3
		public void RequestServerShutdown()
		{
			if (this.serverShuttingDown)
			{
				return;
			}
			this.serverShuttingDown = true;
			base.StartCoroutine(this.ServerShutdownCoroutine());
		}

		// Token: 0x06001F9C RID: 8092 RVA: 0x00095802 File Offset: 0x00093A02
		private IEnumerator ServerShutdownCoroutine()
		{
			Debug.Log("Server shutting down...");
			ReadOnlyCollection<NetworkConnection> connections = NetworkServer.connections;
			for (int i = connections.Count - 1; i >= 0; i--)
			{
				NetworkConnection networkConnection = connections[i];
				if (networkConnection != null && !Util.ConnectionIsLocal(networkConnection))
				{
					this.ServerKickClient(networkConnection, GameNetworkManager.KickReason.ServerShutdown);
				}
			}
			Debug.Log("Issued kick message to all remote clients.");
			float maxWait = 0.2f;
			float t = 0f;
			while (t < maxWait && !GameNetworkManager.<ServerShutdownCoroutine>g__CheckConnectionsEmpty|161_0())
			{
				yield return new WaitForEndOfFrame();
				t += Time.unscaledDeltaTime;
			}
			Debug.Log("Finished waiting for clients to disconnect.");
			if (this.client != null)
			{
				Debug.Log("StopHost()");
				base.StopHost();
			}
			else
			{
				Debug.Log("StopServer()");
				base.StopServer();
			}
			this.serverShuttingDown = false;
			Debug.Log("Server shutdown complete.");
			yield break;
		}

		// Token: 0x040021D7 RID: 8663
		private static readonly FieldInfo loadingSceneAsyncFieldInfo;

		// Token: 0x040021D9 RID: 8665
		private static readonly string[] spawnableFolders = new string[]
		{
			"CharacterBodies",
			"CharacterMasters",
			"Projectiles",
			"NetworkedObjects",
			"GameModes"
		};

		// Token: 0x040021DB RID: 8667
		public float debugServerTime;

		// Token: 0x040021DC RID: 8668
		public float debugRTT;

		// Token: 0x040021DD RID: 8669
		private bool actedUponDesiredHost;

		// Token: 0x040021DE RID: 8670
		private float lastDesiredHostSetTime = float.NegativeInfinity;

		// Token: 0x040021DF RID: 8671
		private GameNetworkManager.HostDescription _desiredHost;

		// Token: 0x040021E4 RID: 8676
		private static bool wasFading = false;

		// Token: 0x040021E6 RID: 8678
		private float rttVelocity;

		// Token: 0x040021E7 RID: 8679
		public float filteredRTTSmoothDuration = 0.1f;

		// Token: 0x040021E9 RID: 8681
		private static readonly string[] sceneWhiteList = new string[]
		{
			"title",
			"crystalworld",
			"logbook"
		};

		// Token: 0x040021ED RID: 8685
		private List<ulong> steamIdBanList = new List<ulong>();

		// Token: 0x040021F3 RID: 8691
		public Server steamworksServer;

		// Token: 0x040021F4 RID: 8692
		public float timeTransmitInterval = 0.016666668f;

		// Token: 0x040021F5 RID: 8693
		private float timeTransmitTimer;

		// Token: 0x040021F6 RID: 8694
		private bool serverShuttingDown;

		// Token: 0x02000579 RID: 1401
		public struct HostDescription : IEquatable<GameNetworkManager.HostDescription>
		{
			// Token: 0x06001F9F RID: 8095 RVA: 0x000958A0 File Offset: 0x00093AA0
			public HostDescription(CSteamID steamId)
			{
				this = default(GameNetworkManager.HostDescription);
				this.hostType = GameNetworkManager.HostDescription.HostType.Steam;
				this.steamId = steamId;
			}

			// Token: 0x06001FA0 RID: 8096 RVA: 0x000958B7 File Offset: 0x00093AB7
			public HostDescription(AddressPortPair addressPortPair)
			{
				this = default(GameNetworkManager.HostDescription);
				this.hostType = GameNetworkManager.HostDescription.HostType.IPv4;
				this.addressPortPair = addressPortPair;
			}

			// Token: 0x06001FA1 RID: 8097 RVA: 0x000958CE File Offset: 0x00093ACE
			public HostDescription(GameNetworkManager.HostDescription.HostingParameters hostingParameters)
			{
				this = default(GameNetworkManager.HostDescription);
				this.hostType = GameNetworkManager.HostDescription.HostType.Self;
				this.hostingParameters = hostingParameters;
			}

			// Token: 0x06001FA2 RID: 8098 RVA: 0x000958E8 File Offset: 0x00093AE8
			public bool DescribesCurrentHost()
			{
				switch (this.hostType)
				{
				case GameNetworkManager.HostDescription.HostType.None:
					return !GameNetworkManager.singleton.isNetworkActive;
				case GameNetworkManager.HostDescription.HostType.Self:
					return NetworkServer.active && this.hostingParameters.listen != NetworkServer.dontListen;
				case GameNetworkManager.HostDescription.HostType.Steam:
				{
					NetworkClient client = GameNetworkManager.singleton.client;
					SteamNetworkConnection steamNetworkConnection;
					return (steamNetworkConnection = (((client != null) ? client.connection : null) as SteamNetworkConnection)) != null && steamNetworkConnection.steamId == this.steamId;
				}
				case GameNetworkManager.HostDescription.HostType.IPv4:
				{
					NetworkClient client2 = GameNetworkManager.singleton.client;
					NetworkConnection networkConnection;
					return (networkConnection = ((client2 != null) ? client2.connection : null)) != null && networkConnection.address == this.addressPortPair.address;
				}
				default:
					throw new ArgumentOutOfRangeException();
				}
			}

			// Token: 0x06001FA3 RID: 8099 RVA: 0x000959B3 File Offset: 0x00093BB3
			private HostDescription(object o)
			{
				this = default(GameNetworkManager.HostDescription);
				this.hostType = GameNetworkManager.HostDescription.HostType.None;
			}

			// Token: 0x06001FA4 RID: 8100 RVA: 0x000959C4 File Offset: 0x00093BC4
			public bool Equals(GameNetworkManager.HostDescription other)
			{
				return this.hostType == other.hostType && this.steamId.Equals(other.steamId) && this.addressPortPair.Equals(other.addressPortPair) && this.hostingParameters.Equals(other.hostingParameters);
			}

			// Token: 0x06001FA5 RID: 8101 RVA: 0x00095A24 File Offset: 0x00093C24
			public override bool Equals(object obj)
			{
				if (obj == null)
				{
					return false;
				}
				if (obj is GameNetworkManager.HostDescription)
				{
					GameNetworkManager.HostDescription other = (GameNetworkManager.HostDescription)obj;
					return this.Equals(other);
				}
				return false;
			}

			// Token: 0x06001FA6 RID: 8102 RVA: 0x00095A50 File Offset: 0x00093C50
			public override int GetHashCode()
			{
				return (int)(((this.hostType * (GameNetworkManager.HostDescription.HostType)397 ^ (GameNetworkManager.HostDescription.HostType)this.steamId.GetHashCode()) * (GameNetworkManager.HostDescription.HostType)397 ^ (GameNetworkManager.HostDescription.HostType)this.addressPortPair.GetHashCode()) * (GameNetworkManager.HostDescription.HostType)397 ^ (GameNetworkManager.HostDescription.HostType)this.hostingParameters.GetHashCode());
			}

			// Token: 0x06001FA7 RID: 8103 RVA: 0x00095AB4 File Offset: 0x00093CB4
			public override string ToString()
			{
				GameNetworkManager.HostDescription.sharedStringBuilder.Clear();
				GameNetworkManager.HostDescription.sharedStringBuilder.Append("{ hostType=").Append(this.hostType);
				switch (this.hostType)
				{
				case GameNetworkManager.HostDescription.HostType.None:
					break;
				case GameNetworkManager.HostDescription.HostType.Self:
					GameNetworkManager.HostDescription.sharedStringBuilder.Append(" listen=").Append(this.hostingParameters.listen);
					GameNetworkManager.HostDescription.sharedStringBuilder.Append(" maxPlayers=").Append(this.hostingParameters.maxPlayers);
					break;
				case GameNetworkManager.HostDescription.HostType.Steam:
					GameNetworkManager.HostDescription.sharedStringBuilder.Append(" steamId=").Append(this.steamId);
					break;
				case GameNetworkManager.HostDescription.HostType.IPv4:
					GameNetworkManager.HostDescription.sharedStringBuilder.Append(" address=").Append(this.addressPortPair.address);
					GameNetworkManager.HostDescription.sharedStringBuilder.Append(" port=").Append(this.addressPortPair.port);
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				GameNetworkManager.HostDescription.sharedStringBuilder.Append(" }");
				return GameNetworkManager.HostDescription.sharedStringBuilder.ToString();
			}

			// Token: 0x040021F7 RID: 8695
			public readonly GameNetworkManager.HostDescription.HostType hostType;

			// Token: 0x040021F8 RID: 8696
			public readonly CSteamID steamId;

			// Token: 0x040021F9 RID: 8697
			public readonly AddressPortPair addressPortPair;

			// Token: 0x040021FA RID: 8698
			public readonly GameNetworkManager.HostDescription.HostingParameters hostingParameters;

			// Token: 0x040021FB RID: 8699
			public static readonly GameNetworkManager.HostDescription none = new GameNetworkManager.HostDescription(null);

			// Token: 0x040021FC RID: 8700
			private static readonly StringBuilder sharedStringBuilder = new StringBuilder();

			// Token: 0x0200057A RID: 1402
			public enum HostType
			{
				// Token: 0x040021FE RID: 8702
				None,
				// Token: 0x040021FF RID: 8703
				Self,
				// Token: 0x04002200 RID: 8704
				Steam,
				// Token: 0x04002201 RID: 8705
				IPv4
			}

			// Token: 0x0200057B RID: 1403
			public struct HostingParameters : IEquatable<GameNetworkManager.HostDescription.HostingParameters>
			{
				// Token: 0x06001FA9 RID: 8105 RVA: 0x00095BEB File Offset: 0x00093DEB
				public bool Equals(GameNetworkManager.HostDescription.HostingParameters other)
				{
					return this.listen == other.listen && this.maxPlayers == other.maxPlayers;
				}

				// Token: 0x06001FAA RID: 8106 RVA: 0x00095C0C File Offset: 0x00093E0C
				public override bool Equals(object obj)
				{
					if (obj == null)
					{
						return false;
					}
					if (obj is GameNetworkManager.HostDescription.HostingParameters)
					{
						GameNetworkManager.HostDescription.HostingParameters other = (GameNetworkManager.HostDescription.HostingParameters)obj;
						return this.Equals(other);
					}
					return false;
				}

				// Token: 0x06001FAB RID: 8107 RVA: 0x00095C38 File Offset: 0x00093E38
				public override int GetHashCode()
				{
					return this.listen.GetHashCode() * 397 ^ this.maxPlayers;
				}

				// Token: 0x04002202 RID: 8706
				public bool listen;

				// Token: 0x04002203 RID: 8707
				public int maxPlayers;
			}
		}

		// Token: 0x0200057C RID: 1404
		private class NetLogLevelConVar : BaseConVar
		{
			// Token: 0x06001FAC RID: 8108 RVA: 0x00037E38 File Offset: 0x00036038
			public NetLogLevelConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001FAD RID: 8109 RVA: 0x00095C54 File Offset: 0x00093E54
			public override void SetString(string newValue)
			{
				int currentLogLevel;
				if (TextSerialization.TryParseInvariant(newValue, out currentLogLevel))
				{
					LogFilter.currentLogLevel = currentLogLevel;
				}
			}

			// Token: 0x06001FAE RID: 8110 RVA: 0x00095C71 File Offset: 0x00093E71
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(LogFilter.currentLogLevel);
			}

			// Token: 0x04002204 RID: 8708
			private static GameNetworkManager.NetLogLevelConVar cvNetLogLevel = new GameNetworkManager.NetLogLevelConVar("net_loglevel", ConVarFlags.Engine, null, "Network log verbosity.");
		}

		// Token: 0x0200057D RID: 1405
		private class SvListenConVar : BaseConVar
		{
			// Token: 0x06001FB0 RID: 8112 RVA: 0x00037E38 File Offset: 0x00036038
			public SvListenConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001FB1 RID: 8113 RVA: 0x00095C98 File Offset: 0x00093E98
			public override void SetString(string newValue)
			{
				if (NetworkServer.active)
				{
					Debug.Log("Can't change value of sv_listen while server is running.");
					return;
				}
				int num;
				if (TextSerialization.TryParseInvariant(newValue, out num))
				{
					NetworkServer.dontListen = (num == 0);
				}
			}

			// Token: 0x06001FB2 RID: 8114 RVA: 0x00095CCA File Offset: 0x00093ECA
			public override string GetString()
			{
				if (!NetworkServer.dontListen)
				{
					return "1";
				}
				return "0";
			}

			// Token: 0x04002205 RID: 8709
			private static GameNetworkManager.SvListenConVar cvSvListen = new GameNetworkManager.SvListenConVar("sv_listen", ConVarFlags.Engine, null, "Whether or not the server will accept connections from other players.");
		}

		// Token: 0x0200057E RID: 1406
		private class SvMaxPlayersConVar : BaseConVar
		{
			// Token: 0x06001FB4 RID: 8116 RVA: 0x00037E38 File Offset: 0x00036038
			public SvMaxPlayersConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001FB5 RID: 8117 RVA: 0x00095CF8 File Offset: 0x00093EF8
			public override void SetString(string newValue)
			{
				if (NetworkServer.active)
				{
					Debug.Log("Can't change value of sv_maxplayers while server is running.");
					return;
				}
				int val;
				if (NetworkManager.singleton && TextSerialization.TryParseInvariant(newValue, out val))
				{
					NetworkManager.singleton.maxConnections = Math.Min(Math.Max(val, 1), RoR2Application.hardMaxPlayers);
				}
			}

			// Token: 0x06001FB6 RID: 8118 RVA: 0x00095D48 File Offset: 0x00093F48
			public override string GetString()
			{
				if (!NetworkManager.singleton)
				{
					return "1";
				}
				return TextSerialization.ToStringInvariant(NetworkManager.singleton.maxConnections);
			}

			// Token: 0x170002C4 RID: 708
			// (get) Token: 0x06001FB7 RID: 8119 RVA: 0x00095D6B File Offset: 0x00093F6B
			public int intValue
			{
				get
				{
					return NetworkManager.singleton.maxConnections;
				}
			}

			// Token: 0x04002206 RID: 8710
			public static readonly GameNetworkManager.SvMaxPlayersConVar instance = new GameNetworkManager.SvMaxPlayersConVar("sv_maxplayers", ConVarFlags.Engine, null, "Maximum number of players allowed.");
		}

		// Token: 0x0200057F RID: 1407
		private class KickMessage : MessageBase
		{
			// Token: 0x170002C5 RID: 709
			// (get) Token: 0x06001FB9 RID: 8121 RVA: 0x00095D90 File Offset: 0x00093F90
			// (set) Token: 0x06001FBA RID: 8122 RVA: 0x00095D98 File Offset: 0x00093F98
			public GameNetworkManager.KickReason reason
			{
				get
				{
					return (GameNetworkManager.KickReason)this.netReason;
				}
				set
				{
					this.netReason = (int)value;
				}
			}

			// Token: 0x06001FBB RID: 8123 RVA: 0x00095DA4 File Offset: 0x00093FA4
			public string GetDisplayToken()
			{
				switch (this.reason)
				{
				case GameNetworkManager.KickReason.ServerShutdown:
					return "KICK_REASON_SERVERSHUTDOWN";
				case GameNetworkManager.KickReason.Timeout:
					return "KICK_REASON_TIMEOUT";
				case GameNetworkManager.KickReason.Kick:
					return "KICK_REASON_KICK";
				case GameNetworkManager.KickReason.Ban:
					return "KICK_REASON_BAN";
				case GameNetworkManager.KickReason.BadPassword:
					return "KICK_REASON_BADPASSWORD";
				case GameNetworkManager.KickReason.BadVersion:
					return "KICK_REASON_BADVERSION";
				case GameNetworkManager.KickReason.ServerFull:
					return "KICK_REASON_SERVERFULL";
				default:
					return "KICK_REASON_UNSPECIFIED";
				}
			}

			// Token: 0x06001FBD RID: 8125 RVA: 0x00095E0D File Offset: 0x0009400D
			public override void Serialize(NetworkWriter writer)
			{
				writer.WritePackedUInt32((uint)this.netReason);
			}

			// Token: 0x06001FBE RID: 8126 RVA: 0x00095E1B File Offset: 0x0009401B
			public override void Deserialize(NetworkReader reader)
			{
				this.netReason = (int)reader.ReadPackedUInt32();
			}

			// Token: 0x04002207 RID: 8711
			public int netReason;
		}

		// Token: 0x02000580 RID: 1408
		protected class AddPlayerMessage : MessageBase
		{
			// Token: 0x06001FC0 RID: 8128 RVA: 0x00095E29 File Offset: 0x00094029
			public override void Serialize(NetworkWriter writer)
			{
				writer.WritePackedUInt64(this.steamId);
				writer.WriteBytesFull(this.steamAuthTicketData);
			}

			// Token: 0x06001FC1 RID: 8129 RVA: 0x00095E43 File Offset: 0x00094043
			public override void Deserialize(NetworkReader reader)
			{
				this.steamId = reader.ReadPackedUInt64();
				this.steamAuthTicketData = reader.ReadBytesAndSize();
			}

			// Token: 0x04002208 RID: 8712
			public ulong steamId;

			// Token: 0x04002209 RID: 8713
			public byte[] steamAuthTicketData;
		}

		// Token: 0x02000581 RID: 1409
		public enum KickReason
		{
			// Token: 0x0400220B RID: 8715
			Unspecified,
			// Token: 0x0400220C RID: 8716
			ServerShutdown,
			// Token: 0x0400220D RID: 8717
			Timeout,
			// Token: 0x0400220E RID: 8718
			Kick,
			// Token: 0x0400220F RID: 8719
			Ban,
			// Token: 0x04002210 RID: 8720
			BadPassword,
			// Token: 0x04002211 RID: 8721
			BadVersion,
			// Token: 0x04002212 RID: 8722
			ServerFull
		}

		// Token: 0x02000582 RID: 1410
		private class PingMessage : MessageBase
		{
			// Token: 0x06001FC3 RID: 8131 RVA: 0x00095E5D File Offset: 0x0009405D
			public override void Serialize(NetworkWriter writer)
			{
				writer.WritePackedUInt32(this.timeStampMs);
			}

			// Token: 0x06001FC4 RID: 8132 RVA: 0x00095E6B File Offset: 0x0009406B
			public override void Deserialize(NetworkReader reader)
			{
				this.timeStampMs = reader.ReadPackedUInt32();
			}

			// Token: 0x04002213 RID: 8723
			public uint timeStampMs;
		}
	}
}
