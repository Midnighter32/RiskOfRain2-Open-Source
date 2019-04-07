using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000572 RID: 1394
	public class CharacterNetworkTransformManager : MonoBehaviour
	{
		// Token: 0x06001F13 RID: 7955 RVA: 0x000928EE File Offset: 0x00090AEE
		private void Awake()
		{
			CharacterNetworkTransformManager.instance = this;
		}

		// Token: 0x06001F14 RID: 7956 RVA: 0x000928F6 File Offset: 0x00090AF6
		[NetworkMessageHandler(msgType = 51, client = true, server = true)]
		private static void HandleTransformUpdates(NetworkMessage netMsg)
		{
			if (CharacterNetworkTransformManager.instance)
			{
				CharacterNetworkTransformManager.instance.HandleTransformUpdatesInternal(netMsg);
			}
		}

		// Token: 0x06001F15 RID: 7957 RVA: 0x00092910 File Offset: 0x00090B10
		private void HandleTransformUpdatesInternal(NetworkMessage netMsg)
		{
			uint num = (uint)netMsg.reader.ReadByte();
			float filteredClientRTT = GameNetworkManager.singleton.filteredClientRTT;
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
							isGrounded = this.currentInMessage.isGrounded
						};
						if (NetworkClient.active)
						{
							snapshot.serverTime += filteredClientRTT;
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

		// Token: 0x06001F16 RID: 7958 RVA: 0x00092A70 File Offset: 0x00090C70
		private void ProcessQueue()
		{
			if (this.snapshotQueue.Count == 0)
			{
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.StartMessage(51);
			int num = Mathf.Min(Mathf.FloorToInt((float)(1000 - networkWriter.Position) / 61f), this.snapshotQueue.Count);
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

		// Token: 0x06001F17 RID: 7959 RVA: 0x00092BC8 File Offset: 0x00090DC8
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

		// Token: 0x040021C4 RID: 8644
		private static CharacterNetworkTransformManager instance;

		// Token: 0x040021C5 RID: 8645
		private CharacterNetworkTransformManager.CharacterUpdateMessage currentInMessage = new CharacterNetworkTransformManager.CharacterUpdateMessage();

		// Token: 0x040021C6 RID: 8646
		private CharacterNetworkTransformManager.CharacterUpdateMessage currentOutMessage = new CharacterNetworkTransformManager.CharacterUpdateMessage();

		// Token: 0x040021C7 RID: 8647
		private readonly Queue<CharacterNetworkTransformManager.NetSnapshot> snapshotQueue = new Queue<CharacterNetworkTransformManager.NetSnapshot>();

		// Token: 0x02000573 RID: 1395
		private class CharacterUpdateMessage : MessageBase
		{
			// Token: 0x06001F1A RID: 7962 RVA: 0x00092C94 File Offset: 0x00090E94
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.gameObject);
				writer.Write(this.newPosition);
				writer.Write(this.aimDirection);
				writer.Write(this.moveVector);
				writer.Write(this.rotation);
				writer.Write(this.timestamp);
				writer.Write(this.isGrounded);
			}

			// Token: 0x06001F1B RID: 7963 RVA: 0x00092CF8 File Offset: 0x00090EF8
			public override void Deserialize(NetworkReader reader)
			{
				this.gameObject = reader.ReadGameObject();
				this.newPosition = reader.ReadVector3();
				this.aimDirection = reader.ReadVector3();
				this.moveVector = reader.ReadVector3();
				this.rotation = reader.ReadQuaternion();
				this.timestamp = reader.ReadSingle();
				this.isGrounded = reader.ReadBoolean();
			}

			// Token: 0x040021C8 RID: 8648
			public GameObject gameObject;

			// Token: 0x040021C9 RID: 8649
			public Vector3 newPosition;

			// Token: 0x040021CA RID: 8650
			public Vector3 aimDirection;

			// Token: 0x040021CB RID: 8651
			public Vector3 moveVector;

			// Token: 0x040021CC RID: 8652
			public Quaternion rotation;

			// Token: 0x040021CD RID: 8653
			public float timestamp;

			// Token: 0x040021CE RID: 8654
			public bool isGrounded;

			// Token: 0x040021CF RID: 8655
			public const int maxNetworkSize = 61;
		}

		// Token: 0x02000574 RID: 1396
		public struct NetSnapshot
		{
			// Token: 0x040021D0 RID: 8656
			public GameObject gameObject;

			// Token: 0x040021D1 RID: 8657
			public CharacterNetworkTransform.Snapshot snapshot;

			// Token: 0x040021D2 RID: 8658
			public const int maxNetworkSize = 61;
		}
	}
}
