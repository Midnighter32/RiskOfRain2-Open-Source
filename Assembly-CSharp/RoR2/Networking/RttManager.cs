using System;
using RoR2.ConVar;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000540 RID: 1344
	public static class RttManager
	{
		// Token: 0x06001FB4 RID: 8116 RVA: 0x00089B44 File Offset: 0x00087D44
		public static float GetConnectionRTT(NetworkConnection connection)
		{
			int num;
			if (RttManager.FindConnectionIndex(connection, out num))
			{
				return RttManager.entries[num].newestRttInSeconds;
			}
			return 0f;
		}

		// Token: 0x06001FB5 RID: 8117 RVA: 0x00089B74 File Offset: 0x00087D74
		public static uint GetConnectionRTTInMilliseconds(NetworkConnection connection)
		{
			int num;
			if (RttManager.FindConnectionIndex(connection, out num))
			{
				return RttManager.entries[num].newestRttInMilliseconds;
			}
			return 0U;
		}

		// Token: 0x06001FB6 RID: 8118 RVA: 0x00089BA0 File Offset: 0x00087DA0
		public static float GetConnectionFrameSmoothedRtt(NetworkConnection connection)
		{
			int num;
			if (RttManager.FindConnectionIndex(connection, out num))
			{
				return RttManager.entries[num].frameSmoothedRtt;
			}
			return 0f;
		}

		// Token: 0x06001FB7 RID: 8119 RVA: 0x00089BD0 File Offset: 0x00087DD0
		public static float GetConnectionFixedSmoothedRtt(NetworkConnection connection)
		{
			int num;
			if (RttManager.FindConnectionIndex(connection, out num))
			{
				return RttManager.entries[num].fixedSmoothedRtt;
			}
			return 0f;
		}

		// Token: 0x06001FB8 RID: 8120 RVA: 0x00089C00 File Offset: 0x00087E00
		private static void OnConnectionDiscovered(NetworkConnection connection)
		{
			RttManager.ConnectionRttInfo connectionRttInfo = new RttManager.ConnectionRttInfo(connection);
			HGArrayUtilities.ArrayAppend<RttManager.ConnectionRttInfo>(ref RttManager.entries, ref connectionRttInfo);
		}

		// Token: 0x06001FB9 RID: 8121 RVA: 0x00089C24 File Offset: 0x00087E24
		private static void OnConnectionLost(NetworkConnection connection)
		{
			int position;
			if (RttManager.FindConnectionIndex(connection, out position))
			{
				HGArrayUtilities.ArrayRemoveAtAndResize<RttManager.ConnectionRttInfo>(ref RttManager.entries, position, 1);
			}
		}

		// Token: 0x06001FBA RID: 8122 RVA: 0x00089C48 File Offset: 0x00087E48
		private static bool FindConnectionIndex(NetworkConnection connection, out int entryIndex)
		{
			RttManager.ConnectionRttInfo[] array = RttManager.entries;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].connection == connection)
				{
					entryIndex = i;
					return true;
				}
			}
			entryIndex = -1;
			return false;
		}

		// Token: 0x06001FBB RID: 8123 RVA: 0x00089C81 File Offset: 0x00087E81
		private static void UpdateFilteredRtt(float deltaTime, float targetValue, ref float currentValue, ref float velocity)
		{
			if (currentValue == 0f)
			{
				currentValue = targetValue;
				velocity = 0f;
				return;
			}
			currentValue = Mathf.SmoothDamp(currentValue, targetValue, ref velocity, RttManager.cvRttSmoothDuration.value, 100f, deltaTime);
		}

		// Token: 0x06001FBC RID: 8124 RVA: 0x00089CB4 File Offset: 0x00087EB4
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			GameNetworkManager.onClientConnectGlobal += RttManager.OnConnectionDiscovered;
			GameNetworkManager.onClientDisconnectGlobal += RttManager.OnConnectionLost;
			GameNetworkManager.onServerConnectGlobal += RttManager.OnConnectionDiscovered;
			GameNetworkManager.onServerDisconnectGlobal += RttManager.OnConnectionLost;
			RoR2Application.onUpdate += RttManager.Update;
			RoR2Application.onFixedUpdate += RttManager.FixedUpdate;
		}

		// Token: 0x06001FBD RID: 8125 RVA: 0x00089D28 File Offset: 0x00087F28
		private static void Update()
		{
			float deltaTime = Time.deltaTime;
			RttManager.ConnectionRttInfo[] array = RttManager.entries;
			for (int i = 0; i < array.Length; i++)
			{
				ref RttManager.ConnectionRttInfo ptr = ref array[i];
				RttManager.UpdateFilteredRtt(deltaTime, ptr.newestRttInSeconds, ref ptr.frameSmoothedRtt, ref ptr.frameVelocity);
			}
		}

		// Token: 0x06001FBE RID: 8126 RVA: 0x00089D70 File Offset: 0x00087F70
		private static void FixedUpdate()
		{
			float fixedDeltaTime = Time.fixedDeltaTime;
			RttManager.ConnectionRttInfo[] array = RttManager.entries;
			for (int i = 0; i < array.Length; i++)
			{
				ref RttManager.ConnectionRttInfo ptr = ref array[i];
				RttManager.UpdateFilteredRtt(fixedDeltaTime, ptr.newestRttInSeconds, ref ptr.fixedSmoothedRtt, ref ptr.fixedVelocity);
			}
		}

		// Token: 0x06001FBF RID: 8127 RVA: 0x00089DB8 File Offset: 0x00087FB8
		[NetworkMessageHandler(msgType = 65, client = true, server = true)]
		private static void HandlePing(NetworkMessage netMsg)
		{
			NetworkReader reader = netMsg.reader;
			netMsg.conn.SendByChannel(66, reader.ReadMessage<RttManager.PingMessage>(), netMsg.channelId);
		}

		// Token: 0x06001FC0 RID: 8128 RVA: 0x00089DE8 File Offset: 0x00087FE8
		[NetworkMessageHandler(msgType = 66, client = true, server = true)]
		private static void HandlePingResponse(NetworkMessage netMsg)
		{
			uint timeStampMs = netMsg.reader.ReadMessage<RttManager.PingMessage>().timeStampMs;
			uint num = (uint)RoR2Application.instance.stopwatch.ElapsedMilliseconds - timeStampMs;
			int num2;
			if (RttManager.FindConnectionIndex(netMsg.conn, out num2))
			{
				RttManager.ConnectionRttInfo[] array = RttManager.entries;
				int num3 = num2;
				array[num3].newestRttInMilliseconds = num;
				array[num3].newestRttInSeconds = num * 0.001f;
			}
		}

		// Token: 0x06001FC1 RID: 8129 RVA: 0x00089E48 File Offset: 0x00088048
		public static void Ping(NetworkConnection conn, int channelId)
		{
			conn.SendByChannel(65, new RttManager.PingMessage
			{
				timeStampMs = (uint)RoR2Application.instance.stopwatch.ElapsedMilliseconds
			}, channelId);
		}

		// Token: 0x04001D67 RID: 7527
		private static RttManager.ConnectionRttInfo[] entries = Array.Empty<RttManager.ConnectionRttInfo>();

		// Token: 0x04001D68 RID: 7528
		private static readonly FloatConVar cvRttSmoothDuration = new FloatConVar("net_rtt_smooth_duration", ConVarFlags.None, "0.1", "The smoothing duration for round-trip time values.");

		// Token: 0x02000541 RID: 1345
		private struct ConnectionRttInfo
		{
			// Token: 0x06001FC3 RID: 8131 RVA: 0x00089E98 File Offset: 0x00088098
			public ConnectionRttInfo(NetworkConnection connection)
			{
				this.connection = connection;
				this.newestRttInMilliseconds = 0U;
				this.newestRttInSeconds = 0f;
				this.frameSmoothedRtt = 0f;
				this.frameVelocity = 0f;
				this.fixedSmoothedRtt = 0f;
				this.fixedVelocity = 0f;
			}

			// Token: 0x04001D69 RID: 7529
			public readonly NetworkConnection connection;

			// Token: 0x04001D6A RID: 7530
			public float newestRttInSeconds;

			// Token: 0x04001D6B RID: 7531
			public uint newestRttInMilliseconds;

			// Token: 0x04001D6C RID: 7532
			public float frameSmoothedRtt;

			// Token: 0x04001D6D RID: 7533
			public float frameVelocity;

			// Token: 0x04001D6E RID: 7534
			public float fixedSmoothedRtt;

			// Token: 0x04001D6F RID: 7535
			public float fixedVelocity;
		}

		// Token: 0x02000542 RID: 1346
		private class PingMessage : MessageBase
		{
			// Token: 0x06001FC5 RID: 8133 RVA: 0x00089EEA File Offset: 0x000880EA
			public override void Serialize(NetworkWriter writer)
			{
				writer.WritePackedUInt32(this.timeStampMs);
			}

			// Token: 0x06001FC6 RID: 8134 RVA: 0x00089EF8 File Offset: 0x000880F8
			public override void Deserialize(NetworkReader reader)
			{
				this.timeStampMs = reader.ReadPackedUInt32();
			}

			// Token: 0x04001D70 RID: 7536
			public uint timeStampMs;
		}
	}
}
