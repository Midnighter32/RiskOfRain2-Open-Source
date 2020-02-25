using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x0200053B RID: 1339
	public class CharacterNetworkTransformManager : MonoBehaviour
	{
		// Token: 0x06001FA1 RID: 8097 RVA: 0x000894C2 File Offset: 0x000876C2
		private void Awake()
		{
			CharacterNetworkTransformManager.instance = this;
		}

		// Token: 0x06001FA2 RID: 8098 RVA: 0x000894CA File Offset: 0x000876CA
		[NetworkMessageHandler(msgType = 51, client = true, server = true)]
		private static void HandleTransformUpdates(NetworkMessage netMsg)
		{
			if (CharacterNetworkTransformManager.instance)
			{
				CharacterNetworkTransformManager.instance.HandleTransformUpdatesInternal(netMsg);
			}
		}

		// Token: 0x06001FA3 RID: 8099 RVA: 0x000894E4 File Offset: 0x000876E4
		private void HandleTransformUpdatesInternal(NetworkMessage netMsg)
		{
			uint num = (uint)netMsg.reader.ReadByte();
			float filteredClientRttFixed = GameNetworkManager.singleton.filteredClientRttFixed;
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				netMsg.ReadMessage<CharacterNetworkTransformManager.CharacterUpdateMessage>(this.currentInMessage);
				GameObject gameObject = this.currentInMessage.gameObject;
				if (gameObject && (!NetworkServer.active || gameObject.GetComponent<NetworkIdentity>().clientAuthorityOwner == netMsg.conn))
				{
					CharacterNetworkTransform component = gameObject.GetComponent<CharacterNetworkTransform>();
					if (component && !component.hasEffectiveAuthority)
					{
						CharacterNetworkTransform.Snapshot snapshot = new CharacterNetworkTransform.Snapshot
						{
							serverTime = this.currentInMessage.timestamp,
							position = this.currentInMessage.newPosition,
							moveVector = this.currentInMessage.moveVector,
							aimDirection = this.currentInMessage.aimDirection,
							rotation = this.currentInMessage.rotation,
							isGrounded = this.currentInMessage.isGrounded,
							groundNormal = this.currentInMessage.groundNormal
						};
						if (NetworkClient.active)
						{
							snapshot.serverTime += filteredClientRttFixed;
						}
						component.PushSnapshot(snapshot);
						if (NetworkServer.active)
						{
							this.snapshotQueue.Enqueue(new CharacterNetworkTransformManager.NetSnapshot
							{
								gameObject = component.gameObject,
								snapshot = snapshot
							});
						}
					}
				}
				num2++;
			}
		}

		// Token: 0x06001FA4 RID: 8100 RVA: 0x00089658 File Offset: 0x00087858
		private void ProcessQueue()
		{
			if (this.snapshotQueue.Count == 0)
			{
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.StartMessage(51);
			int num = Mathf.Min(Mathf.FloorToInt((float)(1000 - networkWriter.Position) / (float)CharacterNetworkTransformManager.CharacterUpdateMessage.maxNetworkSize), this.snapshotQueue.Count);
			networkWriter.Write((byte)num);
			for (int i = 0; i < num; i++)
			{
				CharacterNetworkTransformManager.NetSnapshot netSnapshot = this.snapshotQueue.Dequeue();
				this.currentOutMessage.gameObject = netSnapshot.gameObject;
				this.currentOutMessage.newPosition = netSnapshot.snapshot.position;
				this.currentOutMessage.aimDirection = netSnapshot.snapshot.aimDirection;
				this.currentOutMessage.moveVector = netSnapshot.snapshot.moveVector;
				this.currentOutMessage.rotation = netSnapshot.snapshot.rotation;
				this.currentOutMessage.isGrounded = netSnapshot.snapshot.isGrounded;
				this.currentOutMessage.timestamp = netSnapshot.snapshot.serverTime;
				this.currentOutMessage.groundNormal = netSnapshot.snapshot.groundNormal;
				networkWriter.Write(this.currentOutMessage);
			}
			networkWriter.FinishMessage();
			if (NetworkServer.active)
			{
				NetworkServer.SendWriterToReady(null, networkWriter, QosChannelIndex.characterTransformUnreliable.intVal);
				return;
			}
			if (ClientScene.readyConnection != null)
			{
				ClientScene.readyConnection.SendWriter(networkWriter, QosChannelIndex.characterTransformUnreliable.intVal);
			}
		}

		// Token: 0x06001FA5 RID: 8101 RVA: 0x000897C4 File Offset: 0x000879C4
		private void FixedUpdate()
		{
			if (!NetworkManager.singleton)
			{
				return;
			}
			ReadOnlyCollection<CharacterNetworkTransform> readOnlyInstancesList = CharacterNetworkTransform.readOnlyInstancesList;
			float fixedTime = Time.fixedTime;
			for (int i = 0; i < readOnlyInstancesList.Count; i++)
			{
				CharacterNetworkTransform characterNetworkTransform = readOnlyInstancesList[i];
				if (characterNetworkTransform.hasEffectiveAuthority && fixedTime - characterNetworkTransform.lastPositionTransmitTime > characterNetworkTransform.positionTransmitInterval)
				{
					characterNetworkTransform.lastPositionTransmitTime = fixedTime;
					this.snapshotQueue.Enqueue(new CharacterNetworkTransformManager.NetSnapshot
					{
						gameObject = characterNetworkTransform.gameObject,
						snapshot = characterNetworkTransform.newestNetSnapshot
					});
				}
			}
			while (this.snapshotQueue.Count > 0)
			{
				this.ProcessQueue();
			}
		}

		// Token: 0x04001D4D RID: 7501
		private static CharacterNetworkTransformManager instance;

		// Token: 0x04001D4E RID: 7502
		private CharacterNetworkTransformManager.CharacterUpdateMessage currentInMessage = new CharacterNetworkTransformManager.CharacterUpdateMessage();

		// Token: 0x04001D4F RID: 7503
		private CharacterNetworkTransformManager.CharacterUpdateMessage currentOutMessage = new CharacterNetworkTransformManager.CharacterUpdateMessage();

		// Token: 0x04001D50 RID: 7504
		private readonly Queue<CharacterNetworkTransformManager.NetSnapshot> snapshotQueue = new Queue<CharacterNetworkTransformManager.NetSnapshot>();

		// Token: 0x0200053C RID: 1340
		private class CharacterUpdateMessage : MessageBase
		{
			// Token: 0x06001FA7 RID: 8103 RVA: 0x00089890 File Offset: 0x00087A90
			public override void Serialize(NetworkWriter writer)
			{
				base.Serialize(writer);
				byte b = 0;
				bool flag = this.rotation != Quaternion.identity;
				if (flag)
				{
					b |= CharacterNetworkTransformManager.CharacterUpdateMessage.nonIdentityRotationBit;
				}
				if (this.isGrounded)
				{
					b |= CharacterNetworkTransformManager.CharacterUpdateMessage.isGroundedBit;
				}
				writer.Write(b);
				writer.Write(this.timestamp);
				writer.Write(this.gameObject);
				writer.Write(this.newPosition);
				writer.Write(new PackedUnitVector3(this.aimDirection));
				writer.Write(this.moveVector);
				if (flag)
				{
					writer.Write(this.rotation);
				}
				if (this.isGrounded)
				{
					writer.Write(new PackedUnitVector3(this.groundNormal));
				}
			}

			// Token: 0x06001FA8 RID: 8104 RVA: 0x00089944 File Offset: 0x00087B44
			public override void Deserialize(NetworkReader reader)
			{
				base.Deserialize(reader);
				byte b = reader.ReadByte();
				bool flag = (b & CharacterNetworkTransformManager.CharacterUpdateMessage.nonIdentityRotationBit) > 0;
				this.isGrounded = ((b & CharacterNetworkTransformManager.CharacterUpdateMessage.isGroundedBit) > 0);
				this.timestamp = reader.ReadSingle();
				this.gameObject = reader.ReadGameObject();
				this.newPosition = reader.ReadVector3();
				this.aimDirection = reader.ReadPackedUnitVector3().Unpack();
				this.moveVector = reader.ReadVector3();
				if (flag)
				{
					this.rotation = reader.ReadQuaternion();
				}
				else
				{
					this.rotation = Quaternion.identity;
				}
				if (this.isGrounded)
				{
					this.groundNormal = reader.ReadPackedUnitVector3().Unpack();
				}
			}

			// Token: 0x04001D51 RID: 7505
			private static readonly int byteSize = 1;

			// Token: 0x04001D52 RID: 7506
			private static readonly int floatSize = 4;

			// Token: 0x04001D53 RID: 7507
			private static readonly int vector3Size = CharacterNetworkTransformManager.CharacterUpdateMessage.floatSize * 3;

			// Token: 0x04001D54 RID: 7508
			private static readonly int packedUint32MaxSize = 5;

			// Token: 0x04001D55 RID: 7509
			private static readonly int gameObjectSize = CharacterNetworkTransformManager.CharacterUpdateMessage.packedUint32MaxSize;

			// Token: 0x04001D56 RID: 7510
			private static readonly int packedUnitVector3Size = 2;

			// Token: 0x04001D57 RID: 7511
			private static readonly int quaternionSize = CharacterNetworkTransformManager.CharacterUpdateMessage.floatSize * 4;

			// Token: 0x04001D58 RID: 7512
			public static readonly int maxNetworkSize = CharacterNetworkTransformManager.CharacterUpdateMessage.byteSize + CharacterNetworkTransformManager.CharacterUpdateMessage.floatSize + CharacterNetworkTransformManager.CharacterUpdateMessage.gameObjectSize + CharacterNetworkTransformManager.CharacterUpdateMessage.vector3Size + CharacterNetworkTransformManager.CharacterUpdateMessage.packedUnitVector3Size + CharacterNetworkTransformManager.CharacterUpdateMessage.vector3Size + CharacterNetworkTransformManager.CharacterUpdateMessage.quaternionSize + CharacterNetworkTransformManager.CharacterUpdateMessage.packedUnitVector3Size;

			// Token: 0x04001D59 RID: 7513
			private static readonly byte nonIdentityRotationBit = 2;

			// Token: 0x04001D5A RID: 7514
			private static readonly byte isGroundedBit = 4;

			// Token: 0x04001D5B RID: 7515
			public float timestamp;

			// Token: 0x04001D5C RID: 7516
			public GameObject gameObject;

			// Token: 0x04001D5D RID: 7517
			public Vector3 newPosition;

			// Token: 0x04001D5E RID: 7518
			public Vector3 aimDirection;

			// Token: 0x04001D5F RID: 7519
			public Vector3 moveVector;

			// Token: 0x04001D60 RID: 7520
			public Quaternion rotation;

			// Token: 0x04001D61 RID: 7521
			public bool isGrounded;

			// Token: 0x04001D62 RID: 7522
			public Vector3 groundNormal;
		}

		// Token: 0x0200053D RID: 1341
		public struct NetSnapshot
		{
			// Token: 0x04001D63 RID: 7523
			public GameObject gameObject;

			// Token: 0x04001D64 RID: 7524
			public CharacterNetworkTransform.Snapshot snapshot;
		}
	}
}
