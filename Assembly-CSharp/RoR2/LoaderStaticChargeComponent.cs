using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000274 RID: 628
	[RequireComponent(typeof(CharacterBody))]
	public class LoaderStaticChargeComponent : NetworkBehaviour, IOnDamageDealtServerReceiver, IOnTakeDamageServerReceiver
	{
		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x06000DEF RID: 3567 RVA: 0x0003E7D6 File Offset: 0x0003C9D6
		public float charge
		{
			get
			{
				return this._charge;
			}
		}

		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x06000DF0 RID: 3568 RVA: 0x0003E7DE File Offset: 0x0003C9DE
		public float chargeFraction
		{
			get
			{
				return this.charge / this.maxCharge;
			}
		}

		// Token: 0x06000DF1 RID: 3569 RVA: 0x0003E7ED File Offset: 0x0003C9ED
		private void Awake()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
		}

		// Token: 0x06000DF2 RID: 3570 RVA: 0x0003E7FB File Offset: 0x0003C9FB
		public void OnDamageDealtServer(DamageReport damageReport)
		{
			this.AddChargeServer(damageReport.damageDealt);
		}

		// Token: 0x06000DF3 RID: 3571 RVA: 0x0003E7FB File Offset: 0x0003C9FB
		public void OnTakeDamageServer(DamageReport damageReport)
		{
			this.AddChargeServer(damageReport.damageDealt);
		}

		// Token: 0x06000DF4 RID: 3572 RVA: 0x0003E80C File Offset: 0x0003CA0C
		[Server]
		private void AddChargeServer(float additionalCharge)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.LoaderStaticChargeComponent::AddChargeServer(System.Single)' called on client");
				return;
			}
			float num = this._charge + additionalCharge;
			if (num > this.maxCharge)
			{
				num = this.maxCharge;
			}
			this.Network_charge = num;
		}

		// Token: 0x06000DF5 RID: 3573 RVA: 0x0003E84E File Offset: 0x0003CA4E
		[Server]
		private void ConsumeChargeInternal()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.LoaderStaticChargeComponent::ConsumeChargeInternal()' called on client");
				return;
			}
			this.SetState(LoaderStaticChargeComponent.State.Drain);
		}

		// Token: 0x06000DF6 RID: 3574 RVA: 0x0003E86C File Offset: 0x0003CA6C
		public void ConsumeChargeAuthority()
		{
			if (NetworkServer.active)
			{
				this.ConsumeChargeInternal();
				return;
			}
			this.CallCmdConsumeCharge();
		}

		// Token: 0x06000DF7 RID: 3575 RVA: 0x0003E884 File Offset: 0x0003CA84
		private void SetState(LoaderStaticChargeComponent.State newState)
		{
			if (this.state == newState)
			{
				return;
			}
			if (this.state == LoaderStaticChargeComponent.State.Drain && NetworkServer.active)
			{
				this.characterBody.RemoveBuff(BuffIndex.LoaderOvercharged);
			}
			this.state = newState;
			if (this.state == LoaderStaticChargeComponent.State.Drain && NetworkServer.active)
			{
				this.characterBody.AddBuff(BuffIndex.LoaderOvercharged);
			}
		}

		// Token: 0x06000DF8 RID: 3576 RVA: 0x0003E8DC File Offset: 0x0003CADC
		private void FixedUpdate()
		{
			if (NetworkServer.active && this.state == LoaderStaticChargeComponent.State.Drain)
			{
				this.Network_charge = this._charge - Time.fixedDeltaTime * this.consumptionRate;
				if (this._charge <= 0f)
				{
					this.Network_charge = 0f;
					this.SetState(LoaderStaticChargeComponent.State.Idle);
				}
			}
		}

		// Token: 0x06000DF9 RID: 3577 RVA: 0x0003E931 File Offset: 0x0003CB31
		[Command]
		private void CmdConsumeCharge()
		{
			this.ConsumeChargeInternal();
		}

		// Token: 0x06000DFB RID: 3579 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x06000DFC RID: 3580 RVA: 0x0003E958 File Offset: 0x0003CB58
		// (set) Token: 0x06000DFD RID: 3581 RVA: 0x0003E96B File Offset: 0x0003CB6B
		public float Network_charge
		{
			get
			{
				return this._charge;
			}
			[param: In]
			set
			{
				base.SetSyncVar<float>(value, ref this._charge, 1U);
			}
		}

		// Token: 0x06000DFE RID: 3582 RVA: 0x0003E97F File Offset: 0x0003CB7F
		protected static void InvokeCmdCmdConsumeCharge(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdConsumeCharge called on client.");
				return;
			}
			((LoaderStaticChargeComponent)obj).CmdConsumeCharge();
		}

		// Token: 0x06000DFF RID: 3583 RVA: 0x0003E9A4 File Offset: 0x0003CBA4
		public void CallCmdConsumeCharge()
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdConsumeCharge called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdConsumeCharge();
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)LoaderStaticChargeComponent.kCmdCmdConsumeCharge);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			base.SendCommandInternal(networkWriter, 0, "CmdConsumeCharge");
		}

		// Token: 0x06000E00 RID: 3584 RVA: 0x0003EA20 File Offset: 0x0003CC20
		static LoaderStaticChargeComponent()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(LoaderStaticChargeComponent), LoaderStaticChargeComponent.kCmdCmdConsumeCharge, new NetworkBehaviour.CmdDelegate(LoaderStaticChargeComponent.InvokeCmdCmdConsumeCharge));
			NetworkCRC.RegisterBehaviour("LoaderStaticChargeComponent", 0);
		}

		// Token: 0x06000E01 RID: 3585 RVA: 0x0003EA5C File Offset: 0x0003CC5C
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this._charge);
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
				writer.Write(this._charge);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000E02 RID: 3586 RVA: 0x0003EAC8 File Offset: 0x0003CCC8
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this._charge = reader.ReadSingle();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this._charge = reader.ReadSingle();
			}
		}

		// Token: 0x04000DF0 RID: 3568
		public float maxCharge = 100f;

		// Token: 0x04000DF1 RID: 3569
		public float consumptionRate = 10f;

		// Token: 0x04000DF2 RID: 3570
		[SyncVar]
		private float _charge;

		// Token: 0x04000DF3 RID: 3571
		private CharacterBody characterBody;

		// Token: 0x04000DF4 RID: 3572
		private LoaderStaticChargeComponent.State state;

		// Token: 0x04000DF5 RID: 3573
		private static int kCmdCmdConsumeCharge = -261598328;

		// Token: 0x02000275 RID: 629
		private enum State
		{
			// Token: 0x04000DF7 RID: 3575
			Idle,
			// Token: 0x04000DF8 RID: 3576
			Drain
		}
	}
}
