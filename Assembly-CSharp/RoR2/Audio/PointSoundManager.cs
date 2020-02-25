using System;
using System.Collections.Generic;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace RoR2.Audio
{
	// Token: 0x02000687 RID: 1671
	public static class PointSoundManager
	{
		// Token: 0x0600271F RID: 10015 RVA: 0x000AA06A File Offset: 0x000A826A
		private static AkGameObj RequestEmitter()
		{
			if (PointSoundManager.emitterPool.Count > 0)
			{
				return PointSoundManager.emitterPool.Pop();
			}
			GameObject gameObject = new GameObject("SoundEmitter");
			gameObject.AddComponent<Rigidbody>().isKinematic = true;
			return gameObject.AddComponent<AkGameObj>();
		}

		// Token: 0x06002720 RID: 10016 RVA: 0x000AA09F File Offset: 0x000A829F
		private static void FreeEmitter(AkGameObj emitter)
		{
			if (emitter)
			{
				PointSoundManager.emitterPool.Push(emitter);
			}
		}

		// Token: 0x06002721 RID: 10017 RVA: 0x000AA0B4 File Offset: 0x000A82B4
		public static uint EmitSoundLocal(AkEventIdArg akEventId, Vector3 position)
		{
			if (RoR2Application.noAudio || akEventId == 0U)
			{
				return 0U;
			}
			AkGameObj akGameObj = PointSoundManager.RequestEmitter();
			akGameObj.transform.position = position;
			return AkSoundEngine.PostEvent(akEventId, akGameObj.gameObject, 1U, new AkCallbackManager.EventCallback(PointSoundManager.Callback), akGameObj);
		}

		// Token: 0x06002722 RID: 10018 RVA: 0x000AA103 File Offset: 0x000A8303
		private static void Callback(object cookie, AkCallbackType in_type, AkCallbackInfo in_info)
		{
			if (in_type == AkCallbackType.AK_EndOfEvent)
			{
				PointSoundManager.FreeEmitter((AkGameObj)cookie);
			}
		}

		// Token: 0x06002723 RID: 10019 RVA: 0x000AA114 File Offset: 0x000A8314
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			SceneManager.sceneUnloaded += PointSoundManager.OnSceneUnloaded;
		}

		// Token: 0x06002724 RID: 10020 RVA: 0x000AA127 File Offset: 0x000A8327
		private static void OnSceneUnloaded(Scene scene)
		{
			PointSoundManager.ClearEmitterPool();
		}

		// Token: 0x06002725 RID: 10021 RVA: 0x000AA130 File Offset: 0x000A8330
		private static void ClearEmitterPool()
		{
			foreach (AkGameObj akGameObj in PointSoundManager.emitterPool)
			{
				if (akGameObj)
				{
					UnityEngine.Object.Destroy(akGameObj.gameObject);
				}
			}
			PointSoundManager.emitterPool.Clear();
		}

		// Token: 0x06002726 RID: 10022 RVA: 0x000AA198 File Offset: 0x000A8398
		public static void EmitSoundServer(NetworkSoundEventIndex networkSoundEventIndex, Vector3 position)
		{
			PointSoundManager.sharedMessage.soundEventIndex = networkSoundEventIndex;
			PointSoundManager.sharedMessage.position = position;
			NetworkServer.SendByChannelToAll(72, PointSoundManager.sharedMessage, PointSoundManager.channel.intVal);
		}

		// Token: 0x06002727 RID: 10023 RVA: 0x000AA1C7 File Offset: 0x000A83C7
		[NetworkMessageHandler(client = true, server = false, msgType = 72)]
		private static void HandleMessage(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<PointSoundManager.NetworkSoundEventMessage>(PointSoundManager.sharedMessage);
			PointSoundManager.EmitSoundLocal(NetworkSoundEventCatalog.GetAkIdFromNetworkSoundEventIndex(PointSoundManager.sharedMessage.soundEventIndex), PointSoundManager.sharedMessage.position);
		}

		// Token: 0x040024D7 RID: 9431
		private static readonly Stack<AkGameObj> emitterPool = new Stack<AkGameObj>();

		// Token: 0x040024D8 RID: 9432
		private static readonly PointSoundManager.NetworkSoundEventMessage sharedMessage = new PointSoundManager.NetworkSoundEventMessage();

		// Token: 0x040024D9 RID: 9433
		private static readonly QosChannelIndex channel = QosChannelIndex.effects;

		// Token: 0x040024DA RID: 9434
		private static readonly short messageType = 72;

		// Token: 0x02000688 RID: 1672
		private class NetworkSoundEventMessage : MessageBase
		{
			// Token: 0x06002729 RID: 10025 RVA: 0x000AA21F File Offset: 0x000A841F
			public override void Serialize(NetworkWriter writer)
			{
				base.Serialize(writer);
				writer.WriteNetworkSoundEventIndex(this.soundEventIndex);
				writer.Write(this.position);
			}

			// Token: 0x0600272A RID: 10026 RVA: 0x000AA240 File Offset: 0x000A8440
			public override void Deserialize(NetworkReader reader)
			{
				base.Deserialize(reader);
				this.soundEventIndex = reader.ReadNetworkSoundEventIndex();
				this.position = reader.ReadVector3();
			}

			// Token: 0x040024DB RID: 9435
			public NetworkSoundEventIndex soundEventIndex;

			// Token: 0x040024DC RID: 9436
			public Vector3 position;
		}
	}
}
