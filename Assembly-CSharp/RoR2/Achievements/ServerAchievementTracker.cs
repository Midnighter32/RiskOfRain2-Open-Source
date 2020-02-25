using System;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Achievements
{
	// Token: 0x020006CF RID: 1743
	[RequireComponent(typeof(NetworkUser))]
	public class ServerAchievementTracker : NetworkBehaviour
	{
		// Token: 0x170003F8 RID: 1016
		// (get) Token: 0x0600287A RID: 10362 RVA: 0x000AC4DB File Offset: 0x000AA6DB
		// (set) Token: 0x0600287B RID: 10363 RVA: 0x000AC4E3 File Offset: 0x000AA6E3
		public NetworkUser networkUser { get; private set; }

		// Token: 0x0600287C RID: 10364 RVA: 0x000AC4EC File Offset: 0x000AA6EC
		private void Awake()
		{
			this.networkUser = base.GetComponent<NetworkUser>();
			this.maskBitArrayConverter = new SerializableBitArray(AchievementManager.serverAchievementCount);
			if (NetworkServer.active)
			{
				this.achievementTrackers = new BaseServerAchievement[AchievementManager.serverAchievementCount];
			}
			if (NetworkClient.active)
			{
				this.maskBuffer = new byte[this.maskBitArrayConverter.byteCount];
			}
		}

		// Token: 0x0600287D RID: 10365 RVA: 0x000AC549 File Offset: 0x000AA749
		private void Start()
		{
			if (this.networkUser.localUser != null)
			{
				UserAchievementManager userAchievementManager = AchievementManager.GetUserAchievementManager(this.networkUser.localUser);
				if (userAchievementManager == null)
				{
					return;
				}
				userAchievementManager.TransmitAchievementRequestsToServer();
			}
		}

		// Token: 0x0600287E RID: 10366 RVA: 0x000AC574 File Offset: 0x000AA774
		private void OnDestroy()
		{
			if (this.achievementTrackers != null)
			{
				int serverAchievementCount = AchievementManager.serverAchievementCount;
				for (int i = 0; i < serverAchievementCount; i++)
				{
					this.SetAchievementTracked(new ServerAchievementIndex
					{
						intValue = i
					}, false);
				}
			}
		}

		// Token: 0x0600287F RID: 10367 RVA: 0x000AC5B4 File Offset: 0x000AA7B4
		[Client]
		public void SendAchievementTrackerRequestsMaskToServer(bool[] serverAchievementsToTrackMask)
		{
			if (!NetworkClient.active)
			{
				Debug.LogWarning("[Client] function 'System.Void RoR2.Achievements.ServerAchievementTracker::SendAchievementTrackerRequestsMaskToServer(System.Boolean[])' called on server");
				return;
			}
			int serverAchievementCount = AchievementManager.serverAchievementCount;
			for (int i = 0; i < serverAchievementCount; i++)
			{
				this.maskBitArrayConverter[i] = serverAchievementsToTrackMask[i];
			}
			this.maskBitArrayConverter.GetBytes(this.maskBuffer);
			this.CallCmdSetAchievementTrackerRequests(this.maskBuffer);
		}

		// Token: 0x06002880 RID: 10368 RVA: 0x000AC614 File Offset: 0x000AA814
		[Command]
		private void CmdSetAchievementTrackerRequests(byte[] packedServerAchievementsToTrackMask)
		{
			int serverAchievementCount = AchievementManager.serverAchievementCount;
			if (packedServerAchievementsToTrackMask.Length << 3 < serverAchievementCount)
			{
				return;
			}
			for (int i = 0; i < serverAchievementCount; i++)
			{
				int num = i >> 3;
				int num2 = i & 7;
				this.SetAchievementTracked(new ServerAchievementIndex
				{
					intValue = i
				}, (packedServerAchievementsToTrackMask[num] >> num2 & 1) != 0);
			}
		}

		// Token: 0x06002881 RID: 10369 RVA: 0x000AC66C File Offset: 0x000AA86C
		private void SetAchievementTracked(ServerAchievementIndex serverAchievementIndex, bool shouldTrack)
		{
			BaseServerAchievement baseServerAchievement = this.achievementTrackers[serverAchievementIndex.intValue];
			if (shouldTrack == (baseServerAchievement != null))
			{
				return;
			}
			if (shouldTrack)
			{
				BaseServerAchievement baseServerAchievement2 = BaseServerAchievement.Instantiate(serverAchievementIndex);
				baseServerAchievement2.serverAchievementTracker = this;
				this.achievementTrackers[serverAchievementIndex.intValue] = baseServerAchievement2;
				baseServerAchievement2.OnInstall();
				return;
			}
			baseServerAchievement.OnUninstall();
			this.achievementTrackers[serverAchievementIndex.intValue] = null;
		}

		// Token: 0x06002882 RID: 10370 RVA: 0x000AC6CC File Offset: 0x000AA8CC
		[ClientRpc]
		public void RpcGrantAchievement(ServerAchievementIndex serverAchievementIndex)
		{
			LocalUser localUser = this.networkUser.localUser;
			if (localUser != null)
			{
				UserAchievementManager userAchievementManager = AchievementManager.GetUserAchievementManager(localUser);
				if (userAchievementManager == null)
				{
					return;
				}
				userAchievementManager.HandleServerAchievementCompleted(serverAchievementIndex);
			}
		}

		// Token: 0x06002884 RID: 10372 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x06002885 RID: 10373 RVA: 0x000AC6F9 File Offset: 0x000AA8F9
		protected static void InvokeCmdCmdSetAchievementTrackerRequests(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdSetAchievementTrackerRequests called on client.");
				return;
			}
			((ServerAchievementTracker)obj).CmdSetAchievementTrackerRequests(reader.ReadBytesAndSize());
		}

		// Token: 0x06002886 RID: 10374 RVA: 0x000AC724 File Offset: 0x000AA924
		public void CallCmdSetAchievementTrackerRequests(byte[] packedServerAchievementsToTrackMask)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdSetAchievementTrackerRequests called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdSetAchievementTrackerRequests(packedServerAchievementsToTrackMask);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)ServerAchievementTracker.kCmdCmdSetAchievementTrackerRequests);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.WriteBytesFull(packedServerAchievementsToTrackMask);
			base.SendCommandInternal(networkWriter, 0, "CmdSetAchievementTrackerRequests");
		}

		// Token: 0x06002887 RID: 10375 RVA: 0x000AC7AE File Offset: 0x000AA9AE
		protected static void InvokeRpcRpcGrantAchievement(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcGrantAchievement called on server.");
				return;
			}
			((ServerAchievementTracker)obj).RpcGrantAchievement(GeneratedNetworkCode._ReadServerAchievementIndex_None(reader));
		}

		// Token: 0x06002888 RID: 10376 RVA: 0x000AC7D8 File Offset: 0x000AA9D8
		public void CallRpcGrantAchievement(ServerAchievementIndex serverAchievementIndex)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcGrantAchievement called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)ServerAchievementTracker.kRpcRpcGrantAchievement);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			GeneratedNetworkCode._WriteServerAchievementIndex_None(networkWriter, serverAchievementIndex);
			this.SendRPCInternal(networkWriter, 0, "RpcGrantAchievement");
		}

		// Token: 0x06002889 RID: 10377 RVA: 0x000AC84C File Offset: 0x000AAA4C
		static ServerAchievementTracker()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(ServerAchievementTracker), ServerAchievementTracker.kCmdCmdSetAchievementTrackerRequests, new NetworkBehaviour.CmdDelegate(ServerAchievementTracker.InvokeCmdCmdSetAchievementTrackerRequests));
			ServerAchievementTracker.kRpcRpcGrantAchievement = -1713740939;
			NetworkBehaviour.RegisterRpcDelegate(typeof(ServerAchievementTracker), ServerAchievementTracker.kRpcRpcGrantAchievement, new NetworkBehaviour.CmdDelegate(ServerAchievementTracker.InvokeRpcRpcGrantAchievement));
			NetworkCRC.RegisterBehaviour("ServerAchievementTracker", 0);
		}

		// Token: 0x0600288A RID: 10378 RVA: 0x000AC8BC File Offset: 0x000AAABC
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x0600288B RID: 10379 RVA: 0x0000409B File Offset: 0x0000229B
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x0400251A RID: 9498
		private BaseServerAchievement[] achievementTrackers;

		// Token: 0x0400251B RID: 9499
		private SerializableBitArray maskBitArrayConverter;

		// Token: 0x0400251C RID: 9500
		private byte[] maskBuffer;

		// Token: 0x0400251D RID: 9501
		private static int kCmdCmdSetAchievementTrackerRequests = 387052099;

		// Token: 0x0400251E RID: 9502
		private static int kRpcRpcGrantAchievement;
	}
}
