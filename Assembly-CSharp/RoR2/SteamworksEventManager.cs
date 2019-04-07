using System;
using Facepunch.Steamworks;

namespace RoR2
{
	// Token: 0x0200049E RID: 1182
	public static class SteamworksEventManager
	{
		// Token: 0x06001A45 RID: 6725 RVA: 0x0007C771 File Offset: 0x0007A971
		public static void Init(Client client)
		{
			SteamworksLobbyManager.SetupCallbacks(client);
		}
	}
}
