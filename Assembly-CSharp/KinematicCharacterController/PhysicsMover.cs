using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x0200091B RID: 2331
	[RequireComponent(typeof(Rigidbody))]
	public class PhysicsMover : MonoBehaviour
	{
		// Token: 0x17000495 RID: 1173
		// (get) Token: 0x06003454 RID: 13396 RVA: 0x000E43BA File Offset: 0x000E25BA
		// (set) Token: 0x06003455 RID: 13397 RVA: 0x000E43C2 File Offset: 0x000E25C2
		public int IndexInCharacterSystem { get; set; }

		// Token: 0x17000496 RID: 1174
		// (get) Token: 0x06003456 RID: 13398 RVA: 0x000E43CB File Offset: 0x000E25CB
		// (set) Token: 0x06003457 RID: 13399 RVA: 0x000E43D3 File Offset: 0x000E25D3
		public Vector3 InitialTickPosition { get; set; }

		// Token: 0x17000497 RID: 1175
		// (get) Token: 0x06003458 RID: 13400 RVA: 0x000E43DC File Offset: 0x000E25DC
		// (set) Token: 0x06003459 RID: 13401 RVA: 0x000E43E4 File Offset: 0x000E25E4
		public Quaternion InitialTickRotation { get; set; }

		// Token: 0x17000498 RID: 1176
		// (get) Token: 0x0600345A RID: 13402 RVA: 0x000E43ED File Offset: 0x000E25ED
		// (set) Token: 0x0600345B RID: 13403 RVA: 0x000E43F5 File Offset: 0x000E25F5
		public Transform Transform { get; private set; }

		// Token: 0x17000499 RID: 1177
		// (get) Token: 0x0600345C RID: 13404 RVA: 0x000E43FE File Offset: 0x000E25FE
		// (set) Token: 0x0600345D RID: 13405 RVA: 0x000E4406 File Offset: 0x000E2606
		public Vector3 InitialSimulationPosition { get; private set; }

		// Token: 0x1700049A RID: 1178
		// (get) Token: 0x0600345E RID: 13406 RVA: 0x000E440F File Offset: 0x000E260F
		// (set) Token: 0x0600345F RID: 13407 RVA: 0x000E4417 File Offset: 0x000E2617
		public Quaternion InitialSimulationRotation { get; private set; }

		// Token: 0x1700049B RID: 1179
		// (get) Token: 0x06003460 RID: 13408 RVA: 0x000E4420 File Offset: 0x000E2620
		// (set) Token: 0x06003461 RID: 13409 RVA: 0x000E4428 File Offset: 0x000E2628
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

		// Token: 0x1700049C RID: 1180
		// (get) Token: 0x06003462 RID: 13410 RVA: 0x000E4431 File Offset: 0x000E2631
		// (set) Token: 0x06003463 RID: 13411 RVA: 0x000E4439 File Offset: 0x000E2639
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

		// Token: 0x06003464 RID: 13412 RVA: 0x000E4442 File Offset: 0x000E2642
		private void Reset()
		{
			this.ValidateData();
		}

		// Token: 0x06003465 RID: 13413 RVA: 0x0000409B File Offset: 0x0000229B
		private void OnValidate()
		{
		}

		// Token: 0x06003466 RID: 13414 RVA: 0x000E444C File Offset: 0x000E264C
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

		// Token: 0x06003467 RID: 13415 RVA: 0x000E4501 File Offset: 0x000E2701
		private void OnEnable()
		{
			KinematicCharacterSystem.EnsureCreation();
			KinematicCharacterSystem.RegisterPhysicsMover(this);
		}

		// Token: 0x06003468 RID: 13416 RVA: 0x000E450E File Offset: 0x000E270E
		private void OnDisable()
		{
			KinematicCharacterSystem.UnregisterPhysicsMover(this);
		}

		// Token: 0x06003469 RID: 13417 RVA: 0x000E4518 File Offset: 0x000E2718
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

		// Token: 0x0600346A RID: 13418 RVA: 0x000E4588 File Offset: 0x000E2788
		public void SetPosition(Vector3 position)
		{
			this.Rigidbody.interpolation = RigidbodyInterpolation.None;
			this.Transform.position = position;
			this.Rigidbody.position = position;
			this.InitialSimulationPosition = position;
			this.TransientPosition = position;
			this.Rigidbody.interpolation = ((KinematicCharacterSystem.InterpolationMethod == CharacterSystemInterpolationMethod.Unity) ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None);
		}

		// Token: 0x0600346B RID: 13419 RVA: 0x000E45E0 File Offset: 0x000E27E0
		public void SetRotation(Quaternion rotation)
		{
			this.Rigidbody.interpolation = RigidbodyInterpolation.None;
			this.Transform.rotation = rotation;
			this.Rigidbody.rotation = rotation;
			this.InitialSimulationRotation = rotation;
			this.TransientRotation = rotation;
			this.Rigidbody.interpolation = ((KinematicCharacterSystem.InterpolationMethod == CharacterSystemInterpolationMethod.Unity) ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None);
		}

		// Token: 0x0600346C RID: 13420 RVA: 0x000E4638 File Offset: 0x000E2838
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

		// Token: 0x0600346D RID: 13421 RVA: 0x000E46AC File Offset: 0x000E28AC
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

		// Token: 0x0600346E RID: 13422 RVA: 0x000E4700 File Offset: 0x000E2900
		public void ApplyState(PhysicsMoverState state)
		{
			this.SetPositionAndRotation(state.Position, state.Rotation);
			this.Rigidbody.velocity = state.Velocity;
			this.Rigidbody.angularVelocity = state.AngularVelocity;
		}

		// Token: 0x0600346F RID: 13423 RVA: 0x000E4738 File Offset: 0x000E2938
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

		// Token: 0x040033E6 RID: 13286
		[ReadOnly]
		public Rigidbody Rigidbody;

		// Token: 0x040033E7 RID: 13287
		public BaseMoverController MoverController;

		// Token: 0x040033EE RID: 13294
		private Vector3 _internalTransientPosition;

		// Token: 0x040033EF RID: 13295
		private Quaternion _internalTransientRotation;
	}
}
