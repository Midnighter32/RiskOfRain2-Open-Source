using System;
using System.Collections.Generic;
using EntityStates;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002A1 RID: 673
	[DisallowMultipleComponent]
	public class NetworkStateMachine : NetworkBehaviour
	{
		// Token: 0x06000F0A RID: 3850 RVA: 0x00042694 File Offset: 0x00040894
		private void Awake()
		{
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
			for (int i = 0; i < this.stateMachines.Length; i++)
			{
				this.stateMachines[i].networkIndex = i;
			}
		}

		// Token: 0x06000F0B RID: 3851 RVA: 0x000426D0 File Offset: 0x000408D0
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

		// Token: 0x06000F0C RID: 3852 RVA: 0x00042724 File Offset: 0x00040924
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
							entityState.outer = entityStateMachine;
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

		// Token: 0x06000F0D RID: 3853 RVA: 0x000427C4 File Offset: 0x000409C4
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
			entityState.outer = entityStateMachine;
			entityState.OnDeserialize(netMsg.reader);
			entityStateMachine.SetState(entityState);
		}

		// Token: 0x06000F0E RID: 3854 RVA: 0x00042894 File Offset: 0x00040A94
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

		// Token: 0x06000F0F RID: 3855 RVA: 0x00042938 File Offset: 0x00040B38
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

		// Token: 0x06000F11 RID: 3857 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x04000EC3 RID: 3779
		[SerializeField]
		[Tooltip("The sibling state machine components to network.")]
		private EntityStateMachine[] stateMachines = Array.Empty<EntityStateMachine>();

		// Token: 0x04000EC4 RID: 3780
		private NetworkIdentity networkIdentity;
	}
}
