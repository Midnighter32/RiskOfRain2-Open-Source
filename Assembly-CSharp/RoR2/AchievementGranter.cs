using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000139 RID: 313
	public class AchievementGranter : NetworkBehaviour
	{
		// Token: 0x0600059E RID: 1438 RVA: 0x00017410 File Offset: 0x00015610
		[ClientRpc]
		public void RpcGrantAchievement(string achievementName)
		{
			foreach (LocalUser user in LocalUserManager.readOnlyLocalUsersList)
			{
				AchievementManager.GetUserAchievementManager(user).GrantAchievement(AchievementManager.GetAchievementDef(achievementName));
			}
		}

		// Token: 0x060005A0 RID: 1440 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x060005A1 RID: 1441 RVA: 0x0001746C File Offset: 0x0001566C
		protected static void InvokeRpcRpcGrantAchievement(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcGrantAchievement called on server.");
				return;
			}
			((AchievementGranter)obj).RpcGrantAchievement(reader.ReadString());
		}

		// Token: 0x060005A2 RID: 1442 RVA: 0x00017498 File Offset: 0x00015698
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

		// Token: 0x060005A3 RID: 1443 RVA: 0x0001750B File Offset: 0x0001570B
		static AchievementGranter()
		{
			NetworkBehaviour.RegisterRpcDelegate(typeof(AchievementGranter), AchievementGranter.kRpcRpcGrantAchievement, new NetworkBehaviour.CmdDelegate(AchievementGranter.InvokeRpcRpcGrantAchievement));
			NetworkCRC.RegisterBehaviour("AchievementGranter", 0);
		}

		// Token: 0x060005A4 RID: 1444 RVA: 0x00017548 File Offset: 0x00015748
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x060005A5 RID: 1445 RVA: 0x0000409B File Offset: 0x0000229B
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04000606 RID: 1542
		private static int kRpcRpcGrantAchievement = -180752285;
	}
}
