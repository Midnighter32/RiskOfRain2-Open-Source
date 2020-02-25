using System;
using Facepunch.Steamworks;
using RoR2.ConVar;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000564 RID: 1380
	public class SteamNetworkConnection : NetworkConnection
	{
		// Token: 0x060020F2 RID: 8434 RVA: 0x0008E501 File Offset: 0x0008C701
		public SteamNetworkConnection()
		{
		}

		// Token: 0x060020F3 RID: 8435 RVA: 0x0008E509 File Offset: 0x0008C709
		public SteamNetworkConnection(CSteamID steamId)
		{
			this.steamId = steamId;
			Client.Instance.Networking.CloseSession(steamId.value);
		}

		// Token: 0x060020F4 RID: 8436 RVA: 0x0008E530 File Offset: 0x0008C730
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

		// Token: 0x060020F5 RID: 8437 RVA: 0x0008E69A File Offset: 0x0008C89A
		public override void TransportReceive(byte[] bytes, int numBytes, int channelId)
		{
			if (this.ignore)
			{
				return;
			}
			this.logNetworkMessages = SteamNetworkConnection.cvNetP2PLogMessages.value;
			base.TransportReceive(bytes, numBytes, channelId);
		}

		// Token: 0x060020F6 RID: 8438 RVA: 0x0008E6C0 File Offset: 0x0008C8C0
		protected override void Dispose(bool disposing)
		{
			if (Client.Instance != null && this.steamId.value != 0UL)
			{
				Client.Instance.Networking.CloseSession(this.steamId.value);
				this.steamId = CSteamID.nil;
			}
			base.Dispose(disposing);
		}

		// Token: 0x04001DFD RID: 7677
		public CSteamID steamId;

		// Token: 0x04001DFE RID: 7678
		public bool ignore;

		// Token: 0x04001DFF RID: 7679
		public uint rtt;

		// Token: 0x04001E00 RID: 7680
		public static BoolConVar cvNetP2PDebugTransport = new BoolConVar("net_p2p_debug_transport", ConVarFlags.None, "0", "Allows p2p transport information to print to the console.");

		// Token: 0x04001E01 RID: 7681
		private static BoolConVar cvNetP2PLogMessages = new BoolConVar("net_p2p_log_messages", ConVarFlags.None, "0", "Enables logging of network messages.");
	}
}
