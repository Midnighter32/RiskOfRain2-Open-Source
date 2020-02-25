using System;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Facepunch.Steamworks;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200029F RID: 671
	public class NetworkSession : NetworkBehaviour
	{
		// Token: 0x170001DE RID: 478
		// (get) Token: 0x06000EF6 RID: 3830 RVA: 0x000423AA File Offset: 0x000405AA
		// (set) Token: 0x06000EF7 RID: 3831 RVA: 0x000423B1 File Offset: 0x000405B1
		public static NetworkSession instance { get; private set; }

		// Token: 0x06000EF8 RID: 3832 RVA: 0x000423B9 File Offset: 0x000405B9
		private void OnSyncSteamId(ulong newValue)
		{
			this.NetworksteamId = newValue;
			this.SteamworksAdvertiseGame();
		}

		// Token: 0x06000EF9 RID: 3833 RVA: 0x000423C8 File Offset: 0x000405C8
		private void SteamworksAdvertiseGame()
		{
			if (RoR2Application.instance.steamworksClient != null)
			{
				ulong num = this.steamId;
				uint num2 = 0U;
				ushort num3 = 0;
				NetworkSession.<SteamworksAdvertiseGame>g__CallMethod|6_1(NetworkSession.<SteamworksAdvertiseGame>g__GetField|6_2(NetworkSession.<SteamworksAdvertiseGame>g__GetField|6_2(Client.Instance, "native"), "user"), "AdvertiseGame", new object[]
				{
					num,
					num2,
					num3
				});
			}
		}

		// Token: 0x06000EFA RID: 3834 RVA: 0x00042430 File Offset: 0x00040630
		private void OnEnable()
		{
			NetworkSession.instance = SingletonHelper.Assign<NetworkSession>(NetworkSession.instance, this);
		}

		// Token: 0x06000EFB RID: 3835 RVA: 0x00042442 File Offset: 0x00040642
		private void OnDisable()
		{
			NetworkSession.instance = SingletonHelper.Unassign<NetworkSession>(NetworkSession.instance, this);
		}

		// Token: 0x06000EFC RID: 3836 RVA: 0x00042454 File Offset: 0x00040654
		private void Start()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			if (NetworkServer.active)
			{
				NetworkServer.Spawn(base.gameObject);
			}
		}

		// Token: 0x06000EFD RID: 3837 RVA: 0x00042474 File Offset: 0x00040674
		[Server]
		public Run BeginRun(Run runPrefabComponent, RuleBook ruleBook, ulong seed)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'RoR2.Run RoR2.NetworkSession::BeginRun(RoR2.Run,RoR2.RuleBook,System.UInt64)' called on client");
				return null;
			}
			if (!Run.instance)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(runPrefabComponent.gameObject);
				Run component = gameObject.GetComponent<Run>();
				component.SetRuleBook(ruleBook);
				component.seed = seed;
				NetworkServer.Spawn(gameObject);
				return component;
			}
			return null;
		}

		// Token: 0x06000EFE RID: 3838 RVA: 0x000424D6 File Offset: 0x000406D6
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

		// Token: 0x06000F00 RID: 3840 RVA: 0x00042508 File Offset: 0x00040708
		[CompilerGenerated]
		internal static uint <SteamworksAdvertiseGame>g__GetServerAddress|6_0()
		{
			byte[] addressBytes = IPAddress.Parse(NetworkClient.allClients[0].connection.address).GetAddressBytes();
			if (addressBytes.Length != 4)
			{
				return 0U;
			}
			return (uint)IPAddress.NetworkToHostOrder((long)((ulong)BitConverter.ToUInt32(addressBytes, 0)));
		}

		// Token: 0x06000F01 RID: 3841 RVA: 0x0004254B File Offset: 0x0004074B
		[CompilerGenerated]
		internal static void <SteamworksAdvertiseGame>g__CallMethod|6_1(object obj, string methodName, object[] args)
		{
			obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(obj, args);
		}

		// Token: 0x06000F02 RID: 3842 RVA: 0x00042563 File Offset: 0x00040763
		[CompilerGenerated]
		internal static object <SteamworksAdvertiseGame>g__GetField|6_2(object obj, string fieldName)
		{
			return obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj);
		}

		// Token: 0x06000F03 RID: 3843 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x170001DF RID: 479
		// (get) Token: 0x06000F04 RID: 3844 RVA: 0x0004257C File Offset: 0x0004077C
		// (set) Token: 0x06000F05 RID: 3845 RVA: 0x0004258F File Offset: 0x0004078F
		public ulong NetworksteamId
		{
			get
			{
				return this.steamId;
			}
			[param: In]
			set
			{
				uint dirtyBit = 1U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncSteamId(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<ulong>(value, ref this.steamId, dirtyBit);
			}
		}

		// Token: 0x06000F06 RID: 3846 RVA: 0x000425D0 File Offset: 0x000407D0
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.WritePackedUInt64(this.steamId);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1U) != 0U)
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

		// Token: 0x06000F07 RID: 3847 RVA: 0x0004263C File Offset: 0x0004083C
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

		// Token: 0x04000EC2 RID: 3778
		[SyncVar(hook = "OnSyncSteamId")]
		private ulong steamId;
	}
}
