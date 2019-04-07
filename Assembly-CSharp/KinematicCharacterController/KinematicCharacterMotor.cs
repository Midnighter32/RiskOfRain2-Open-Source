using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x020006CC RID: 1740
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Rigidbody))]
	public class KinematicCharacterMotor : MonoBehaviour
	{
		// Token: 0x1700033C RID: 828
		// (get) Token: 0x06002693 RID: 9875 RVA: 0x000B19A9 File Offset: 0x000AFBA9
		// (set) Token: 0x06002694 RID: 9876 RVA: 0x000B19B1 File Offset: 0x000AFBB1
		public Transform Transform { get; private set; }

		// Token: 0x1700033D RID: 829
		// (get) Token: 0x06002695 RID: 9877 RVA: 0x000B19BA File Offset: 0x000AFBBA
		// (set) Token: 0x06002696 RID: 9878 RVA: 0x000B19C2 File Offset: 0x000AFBC2
		public Vector3 CharacterUp { get; private set; }

		// Token: 0x1700033E RID: 830
		// (get) Token: 0x06002697 RID: 9879 RVA: 0x000B19CB File Offset: 0x000AFBCB
		// (set) Token: 0x06002698 RID: 9880 RVA: 0x000B19D3 File Offset: 0x000AFBD3
		public Vector3 CharacterForward { get; private set; }

		// Token: 0x1700033F RID: 831
		// (get) Token: 0x06002699 RID: 9881 RVA: 0x000B19DC File Offset: 0x000AFBDC
		// (set) Token: 0x0600269A RID: 9882 RVA: 0x000B19E4 File Offset: 0x000AFBE4
		public Vector3 CharacterRight { get; private set; }

		// Token: 0x17000340 RID: 832
		// (get) Token: 0x0600269B RID: 9883 RVA: 0x000B19ED File Offset: 0x000AFBED
		// (set) Token: 0x0600269C RID: 9884 RVA: 0x000B19F5 File Offset: 0x000AFBF5
		public Vector3 InitialSimulationPosition { get; private set; }

		// Token: 0x17000341 RID: 833
		// (get) Token: 0x0600269D RID: 9885 RVA: 0x000B19FE File Offset: 0x000AFBFE
		// (set) Token: 0x0600269E RID: 9886 RVA: 0x000B1A06 File Offset: 0x000AFC06
		public Quaternion InitialSimulationRotation { get; private set; }

		// Token: 0x17000342 RID: 834
		// (get) Token: 0x0600269F RID: 9887 RVA: 0x000B1A0F File Offset: 0x000AFC0F
		// (set) Token: 0x060026A0 RID: 9888 RVA: 0x000B1A17 File Offset: 0x000AFC17
		public Rigidbody AttachedRigidbody { get; private set; }

		// Token: 0x17000343 RID: 835
		// (get) Token: 0x060026A1 RID: 9889 RVA: 0x000B1A20 File Offset: 0x000AFC20
		// (set) Token: 0x060026A2 RID: 9890 RVA: 0x000B1A28 File Offset: 0x000AFC28
		public Vector3 CharacterTransformToCapsuleCenter { get; private set; }

		// Token: 0x17000344 RID: 836
		// (get) Token: 0x060026A3 RID: 9891 RVA: 0x000B1A31 File Offset: 0x000AFC31
		// (set) Token: 0x060026A4 RID: 9892 RVA: 0x000B1A39 File Offset: 0x000AFC39
		public Vector3 CharacterTransformToCapsuleBottom { get; private set; }

		// Token: 0x17000345 RID: 837
		// (get) Token: 0x060026A5 RID: 9893 RVA: 0x000B1A42 File Offset: 0x000AFC42
		// (set) Token: 0x060026A6 RID: 9894 RVA: 0x000B1A4A File Offset: 0x000AFC4A
		public Vector3 CharacterTransformToCapsuleTop { get; private set; }

		// Token: 0x17000346 RID: 838
		// (get) Token: 0x060026A7 RID: 9895 RVA: 0x000B1A53 File Offset: 0x000AFC53
		// (set) Token: 0x060026A8 RID: 9896 RVA: 0x000B1A5B File Offset: 0x000AFC5B
		public Vector3 CharacterTransformToCapsuleBottomHemi { get; private set; }

		// Token: 0x17000347 RID: 839
		// (get) Token: 0x060026A9 RID: 9897 RVA: 0x000B1A64 File Offset: 0x000AFC64
		// (set) Token: 0x060026AA RID: 9898 RVA: 0x000B1A6C File Offset: 0x000AFC6C
		public Vector3 CharacterTransformToCapsuleTopHemi { get; private set; }

		// Token: 0x17000348 RID: 840
		// (get) Token: 0x060026AB RID: 9899 RVA: 0x000B1A75 File Offset: 0x000AFC75
		// (set) Token: 0x060026AC RID: 9900 RVA: 0x000B1A7D File Offset: 0x000AFC7D
		public bool MustUnground { get; set; }

		// Token: 0x17000349 RID: 841
		// (get) Token: 0x060026AD RID: 9901 RVA: 0x000B1A86 File Offset: 0x000AFC86
		// (set) Token: 0x060026AE RID: 9902 RVA: 0x000B1A8E File Offset: 0x000AFC8E
		public bool LastMovementIterationFoundAnyGround { get; set; }

		// Token: 0x1700034A RID: 842
		// (get) Token: 0x060026AF RID: 9903 RVA: 0x000B1A97 File Offset: 0x000AFC97
		// (set) Token: 0x060026B0 RID: 9904 RVA: 0x000B1A9F File Offset: 0x000AFC9F
		public int IndexInCharacterSystem { get; set; }

		// Token: 0x1700034B RID: 843
		// (get) Token: 0x060026B1 RID: 9905 RVA: 0x000B1AA8 File Offset: 0x000AFCA8
		// (set) Token: 0x060026B2 RID: 9906 RVA: 0x000B1AB0 File Offset: 0x000AFCB0
		public Vector3 InitialTickPosition { get; set; }

		// Token: 0x1700034C RID: 844
		// (get) Token: 0x060026B3 RID: 9907 RVA: 0x000B1AB9 File Offset: 0x000AFCB9
		// (set) Token: 0x060026B4 RID: 9908 RVA: 0x000B1AC1 File Offset: 0x000AFCC1
		public Quaternion InitialTickRotation { get; set; }

		// Token: 0x1700034D RID: 845
		// (get) Token: 0x060026B5 RID: 9909 RVA: 0x000B1ACA File Offset: 0x000AFCCA
		// (set) Token: 0x060026B6 RID: 9910 RVA: 0x000B1AD2 File Offset: 0x000AFCD2
		public Rigidbody AttachedRigidbodyOverride { get; set; }

		// Token: 0x1700034E RID: 846
		// (get) Token: 0x060026B7 RID: 9911 RVA: 0x000B1ADB File Offset: 0x000AFCDB
		// (set) Token: 0x060026B8 RID: 9912 RVA: 0x000B1AE3 File Offset: 0x000AFCE3
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

		// Token: 0x1700034F RID: 847
		// (get) Token: 0x060026B9 RID: 9913 RVA: 0x000B1AEC File Offset: 0x000AFCEC
		// (set) Token: 0x060026BA RID: 9914 RVA: 0x000B1AF4 File Offset: 0x000AFCF4
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

		// Token: 0x17000350 RID: 848
		// (get) Token: 0x060026BB RID: 9915 RVA: 0x000B1B4D File Offset: 0x000AFD4D
		public Vector3 InterpolatedPosition
		{
			get
			{
				return this.Transform.position;
			}
		}

		// Token: 0x17000351 RID: 849
		// (get) Token: 0x060026BC RID: 9916 RVA: 0x000B1B5A File Offset: 0x000AFD5A
		public Quaternion InterpolatedRotation
		{
			get
			{
				return this.Transform.rotation;
			}
		}

		// Token: 0x17000352 RID: 850
		// (get) Token: 0x060026BD RID: 9917 RVA: 0x000B1B67 File Offset: 0x000AFD67
		public Vector3 Velocity
		{
			get
			{
				return this._baseVelocity + this._attachedRigidbodyVelocity;
			}
		}

		// Token: 0x17000353 RID: 851
		// (get) Token: 0x060026BE RID: 9918 RVA: 0x000B1B7A File Offset: 0x000AFD7A
		// (set) Token: 0x060026BF RID: 9919 RVA: 0x000B1B82 File Offset: 0x000AFD82
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

		// Token: 0x17000354 RID: 852
		// (get) Token: 0x060026C0 RID: 9920 RVA: 0x000B1B8B File Offset: 0x000AFD8B
		// (set) Token: 0x060026C1 RID: 9921 RVA: 0x000B1B93 File Offset: 0x000AFD93
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

		// Token: 0x17000355 RID: 853
		// (get) Token: 0x060026C2 RID: 9922 RVA: 0x000B1B9C File Offset: 0x000AFD9C
		// (set) Token: 0x060026C3 RID: 9923 RVA: 0x000B1BA4 File Offset: 0x000AFDA4
		public int OverlapsCount { get; private set; }

		// Token: 0x17000356 RID: 854
		// (get) Token: 0x060026C4 RID: 9924 RVA: 0x000B1BAD File Offset: 0x000AFDAD
		public OverlapResult[] Overlaps
		{
			get
			{
				return this._overlaps;
			}
		}

		// Token: 0x060026C5 RID: 9925 RVA: 0x000B1BB5 File Offset: 0x000AFDB5
		private void OnEnable()
		{
			KinematicCharacterSystem.EnsureCreation();
			KinematicCharacterSystem.RegisterCharacterMotor(this);
		}

		// Token: 0x060026C6 RID: 9926 RVA: 0x000B1BC2 File Offset: 0x000AFDC2
		private void OnDisable()
		{
			KinematicCharacterSystem.UnregisterCharacterMotor(this);
		}

		// Token: 0x060026C7 RID: 9927 RVA: 0x000B1BCA File Offset: 0x000AFDCA
		private void Reset()
		{
			this.ValidateData();
		}

		// Token: 0x060026C8 RID: 9928 RVA: 0x00004507 File Offset: 0x00002707
		private void OnValidate()
		{
		}

		// Token: 0x060026C9 RID: 9929 RVA: 0x000B1BD4 File Offset: 0x000AFDD4
		[ContextMenu("Remove Component")]
		private void HandleRemoveComponent()
		{
			Rigidbody component = base.gameObject.GetComponent<Rigidbody>();
			UnityEngine.Object component2 = base.gameObject.GetComponent<CapsuleCollider>();
			UnityEngine.Object.DestroyImmediate(this);
			UnityEngine.Object.DestroyImmediate(component);
			UnityEngine.Object.DestroyImmediate(component2);
		}

		// Token: 0x060026CA RID: 9930 RVA: 0x000B1C0C File Offset: 0x000AFE0C
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

		// Token: 0x060026CB RID: 9931 RVA: 0x000B1D8E File Offset: 0x000AFF8E
		public void SetCapsuleCollisionsActivation(bool kinematicCapsuleActive)
		{
			this.Rigidbody.detectCollisions = kinematicCapsuleActive;
		}

		// Token: 0x060026CC RID: 9932 RVA: 0x000B1D9C File Offset: 0x000AFF9C
		public void SetMovementCollisionsSolvingActivation(bool movementCollisionsSolvingActive)
		{
			this._solveMovementCollisions = movementCollisionsSolvingActive;
		}

		// Token: 0x060026CD RID: 9933 RVA: 0x000B1DA5 File Offset: 0x000AFFA5
		public void SetGroundSolvingActivation(bool stabilitySolvingActive)
		{
			this._solveGrounding = stabilitySolvingActive;
		}

		// Token: 0x060026CE RID: 9934 RVA: 0x000B1DB0 File Offset: 0x000AFFB0
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

		// Token: 0x060026CF RID: 9935 RVA: 0x000B1E10 File Offset: 0x000B0010
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

		// Token: 0x060026D0 RID: 9936 RVA: 0x000B1E70 File Offset: 0x000B0070
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

		// Token: 0x060026D1 RID: 9937 RVA: 0x000B1EF2 File Offset: 0x000B00F2
		public void MoveCharacter(Vector3 toPosition)
		{
			this._movePositionDirty = true;
			this._movePositionTarget = toPosition;
		}

		// Token: 0x060026D2 RID: 9938 RVA: 0x000B1F02 File Offset: 0x000B0102
		public void RotateCharacter(Quaternion toRotation)
		{
			this._moveRotationDirty = true;
			this._moveRotationTarget = toRotation;
		}

		// Token: 0x060026D3 RID: 9939 RVA: 0x000B1F14 File Offset: 0x000B0114
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

		// Token: 0x060026D4 RID: 9940 RVA: 0x000B1F98 File Offset: 0x000B0198
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

		// Token: 0x060026D5 RID: 9941 RVA: 0x000B2008 File Offset: 0x000B0208
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

		// Token: 0x060026D6 RID: 9942 RVA: 0x000B21A4 File Offset: 0x000B03A4
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

		// Token: 0x060026D7 RID: 9943 RVA: 0x000B2240 File Offset: 0x000B0440
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

		// Token: 0x060026D8 RID: 9944 RVA: 0x000B2298 File Offset: 0x000B0498
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

		// Token: 0x060026D9 RID: 9945 RVA: 0x000B28E4 File Offset: 0x000B0AE4
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

		// Token: 0x060026DA RID: 9946 RVA: 0x000B2DD7 File Offset: 0x000B0FD7
		private bool IsStableOnNormal(Vector3 normal)
		{
			return Vector3.Angle(this.CharacterUp, normal) <= this.MaxStableSlopeAngle;
		}

		// Token: 0x060026DB RID: 9947 RVA: 0x000B2DF0 File Offset: 0x000B0FF0
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

		// Token: 0x060026DC RID: 9948 RVA: 0x000B30BA File Offset: 0x000B12BA
		public void ForceUnground()
		{
			this.MustUnground = true;
		}

		// Token: 0x060026DD RID: 9949 RVA: 0x000B30C4 File Offset: 0x000B12C4
		public Vector3 GetDirectionTangentToSurface(Vector3 direction, Vector3 surfaceNormal)
		{
			Vector3 rhs = Vector3.Cross(direction, this.CharacterUp);
			return Vector3.Cross(surfaceNormal, rhs).normalized;
		}

		// Token: 0x060026DE RID: 9950 RVA: 0x000B30F0 File Offset: 0x000B12F0
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

		// Token: 0x060026DF RID: 9951 RVA: 0x000B34DC File Offset: 0x000B16DC
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

		// Token: 0x060026E0 RID: 9952 RVA: 0x000B3548 File Offset: 0x000B1748
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

		// Token: 0x060026E1 RID: 9953 RVA: 0x000B35CC File Offset: 0x000B17CC
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

		// Token: 0x060026E2 RID: 9954 RVA: 0x000B36C8 File Offset: 0x000B18C8
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

		// Token: 0x060026E3 RID: 9955 RVA: 0x000B3704 File Offset: 0x000B1904
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

		// Token: 0x060026E4 RID: 9956 RVA: 0x000B37F9 File Offset: 0x000B19F9
		private bool CheckIfColliderValidForCollisions(Collider coll)
		{
			return !(coll == null) && !(coll == this.Capsule) && this.IsColliderValidForCollisions(coll);
		}

		// Token: 0x060026E5 RID: 9957 RVA: 0x000B3820 File Offset: 0x000B1A20
		private bool IsColliderValidForCollisions(Collider coll)
		{
			return ((!this._isMovingFromAttachedRigidbody && this.RigidbodyInteractionType != RigidbodyInteractionType.Kinematic) || !coll.attachedRigidbody || coll.attachedRigidbody.isKinematic) && (!this._isMovingFromAttachedRigidbody || !(coll.attachedRigidbody == this.AttachedRigidbody)) && this.CharacterController.IsColliderValidForCollisions(coll);
		}

		// Token: 0x060026E6 RID: 9958 RVA: 0x000B3888 File Offset: 0x000B1A88
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

		// Token: 0x060026E7 RID: 9959 RVA: 0x000B3AD8 File Offset: 0x000B1CD8
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

		// Token: 0x060026E8 RID: 9960 RVA: 0x000B3BF4 File Offset: 0x000B1DF4
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

		// Token: 0x060026E9 RID: 9961 RVA: 0x000B3E50 File Offset: 0x000B2050
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

		// Token: 0x060026EA RID: 9962 RVA: 0x000B3EE0 File Offset: 0x000B20E0
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

		// Token: 0x060026EB RID: 9963 RVA: 0x000B3F2D File Offset: 0x000B212D
		public Vector3 GetVelocityForMovePosition(Vector3 fromPosition, Vector3 toPosition, float deltaTime)
		{
			if (deltaTime > 0f)
			{
				return (toPosition - fromPosition) / deltaTime;
			}
			return Vector3.zero;
		}

		// Token: 0x060026EC RID: 9964 RVA: 0x000B3F4C File Offset: 0x000B214C
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

		// Token: 0x060026ED RID: 9965 RVA: 0x000B3FD0 File Offset: 0x000B21D0
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

		// Token: 0x060026EE RID: 9966 RVA: 0x000B404C File Offset: 0x000B224C
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

		// Token: 0x060026EF RID: 9967 RVA: 0x000B40CC File Offset: 0x000B22CC
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

		// Token: 0x060026F0 RID: 9968 RVA: 0x000B4200 File Offset: 0x000B2400
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

		// Token: 0x060026F1 RID: 9969 RVA: 0x000B42F0 File Offset: 0x000B24F0
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

		// Token: 0x060026F2 RID: 9970 RVA: 0x000B4418 File Offset: 0x000B2618
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

		// Token: 0x040028E0 RID: 10464
		[Header("Components")]
		public BaseCharacterController CharacterController;

		// Token: 0x040028E1 RID: 10465
		[ReadOnly]
		public CapsuleCollider Capsule;

		// Token: 0x040028E2 RID: 10466
		[ReadOnly]
		public Rigidbody Rigidbody;

		// Token: 0x040028E3 RID: 10467
		[Header("Capsule Settings")]
		[SerializeField]
		[Tooltip("Radius of the Character Capsule")]
		private float CapsuleRadius = 0.5f;

		// Token: 0x040028E4 RID: 10468
		[SerializeField]
		[Tooltip("Height of the Character Capsule")]
		private float CapsuleHeight = 2f;

		// Token: 0x040028E5 RID: 10469
		[SerializeField]
		[Tooltip("Height of the Character Capsule")]
		private float CapsuleYOffset = 1f;

		// Token: 0x040028E6 RID: 10470
		[SerializeField]
		[Tooltip("Physics material of the Character Capsule (Does not affect character movement. Only affects things colliding with it)")]
		private PhysicMaterial CapsulePhysicsMaterial;

		// Token: 0x040028E7 RID: 10471
		[Header("Misc Options")]
		[Tooltip("Notifies the Character Controller when discrete collisions are detected")]
		public bool DetectDiscreteCollisions;

		// Token: 0x040028E8 RID: 10472
		[Tooltip("Increases the range of ground detection, to allow snapping to ground at very high speeds")]
		public float GroundDetectionExtraDistance;

		// Token: 0x040028E9 RID: 10473
		[Tooltip("Maximum height of a step which the character can climb")]
		public float MaxStepHeight = 0.5f;

		// Token: 0x040028EA RID: 10474
		[Tooltip("Minimum length of a step that the character can step on (used in Extra stepping method). Use this to let the character step on steps that are smaller that its radius")]
		public float MinRequiredStepDepth = 0.1f;

		// Token: 0x040028EB RID: 10475
		[Tooltip("Maximum slope angle on which the character can be stable")]
		[Range(0f, 89f)]
		public float MaxStableSlopeAngle = 60f;

		// Token: 0x040028EC RID: 10476
		[Tooltip("The distance from the capsule central axis at which the character can stand on a ledge and still be stable")]
		public float MaxStableDistanceFromLedge = 0.5f;

		// Token: 0x040028ED RID: 10477
		[Tooltip("Prevents snapping to ground on ledges. Set this to true if you want more determinism when launching off slopes")]
		public bool PreventSnappingOnLedges;

		// Token: 0x040028EE RID: 10478
		[Tooltip("The maximun downward slope angle change that the character can be subjected to and still be snapping to the ground")]
		[Range(1f, 180f)]
		public float MaxStableDenivelationAngle = 180f;

		// Token: 0x040028EF RID: 10479
		[Tooltip("How the character interacts with non-kinematic rigidbodies. \"Kinematic\" mode means the character pushes the rigidbodies with infinite force (as a kinematic body would). \"SimulatedDynamic\" pushes the rigidbodies with a simulated mass value.")]
		[Header("Rigidbody interactions")]
		public RigidbodyInteractionType RigidbodyInteractionType;

		// Token: 0x040028F0 RID: 10480
		[Tooltip("Determines if the character preserves moving platform velocities when de-grounding from them")]
		public bool PreserveAttachedRigidbodyMomentum = true;

		// Token: 0x040028F1 RID: 10481
		[Tooltip("Determines if the character's movement uses the planar constraint")]
		[Header("Constraints")]
		public bool HasPlanarConstraint;

		// Token: 0x040028F2 RID: 10482
		[Tooltip("Defines the plane that the character's movement is constrained on, if HasMovementConstraintPlane is active")]
		public Vector3 PlanarConstraintAxis = Vector3.forward;

		// Token: 0x040028F3 RID: 10483
		[Tooltip("Handles properly detecting grounding status on steps, but has a performance cost.")]
		[Header("Features & Optimizations")]
		public StepHandlingMethod StepHandling = StepHandlingMethod.Standard;

		// Token: 0x040028F4 RID: 10484
		[Tooltip("Handles properly detecting ledge information and grounding status, but has a performance cost.")]
		public bool LedgeHandling = true;

		// Token: 0x040028F5 RID: 10485
		[Tooltip("Handles properly being pushed by and standing on PhysicsMovers or dynamic rigidbodies. Also handles pushing dynamic rigidbodies")]
		public bool InteractiveRigidbodyHandling = true;

		// Token: 0x040028F6 RID: 10486
		[Tooltip("(We suggest leaving this off. This has a pretty heavy performance cost, and is not necessary unless you start seeing situations where a fast-moving character moves through colliders) Makes sure the character cannot perform a move at all if it would be overlapping with any collidable objects at its destination. Useful for preventing \"tunneling\". ")]
		public bool SafeMovement = true;

		// Token: 0x040028F7 RID: 10487
		[NonSerialized]
		public CharacterGroundingReport GroundingStatus;

		// Token: 0x040028F8 RID: 10488
		[NonSerialized]
		public CharacterTransientGroundingReport LastGroundingStatus;

		// Token: 0x040028F9 RID: 10489
		[NonSerialized]
		public LayerMask CollidableLayers = -1;

		// Token: 0x0400290C RID: 10508
		private RaycastHit[] _internalCharacterHits = new RaycastHit[16];

		// Token: 0x0400290D RID: 10509
		private Collider[] _internalProbedColliders = new Collider[16];

		// Token: 0x0400290E RID: 10510
		private Rigidbody[] _rigidbodiesPushedThisMove = new Rigidbody[16];

		// Token: 0x0400290F RID: 10511
		private RigidbodyProjectionHit[] _internalRigidbodyProjectionHits = new RigidbodyProjectionHit[6];

		// Token: 0x04002910 RID: 10512
		private Rigidbody _lastAttachedRigidbody;

		// Token: 0x04002911 RID: 10513
		private bool _solveMovementCollisions = true;

		// Token: 0x04002912 RID: 10514
		private bool _solveGrounding = true;

		// Token: 0x04002913 RID: 10515
		private bool _movePositionDirty;

		// Token: 0x04002914 RID: 10516
		private Vector3 _movePositionTarget = Vector3.zero;

		// Token: 0x04002915 RID: 10517
		private bool _moveRotationDirty;

		// Token: 0x04002916 RID: 10518
		private Quaternion _moveRotationTarget = Quaternion.identity;

		// Token: 0x04002917 RID: 10519
		private bool _lastSolvedOverlapNormalDirty;

		// Token: 0x04002918 RID: 10520
		private Vector3 _lastSolvedOverlapNormal = Vector3.forward;

		// Token: 0x04002919 RID: 10521
		private int _rigidbodiesPushedCount;

		// Token: 0x0400291A RID: 10522
		private int _rigidbodyProjectionHitCount;

		// Token: 0x0400291B RID: 10523
		private float _internalResultingMovementMagnitude;

		// Token: 0x0400291C RID: 10524
		private Vector3 _internalResultingMovementDirection = Vector3.zero;

		// Token: 0x0400291D RID: 10525
		private bool _isMovingFromAttachedRigidbody;

		// Token: 0x0400291E RID: 10526
		private Vector3 _cachedWorldUp = Vector3.up;

		// Token: 0x0400291F RID: 10527
		private Vector3 _cachedWorldForward = Vector3.forward;

		// Token: 0x04002920 RID: 10528
		private Vector3 _cachedWorldRight = Vector3.right;

		// Token: 0x04002921 RID: 10529
		private Vector3 _cachedZeroVector = Vector3.zero;

		// Token: 0x04002922 RID: 10530
		private Vector3 _internalTransientPosition;

		// Token: 0x04002923 RID: 10531
		private Quaternion _internalTransientRotation;

		// Token: 0x04002924 RID: 10532
		private Vector3 _baseVelocity;

		// Token: 0x04002925 RID: 10533
		private Vector3 _attachedRigidbodyVelocity;

		// Token: 0x04002927 RID: 10535
		private OverlapResult[] _overlaps = new OverlapResult[16];

		// Token: 0x04002928 RID: 10536
		public const int MaxHitsBudget = 16;

		// Token: 0x04002929 RID: 10537
		public const int MaxCollisionBudget = 16;

		// Token: 0x0400292A RID: 10538
		public const int MaxGroundingSweepIterations = 2;

		// Token: 0x0400292B RID: 10539
		public const int MaxMovementSweepIterations = 6;

		// Token: 0x0400292C RID: 10540
		public const int MaxSteppingSweepIterations = 3;

		// Token: 0x0400292D RID: 10541
		public const int MaxRigidbodyOverlapsCount = 16;

		// Token: 0x0400292E RID: 10542
		public const int MaxDiscreteCollisionIterations = 3;

		// Token: 0x0400292F RID: 10543
		public const float CollisionOffset = 0.001f;

		// Token: 0x04002930 RID: 10544
		public const float GroundProbeReboundDistance = 0.02f;

		// Token: 0x04002931 RID: 10545
		public const float MinimumGroundProbingDistance = 0.005f;

		// Token: 0x04002932 RID: 10546
		public const float GroundProbingBackstepDistance = 0.1f;

		// Token: 0x04002933 RID: 10547
		public const float SweepProbingBackstepDistance = 0.002f;

		// Token: 0x04002934 RID: 10548
		public const float SecondaryProbesVertical = 0.02f;

		// Token: 0x04002935 RID: 10549
		public const float SecondaryProbesHorizontal = 0.001f;

		// Token: 0x04002936 RID: 10550
		public const float MinVelocityMagnitude = 0.01f;

		// Token: 0x04002937 RID: 10551
		public const float SteppingForwardDistance = 0.03f;

		// Token: 0x04002938 RID: 10552
		public const float MinDistanceForLedge = 0.05f;

		// Token: 0x04002939 RID: 10553
		public const float CorrelationForVerticalObstruction = 0.01f;

		// Token: 0x0400293A RID: 10554
		public const float ExtraSteppingForwardDistance = 0.01f;

		// Token: 0x0400293B RID: 10555
		public const float ExtraStepHeightPadding = 0.01f;
	}
}
