using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x02000917 RID: 2327
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Rigidbody))]
	public class KinematicCharacterMotor : MonoBehaviour
	{
		// Token: 0x17000479 RID: 1145
		// (get) Token: 0x060033DD RID: 13277 RVA: 0x000E0DF1 File Offset: 0x000DEFF1
		// (set) Token: 0x060033DE RID: 13278 RVA: 0x000E0DF9 File Offset: 0x000DEFF9
		public Transform Transform { get; private set; }

		// Token: 0x1700047A RID: 1146
		// (get) Token: 0x060033DF RID: 13279 RVA: 0x000E0E02 File Offset: 0x000DF002
		// (set) Token: 0x060033E0 RID: 13280 RVA: 0x000E0E0A File Offset: 0x000DF00A
		public Vector3 CharacterUp { get; private set; }

		// Token: 0x1700047B RID: 1147
		// (get) Token: 0x060033E1 RID: 13281 RVA: 0x000E0E13 File Offset: 0x000DF013
		// (set) Token: 0x060033E2 RID: 13282 RVA: 0x000E0E1B File Offset: 0x000DF01B
		public Vector3 CharacterForward { get; private set; }

		// Token: 0x1700047C RID: 1148
		// (get) Token: 0x060033E3 RID: 13283 RVA: 0x000E0E24 File Offset: 0x000DF024
		// (set) Token: 0x060033E4 RID: 13284 RVA: 0x000E0E2C File Offset: 0x000DF02C
		public Vector3 CharacterRight { get; private set; }

		// Token: 0x1700047D RID: 1149
		// (get) Token: 0x060033E5 RID: 13285 RVA: 0x000E0E35 File Offset: 0x000DF035
		// (set) Token: 0x060033E6 RID: 13286 RVA: 0x000E0E3D File Offset: 0x000DF03D
		public Vector3 InitialSimulationPosition { get; private set; }

		// Token: 0x1700047E RID: 1150
		// (get) Token: 0x060033E7 RID: 13287 RVA: 0x000E0E46 File Offset: 0x000DF046
		// (set) Token: 0x060033E8 RID: 13288 RVA: 0x000E0E4E File Offset: 0x000DF04E
		public Quaternion InitialSimulationRotation { get; private set; }

		// Token: 0x1700047F RID: 1151
		// (get) Token: 0x060033E9 RID: 13289 RVA: 0x000E0E57 File Offset: 0x000DF057
		// (set) Token: 0x060033EA RID: 13290 RVA: 0x000E0E5F File Offset: 0x000DF05F
		public Rigidbody AttachedRigidbody { get; private set; }

		// Token: 0x17000480 RID: 1152
		// (get) Token: 0x060033EB RID: 13291 RVA: 0x000E0E68 File Offset: 0x000DF068
		// (set) Token: 0x060033EC RID: 13292 RVA: 0x000E0E70 File Offset: 0x000DF070
		public Vector3 CharacterTransformToCapsuleCenter { get; private set; }

		// Token: 0x17000481 RID: 1153
		// (get) Token: 0x060033ED RID: 13293 RVA: 0x000E0E79 File Offset: 0x000DF079
		// (set) Token: 0x060033EE RID: 13294 RVA: 0x000E0E81 File Offset: 0x000DF081
		public Vector3 CharacterTransformToCapsuleBottom { get; private set; }

		// Token: 0x17000482 RID: 1154
		// (get) Token: 0x060033EF RID: 13295 RVA: 0x000E0E8A File Offset: 0x000DF08A
		// (set) Token: 0x060033F0 RID: 13296 RVA: 0x000E0E92 File Offset: 0x000DF092
		public Vector3 CharacterTransformToCapsuleTop { get; private set; }

		// Token: 0x17000483 RID: 1155
		// (get) Token: 0x060033F1 RID: 13297 RVA: 0x000E0E9B File Offset: 0x000DF09B
		// (set) Token: 0x060033F2 RID: 13298 RVA: 0x000E0EA3 File Offset: 0x000DF0A3
		public Vector3 CharacterTransformToCapsuleBottomHemi { get; private set; }

		// Token: 0x17000484 RID: 1156
		// (get) Token: 0x060033F3 RID: 13299 RVA: 0x000E0EAC File Offset: 0x000DF0AC
		// (set) Token: 0x060033F4 RID: 13300 RVA: 0x000E0EB4 File Offset: 0x000DF0B4
		public Vector3 CharacterTransformToCapsuleTopHemi { get; private set; }

		// Token: 0x17000485 RID: 1157
		// (get) Token: 0x060033F5 RID: 13301 RVA: 0x000E0EBD File Offset: 0x000DF0BD
		// (set) Token: 0x060033F6 RID: 13302 RVA: 0x000E0EC5 File Offset: 0x000DF0C5
		public bool MustUnground { get; set; }

		// Token: 0x17000486 RID: 1158
		// (get) Token: 0x060033F7 RID: 13303 RVA: 0x000E0ECE File Offset: 0x000DF0CE
		// (set) Token: 0x060033F8 RID: 13304 RVA: 0x000E0ED6 File Offset: 0x000DF0D6
		public bool LastMovementIterationFoundAnyGround { get; set; }

		// Token: 0x17000487 RID: 1159
		// (get) Token: 0x060033F9 RID: 13305 RVA: 0x000E0EDF File Offset: 0x000DF0DF
		// (set) Token: 0x060033FA RID: 13306 RVA: 0x000E0EE7 File Offset: 0x000DF0E7
		public int IndexInCharacterSystem { get; set; }

		// Token: 0x17000488 RID: 1160
		// (get) Token: 0x060033FB RID: 13307 RVA: 0x000E0EF0 File Offset: 0x000DF0F0
		// (set) Token: 0x060033FC RID: 13308 RVA: 0x000E0EF8 File Offset: 0x000DF0F8
		public Vector3 InitialTickPosition { get; set; }

		// Token: 0x17000489 RID: 1161
		// (get) Token: 0x060033FD RID: 13309 RVA: 0x000E0F01 File Offset: 0x000DF101
		// (set) Token: 0x060033FE RID: 13310 RVA: 0x000E0F09 File Offset: 0x000DF109
		public Quaternion InitialTickRotation { get; set; }

		// Token: 0x1700048A RID: 1162
		// (get) Token: 0x060033FF RID: 13311 RVA: 0x000E0F12 File Offset: 0x000DF112
		// (set) Token: 0x06003400 RID: 13312 RVA: 0x000E0F1A File Offset: 0x000DF11A
		public Rigidbody AttachedRigidbodyOverride { get; set; }

		// Token: 0x1700048B RID: 1163
		// (get) Token: 0x06003401 RID: 13313 RVA: 0x000E0F23 File Offset: 0x000DF123
		// (set) Token: 0x06003402 RID: 13314 RVA: 0x000E0F2B File Offset: 0x000DF12B
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

		// Token: 0x1700048C RID: 1164
		// (get) Token: 0x06003403 RID: 13315 RVA: 0x000E0F34 File Offset: 0x000DF134
		// (set) Token: 0x06003404 RID: 13316 RVA: 0x000E0F3C File Offset: 0x000DF13C
		public Quaternion TransientRotation
		{
			get
			{
				return this._internalTransientRotation;
			}
			private set
			{
				this._internalTransientRotation = value;
				this.CharacterUp = this._internalTransientRotation * this._cachedWorldUp;
				this.CharacterForward = this._internalTransientRotation * this._cachedWorldForward;
				this.CharacterRight = this._internalTransientRotation * this._cachedWorldRight;
			}
		}

		// Token: 0x1700048D RID: 1165
		// (get) Token: 0x06003405 RID: 13317 RVA: 0x000E0F95 File Offset: 0x000DF195
		public Vector3 InterpolatedPosition
		{
			get
			{
				return this.Transform.position;
			}
		}

		// Token: 0x1700048E RID: 1166
		// (get) Token: 0x06003406 RID: 13318 RVA: 0x000E0FA2 File Offset: 0x000DF1A2
		public Quaternion InterpolatedRotation
		{
			get
			{
				return this.Transform.rotation;
			}
		}

		// Token: 0x1700048F RID: 1167
		// (get) Token: 0x06003407 RID: 13319 RVA: 0x000E0FAF File Offset: 0x000DF1AF
		public Vector3 Velocity
		{
			get
			{
				return this._baseVelocity + this._attachedRigidbodyVelocity;
			}
		}

		// Token: 0x17000490 RID: 1168
		// (get) Token: 0x06003408 RID: 13320 RVA: 0x000E0FC2 File Offset: 0x000DF1C2
		// (set) Token: 0x06003409 RID: 13321 RVA: 0x000E0FCA File Offset: 0x000DF1CA
		public Vector3 BaseVelocity
		{
			get
			{
				return this._baseVelocity;
			}
			set
			{
				this._baseVelocity = value;
			}
		}

		// Token: 0x17000491 RID: 1169
		// (get) Token: 0x0600340A RID: 13322 RVA: 0x000E0FD3 File Offset: 0x000DF1D3
		// (set) Token: 0x0600340B RID: 13323 RVA: 0x000E0FDB File Offset: 0x000DF1DB
		public Vector3 AttachedRigidbodyVelocity
		{
			get
			{
				return this._attachedRigidbodyVelocity;
			}
			set
			{
				this._attachedRigidbodyVelocity = value;
			}
		}

		// Token: 0x17000492 RID: 1170
		// (get) Token: 0x0600340C RID: 13324 RVA: 0x000E0FE4 File Offset: 0x000DF1E4
		// (set) Token: 0x0600340D RID: 13325 RVA: 0x000E0FEC File Offset: 0x000DF1EC
		public int OverlapsCount { get; private set; }

		// Token: 0x17000493 RID: 1171
		// (get) Token: 0x0600340E RID: 13326 RVA: 0x000E0FF5 File Offset: 0x000DF1F5
		public OverlapResult[] Overlaps
		{
			get
			{
				return this._overlaps;
			}
		}

		// Token: 0x0600340F RID: 13327 RVA: 0x000E0FFD File Offset: 0x000DF1FD
		private void OnEnable()
		{
			KinematicCharacterSystem.EnsureCreation();
			KinematicCharacterSystem.RegisterCharacterMotor(this);
		}

		// Token: 0x06003410 RID: 13328 RVA: 0x000E100A File Offset: 0x000DF20A
		private void OnDisable()
		{
			KinematicCharacterSystem.UnregisterCharacterMotor(this);
		}

		// Token: 0x06003411 RID: 13329 RVA: 0x000E1012 File Offset: 0x000DF212
		private void Reset()
		{
			this.ValidateData();
		}

		// Token: 0x06003412 RID: 13330 RVA: 0x0000409B File Offset: 0x0000229B
		private void OnValidate()
		{
		}

		// Token: 0x06003413 RID: 13331 RVA: 0x000E101C File Offset: 0x000DF21C
		[ContextMenu("Remove Component")]
		private void HandleRemoveComponent()
		{
			Rigidbody component = base.gameObject.GetComponent<Rigidbody>();
			UnityEngine.Object component2 = base.gameObject.GetComponent<CapsuleCollider>();
			UnityEngine.Object.DestroyImmediate(this);
			UnityEngine.Object.DestroyImmediate(component);
			UnityEngine.Object.DestroyImmediate(component2);
		}

		// Token: 0x06003414 RID: 13332 RVA: 0x000E1054 File Offset: 0x000DF254
		public void ValidateData()
		{
			this.Rigidbody = base.GetComponent<Rigidbody>();
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
			this.Capsule = base.GetComponent<CapsuleCollider>();
			this.CapsuleRadius = Mathf.Clamp(this.CapsuleRadius, 0f, this.CapsuleHeight * 0.5f);
			this.Capsule.isTrigger = false;
			this.Capsule.direction = 1;
			this.Capsule.sharedMaterial = this.CapsulePhysicsMaterial;
			this.SetCapsuleDimensions(this.CapsuleRadius, this.CapsuleHeight, this.CapsuleYOffset);
			this.MaxStepHeight = Mathf.Clamp(this.MaxStepHeight, 0f, float.PositiveInfinity);
			this.MinRequiredStepDepth = Mathf.Clamp(this.MinRequiredStepDepth, 0f, this.CapsuleRadius);
			this.MaxStableDistanceFromLedge = Mathf.Clamp(this.MaxStableDistanceFromLedge, 0f, this.CapsuleRadius);
			base.transform.localScale = Vector3.one;
		}

		// Token: 0x06003415 RID: 13333 RVA: 0x000E11D6 File Offset: 0x000DF3D6
		public void SetCapsuleCollisionsActivation(bool kinematicCapsuleActive)
		{
			this.Rigidbody.detectCollisions = kinematicCapsuleActive;
		}

		// Token: 0x06003416 RID: 13334 RVA: 0x000E11E4 File Offset: 0x000DF3E4
		public void SetMovementCollisionsSolvingActivation(bool movementCollisionsSolvingActive)
		{
			this._solveMovementCollisions = movementCollisionsSolvingActive;
		}

		// Token: 0x06003417 RID: 13335 RVA: 0x000E11ED File Offset: 0x000DF3ED
		public void SetGroundSolvingActivation(bool stabilitySolvingActive)
		{
			this._solveGrounding = stabilitySolvingActive;
		}

		// Token: 0x06003418 RID: 13336 RVA: 0x000E11F8 File Offset: 0x000DF3F8
		public void SetPosition(Vector3 position, bool bypassInterpolation = true)
		{
			this.Rigidbody.interpolation = RigidbodyInterpolation.None;
			this.Transform.position = position;
			this.Rigidbody.position = position;
			this.InitialSimulationPosition = position;
			this.TransientPosition = position;
			if (bypassInterpolation)
			{
				this.InitialTickPosition = position;
			}
			this.Rigidbody.interpolation = ((KinematicCharacterSystem.InterpolationMethod == CharacterSystemInterpolationMethod.Unity) ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None);
		}

		// Token: 0x06003419 RID: 13337 RVA: 0x000E1258 File Offset: 0x000DF458
		public void SetRotation(Quaternion rotation, bool bypassInterpolation = true)
		{
			this.Rigidbody.interpolation = RigidbodyInterpolation.None;
			this.Transform.rotation = rotation;
			this.Rigidbody.rotation = rotation;
			this.InitialSimulationRotation = rotation;
			this.TransientRotation = rotation;
			if (bypassInterpolation)
			{
				this.InitialTickRotation = rotation;
			}
			this.Rigidbody.interpolation = ((KinematicCharacterSystem.InterpolationMethod == CharacterSystemInterpolationMethod.Unity) ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None);
		}

		// Token: 0x0600341A RID: 13338 RVA: 0x000E12B8 File Offset: 0x000DF4B8
		public void SetPositionAndRotation(Vector3 position, Quaternion rotation, bool bypassInterpolation = true)
		{
			this.Rigidbody.interpolation = RigidbodyInterpolation.None;
			this.Transform.SetPositionAndRotation(position, rotation);
			this.Rigidbody.position = position;
			this.Rigidbody.rotation = rotation;
			this.InitialSimulationPosition = position;
			this.InitialSimulationRotation = rotation;
			this.TransientPosition = position;
			this.TransientRotation = rotation;
			if (bypassInterpolation)
			{
				this.InitialTickPosition = position;
				this.InitialTickRotation = rotation;
			}
			this.Rigidbody.interpolation = ((KinematicCharacterSystem.InterpolationMethod == CharacterSystemInterpolationMethod.Unity) ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None);
		}

		// Token: 0x0600341B RID: 13339 RVA: 0x000E133A File Offset: 0x000DF53A
		public void MoveCharacter(Vector3 toPosition)
		{
			this._movePositionDirty = true;
			this._movePositionTarget = toPosition;
		}

		// Token: 0x0600341C RID: 13340 RVA: 0x000E134A File Offset: 0x000DF54A
		public void RotateCharacter(Quaternion toRotation)
		{
			this._moveRotationDirty = true;
			this._moveRotationTarget = toRotation;
		}

		// Token: 0x0600341D RID: 13341 RVA: 0x000E135C File Offset: 0x000DF55C
		public KinematicCharacterMotorState GetState()
		{
			KinematicCharacterMotorState result = default(KinematicCharacterMotorState);
			result.Position = this.TransientPosition;
			result.Rotation = this.TransientRotation;
			result.BaseVelocity = this._baseVelocity;
			result.AttachedRigidbodyVelocity = this._attachedRigidbodyVelocity;
			result.MustUnground = this.MustUnground;
			result.LastMovementIterationFoundAnyGround = this.LastMovementIterationFoundAnyGround;
			result.GroundingStatus.CopyFrom(this.GroundingStatus);
			result.AttachedRigidbody = this.AttachedRigidbody;
			return result;
		}

		// Token: 0x0600341E RID: 13342 RVA: 0x000E13E0 File Offset: 0x000DF5E0
		public void ApplyState(KinematicCharacterMotorState state, bool bypassInterpolation = true)
		{
			this.SetPositionAndRotation(state.Position, state.Rotation, bypassInterpolation);
			this.BaseVelocity = state.BaseVelocity;
			this.AttachedRigidbodyVelocity = state.AttachedRigidbodyVelocity;
			this.MustUnground = state.MustUnground;
			this.LastMovementIterationFoundAnyGround = state.LastMovementIterationFoundAnyGround;
			this.GroundingStatus.CopyFrom(state.GroundingStatus);
			this.AttachedRigidbody = state.AttachedRigidbody;
		}

		// Token: 0x0600341F RID: 13343 RVA: 0x000E1450 File Offset: 0x000DF650
		public void SetCapsuleDimensions(float radius, float height, float yOffset)
		{
			this.CapsuleRadius = radius;
			this.CapsuleHeight = height;
			this.CapsuleYOffset = yOffset;
			this.Capsule.radius = this.CapsuleRadius;
			this.Capsule.height = Mathf.Clamp(this.CapsuleHeight, this.CapsuleRadius * 2f, this.CapsuleHeight);
			this.Capsule.center = new Vector3(0f, this.CapsuleYOffset, 0f);
			this.CharacterTransformToCapsuleCenter = this.Capsule.center;
			this.CharacterTransformToCapsuleBottom = this.Capsule.center + -this._cachedWorldUp * (this.Capsule.height * 0.5f);
			this.CharacterTransformToCapsuleTop = this.Capsule.center + this._cachedWorldUp * (this.Capsule.height * 0.5f);
			this.CharacterTransformToCapsuleBottomHemi = this.Capsule.center + -this._cachedWorldUp * (this.Capsule.height * 0.5f) + this._cachedWorldUp * this.Capsule.radius;
			this.CharacterTransformToCapsuleTopHemi = this.Capsule.center + this._cachedWorldUp * (this.Capsule.height * 0.5f) + -this._cachedWorldUp * this.Capsule.radius;
		}

		// Token: 0x06003420 RID: 13344 RVA: 0x000E15EC File Offset: 0x000DF7EC
		private void Awake()
		{
			this.Transform = base.transform;
			this.ValidateData();
			this.InitialTickPosition = this.Transform.position;
			this.InitialTickRotation = this.Transform.rotation;
			this.TransientPosition = this.Transform.position;
			this.TransientRotation = this.Transform.rotation;
			this.RebuildCollidableLayers();
			if (this.CharacterController)
			{
				this.CharacterController.SetupCharacterMotor(this);
			}
			this.SetCapsuleDimensions(this.CapsuleRadius, this.CapsuleHeight, this.CapsuleYOffset);
		}

		// Token: 0x06003421 RID: 13345 RVA: 0x000E1688 File Offset: 0x000DF888
		public void RebuildCollidableLayers()
		{
			this.CollidableLayers = 0;
			for (int i = 0; i < 32; i++)
			{
				if (!Physics.GetIgnoreLayerCollision(base.gameObject.layer, i))
				{
					this.CollidableLayers |= 1 << i;
				}
			}
		}

		// Token: 0x06003422 RID: 13346 RVA: 0x000E16E0 File Offset: 0x000DF8E0
		public void UpdatePhase1(float deltaTime)
		{
			if (float.IsNaN(this._baseVelocity.x) || float.IsNaN(this._baseVelocity.y) || float.IsNaN(this._baseVelocity.z))
			{
				this._baseVelocity = Vector3.zero;
			}
			if (float.IsNaN(this._attachedRigidbodyVelocity.x) || float.IsNaN(this._attachedRigidbodyVelocity.y) || float.IsNaN(this._attachedRigidbodyVelocity.z))
			{
				this._attachedRigidbodyVelocity = Vector3.zero;
			}
			this.CharacterController.BeforeCharacterUpdate(deltaTime);
			this.TransientPosition = this.Transform.position;
			this.TransientRotation = this.Transform.rotation;
			this.InitialSimulationPosition = this.TransientPosition;
			this.InitialSimulationRotation = this.TransientRotation;
			this._rigidbodyProjectionHitCount = 0;
			this.OverlapsCount = 0;
			this._lastSolvedOverlapNormalDirty = false;
			if (this._movePositionDirty)
			{
				if (this._solveMovementCollisions)
				{
					if (this.InternalCharacterMove(this._movePositionTarget - this.TransientPosition, deltaTime, out this._internalResultingMovementMagnitude, out this._internalResultingMovementDirection) && this.InteractiveRigidbodyHandling)
					{
						Vector3 zero = Vector3.zero;
						this.ProcessVelocityForRigidbodyHits(ref zero, deltaTime);
					}
				}
				else
				{
					this.TransientPosition = this._movePositionTarget;
				}
				this._movePositionDirty = false;
			}
			this.LastGroundingStatus.CopyFrom(this.GroundingStatus);
			this.GroundingStatus = default(CharacterGroundingReport);
			this.GroundingStatus.GroundNormal = this.CharacterUp;
			if (this._solveMovementCollisions)
			{
				Vector3 vector = this._cachedWorldUp;
				float num = 0f;
				int num2 = 0;
				bool flag = false;
				while (num2 < 3 && !flag)
				{
					int num3 = this.CharacterCollisionsOverlap(this.TransientPosition, this.TransientRotation, this._internalProbedColliders, 0f);
					if (num3 > 0)
					{
						for (int i = 0; i < num3; i++)
						{
							Rigidbody attachedRigidbody = this._internalProbedColliders[i].attachedRigidbody;
							if (!attachedRigidbody || (attachedRigidbody.isKinematic && !attachedRigidbody.GetComponent<PhysicsMover>()))
							{
								Transform component = this._internalProbedColliders[i].GetComponent<Transform>();
								if (Physics.ComputePenetration(this.Capsule, this.TransientPosition, this.TransientRotation, this._internalProbedColliders[i], component.position, component.rotation, out vector, out num))
								{
									HitStabilityReport hitStabilityReport = new HitStabilityReport
									{
										IsStable = this.IsStableOnNormal(vector)
									};
									vector = this.GetObstructionNormal(vector, hitStabilityReport);
									Vector3 b = vector * (num + 0.001f);
									this.TransientPosition += b;
									if (this.OverlapsCount < this._overlaps.Length)
									{
										this._overlaps[this.OverlapsCount] = new OverlapResult(vector, this._internalProbedColliders[i]);
										int overlapsCount = this.OverlapsCount;
										this.OverlapsCount = overlapsCount + 1;
										break;
									}
									break;
								}
							}
						}
					}
					else
					{
						flag = true;
					}
					num2++;
				}
			}
			if (this._solveGrounding)
			{
				if (this.MustUnground)
				{
					this.TransientPosition += this.CharacterUp * 0.0075f;
				}
				else
				{
					float num4 = 0.005f;
					if (!this.LastGroundingStatus.SnappingPrevented && (this.LastGroundingStatus.IsStableOnGround || this.LastMovementIterationFoundAnyGround))
					{
						if (this.StepHandling != StepHandlingMethod.None)
						{
							num4 = Mathf.Max(this.CapsuleRadius, this.MaxStepHeight);
						}
						else
						{
							num4 = this.CapsuleRadius;
						}
						num4 += this.GroundDetectionExtraDistance;
					}
					this.ProbeGround(ref this._internalTransientPosition, this.TransientRotation, num4, ref this.GroundingStatus);
				}
			}
			this.LastMovementIterationFoundAnyGround = false;
			this.MustUnground = false;
			if (this._solveGrounding)
			{
				this.CharacterController.PostGroundingUpdate(deltaTime);
			}
			if (this.InteractiveRigidbodyHandling)
			{
				this._lastAttachedRigidbody = this.AttachedRigidbody;
				if (this.AttachedRigidbodyOverride)
				{
					this.AttachedRigidbody = this.AttachedRigidbodyOverride;
				}
				else if (this.GroundingStatus.IsStableOnGround && this.GroundingStatus.GroundCollider.attachedRigidbody)
				{
					Rigidbody interactiveRigidbody = this.GetInteractiveRigidbody(this.GroundingStatus.GroundCollider);
					if (interactiveRigidbody)
					{
						this.AttachedRigidbody = interactiveRigidbody;
					}
				}
				else
				{
					this.AttachedRigidbody = null;
				}
				Vector3 vector2 = Vector3.zero;
				if (this.AttachedRigidbody)
				{
					vector2 = this.GetVelocityFromRigidbodyMovement(this.AttachedRigidbody, this.TransientPosition, deltaTime);
				}
				if (this.PreserveAttachedRigidbodyMomentum && this._lastAttachedRigidbody != null && this.AttachedRigidbody != this._lastAttachedRigidbody)
				{
					this._baseVelocity += this._attachedRigidbodyVelocity;
					this._baseVelocity -= vector2;
				}
				this._attachedRigidbodyVelocity = this._cachedZeroVector;
				if (this.AttachedRigidbody)
				{
					this._attachedRigidbodyVelocity = vector2;
					Vector3 normalized = Vector3.ProjectOnPlane(Quaternion.Euler(57.29578f * this.AttachedRigidbody.angularVelocity * deltaTime) * this.CharacterForward, this.CharacterUp).normalized;
					this.TransientRotation = Quaternion.LookRotation(normalized, this.CharacterUp);
				}
				if (this.GroundingStatus.GroundCollider && this.GroundingStatus.GroundCollider.attachedRigidbody && this.GroundingStatus.GroundCollider.attachedRigidbody == this.AttachedRigidbody && this.AttachedRigidbody != null && this._lastAttachedRigidbody == null)
				{
					this._baseVelocity -= Vector3.ProjectOnPlane(this._attachedRigidbodyVelocity, this.CharacterUp);
				}
				if (this._attachedRigidbodyVelocity.sqrMagnitude > 0f)
				{
					this._isMovingFromAttachedRigidbody = true;
					if (this._solveMovementCollisions)
					{
						if (this.InternalCharacterMove(this._attachedRigidbodyVelocity * deltaTime, deltaTime, out this._internalResultingMovementMagnitude, out this._internalResultingMovementDirection))
						{
							this._attachedRigidbodyVelocity = this._internalResultingMovementDirection * this._internalResultingMovementMagnitude / deltaTime;
						}
						else
						{
							this._attachedRigidbodyVelocity = Vector3.zero;
						}
					}
					else
					{
						this.TransientPosition += this._attachedRigidbodyVelocity * deltaTime;
					}
					this._isMovingFromAttachedRigidbody = false;
				}
			}
		}

		// Token: 0x06003423 RID: 13347 RVA: 0x000E1D2C File Offset: 0x000DFF2C
		public void UpdatePhase2(float deltaTime)
		{
			this.CharacterController.UpdateRotation(ref this._internalTransientRotation, deltaTime);
			this.TransientRotation = this._internalTransientRotation;
			if (this._moveRotationDirty)
			{
				this.TransientRotation = this._moveRotationTarget;
				this._moveRotationDirty = false;
			}
			if (this._solveMovementCollisions && this.InteractiveRigidbodyHandling)
			{
				if (this.InteractiveRigidbodyHandling && this.AttachedRigidbody)
				{
					float radius = this.Capsule.radius;
					RaycastHit raycastHit;
					if (this.CharacterGroundSweep(this.TransientPosition + this.CharacterUp * radius, this.TransientRotation, -this.CharacterUp, radius, out raycastHit) && raycastHit.collider.attachedRigidbody == this.AttachedRigidbody && this.IsStableOnNormal(raycastHit.normal))
					{
						float d = radius - raycastHit.distance;
						this.TransientPosition = this.TransientPosition + this.CharacterUp * d + this.CharacterUp * 0.001f;
					}
				}
				if (this.SafeMovement || this.InteractiveRigidbodyHandling)
				{
					Vector3 vector = this._cachedWorldUp;
					float num = 0f;
					int num2 = 0;
					bool flag = false;
					while (num2 < 3 && !flag)
					{
						int num3 = this.CharacterCollisionsOverlap(this.TransientPosition, this.TransientRotation, this._internalProbedColliders, 0f);
						if (num3 > 0)
						{
							int i = 0;
							while (i < num3)
							{
								Transform component = this._internalProbedColliders[i].GetComponent<Transform>();
								if (Physics.ComputePenetration(this.Capsule, this.TransientPosition, this.TransientRotation, this._internalProbedColliders[i], component.position, component.rotation, out vector, out num))
								{
									HitStabilityReport hitStabilityReport = new HitStabilityReport
									{
										IsStable = this.IsStableOnNormal(vector)
									};
									vector = this.GetObstructionNormal(vector, hitStabilityReport);
									Vector3 b = vector * (num + 0.001f);
									this.TransientPosition += b;
									if (this.InteractiveRigidbodyHandling)
									{
										Rigidbody attachedRigidbody = this._internalProbedColliders[i].attachedRigidbody;
										if (attachedRigidbody)
										{
											PhysicsMover component2 = attachedRigidbody.GetComponent<PhysicsMover>();
											if (component2 && (attachedRigidbody && (!attachedRigidbody.isKinematic || component2)))
											{
												HitStabilityReport hitStabilityReport2 = new HitStabilityReport
												{
													IsStable = this.IsStableOnNormal(vector)
												};
												if (hitStabilityReport2.IsStable)
												{
													this.LastMovementIterationFoundAnyGround = hitStabilityReport2.IsStable;
												}
												if (component2.Rigidbody && component2.Rigidbody != this.AttachedRigidbody)
												{
													Vector3 point = this.TransientPosition + this.TransientRotation * this.CharacterTransformToCapsuleCenter;
													Vector3 transientPosition = this.TransientPosition;
													MeshCollider meshCollider = this._internalProbedColliders[i] as MeshCollider;
													if (!meshCollider || meshCollider.convex)
													{
														Physics.ClosestPoint(point, this._internalProbedColliders[i], component.position, component.rotation);
													}
													this.StoreRigidbodyHit(component2.Rigidbody, this.Velocity, transientPosition, vector, hitStabilityReport2);
												}
											}
										}
									}
									if (this.OverlapsCount < this._overlaps.Length)
									{
										this._overlaps[this.OverlapsCount] = new OverlapResult(vector, this._internalProbedColliders[i]);
										int overlapsCount = this.OverlapsCount;
										this.OverlapsCount = overlapsCount + 1;
										break;
									}
									break;
								}
								else
								{
									i++;
								}
							}
						}
						else
						{
							flag = true;
						}
						num2++;
					}
				}
			}
			this.CharacterController.UpdateVelocity(ref this._baseVelocity, deltaTime);
			if (this._baseVelocity.magnitude < 0.01f)
			{
				this._baseVelocity = Vector3.zero;
			}
			if (this._baseVelocity.sqrMagnitude > 0f)
			{
				if (this._solveMovementCollisions)
				{
					if (this.InternalCharacterMove(this._baseVelocity * deltaTime, deltaTime, out this._internalResultingMovementMagnitude, out this._internalResultingMovementDirection))
					{
						this._baseVelocity = this._internalResultingMovementDirection * this._internalResultingMovementMagnitude / deltaTime;
					}
					else
					{
						this._baseVelocity = Vector3.zero;
					}
				}
				else
				{
					this.TransientPosition += this._baseVelocity * deltaTime;
				}
			}
			if (this.InteractiveRigidbodyHandling)
			{
				this.ProcessVelocityForRigidbodyHits(ref this._baseVelocity, deltaTime);
			}
			if (this.HasPlanarConstraint)
			{
				this.TransientPosition = this.InitialSimulationPosition + Vector3.ProjectOnPlane(this.TransientPosition - this.InitialSimulationPosition, this.PlanarConstraintAxis.normalized);
			}
			if (this.DetectDiscreteCollisions)
			{
				int num4 = this.CharacterCollisionsOverlap(this.TransientPosition, this.TransientRotation, this._internalProbedColliders, 0.002f);
				for (int j = 0; j < num4; j++)
				{
					this.CharacterController.OnDiscreteCollisionDetected(this._internalProbedColliders[j]);
				}
			}
			this.CharacterController.AfterCharacterUpdate(deltaTime);
		}

		// Token: 0x06003424 RID: 13348 RVA: 0x000E221F File Offset: 0x000E041F
		private bool IsStableOnNormal(Vector3 normal)
		{
			return Vector3.Angle(this.CharacterUp, normal) <= this.MaxStableSlopeAngle;
		}

		// Token: 0x06003425 RID: 13349 RVA: 0x000E2238 File Offset: 0x000E0438
		public void ProbeGround(ref Vector3 probingPosition, Quaternion atRotation, float probingDistance, ref CharacterGroundingReport groundingReport)
		{
			if (probingDistance < 0.005f)
			{
				probingDistance = 0.005f;
			}
			int num = 0;
			RaycastHit raycastHit = default(RaycastHit);
			bool flag = false;
			Vector3 vector = probingPosition;
			Vector3 vector2 = atRotation * -this._cachedWorldUp;
			float num2 = probingDistance;
			while (num2 > 0f && num <= 2 && !flag)
			{
				if (this.CharacterGroundSweep(vector, atRotation, vector2, num2, out raycastHit))
				{
					Vector3 vector3 = vector + vector2 * raycastHit.distance;
					HitStabilityReport hitStabilityReport = default(HitStabilityReport);
					this.EvaluateHitStability(raycastHit.collider, raycastHit.normal, raycastHit.point, vector3, this.TransientRotation, ref hitStabilityReport);
					if (hitStabilityReport.LedgeDetected && hitStabilityReport.IsOnEmptySideOfLedge && hitStabilityReport.DistanceFromLedge > this.MaxStableDistanceFromLedge)
					{
						hitStabilityReport.IsStable = false;
					}
					groundingReport.FoundAnyGround = true;
					groundingReport.GroundNormal = raycastHit.normal;
					groundingReport.InnerGroundNormal = hitStabilityReport.InnerNormal;
					groundingReport.OuterGroundNormal = hitStabilityReport.OuterNormal;
					groundingReport.GroundCollider = raycastHit.collider;
					groundingReport.GroundPoint = raycastHit.point;
					groundingReport.SnappingPrevented = false;
					if (hitStabilityReport.IsStable)
					{
						if (this.LedgeHandling)
						{
							if (this.LastGroundingStatus.FoundAnyGround && hitStabilityReport.InnerNormal.sqrMagnitude != 0f && hitStabilityReport.OuterNormal.sqrMagnitude != 0f)
							{
								if (Vector3.Angle(hitStabilityReport.InnerNormal, hitStabilityReport.OuterNormal) > this.MaxStableDenivelationAngle)
								{
									groundingReport.SnappingPrevented = true;
								}
								else if (Vector3.Angle(this.LastGroundingStatus.InnerGroundNormal, hitStabilityReport.OuterNormal) > this.MaxStableDenivelationAngle)
								{
									groundingReport.SnappingPrevented = true;
								}
							}
							if (this.PreventSnappingOnLedges && hitStabilityReport.LedgeDetected)
							{
								groundingReport.SnappingPrevented = true;
							}
						}
						groundingReport.IsStableOnGround = true;
						if (!groundingReport.SnappingPrevented)
						{
							vector3 += -vector2 * 0.001f;
							this.InternalMoveCharacterPosition(ref probingPosition, vector3, atRotation);
						}
						this.CharacterController.OnGroundHit(raycastHit.collider, raycastHit.normal, raycastHit.point, ref hitStabilityReport);
						flag = true;
					}
					else
					{
						Vector3 b = vector2 * raycastHit.distance + atRotation * Vector3.up * Mathf.Clamp(0.001f, 0f, raycastHit.distance);
						vector += b;
						num2 = Mathf.Min(0.02f, Mathf.Clamp(num2 - b.magnitude, 0f, float.PositiveInfinity));
						vector2 = Vector3.ProjectOnPlane(vector2, raycastHit.normal).normalized;
					}
				}
				else
				{
					flag = true;
				}
				num++;
			}
		}

		// Token: 0x06003426 RID: 13350 RVA: 0x000E2502 File Offset: 0x000E0702
		public void ForceUnground()
		{
			this.MustUnground = true;
		}

		// Token: 0x06003427 RID: 13351 RVA: 0x000E250C File Offset: 0x000E070C
		public Vector3 GetDirectionTangentToSurface(Vector3 direction, Vector3 surfaceNormal)
		{
			Vector3 rhs = Vector3.Cross(direction, this.CharacterUp);
			return Vector3.Cross(surfaceNormal, rhs).normalized;
		}

		// Token: 0x06003428 RID: 13352 RVA: 0x000E2538 File Offset: 0x000E0738
		private bool InternalCharacterMove(Vector3 movement, float deltaTime, out float resultingMovementMagnitude, out Vector3 resultingMovementDirection)
		{
			this._rigidbodiesPushedCount = 0;
			bool result = true;
			Vector3 normalized = movement.normalized;
			float num = movement.magnitude;
			resultingMovementDirection = normalized;
			resultingMovementMagnitude = num;
			int num2 = 0;
			bool flag = true;
			Vector3 vector = this.TransientPosition;
			Vector3 vector2 = this.TransientPosition;
			Vector3 normalized2 = movement.normalized;
			Vector3 cachedZeroVector = this._cachedZeroVector;
			MovementSweepState movementSweepState = MovementSweepState.Initial;
			for (int i = 0; i < this.OverlapsCount; i++)
			{
				if (Vector3.Dot(normalized, this._overlaps[i].Normal) < 0f)
				{
					this.InternalHandleMovementProjection(this.IsStableOnNormal(this._overlaps[i].Normal) && !this.MustUnground, this._overlaps[i].Normal, this._overlaps[i].Normal, normalized2, ref movementSweepState, ref cachedZeroVector, ref resultingMovementMagnitude, ref normalized, ref num);
				}
			}
			while (num > 0f && num2 <= 6 && flag)
			{
				RaycastHit raycastHit;
				if (this.CharacterCollisionsSweep(vector, this.TransientRotation, normalized, num + 0.001f, out raycastHit, this._internalCharacterHits, 0f) > 0)
				{
					vector2 = vector + normalized * raycastHit.distance + raycastHit.normal * 0.001f;
					Vector3 vector3 = vector2 - vector;
					HitStabilityReport hitStabilityReport = default(HitStabilityReport);
					this.EvaluateHitStability(raycastHit.collider, raycastHit.normal, raycastHit.point, vector2, this.TransientRotation, ref hitStabilityReport);
					bool flag2 = false;
					if (this._solveGrounding && this.StepHandling != StepHandlingMethod.None && hitStabilityReport.ValidStepDetected && Mathf.Abs(Vector3.Dot(raycastHit.normal, this.CharacterUp)) <= 0.01f)
					{
						Vector3 normalized3 = Vector3.ProjectOnPlane(-raycastHit.normal, this.CharacterUp).normalized;
						Vector3 vector4 = vector2 + normalized3 * 0.03f + this.CharacterUp * this.MaxStepHeight;
						RaycastHit raycastHit2;
						int num3 = this.CharacterCollisionsSweep(vector4, this.TransientRotation, -this.CharacterUp, this.MaxStepHeight, out raycastHit2, this._internalCharacterHits, 0f);
						for (int j = 0; j < num3; j++)
						{
							if (this._internalCharacterHits[j].collider == hitStabilityReport.SteppedCollider)
							{
								vector = vector4 + -this.CharacterUp * (this._internalCharacterHits[j].distance - 0.001f);
								flag2 = true;
								num = Mathf.Clamp(num - vector3.magnitude, 0f, float.PositiveInfinity);
								break;
							}
						}
					}
					if (!flag2)
					{
						vector = vector2;
						num = Mathf.Clamp(num - vector3.magnitude, 0f, float.PositiveInfinity);
						this.CharacterController.OnMovementHit(raycastHit.collider, raycastHit.normal, raycastHit.point, ref hitStabilityReport);
						Vector3 obstructionNormal = this.GetObstructionNormal(raycastHit.normal, hitStabilityReport);
						if (this.InteractiveRigidbodyHandling && raycastHit.collider.attachedRigidbody)
						{
							this.StoreRigidbodyHit(raycastHit.collider.attachedRigidbody, normalized * resultingMovementMagnitude / deltaTime, raycastHit.point, obstructionNormal, hitStabilityReport);
						}
						this.InternalHandleMovementProjection(hitStabilityReport.IsStable && !this.MustUnground, raycastHit.normal, obstructionNormal, normalized2, ref movementSweepState, ref cachedZeroVector, ref resultingMovementMagnitude, ref normalized, ref num);
					}
				}
				else
				{
					flag = false;
				}
				num2++;
				if (num2 > 6)
				{
					num = 0f;
					result = false;
				}
			}
			Vector3 targetPosition = vector + normalized * num;
			this.InternalMoveCharacterPosition(ref this._internalTransientPosition, targetPosition, this.TransientRotation);
			resultingMovementDirection = normalized;
			return result;
		}

		// Token: 0x06003429 RID: 13353 RVA: 0x000E2924 File Offset: 0x000E0B24
		private Vector3 GetObstructionNormal(Vector3 hitNormal, HitStabilityReport hitStabilityReport)
		{
			Vector3 vector = hitNormal;
			if (this.GroundingStatus.IsStableOnGround && !this.MustUnground && !hitStabilityReport.IsStable)
			{
				vector = Vector3.Cross(Vector3.Cross(this.GroundingStatus.GroundNormal, vector).normalized, this.CharacterUp).normalized;
			}
			if (vector == Vector3.zero)
			{
				vector = hitNormal;
			}
			return vector;
		}

		// Token: 0x0600342A RID: 13354 RVA: 0x000E2990 File Offset: 0x000E0B90
		private void StoreRigidbodyHit(Rigidbody hitRigidbody, Vector3 hitVelocity, Vector3 hitPoint, Vector3 obstructionNormal, HitStabilityReport hitStabilityReport)
		{
			if (this._rigidbodyProjectionHitCount < this._internalRigidbodyProjectionHits.Length && !hitRigidbody.GetComponent<KinematicCharacterMotor>())
			{
				RigidbodyProjectionHit rigidbodyProjectionHit = default(RigidbodyProjectionHit);
				rigidbodyProjectionHit.Rigidbody = hitRigidbody;
				rigidbodyProjectionHit.HitPoint = hitPoint;
				rigidbodyProjectionHit.EffectiveHitNormal = obstructionNormal;
				rigidbodyProjectionHit.HitVelocity = hitVelocity;
				rigidbodyProjectionHit.StableOnHit = hitStabilityReport.IsStable;
				this._internalRigidbodyProjectionHits[this._rigidbodyProjectionHitCount] = rigidbodyProjectionHit;
				this._rigidbodyProjectionHitCount++;
			}
		}

		// Token: 0x0600342B RID: 13355 RVA: 0x000E2A14 File Offset: 0x000E0C14
		private void InternalHandleMovementProjection(bool stableOnHit, Vector3 hitNormal, Vector3 obstructionNormal, Vector3 originalMoveDirection, ref MovementSweepState sweepState, ref Vector3 previousObstructionNormal, ref float resultingMovementMagnitude, ref Vector3 remainingMovementDirection, ref float remainingMovementMagnitude)
		{
			if (remainingMovementMagnitude <= 0f)
			{
				return;
			}
			Vector3 vector = originalMoveDirection * remainingMovementMagnitude;
			float num = remainingMovementMagnitude;
			if (stableOnHit)
			{
				this.LastMovementIterationFoundAnyGround = true;
			}
			if (sweepState == MovementSweepState.FoundBlockingCrease)
			{
				remainingMovementMagnitude = 0f;
				resultingMovementMagnitude = 0f;
				sweepState = MovementSweepState.FoundBlockingCorner;
			}
			else
			{
				this.CharacterController.HandleMovementProjection(ref vector, obstructionNormal, stableOnHit);
				remainingMovementDirection = vector.normalized;
				remainingMovementMagnitude = vector.magnitude;
				resultingMovementMagnitude = remainingMovementMagnitude / num * resultingMovementMagnitude;
				if (sweepState == MovementSweepState.Initial)
				{
					sweepState = MovementSweepState.AfterFirstHit;
				}
				else if (sweepState == MovementSweepState.AfterFirstHit && Vector3.Dot(previousObstructionNormal, remainingMovementDirection) < 0f)
				{
					Vector3 normalized = Vector3.Cross(previousObstructionNormal, obstructionNormal).normalized;
					vector = Vector3.Project(vector, normalized);
					remainingMovementDirection = vector.normalized;
					remainingMovementMagnitude = vector.magnitude;
					resultingMovementMagnitude = remainingMovementMagnitude / num * resultingMovementMagnitude;
					sweepState = MovementSweepState.FoundBlockingCrease;
				}
			}
			previousObstructionNormal = obstructionNormal;
		}

		// Token: 0x0600342C RID: 13356 RVA: 0x000E2B10 File Offset: 0x000E0D10
		private bool InternalMoveCharacterPosition(ref Vector3 movedPosition, Vector3 targetPosition, Quaternion atRotation)
		{
			bool flag = true;
			if (this.SafeMovement && this.CharacterCollisionsOverlap(targetPosition, atRotation, this._internalProbedColliders, 0f) > 0)
			{
				flag = false;
			}
			if (flag)
			{
				movedPosition = targetPosition;
				return true;
			}
			return false;
		}

		// Token: 0x0600342D RID: 13357 RVA: 0x000E2B4C File Offset: 0x000E0D4C
		private void ProcessVelocityForRigidbodyHits(ref Vector3 processedVelocity, float deltaTime)
		{
			for (int i = 0; i < this._rigidbodyProjectionHitCount; i++)
			{
				if (this._internalRigidbodyProjectionHits[i].Rigidbody)
				{
					bool flag = false;
					for (int j = 0; j < this._rigidbodiesPushedCount; j++)
					{
						if (this._rigidbodiesPushedThisMove[j] == this._internalRigidbodyProjectionHits[j].Rigidbody)
						{
							flag = true;
							break;
						}
					}
					if (!flag && this._internalRigidbodyProjectionHits[i].Rigidbody != this.AttachedRigidbody && this._rigidbodiesPushedCount < this._rigidbodiesPushedThisMove.Length)
					{
						this._rigidbodiesPushedThisMove[this._rigidbodiesPushedCount] = this._internalRigidbodyProjectionHits[i].Rigidbody;
						this._rigidbodiesPushedCount++;
						if (this.RigidbodyInteractionType == RigidbodyInteractionType.SimulatedDynamic)
						{
							this.CharacterController.HandleSimulatedRigidbodyInteraction(ref processedVelocity, this._internalRigidbodyProjectionHits[i], deltaTime);
						}
					}
				}
			}
		}

		// Token: 0x0600342E RID: 13358 RVA: 0x000E2C41 File Offset: 0x000E0E41
		private bool CheckIfColliderValidForCollisions(Collider coll)
		{
			return !(coll == null) && !(coll == this.Capsule) && this.IsColliderValidForCollisions(coll);
		}

		// Token: 0x0600342F RID: 13359 RVA: 0x000E2C68 File Offset: 0x000E0E68
		private bool IsColliderValidForCollisions(Collider coll)
		{
			return ((!this._isMovingFromAttachedRigidbody && this.RigidbodyInteractionType != RigidbodyInteractionType.Kinematic) || !coll.attachedRigidbody || coll.attachedRigidbody.isKinematic) && (!this._isMovingFromAttachedRigidbody || !(coll.attachedRigidbody == this.AttachedRigidbody)) && this.CharacterController.IsColliderValidForCollisions(coll);
		}

		// Token: 0x06003430 RID: 13360 RVA: 0x000E2CD0 File Offset: 0x000E0ED0
		public void EvaluateHitStability(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport stabilityReport)
		{
			if (!this._solveGrounding)
			{
				stabilityReport.IsStable = false;
				return;
			}
			Vector3 vector = atCharacterRotation * Vector3.up;
			Vector3 normalized = Vector3.ProjectOnPlane(hitNormal, vector).normalized;
			bool flag = this.IsStableOnNormal(hitNormal);
			stabilityReport.InnerNormal = hitNormal;
			stabilityReport.OuterNormal = hitNormal;
			if (this.StepHandling != StepHandlingMethod.None && !flag)
			{
				Rigidbody attachedRigidbody = hitCollider.attachedRigidbody;
				if (!attachedRigidbody || attachedRigidbody.isKinematic)
				{
					this.DetectSteps(atCharacterPosition, atCharacterRotation, hitPoint, normalized, ref stabilityReport);
				}
			}
			if (this.LedgeHandling)
			{
				float num = 0.05f;
				if (this.StepHandling != StepHandlingMethod.None)
				{
					num = this.MaxStepHeight;
				}
				bool flag2 = false;
				bool flag3 = false;
				RaycastHit raycastHit;
				if (this.CharacterCollisionsRaycast(hitPoint + vector * 0.02f + normalized * 0.001f, -vector, num + 0.02f, out raycastHit, this._internalCharacterHits) > 0)
				{
					stabilityReport.InnerNormal = raycastHit.normal;
					flag2 = this.IsStableOnNormal(raycastHit.normal);
				}
				RaycastHit raycastHit2;
				if (this.CharacterCollisionsRaycast(hitPoint + vector * 0.02f + -normalized * 0.001f, -vector, num + 0.02f, out raycastHit2, this._internalCharacterHits) > 0)
				{
					stabilityReport.OuterNormal = raycastHit2.normal;
					flag3 = this.IsStableOnNormal(raycastHit2.normal);
				}
				stabilityReport.LedgeDetected = (flag2 != flag3);
				if (stabilityReport.LedgeDetected)
				{
					stabilityReport.IsOnEmptySideOfLedge = (flag3 && !flag2);
					stabilityReport.LedgeGroundNormal = (flag3 ? raycastHit2.normal : raycastHit.normal);
					stabilityReport.LedgeRightDirection = Vector3.Cross(hitNormal, raycastHit2.normal).normalized;
					stabilityReport.LedgeFacingDirection = Vector3.Cross(stabilityReport.LedgeGroundNormal, stabilityReport.LedgeRightDirection).normalized;
					stabilityReport.DistanceFromLedge = Vector3.ProjectOnPlane(hitPoint - (atCharacterPosition + atCharacterRotation * this.CharacterTransformToCapsuleBottom), vector).magnitude;
				}
			}
			if (flag || stabilityReport.ValidStepDetected)
			{
				stabilityReport.IsStable = true;
			}
			this.CharacterController.ProcessHitStabilityReport(hitCollider, hitNormal, hitPoint, atCharacterPosition, atCharacterRotation, ref stabilityReport);
		}

		// Token: 0x06003431 RID: 13361 RVA: 0x000E2F20 File Offset: 0x000E1120
		private void DetectSteps(Vector3 characterPosition, Quaternion characterRotation, Vector3 hitPoint, Vector3 innerHitDirection, ref HitStabilityReport stabilityReport)
		{
			Vector3 a = characterRotation * Vector3.up;
			Vector3 vector = characterPosition + a * this.MaxStepHeight + -innerHitDirection * this.CapsuleRadius;
			RaycastHit raycastHit;
			int nbStepHits = this.CharacterCollisionsSweep(vector, characterRotation, -a, this.MaxStepHeight - 0.001f, out raycastHit, this._internalCharacterHits, 0f);
			Collider steppedCollider;
			if (this.CheckStepValidity(nbStepHits, characterPosition, characterRotation, innerHitDirection, vector, out steppedCollider))
			{
				stabilityReport.ValidStepDetected = true;
				stabilityReport.SteppedCollider = steppedCollider;
			}
			if (this.StepHandling == StepHandlingMethod.Extra && !stabilityReport.ValidStepDetected)
			{
				vector = characterPosition + a * this.MaxStepHeight + -innerHitDirection * this.MinRequiredStepDepth;
				nbStepHits = this.CharacterCollisionsSweep(vector, characterRotation, -a, this.MaxStepHeight - 0.001f, out raycastHit, this._internalCharacterHits, 0f);
				if (this.CheckStepValidity(nbStepHits, characterPosition, characterRotation, innerHitDirection, vector, out steppedCollider))
				{
					stabilityReport.ValidStepDetected = true;
					stabilityReport.SteppedCollider = steppedCollider;
				}
			}
		}

		// Token: 0x06003432 RID: 13362 RVA: 0x000E303C File Offset: 0x000E123C
		private bool CheckStepValidity(int nbStepHits, Vector3 characterPosition, Quaternion characterRotation, Vector3 innerHitDirection, Vector3 stepCheckStartPos, out Collider hitCollider)
		{
			hitCollider = null;
			Vector3 vector = characterRotation * Vector3.up;
			bool flag = false;
			while (nbStepHits > 0 && !flag)
			{
				RaycastHit raycastHit = default(RaycastHit);
				float num = 0f;
				int num2 = 0;
				for (int i = 0; i < nbStepHits; i++)
				{
					if (this._internalCharacterHits[i].distance > num)
					{
						num = this._internalCharacterHits[i].distance;
						raycastHit = this._internalCharacterHits[i];
						num2 = i;
					}
				}
				Vector3 b = characterPosition + characterRotation * this.CharacterTransformToCapsuleBottom;
				float magnitude = Vector3.Project(raycastHit.point - b, vector).magnitude;
				Vector3 vector2 = stepCheckStartPos + -vector * (raycastHit.distance - 0.001f);
				RaycastHit raycastHit2;
				RaycastHit raycastHit3;
				if (magnitude <= this.MaxStepHeight && this.CharacterCollisionsOverlap(vector2, characterRotation, this._internalProbedColliders, 0f) <= 0 && this.CharacterCollisionsRaycast(raycastHit.point + vector * 0.02f + -innerHitDirection * 0.001f, -vector, this.MaxStepHeight + 0.02f, out raycastHit2, this._internalCharacterHits) > 0 && this.IsStableOnNormal(raycastHit2.normal) && this.CharacterCollisionsSweep(characterPosition, characterRotation, vector, this.MaxStepHeight - raycastHit.distance, out raycastHit3, this._internalCharacterHits, 0f) <= 0)
				{
					bool flag2 = false;
					RaycastHit raycastHit4;
					if (this.CharacterCollisionsRaycast(characterPosition + Vector3.Project(vector2 - characterPosition, vector), -vector, this.MaxStepHeight, out raycastHit4, this._internalCharacterHits) > 0 && this.IsStableOnNormal(raycastHit4.normal))
					{
						flag2 = true;
					}
					if (!flag2 && this.CharacterCollisionsRaycast(raycastHit.point + innerHitDirection * 0.001f, -vector, this.MaxStepHeight, out raycastHit4, this._internalCharacterHits) > 0 && this.IsStableOnNormal(raycastHit4.normal))
					{
						flag2 = true;
					}
					if (flag2)
					{
						hitCollider = raycastHit.collider;
						return true;
					}
				}
				if (!flag)
				{
					nbStepHits--;
					if (num2 < nbStepHits)
					{
						this._internalCharacterHits[num2] = this._internalCharacterHits[nbStepHits];
					}
				}
			}
			return false;
		}

		// Token: 0x06003433 RID: 13363 RVA: 0x000E3298 File Offset: 0x000E1498
		public Vector3 GetVelocityFromRigidbodyMovement(Rigidbody interactiveRigidbody, Vector3 atPoint, float deltaTime)
		{
			if (deltaTime > 0f)
			{
				Vector3 vector = interactiveRigidbody.velocity;
				if (interactiveRigidbody.angularVelocity != Vector3.zero)
				{
					Vector3 vector2 = interactiveRigidbody.position + interactiveRigidbody.centerOfMass;
					Vector3 point = atPoint - vector2;
					Quaternion rotation = Quaternion.Euler(57.29578f * interactiveRigidbody.angularVelocity * deltaTime);
					Vector3 a = vector2 + rotation * point;
					vector += (a - atPoint) / deltaTime;
				}
				return vector;
			}
			return Vector3.zero;
		}

		// Token: 0x06003434 RID: 13364 RVA: 0x000E3328 File Offset: 0x000E1528
		private Rigidbody GetInteractiveRigidbody(Collider onCollider)
		{
			if (onCollider.attachedRigidbody)
			{
				if (onCollider.attachedRigidbody.gameObject.GetComponent<PhysicsMover>())
				{
					return onCollider.attachedRigidbody;
				}
				if (!onCollider.attachedRigidbody.isKinematic)
				{
					return onCollider.attachedRigidbody;
				}
			}
			return null;
		}

		// Token: 0x06003435 RID: 13365 RVA: 0x000E3375 File Offset: 0x000E1575
		public Vector3 GetVelocityForMovePosition(Vector3 fromPosition, Vector3 toPosition, float deltaTime)
		{
			if (deltaTime > 0f)
			{
				return (toPosition - fromPosition) / deltaTime;
			}
			return Vector3.zero;
		}

		// Token: 0x06003436 RID: 13366 RVA: 0x000E3394 File Offset: 0x000E1594
		private void RestrictVectorToPlane(ref Vector3 vector, Vector3 toPlane)
		{
			if (vector.x > 0f != toPlane.x > 0f)
			{
				vector.x = 0f;
			}
			if (vector.y > 0f != toPlane.y > 0f)
			{
				vector.y = 0f;
			}
			if (vector.z > 0f != toPlane.z > 0f)
			{
				vector.z = 0f;
			}
		}

		// Token: 0x06003437 RID: 13367 RVA: 0x000E3418 File Offset: 0x000E1618
		public int CharacterCollisionsOverlap(Vector3 atPosition, Quaternion atRotation, Collider[] overlappedColliders, float radiusInflate = 0f)
		{
			int num;
			for (int i = (num = Physics.OverlapCapsuleNonAlloc(atPosition + atRotation * this.CharacterTransformToCapsuleBottomHemi, atPosition + atRotation * this.CharacterTransformToCapsuleTopHemi, this.Capsule.radius + radiusInflate, overlappedColliders, this.CollidableLayers, QueryTriggerInteraction.Ignore)) - 1; i >= 0; i--)
			{
				if (!this.CheckIfColliderValidForCollisions(overlappedColliders[i]))
				{
					num--;
					if (i < num)
					{
						overlappedColliders[i] = overlappedColliders[num];
					}
				}
			}
			return num;
		}

		// Token: 0x06003438 RID: 13368 RVA: 0x000E3494 File Offset: 0x000E1694
		public int CharacterOverlap(Vector3 atPosition, Quaternion atRotation, Collider[] overlappedColliders, LayerMask layers, QueryTriggerInteraction triggerInteraction, float radiusInflate = 0f)
		{
			int num;
			for (int i = (num = Physics.OverlapCapsuleNonAlloc(atPosition + atRotation * this.CharacterTransformToCapsuleBottomHemi, atPosition + atRotation * this.CharacterTransformToCapsuleTopHemi, this.Capsule.radius + radiusInflate, overlappedColliders, layers, triggerInteraction)) - 1; i >= 0; i--)
			{
				if (overlappedColliders[i] == this.Capsule)
				{
					num--;
					if (i < num)
					{
						overlappedColliders[i] = overlappedColliders[num];
					}
				}
			}
			return num;
		}

		// Token: 0x06003439 RID: 13369 RVA: 0x000E3514 File Offset: 0x000E1714
		public int CharacterCollisionsSweep(Vector3 position, Quaternion rotation, Vector3 direction, float distance, out RaycastHit closestHit, RaycastHit[] hits, float radiusInflate = 0f)
		{
			direction.Normalize();
			int num = Physics.CapsuleCastNonAlloc(position + rotation * this.CharacterTransformToCapsuleBottomHemi - direction * 0.002f, position + rotation * this.CharacterTransformToCapsuleTopHemi - direction * 0.002f, this.Capsule.radius + radiusInflate, direction, hits, distance + 0.002f, this.CollidableLayers, QueryTriggerInteraction.Ignore);
			closestHit = default(RaycastHit);
			float num2 = float.PositiveInfinity;
			int num3 = num;
			for (int i = num - 1; i >= 0; i--)
			{
				int num4 = i;
				hits[num4].distance = hits[num4].distance - 0.002f;
				if (hits[i].distance <= 0f || !this.CheckIfColliderValidForCollisions(hits[i].collider))
				{
					num3--;
					if (i < num3)
					{
						hits[i] = hits[num3];
					}
				}
				else if (hits[i].distance < num2)
				{
					closestHit = hits[i];
					num2 = hits[i].distance;
				}
			}
			return num3;
		}

		// Token: 0x0600343A RID: 13370 RVA: 0x000E3648 File Offset: 0x000E1848
		public int CharacterSweep(Vector3 position, Quaternion rotation, Vector3 direction, float distance, out RaycastHit closestHit, RaycastHit[] hits, LayerMask layers, QueryTriggerInteraction triggerInteraction, float radiusInflate = 0f)
		{
			direction.Normalize();
			closestHit = default(RaycastHit);
			int num = Physics.CapsuleCastNonAlloc(position + rotation * this.CharacterTransformToCapsuleBottomHemi, position + rotation * this.CharacterTransformToCapsuleTopHemi, this.Capsule.radius + radiusInflate, direction, hits, distance, layers, triggerInteraction);
			float num2 = float.PositiveInfinity;
			int num3 = num;
			for (int i = num - 1; i >= 0; i--)
			{
				if (hits[i].distance <= 0f || hits[i].collider == this.Capsule)
				{
					num3--;
					if (i < num3)
					{
						hits[i] = hits[num3];
					}
				}
				else if (hits[i].distance < num2)
				{
					closestHit = hits[i];
					num2 = hits[i].distance;
				}
			}
			return num3;
		}

		// Token: 0x0600343B RID: 13371 RVA: 0x000E3738 File Offset: 0x000E1938
		private bool CharacterGroundSweep(Vector3 position, Quaternion rotation, Vector3 direction, float distance, out RaycastHit closestHit)
		{
			direction.Normalize();
			closestHit = default(RaycastHit);
			int num = Physics.CapsuleCastNonAlloc(position + rotation * this.CharacterTransformToCapsuleBottomHemi - direction * 0.1f, position + rotation * this.CharacterTransformToCapsuleTopHemi - direction * 0.1f, this.Capsule.radius, direction, this._internalCharacterHits, distance + 0.1f, this.CollidableLayers, QueryTriggerInteraction.Ignore);
			bool result = false;
			float num2 = float.PositiveInfinity;
			for (int i = 0; i < num; i++)
			{
				if (this._internalCharacterHits[i].distance > 0f && this.CheckIfColliderValidForCollisions(this._internalCharacterHits[i].collider) && this._internalCharacterHits[i].distance < num2)
				{
					closestHit = this._internalCharacterHits[i];
					closestHit.distance -= 0.1f;
					num2 = this._internalCharacterHits[i].distance;
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600343C RID: 13372 RVA: 0x000E3860 File Offset: 0x000E1A60
		public int CharacterCollisionsRaycast(Vector3 position, Vector3 direction, float distance, out RaycastHit closestHit, RaycastHit[] hits)
		{
			direction.Normalize();
			int num = Physics.RaycastNonAlloc(position, direction, hits, distance, this.CollidableLayers, QueryTriggerInteraction.Ignore);
			closestHit = default(RaycastHit);
			float num2 = float.PositiveInfinity;
			int num3 = num;
			for (int i = num - 1; i >= 0; i--)
			{
				if (hits[i].distance <= 0f || !this.CheckIfColliderValidForCollisions(hits[i].collider))
				{
					num3--;
					if (i < num3)
					{
						hits[i] = hits[num3];
					}
				}
				else if (hits[i].distance < num2)
				{
					closestHit = hits[i];
					num2 = hits[i].distance;
				}
			}
			return num3;
		}

		// Token: 0x04003379 RID: 13177
		[Header("Components")]
		public BaseCharacterController CharacterController;

		// Token: 0x0400337A RID: 13178
		[ReadOnly]
		public CapsuleCollider Capsule;

		// Token: 0x0400337B RID: 13179
		[ReadOnly]
		public Rigidbody Rigidbody;

		// Token: 0x0400337C RID: 13180
		[SerializeField]
		[Tooltip("Radius of the Character Capsule")]
		[Header("Capsule Settings")]
		private float CapsuleRadius = 0.5f;

		// Token: 0x0400337D RID: 13181
		[SerializeField]
		[Tooltip("Height of the Character Capsule")]
		private float CapsuleHeight = 2f;

		// Token: 0x0400337E RID: 13182
		[SerializeField]
		[Tooltip("Height of the Character Capsule")]
		private float CapsuleYOffset = 1f;

		// Token: 0x0400337F RID: 13183
		[SerializeField]
		[Tooltip("Physics material of the Character Capsule (Does not affect character movement. Only affects things colliding with it)")]
		private PhysicMaterial CapsulePhysicsMaterial;

		// Token: 0x04003380 RID: 13184
		[Header("Misc Options")]
		[Tooltip("Notifies the Character Controller when discrete collisions are detected")]
		public bool DetectDiscreteCollisions;

		// Token: 0x04003381 RID: 13185
		[Tooltip("Increases the range of ground detection, to allow snapping to ground at very high speeds")]
		public float GroundDetectionExtraDistance;

		// Token: 0x04003382 RID: 13186
		[Tooltip("Maximum height of a step which the character can climb")]
		public float MaxStepHeight = 0.5f;

		// Token: 0x04003383 RID: 13187
		[Tooltip("Minimum length of a step that the character can step on (used in Extra stepping method). Use this to let the character step on steps that are smaller that its radius")]
		public float MinRequiredStepDepth = 0.1f;

		// Token: 0x04003384 RID: 13188
		[Range(0f, 89f)]
		[Tooltip("Maximum slope angle on which the character can be stable")]
		public float MaxStableSlopeAngle = 60f;

		// Token: 0x04003385 RID: 13189
		[Tooltip("The distance from the capsule central axis at which the character can stand on a ledge and still be stable")]
		public float MaxStableDistanceFromLedge = 0.5f;

		// Token: 0x04003386 RID: 13190
		[Tooltip("Prevents snapping to ground on ledges. Set this to true if you want more determinism when launching off slopes")]
		public bool PreventSnappingOnLedges;

		// Token: 0x04003387 RID: 13191
		[Tooltip("The maximun downward slope angle change that the character can be subjected to and still be snapping to the ground")]
		[Range(1f, 180f)]
		public float MaxStableDenivelationAngle = 180f;

		// Token: 0x04003388 RID: 13192
		[Tooltip("How the character interacts with non-kinematic rigidbodies. \"Kinematic\" mode means the character pushes the rigidbodies with infinite force (as a kinematic body would). \"SimulatedDynamic\" pushes the rigidbodies with a simulated mass value.")]
		[Header("Rigidbody interactions")]
		public RigidbodyInteractionType RigidbodyInteractionType;

		// Token: 0x04003389 RID: 13193
		[Tooltip("Determines if the character preserves moving platform velocities when de-grounding from them")]
		public bool PreserveAttachedRigidbodyMomentum = true;

		// Token: 0x0400338A RID: 13194
		[Tooltip("Determines if the character's movement uses the planar constraint")]
		[Header("Constraints")]
		public bool HasPlanarConstraint;

		// Token: 0x0400338B RID: 13195
		[Tooltip("Defines the plane that the character's movement is constrained on, if HasMovementConstraintPlane is active")]
		public Vector3 PlanarConstraintAxis = Vector3.forward;

		// Token: 0x0400338C RID: 13196
		[Header("Features & Optimizations")]
		[Tooltip("Handles properly detecting grounding status on steps, but has a performance cost.")]
		public StepHandlingMethod StepHandling = StepHandlingMethod.Standard;

		// Token: 0x0400338D RID: 13197
		[Tooltip("Handles properly detecting ledge information and grounding status, but has a performance cost.")]
		public bool LedgeHandling = true;

		// Token: 0x0400338E RID: 13198
		[Tooltip("Handles properly being pushed by and standing on PhysicsMovers or dynamic rigidbodies. Also handles pushing dynamic rigidbodies")]
		public bool InteractiveRigidbodyHandling = true;

		// Token: 0x0400338F RID: 13199
		[Tooltip("(We suggest leaving this off. This has a pretty heavy performance cost, and is not necessary unless you start seeing situations where a fast-moving character moves through colliders) Makes sure the character cannot perform a move at all if it would be overlapping with any collidable objects at its destination. Useful for preventing \"tunneling\". ")]
		public bool SafeMovement = true;

		// Token: 0x04003390 RID: 13200
		[NonSerialized]
		public CharacterGroundingReport GroundingStatus;

		// Token: 0x04003391 RID: 13201
		[NonSerialized]
		public CharacterTransientGroundingReport LastGroundingStatus;

		// Token: 0x04003392 RID: 13202
		[NonSerialized]
		public LayerMask CollidableLayers = -1;

		// Token: 0x040033A5 RID: 13221
		private RaycastHit[] _internalCharacterHits = new RaycastHit[16];

		// Token: 0x040033A6 RID: 13222
		private Collider[] _internalProbedColliders = new Collider[16];

		// Token: 0x040033A7 RID: 13223
		private Rigidbody[] _rigidbodiesPushedThisMove = new Rigidbody[16];

		// Token: 0x040033A8 RID: 13224
		private RigidbodyProjectionHit[] _internalRigidbodyProjectionHits = new RigidbodyProjectionHit[6];

		// Token: 0x040033A9 RID: 13225
		private Rigidbody _lastAttachedRigidbody;

		// Token: 0x040033AA RID: 13226
		private bool _solveMovementCollisions = true;

		// Token: 0x040033AB RID: 13227
		private bool _solveGrounding = true;

		// Token: 0x040033AC RID: 13228
		private bool _movePositionDirty;

		// Token: 0x040033AD RID: 13229
		private Vector3 _movePositionTarget = Vector3.zero;

		// Token: 0x040033AE RID: 13230
		private bool _moveRotationDirty;

		// Token: 0x040033AF RID: 13231
		private Quaternion _moveRotationTarget = Quaternion.identity;

		// Token: 0x040033B0 RID: 13232
		private bool _lastSolvedOverlapNormalDirty;

		// Token: 0x040033B1 RID: 13233
		private Vector3 _lastSolvedOverlapNormal = Vector3.forward;

		// Token: 0x040033B2 RID: 13234
		private int _rigidbodiesPushedCount;

		// Token: 0x040033B3 RID: 13235
		private int _rigidbodyProjectionHitCount;

		// Token: 0x040033B4 RID: 13236
		private float _internalResultingMovementMagnitude;

		// Token: 0x040033B5 RID: 13237
		private Vector3 _internalResultingMovementDirection = Vector3.zero;

		// Token: 0x040033B6 RID: 13238
		private bool _isMovingFromAttachedRigidbody;

		// Token: 0x040033B7 RID: 13239
		private Vector3 _cachedWorldUp = Vector3.up;

		// Token: 0x040033B8 RID: 13240
		private Vector3 _cachedWorldForward = Vector3.forward;

		// Token: 0x040033B9 RID: 13241
		private Vector3 _cachedWorldRight = Vector3.right;

		// Token: 0x040033BA RID: 13242
		private Vector3 _cachedZeroVector = Vector3.zero;

		// Token: 0x040033BB RID: 13243
		private Vector3 _internalTransientPosition;

		// Token: 0x040033BC RID: 13244
		private Quaternion _internalTransientRotation;

		// Token: 0x040033BD RID: 13245
		private Vector3 _baseVelocity;

		// Token: 0x040033BE RID: 13246
		private Vector3 _attachedRigidbodyVelocity;

		// Token: 0x040033C0 RID: 13248
		private OverlapResult[] _overlaps = new OverlapResult[16];

		// Token: 0x040033C1 RID: 13249
		public const int MaxHitsBudget = 16;

		// Token: 0x040033C2 RID: 13250
		public const int MaxCollisionBudget = 16;

		// Token: 0x040033C3 RID: 13251
		public const int MaxGroundingSweepIterations = 2;

		// Token: 0x040033C4 RID: 13252
		public const int MaxMovementSweepIterations = 6;

		// Token: 0x040033C5 RID: 13253
		public const int MaxSteppingSweepIterations = 3;

		// Token: 0x040033C6 RID: 13254
		public const int MaxRigidbodyOverlapsCount = 16;

		// Token: 0x040033C7 RID: 13255
		public const int MaxDiscreteCollisionIterations = 3;

		// Token: 0x040033C8 RID: 13256
		public const float CollisionOffset = 0.001f;

		// Token: 0x040033C9 RID: 13257
		public const float GroundProbeReboundDistance = 0.02f;

		// Token: 0x040033CA RID: 13258
		public const float MinimumGroundProbingDistance = 0.005f;

		// Token: 0x040033CB RID: 13259
		public const float GroundProbingBackstepDistance = 0.1f;

		// Token: 0x040033CC RID: 13260
		public const float SweepProbingBackstepDistance = 0.002f;

		// Token: 0x040033CD RID: 13261
		public const float SecondaryProbesVertical = 0.02f;

		// Token: 0x040033CE RID: 13262
		public const float SecondaryProbesHorizontal = 0.001f;

		// Token: 0x040033CF RID: 13263
		public const float MinVelocityMagnitude = 0.01f;

		// Token: 0x040033D0 RID: 13264
		public const float SteppingForwardDistance = 0.03f;

		// Token: 0x040033D1 RID: 13265
		public const float MinDistanceForLedge = 0.05f;

		// Token: 0x040033D2 RID: 13266
		public const float CorrelationForVerticalObstruction = 0.01f;

		// Token: 0x040033D3 RID: 13267
		public const float ExtraSteppingForwardDistance = 0.01f;

		// Token: 0x040033D4 RID: 13268
		public const float ExtraStepHeightPadding = 0.01f;
	}
}
