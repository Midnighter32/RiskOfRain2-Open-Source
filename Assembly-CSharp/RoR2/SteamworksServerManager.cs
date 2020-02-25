using System;
using System.Net;
using Facepunch.Steamworks;
using RoR2.ConVar;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace RoR2
{
	// Token: 0x02000448 RID: 1096
	internal sealed class SteamworksServerManager : IDisposable
	{
		// Token: 0x06001AAE RID: 6830 RVA: 0x000712D0 File Offset: 0x0006F4D0
		private SteamworksServerManager()
		{
			string modDir = "Risk of Rain 2";
			string gameDesc = "Risk of Rain 2";
			this.steamworksServer = new Server(632360U, new ServerInit(modDir, gameDesc)
			{
				IpAddress = IPAddress.Any,
				Secure = true,
				VersionString = "0.0.0.0",
				GamePort = GameNetworkManager.SvPortConVar.instance.value,
				QueryPort = SteamworksServerManager.cvSteamServerQueryPort.value,
				SteamPort = SteamworksServerManager.cvSteamServerSteamPort.value
			});
			Debug.LogFormat("steamworksServer.IsValid={0}", new object[]
			{
				this.steamworksServer.IsValid
			});
			if (!this.steamworksServer.IsValid)
			{
				this.Dispose();
				return;
			}
			this.steamworksServer.Auth.OnAuthChange = new Action<ulong, ulong, ServerAuth.Status>(this.OnAuthChange);
			this.steamworksServer.MaxPlayers = this.GetMaxPlayers();
			this.UpdateHostName(GameNetworkManager.SvHostNameConVar.instance.GetString());
			GameNetworkManager.SvHostNameConVar.instance.onValueChanged += this.UpdateHostName;
			this.UpdateMapName(SceneManager.GetActiveScene().name);
			GameNetworkManager.onServerSceneChangedGlobal += this.UpdateMapName;
			this.UpdatePassword(GameNetworkManager.SvPasswordConVar.instance.value);
			GameNetworkManager.SvPasswordConVar.instance.onValueChanged += this.UpdatePassword;
			this.steamworksServer.DedicatedServer = false;
			this.steamworksServer.AutomaticHeartbeats = SteamworksServerManager.SteamServerHeartbeatEnabledConVar.instance.value;
			this.steamworksServer.LogOnAnonymous();
			Debug.LogFormat("steamworksServer.LoggedOn={0}", new object[]
			{
				this.steamworksServer.LoggedOn
			});
			RoR2Application.onUpdate += this.Update;
			GameNetworkManager.onServerConnectGlobal += this.OnServerConnectClient;
			GameNetworkManager.onServerDisconnectGlobal += this.OnServerDisconnectClient;
			ServerAuthManager.onAuthDataReceivedFromClient += this.OnAuthDataReceivedFromClient;
			ServerAuthManager.onAuthExpired += this.OnAuthExpired;
			this.steamworksServer.ForceHeartbeat();
		}

		// Token: 0x06001AAF RID: 6831 RVA: 0x000714E4 File Offset: 0x0006F6E4
		private void OnAuthExpired(NetworkConnection conn, ClientAuthData authData)
		{
			if (authData != null)
			{
				CSteamID steamId = authData.steamId;
				SteamNetworkConnection steamNetworkConnection;
				if ((steamNetworkConnection = (conn as SteamNetworkConnection)) != null)
				{
					steamId = steamNetworkConnection.steamId;
				}
				this.steamworksServer.Auth.EndSession(steamId.value);
			}
		}

		// Token: 0x06001AB0 RID: 6832 RVA: 0x00071524 File Offset: 0x0006F724
		private void OnAuthDataReceivedFromClient(NetworkConnection conn, ClientAuthData authData)
		{
			CSteamID steamId = authData.steamId;
			SteamNetworkConnection steamNetworkConnection;
			if ((steamNetworkConnection = (conn as SteamNetworkConnection)) != null)
			{
				steamId = steamNetworkConnection.steamId;
			}
			this.steamworksServer.Auth.StartSession(authData.authTicket, steamId.value);
		}

		// Token: 0x06001AB1 RID: 6833 RVA: 0x0000409B File Offset: 0x0000229B
		private void OnServerConnectClient(NetworkConnection conn)
		{
		}

		// Token: 0x06001AB2 RID: 6834 RVA: 0x0000409B File Offset: 0x0000229B
		private void OnServerDisconnectClient(NetworkConnection conn)
		{
		}

		// Token: 0x06001AB3 RID: 6835 RVA: 0x00071568 File Offset: 0x0006F768
		public void Dispose()
		{
			if (this.disposed)
			{
				return;
			}
			this.disposed = true;
			Server server = this.steamworksServer;
			if (server != null)
			{
				server.Dispose();
			}
			this.steamworksServer = null;
			RoR2Application.onUpdate -= this.Update;
			GameNetworkManager.SvHostNameConVar.instance.onValueChanged -= this.UpdateHostName;
			GameNetworkManager.SvPasswordConVar.instance.onValueChanged -= this.UpdatePassword;
			GameNetworkManager.onServerSceneChangedGlobal -= this.UpdateMapName;
			GameNetworkManager.onServerConnectGlobal -= this.OnServerConnectClient;
			GameNetworkManager.onServerDisconnectGlobal -= this.OnServerDisconnectClient;
			ServerAuthManager.onAuthDataReceivedFromClient -= this.OnAuthDataReceivedFromClient;
			ServerAuthManager.onAuthExpired -= this.OnAuthExpired;
		}

		// Token: 0x06001AB4 RID: 6836 RVA: 0x0007162F File Offset: 0x0006F82F
		private int GetMaxPlayers()
		{
			return GameNetworkManager.singleton.maxConnections;
		}

		// Token: 0x06001AB5 RID: 6837 RVA: 0x0007163B File Offset: 0x0006F83B
		private void UpdateHostName(string newHostName)
		{
			this.steamworksServer.ServerName = newHostName;
		}

		// Token: 0x06001AB6 RID: 6838 RVA: 0x00071649 File Offset: 0x0006F849
		private void UpdateMapName(string sceneName)
		{
			this.steamworksServer.MapName = sceneName;
		}

		// Token: 0x06001AB7 RID: 6839 RVA: 0x00071657 File Offset: 0x0006F857
		private void UpdatePassword(string newPassword)
		{
			this.steamworksServer.Passworded = (newPassword.Length > 0);
		}

		// Token: 0x06001AB8 RID: 6840 RVA: 0x0007166D File Offset: 0x0006F86D
		private void OnAddressDiscovered()
		{
			Debug.LogFormat("Steamworks Server IP discovered: {0}", new object[]
			{
				this.address
			});
		}

		// Token: 0x06001AB9 RID: 6841 RVA: 0x00071688 File Offset: 0x0006F888
		private void RefreshSteamServerPlayers()
		{
			foreach (NetworkUser networkUser in NetworkUser.readOnlyInstancesList)
			{
				ClientAuthData clientAuthData = ServerAuthManager.FindAuthData(networkUser.connectionToClient);
				if (clientAuthData != null)
				{
					this.steamworksServer.UpdatePlayer(clientAuthData.steamId.value, networkUser.userName, 0);
				}
			}
		}

		// Token: 0x06001ABA RID: 6842 RVA: 0x000716FC File Offset: 0x0006F8FC
		private void Update()
		{
			this.steamworksServer.Update();
			this.playerUpdateTimer -= Time.unscaledDeltaTime;
			if (this.playerUpdateTimer <= 0f)
			{
				this.playerUpdateTimer = this.playerUpdateInterval;
				this.RefreshSteamServerPlayers();
			}
			if (this.address == null)
			{
				this.address = this.steamworksServer.PublicIp;
				if (this.address != null)
				{
					this.OnAddressDiscovered();
				}
			}
		}

		// Token: 0x06001ABB RID: 6843 RVA: 0x0007176C File Offset: 0x0006F96C
		private void OnAuthChange(ulong steamId, ulong ownerId, ServerAuth.Status status)
		{
			NetworkConnection conn = new NetworkConnection();
			ServerAuthManager.FindConnectionForSteamID(new CSteamID(steamId));
			switch (status)
			{
			case ServerAuth.Status.OK:
				return;
			case ServerAuth.Status.UserNotConnectedToSteam:
			case ServerAuth.Status.NoLicenseOrExpired:
			case ServerAuth.Status.VACBanned:
			case ServerAuth.Status.LoggedInElseWhere:
			case ServerAuth.Status.VACCheckTimedOut:
			case ServerAuth.Status.AuthTicketCanceled:
			case ServerAuth.Status.AuthTicketInvalidAlreadyUsed:
			case ServerAuth.Status.AuthTicketInvalid:
			case ServerAuth.Status.PublisherIssuedBan:
				GameNetworkManager.singleton.ServerKickClient(conn, GameNetworkManager.KickReason.Unspecified);
				return;
			default:
				throw new ArgumentOutOfRangeException("status", status, null);
			}
		}

		// Token: 0x06001ABC RID: 6844 RVA: 0x000717DA File Offset: 0x0006F9DA
		[SystemInitializer(new Type[]
		{

		})]
		private static void Init()
		{
			GameNetworkManager.onStartServerGlobal += SteamworksServerManager.OnStartServer;
			GameNetworkManager.onStopServerGlobal += SteamworksServerManager.OnStopServer;
		}

		// Token: 0x06001ABD RID: 6845 RVA: 0x000717FE File Offset: 0x0006F9FE
		private static void OnStartServer()
		{
			SteamworksServerManager steamworksServerManager = SteamworksServerManager.instance;
			if (steamworksServerManager != null)
			{
				steamworksServerManager.Dispose();
			}
			SteamworksServerManager.instance = null;
			if (NetworkServer.dontListen)
			{
				return;
			}
			SteamworksServerManager.instance = new SteamworksServerManager();
			if (SteamworksServerManager.instance.disposed)
			{
				SteamworksServerManager.instance = null;
			}
		}

		// Token: 0x06001ABE RID: 6846 RVA: 0x0007183A File Offset: 0x0006FA3A
		private static void OnStopServer()
		{
			SteamworksServerManager steamworksServerManager = SteamworksServerManager.instance;
			if (steamworksServerManager != null)
			{
				steamworksServerManager.Dispose();
			}
			SteamworksServerManager.instance = null;
		}

		// Token: 0x06001ABF RID: 6847 RVA: 0x00071852 File Offset: 0x0006FA52
		[ConCommand(commandName = "steam_server_force_heartbeat", flags = ConVarFlags.None, helpText = "Forces the server to issue a heartbeat to the master server.")]
		private static void CCSteamServerForceHeartbeat(ConCommandArgs args)
		{
			SteamworksServerManager steamworksServerManager = SteamworksServerManager.instance;
			Server server = (steamworksServerManager != null) ? steamworksServerManager.steamworksServer : null;
			if (server == null)
			{
				throw new ConCommandException("No Steamworks server is running.");
			}
			server.ForceHeartbeat();
		}

		// Token: 0x06001AC0 RID: 6848 RVA: 0x00071878 File Offset: 0x0006FA78
		[ConCommand(commandName = "steam_server_print_info", flags = ConVarFlags.None, helpText = "Prints debug info about the currently hosted Steamworks server.")]
		private static void CCSteamServerPrintInfo(ConCommandArgs args)
		{
			SteamworksServerManager steamworksServerManager = SteamworksServerManager.instance;
			Server server = (steamworksServerManager != null) ? steamworksServerManager.steamworksServer : null;
			if (server == null)
			{
				throw new ConCommandException("No Steamworks server is running.");
			}
			Debug.Log("" + string.Format("IsValid={0}\n", server.IsValid) + string.Format("Product={0}\n", server.Product) + string.Format("ModDir={0}\n", server.ModDir) + string.Format("SteamId={0}\n", server.SteamId) + string.Format("DedicatedServer={0}\n", server.DedicatedServer) + string.Format("LoggedOn={0}\n", server.LoggedOn) + string.Format("ServerName={0}\n", server.ServerName) + string.Format("PublicIp={0}\n", server.PublicIp) + string.Format("Passworded={0}\n", server.Passworded) + string.Format("MaxPlayers={0}\n", server.MaxPlayers) + string.Format("BotCount={0}\n", server.BotCount) + string.Format("MapName={0}\n", server.MapName) + string.Format("GameDescription={0}\n", server.GameDescription) + string.Format("GameTags={0}\n", server.GameTags));
		}

		// Token: 0x0400183E RID: 6206
		private static SteamworksServerManager instance;

		// Token: 0x0400183F RID: 6207
		private Server steamworksServer;

		// Token: 0x04001840 RID: 6208
		private bool disposed;

		// Token: 0x04001841 RID: 6209
		private IPAddress address;

		// Token: 0x04001842 RID: 6210
		private float playerUpdateTimer;

		// Token: 0x04001843 RID: 6211
		private float playerUpdateInterval = 5f;

		// Token: 0x04001844 RID: 6212
		private static readonly SteamworksServerManager.SteamServerPortConVar cvSteamServerQueryPort = new SteamworksServerManager.SteamServerPortConVar("steam_server_query_port", ConVarFlags.Engine, "27016", "The port for queries.");

		// Token: 0x04001845 RID: 6213
		private static readonly SteamworksServerManager.SteamServerPortConVar cvSteamServerSteamPort = new SteamworksServerManager.SteamServerPortConVar("steam_server_steam_port", ConVarFlags.Engine, "0", "The port for steam. 0 for a random port in the range 10000-60000.");

		// Token: 0x02000449 RID: 1097
		private sealed class SteamServerHeartbeatEnabledConVar : BaseConVar
		{
			// Token: 0x1700030C RID: 780
			// (get) Token: 0x06001AC2 RID: 6850 RVA: 0x00071A30 File Offset: 0x0006FC30
			// (set) Token: 0x06001AC3 RID: 6851 RVA: 0x00071A38 File Offset: 0x0006FC38
			public bool value { get; private set; }

			// Token: 0x06001AC4 RID: 6852 RVA: 0x0000972B File Offset: 0x0000792B
			public SteamServerHeartbeatEnabledConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001AC5 RID: 6853 RVA: 0x00071A44 File Offset: 0x0006FC44
			public override void SetString(string newValueString)
			{
				int num;
				if (TextSerialization.TryParseInvariant(newValueString, out num))
				{
					bool flag = num != 0;
					if (flag != this.value)
					{
						this.value = flag;
						if (SteamworksServerManager.instance != null)
						{
							SteamworksServerManager.instance.steamworksServer.AutomaticHeartbeats = this.value;
						}
					}
				}
			}

			// Token: 0x06001AC6 RID: 6854 RVA: 0x00071A8C File Offset: 0x0006FC8C
			public override string GetString()
			{
				if (!this.value)
				{
					return "0";
				}
				return "1";
			}

			// Token: 0x04001846 RID: 6214
			public static readonly SteamworksServerManager.SteamServerHeartbeatEnabledConVar instance = new SteamworksServerManager.SteamServerHeartbeatEnabledConVar("steam_server_heartbeat_enabled", ConVarFlags.Engine, null, "Whether or not this server issues any heartbeats to the Steam master server and effectively advertises it in the master server list. Default is 1 for dedicated servers, 0 for client builds.");
		}

		// Token: 0x0200044A RID: 1098
		public class SteamServerPortConVar : BaseConVar
		{
			// Token: 0x1700030D RID: 781
			// (get) Token: 0x06001AC8 RID: 6856 RVA: 0x00071ABA File Offset: 0x0006FCBA
			// (set) Token: 0x06001AC9 RID: 6857 RVA: 0x00071AC2 File Offset: 0x0006FCC2
			public ushort value { get; private set; }

			// Token: 0x06001ACA RID: 6858 RVA: 0x0000972B File Offset: 0x0000792B
			public SteamServerPortConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001ACB RID: 6859 RVA: 0x00071ACC File Offset: 0x0006FCCC
			public override void SetString(string newValueString)
			{
				if (NetworkServer.active)
				{
					throw new ConCommandException("Cannot change this convar while the server is running.");
				}
				ushort value;
				if (TextSerialization.TryParseInvariant(newValueString, out value))
				{
					this.value = value;
				}
			}

			// Token: 0x06001ACC RID: 6860 RVA: 0x00071AFC File Offset: 0x0006FCFC
			public override string GetString()
			{
				return this.value.ToString();
			}
		}
	}
}
