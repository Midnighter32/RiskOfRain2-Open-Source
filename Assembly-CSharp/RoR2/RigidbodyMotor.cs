using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002E4 RID: 740
	[RequireComponent(typeof(CharacterBody))]
	[RequireComponent(typeof(VectorPID))]
	[RequireComponent(typeof(InputBankTest))]
	public class RigidbodyMotor : MonoBehaviour, IDisplacementReceiver
	{
		// Token: 0x060010F1 RID: 4337 RVA: 0x0004A7AA File Offset: 0x000489AA
		private void Awake()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
			this.inputBank = base.GetComponent<InputBankTest>();
			this.modelLocator = base.GetComponent<ModelLocator>();
			this.healthComponent = base.GetComponent<HealthComponent>();
			this.bodyAnimatorSmoothingParameters = base.GetComponent<BodyAnimatorSmoothingParameters>();
		}

		// Token: 0x060010F2 RID: 4338 RVA: 0x0004A7E8 File Offset: 0x000489E8
		private void Start()
		{
			Vector3 vector = this.rigid.centerOfMass;
			vector += this.centerOfMassOffset;
			this.rigid.centerOfMass = vector;
			if (this.modelLocator)
			{
				Transform modelTransform = this.modelLocator.modelTransform;
				if (modelTransform)
				{
					this.animator = modelTransform.GetComponent<Animator>();
				}
			}
		}

		// Token: 0x060010F3 RID: 4339 RVA: 0x0004A847 File Offset: 0x00048A47
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(base.transform.position + this.rigid.centerOfMass, 0.5f);
		}

		// Token: 0x060010F4 RID: 4340 RVA: 0x0004A878 File Offset: 0x00048A78
		public static float GetPitch(Vector3 v)
		{
			float x = Mathf.Sqrt(v.x * v.x + v.z * v.z);
			return -Mathf.Atan2(v.y, x);
		}

		// Token: 0x060010F5 RID: 4341 RVA: 0x0004A8B4 File Offset: 0x00048AB4
		private void Update()
		{
			if (this.animator)
			{
				Vector3 vector = base.transform.InverseTransformVector(this.moveVector) / Mathf.Max(1f, this.moveVector.magnitude);
				BodyAnimatorSmoothingParameters.SmoothingParameters smoothingParameters = this.bodyAnimatorSmoothingParameters ? this.bodyAnimatorSmoothingParameters.smoothingParameters : BodyAnimatorSmoothingParameters.defaultParameters;
				if (this.animatorForward.Length > 0)
				{
					this.animator.SetFloat(this.animatorForward, vector.z, smoothingParameters.forwardSpeedSmoothDamp, Time.deltaTime);
				}
				if (this.animatorRight.Length > 0)
				{
					this.animator.SetFloat(this.animatorRight, vector.x, smoothingParameters.rightSpeedSmoothDamp, Time.deltaTime);
				}
				if (this.animatorUp.Length > 0)
				{
					this.animator.SetFloat(this.animatorUp, vector.y, smoothingParameters.forwardSpeedSmoothDamp, Time.deltaTime);
				}
			}
		}

		// Token: 0x060010F6 RID: 4342 RVA: 0x0004A9B0 File Offset: 0x00048BB0
		private void FixedUpdate()
		{
			if (this.inputBank && this.rigid)
			{
				if (this.forcePID)
				{
					if (this.enableOverrideMoveVectorInLocalSpace)
					{
						this.moveVector = base.transform.TransformDirection(this.overrideMoveVectorInLocalSpace) * this.characterBody.moveSpeed;
					}
					Vector3 aimDirection = this.inputBank.aimDirection;
					Vector3 targetVector = this.moveVector;
					this.forcePID.inputVector = this.rigid.velocity;
					this.forcePID.targetVector = targetVector;
					Debug.DrawLine(base.transform.position, base.transform.position + this.forcePID.targetVector, Color.red, 0.1f);
					Vector3 a = this.forcePID.UpdatePID();
					this.rigid.AddForceAtPosition(Vector3.ClampMagnitude(a * (this.characterBody.acceleration / 3f), this.characterBody.acceleration), base.transform.position, ForceMode.Acceleration);
				}
				if (this.rootMotion != Vector3.zero)
				{
					this.rigid.MovePosition(this.rigid.position + this.rootMotion);
					this.rootMotion = Vector3.zero;
				}
			}
		}

		// Token: 0x060010F7 RID: 4343 RVA: 0x0004AB10 File Offset: 0x00048D10
		private void OnCollisionEnter(Collision collision)
		{
			if (this.canTakeImpactDamage && collision.gameObject.layer == LayerIndex.world.intVal)
			{
				float num = Mathf.Max(this.characterBody.moveSpeed, this.characterBody.baseMoveSpeed) * 4f;
				float magnitude = collision.relativeVelocity.magnitude;
				if (magnitude >= num)
				{
					float num2 = magnitude / this.characterBody.moveSpeed * 0.07f;
					DamageInfo damageInfo = new DamageInfo();
					damageInfo.damage = Mathf.Min(this.healthComponent.fullCombinedHealth, this.healthComponent.fullCombinedHealth * num2);
					damageInfo.procCoefficient = 0f;
					damageInfo.position = collision.contacts[0].point;
					damageInfo.attacker = this.healthComponent.lastHitAttacker;
					this.healthComponent.TakeDamage(damageInfo);
				}
			}
		}

		// Token: 0x060010F8 RID: 4344 RVA: 0x0004ABFC File Offset: 0x00048DFC
		public void AddDisplacement(Vector3 displacement)
		{
			this.rootMotion += displacement;
		}

		// Token: 0x0400105B RID: 4187
		[HideInInspector]
		public Vector3 moveVector;

		// Token: 0x0400105C RID: 4188
		public Rigidbody rigid;

		// Token: 0x0400105D RID: 4189
		public VectorPID forcePID;

		// Token: 0x0400105E RID: 4190
		public Vector3 centerOfMassOffset;

		// Token: 0x0400105F RID: 4191
		public string animatorForward;

		// Token: 0x04001060 RID: 4192
		public string animatorRight;

		// Token: 0x04001061 RID: 4193
		public string animatorUp;

		// Token: 0x04001062 RID: 4194
		public bool enableOverrideMoveVectorInLocalSpace;

		// Token: 0x04001063 RID: 4195
		public bool canTakeImpactDamage = true;

		// Token: 0x04001064 RID: 4196
		public Vector3 overrideMoveVectorInLocalSpace;

		// Token: 0x04001065 RID: 4197
		private CharacterBody characterBody;

		// Token: 0x04001066 RID: 4198
		private InputBankTest inputBank;

		// Token: 0x04001067 RID: 4199
		private ModelLocator modelLocator;

		// Token: 0x04001068 RID: 4200
		private Animator animator;

		// Token: 0x04001069 RID: 4201
		private BodyAnimatorSmoothingParameters bodyAnimatorSmoothingParameters;

		// Token: 0x0400106A RID: 4202
		private HealthComponent healthComponent;

		// Token: 0x0400106B RID: 4203
		private Vector3 rootMotion;

		// Token: 0x0400106C RID: 4204
		private const float impactDamageStrength = 0.07f;

		// Token: 0x020002E5 RID: 741
		private struct CollisionInfo
		{
			// Token: 0x0400106D RID: 4205
			public Vector3 relativeVelocity;
		}
	}
}
