using System;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000592 RID: 1426
	public class SteamNetworkClient : NetworkClient
	{
		// Token: 0x170002D1 RID: 721
		// (get) Token: 0x06002024 RID: 8228 RVA: 0x00096EE8 File Offset: 0x000950E8
		public SteamNetworkConnection steamConnection
		{
			get
			{
				return (SteamNetworkConnection)base.connection;
			}
		}

		// Token: 0x170002D2 RID: 722
		// (get) Token: 0x06002025 RID: 8229 RVA: 0x00096EF5 File Offset: 0x000950F5
		public string status
		{
			get
			{
				return this.m_AsyncConnect.ToString();
			}
		}

		// Token: 0x06002026 RID: 8230 RVA: 0x00096F08 File Offset: 0x00095108
		public void Connect()
		{
			base.Connect("localhost", 0);
			this.m_AsyncConnect = NetworkClient.ConnectState.Connected;
			base.connection.ForceInitialize(base.hostTopology);
		}

		// Token: 0x06002027 RID: 8231 RVA: 0x00096F2E File Offset: 0x0009512E
		public SteamNetworkClient(NetworkConnection conn) : base(conn)
		{
			base.SetNetworkConnectionClass<SteamNetworkConnection>();
		}
	}
}
