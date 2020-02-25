using System;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000563 RID: 1379
	public class SteamNetworkClient : NetworkClient
	{
		// Token: 0x17000374 RID: 884
		// (get) Token: 0x060020EE RID: 8430 RVA: 0x0008E4AC File Offset: 0x0008C6AC
		public SteamNetworkConnection steamConnection
		{
			get
			{
				return (SteamNetworkConnection)base.connection;
			}
		}

		// Token: 0x17000375 RID: 885
		// (get) Token: 0x060020EF RID: 8431 RVA: 0x0008E4B9 File Offset: 0x0008C6B9
		public string status
		{
			get
			{
				return this.m_AsyncConnect.ToString();
			}
		}

		// Token: 0x060020F0 RID: 8432 RVA: 0x0008E4CC File Offset: 0x0008C6CC
		public void Connect()
		{
			base.Connect("localhost", 0);
			this.m_AsyncConnect = NetworkClient.ConnectState.Connected;
			base.connection.ForceInitialize(base.hostTopology);
		}

		// Token: 0x060020F1 RID: 8433 RVA: 0x0008E4F2 File Offset: 0x0008C6F2
		public SteamNetworkClient(NetworkConnection conn) : base(conn)
		{
			base.SetNetworkConnectionClass<SteamNetworkConnection>();
		}
	}
}
