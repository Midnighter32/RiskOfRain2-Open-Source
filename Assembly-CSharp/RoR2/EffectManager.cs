using System;
using System.Collections.Generic;
using RoR2.Audio;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000129 RID: 297
	public static class EffectManager
	{
		// Token: 0x0600055B RID: 1371 RVA: 0x00015BD3 File Offset: 0x00013DD3
		[NetworkMessageHandler(msgType = 52, server = true)]
		private static void HandleEffectServer(NetworkMessage netMsg)
		{
			EffectManager.HandleEffectServerInternal(netMsg);
		}

		// Token: 0x0600055C RID: 1372 RVA: 0x00015BDB File Offset: 0x00013DDB
		[NetworkMessageHandler(msgType = 52, client = true)]
		private static void HandleEffectClient(NetworkMessage netMsg)
		{
			EffectManager.HandleEffectClientInternal(netMsg);
		}

		// Token: 0x0600055D RID: 1373 RVA: 0x00015BE3 File Offset: 0x00013DE3
		public static void SpawnEffect(GameObject effectPrefab, EffectData effectData, bool transmit)
		{
			EffectManager.SpawnEffect(EffectCatalog.FindEffectIndexFromPrefab(effectPrefab), effectData, transmit);
		}

		// Token: 0x0600055E RID: 1374 RVA: 0x00015BF4 File Offset: 0x00013DF4
		public static void SpawnEffect(EffectIndex effectIndex, EffectData effectData, bool transmit)
		{
			if (transmit)
			{
				EffectManager.TransmitEffect(effectIndex, effectData, null);
				if (NetworkServer.active)
				{
					return;
				}
			}
			if (NetworkClient.active)
			{
				if (effectData.networkSoundEventIndex != NetworkSoundEventIndex.Invalid)
				{
					PointSoundManager.EmitSoundLocal(NetworkSoundEventCatalog.GetAkIdFromNetworkSoundEventIndex(effectData.networkSoundEventIndex), effectData.origin);
				}
				EffectDef effectDef = EffectCatalog.GetEffectDef(effectIndex);
				if (effectDef == null)
				{
					return;
				}
				string spawnSoundEventName = effectDef.spawnSoundEventName;
				if (!string.IsNullOrEmpty(spawnSoundEventName))
				{
					PointSoundManager.EmitSoundLocal((AkEventIdArg)spawnSoundEventName, effectData.origin);
				}
				SurfaceDef surfaceDef = SurfaceDefCatalog.GetSurfaceDef(effectData.surfaceDefIndex);
				if (surfaceDef != null)
				{
					string impactSoundString = surfaceDef.impactSoundString;
					if (!string.IsNullOrEmpty(impactSoundString))
					{
						PointSoundManager.EmitSoundLocal((AkEventIdArg)impactSoundString, effectData.origin);
					}
				}
				if (!VFXBudget.CanAffordSpawn(effectDef.prefabVfxAttributes))
				{
					return;
				}
				if (effectDef.cullMethod != null && !effectDef.cullMethod(effectData))
				{
					return;
				}
				EffectData effectData2 = effectData.Clone();
				EffectComponent component = UnityEngine.Object.Instantiate<GameObject>(effectDef.prefab, effectData2.origin, effectData2.rotation).GetComponent<EffectComponent>();
				if (component)
				{
					component.effectData = effectData2.Clone();
				}
			}
		}

		// Token: 0x0600055F RID: 1375 RVA: 0x00015D04 File Offset: 0x00013F04
		private static void TransmitEffect(EffectIndex effectIndex, EffectData effectData, NetworkConnection netOrigin = null)
		{
			EffectManager.outgoingEffectMessage.effectIndex = effectIndex;
			EffectData.Copy(effectData, EffectManager.outgoingEffectMessage.effectData);
			if (NetworkServer.active)
			{
				using (IEnumerator<NetworkConnection> enumerator = NetworkServer.connections.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						NetworkConnection networkConnection = enumerator.Current;
						if (networkConnection != null && networkConnection != netOrigin)
						{
							networkConnection.SendByChannel(52, EffectManager.outgoingEffectMessage, EffectManager.qosChannel.intVal);
						}
					}
					return;
				}
			}
			if (ClientScene.readyConnection != null)
			{
				ClientScene.readyConnection.SendByChannel(52, EffectManager.outgoingEffectMessage, EffectManager.qosChannel.intVal);
			}
		}

		// Token: 0x06000560 RID: 1376 RVA: 0x00015DB0 File Offset: 0x00013FB0
		private static void HandleEffectClientInternal(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<EffectManager.EffectMessage>(EffectManager.incomingEffectMessage);
			if (EffectCatalog.GetEffectDef(EffectManager.incomingEffectMessage.effectIndex) == null)
			{
				return;
			}
			EffectManager.SpawnEffect(EffectManager.incomingEffectMessage.effectIndex, EffectManager.incomingEffectMessage.effectData, false);
		}

		// Token: 0x06000561 RID: 1377 RVA: 0x00015DE9 File Offset: 0x00013FE9
		private static void HandleEffectServerInternal(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<EffectManager.EffectMessage>(EffectManager.incomingEffectMessage);
			EffectManager.TransmitEffect(EffectManager.incomingEffectMessage.effectIndex, EffectManager.incomingEffectMessage.effectData, netMsg.conn);
		}

		// Token: 0x06000562 RID: 1378 RVA: 0x00015E18 File Offset: 0x00014018
		public static void SimpleMuzzleFlash(GameObject effectPrefab, GameObject obj, string muzzleName, bool transmit)
		{
			if (!obj)
			{
				return;
			}
			ModelLocator component = obj.GetComponent<ModelLocator>();
			if (component && component.modelTransform)
			{
				ChildLocator component2 = component.modelTransform.GetComponent<ChildLocator>();
				if (component2)
				{
					int childIndex = component2.FindChildIndex(muzzleName);
					Transform transform = component2.FindChild(childIndex);
					if (transform)
					{
						EffectData effectData = new EffectData
						{
							origin = transform.position
						};
						effectData.SetChildLocatorTransformReference(obj, childIndex);
						EffectManager.SpawnEffect(effectPrefab, effectData, transmit);
					}
				}
			}
		}

		// Token: 0x06000563 RID: 1379 RVA: 0x00015E9B File Offset: 0x0001409B
		public static void SimpleImpactEffect(GameObject effectPrefab, Vector3 hitPos, Vector3 normal, bool transmit)
		{
			EffectManager.SpawnEffect(effectPrefab, new EffectData
			{
				origin = hitPos,
				rotation = ((normal == Vector3.zero) ? Quaternion.identity : Util.QuaternionSafeLookRotation(normal))
			}, transmit);
		}

		// Token: 0x06000564 RID: 1380 RVA: 0x00015ED0 File Offset: 0x000140D0
		public static void SimpleImpactEffect(GameObject effectPrefab, Vector3 hitPos, Vector3 normal, Color color, bool transmit)
		{
			EffectManager.SpawnEffect(effectPrefab, new EffectData
			{
				origin = hitPos,
				rotation = Util.QuaternionSafeLookRotation(normal),
				color = color
			}, transmit);
		}

		// Token: 0x06000565 RID: 1381 RVA: 0x00015EFE File Offset: 0x000140FE
		public static void SimpleEffect(GameObject effectPrefab, Vector3 position, Quaternion rotation, bool transmit)
		{
			EffectManager.SpawnEffect(effectPrefab, new EffectData
			{
				origin = position,
				rotation = rotation
			}, transmit);
		}

		// Token: 0x06000566 RID: 1382 RVA: 0x00015F1A File Offset: 0x0001411A
		public static void SimpleSoundEffect(NetworkSoundEventIndex soundEventIndex, Vector3 position, bool transmit)
		{
			EffectManager.SpawnEffect(null, new EffectData
			{
				origin = position,
				networkSoundEventIndex = soundEventIndex
			}, transmit);
		}

		// Token: 0x0400059E RID: 1438
		private static readonly QosChannelIndex qosChannel = QosChannelIndex.effects;

		// Token: 0x0400059F RID: 1439
		private static readonly EffectManager.EffectMessage outgoingEffectMessage = new EffectManager.EffectMessage();

		// Token: 0x040005A0 RID: 1440
		private static readonly EffectManager.EffectMessage incomingEffectMessage = new EffectManager.EffectMessage();

		// Token: 0x0200012A RID: 298
		private class EffectMessage : MessageBase
		{
			// Token: 0x06000568 RID: 1384 RVA: 0x00015F56 File Offset: 0x00014156
			public override void Serialize(NetworkWriter writer)
			{
				writer.WriteEffectIndex(this.effectIndex);
				writer.Write(this.effectData);
			}

			// Token: 0x06000569 RID: 1385 RVA: 0x00015F70 File Offset: 0x00014170
			public override void Deserialize(NetworkReader reader)
			{
				this.effectIndex = reader.ReadEffectIndex();
				reader.ReadEffectData(this.effectData);
			}

			// Token: 0x040005A1 RID: 1441
			public EffectIndex effectIndex;

			// Token: 0x040005A2 RID: 1442
			public readonly EffectData effectData = new EffectData();
		}
	}
}
