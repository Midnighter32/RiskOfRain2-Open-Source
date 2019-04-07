using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200033B RID: 827
	public class Interactor : NetworkBehaviour
	{
		// Token: 0x060010EC RID: 4332 RVA: 0x000546C4 File Offset: 0x000528C4
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
					if (component != null && component.GetInteractability(this) != Interactability.Disabled)
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
					if (component2 != null && component2.GetInteractability(this) != Interactability.Disabled && !component2.ShouldIgnoreSpherecastForInteractibility(this))
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

		// Token: 0x060010ED RID: 4333 RVA: 0x00054808 File Offset: 0x00052A08
		[Command]
		public void CmdInteract(GameObject interactableObject)
		{
			this.PerformInteraction(interactableObject);
		}

		// Token: 0x060010EE RID: 4334 RVA: 0x00054814 File Offset: 0x00052A14
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

		// Token: 0x060010EF RID: 4335 RVA: 0x00054896 File Offset: 0x00052A96
		[ClientRpc]
		private void RpcInteractionResult(bool anyInteractionSucceeded)
		{
			if (!anyInteractionSucceeded && CameraRigController.IsObjectSpectatedByAnyCamera(base.gameObject))
			{
				Util.PlaySound("Play_UI_insufficient_funds", RoR2Application.instance.gameObject);
			}
		}

		// Token: 0x060010F0 RID: 4336 RVA: 0x000548BD File Offset: 0x00052ABD
		public void AttemptInteraction(GameObject interactableObject)
		{
			if (NetworkServer.active)
			{
				this.PerformInteraction(interactableObject);
				return;
			}
			this.CallCmdInteract(interactableObject);
		}

		// Token: 0x060010F2 RID: 4338 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x060010F3 RID: 4339 RVA: 0x000548E8 File Offset: 0x00052AE8
		protected static void InvokeCmdCmdInteract(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdInteract called on client.");
				return;
			}
			((Interactor)obj).CmdInteract(reader.ReadGameObject());
		}

		// Token: 0x060010F4 RID: 4340 RVA: 0x00054914 File Offset: 0x00052B14
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

		// Token: 0x060010F5 RID: 4341 RVA: 0x0005499E File Offset: 0x00052B9E
		protected static void InvokeRpcRpcInteractionResult(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcInteractionResult called on server.");
				return;
			}
			((Interactor)obj).RpcInteractionResult(reader.ReadBoolean());
		}

		// Token: 0x060010F6 RID: 4342 RVA: 0x000549C8 File Offset: 0x00052BC8
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

		// Token: 0x060010F7 RID: 4343 RVA: 0x00054A3C File Offset: 0x00052C3C
		static Interactor()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(Interactor), Interactor.kCmdCmdInteract, new NetworkBehaviour.CmdDelegate(Interactor.InvokeCmdCmdInteract));
			Interactor.kRpcRpcInteractionResult = 804118976;
			NetworkBehaviour.RegisterRpcDelegate(typeof(Interactor), Interactor.kRpcRpcInteractionResult, new NetworkBehaviour.CmdDelegate(Interactor.InvokeRpcRpcInteractionResult));
			NetworkCRC.RegisterBehaviour("Interactor", 0);
		}

		// Token: 0x060010F8 RID: 4344 RVA: 0x00054AAC File Offset: 0x00052CAC
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x060010F9 RID: 4345 RVA: 0x00004507 File Offset: 0x00002707
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x0400152F RID: 5423
		public float maxInteractionDistance = 1f;

		// Token: 0x04001530 RID: 5424
		private static int kCmdCmdInteract = 591229007;

		// Token: 0x04001531 RID: 5425
		private static int kRpcRpcInteractionResult;
	}
}
