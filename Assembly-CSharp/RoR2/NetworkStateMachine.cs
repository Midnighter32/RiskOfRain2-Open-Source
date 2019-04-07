using System;
using System.Collections.Generic;
using EntityStates;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000370 RID: 880
	[DisallowMultipleComponent]
	public class NetworkStateMachine : NetworkBehaviour
	{
		// Token: 0x06001229 RID: 4649 RVA: 0x000595F0 File Offset: 0x000577F0
		private void Awake()
		{
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
			for (int i = 0; i < this.stateMachines.Length; i++)
			{
				this.stateMachines[i].networkIndex = i;
			}
		}

		// Token: 0x0600122A RID: 4650 RVA: 0x0005962C File Offset: 0x0005782C
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			if (initialState)
			{
				for (int i = 0; i < this.stateMachines.Length; i++)
				{
					EntityStateMachine entityStateMachine = this.stateMachines[i];
					writer.WritePackedUInt32((uint)(StateIndexTable.TypeToIndex(entityStateMachine.state.GetType()) + 1));
					entityStateMachine.state.OnSerialize(writer);
				}
				return true;
			}
			return false;
		}

		// Token: 0x0600122B RID: 4651 RVA: 0x00059680 File Offset: 0x00057880
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				for (int i = 0; i < this.stateMachines.Length; i++)
				{
					EntityStateMachine entityStateMachine = this.stateMachines[i];
					short stateTypeIndex = (short)reader.ReadPackedUInt32() - 1;
					if (!base.hasAuthority)
					{
						EntityState entityState = EntityState.Instantiate(stateTypeIndex);
						if (entityState != null)
						{
							entityState.OnDeserialize(reader);
							if (!this.stateMachines[i])
							{
								Debug.LogErrorFormat("State machine [{0}] on object {1} is not set! incoming state = {2}", new object[]
								{
									i,
									base.gameObject,
									entityState.GetType()
								});
							}
							entityStateMachine.SetNextState(entityState);
						}
					}
				}
			}
		}

		// Token: 0x0600122C RID: 4652 RVA: 0x00059714 File Offset: 0x00057914
		[NetworkMessageHandler(msgType = 48, client = true, server = true)]
		public static void HandleSetEntityState(NetworkMessage netMsg)
		{
			NetworkIdentity networkIdentity = netMsg.reader.ReadNetworkIdentity();
			byte b = netMsg.reader.ReadByte();
			short stateTypeIndex = netMsg.reader.ReadInt16();
			if (networkIdentity == null)
			{
				return;
			}
			NetworkStateMachine component = networkIdentity.gameObject.GetComponent<NetworkStateMachine>();
			if (component == null || (int)b >= component.stateMachines.Length)
			{
				return;
			}
			EntityStateMachine entityStateMachine = component.stateMachines[(int)b];
			if (entityStateMachine == null)
			{
				return;
			}
			if (networkIdentity.isServer)
			{
				HashSet<NetworkInstanceId> clientOwnedObjects = netMsg.conn.clientOwnedObjects;
				if (clientOwnedObjects == null || !clientOwnedObjects.Contains(networkIdentity.netId))
				{
					return;
				}
			}
			else if (networkIdentity.hasAuthority)
			{
				return;
			}
			EntityState entityState = EntityState.Instantiate(stateTypeIndex);
			if (entityState == null)
			{
				return;
			}
			entityState.OnDeserialize(netMsg.reader);
			entityStateMachine.SetState(entityState);
		}

		// Token: 0x0600122D RID: 4653 RVA: 0x000597DC File Offset: 0x000579DC
		public void SendSetEntityState(int stateMachineIndex)
		{
			if (!NetworkServer.active && !base.hasAuthority)
			{
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			EntityStateMachine entityStateMachine = this.stateMachines[stateMachineIndex];
			short value = StateIndexTable.TypeToIndex(entityStateMachine.state.GetType());
			networkWriter.StartMessage(48);
			networkWriter.Write(this.networkIdentity);
			networkWriter.Write((byte)stateMachineIndex);
			networkWriter.Write(value);
			entityStateMachine.state.OnSerialize(networkWriter);
			networkWriter.FinishMessage();
			if (NetworkServer.active)
			{
				NetworkServer.SendWriterToReady(base.gameObject, networkWriter, this.GetNetworkChannel());
				return;
			}
			if (ClientScene.readyConnection != null)
			{
				ClientScene.readyConnection.SendWriter(networkWriter, this.GetNetworkChannel());
			}
		}

		// Token: 0x0600122E RID: 4654 RVA: 0x00059880 File Offset: 0x00057A80
		private void OnValidate()
		{
			for (int i = 0; i < this.stateMachines.Length; i++)
			{
				if (!this.stateMachines[i])
				{
					Debug.LogErrorFormat("{0} has a blank entry for NetworkStateMachine!", new object[]
					{
						base.gameObject
					});
				}
			}
		}

		// Token: 0x06001230 RID: 4656 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x04001615 RID: 5653
		[SerializeField]
		[Tooltip("The sibling state machine components to network.")]
		private EntityStateMachine[] stateMachines;

		// Token: 0x04001616 RID: 5654
		private NetworkIdentity networkIdentity;
	}
}
