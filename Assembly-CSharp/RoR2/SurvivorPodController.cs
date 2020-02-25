using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000348 RID: 840
	[RequireComponent(typeof(VehicleSeat))]
	[RequireComponent(typeof(EntityStateMachine))]
	public sealed class SurvivorPodController : NetworkBehaviour, ICameraStateProvider
	{
		// Token: 0x1700025F RID: 607
		// (get) Token: 0x060013FB RID: 5115 RVA: 0x0005582C File Offset: 0x00053A2C
		// (set) Token: 0x060013FC RID: 5116 RVA: 0x00055834 File Offset: 0x00053A34
		public EntityStateMachine characterStateMachine { get; private set; }

		// Token: 0x17000260 RID: 608
		// (get) Token: 0x060013FD RID: 5117 RVA: 0x0005583D File Offset: 0x00053A3D
		// (set) Token: 0x060013FE RID: 5118 RVA: 0x00055845 File Offset: 0x00053A45
		public VehicleSeat vehicleSeat { get; private set; }

		// Token: 0x060013FF RID: 5119 RVA: 0x00055850 File Offset: 0x00053A50
		private void Awake()
		{
			this.stateMachine = base.GetComponent<EntityStateMachine>();
			this.vehicleSeat = base.GetComponent<VehicleSeat>();
			this.vehicleSeat.onPassengerEnter += this.OnPassengerEnter;
			this.vehicleSeat.onPassengerExit += this.OnPassengerExit;
			this.vehicleSeat.enterVehicleAllowedCheck.AddCallback(new CallbackCheck<Interactability, CharacterBody>.CallbackDelegate(this.CheckEnterAllowed));
			this.vehicleSeat.exitVehicleAllowedCheck.AddCallback(new CallbackCheck<Interactability, CharacterBody>.CallbackDelegate(this.CheckExitAllowed));
		}

		// Token: 0x06001400 RID: 5120 RVA: 0x000558DB File Offset: 0x00053ADB
		private void OnPassengerEnter(GameObject passenger)
		{
			this.UpdateCameras(passenger);
		}

		// Token: 0x06001401 RID: 5121 RVA: 0x000558E4 File Offset: 0x00053AE4
		private void OnPassengerExit(GameObject passenger)
		{
			this.UpdateCameras(null);
			this.vehicleSeat.enabled = false;
		}

		// Token: 0x06001402 RID: 5122 RVA: 0x000558F9 File Offset: 0x00053AF9
		private void CheckEnterAllowed(CharacterBody characterBody, ref Interactability? resultOverride)
		{
			resultOverride = new Interactability?(Interactability.Disabled);
		}

		// Token: 0x06001403 RID: 5123 RVA: 0x00055907 File Offset: 0x00053B07
		private void CheckExitAllowed(CharacterBody characterBody, ref Interactability? resultOverride)
		{
			resultOverride = new Interactability?(this.exitAllowed ? Interactability.Available : Interactability.Disabled);
		}

		// Token: 0x06001404 RID: 5124 RVA: 0x00055920 File Offset: 0x00053B20
		private void Update()
		{
			this.UpdateCameras(this.vehicleSeat.currentPassengerBody ? this.vehicleSeat.currentPassengerBody.gameObject : null);
		}

		// Token: 0x06001405 RID: 5125 RVA: 0x00055950 File Offset: 0x00053B50
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

		// Token: 0x06001406 RID: 5126 RVA: 0x000559D0 File Offset: 0x00053BD0
		void ICameraStateProvider.GetCameraState(CameraRigController cameraRigController, ref CameraState cameraState)
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
			cameraState = new CameraState
			{
				position = position3,
				rotation = this.cameraBone.rotation,
				fov = 60f
			};
		}

		// Token: 0x06001407 RID: 5127 RVA: 0x0000AC89 File Offset: 0x00008E89
		bool ICameraStateProvider.AllowUserLook(CameraRigController cameraRigController)
		{
			return false;
		}

		// Token: 0x06001409 RID: 5129 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x0600140A RID: 5130 RVA: 0x00055A84 File Offset: 0x00053C84
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x0600140B RID: 5131 RVA: 0x0000409B File Offset: 0x0000229B
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x040012CD RID: 4813
		private EntityStateMachine stateMachine;

		// Token: 0x040012CE RID: 4814
		[Tooltip("The bone which controls the camera during the entry animation.")]
		public Transform cameraBone;

		// Token: 0x040012CF RID: 4815
		[Tooltip("The transform at which the survivor will be held until they exit the pod.")]
		public Transform holdingPosition;

		// Token: 0x040012D0 RID: 4816
		[Tooltip("The transform at which the survivor will be placed upon exiting the pod.")]
		public Transform exitPosition;

		// Token: 0x040012D1 RID: 4817
		[Tooltip("The animator for the pod.")]
		public Animator animator;

		// Token: 0x040012D2 RID: 4818
		public bool exitAllowed;
	}
}
