using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000559 RID: 1369
	public static class ServerAuthManager
	{
		// Token: 0x060020AF RID: 8367 RVA: 0x0000409B File Offset: 0x0000229B
		private static void OnConnectionDiscovered(NetworkConnection conn)
		{
		}

		// Token: 0x060020B0 RID: 8368 RVA: 0x0008D700 File Offset: 0x0008B900
		private static void OnConnectionLost(NetworkConnection conn)
		{
			for (int i = 0; i < ServerAuthManager.instanceCount; i++)
			{
				if (ServerAuthManager.instances[i].conn == conn)
				{
					HGArrayUtilities.ArrayRemoveAt<ServerAuthManager.KeyValue>(ref ServerAuthManager.instances, ref ServerAuthManager.instanceCount, i, 1);
					return;
				}
			}
		}

		// Token: 0x060020B1 RID: 8369 RVA: 0x0008D744 File Offset: 0x0008B944
		public static ClientAuthData FindAuthData(NetworkConnection conn)
		{
			for (int i = 0; i < ServerAuthManager.instanceCount; i++)
			{
				if (ServerAuthManager.instances[i].conn == conn)
				{
					return ServerAuthManager.instances[i].authData;
				}
			}
			return null;
		}

		// Token: 0x060020B2 RID: 8370 RVA: 0x0008D788 File Offset: 0x0008B988
		public static NetworkConnection FindConnectionForSteamID(CSteamID steamId)
		{
			for (int i = 0; i < ServerAuthManager.instanceCount; i++)
			{
				if (ServerAuthManager.instances[i].authData.steamId == steamId)
				{
					return ServerAuthManager.instances[i].conn;
				}
			}
			return null;
		}

		// Token: 0x060020B3 RID: 8371 RVA: 0x0008D7D4 File Offset: 0x0008B9D4
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			GameNetworkManager.onServerConnectGlobal += ServerAuthManager.OnConnectionDiscovered;
			GameNetworkManager.onServerDisconnectGlobal += ServerAuthManager.OnConnectionLost;
		}

		// Token: 0x060020B4 RID: 8372 RVA: 0x0008D7F8 File Offset: 0x0008B9F8
		[NetworkMessageHandler(client = false, server = true, msgType = 74)]
		private static void HandleSetClientAuth(NetworkMessage netMsg)
		{
			if (ServerAuthManager.FindAuthData(netMsg.conn) != null)
			{
				return;
			}
			GameNetworkManager.KickReason? kickReason = null;
			try
			{
				ClientAuthData clientAuthData = netMsg.ReadMessage<ClientAuthData>();
				NetworkConnection networkConnection = ServerAuthManager.FindConnectionForSteamID(clientAuthData.steamId);
				if (networkConnection != null)
				{
					Debug.LogFormat("SteamID {0} is already claimed by connection [{1}]. Connection [{2}] rejected.", new object[]
					{
						clientAuthData.steamId,
						networkConnection,
						netMsg.conn
					});
					GameNetworkManager.singleton.ServerKickClient(netMsg.conn, GameNetworkManager.KickReason.Unspecified);
					return;
				}
				ServerAuthManager.KeyValue keyValue = new ServerAuthManager.KeyValue(netMsg.conn, clientAuthData);
				HGArrayUtilities.ArrayAppend<ServerAuthManager.KeyValue>(ref ServerAuthManager.instances, ref ServerAuthManager.instanceCount, ref keyValue);
				string value = GameNetworkManager.SvPasswordConVar.instance.value;
				if (value.Length != 0 && !(clientAuthData.password == value))
				{
					Debug.LogFormat("Rejecting connection from [{0}]: {1}", new object[]
					{
						netMsg.conn,
						"Bad password."
					});
					kickReason = new GameNetworkManager.KickReason?(GameNetworkManager.KickReason.BadPassword);
				}
				if (!true)
				{
					Debug.LogFormat("Rejecting connection from [{0}]: {1}", new object[]
					{
						netMsg.conn,
						"Bad version."
					});
					kickReason = new GameNetworkManager.KickReason?(GameNetworkManager.KickReason.BadVersion);
				}
				Action<NetworkConnection, ClientAuthData> action = ServerAuthManager.onAuthDataReceivedFromClient;
				if (action != null)
				{
					action(keyValue.conn, keyValue.authData);
				}
			}
			catch
			{
				Debug.LogFormat("Rejecting connection from [{0}]: {1}", new object[]
				{
					netMsg.conn,
					"Malformed auth data."
				});
				kickReason = new GameNetworkManager.KickReason?(GameNetworkManager.KickReason.MalformedAuthData);
			}
			if (kickReason != null)
			{
				GameNetworkManager.singleton.ServerKickClient(netMsg.conn, kickReason.Value);
			}
		}

		// Token: 0x1400007E RID: 126
		// (add) Token: 0x060020B5 RID: 8373 RVA: 0x0008D994 File Offset: 0x0008BB94
		// (remove) Token: 0x060020B6 RID: 8374 RVA: 0x0008D9C8 File Offset: 0x0008BBC8
		public static event Action<NetworkConnection, ClientAuthData> onAuthDataReceivedFromClient;

		// Token: 0x1400007F RID: 127
		// (add) Token: 0x060020B7 RID: 8375 RVA: 0x0008D9FC File Offset: 0x0008BBFC
		// (remove) Token: 0x060020B8 RID: 8376 RVA: 0x0008DA30 File Offset: 0x0008BC30
		public static event Action<NetworkConnection, ClientAuthData> onAuthExpired;

		// Token: 0x04001DE5 RID: 7653
		private static readonly int initialSize = 16;

		// Token: 0x04001DE6 RID: 7654
		private static ServerAuthManager.KeyValue[] instances = new ServerAuthManager.KeyValue[ServerAuthManager.initialSize];

		// Token: 0x04001DE7 RID: 7655
		private static int instanceCount = 0;

		// Token: 0x0200055A RID: 1370
		private struct KeyValue
		{
			// Token: 0x060020BA RID: 8378 RVA: 0x0008DA81 File Offset: 0x0008BC81
			public KeyValue(NetworkConnection conn, ClientAuthData authData)
			{
				this.conn = conn;
				this.authData = authData;
			}

			// Token: 0x04001DEA RID: 7658
			public readonly NetworkConnection conn;

			// Token: 0x04001DEB RID: 7659
			public readonly ClientAuthData authData;
		}
	}
}
