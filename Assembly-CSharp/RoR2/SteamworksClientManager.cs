using System;
using System.IO;
using Facepunch.Steamworks;
using RoR2.Networking;
using SteamAPIValidator;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200042E RID: 1070
	public sealed class SteamworksClientManager : IDisposable
	{
		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x0600199D RID: 6557 RVA: 0x0006DE67 File Offset: 0x0006C067
		// (set) Token: 0x0600199E RID: 6558 RVA: 0x0006DE6E File Offset: 0x0006C06E
		public static SteamworksClientManager instance { get; private set; }

		// Token: 0x0600199F RID: 6559 RVA: 0x0006DE78 File Offset: 0x0006C078
		private SteamworksClientManager()
		{
			if (!Application.isEditor && File.Exists("steam_appid.txt"))
			{
				try
				{
					File.Delete("steam_appid.txt");
				}
				catch (Exception ex)
				{
					Debug.Log(ex.Message);
				}
				if (File.Exists("steam_appid.txt"))
				{
					Debug.Log("Cannot delete steam_appid.txt. Quitting...");
					this.Dispose();
					return;
				}
			}
			Config.ForUnity(Application.platform.ToString());
			this.steamworksClient = new Client(632360U);
			if (!this.steamworksClient.IsValid)
			{
				this.Dispose();
				return;
			}
			if (!Application.isEditor)
			{
				if (Client.RestartIfNecessary(632360U) || !this.steamworksClient.IsValid || !SteamApiValidator.IsValidSteamApiDll())
				{
					Debug.Log("Unable to initialize Facepunch.Steamworks.");
					this.Dispose();
					return;
				}
				if (!this.steamworksClient.App.IsSubscribed(632360U))
				{
					Debug.Log("Steam user not subscribed to app. Quitting...");
					this.Dispose();
					return;
				}
			}
			RoR2Application.steamBuildId = TextSerialization.ToStringInvariant(this.steamworksClient.BuildId);
			RoR2Application.onUpdate += this.Update;
			RoR2Application.cloudStorage = new SteamworksRemoteStorageFileSystem();
			SteamworksLobbyManager.Init();
			SteamLobbyFinder.Init();
		}

		// Token: 0x060019A0 RID: 6560 RVA: 0x0006DFBC File Offset: 0x0006C1BC
		private void Update()
		{
			this.steamworksClient.Update();
		}

		// Token: 0x060019A1 RID: 6561 RVA: 0x0006DFCC File Offset: 0x0006C1CC
		public void Dispose()
		{
			if (this.disposed)
			{
				return;
			}
			this.disposed = true;
			RoR2Application.onUpdate -= this.Update;
			if (this.steamworksClient != null)
			{
				if (GameNetworkManager.singleton)
				{
					GameNetworkManager.singleton.ForceCloseAllSteamConnections();
				}
				Debug.Log("Shutting down Steamworks...");
				this.steamworksClient.Lobby.Leave();
				if (Server.Instance != null)
				{
					Server.Instance.Dispose();
				}
				this.steamworksClient.Update();
				this.steamworksClient.Dispose();
				this.steamworksClient = null;
				Debug.Log("Shut down Steamworks.");
			}
		}

		// Token: 0x060019A2 RID: 6562 RVA: 0x0006E06C File Offset: 0x0006C26C
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			RoR2Application.loadSteamworksClient = delegate()
			{
				SteamworksClientManager.instance = new SteamworksClientManager();
				if (!SteamworksClientManager.instance.disposed)
				{
					Action action = SteamworksClientManager.onLoaded;
					if (action != null)
					{
						action();
					}
					return true;
				}
				return false;
			};
			RoR2Application.unloadSteamworksClient = delegate()
			{
				SteamworksClientManager instance = SteamworksClientManager.instance;
				if (instance != null)
				{
					instance.Dispose();
				}
				SteamworksClientManager.instance = null;
			};
		}

		// Token: 0x1400005A RID: 90
		// (add) Token: 0x060019A3 RID: 6563 RVA: 0x0006E0C4 File Offset: 0x0006C2C4
		// (remove) Token: 0x060019A4 RID: 6564 RVA: 0x0006E0F8 File Offset: 0x0006C2F8
		public static event Action onLoaded;

		// Token: 0x040017CB RID: 6091
		private Client steamworksClient;

		// Token: 0x040017CC RID: 6092
		private bool disposed;
	}
}
