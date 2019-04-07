using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000248 RID: 584
	public class AchievementGranter : NetworkBehaviour
	{
		// Token: 0x06000AF6 RID: 2806 RVA: 0x000368E4 File Offset: 0x00034AE4
		[ClientRpc]
		public void RpcGrantAchievement(string achievementName)
		{
			foreach (LocalUser user in LocalUserManager.readOnlyLocalUsersList)
			{
				AchievementManager.GetUserAchievementManager(user).GrantAchievement(AchievementManager.GetAchievementDef(achievementName));
			}
		}

		// Token: 0x06000AF8 RID: 2808 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x06000AF9 RID: 2809 RVA: 0x00036940 File Offset: 0x00034B40
		protected static void InvokeRpcRpcGrantAchievement(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcGrantAchievement called on server.");
				return;
			}
			((AchievementGranter)obj).RpcGrantAchievement(reader.ReadString());
		}

		// Token: 0x06000AFA RID: 2810 RVA: 0x0003696C File Offset: 0x00034B6C
		public void CallRpcGrantAchievement(string achievementName)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcGrantAchievement called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)AchievementGranter.kRpcRpcGrantAchievement);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(achievementName);
			this.SendRPCInternal(networkWriter, 0, "RpcGrantAchievement");
		}

		// Token: 0x06000AFB RID: 2811 RVA: 0x000369DF File Offset: 0x00034BDF
		static AchievementGranter()
		{
			NetworkBehaviour.RegisterRpcDelegate(typeof(AchievementGranter), AchievementGranter.kRpcRpcGrantAchievement, new NetworkBehaviour.CmdDelegate(AchievementGranter.InvokeRpcRpcGrantAchievement));
			NetworkCRC.RegisterBehaviour("AchievementGranter", 0);
		}

		// Token: 0x06000AFC RID: 2812 RVA: 0x00036A1C File Offset: 0x00034C1C
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06000AFD RID: 2813 RVA: 0x00004507 File Offset: 0x00002707
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04000EE9 RID: 3817
		private static int kRpcRpcGrantAchievement = -180752285;
	}
}
