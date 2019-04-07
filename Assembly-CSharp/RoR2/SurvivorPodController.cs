using System;
using EntityStates;
using EntityStates.SurvivorPod;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003F1 RID: 1009
	[RequireComponent(typeof(EntityStateMachine))]
	public class SurvivorPodController : NetworkBehaviour, ICameraStateProvider, IInteractable
	{
		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x060015FD RID: 5629 RVA: 0x0006977E File Offset: 0x0006797E
		// (set) Token: 0x060015FE RID: 5630 RVA: 0x00069786 File Offset: 0x00067986
		public EntityStateMachine characterStateMachine { get; private set; }

		// Token: 0x170001FA RID: 506
		// (get) Token: 0x060015FF RID: 5631 RVA: 0x0006978F File Offset: 0x0006798F
		// (set) Token: 0x06001600 RID: 5632 RVA: 0x00069797 File Offset: 0x00067997
		public Transform characterTransform { get; private set; }

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x06001601 RID: 5633 RVA: 0x000697A0 File Offset: 0x000679A0
		// (set) Token: 0x06001602 RID: 5634 RVA: 0x000697A8 File Offset: 0x000679A8
		public InputBankTest characterInputBank { get; private set; }

		// Token: 0x06001603 RID: 5635 RVA: 0x000697B4 File Offset: 0x000679B4
		private void OnSyncBodyObject(GameObject newCharacterBodyObject)
		{
			this.characterStateMachine = (newCharacterBodyObject ? newCharacterBodyObject.GetComponent<EntityStateMachine>() : null);
			this.characterTransform = (newCharacterBodyObject ? newCharacterBodyObject.transform : null);
			if (this.characterStateMachine)
			{
				this.characterStateMachine.SetState(new GenericCharacterPod());
			}
			this.UpdateCameras(newCharacterBodyObject);
			base.enabled = newCharacterBodyObject;
		}

		// Token: 0x06001604 RID: 5636 RVA: 0x00069820 File Offset: 0x00067A20
		private void UpdateCameras(GameObject characterBodyObject)
		{
			foreach (CameraRigController cameraRigController in CameraRigController.readOnlyInstancesList)
			{
				if (characterBodyObject && cameraRigController.target == characterBodyObject)
				{
					cameraRigController.SetOverrideCam(this, 0f);
				}
				else if (cameraRigController.IsOverrideCam(this))
				{
					cameraRigController.SetOverrideCam(null, 0.05f);
				}
			}
		}

		// Token: 0x06001605 RID: 5637 RVA: 0x000698A0 File Offset: 0x00067AA0
		private void Awake()
		{
			this.stateMachine = base.GetComponent<EntityStateMachine>();
		}

		// Token: 0x06001606 RID: 5638 RVA: 0x000698AE File Offset: 0x00067AAE
		private void Start()
		{
			if (!NetworkServer.active)
			{
				this.NetworkcharacterBodyObject = this.characterBodyObject;
			}
		}

		// Token: 0x06001607 RID: 5639 RVA: 0x000698C3 File Offset: 0x00067AC3
		private void Update()
		{
			this.UpdateCameras(this.characterBodyObject);
		}

		// Token: 0x06001608 RID: 5640 RVA: 0x000698D1 File Offset: 0x00067AD1
		private void FixedUpdate()
		{
			this.UpdatePassengerPosition();
		}

		// Token: 0x06001609 RID: 5641 RVA: 0x000698D1 File Offset: 0x00067AD1
		private void LateUpdate()
		{
			this.UpdatePassengerPosition();
		}

		// Token: 0x0600160A RID: 5642 RVA: 0x000698DC File Offset: 0x00067ADC
		private void UpdatePassengerPosition()
		{
			if (this.characterTransform && this.characterStateMachine && this.characterStateMachine.state is GenericCharacterPod)
			{
				this.characterTransform.position = this.holdingPosition.position;
			}
		}

		// Token: 0x0600160B RID: 5643 RVA: 0x0006992C File Offset: 0x00067B2C
		CameraState ICameraStateProvider.GetCameraState(CameraRigController cameraRigController)
		{
			Vector3 position = base.transform.position;
			Vector3 position2 = this.cameraBone.position;
			Vector3 direction = position2 - position;
			Ray ray = new Ray(position, direction);
			Vector3 position3 = position2;
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, direction.magnitude, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
			{
				position3 = ray.GetPoint(Mathf.Max(raycastHit.distance - 0.25f, 0.25f));
			}
			return new CameraState
			{
				position = position3,
				rotation = this.cameraBone.rotation,
				fov = 60f
			};
		}

		// Token: 0x0600160C RID: 5644 RVA: 0x000699D9 File Offset: 0x00067BD9
		Interactability IInteractable.GetInteractability(Interactor interactor)
		{
			if (!(interactor.gameObject == this.characterBodyObject) || !(this.stateMachine.state is Landed))
			{
				return Interactability.Disabled;
			}
			return Interactability.Available;
		}

		// Token: 0x0600160D RID: 5645 RVA: 0x0000A1ED File Offset: 0x000083ED
		public bool ShouldIgnoreSpherecastForInteractibility(Interactor activator)
		{
			return false;
		}

		// Token: 0x0600160E RID: 5646 RVA: 0x00069A03 File Offset: 0x00067C03
		void IInteractable.OnInteractionBegin(Interactor interactor)
		{
			this.stateMachine.SetNextState(new PreRelease());
		}

		// Token: 0x0600160F RID: 5647 RVA: 0x00069A15 File Offset: 0x00067C15
		string IInteractable.GetContextString(Interactor activator)
		{
			return Language.GetString("SURVIVOR_POD_HATCH_OPEN_CONTEXT");
		}

		// Token: 0x06001611 RID: 5649 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x06001612 RID: 5650 RVA: 0x00069A24 File Offset: 0x00067C24
		// (set) Token: 0x06001613 RID: 5651 RVA: 0x00069A38 File Offset: 0x00067C38
		public GameObject NetworkcharacterBodyObject
		{
			get
			{
				return this.characterBodyObject;
			}
			set
			{
				uint dirtyBit = 1u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncBodyObject(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVarGameObject(value, ref this.characterBodyObject, dirtyBit, ref this.___characterBodyObjectNetId);
			}
		}

		// Token: 0x06001614 RID: 5652 RVA: 0x00069A88 File Offset: 0x00067C88
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.characterBodyObject);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.characterBodyObject);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001615 RID: 5653 RVA: 0x00069AF4 File Offset: 0x00067CF4
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.___characterBodyObjectNetId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.OnSyncBodyObject(reader.ReadGameObject());
			}
		}

		// Token: 0x06001616 RID: 5654 RVA: 0x00069B35 File Offset: 0x00067D35
		public override void PreStartClient()
		{
			if (!this.___characterBodyObjectNetId.IsEmpty())
			{
				this.NetworkcharacterBodyObject = ClientScene.FindLocalObject(this.___characterBodyObjectNetId);
			}
		}

		// Token: 0x0400196B RID: 6507
		[SyncVar(hook = "OnSyncBodyObject")]
		[NonSerialized]
		public GameObject characterBodyObject;

		// Token: 0x0400196F RID: 6511
		private EntityStateMachine stateMachine;

		// Token: 0x04001970 RID: 6512
		[Tooltip("The bone which controls the camera during the entry animation.")]
		public Transform cameraBone;

		// Token: 0x04001971 RID: 6513
		[Tooltip("The transform at which the survivor will be held until they exit the pod.")]
		public Transform holdingPosition;

		// Token: 0x04001972 RID: 6514
		[Tooltip("The transform at which the survivor will be placed upon exiting the pod.")]
		public Transform exitPosition;

		// Token: 0x04001973 RID: 6515
		[Tooltip("The animator for the pod.")]
		public Animator animator;

		// Token: 0x04001974 RID: 6516
		private NetworkInstanceId ___characterBodyObjectNetId;
	}
}
