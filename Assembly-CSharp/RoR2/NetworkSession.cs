using System;
using Facepunch.Steamworks;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200036E RID: 878
	public class NetworkSession : NetworkBehaviour
	{
		// Token: 0x1700018F RID: 399
		// (get) Token: 0x06001215 RID: 4629 RVA: 0x00059315 File Offset: 0x00057515
		// (set) Token: 0x06001216 RID: 4630 RVA: 0x0005931C File Offset: 0x0005751C
		public static NetworkSession instance { get; private set; }

		// Token: 0x06001217 RID: 4631 RVA: 0x00059324 File Offset: 0x00057524
		private void OnSyncSteamId(ulong newValue)
		{
			this.NetworksteamId = newValue;
			this.SteamworksAdvertiseGame();
		}

		// Token: 0x06001218 RID: 4632 RVA: 0x00059334 File Offset: 0x00057534
		private void SteamworksAdvertiseGame()
		{
			if (RoR2Application.instance.steamworksClient != null)
			{
				ulong num = this.steamId;
				uint num2 = 0u;
				ushort num3 = 0;
				NetworkSession.<SteamworksAdvertiseGame>g__CallMethod|6_1(NetworkSession.<SteamworksAdvertiseGame>g__GetField|6_2(NetworkSession.<SteamworksAdvertiseGame>g__GetField|6_2(Client.Instance, "native"), "user"), "AdvertiseGame", new object[]
				{
					num,
					num2,
					num3
				});
			}
		}

		// Token: 0x06001219 RID: 4633 RVA: 0x0005939C File Offset: 0x0005759C
		private void OnEnable()
		{
			NetworkSession.instance = SingletonHelper.Assign<NetworkSession>(NetworkSession.instance, this);
		}

		// Token: 0x0600121A RID: 4634 RVA: 0x000593AE File Offset: 0x000575AE
		private void OnDisable()
		{
			NetworkSession.instance = SingletonHelper.Unassign<NetworkSession>(NetworkSession.instance, this);
		}

		// Token: 0x0600121B RID: 4635 RVA: 0x000593C0 File Offset: 0x000575C0
		private void Start()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			if (NetworkServer.active)
			{
				NetworkServer.Spawn(base.gameObject);
			}
		}

		// Token: 0x0600121C RID: 4636 RVA: 0x000593E0 File Offset: 0x000575E0
		[Server]
		public Run BeginRun(Run runPrefabComponent)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'RoR2.Run RoR2.NetworkSession::BeginRun(RoR2.Run)' called on client");
				return null;
			}
			if (!Run.instance)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(runPrefabComponent.gameObject);
				NetworkServer.Spawn(gameObject);
				return gameObject.GetComponent<Run>();
			}
			return null;
		}

		// Token: 0x0600121D RID: 4637 RVA: 0x00059432 File Offset: 0x00057632
		[Server]
		public void EndRun()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.NetworkSession::EndRun()' called on client");
				return;
			}
			if (Run.instance)
			{
				UnityEngine.Object.Destroy(Run.instance.gameObject);
			}
		}

		// Token: 0x06001222 RID: 4642 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x06001223 RID: 4643 RVA: 0x000594D8 File Offset: 0x000576D8
		// (set) Token: 0x06001224 RID: 4644 RVA: 0x000594EB File Offset: 0x000576EB
		public ulong NetworksteamId
		{
			get
			{
				return this.steamId;
			}
			set
			{
				uint dirtyBit = 1u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncSteamId(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<ulong>(value, ref this.steamId, dirtyBit);
			}
		}

		// Token: 0x06001225 RID: 4645 RVA: 0x0005952C File Offset: 0x0005772C
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.WritePackedUInt64(this.steamId);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt64(this.steamId);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001226 RID: 4646 RVA: 0x00059598 File Offset: 0x00057798
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.steamId = reader.ReadPackedUInt64();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.OnSyncSteamId(reader.ReadPackedUInt64());
			}
		}

		// Token: 0x04001614 RID: 5652
		[SyncVar(hook = "OnSyncSteamId")]
		private ulong steamId;
	}
}
