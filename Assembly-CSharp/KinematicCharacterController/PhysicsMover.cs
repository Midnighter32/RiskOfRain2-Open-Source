using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x020006D0 RID: 1744
	[RequireComponent(typeof(Rigidbody))]
	public class PhysicsMover : MonoBehaviour
	{
		// Token: 0x17000358 RID: 856
		// (get) Token: 0x0600270A RID: 9994 RVA: 0x000B4F72 File Offset: 0x000B3172
		// (set) Token: 0x0600270B RID: 9995 RVA: 0x000B4F7A File Offset: 0x000B317A
		public int IndexInCharacterSystem { get; set; }

		// Token: 0x17000359 RID: 857
		// (get) Token: 0x0600270C RID: 9996 RVA: 0x000B4F83 File Offset: 0x000B3183
		// (set) Token: 0x0600270D RID: 9997 RVA: 0x000B4F8B File Offset: 0x000B318B
		public Vector3 InitialTickPosition { get; set; }

		// Token: 0x1700035A RID: 858
		// (get) Token: 0x0600270E RID: 9998 RVA: 0x000B4F94 File Offset: 0x000B3194
		// (set) Token: 0x0600270F RID: 9999 RVA: 0x000B4F9C File Offset: 0x000B319C
		public Quaternion InitialTickRotation { get; set; }

		// Token: 0x1700035B RID: 859
		// (get) Token: 0x06002710 RID: 10000 RVA: 0x000B4FA5 File Offset: 0x000B31A5
		// (set) Token: 0x06002711 RID: 10001 RVA: 0x000B4FAD File Offset: 0x000B31AD
		public Transform Transform { get; private set; }

		// Token: 0x1700035C RID: 860
		// (get) Token: 0x06002712 RID: 10002 RVA: 0x000B4FB6 File Offset: 0x000B31B6
		// (set) Token: 0x06002713 RID: 10003 RVA: 0x000B4FBE File Offset: 0x000B31BE
		public Vector3 InitialSimulationPosition { get; private set; }

		// Token: 0x1700035D RID: 861
		// (get) Token: 0x06002714 RID: 10004 RVA: 0x000B4FC7 File Offset: 0x000B31C7
		// (set) Token: 0x06002715 RID: 10005 RVA: 0x000B4FCF File Offset: 0x000B31CF
		public Quaternion InitialSimulationRotation { get; private set; }

		// Token: 0x1700035E RID: 862
		// (get) Token: 0x06002716 RID: 10006 RVA: 0x000B4FD8 File Offset: 0x000B31D8
		// (set) Token: 0x06002717 RID: 10007 RVA: 0x000B4FE0 File Offset: 0x000B31E0
		public Vector3 TransientPosition
		{
			get
			{
				return this._internalTransientPosition;
			}
			private set
			{
				this._internalTransientPosition = value;
			}
		}

		// Token: 0x1700035F RID: 863
		// (get) Token: 0x06002718 RID: 10008 RVA: 0x000B4FE9 File Offset: 0x000B31E9
		// (set) Token: 0x06002719 RID: 10009 RVA: 0x000B4FF1 File Offset: 0x000B31F1
		public Quaternion TransientRotation
		{
			get
			{
				return this._internalTransientRotation;
			}
			private set
			{
				this._internalTransientRotation = value;
			}
		}

		// Token: 0x0600271A RID: 10010 RVA: 0x000B4FFA File Offset: 0x000B31FA
		private void Reset()
		{
			this.ValidateData();
		}

		// Token: 0x0600271B RID: 10011 RVA: 0x00004507 File Offset: 0x00002707
		private void OnValidate()
		{
		}

		// Token: 0x0600271C RID: 10012 RVA: 0x000B5004 File Offset: 0x000B3204
		public void ValidateData()
		{
			this.Rigidbody = base.gameObject.GetComponent<Rigidbody>();
			this.Rigidbody.centerOfMass = Vector3.zero;
			this.Rigidbody.useGravity = false;
			this.Rigidbody.drag = 0f;
			this.Rigidbody.angularDrag = 0f;
			this.Rigidbody.maxAngularVelocity = float.PositiveInfinity;
			this.Rigidbody.maxDepenetrationVelocity = float.PositiveInfinity;
			this.Rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
			this.Rigidbody.isKinematic = true;
			this.Rigidbody.constraints = RigidbodyConstraints.None;
			this.Rigidbody.interpolation = ((KinematicCharacterSystem.InterpolationMethod == CharacterSystemInterpolationMethod.Unity) ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None);
		}

		// Token: 0x0600271D RID: 10013 RVA: 0x000B50B9 File Offset: 0x000B32B9
		private void OnEnable()
		{
			KinematicCharacterSystem.EnsureCreation();
			KinematicCharacterSystem.RegisterPhysicsMover(this);
		}

		// Token: 0x0600271E RID: 10014 RVA: 0x000B50C6 File Offset: 0x000B32C6
		private void OnDisable()
		{
			KinematicCharacterSystem.UnregisterPhysicsMover(this);
		}

		// Token: 0x0600271F RID: 10015 RVA: 0x000B50D0 File Offset: 0x000B32D0
		private void Awake()
		{
			this.Transform = base.transform;
			this.ValidateData();
			this.MoverController.SetupMover(this);
			this.TransientPosition = this.Rigidbody.position;
			this.TransientRotation = this.Rigidbody.rotation;
			this.InitialSimulationPosition = this.Rigidbody.position;
			this.InitialSimulationRotation = this.Rigidbody.rotation;
		}

		// Token: 0x06002720 RID: 10016 RVA: 0x000B5140 File Offset: 0x000B3340
		public void SetPosition(Vector3 position)
		{
			this.Rigidbody.interpolation = RigidbodyInterpolation.None;
			this.Transform.position = position;
			this.Rigidbody.position = position;
			this.InitialSimulationPosition = position;
			this.TransientPosition = position;
			this.Rigidbody.interpolation = ((KinematicCharacterSystem.InterpolationMethod == CharacterSystemInterpolationMethod.Unity) ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None);
		}

		// Token: 0x06002721 RID: 10017 RVA: 0x000B5198 File Offset: 0x000B3398
		public void SetRotation(Quaternion rotation)
		{
			this.Rigidbody.interpolation = RigidbodyInterpolation.None;
			this.Transform.rotation = rotation;
			this.Rigidbody.rotation = rotation;
			this.InitialSimulationRotation = rotation;
			this.TransientRotation = rotation;
			this.Rigidbody.interpolation = ((KinematicCharacterSystem.InterpolationMethod == CharacterSystemInterpolationMethod.Unity) ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None);
		}

		// Token: 0x06002722 RID: 10018 RVA: 0x000B51F0 File Offset: 0x000B33F0
		public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
		{
			this.Rigidbody.interpolation = RigidbodyInterpolation.None;
			this.Transform.SetPositionAndRotation(position, rotation);
			this.Rigidbody.position = position;
			this.Rigidbody.rotation = rotation;
			this.InitialSimulationPosition = position;
			this.InitialSimulationRotation = rotation;
			this.TransientPosition = position;
			this.TransientRotation = rotation;
			this.Rigidbody.interpolation = ((KinematicCharacterSystem.InterpolationMethod == CharacterSystemInterpolationMethod.Unity) ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None);
		}

		// Token: 0x06002723 RID: 10019 RVA: 0x000B5264 File Offset: 0x000B3464
		public PhysicsMoverState GetState()
		{
			return new PhysicsMoverState
			{
				Position = this.TransientPosition,
				Rotation = this.TransientRotation,
				Velocity = this.Rigidbody.velocity,
				AngularVelocity = this.Rigidbody.velocity
			};
		}

		// Token: 0x06002724 RID: 10020 RVA: 0x000B52B8 File Offset: 0x000B34B8
		public void ApplyState(PhysicsMoverState state)
		{
			this.SetPositionAndRotation(state.Position, state.Rotation);
			this.Rigidbody.velocity = state.Velocity;
			this.Rigidbody.angularVelocity = state.AngularVelocity;
		}

		// Token: 0x06002725 RID: 10021 RVA: 0x000B52F0 File Offset: 0x000B34F0
		public void VelocityUpdate(float deltaTime)
		{
			this.InitialSimulationPosition = this.TransientPosition;
			this.InitialSimulationRotation = this.TransientRotation;
			this.MoverController.UpdateMovement(out this._internalTransientPosition, out this._internalTransientRotation, deltaTime);
			if (deltaTime > 0f)
			{
				this.Rigidbody.velocity = (this.TransientPosition - this.InitialSimulationPosition) / deltaTime;
				Quaternion quaternion = this.TransientRotation * Quaternion.Inverse(this.InitialSimulationRotation);
				this.Rigidbody.angularVelocity = 0.017453292f * quaternion.eulerAngles / deltaTime;
			}
		}

		// Token: 0x0400294D RID: 10573
		[ReadOnly]
		public Rigidbody Rigidbody;

		// Token: 0x0400294E RID: 10574
		public BaseMoverController MoverController;

		// Token: 0x04002955 RID: 10581
		private Vector3 _internalTransientPosition;

		// Token: 0x04002956 RID: 10582
		private Quaternion _internalTransientRotation;
	}
}
