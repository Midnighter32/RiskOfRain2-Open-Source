using System;
using EntityStates;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000326 RID: 806
	public class SetStateOnHurt : NetworkBehaviour, IOnTakeDamageServerReceiver
	{
		// Token: 0x060012ED RID: 4845 RVA: 0x00051326 File Offset: 0x0004F526
		public override void OnStartAuthority()
		{
			base.OnStartAuthority();
			this.UpdateAuthority();
		}

		// Token: 0x060012EE RID: 4846 RVA: 0x00051334 File Offset: 0x0004F534
		public override void OnStopAuthority()
		{
			base.OnStopAuthority();
			this.UpdateAuthority();
		}

		// Token: 0x060012EF RID: 4847 RVA: 0x00051342 File Offset: 0x0004F542
		private void UpdateAuthority()
		{
			this.hasEffectiveAuthority = Util.HasEffectiveAuthority(base.gameObject);
		}

		// Token: 0x060012F0 RID: 4848 RVA: 0x00051355 File Offset: 0x0004F555
		private void Start()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
			this.UpdateAuthority();
		}

		// Token: 0x060012F1 RID: 4849 RVA: 0x0005136C File Offset: 0x0004F56C
		public void OnTakeDamageServer(DamageReport damageReport)
		{
			DamageInfo damageInfo = damageReport.damageInfo;
			if (this.targetStateMachine && base.isServer && this.characterBody)
			{
				bool flag = damageInfo.procCoefficient >= Mathf.Epsilon;
				float num = damageInfo.crit ? (damageInfo.damage * 2f) : damageInfo.damage;
				if (flag && this.canBeFrozen && (damageInfo.damageType & DamageType.Freeze2s) != DamageType.Generic)
				{
					this.SetFrozen(2f * damageInfo.procCoefficient);
					return;
				}
				if (!this.characterBody.healthComponent.isInFrozenState)
				{
					if (flag && this.canBeStunned && (damageInfo.damageType & DamageType.Stun1s) != DamageType.Generic)
					{
						this.SetStun(1f);
						return;
					}
					if (this.canBeHitStunned && num > this.characterBody.maxHealth * this.hitThreshold)
					{
						this.SetPain();
					}
				}
			}
		}

		// Token: 0x060012F2 RID: 4850 RVA: 0x0005145C File Offset: 0x0004F65C
		[Server]
		public void SetStun(float duration)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.SetStateOnHurt::SetStun(System.Single)' called on client");
				return;
			}
			if (this.hasEffectiveAuthority)
			{
				this.SetStunInternal(duration);
				return;
			}
			this.CallRpcSetStun(duration);
		}

		// Token: 0x060012F3 RID: 4851 RVA: 0x0005148A File Offset: 0x0004F68A
		[ClientRpc]
		private void RpcSetStun(float duration)
		{
			if (this.hasEffectiveAuthority)
			{
				this.SetStunInternal(duration);
			}
		}

		// Token: 0x060012F4 RID: 4852 RVA: 0x0005149C File Offset: 0x0004F69C
		private void SetStunInternal(float duration)
		{
			if (this.targetStateMachine)
			{
				StunState stunState = new StunState();
				stunState.stunDuration = duration;
				this.targetStateMachine.SetInterruptState(stunState, InterruptPriority.Pain);
			}
			EntityStateMachine[] array = this.idleStateMachine;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetNextStateToMain();
			}
		}

		// Token: 0x060012F5 RID: 4853 RVA: 0x000514EE File Offset: 0x0004F6EE
		[Server]
		public void SetFrozen(float duration)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.SetStateOnHurt::SetFrozen(System.Single)' called on client");
				return;
			}
			if (this.hasEffectiveAuthority)
			{
				this.SetFrozenInternal(duration);
				return;
			}
			this.CallRpcSetFrozen(duration);
		}

		// Token: 0x060012F6 RID: 4854 RVA: 0x0005151C File Offset: 0x0004F71C
		[ClientRpc]
		private void RpcSetFrozen(float duration)
		{
			if (this.hasEffectiveAuthority)
			{
				this.SetFrozenInternal(duration);
			}
		}

		// Token: 0x060012F7 RID: 4855 RVA: 0x00051530 File Offset: 0x0004F730
		private void SetFrozenInternal(float duration)
		{
			if (this.targetStateMachine)
			{
				FrozenState frozenState = new FrozenState();
				frozenState.freezeDuration = duration;
				this.targetStateMachine.SetInterruptState(frozenState, InterruptPriority.Frozen);
			}
			EntityStateMachine[] array = this.idleStateMachine;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetNextState(new Idle());
			}
		}

		// Token: 0x060012F8 RID: 4856 RVA: 0x00051587 File Offset: 0x0004F787
		[Server]
		public void SetPain()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.SetStateOnHurt::SetPain()' called on client");
				return;
			}
			if (this.hasEffectiveAuthority)
			{
				this.SetPainInternal();
				return;
			}
			this.CallRpcSetPain();
		}

		// Token: 0x060012F9 RID: 4857 RVA: 0x000515B3 File Offset: 0x0004F7B3
		[ClientRpc]
		private void RpcSetPain()
		{
			if (this.hasEffectiveAuthority)
			{
				this.SetPainInternal();
			}
		}

		// Token: 0x060012FA RID: 4858 RVA: 0x000515C4 File Offset: 0x0004F7C4
		private void SetPainInternal()
		{
			if (this.targetStateMachine)
			{
				this.targetStateMachine.SetInterruptState(EntityState.Instantiate(this.hurtState), InterruptPriority.Pain);
			}
			EntityStateMachine[] array = this.idleStateMachine;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetNextState(new Idle());
			}
		}

		// Token: 0x060012FB RID: 4859 RVA: 0x00051618 File Offset: 0x0004F818
		[Server]
		public void Cleanse()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.SetStateOnHurt::Cleanse()' called on client");
				return;
			}
			if (this.hasEffectiveAuthority)
			{
				this.CleanseInternal();
				return;
			}
			this.CallRpcCleanse();
		}

		// Token: 0x060012FC RID: 4860 RVA: 0x00051644 File Offset: 0x0004F844
		[ClientRpc]
		private void RpcCleanse()
		{
			if (this.hasEffectiveAuthority)
			{
				this.CleanseInternal();
			}
		}

		// Token: 0x060012FD RID: 4861 RVA: 0x00051654 File Offset: 0x0004F854
		private void CleanseInternal()
		{
			if (this.targetStateMachine && (this.targetStateMachine.state is FrozenState || this.targetStateMachine.state is StunState))
			{
				this.targetStateMachine.SetInterruptState(EntityState.Instantiate(this.targetStateMachine.mainStateType), InterruptPriority.Frozen);
			}
		}

		// Token: 0x060012FF RID: 4863 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x06001300 RID: 4864 RVA: 0x000516DE File Offset: 0x0004F8DE
		protected static void InvokeRpcRpcSetStun(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcSetStun called on server.");
				return;
			}
			((SetStateOnHurt)obj).RpcSetStun(reader.ReadSingle());
		}

		// Token: 0x06001301 RID: 4865 RVA: 0x00051708 File Offset: 0x0004F908
		protected static void InvokeRpcRpcSetFrozen(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcSetFrozen called on server.");
				return;
			}
			((SetStateOnHurt)obj).RpcSetFrozen(reader.ReadSingle());
		}

		// Token: 0x06001302 RID: 4866 RVA: 0x00051732 File Offset: 0x0004F932
		protected static void InvokeRpcRpcSetPain(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcSetPain called on server.");
				return;
			}
			((SetStateOnHurt)obj).RpcSetPain();
		}

		// Token: 0x06001303 RID: 4867 RVA: 0x00051755 File Offset: 0x0004F955
		protected static void InvokeRpcRpcCleanse(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcCleanse called on server.");
				return;
			}
			((SetStateOnHurt)obj).RpcCleanse();
		}

		// Token: 0x06001304 RID: 4868 RVA: 0x00051778 File Offset: 0x0004F978
		public void CallRpcSetStun(float duration)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcSetStun called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)SetStateOnHurt.kRpcRpcSetStun);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(duration);
			this.SendRPCInternal(networkWriter, 0, "RpcSetStun");
		}

		// Token: 0x06001305 RID: 4869 RVA: 0x000517EC File Offset: 0x0004F9EC
		public void CallRpcSetFrozen(float duration)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcSetFrozen called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)SetStateOnHurt.kRpcRpcSetFrozen);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(duration);
			this.SendRPCInternal(networkWriter, 0, "RpcSetFrozen");
		}

		// Token: 0x06001306 RID: 4870 RVA: 0x00051860 File Offset: 0x0004FA60
		public void CallRpcSetPain()
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcSetPain called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)SetStateOnHurt.kRpcRpcSetPain);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			this.SendRPCInternal(networkWriter, 0, "RpcSetPain");
		}

		// Token: 0x06001307 RID: 4871 RVA: 0x000518CC File Offset: 0x0004FACC
		public void CallRpcCleanse()
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcCleanse called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)SetStateOnHurt.kRpcRpcCleanse);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			this.SendRPCInternal(networkWriter, 0, "RpcCleanse");
		}

		// Token: 0x06001308 RID: 4872 RVA: 0x00051938 File Offset: 0x0004FB38
		static SetStateOnHurt()
		{
			NetworkBehaviour.RegisterRpcDelegate(typeof(SetStateOnHurt), SetStateOnHurt.kRpcRpcSetStun, new NetworkBehaviour.CmdDelegate(SetStateOnHurt.InvokeRpcRpcSetStun));
			SetStateOnHurt.kRpcRpcSetFrozen = 1781279215;
			NetworkBehaviour.RegisterRpcDelegate(typeof(SetStateOnHurt), SetStateOnHurt.kRpcRpcSetFrozen, new NetworkBehaviour.CmdDelegate(SetStateOnHurt.InvokeRpcRpcSetFrozen));
			SetStateOnHurt.kRpcRpcSetPain = 788726245;
			NetworkBehaviour.RegisterRpcDelegate(typeof(SetStateOnHurt), SetStateOnHurt.kRpcRpcSetPain, new NetworkBehaviour.CmdDelegate(SetStateOnHurt.InvokeRpcRpcSetPain));
			SetStateOnHurt.kRpcRpcCleanse = -339360280;
			NetworkBehaviour.RegisterRpcDelegate(typeof(SetStateOnHurt), SetStateOnHurt.kRpcRpcCleanse, new NetworkBehaviour.CmdDelegate(SetStateOnHurt.InvokeRpcRpcCleanse));
			NetworkCRC.RegisterBehaviour("SetStateOnHurt", 0);
		}

		// Token: 0x06001309 RID: 4873 RVA: 0x000519FC File Offset: 0x0004FBFC
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x0600130A RID: 4874 RVA: 0x0000409B File Offset: 0x0000229B
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040011C6 RID: 4550
		[Tooltip("The percentage of their max HP they need to take to get stunned. Ranges from 0-1.")]
		public float hitThreshold = 0.1f;

		// Token: 0x040011C7 RID: 4551
		[Tooltip("The state machine to set the state of when this character is hurt.")]
		public EntityStateMachine targetStateMachine;

		// Token: 0x040011C8 RID: 4552
		[Tooltip("The state machine to set to idle when this character is hurt.")]
		public EntityStateMachine[] idleStateMachine;

		// Token: 0x040011C9 RID: 4553
		[Tooltip("The state to enter when this character is hurt.")]
		public SerializableEntityStateType hurtState;

		// Token: 0x040011CA RID: 4554
		public bool canBeHitStunned = true;

		// Token: 0x040011CB RID: 4555
		public bool canBeStunned = true;

		// Token: 0x040011CC RID: 4556
		public bool canBeFrozen = true;

		// Token: 0x040011CD RID: 4557
		private bool hasEffectiveAuthority = true;

		// Token: 0x040011CE RID: 4558
		private CharacterBody characterBody;

		// Token: 0x040011CF RID: 4559
		private static int kRpcRpcSetStun = 788834249;

		// Token: 0x040011D0 RID: 4560
		private static int kRpcRpcSetFrozen;

		// Token: 0x040011D1 RID: 4561
		private static int kRpcRpcSetPain;

		// Token: 0x040011D2 RID: 4562
		private static int kRpcRpcCleanse;
	}
}
