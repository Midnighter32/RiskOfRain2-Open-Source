using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Facepunch.Steamworks;
using RoR2.ConVar;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace RoR2.Networking
{
	// Token: 0x02000544 RID: 1348
	public class GameNetworkManager : NetworkManager
	{
		// Token: 0x06001FCE RID: 8142 RVA: 0x0008A04C File Offset: 0x0008824C
		static GameNetworkManager()
		{
			GameNetworkManager.loadingSceneAsyncFieldInfo = typeof(NetworkManager).GetField("s_LoadingSceneAsync", BindingFlags.Static | BindingFlags.NonPublic);
			if (GameNetworkManager.loadingSceneAsyncFieldInfo == null)
			{
				Debug.LogError("NetworkManager.s_LoadingSceneAsync field could not be found! Make sure to provide a proper implementation for this version of Unity.");
			}
		}

		// Token: 0x17000359 RID: 857
		// (get) Token: 0x06001FCF RID: 8143 RVA: 0x0008A164 File Offset: 0x00088364
		private static bool isLoadingScene
		{
			get
			{
				AsyncOperation asyncOperation = (AsyncOperation)GameNetworkManager.loadingSceneAsyncFieldInfo.GetValue(null);
				return asyncOperation != null && !asyncOperation.isDone;
			}
		}

		// Token: 0x1700035A RID: 858
		// (get) Token: 0x06001FD0 RID: 8144 RVA: 0x0008A190 File Offset: 0x00088390
		public new static GameNetworkManager singleton
		{
			get
			{
				return (GameNetworkManager)NetworkManager.singleton;
			}
		}

		// Token: 0x1700035B RID: 859
		// (get) Token: 0x06001FD1 RID: 8145 RVA: 0x0008A19C File Offset: 0x0008839C
		public float unpredictedServerFixedTime
		{
			get
			{
				return this._unpredictedServerFixedTime;
			}
		}

		// Token: 0x1700035C RID: 860
		// (get) Token: 0x06001FD2 RID: 8146 RVA: 0x0008A1A4 File Offset: 0x000883A4
		public float unpredictedServerFixedTimeSmoothed
		{
			get
			{
				return this._unpredictedServerFixedTimeSmoothed;
			}
		}

		// Token: 0x1700035D RID: 861
		// (get) Token: 0x06001FD3 RID: 8147 RVA: 0x0008A1AC File Offset: 0x000883AC
		public float serverFixedTime
		{
			get
			{
				return this.unpredictedServerFixedTimeSmoothed + this.filteredClientRttFixed;
			}
		}

		// Token: 0x1700035E RID: 862
		// (get) Token: 0x06001FD4 RID: 8148 RVA: 0x0008A1BB File Offset: 0x000883BB
		public float unpredictedServerFrameTime
		{
			get
			{
				return this._unpredictedServerFrameTime;
			}
		}

		// Token: 0x1700035F RID: 863
		// (get) Token: 0x06001FD5 RID: 8149 RVA: 0x0008A1C3 File Offset: 0x000883C3
		public float unpredictedServerFrameTimeSmoothed
		{
			get
			{
				return this._unpredictedServerFrameTimeSmoothed;
			}
		}

		// Token: 0x17000360 RID: 864
		// (get) Token: 0x06001FD6 RID: 8150 RVA: 0x0008A1CB File Offset: 0x000883CB
		public float serverFrameTime
		{
			get
			{
				return this.unpredictedServerFrameTimeSmoothed + this.filteredClientRttFrame;
			}
		}

		// Token: 0x06001FD7 RID: 8151 RVA: 0x0008A1DC File Offset: 0x000883DC
		private void InitializeTime()
		{
			this._unpredictedServerFixedTime = 0f;
			this._unpredictedServerFixedTimeSmoothed = 0f;
			this.unpredictedServerFixedTimeVelocity = 1f;
			this._unpredictedServerFrameTime = 0f;
			this._unpredictedServerFrameTimeSmoothed = 0f;
			this.unpredictedServerFrameTimeVelocity = 1f;
		}

		// Token: 0x06001FD8 RID: 8152 RVA: 0x0008A22C File Offset: 0x0008842C
		private void UpdateTime(ref float targetValue, ref float currentValue, ref float velocity, float deltaTime)
		{
			if (deltaTime <= 0f)
			{
				return;
			}
			targetValue += deltaTime;
			float num = (targetValue - currentValue) / deltaTime;
			float num2 = 1f;
			if (velocity == 0f || Mathf.Abs(num) > num2 * 3f)
			{
				currentValue = targetValue;
				velocity = num2;
				return;
			}
			currentValue += velocity * deltaTime;
			velocity = Mathf.MoveTowards(velocity, num, GameNetworkManager.cvNetTimeSmoothRate.value * deltaTime);
		}

		// Token: 0x06001FD9 RID: 8153 RVA: 0x0008A29C File Offset: 0x0008849C
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

		// Token: 0x06001FDA RID: 8154 RVA: 0x0008A2E4 File Offset: 0x000884E4
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

		// Token: 0x1400006F RID: 111
		// (add) Token: 0x06001FDB RID: 8155 RVA: 0x0008A3D0 File Offset: 0x000885D0
		// (remove) Token: 0x06001FDC RID: 8156 RVA: 0x0008A404 File Offset: 0x00088604
		public static event Action onStartGlobal;

		// Token: 0x06001FDD RID: 8157 RVA: 0x0008A437 File Offset: 0x00088637
		private void OnDestroy()
		{
			typeof(NetworkManager).GetMethod("OnDestroy", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(this, null);
		}

		// Token: 0x06001FDE RID: 8158 RVA: 0x0008A458 File Offset: 0x00088658
		protected void FixedUpdate()
		{
			this.UpdateTime(ref this._unpredictedServerFixedTime, ref this._unpredictedServerFixedTimeSmoothed, ref this.unpredictedServerFixedTimeVelocity, Time.fixedDeltaTime);
			this.FixedUpdateServer();
			this.FixedUpdateClient();
			this.debugServerTime = this.unpredictedServerFixedTime;
			this.debugRTT = this.clientRttFrame;
		}

		// Token: 0x06001FDF RID: 8159 RVA: 0x0008A4A6 File Offset: 0x000886A6
		protected void Update()
		{
			this.UpdateTime(ref this._unpredictedServerFrameTime, ref this._unpredictedServerFrameTimeSmoothed, ref this.unpredictedServerFrameTimeVelocity, Time.deltaTime);
			this.EnsureDesiredHost();
			this.UpdateServer();
			this.UpdateClient();
		}

		// Token: 0x06001FE0 RID: 8160 RVA: 0x0008A4D8 File Offset: 0x000886D8
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
					if (LocalUserManager.readOnlyLocalUsersList.Count == 0)
					{
						base.StartServer();
					}
					else
					{
						this.StartHost();
					}
				}
				if (this.desiredHost.hostType == GameNetworkManager.HostDescription.HostType.Steam && Time.unscaledTime - this.lastDesiredHostSetTime >= 0f)
				{
					this.actedUponDesiredHost = true;
					this.StartClientSteam(this.desiredHost.steamId);
				}
				if (this.desiredHost.hostType == GameNetworkManager.HostDescription.HostType.IPv4 && Time.unscaledTime - this.lastDesiredHostSetTime >= 0f)
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

		// Token: 0x17000361 RID: 865
		// (get) Token: 0x06001FE1 RID: 8161 RVA: 0x0008A691 File Offset: 0x00088891
		// (set) Token: 0x06001FE2 RID: 8162 RVA: 0x0008A69C File Offset: 0x0008889C
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

		// Token: 0x06001FE3 RID: 8163 RVA: 0x0008A6F8 File Offset: 0x000888F8
		public void ForceCloseAllSteamConnections()
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

		// Token: 0x17000362 RID: 866
		// (get) Token: 0x06001FE4 RID: 8164 RVA: 0x0008A79C File Offset: 0x0008899C
		// (set) Token: 0x06001FE5 RID: 8165 RVA: 0x0008A7A4 File Offset: 0x000889A4
		public bool clientHasConfirmedQuit { get; private set; }

		// Token: 0x17000363 RID: 867
		// (get) Token: 0x06001FE6 RID: 8166 RVA: 0x0008A7AD File Offset: 0x000889AD
		private bool clientIsConnecting
		{
			get
			{
				NetworkClient client = this.client;
				return ((client != null) ? client.connection : null) != null && !this.client.isConnected;
			}
		}

		// Token: 0x14000070 RID: 112
		// (add) Token: 0x06001FE7 RID: 8167 RVA: 0x0008A7D4 File Offset: 0x000889D4
		// (remove) Token: 0x06001FE8 RID: 8168 RVA: 0x0008A808 File Offset: 0x00088A08
		public static event Action<NetworkClient> onStartClientGlobal;

		// Token: 0x14000071 RID: 113
		// (add) Token: 0x06001FE9 RID: 8169 RVA: 0x0008A83C File Offset: 0x00088A3C
		// (remove) Token: 0x06001FEA RID: 8170 RVA: 0x0008A870 File Offset: 0x00088A70
		public static event Action onStopClientGlobal;

		// Token: 0x14000072 RID: 114
		// (add) Token: 0x06001FEB RID: 8171 RVA: 0x0008A8A4 File Offset: 0x00088AA4
		// (remove) Token: 0x06001FEC RID: 8172 RVA: 0x0008A8D8 File Offset: 0x00088AD8
		public static event Action<NetworkConnection> onClientConnectGlobal;

		// Token: 0x14000073 RID: 115
		// (add) Token: 0x06001FED RID: 8173 RVA: 0x0008A90C File Offset: 0x00088B0C
		// (remove) Token: 0x06001FEE RID: 8174 RVA: 0x0008A940 File Offset: 0x00088B40
		public static event Action<NetworkConnection> onClientDisconnectGlobal;

		// Token: 0x06001FEF RID: 8175 RVA: 0x0008A974 File Offset: 0x00088B74
		public override void OnStartClient(NetworkClient newClient)
		{
			base.OnStartClient(newClient);
			this.InitializeTime();
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

		// Token: 0x06001FF0 RID: 8176 RVA: 0x0008AA0C File Offset: 0x00088C0C
		public override void OnStopClient()
		{
			foreach (NetworkClient networkClient in NetworkClient.allClients)
			{
				if (networkClient != null)
				{
					NetworkConnection connection = networkClient.connection;
					if (connection != null)
					{
						connection.Disconnect();
					}
				}
			}
			this.ForceCloseAllSteamConnections();
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

		// Token: 0x06001FF1 RID: 8177 RVA: 0x0008AAA4 File Offset: 0x00088CA4
		public override void OnClientConnect(NetworkConnection conn)
		{
			base.OnClientConnect(conn);
			this.clientRttFrame = 0f;
			this.filteredClientRttFixed = 0f;
			this.ClientSendAuth(conn);
			this.ClientSetPlayers(conn);
			Action<NetworkConnection> action = GameNetworkManager.onClientConnectGlobal;
			if (action == null)
			{
				return;
			}
			action(conn);
		}

		// Token: 0x06001FF2 RID: 8178 RVA: 0x0008AAE4 File Offset: 0x00088CE4
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

		// Token: 0x06001FF3 RID: 8179 RVA: 0x0008AB30 File Offset: 0x00088D30
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
			if (Client.Instance != null)
			{
				extraMessage = new GameNetworkManager.AddPlayerMessage
				{
					steamId = Client.Instance.SteamId,
					steamAuthTicketData = Client.Instance.Auth.GetAuthSessionTicket().Data
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

		// Token: 0x06001FF4 RID: 8180 RVA: 0x0008AC30 File Offset: 0x00088E30
		private void UpdateClient()
		{
			NetworkClient client = this.client;
			if (((client != null) ? client.connection : null) is SteamNetworkConnection)
			{
				Networking.P2PSessionState p2PSessionState = default(Networking.P2PSessionState);
				if (Client.Instance.Networking.GetP2PSessionState(((SteamNetworkConnection)this.client.connection).steamId.value, ref p2PSessionState) && p2PSessionState.Connecting == 0 && p2PSessionState.ConnectionActive == 0)
				{
					this.client.connection.InvokeHandlerNoData(33);
					base.StopClient();
				}
			}
			NetworkClient client2 = this.client;
			if (((client2 != null) ? client2.connection : null) != null)
			{
				this.filteredClientRttFrame = RttManager.GetConnectionFrameSmoothedRtt(this.client.connection);
				this.clientRttFrame = RttManager.GetConnectionRTT(this.client.connection);
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

		// Token: 0x17000364 RID: 868
		// (get) Token: 0x06001FF5 RID: 8181 RVA: 0x0008AD37 File Offset: 0x00088F37
		// (set) Token: 0x06001FF6 RID: 8182 RVA: 0x0008AD3F File Offset: 0x00088F3F
		public float clientRttFixed { get; private set; }

		// Token: 0x17000365 RID: 869
		// (get) Token: 0x06001FF7 RID: 8183 RVA: 0x0008AD48 File Offset: 0x00088F48
		// (set) Token: 0x06001FF8 RID: 8184 RVA: 0x0008AD50 File Offset: 0x00088F50
		public float clientRttFrame { get; private set; }

		// Token: 0x17000366 RID: 870
		// (get) Token: 0x06001FF9 RID: 8185 RVA: 0x0008AD59 File Offset: 0x00088F59
		// (set) Token: 0x06001FFA RID: 8186 RVA: 0x0008AD61 File Offset: 0x00088F61
		public float filteredClientRttFixed { get; private set; }

		// Token: 0x17000367 RID: 871
		// (get) Token: 0x06001FFB RID: 8187 RVA: 0x0008AD6A File Offset: 0x00088F6A
		// (set) Token: 0x06001FFC RID: 8188 RVA: 0x0008AD72 File Offset: 0x00088F72
		public float filteredClientRttFrame { get; private set; }

		// Token: 0x06001FFD RID: 8189 RVA: 0x0008AD7C File Offset: 0x00088F7C
		private void FixedUpdateClient()
		{
			if (!NetworkClient.active || this.client == null)
			{
				return;
			}
			NetworkClient client = this.client;
			if (((client != null) ? client.connection : null) != null && this.client.connection.isConnected)
			{
				NetworkConnection connection = this.client.connection;
				this.filteredClientRttFixed = RttManager.GetConnectionFixedSmoothedRtt(connection);
				this.clientRttFixed = RttManager.GetConnectionRTT(connection);
				if (!Util.ConnectionIsLocal(connection))
				{
					RttManager.Ping(connection, QosChannelIndex.ping.intVal);
				}
			}
		}

		// Token: 0x06001FFE RID: 8190 RVA: 0x0008ADFC File Offset: 0x00088FFC
		public override void OnClientSceneChanged(NetworkConnection conn)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(NetworkManager.networkSceneName);
			string fileNameWithoutExtension2 = Path.GetFileNameWithoutExtension(SceneManager.GetActiveScene().name);
			Debug.LogFormat("OnClientSceneChanged networkSceneName={0} currentSceneName={1}", new object[]
			{
				fileNameWithoutExtension,
				fileNameWithoutExtension2
			});
			if (fileNameWithoutExtension != fileNameWithoutExtension2)
			{
				Debug.Log("OnClientSceneChanged skipped due to scene mismatch.");
				return;
			}
			base.autoCreatePlayer = false;
			base.OnClientSceneChanged(conn);
			this.ClientSetPlayers(conn);
			FadeToBlackManager.ForceFullBlack();
		}

		// Token: 0x06001FFF RID: 8191 RVA: 0x0008AE70 File Offset: 0x00089070
		private void ClientSendAuth(NetworkConnection conn)
		{
			conn.Send(74, new ClientAuthData
			{
				steamId = new CSteamID(Client.Instance.SteamId),
				authTicket = Client.Instance.Auth.GetAuthSessionTicket().Data,
				password = GameNetworkManager.cvClPassword.value,
				version = ""
			});
		}

		// Token: 0x06002000 RID: 8192 RVA: 0x0008AED8 File Offset: 0x000890D8
		private void ClientSetPlayers(NetworkConnection conn)
		{
			ReadOnlyCollection<LocalUser> readOnlyLocalUsersList = LocalUserManager.readOnlyLocalUsersList;
			for (int i = 0; i < readOnlyLocalUsersList.Count; i++)
			{
				this.ClientAddPlayer((short)readOnlyLocalUsersList[i].id, conn);
			}
		}

		// Token: 0x06002001 RID: 8193 RVA: 0x0008AF10 File Offset: 0x00089110
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

		// Token: 0x06002002 RID: 8194 RVA: 0x0008B038 File Offset: 0x00089238
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

		// Token: 0x06002003 RID: 8195 RVA: 0x0008B0B9 File Offset: 0x000892B9
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void ClientInit()
		{
			SceneCatalog.onMostRecentSceneDefChanged += GameNetworkManager.ClientUpdateOfflineScene;
		}

		// Token: 0x06002004 RID: 8196 RVA: 0x0008B0CC File Offset: 0x000892CC
		private static void ClientUpdateOfflineScene(SceneDef sceneDef)
		{
			if (GameNetworkManager.singleton && sceneDef.isOfflineScene)
			{
				GameNetworkManager.singleton.offlineScene = sceneDef.baseSceneName;
			}
		}

		// Token: 0x06002005 RID: 8197 RVA: 0x0008B0F2 File Offset: 0x000892F2
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

		// Token: 0x06002006 RID: 8198 RVA: 0x0008B120 File Offset: 0x00089320
		[ConCommand(commandName = "client_set_players", flags = ConVarFlags.None, helpText = "Adds network players for all local players. Debug only.")]
		private static void CCClientSetPlayers(ConCommandArgs args)
		{
			if (GameNetworkManager.singleton && GameNetworkManager.singleton.client != null && GameNetworkManager.singleton.client.connection != null)
			{
				GameNetworkManager.singleton.ClientSetPlayers(GameNetworkManager.singleton.client.connection);
			}
		}

		// Token: 0x06002007 RID: 8199 RVA: 0x0008B170 File Offset: 0x00089370
		[ConCommand(commandName = "ping", flags = ConVarFlags.None, helpText = "Prints the current round trip time from this client to the server and back.")]
		private static void CCPing(ConCommandArgs args)
		{
			GameNetworkManager singleton = GameNetworkManager.singleton;
			NetworkConnection networkConnection;
			if (singleton == null)
			{
				networkConnection = null;
			}
			else
			{
				NetworkClient client = singleton.client;
				networkConnection = ((client != null) ? client.connection : null);
			}
			NetworkConnection networkConnection2 = networkConnection;
			if (networkConnection2 != null)
			{
				Debug.LogFormat("rtt={0}ms smoothedFrame={1} smoothedFixed={2}", new object[]
				{
					RttManager.GetConnectionRTTInMilliseconds(networkConnection2),
					RttManager.GetConnectionFrameSmoothedRtt(networkConnection2),
					RttManager.GetConnectionFixedSmoothedRtt(networkConnection2)
				});
				return;
			}
			Debug.Log("No connection to server.");
		}

		// Token: 0x06002008 RID: 8200 RVA: 0x0008B1E4 File Offset: 0x000893E4
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

		// Token: 0x06002009 RID: 8201 RVA: 0x0008B2C4 File Offset: 0x000894C4
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

		// Token: 0x0600200A RID: 8202 RVA: 0x0008B318 File Offset: 0x00089518
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

		// Token: 0x0600200B RID: 8203 RVA: 0x0008B374 File Offset: 0x00089574
		[ConCommand(commandName = "disconnect", flags = ConVarFlags.None, helpText = "Disconnect from a server or shut down the current server.")]
		private static void CCDisconnect(ConCommandArgs args)
		{
			GameNetworkManager.singleton.desiredHost = default(GameNetworkManager.HostDescription);
		}

		// Token: 0x0600200C RID: 8204 RVA: 0x0008B394 File Offset: 0x00089594
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

		// Token: 0x0600200D RID: 8205 RVA: 0x0008B3D4 File Offset: 0x000895D4
		[ConCommand(commandName = "connect", flags = ConVarFlags.None, helpText = "Connect to a server.")]
		private static void CCConnect(ConCommandArgs args)
		{
			args.CheckArgumentCount(1);
			if (!GameNetworkManager.singleton)
			{
				return;
			}
			GameNetworkManager.EnsureNetworkManagerNotBusy();
			AddressPortPair addressPortPair;
			if (AddressPortPair.TryParse(args[0], out addressPortPair))
			{
				Debug.LogFormat("Parsed address={0} port={1}. Setting desired host.", new object[]
				{
					addressPortPair.address,
					addressPortPair.port
				});
				GameNetworkManager.singleton.desiredHost = new GameNetworkManager.HostDescription(addressPortPair);
				return;
			}
			Debug.LogFormat("Could not parse address and port from \"{0}\".", new object[]
			{
				args[0]
			});
		}

		// Token: 0x0600200E RID: 8206 RVA: 0x0008B460 File Offset: 0x00089660
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

		// Token: 0x0600200F RID: 8207 RVA: 0x0008B4F8 File Offset: 0x000896F8
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

		// Token: 0x06002010 RID: 8208 RVA: 0x0008B590 File Offset: 0x00089790
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

		// Token: 0x06002011 RID: 8209 RVA: 0x0008B694 File Offset: 0x00089894
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

		// Token: 0x06002012 RID: 8210 RVA: 0x0008B6DC File Offset: 0x000898DC
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

		// Token: 0x17000368 RID: 872
		// (get) Token: 0x06002013 RID: 8211 RVA: 0x0008B722 File Offset: 0x00089922
		// (set) Token: 0x06002014 RID: 8212 RVA: 0x0008B72A File Offset: 0x0008992A
		public bool isHost { get; private set; }

		// Token: 0x14000074 RID: 116
		// (add) Token: 0x06002015 RID: 8213 RVA: 0x0008B734 File Offset: 0x00089934
		// (remove) Token: 0x06002016 RID: 8214 RVA: 0x0008B768 File Offset: 0x00089968
		public static event Action onStartHostGlobal;

		// Token: 0x14000075 RID: 117
		// (add) Token: 0x06002017 RID: 8215 RVA: 0x0008B79C File Offset: 0x0008999C
		// (remove) Token: 0x06002018 RID: 8216 RVA: 0x0008B7D0 File Offset: 0x000899D0
		public static event Action onStopHostGlobal;

		// Token: 0x06002019 RID: 8217 RVA: 0x0008B803 File Offset: 0x00089A03
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

		// Token: 0x0600201A RID: 8218 RVA: 0x0008B821 File Offset: 0x00089A21
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

		// Token: 0x0600201B RID: 8219 RVA: 0x0008B840 File Offset: 0x00089A40
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

		// Token: 0x0600201C RID: 8220 RVA: 0x0008B8E0 File Offset: 0x00089AE0
		[NetworkMessageHandler(msgType = 54, client = true)]
		private static void HandleUpdateTime(NetworkMessage netMsg)
		{
			float num = netMsg.reader.ReadSingle();
			GameNetworkManager.singleton._unpredictedServerFixedTime = num;
			float num2 = Time.time - Time.fixedTime;
			GameNetworkManager.singleton._unpredictedServerFrameTime = num + num2;
		}

		// Token: 0x0600201D RID: 8221 RVA: 0x0008B920 File Offset: 0x00089B20
		[NetworkMessageHandler(msgType = 64, client = true, server = true)]
		private static void HandleTest(NetworkMessage netMsg)
		{
			int num = netMsg.reader.ReadInt32();
			Debug.LogFormat("Received test packet. value={0}", new object[]
			{
				num
			});
		}

		// Token: 0x0600201E RID: 8222 RVA: 0x0008B952 File Offset: 0x00089B52
		public static bool IsMemberInSteamLobby(CSteamID steamId)
		{
			return Client.Instance.Lobby.UserIsInCurrentLobby(steamId.value);
		}

		// Token: 0x0600201F RID: 8223 RVA: 0x0008B96C File Offset: 0x00089B6C
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

		// Token: 0x06002020 RID: 8224 RVA: 0x0008B9EC File Offset: 0x00089BEC
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

		// Token: 0x06002021 RID: 8225 RVA: 0x0008BA70 File Offset: 0x00089C70
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

		// Token: 0x06002022 RID: 8226 RVA: 0x0008BB14 File Offset: 0x00089D14
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
						this.ServerHandleClientDisconnect(steamNetworkConnection2);
					}
				}
			}
		}

		// Token: 0x06002023 RID: 8227 RVA: 0x0008BBC8 File Offset: 0x00089DC8
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

		// Token: 0x14000076 RID: 118
		// (add) Token: 0x06002024 RID: 8228 RVA: 0x0008BC54 File Offset: 0x00089E54
		// (remove) Token: 0x06002025 RID: 8229 RVA: 0x0008BC88 File Offset: 0x00089E88
		public static event Action onStartServerGlobal;

		// Token: 0x14000077 RID: 119
		// (add) Token: 0x06002026 RID: 8230 RVA: 0x0008BCBC File Offset: 0x00089EBC
		// (remove) Token: 0x06002027 RID: 8231 RVA: 0x0008BCF0 File Offset: 0x00089EF0
		public static event Action onStopServerGlobal;

		// Token: 0x14000078 RID: 120
		// (add) Token: 0x06002028 RID: 8232 RVA: 0x0008BD24 File Offset: 0x00089F24
		// (remove) Token: 0x06002029 RID: 8233 RVA: 0x0008BD58 File Offset: 0x00089F58
		public static event Action<NetworkConnection> onServerConnectGlobal;

		// Token: 0x14000079 RID: 121
		// (add) Token: 0x0600202A RID: 8234 RVA: 0x0008BD8C File Offset: 0x00089F8C
		// (remove) Token: 0x0600202B RID: 8235 RVA: 0x0008BDC0 File Offset: 0x00089FC0
		public static event Action<NetworkConnection> onServerDisconnectGlobal;

		// Token: 0x1400007A RID: 122
		// (add) Token: 0x0600202C RID: 8236 RVA: 0x0008BDF4 File Offset: 0x00089FF4
		// (remove) Token: 0x0600202D RID: 8237 RVA: 0x0008BE28 File Offset: 0x0008A028
		public static event Action<string> onServerSceneChangedGlobal;

		// Token: 0x0600202E RID: 8238 RVA: 0x0008BE5C File Offset: 0x0008A05C
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

		// Token: 0x0600202F RID: 8239 RVA: 0x0008BF04 File Offset: 0x0008A104
		public override void OnStartServer()
		{
			base.OnStartServer();
			NetworkMessageHandlerAttribute.RegisterServerMessages();
			this.InitializeTime();
			UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkSession"));
			Action action = GameNetworkManager.onStartServerGlobal;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06002030 RID: 8240 RVA: 0x0008BF38 File Offset: 0x0008A138
		public override void OnStopServer()
		{
			Action action = GameNetworkManager.onStopServerGlobal;
			if (action != null)
			{
				action();
			}
			for (int i = 0; i < NetworkServer.connections.Count; i++)
			{
				NetworkConnection networkConnection = NetworkServer.connections[i];
				if (networkConnection != null)
				{
					this.OnServerDisconnect(networkConnection);
				}
			}
			base.OnStopServer();
		}

		// Token: 0x06002031 RID: 8241 RVA: 0x0008BF86 File Offset: 0x0008A186
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

		// Token: 0x06002032 RID: 8242 RVA: 0x0008BFBC File Offset: 0x0008A1BC
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

		// Token: 0x06002033 RID: 8243 RVA: 0x0008C0D8 File Offset: 0x0008A2D8
		private void ServerHandleClientDisconnect(NetworkConnection conn)
		{
			this.OnServerDisconnect(conn);
			conn.InvokeHandlerNoData(33);
			conn.Disconnect();
			conn.Dispose();
			if (conn is SteamNetworkConnection)
			{
				NetworkServer.RemoveExternalConnection(conn.connectionId);
			}
		}

		// Token: 0x06002034 RID: 8244 RVA: 0x0008C10C File Offset: 0x0008A30C
		public void ServerKickClient(NetworkConnection conn, GameNetworkManager.KickReason reason)
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

		// Token: 0x06002035 RID: 8245 RVA: 0x0008C190 File Offset: 0x0008A390
		public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
		{
			this.OnServerAddPlayer(conn, playerControllerId, null);
		}

		// Token: 0x06002036 RID: 8246 RVA: 0x0008C19B File Offset: 0x0008A39B
		public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
		{
			this.OnServerAddPlayerInternal(conn, playerControllerId, extraMessageReader);
		}

		// Token: 0x06002037 RID: 8247 RVA: 0x0008C1A8 File Offset: 0x0008A3A8
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
			extraMessageReader.ReadMessage<GameNetworkManager.AddPlayerMessage>();
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
			Util.ConnectionIsLocal(conn);
			NetworkUserId id = NetworkUserId.FromIp(conn.address, (byte)playerControllerId);
			ClientAuthData clientAuthData = ServerAuthManager.FindAuthData(conn);
			CSteamID csteamID = (clientAuthData != null) ? clientAuthData.steamId : CSteamID.nil;
			if (csteamID != CSteamID.nil)
			{
				id = NetworkUserId.FromSteamId(csteamID.value, (byte)playerControllerId);
			}
			component.id = id;
			Chat.SendPlayerConnectedMessage(component);
			NetworkServer.AddPlayerForConnection(conn, gameObject, playerControllerId);
		}

		// Token: 0x06002038 RID: 8248 RVA: 0x0008C334 File Offset: 0x0008A534
		private void UpdateServer()
		{
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

		// Token: 0x06002039 RID: 8249 RVA: 0x0008C3B0 File Offset: 0x0008A5B0
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
				this.timeTransmitTimer += GameNetworkManager.svTimeTransmitInterval.value;
			}
			foreach (NetworkConnection networkConnection in NetworkServer.connections)
			{
				if (networkConnection != null && !Util.ConnectionIsLocal(networkConnection))
				{
					RttManager.Ping(networkConnection, QosChannelIndex.ping.intVal);
				}
			}
		}

		// Token: 0x0600203A RID: 8250 RVA: 0x0008C480 File Offset: 0x0008A680
		public override void OnServerSceneChanged(string sceneName)
		{
			base.OnServerSceneChanged(sceneName);
			if (Run.instance)
			{
				Run.instance.OnServerSceneChanged(sceneName);
			}
			Action<string> action = GameNetworkManager.onServerSceneChangedGlobal;
			if (action != null)
			{
				action(sceneName);
			}
			while (GameNetworkManager.clientsReadyDuringLevelTransition.Count > 0)
			{
				NetworkConnection networkConnection = GameNetworkManager.clientsReadyDuringLevelTransition.Dequeue();
				try
				{
					if (networkConnection.isConnected)
					{
						this.OnServerReady(networkConnection);
					}
				}
				catch (Exception ex)
				{
					Debug.LogErrorFormat("OnServerReady could not be called for client: {0}", new object[]
					{
						ex.Message
					});
				}
			}
		}

		// Token: 0x0600203B RID: 8251 RVA: 0x0008C514 File Offset: 0x0008A714
		private bool IsServerAtMaxConnections()
		{
			ReadOnlyCollection<NetworkConnection> connections = NetworkServer.connections;
			if (connections.Count >= base.maxConnections)
			{
				int num = 0;
				for (int i = 0; i < connections.Count; i++)
				{
					if (connections[i] != null)
					{
						num++;
					}
				}
				return num >= base.maxConnections;
			}
			return false;
		}

		// Token: 0x0600203C RID: 8252 RVA: 0x0008C564 File Offset: 0x0008A764
		private NetworkUser FindNetworkUserForConnectionServer(NetworkConnection connection)
		{
			ReadOnlyCollection<NetworkUser> readOnlyInstancesList = NetworkUser.readOnlyInstancesList;
			int count = readOnlyInstancesList.Count;
			for (int i = 0; i < count; i++)
			{
				NetworkUser networkUser = readOnlyInstancesList[i];
				if (networkUser.connectionToClient == connection)
				{
					return networkUser;
				}
			}
			return null;
		}

		// Token: 0x0600203D RID: 8253 RVA: 0x0008C5A0 File Offset: 0x0008A7A0
		public int GetConnectingClientCount()
		{
			int num = 0;
			ReadOnlyCollection<NetworkConnection> connections = NetworkServer.connections;
			int count = connections.Count;
			for (int i = 0; i < count; i++)
			{
				NetworkConnection networkConnection = connections[i];
				if (networkConnection != null && !this.FindNetworkUserForConnectionServer(networkConnection))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x0600203E RID: 8254 RVA: 0x0008C5E9 File Offset: 0x0008A7E9
		public void RequestServerShutdown()
		{
			if (this.serverShuttingDown)
			{
				return;
			}
			this.serverShuttingDown = true;
			base.StartCoroutine(this.ServerShutdownCoroutine());
		}

		// Token: 0x0600203F RID: 8255 RVA: 0x0008C608 File Offset: 0x0008A808
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
			while (t < maxWait && !GameNetworkManager.<ServerShutdownCoroutine>g__CheckConnectionsEmpty|178_0())
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

		// Token: 0x06002040 RID: 8256 RVA: 0x0008C617 File Offset: 0x0008A817
		private static void ServerHandleReady(NetworkMessage netMsg)
		{
			if (GameNetworkManager.isLoadingScene)
			{
				GameNetworkManager.clientsReadyDuringLevelTransition.Enqueue(netMsg.conn);
				Debug.Log("Client readied during a level transition! Queuing their request.");
				return;
			}
			GameNetworkManager.singleton.OnServerReady(netMsg.conn);
			Debug.Log("Client ready.");
		}

		// Token: 0x06002041 RID: 8257 RVA: 0x0008C655 File Offset: 0x0008A855
		private void RegisterServerOverrideMessages()
		{
			NetworkServer.RegisterHandler(35, new NetworkMessageDelegate(GameNetworkManager.ServerHandleReady));
		}

		// Token: 0x06002042 RID: 8258 RVA: 0x0008C66A File Offset: 0x0008A86A
		public override void ServerChangeScene(string newSceneName)
		{
			this.RegisterServerOverrideMessages();
			base.ServerChangeScene(newSceneName);
		}

		// Token: 0x06002044 RID: 8260 RVA: 0x0008C698 File Offset: 0x0008A898
		[CompilerGenerated]
		internal static bool <ServerShutdownCoroutine>g__CheckConnectionsEmpty|178_0()
		{
			foreach (NetworkConnection networkConnection in NetworkServer.connections)
			{
				if (networkConnection != null && !Util.ConnectionIsLocal(networkConnection))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x04001D73 RID: 7539
		private static readonly FieldInfo loadingSceneAsyncFieldInfo;

		// Token: 0x04001D74 RID: 7540
		private float _unpredictedServerFixedTime;

		// Token: 0x04001D75 RID: 7541
		private float _unpredictedServerFixedTimeSmoothed;

		// Token: 0x04001D76 RID: 7542
		private float unpredictedServerFixedTimeVelocity;

		// Token: 0x04001D77 RID: 7543
		private float _unpredictedServerFrameTime;

		// Token: 0x04001D78 RID: 7544
		private float _unpredictedServerFrameTimeSmoothed;

		// Token: 0x04001D79 RID: 7545
		private float unpredictedServerFrameTimeVelocity;

		// Token: 0x04001D7A RID: 7546
		private static FloatConVar cvNetTimeSmoothRate = new FloatConVar("net_time_smooth_rate", ConVarFlags.None, "1.05", "The smoothing rate for the network time.");

		// Token: 0x04001D7B RID: 7547
		private static readonly string[] spawnableFolders = new string[]
		{
			"CharacterBodies",
			"CharacterMasters",
			"Projectiles",
			"NetworkedObjects",
			"GameModes"
		};

		// Token: 0x04001D7D RID: 7549
		public float debugServerTime;

		// Token: 0x04001D7E RID: 7550
		public float debugRTT;

		// Token: 0x04001D7F RID: 7551
		private bool actedUponDesiredHost;

		// Token: 0x04001D80 RID: 7552
		private float lastDesiredHostSetTime = float.NegativeInfinity;

		// Token: 0x04001D81 RID: 7553
		private GameNetworkManager.HostDescription _desiredHost;

		// Token: 0x04001D87 RID: 7559
		private static bool wasFading = false;

		// Token: 0x04001D8C RID: 7564
		private static readonly string[] sceneWhiteList = new string[]
		{
			"title",
			"crystalworld",
			"logbook"
		};

		// Token: 0x04001D8D RID: 7565
		private static readonly StringConVar cvSvPassword = new StringConVar("sv_password", ConVarFlags.None, "", "The password clients must provide before joining the server.");

		// Token: 0x04001D8E RID: 7566
		private static readonly StringConVar cvClPassword = new StringConVar("cl_password", ConVarFlags.None, "", "The password to use when joining a passworded server.");

		// Token: 0x04001D92 RID: 7570
		private List<ulong> steamIdBanList = new List<ulong>();

		// Token: 0x04001D98 RID: 7576
		public Server steamworksServer;

		// Token: 0x04001D99 RID: 7577
		private static readonly FloatConVar svTimeTransmitInterval = new FloatConVar("sv_time_transmit_interval", ConVarFlags.Cheat, 0.016666668f.ToString(), "How long it takes for the server to issue a time update to clients.");

		// Token: 0x04001D9A RID: 7578
		private float timeTransmitTimer;

		// Token: 0x04001D9B RID: 7579
		private bool serverShuttingDown;

		// Token: 0x04001D9C RID: 7580
		private static readonly Queue<NetworkConnection> clientsReadyDuringLevelTransition = new Queue<NetworkConnection>();

		// Token: 0x02000545 RID: 1349
		public struct HostDescription : IEquatable<GameNetworkManager.HostDescription>
		{
			// Token: 0x06002045 RID: 8261 RVA: 0x0008C6F0 File Offset: 0x0008A8F0
			public HostDescription(CSteamID steamId)
			{
				this = default(GameNetworkManager.HostDescription);
				this.hostType = GameNetworkManager.HostDescription.HostType.Steam;
				this.steamId = steamId;
			}

			// Token: 0x06002046 RID: 8262 RVA: 0x0008C707 File Offset: 0x0008A907
			public HostDescription(AddressPortPair addressPortPair)
			{
				this = default(GameNetworkManager.HostDescription);
				this.hostType = GameNetworkManager.HostDescription.HostType.IPv4;
				this.addressPortPair = addressPortPair;
			}

			// Token: 0x06002047 RID: 8263 RVA: 0x0008C71E File Offset: 0x0008A91E
			public HostDescription(GameNetworkManager.HostDescription.HostingParameters hostingParameters)
			{
				this = default(GameNetworkManager.HostDescription);
				this.hostType = GameNetworkManager.HostDescription.HostType.Self;
				this.hostingParameters = hostingParameters;
			}

			// Token: 0x06002048 RID: 8264 RVA: 0x0008C738 File Offset: 0x0008A938
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

			// Token: 0x06002049 RID: 8265 RVA: 0x0008C803 File Offset: 0x0008AA03
			private HostDescription(object o)
			{
				this = default(GameNetworkManager.HostDescription);
				this.hostType = GameNetworkManager.HostDescription.HostType.None;
			}

			// Token: 0x0600204A RID: 8266 RVA: 0x0008C814 File Offset: 0x0008AA14
			public bool Equals(GameNetworkManager.HostDescription other)
			{
				return this.hostType == other.hostType && this.steamId.Equals(other.steamId) && this.addressPortPair.Equals(other.addressPortPair) && this.hostingParameters.Equals(other.hostingParameters);
			}

			// Token: 0x0600204B RID: 8267 RVA: 0x0008C874 File Offset: 0x0008AA74
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

			// Token: 0x0600204C RID: 8268 RVA: 0x0008C8A0 File Offset: 0x0008AAA0
			public override int GetHashCode()
			{
				return (int)(((this.hostType * (GameNetworkManager.HostDescription.HostType)397 ^ (GameNetworkManager.HostDescription.HostType)this.steamId.GetHashCode()) * (GameNetworkManager.HostDescription.HostType)397 ^ (GameNetworkManager.HostDescription.HostType)this.addressPortPair.GetHashCode()) * (GameNetworkManager.HostDescription.HostType)397 ^ (GameNetworkManager.HostDescription.HostType)this.hostingParameters.GetHashCode());
			}

			// Token: 0x0600204D RID: 8269 RVA: 0x0008C904 File Offset: 0x0008AB04
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

			// Token: 0x04001D9D RID: 7581
			public readonly GameNetworkManager.HostDescription.HostType hostType;

			// Token: 0x04001D9E RID: 7582
			public readonly CSteamID steamId;

			// Token: 0x04001D9F RID: 7583
			public readonly AddressPortPair addressPortPair;

			// Token: 0x04001DA0 RID: 7584
			public readonly GameNetworkManager.HostDescription.HostingParameters hostingParameters;

			// Token: 0x04001DA1 RID: 7585
			public static readonly GameNetworkManager.HostDescription none = new GameNetworkManager.HostDescription(null);

			// Token: 0x04001DA2 RID: 7586
			private static readonly StringBuilder sharedStringBuilder = new StringBuilder();

			// Token: 0x02000546 RID: 1350
			public enum HostType
			{
				// Token: 0x04001DA4 RID: 7588
				None,
				// Token: 0x04001DA5 RID: 7589
				Self,
				// Token: 0x04001DA6 RID: 7590
				Steam,
				// Token: 0x04001DA7 RID: 7591
				IPv4
			}

			// Token: 0x02000547 RID: 1351
			public struct HostingParameters : IEquatable<GameNetworkManager.HostDescription.HostingParameters>
			{
				// Token: 0x0600204F RID: 8271 RVA: 0x0008CA3B File Offset: 0x0008AC3B
				public bool Equals(GameNetworkManager.HostDescription.HostingParameters other)
				{
					return this.listen == other.listen && this.maxPlayers == other.maxPlayers;
				}

				// Token: 0x06002050 RID: 8272 RVA: 0x0008CA5C File Offset: 0x0008AC5C
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

				// Token: 0x06002051 RID: 8273 RVA: 0x0008CA88 File Offset: 0x0008AC88
				public override int GetHashCode()
				{
					return this.listen.GetHashCode() * 397 ^ this.maxPlayers;
				}

				// Token: 0x04001DA8 RID: 7592
				public bool listen;

				// Token: 0x04001DA9 RID: 7593
				public int maxPlayers;
			}
		}

		// Token: 0x02000548 RID: 1352
		private class NetLogLevelConVar : BaseConVar
		{
			// Token: 0x06002052 RID: 8274 RVA: 0x0000972B File Offset: 0x0000792B
			public NetLogLevelConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06002053 RID: 8275 RVA: 0x0008CAA4 File Offset: 0x0008ACA4
			public override void SetString(string newValue)
			{
				int currentLogLevel;
				if (TextSerialization.TryParseInvariant(newValue, out currentLogLevel))
				{
					LogFilter.currentLogLevel = currentLogLevel;
				}
			}

			// Token: 0x06002054 RID: 8276 RVA: 0x0008CAC1 File Offset: 0x0008ACC1
			public override string GetString()
			{
				return TextSerialization.ToStringInvariant(LogFilter.currentLogLevel);
			}

			// Token: 0x04001DAA RID: 7594
			private static GameNetworkManager.NetLogLevelConVar cvNetLogLevel = new GameNetworkManager.NetLogLevelConVar("net_loglevel", ConVarFlags.Engine, null, "Network log verbosity.");
		}

		// Token: 0x02000549 RID: 1353
		private class SvListenConVar : BaseConVar
		{
			// Token: 0x06002056 RID: 8278 RVA: 0x0000972B File Offset: 0x0000792B
			public SvListenConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06002057 RID: 8279 RVA: 0x0008CAE8 File Offset: 0x0008ACE8
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

			// Token: 0x06002058 RID: 8280 RVA: 0x0008CB1A File Offset: 0x0008AD1A
			public override string GetString()
			{
				if (!NetworkServer.dontListen)
				{
					return "1";
				}
				return "0";
			}

			// Token: 0x04001DAB RID: 7595
			private static GameNetworkManager.SvListenConVar cvSvListen = new GameNetworkManager.SvListenConVar("sv_listen", ConVarFlags.Engine, null, "Whether or not the server will accept connections from other players.");
		}

		// Token: 0x0200054A RID: 1354
		private class SvMaxPlayersConVar : BaseConVar
		{
			// Token: 0x0600205A RID: 8282 RVA: 0x0000972B File Offset: 0x0000792B
			public SvMaxPlayersConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x0600205B RID: 8283 RVA: 0x0008CB48 File Offset: 0x0008AD48
			public override void SetString(string newValue)
			{
				if (NetworkServer.active)
				{
					throw new ConCommandException("Cannot change this convar while the server is running.");
				}
				int val;
				if (NetworkManager.singleton && TextSerialization.TryParseInvariant(newValue, out val))
				{
					NetworkManager.singleton.maxConnections = Math.Min(Math.Max(val, 1), RoR2Application.hardMaxPlayers);
				}
			}

			// Token: 0x0600205C RID: 8284 RVA: 0x0008CB98 File Offset: 0x0008AD98
			public override string GetString()
			{
				if (!NetworkManager.singleton)
				{
					return "1";
				}
				return TextSerialization.ToStringInvariant(NetworkManager.singleton.maxConnections);
			}

			// Token: 0x17000369 RID: 873
			// (get) Token: 0x0600205D RID: 8285 RVA: 0x0008CBBB File Offset: 0x0008ADBB
			public int intValue
			{
				get
				{
					return NetworkManager.singleton.maxConnections;
				}
			}

			// Token: 0x04001DAC RID: 7596
			public static readonly GameNetworkManager.SvMaxPlayersConVar instance = new GameNetworkManager.SvMaxPlayersConVar("sv_maxplayers", ConVarFlags.Engine, null, "Maximum number of players allowed.");
		}

		// Token: 0x0200054B RID: 1355
		private class KickMessage : MessageBase
		{
			// Token: 0x1700036A RID: 874
			// (get) Token: 0x0600205F RID: 8287 RVA: 0x0008CBE0 File Offset: 0x0008ADE0
			// (set) Token: 0x06002060 RID: 8288 RVA: 0x0008CBE8 File Offset: 0x0008ADE8
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

			// Token: 0x06002061 RID: 8289 RVA: 0x0008CBF4 File Offset: 0x0008ADF4
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

			// Token: 0x06002063 RID: 8291 RVA: 0x0008CC5D File Offset: 0x0008AE5D
			public override void Serialize(NetworkWriter writer)
			{
				writer.WritePackedUInt32((uint)this.netReason);
			}

			// Token: 0x06002064 RID: 8292 RVA: 0x0008CC6B File Offset: 0x0008AE6B
			public override void Deserialize(NetworkReader reader)
			{
				this.netReason = (int)reader.ReadPackedUInt32();
			}

			// Token: 0x04001DAD RID: 7597
			public int netReason;
		}

		// Token: 0x0200054C RID: 1356
		protected class AddPlayerMessage : MessageBase
		{
			// Token: 0x06002066 RID: 8294 RVA: 0x0008CC79 File Offset: 0x0008AE79
			public override void Serialize(NetworkWriter writer)
			{
				writer.WritePackedUInt64(this.steamId);
				writer.WriteBytesFull(this.steamAuthTicketData);
			}

			// Token: 0x06002067 RID: 8295 RVA: 0x0008CC93 File Offset: 0x0008AE93
			public override void Deserialize(NetworkReader reader)
			{
				this.steamId = reader.ReadPackedUInt64();
				this.steamAuthTicketData = reader.ReadBytesAndSize();
			}

			// Token: 0x04001DAE RID: 7598
			public ulong steamId;

			// Token: 0x04001DAF RID: 7599
			public byte[] steamAuthTicketData;
		}

		// Token: 0x0200054D RID: 1357
		public enum KickReason : uint
		{
			// Token: 0x04001DB1 RID: 7601
			Unspecified,
			// Token: 0x04001DB2 RID: 7602
			ServerShutdown,
			// Token: 0x04001DB3 RID: 7603
			Timeout,
			// Token: 0x04001DB4 RID: 7604
			Kick,
			// Token: 0x04001DB5 RID: 7605
			Ban,
			// Token: 0x04001DB6 RID: 7606
			BadPassword,
			// Token: 0x04001DB7 RID: 7607
			BadVersion,
			// Token: 0x04001DB8 RID: 7608
			ServerFull,
			// Token: 0x04001DB9 RID: 7609
			MalformedAuthData
		}

		// Token: 0x0200054E RID: 1358
		public class SvHostNameConVar : BaseConVar
		{
			// Token: 0x1400007B RID: 123
			// (add) Token: 0x06002068 RID: 8296 RVA: 0x0008CCB0 File Offset: 0x0008AEB0
			// (remove) Token: 0x06002069 RID: 8297 RVA: 0x0008CCE8 File Offset: 0x0008AEE8
			public event Action<string> onValueChanged;

			// Token: 0x0600206A RID: 8298 RVA: 0x0008CD1D File Offset: 0x0008AF1D
			public SvHostNameConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x0600206B RID: 8299 RVA: 0x0008CD35 File Offset: 0x0008AF35
			public override void SetString(string newValue)
			{
				this.value = newValue;
				Action<string> action = this.onValueChanged;
				if (action == null)
				{
					return;
				}
				action(newValue);
			}

			// Token: 0x0600206C RID: 8300 RVA: 0x0008CD4F File Offset: 0x0008AF4F
			public override string GetString()
			{
				return this.value;
			}

			// Token: 0x04001DBA RID: 7610
			public static readonly GameNetworkManager.SvHostNameConVar instance = new GameNetworkManager.SvHostNameConVar("sv_hostname", ConVarFlags.None, "NAME", "The public name to use for the server if hosting.");

			// Token: 0x04001DBB RID: 7611
			private string value = "NAME";
		}

		// Token: 0x0200054F RID: 1359
		public class SvPortConVar : BaseConVar
		{
			// Token: 0x1700036B RID: 875
			// (get) Token: 0x0600206E RID: 8302 RVA: 0x0008CD73 File Offset: 0x0008AF73
			public ushort value
			{
				get
				{
					return (ushort)GameNetworkManager.singleton.networkPort;
				}
			}

			// Token: 0x0600206F RID: 8303 RVA: 0x0000972B File Offset: 0x0000792B
			public SvPortConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06002070 RID: 8304 RVA: 0x0008CD80 File Offset: 0x0008AF80
			public override void SetString(string newValueString)
			{
				if (NetworkServer.active)
				{
					throw new ConCommandException("Cannot change this convar while the server is running.");
				}
				ushort networkPort;
				if (TextSerialization.TryParseInvariant(newValueString, out networkPort))
				{
					GameNetworkManager.singleton.networkPort = (int)networkPort;
				}
			}

			// Token: 0x06002071 RID: 8305 RVA: 0x0008CDB4 File Offset: 0x0008AFB4
			public override string GetString()
			{
				return this.value.ToString();
			}

			// Token: 0x04001DBD RID: 7613
			public static readonly GameNetworkManager.SvPortConVar instance = new GameNetworkManager.SvPortConVar("sv_port", ConVarFlags.Engine, null, "The port to use for the server if hosting.");
		}

		// Token: 0x02000550 RID: 1360
		public class SvIPConVar : BaseConVar
		{
			// Token: 0x06002073 RID: 8307 RVA: 0x0000972B File Offset: 0x0000792B
			public SvIPConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06002074 RID: 8308 RVA: 0x0008CDE8 File Offset: 0x0008AFE8
			public override void SetString(string newValueString)
			{
				if (NetworkServer.active)
				{
					throw new ConCommandException("Cannot change this convar while the server is running.");
				}
				GameNetworkManager.singleton.serverBindAddress = newValueString;
			}

			// Token: 0x06002075 RID: 8309 RVA: 0x0008CE07 File Offset: 0x0008B007
			public override string GetString()
			{
				return GameNetworkManager.singleton.serverBindAddress;
			}

			// Token: 0x04001DBE RID: 7614
			public static readonly GameNetworkManager.SvIPConVar instance = new GameNetworkManager.SvIPConVar("sv_ip", ConVarFlags.Engine, null, "The IP for the server to bind to if hosting.");
		}

		// Token: 0x02000551 RID: 1361
		public class SvPasswordConVar : BaseConVar
		{
			// Token: 0x1700036C RID: 876
			// (get) Token: 0x06002077 RID: 8311 RVA: 0x0008CE2C File Offset: 0x0008B02C
			// (set) Token: 0x06002078 RID: 8312 RVA: 0x0008CE34 File Offset: 0x0008B034
			public string value { get; private set; }

			// Token: 0x1400007C RID: 124
			// (add) Token: 0x06002079 RID: 8313 RVA: 0x0008CE40 File Offset: 0x0008B040
			// (remove) Token: 0x0600207A RID: 8314 RVA: 0x0008CE78 File Offset: 0x0008B078
			public event Action<string> onValueChanged;

			// Token: 0x0600207B RID: 8315 RVA: 0x0000972B File Offset: 0x0000792B
			public SvPasswordConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x0600207C RID: 8316 RVA: 0x0008CEAD File Offset: 0x0008B0AD
			public override void SetString(string newValue)
			{
				if (newValue == null)
				{
					newValue = "";
				}
				if (this.value == newValue)
				{
					return;
				}
				this.value = newValue;
				Action<string> action = this.onValueChanged;
				if (action == null)
				{
					return;
				}
				action(this.value);
			}

			// Token: 0x0600207D RID: 8317 RVA: 0x0008CEE5 File Offset: 0x0008B0E5
			public override string GetString()
			{
				return this.value;
			}

			// Token: 0x04001DBF RID: 7615
			public static readonly GameNetworkManager.SvPasswordConVar instance = new GameNetworkManager.SvPasswordConVar("sv_password", ConVarFlags.None, "", "The password to use for the server if hosting.");
		}
	}
}
