using System;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000459 RID: 1113
	public static class TeleportHelper
	{
		// Token: 0x06001AF5 RID: 6901 RVA: 0x0007262C File Offset: 0x0007082C
		public static void TeleportGameObject(GameObject gameObject, Vector3 newPosition)
		{
			bool hasEffectiveAuthority = Util.HasEffectiveAuthority(gameObject);
			TeleportHelper.TeleportGameObject(gameObject, newPosition, newPosition - gameObject.transform.position, hasEffectiveAuthority);
		}

		// Token: 0x06001AF6 RID: 6902 RVA: 0x0007265C File Offset: 0x0007085C
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

		// Token: 0x06001AF7 RID: 6903 RVA: 0x000726D0 File Offset: 0x000708D0
		private static void OnTeleport(GameObject gameObject, Vector3 newPosition, Vector3 delta)
		{
			CharacterMotor component = gameObject.GetComponent<CharacterMotor>();
			if (component)
			{
				component.Motor.SetPosition(newPosition, true);
				component.Motor.BaseVelocity = Vector3.zero;
				component.velocity = Vector3.zero;
				component.rootMotion = Vector3.zero;
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

		// Token: 0x06001AF8 RID: 6904 RVA: 0x00072754 File Offset: 0x00070954
		public static void TeleportBody(CharacterBody body, Vector3 targetFootPosition)
		{
			Vector3 b = body.footPosition - body.transform.position;
			TeleportHelper.TeleportGameObject(body.gameObject, targetFootPosition - b);
		}

		// Token: 0x06001AF9 RID: 6905 RVA: 0x0007278C File Offset: 0x0007098C
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

		// Token: 0x0400187D RID: 6269
		private static readonly TeleportHelper.TeleportMessage messageBuffer = new TeleportHelper.TeleportMessage();

		// Token: 0x0200045A RID: 1114
		private class TeleportMessage : MessageBase
		{
			// Token: 0x06001AFC RID: 6908 RVA: 0x00072808 File Offset: 0x00070A08
			public override void Serialize(NetworkWriter writer)
			{
				writer.Write(this.gameObject);
				writer.Write(this.newPosition);
				writer.Write(this.delta);
			}

			// Token: 0x06001AFD RID: 6909 RVA: 0x0007282E File Offset: 0x00070A2E
			public override void Deserialize(NetworkReader reader)
			{
				this.gameObject = reader.ReadGameObject();
				this.newPosition = reader.ReadVector3();
				this.delta = reader.ReadVector3();
			}

			// Token: 0x0400187E RID: 6270
			public GameObject gameObject;

			// Token: 0x0400187F RID: 6271
			public Vector3 newPosition;

			// Token: 0x04001880 RID: 6272
			public Vector3 delta;
		}
	}
}
