using System;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Achievements
{
	// Token: 0x020006BD RID: 1725
	[RequireComponent(typeof(NetworkUser))]
	public class ServerAchievementTracker : NetworkBehaviour
	{
		// Token: 0x1700032F RID: 815
		// (get) Token: 0x0600263F RID: 9791 RVA: 0x000B07D7 File Offset: 0x000AE9D7
		// (set) Token: 0x06002640 RID: 9792 RVA: 0x000B07DF File Offset: 0x000AE9DF
		public NetworkUser networkUser { get; private set; }

		// Token: 0x06002641 RID: 9793 RVA: 0x000B07E8 File Offset: 0x000AE9E8
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

		// Token: 0x06002642 RID: 9794 RVA: 0x000B0845 File Offset: 0x000AEA45
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

		// Token: 0x06002643 RID: 9795 RVA: 0x000B0870 File Offset: 0x000AEA70
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

		// Token: 0x06002644 RID: 9796 RVA: 0x000B08B0 File Offset: 0x000AEAB0
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

		// Token: 0x06002645 RID: 9797 RVA: 0x000B0910 File Offset: 0x000AEB10
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

		// Token: 0x06002646 RID: 9798 RVA: 0x000B0968 File Offset: 0x000AEB68
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

		// Token: 0x06002647 RID: 9799 RVA: 0x000B09C8 File Offset: 0x000AEBC8
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

		// Token: 0x06002649 RID: 9801 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x0600264A RID: 9802 RVA: 0x000B09F5 File Offset: 0x000AEBF5
		protected static void InvokeCmdCmdSetAchievementTrackerRequests(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdSetAchievementTrackerRequests called on client.");
				return;
			}
			((ServerAchievementTracker)obj).CmdSetAchievementTrackerRequests(reader.ReadBytesAndSize());
		}

		// Token: 0x0600264B RID: 9803 RVA: 0x000B0A20 File Offset: 0x000AEC20
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

		// Token: 0x0600264C RID: 9804 RVA: 0x000B0AAA File Offset: 0x000AECAA
		protected static void InvokeRpcRpcGrantAchievement(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcGrantAchievement called on server.");
				return;
			}
			((ServerAchievementTracker)obj).RpcGrantAchievement(GeneratedNetworkCode._ReadServerAchievementIndex_None(reader));
		}

		// Token: 0x0600264D RID: 9805 RVA: 0x000B0AD4 File Offset: 0x000AECD4
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

		// Token: 0x0600264E RID: 9806 RVA: 0x000B0B48 File Offset: 0x000AED48
		static ServerAchievementTracker()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(ServerAchievementTracker), ServerAchievementTracker.kCmdCmdSetAchievementTrackerRequests, new NetworkBehaviour.CmdDelegate(ServerAchievementTracker.InvokeCmdCmdSetAchievementTrackerRequests));
			ServerAchievementTracker.kRpcRpcGrantAchievement = -1713740939;
			NetworkBehaviour.RegisterRpcDelegate(typeof(ServerAchievementTracker), ServerAchievementTracker.kRpcRpcGrantAchievement, new NetworkBehaviour.CmdDelegate(ServerAchievementTracker.InvokeRpcRpcGrantAchievement));
			NetworkCRC.RegisterBehaviour("ServerAchievementTracker", 0);
		}

		// Token: 0x0600264F RID: 9807 RVA: 0x000B0BB8 File Offset: 0x000AEDB8
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06002650 RID: 9808 RVA: 0x00004507 File Offset: 0x00002707
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04002882 RID: 10370
		private BaseServerAchievement[] achievementTrackers;

		// Token: 0x04002883 RID: 10371
		private SerializableBitArray maskBitArrayConverter;

		// Token: 0x04002884 RID: 10372
		private byte[] maskBuffer;

		// Token: 0x04002885 RID: 10373
		private static int kCmdCmdSetAchievementTrackerRequests = 387052099;

		// Token: 0x04002886 RID: 10374
		private static int kRpcRpcGrantAchievement;
	}
}
