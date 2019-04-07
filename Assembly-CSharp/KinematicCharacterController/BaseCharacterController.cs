using System;
using UnityEngine;
using UnityEngine.Networking;

namespace KinematicCharacterController
{
	// Token: 0x020006C0 RID: 1728
	public abstract class BaseCharacterController : NetworkBehaviour
	{
		// Token: 0x1700033A RID: 826
		// (get) Token: 0x06002674 RID: 9844 RVA: 0x000B165D File Offset: 0x000AF85D
		// (set) Token: 0x06002675 RID: 9845 RVA: 0x000B1665 File Offset: 0x000AF865
		public KinematicCharacterMotor Motor { get; private set; }

		// Token: 0x06002676 RID: 9846 RVA: 0x000B166E File Offset: 0x000AF86E
		public void SetupCharacterMotor(KinematicCharacterMotor motor)
		{
			this.Motor = motor;
			motor.CharacterController = this;
		}

		// Token: 0x06002677 RID: 9847
		public abstract void UpdateRotation(ref Quaternion currentRotation, float deltaTime);

		// Token: 0x06002678 RID: 9848
		public abstract void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime);

		// Token: 0x06002679 RID: 9849
		public abstract void BeforeCharacterUpdate(float deltaTime);

		// Token: 0x0600267A RID: 9850
		public abstract void PostGroundingUpdate(float deltaTime);

		// Token: 0x0600267B RID: 9851
		public abstract void AfterCharacterUpdate(float deltaTime);

		// Token: 0x0600267C RID: 9852
		public abstract bool IsColliderValidForCollisions(Collider coll);

		// Token: 0x0600267D RID: 9853
		public abstract void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport);

		// Token: 0x0600267E RID: 9854
		public abstract void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport);

		// Token: 0x0600267F RID: 9855
		public abstract void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport);

		// Token: 0x06002680 RID: 9856 RVA: 0x00004507 File Offset: 0x00002707
		public virtual void OnDiscreteCollisionDetected(Collider hitCollider)
		{
		}

		// Token: 0x06002681 RID: 9857 RVA: 0x000B1680 File Offset: 0x000AF880
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

		// Token: 0x06002682 RID: 9858 RVA: 0x000B1794 File Offset: 0x000AF994
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

		// Token: 0x06002684 RID: 9860 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x06002685 RID: 9861 RVA: 0x000B1878 File Offset: 0x000AFA78
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x06002686 RID: 9862 RVA: 0x00004507 File Offset: 0x00002707
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}
	}
}
