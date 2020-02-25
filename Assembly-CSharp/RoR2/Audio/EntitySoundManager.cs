using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Audio
{
	// Token: 0x0200067D RID: 1661
	public static class EntitySoundManager
	{
		// Token: 0x060026F4 RID: 9972 RVA: 0x000A9A10 File Offset: 0x000A7C10
		[NetworkMessageHandler(client = true, server = false, msgType = 73)]
		private static void HandleMessage(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<EntitySoundManager.EntitySoundMessage>(EntitySoundManager.sharedMessage);
			if (EntitySoundManager.sharedMessage.networkIdentity)
			{
				EntitySoundManager.EmitSoundLocal(NetworkSoundEventCatalog.GetAkIdFromNetworkSoundEventIndex(EntitySoundManager.sharedMessage.networkSoundEventIndex), EntitySoundManager.sharedMessage.networkIdentity.gameObject);
			}
			EntitySoundManager.sharedMessage.Clear();
		}

		// Token: 0x060026F5 RID: 9973 RVA: 0x000A9A6C File Offset: 0x000A7C6C
		public static uint EmitSoundLocal(AkEventIdArg akEventId, GameObject gameObject)
		{
			if (akEventId == 0U)
			{
				return 0U;
			}
			return AkSoundEngine.PostEvent(akEventId, gameObject);
		}

		// Token: 0x060026F6 RID: 9974 RVA: 0x000A9A84 File Offset: 0x000A7C84
		public static void EmitSoundServer(AkEventIdArg akEventId, GameObject gameObject)
		{
			NetworkSoundEventIndex networkSoundEventIndex = NetworkSoundEventCatalog.FindNetworkSoundEventIndex(akEventId);
			if (networkSoundEventIndex == NetworkSoundEventIndex.Invalid)
			{
				Debug.LogWarningFormat("Cannot emit sound \"{0}\" on object \"{1}\": Event is not registered in NetworkSoundEventCatalog.", new object[]
				{
					akEventId.id,
					gameObject
				});
				return;
			}
			EntitySoundManager.EmitSoundServer(networkSoundEventIndex, gameObject);
		}

		// Token: 0x060026F7 RID: 9975 RVA: 0x000A9ACC File Offset: 0x000A7CCC
		public static void EmitSoundServer(NetworkSoundEventIndex networkSoundEventIndex, GameObject gameObject)
		{
			NetworkIdentity component = gameObject.GetComponent<NetworkIdentity>();
			if (!component)
			{
				Debug.LogWarningFormat("Cannot emit sound \"{0}\" on object \"{1}\": Object has no NetworkIdentity.", new object[]
				{
					NetworkSoundEventCatalog.GetAkIdFromNetworkSoundEventIndex(networkSoundEventIndex),
					gameObject
				});
				return;
			}
			EntitySoundManager.EmitSoundServer(networkSoundEventIndex, component);
		}

		// Token: 0x060026F8 RID: 9976 RVA: 0x000A9B12 File Offset: 0x000A7D12
		public static void EmitSoundServer(NetworkSoundEventIndex networkSoundEventIndex, NetworkIdentity networkIdentity)
		{
			EntitySoundManager.sharedMessage.networkIdentity = networkIdentity;
			EntitySoundManager.sharedMessage.networkSoundEventIndex = networkSoundEventIndex;
			NetworkServer.SendByChannelToAll(EntitySoundManager.messageType, EntitySoundManager.sharedMessage, EntitySoundManager.channel.intVal);
			EntitySoundManager.sharedMessage.Clear();
		}

		// Token: 0x040024BE RID: 9406
		private static readonly EntitySoundManager.EntitySoundMessage sharedMessage = new EntitySoundManager.EntitySoundMessage();

		// Token: 0x040024BF RID: 9407
		private static readonly QosChannelIndex channel = QosChannelIndex.defaultReliable;

		// Token: 0x040024C0 RID: 9408
		private static readonly short messageType = 73;

		// Token: 0x0200067E RID: 1662
		private class EntitySoundMessage : MessageBase
		{
			// Token: 0x060026FA RID: 9978 RVA: 0x000A9B6B File Offset: 0x000A7D6B
			public override void Serialize(NetworkWriter writer)
			{
				base.Serialize(writer);
				writer.WriteNetworkSoundEventIndex(this.networkSoundEventIndex);
				writer.Write(this.networkIdentity);
			}

			// Token: 0x060026FB RID: 9979 RVA: 0x000A9B8C File Offset: 0x000A7D8C
			public override void Deserialize(NetworkReader reader)
			{
				base.Deserialize(reader);
				this.networkSoundEventIndex = reader.ReadNetworkSoundEventIndex();
				this.networkIdentity = reader.ReadNetworkIdentity();
			}

			// Token: 0x060026FC RID: 9980 RVA: 0x000A9BAD File Offset: 0x000A7DAD
			public void Clear()
			{
				this.networkSoundEventIndex = NetworkSoundEventIndex.Invalid;
				this.networkIdentity = null;
			}

			// Token: 0x040024C1 RID: 9409
			public NetworkSoundEventIndex networkSoundEventIndex;

			// Token: 0x040024C2 RID: 9410
			public NetworkIdentity networkIdentity;
		}
	}
}
