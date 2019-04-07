using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020004C1 RID: 1217
	public static class TeleportHelper
	{
		// Token: 0x06001B5E RID: 7006 RVA: 0x0007FF48 File Offset: 0x0007E148
		public static void TeleportGameObject(GameObject gameObject, Vector3 newPosition)
		{
			bool hasEffectiveAuthority = Util.HasEffectiveAuthority(gameObject);
			TeleportHelper.TeleportGameObject(gameObject, newPosition, newPosition - gameObject.transform.position, hasEffectiveAuthority);
		}

		// Token: 0x06001B5F RID: 7007 RVA: 0x0007FF78 File Offset: 0x0007E178
		private static void TeleportGameObject(GameObject gameObject, Vector3 newPosition, Vector3 delta, bool hasEffectiveAuthority)
		{
			TeleportHelper.OnTeleport(gameObject, newPosition, delta);
			if (NetworkServer.active || hasEffectiveAuthority)
			{
				TeleportHelper.TeleportMessage msg = new TeleportHelper.TeleportMessage
				{
					gameObject = gameObject,
					newPosition = newPosition,
					delta = delta
				};
				QosChannelIndex defaultReliable = QosChannelIndex.defaultReliable;
				if (NetworkServer.active)
				{
					NetworkServer.SendByChannelToAll(68, msg, defaultReliable.intVal);
					return;
				}
				NetworkManager.singleton.client.connection.SendByChannel(68, msg, defaultReliable.intVal);
			}
		}

		// Token: 0x06001B60 RID: 7008 RVA: 0x0007FFEC File Offset: 0x0007E1EC
		private static void OnTeleport(GameObject gameObject, Vector3 newPosition, Vector3 delta)
		{
			CharacterMotor component = gameObject.GetComponent<CharacterMotor>();
			if (component)
			{
				component.Motor.SetPositionAndRotation(newPosition, Quaternion.identity, true);
				component.velocity = Vector3.zero;
			}
			else
			{
				gameObject.transform.position = newPosition;
			}
			ITeleportHandler[] componentsInChildren = gameObject.GetComponentsInChildren<ITeleportHandler>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].OnTeleport(newPosition - delta, newPosition);
			}
		}

		// Token: 0x06001B61 RID: 7009 RVA: 0x00080058 File Offset: 0x0007E258
		public static void TeleportBody(CharacterBody body, Vector3 targetFootPosition)
		{
			Vector3 b = body.footPosition - body.transform.position;
			TeleportHelper.TeleportGameObject(body.gameObject, targetFootPosition - b);
		}

		// Token: 0x06001B62 RID: 7010 RVA: 0x00080090 File Offset: 0x0007E290
		[NetworkMessageHandler(client = true, server = true, msgType = 68)]
		private static void HandleTeleport(NetworkMessage netMsg)
		{
			if (Util.ConnectionIsLocal(netMsg.conn))
			{
				return;
			}
			netMsg.ReadMessage<TeleportHelper.TeleportMessage>(TeleportHelper.messageBuffer);
			if (!TeleportHelper.messageBuffer.gameObject)
			{
				return;
			}
			bool flag = Util.HasEffectiveAuthority(TeleportHelper.messageBuffer.gameObject);
			if (flag)
			{
				return;
			}
			TeleportHelper.TeleportGameObject(TeleportHelper.messageBuffer.gameObject, TeleportHelper.messageBuffer.newPosition, TeleportHelper.messageBuffer.delta, flag);
		}

		// Token: 0x04001DFF RID: 7679
		private static readonly TeleportHelper.TeleportMessage messageBuffer = new TeleportHelper.TeleportMessage();

		// Token: 0x020004C2 RID: 1218
		private class TeleportMessage : MessageBase
		{
			// Token: 0x06001B65 RID: 7013 RVA: 0x0008010C File Offset: 0x0007E30C
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.gameObject);
				writer.Write(this.newPosition);
				writer.Write(this.delta);
			}

			// Token: 0x06001B66 RID: 7014 RVA: 0x00080132 File Offset: 0x0007E332
			public override void Deserialize(NetworkReader reader)
			{
				this.gameObject = reader.ReadGameObject();
				this.newPosition = reader.ReadVector3();
				this.delta = reader.ReadVector3();
			}

			// Token: 0x04001E00 RID: 7680
			public GameObject gameObject;

			// Token: 0x04001E01 RID: 7681
			public Vector3 newPosition;

			// Token: 0x04001E02 RID: 7682
			public Vector3 delta;
		}
	}
}
