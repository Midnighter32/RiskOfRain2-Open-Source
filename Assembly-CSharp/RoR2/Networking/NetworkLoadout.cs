using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000556 RID: 1366
	public class NetworkLoadout : NetworkBehaviour
	{
		// Token: 0x1400007D RID: 125
		// (add) Token: 0x0600209D RID: 8349 RVA: 0x0008D324 File Offset: 0x0008B524
		// (remove) Token: 0x0600209E RID: 8350 RVA: 0x0008D35C File Offset: 0x0008B55C
		public event Action onLoadoutUpdated;

		// Token: 0x0600209F RID: 8351 RVA: 0x0008D391 File Offset: 0x0008B591
		public void CopyLoadout(Loadout dest)
		{
			this.loadout.Copy(dest);
		}

		// Token: 0x060020A0 RID: 8352 RVA: 0x0008D39F File Offset: 0x0008B59F
		public void SetLoadout(Loadout src)
		{
			src.Copy(this.loadout);
			if (NetworkServer.active)
			{
				base.SetDirtyBit(1U);
			}
			else if (base.isLocalPlayer)
			{
				this.SendLoadoutClient();
			}
			this.OnLoadoutUpdated();
		}

		// Token: 0x060020A1 RID: 8353 RVA: 0x0008D3D4 File Offset: 0x0008B5D4
		[Command]
		private void CmdSendLoadout(byte[] bytes)
		{
			NetworkReader reader = new NetworkReader(bytes);
			NetworkLoadout.temp.Deserialize(reader);
			this.SetLoadout(NetworkLoadout.temp);
		}

		// Token: 0x060020A2 RID: 8354 RVA: 0x0008D400 File Offset: 0x0008B600
		[Client]
		private void SendLoadoutClient()
		{
			if (!NetworkClient.active)
			{
				Debug.LogWarning("[Client] function 'System.Void RoR2.Networking.NetworkLoadout::SendLoadoutClient()' called on server");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			this.loadout.Serialize(networkWriter);
			this.CallCmdSendLoadout(networkWriter.ToArray());
		}

		// Token: 0x060020A3 RID: 8355 RVA: 0x0008D440 File Offset: 0x0008B640
		private void OnLoadoutUpdated()
		{
			Action action = this.onLoadoutUpdated;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x060020A4 RID: 8356 RVA: 0x0008D454 File Offset: 0x0008B654
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint num = base.syncVarDirtyBits;
			if (initialState)
			{
				num = 1U;
			}
			writer.WritePackedUInt32(num);
			if ((num & 1U) != 0U)
			{
				this.loadout.Serialize(writer);
			}
			return num > 0U;
		}

		// Token: 0x060020A5 RID: 8357 RVA: 0x0008D489 File Offset: 0x0008B689
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if ((reader.ReadPackedUInt32() & 1U) != 0U)
			{
				NetworkLoadout.temp.Deserialize(reader);
				if (!base.isLocalPlayer)
				{
					NetworkLoadout.temp.Copy(this.loadout);
					this.OnLoadoutUpdated();
				}
			}
		}

		// Token: 0x060020A7 RID: 8359 RVA: 0x0008D4D4 File Offset: 0x0008B6D4
		static NetworkLoadout()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(NetworkLoadout), NetworkLoadout.kCmdCmdSendLoadout, new NetworkBehaviour.CmdDelegate(NetworkLoadout.InvokeCmdCmdSendLoadout));
			NetworkCRC.RegisterBehaviour("NetworkLoadout", 0);
		}

		// Token: 0x060020A8 RID: 8360 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x060020A9 RID: 8361 RVA: 0x0008D524 File Offset: 0x0008B724
		protected static void InvokeCmdCmdSendLoadout(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdSendLoadout called on client.");
				return;
			}
			((NetworkLoadout)obj).CmdSendLoadout(reader.ReadBytesAndSize());
		}

		// Token: 0x060020AA RID: 8362 RVA: 0x0008D550 File Offset: 0x0008B750
		public void CallCmdSendLoadout(byte[] bytes)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdSendLoadout called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdSendLoadout(bytes);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)NetworkLoadout.kCmdCmdSendLoadout);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.WriteBytesFull(bytes);
			base.SendCommandInternal(networkWriter, 0, "CmdSendLoadout");
		}

		// Token: 0x04001DD3 RID: 7635
		private static readonly Loadout temp = new Loadout();

		// Token: 0x04001DD4 RID: 7636
		private readonly Loadout loadout = new Loadout();

		// Token: 0x04001DD6 RID: 7638
		private static int kCmdCmdSendLoadout = 1217513894;
	}
}
