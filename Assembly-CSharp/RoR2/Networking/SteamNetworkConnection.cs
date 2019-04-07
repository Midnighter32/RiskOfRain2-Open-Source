using System;
using Facepunch.Steamworks;
using RoR2.ConVar;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000593 RID: 1427
	public class SteamNetworkConnection : NetworkConnection
	{
		// Token: 0x06002028 RID: 8232 RVA: 0x00096F3D File Offset: 0x0009513D
		public SteamNetworkConnection()
		{
		}

		// Token: 0x06002029 RID: 8233 RVA: 0x00096F45 File Offset: 0x00095145
		public SteamNetworkConnection(CSteamID steamId)
		{
			this.steamId = steamId;
		}

		// Token: 0x0600202A RID: 8234 RVA: 0x00096F54 File Offset: 0x00095154
		public override bool TransportSend(byte[] bytes, int numBytes, int channelId, out byte error)
		{
			if (this.ignore)
			{
				error = 0;
				return true;
			}
			this.logNetworkMessages = SteamNetworkConnection.cvNetP2PLogMessages.value;
			Client instance = Client.Instance;
			if (this.steamId.value == instance.SteamId)
			{
				this.TransportReceive(bytes, numBytes, channelId);
				error = 0;
				if (SteamNetworkConnection.cvNetP2PDebugTransport.value)
				{
					Debug.LogFormat("SteamNetworkConnection.TransportSend steamId=self numBytes={1} channelId={2}", new object[]
					{
						numBytes,
						channelId
					});
				}
				return true;
			}
			Networking.SendType eP2PSendType = Networking.SendType.Reliable;
			QosType qos = GameNetworkManager.singleton.connectionConfig.Channels[channelId].QOS;
			if (qos == QosType.Unreliable || qos == QosType.UnreliableFragmented || qos == QosType.UnreliableSequenced)
			{
				eP2PSendType = Networking.SendType.Unreliable;
			}
			if (instance.Networking.SendP2PPacket(this.steamId.value, bytes, numBytes, eP2PSendType, 0))
			{
				error = 0;
				if (SteamNetworkConnection.cvNetP2PDebugTransport.value)
				{
					Debug.LogFormat("SteamNetworkConnection.TransportSend steamId={0} numBytes={1} channelId={2} error={3}", new object[]
					{
						this.steamId.value,
						numBytes,
						channelId,
						error
					});
				}
				return true;
			}
			error = 1;
			if (SteamNetworkConnection.cvNetP2PDebugTransport.value)
			{
				Debug.LogFormat("SteamNetworkConnection.TransportSend steamId={0} numBytes={1} channelId={2} error={3}", new object[]
				{
					this.steamId.value,
					numBytes,
					channelId,
					error
				});
			}
			return false;
		}

		// Token: 0x0600202B RID: 8235 RVA: 0x000970BE File Offset: 0x000952BE
		public override void TransportReceive(byte[] bytes, int numBytes, int channelId)
		{
			if (this.ignore)
			{
				return;
			}
			this.logNetworkMessages = SteamNetworkConnection.cvNetP2PLogMessages.value;
			base.TransportReceive(bytes, numBytes, channelId);
		}

		// Token: 0x0600202C RID: 8236 RVA: 0x000970E4 File Offset: 0x000952E4
		protected override void Dispose(bool disposing)
		{
			if (Client.Instance != null && this.steamId.value != 0UL)
			{
				Client.Instance.Networking.CloseSession(this.steamId.value);
				this.steamId = CSteamID.nil;
			}
			base.Dispose(disposing);
		}

		// Token: 0x04002246 RID: 8774
		public CSteamID steamId;

		// Token: 0x04002247 RID: 8775
		public bool ignore;

		// Token: 0x04002248 RID: 8776
		public uint rtt;

		// Token: 0x04002249 RID: 8777
		public static BoolConVar cvNetP2PDebugTransport = new BoolConVar("net_p2p_debug_transport", ConVarFlags.None, "0", "Allows p2p transport information to print to the console.");

		// Token: 0x0400224A RID: 8778
		private static BoolConVar cvNetP2PLogMessages = new BoolConVar("net_p2p_log_messages", ConVarFlags.None, "0", "Enables logging of network messages.");
	}
}
