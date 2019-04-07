using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000588 RID: 1416
	public class NetworkedViewAngles : NetworkBehaviour
	{
		// Token: 0x170002CC RID: 716
		// (get) Token: 0x06001FE2 RID: 8162 RVA: 0x0009618E File Offset: 0x0009438E
		// (set) Token: 0x06001FE3 RID: 8163 RVA: 0x00096196 File Offset: 0x00094396
		public bool hasEffectiveAuthority { get; private set; }

		// Token: 0x06001FE4 RID: 8164 RVA: 0x0009619F File Offset: 0x0009439F
		private void Awake()
		{
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
		}

		// Token: 0x06001FE5 RID: 8165 RVA: 0x000961B0 File Offset: 0x000943B0
		private void Update()
		{
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(this.networkIdentity);
			if (this.hasEffectiveAuthority)
			{
				this.networkDesiredViewAngles = this.viewAngles;
				return;
			}
			this.viewAngles = PitchYawPair.SmoothDamp(this.viewAngles, this.networkDesiredViewAngles, ref this.velocity, this.GetNetworkSendInterval() * this.bufferMultiplier, this.maxSmoothVelocity);
		}

		// Token: 0x06001FE6 RID: 8166 RVA: 0x00096213 File Offset: 0x00094413
		public override float GetNetworkSendInterval()
		{
			return this.sendRate;
		}

		// Token: 0x06001FE7 RID: 8167 RVA: 0x0009621B File Offset: 0x0009441B
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.viewAngles.intVal;
		}

		// Token: 0x06001FE8 RID: 8168 RVA: 0x00096228 File Offset: 0x00094428
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				base.SetDirtyBit(1u);
			}
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(this.networkIdentity);
			if (this.hasEffectiveAuthority)
			{
				this.networkDesiredViewAngles = this.viewAngles;
				if (!NetworkServer.active)
				{
					this.sendTimer -= Time.deltaTime;
					if (this.sendTimer <= 0f)
					{
						this.CallCmdUpdateViewAngles(this.viewAngles.pitch, this.viewAngles.yaw);
						this.sendTimer = this.GetNetworkSendInterval();
					}
				}
			}
		}

		// Token: 0x06001FE9 RID: 8169 RVA: 0x000962B6 File Offset: 0x000944B6
		[Command(channel = 5)]
		public void CmdUpdateViewAngles(float pitch, float yaw)
		{
			this.networkDesiredViewAngles = new PitchYawPair(pitch, yaw);
		}

		// Token: 0x06001FEA RID: 8170 RVA: 0x000962C5 File Offset: 0x000944C5
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			writer.Write(this.networkDesiredViewAngles);
			return !initialState;
		}

		// Token: 0x06001FEB RID: 8171 RVA: 0x000962D8 File Offset: 0x000944D8
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			PitchYawPair pitchYawPair = reader.ReadPitchYawPair();
			if (this.hasEffectiveAuthority)
			{
				return;
			}
			this.networkDesiredViewAngles = pitchYawPair;
			if (initialState)
			{
				this.viewAngles = pitchYawPair;
				this.velocity = PitchYawPair.zero;
			}
		}

		// Token: 0x06001FED RID: 8173 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x06001FEE RID: 8174 RVA: 0x0009633A File Offset: 0x0009453A
		protected static void InvokeCmdCmdUpdateViewAngles(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdUpdateViewAngles called on client.");
				return;
			}
			((NetworkedViewAngles)obj).CmdUpdateViewAngles(reader.ReadSingle(), reader.ReadSingle());
		}

		// Token: 0x06001FEF RID: 8175 RVA: 0x0009636C File Offset: 0x0009456C
		public void CallCmdUpdateViewAngles(float pitch, float yaw)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdUpdateViewAngles called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdUpdateViewAngles(pitch, yaw);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)NetworkedViewAngles.kCmdCmdUpdateViewAngles);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(pitch);
			networkWriter.Write(yaw);
			base.SendCommandInternal(networkWriter, 5, "CmdUpdateViewAngles");
		}

		// Token: 0x06001FF0 RID: 8176 RVA: 0x00096404 File Offset: 0x00094604
		static NetworkedViewAngles()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(NetworkedViewAngles), NetworkedViewAngles.kCmdCmdUpdateViewAngles, new NetworkBehaviour.CmdDelegate(NetworkedViewAngles.InvokeCmdCmdUpdateViewAngles));
			NetworkCRC.RegisterBehaviour("NetworkedViewAngles", 0);
		}

		// Token: 0x04002224 RID: 8740
		public PitchYawPair viewAngles;

		// Token: 0x04002225 RID: 8741
		private PitchYawPair networkDesiredViewAngles;

		// Token: 0x04002226 RID: 8742
		private PitchYawPair velocity;

		// Token: 0x04002227 RID: 8743
		private NetworkIdentity networkIdentity;

		// Token: 0x04002229 RID: 8745
		public float sendRate = 0.05f;

		// Token: 0x0400222A RID: 8746
		public float bufferMultiplier = 3f;

		// Token: 0x0400222B RID: 8747
		public float maxSmoothVelocity = 1440f;

		// Token: 0x0400222C RID: 8748
		private float sendTimer;

		// Token: 0x0400222D RID: 8749
		private static int kCmdCmdUpdateViewAngles = -1684781536;
	}
}
