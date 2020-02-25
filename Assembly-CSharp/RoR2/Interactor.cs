using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200025F RID: 607
	public class Interactor : NetworkBehaviour
	{
		// Token: 0x06000D41 RID: 3393 RVA: 0x0003B944 File Offset: 0x00039B44
		public GameObject FindBestInteractableObject(Ray raycastRay, float maxRaycastDistance, Vector3 overlapPosition, float overlapRadius)
		{
			LayerMask mask = LayerIndex.defaultLayer.mask | LayerIndex.world.mask | LayerIndex.pickups.mask;
			RaycastHit raycastHit;
			if (Physics.Raycast(raycastRay, out raycastHit, maxRaycastDistance, mask, QueryTriggerInteraction.Collide))
			{
				GameObject entity = EntityLocator.GetEntity(raycastHit.collider.gameObject);
				if (entity)
				{
					IInteractable component = entity.GetComponent<IInteractable>();
					if (component != null && ((MonoBehaviour)component).isActiveAndEnabled && component.GetInteractability(this) != Interactability.Disabled)
					{
						return entity;
					}
				}
			}
			Collider[] array = Physics.OverlapSphere(overlapPosition, overlapRadius, mask, QueryTriggerInteraction.Collide);
			int num = array.Length;
			GameObject result = null;
			float num2 = 0f;
			for (int i = 0; i < num; i++)
			{
				Collider collider = array[i];
				GameObject entity2 = EntityLocator.GetEntity(collider.gameObject);
				if (entity2)
				{
					IInteractable component2 = entity2.GetComponent<IInteractable>();
					if (component2 != null && ((MonoBehaviour)component2).isActiveAndEnabled && component2.GetInteractability(this) != Interactability.Disabled && !component2.ShouldIgnoreSpherecastForInteractibility(this))
					{
						float num3 = Vector3.Dot((collider.transform.position - overlapPosition).normalized, raycastRay.direction);
						if (num3 > num2)
						{
							num2 = num3;
							result = entity2.gameObject;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000D42 RID: 3394 RVA: 0x0003BAA4 File Offset: 0x00039CA4
		[Command]
		public void CmdInteract(GameObject interactableObject)
		{
			this.PerformInteraction(interactableObject);
		}

		// Token: 0x06000D43 RID: 3395 RVA: 0x0003BAB0 File Offset: 0x00039CB0
		[Server]
		private void PerformInteraction(GameObject interactableObject)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Interactor::PerformInteraction(UnityEngine.GameObject)' called on client");
				return;
			}
			if (!interactableObject)
			{
				return;
			}
			bool flag = false;
			bool anyInteractionSucceeded = false;
			foreach (IInteractable interactable in interactableObject.GetComponents<IInteractable>())
			{
				Interactability interactability = interactable.GetInteractability(this);
				if (interactability == Interactability.Available)
				{
					interactable.OnInteractionBegin(this);
					GlobalEventManager.instance.OnInteractionBegin(this, interactable, interactableObject);
					anyInteractionSucceeded = true;
				}
				flag |= (interactability > Interactability.Disabled);
			}
			if (flag)
			{
				this.CallRpcInteractionResult(anyInteractionSucceeded);
			}
		}

		// Token: 0x06000D44 RID: 3396 RVA: 0x0003BB32 File Offset: 0x00039D32
		[ClientRpc]
		private void RpcInteractionResult(bool anyInteractionSucceeded)
		{
			if (!anyInteractionSucceeded && CameraRigController.IsObjectSpectatedByAnyCamera(base.gameObject))
			{
				Util.PlaySound("Play_UI_insufficient_funds", RoR2Application.instance.gameObject);
			}
		}

		// Token: 0x06000D45 RID: 3397 RVA: 0x0003BB59 File Offset: 0x00039D59
		public void AttemptInteraction(GameObject interactableObject)
		{
			if (NetworkServer.active)
			{
				this.PerformInteraction(interactableObject);
				return;
			}
			this.CallCmdInteract(interactableObject);
		}

		// Token: 0x06000D47 RID: 3399 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x06000D48 RID: 3400 RVA: 0x0003BB84 File Offset: 0x00039D84
		protected static void InvokeCmdCmdInteract(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdInteract called on client.");
				return;
			}
			((Interactor)obj).CmdInteract(reader.ReadGameObject());
		}

		// Token: 0x06000D49 RID: 3401 RVA: 0x0003BBB0 File Offset: 0x00039DB0
		public void CallCmdInteract(GameObject interactableObject)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdInteract called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdInteract(interactableObject);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)Interactor.kCmdCmdInteract);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(interactableObject);
			base.SendCommandInternal(networkWriter, 0, "CmdInteract");
		}

		// Token: 0x06000D4A RID: 3402 RVA: 0x0003BC3A File Offset: 0x00039E3A
		protected static void InvokeRpcRpcInteractionResult(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcInteractionResult called on server.");
				return;
			}
			((Interactor)obj).RpcInteractionResult(reader.ReadBoolean());
		}

		// Token: 0x06000D4B RID: 3403 RVA: 0x0003BC64 File Offset: 0x00039E64
		public void CallRpcInteractionResult(bool anyInteractionSucceeded)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcInteractionResult called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)Interactor.kRpcRpcInteractionResult);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(anyInteractionSucceeded);
			this.SendRPCInternal(networkWriter, 0, "RpcInteractionResult");
		}

		// Token: 0x06000D4C RID: 3404 RVA: 0x0003BCD8 File Offset: 0x00039ED8
		static Interactor()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(Interactor), Interactor.kCmdCmdInteract, new NetworkBehaviour.CmdDelegate(Interactor.InvokeCmdCmdInteract));
			Interactor.kRpcRpcInteractionResult = 804118976;
			NetworkBehaviour.RegisterRpcDelegate(typeof(Interactor), Interactor.kRpcRpcInteractionResult, new NetworkBehaviour.CmdDelegate(Interactor.InvokeRpcRpcInteractionResult));
			NetworkCRC.RegisterBehaviour("Interactor", 0);
		}

		// Token: 0x06000D4D RID: 3405 RVA: 0x0003BD48 File Offset: 0x00039F48
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06000D4E RID: 3406 RVA: 0x0000409B File Offset: 0x0000229B
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04000D7E RID: 3454
		public float maxInteractionDistance = 1f;

		// Token: 0x04000D7F RID: 3455
		private static int kCmdCmdInteract = 591229007;

		// Token: 0x04000D80 RID: 3456
		private static int kRpcRpcInteractionResult;
	}
}
