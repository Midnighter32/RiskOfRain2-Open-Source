using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003A6 RID: 934
	[RequireComponent(typeof(CharacterBody))]
	[RequireComponent(typeof(VectorPID))]
	[RequireComponent(typeof(InputBankTest))]
	public class RigidbodyMotor : MonoBehaviour
	{
		// Token: 0x060013C7 RID: 5063 RVA: 0x00060D44 File Offset: 0x0005EF44
		private void Start()
		{
			Vector3 vector = this.rigid.centerOfMass;
			vector += this.centerOfMassOffset;
			this.rigid.centerOfMass = vector;
			this.characterBody = base.GetComponent<CharacterBody>();
			this.inputBank = base.GetComponent<InputBankTest>();
			this.modelLocator = base.GetComponent<ModelLocator>();
			this.bodyAnimatorSmoothingParameters = base.GetComponent<BodyAnimatorSmoothingParameters>();
			if (this.modelLocator)
			{
				Transform modelTransform = this.modelLocator.modelTransform;
				if (modelTransform)
				{
					this.animator = modelTransform.GetComponent<Animator>();
				}
			}
		}

		// Token: 0x060013C8 RID: 5064 RVA: 0x00060DD3 File Offset: 0x0005EFD3
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(base.transform.position + this.rigid.centerOfMass, 0.5f);
		}

		// Token: 0x060013C9 RID: 5065 RVA: 0x00060E04 File Offset: 0x0005F004
		public static float GetPitch(Vector3 v)
		{
			float x = Mathf.Sqrt(v.x * v.x + v.z * v.z);
			return -Mathf.Atan2(v.y, x);
		}

		// Token: 0x060013CA RID: 5066 RVA: 0x00060E40 File Offset: 0x0005F040
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

		// Token: 0x060013CB RID: 5067 RVA: 0x00060F3C File Offset: 0x0005F13C
		private void FixedUpdate()
		{
			if (this.inputBank && this.rigid && this.forcePID)
			{
				Vector3 aimDirection = this.inputBank.aimDirection;
				Vector3 targetVector = this.moveVector;
				this.forcePID.inputVector = this.rigid.velocity;
				this.forcePID.targetVector = targetVector;
				Debug.DrawLine(base.transform.position, base.transform.position + this.forcePID.targetVector, Color.red, 0.1f);
				Vector3 vector = this.forcePID.UpdatePID();
				this.rigid.AddForceAtPosition(Vector3.ClampMagnitude(vector, this.characterBody.acceleration), base.transform.position, ForceMode.Acceleration);
			}
		}

		// Token: 0x04001789 RID: 6025
		[HideInInspector]
		public Vector3 moveVector;

		// Token: 0x0400178A RID: 6026
		public Rigidbody rigid;

		// Token: 0x0400178B RID: 6027
		public VectorPID forcePID;

		// Token: 0x0400178C RID: 6028
		public Vector3 centerOfMassOffset;

		// Token: 0x0400178D RID: 6029
		public string animatorForward;

		// Token: 0x0400178E RID: 6030
		public string animatorRight;

		// Token: 0x0400178F RID: 6031
		public string animatorUp;

		// Token: 0x04001790 RID: 6032
		private CharacterBody characterBody;

		// Token: 0x04001791 RID: 6033
		private InputBankTest inputBank;

		// Token: 0x04001792 RID: 6034
		private ModelLocator modelLocator;

		// Token: 0x04001793 RID: 6035
		private Animator animator;

		// Token: 0x04001794 RID: 6036
		private BodyAnimatorSmoothingParameters bodyAnimatorSmoothingParameters;
	}
}
