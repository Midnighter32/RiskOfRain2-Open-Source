using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000555 RID: 1365
	public class NetworkedViewAngles : NetworkBehaviour
	{
		// Token: 0x1700036F RID: 879
		// (get) Token: 0x0600208E RID: 8334 RVA: 0x0008D076 File Offset: 0x0008B276
		// (set) Token: 0x0600208F RID: 8335 RVA: 0x0008D07E File Offset: 0x0008B27E
		public bool hasEffectiveAuthority { get; private set; }

		// Token: 0x06002090 RID: 8336 RVA: 0x0008D087 File Offset: 0x0008B287
		private void Awake()
		{
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
		}

		// Token: 0x06002091 RID: 8337 RVA: 0x0008D098 File Offset: 0x0008B298
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

		// Token: 0x06002092 RID: 8338 RVA: 0x0008D0FB File Offset: 0x0008B2FB
		public override float GetNetworkSendInterval()
		{
			return this.sendRate;
		}

		// Token: 0x06002093 RID: 8339 RVA: 0x0008D103 File Offset: 0x0008B303
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.viewAngles.intVal;
		}

		// Token: 0x06002094 RID: 8340 RVA: 0x0008D110 File Offset: 0x0008B310
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				base.SetDirtyBit(1U);
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

		// Token: 0x06002095 RID: 8341 RVA: 0x0008D19E File Offset: 0x0008B39E
		[Command(channel = 5)]
		public void CmdUpdateViewAngles(float pitch, float yaw)
		{
			this.networkDesiredViewAngles = new PitchYawPair(pitch, yaw);
		}

		// Token: 0x06002096 RID: 8342 RVA: 0x0008D1AD File Offset: 0x0008B3AD
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			writer.Write(this.networkDesiredViewAngles);
			return true;
		}

		// Token: 0x06002097 RID: 8343 RVA: 0x0008D1BC File Offset: 0x0008B3BC
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

		// Token: 0x06002099 RID: 8345 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x0600209A RID: 8346 RVA: 0x0008D21E File Offset: 0x0008B41E
		protected static void InvokeCmdCmdUpdateViewAngles(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdUpdateViewAngles called on client.");
				return;
			}
			((NetworkedViewAngles)obj).CmdUpdateViewAngles(reader.ReadSingle(), reader.ReadSingle());
		}

		// Token: 0x0600209B RID: 8347 RVA: 0x0008D250 File Offset: 0x0008B450
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

		// Token: 0x0600209C RID: 8348 RVA: 0x0008D2E8 File Offset: 0x0008B4E8
		static NetworkedViewAngles()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(NetworkedViewAngles), NetworkedViewAngles.kCmdCmdUpdateViewAngles, new NetworkBehaviour.CmdDelegate(NetworkedViewAngles.InvokeCmdCmdUpdateViewAngles));
			NetworkCRC.RegisterBehaviour("NetworkedViewAngles", 0);
		}

		// Token: 0x04001DC9 RID: 7625
		public PitchYawPair viewAngles;

		// Token: 0x04001DCA RID: 7626
		private PitchYawPair networkDesiredViewAngles;

		// Token: 0x04001DCB RID: 7627
		private PitchYawPair velocity;

		// Token: 0x04001DCC RID: 7628
		private NetworkIdentity networkIdentity;

		// Token: 0x04001DCE RID: 7630
		public float sendRate = 0.05f;

		// Token: 0x04001DCF RID: 7631
		public float bufferMultiplier = 3f;

		// Token: 0x04001DD0 RID: 7632
		public float maxSmoothVelocity = 1440f;

		// Token: 0x04001DD1 RID: 7633
		private float sendTimer;

		// Token: 0x04001DD2 RID: 7634
		private static int kCmdCmdUpdateViewAngles = -1684781536;
	}
}
