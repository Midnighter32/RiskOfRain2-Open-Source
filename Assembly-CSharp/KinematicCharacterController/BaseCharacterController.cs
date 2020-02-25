using System;
using UnityEngine;
using UnityEngine.Networking;

namespace KinematicCharacterController
{
	// Token: 0x0200090B RID: 2315
	public abstract class BaseCharacterController : NetworkBehaviour
	{
		// Token: 0x17000477 RID: 1143
		// (get) Token: 0x060033BE RID: 13246 RVA: 0x000E0AA5 File Offset: 0x000DECA5
		// (set) Token: 0x060033BF RID: 13247 RVA: 0x000E0AAD File Offset: 0x000DECAD
		public KinematicCharacterMotor Motor { get; private set; }

		// Token: 0x060033C0 RID: 13248 RVA: 0x000E0AB6 File Offset: 0x000DECB6
		public void SetupCharacterMotor(KinematicCharacterMotor motor)
		{
			this.Motor = motor;
			motor.CharacterController = this;
		}

		// Token: 0x060033C1 RID: 13249
		public abstract void UpdateRotation(ref Quaternion currentRotation, float deltaTime);

		// Token: 0x060033C2 RID: 13250
		public abstract void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime);

		// Token: 0x060033C3 RID: 13251
		public abstract void BeforeCharacterUpdate(float deltaTime);

		// Token: 0x060033C4 RID: 13252
		public abstract void PostGroundingUpdate(float deltaTime);

		// Token: 0x060033C5 RID: 13253
		public abstract void AfterCharacterUpdate(float deltaTime);

		// Token: 0x060033C6 RID: 13254
		public abstract bool IsColliderValidForCollisions(Collider coll);

		// Token: 0x060033C7 RID: 13255
		public abstract void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport);

		// Token: 0x060033C8 RID: 13256
		public abstract void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport);

		// Token: 0x060033C9 RID: 13257
		public abstract void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport);

		// Token: 0x060033CA RID: 13258 RVA: 0x0000409B File Offset: 0x0000229B
		public virtual void OnDiscreteCollisionDetected(Collider hitCollider)
		{
		}

		// Token: 0x060033CB RID: 13259 RVA: 0x000E0AC8 File Offset: 0x000DECC8
		public virtual void HandleMovementProjection(ref Vector3 movement, Vector3 obstructionNormal, bool stableOnHit)
		{
			if (this.Motor.GroundingStatus.IsStableOnGround && !this.Motor.MustUnground)
			{
				if (stableOnHit)
				{
					movement = this.Motor.GetDirectionTangentToSurface(movement, obstructionNormal) * movement.magnitude;
					return;
				}
				Vector3 normalized = Vector3.Cross(Vector3.Cross(obstructionNormal, this.Motor.GroundingStatus.GroundNormal).normalized, obstructionNormal).normalized;
				movement = this.Motor.GetDirectionTangentToSurface(movement, normalized) * movement.magnitude;
				movement = Vector3.ProjectOnPlane(movement, obstructionNormal);
				return;
			}
			else
			{
				if (stableOnHit)
				{
					movement = Vector3.ProjectOnPlane(movement, this.Motor.CharacterUp);
					movement = this.Motor.GetDirectionTangentToSurface(movement, obstructionNormal) * movement.magnitude;
					return;
				}
				movement = Vector3.ProjectOnPlane(movement, obstructionNormal);
				return;
			}
		}

		// Token: 0x060033CC RID: 13260 RVA: 0x000E0BDC File Offset: 0x000DEDDC
		public virtual void HandleSimulatedRigidbodyInteraction(ref Vector3 processedVelocity, RigidbodyProjectionHit hit, float deltaTime)
		{
			float num = 0.2f;
			if (num > 0f && !hit.StableOnHit && !hit.Rigidbody.isKinematic)
			{
				float d = num / hit.Rigidbody.mass;
				Vector3 velocityFromRigidbodyMovement = this.Motor.GetVelocityFromRigidbodyMovement(hit.Rigidbody, hit.HitPoint, deltaTime);
				Vector3 a = Vector3.Project(hit.HitVelocity, hit.EffectiveHitNormal) - velocityFromRigidbodyMovement;
				hit.Rigidbody.AddForceAtPosition(d * a, hit.HitPoint, ForceMode.VelocityChange);
			}
			if (!hit.StableOnHit)
			{
				Vector3 a2 = Vector3.Project(this.Motor.GetVelocityFromRigidbodyMovement(hit.Rigidbody, hit.HitPoint, deltaTime), hit.EffectiveHitNormal);
				Vector3 b = Vector3.Project(processedVelocity, hit.EffectiveHitNormal);
				processedVelocity += a2 - b;
			}
		}

		// Token: 0x060033CE RID: 13262 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x060033CF RID: 13263 RVA: 0x000E0CC0 File Offset: 0x000DEEC0
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x060033D0 RID: 13264 RVA: 0x0000409B File Offset: 0x0000229B
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}
	}
}
