using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002E3 RID: 739
	[RequireComponent(typeof(VectorPID))]
	[RequireComponent(typeof(QuaternionPID))]
	public class RigidbodyDirection : MonoBehaviour
	{
		// Token: 0x060010ED RID: 4333 RVA: 0x0004A520 File Offset: 0x00048720
		private void Start()
		{
			this.inputBank = base.GetComponent<InputBankTest>();
			this.modelLocator = base.GetComponent<ModelLocator>();
			if (this.modelLocator)
			{
				Transform modelTransform = this.modelLocator.modelTransform;
				if (modelTransform)
				{
					this.animator = modelTransform.GetComponent<Animator>();
				}
			}
		}

		// Token: 0x060010EE RID: 4334 RVA: 0x0004A574 File Offset: 0x00048774
		private void Update()
		{
			if (this.animator)
			{
				if (this.animatorXCycle.Length > 0)
				{
					this.animator.SetFloat(this.animatorXCycle, Mathf.Clamp(0.5f + this.targetTorque.x * 0.5f * this.animatorTorqueScale, -1f, 1f), 0.1f, Time.deltaTime);
				}
				if (this.animatorYCycle.Length > 0)
				{
					this.animator.SetFloat(this.animatorYCycle, Mathf.Clamp(0.5f + this.targetTorque.y * 0.5f * this.animatorTorqueScale, -1f, 1f), 0.1f, Time.deltaTime);
				}
				if (this.animatorZCycle.Length > 0)
				{
					this.animator.SetFloat(this.animatorZCycle, Mathf.Clamp(0.5f + this.targetTorque.z * 0.5f * this.animatorTorqueScale, -1f, 1f), 0.1f, Time.deltaTime);
				}
			}
		}

		// Token: 0x060010EF RID: 4335 RVA: 0x0004A694 File Offset: 0x00048894
		private void FixedUpdate()
		{
			if (this.inputBank && this.rigid && this.angularVelocityPID && this.torquePID)
			{
				this.angularVelocityPID.inputQuat = this.rigid.transform.rotation;
				Quaternion targetQuat = Util.QuaternionSafeLookRotation(this.aimDirection);
				if (this.freezeXRotation)
				{
					targetQuat.x = 0f;
				}
				if (this.freezeYRotation)
				{
					targetQuat.y = 0f;
				}
				if (this.freezeZRotation)
				{
					targetQuat.z = 0f;
				}
				this.angularVelocityPID.targetQuat = targetQuat;
				Vector3 targetVector = this.angularVelocityPID.UpdatePID();
				this.torquePID.inputVector = this.rigid.angularVelocity;
				this.torquePID.targetVector = targetVector;
				Vector3 torque = this.torquePID.UpdatePID();
				this.rigid.AddTorque(torque, ForceMode.Acceleration);
			}
		}

		// Token: 0x0400104C RID: 4172
		public Vector3 aimDirection = Vector3.one;

		// Token: 0x0400104D RID: 4173
		public Rigidbody rigid;

		// Token: 0x0400104E RID: 4174
		public QuaternionPID angularVelocityPID;

		// Token: 0x0400104F RID: 4175
		public VectorPID torquePID;

		// Token: 0x04001050 RID: 4176
		public bool freezeXRotation;

		// Token: 0x04001051 RID: 4177
		public bool freezeYRotation;

		// Token: 0x04001052 RID: 4178
		public bool freezeZRotation;

		// Token: 0x04001053 RID: 4179
		private ModelLocator modelLocator;

		// Token: 0x04001054 RID: 4180
		private Animator animator;

		// Token: 0x04001055 RID: 4181
		public string animatorXCycle;

		// Token: 0x04001056 RID: 4182
		public string animatorYCycle;

		// Token: 0x04001057 RID: 4183
		public string animatorZCycle;

		// Token: 0x04001058 RID: 4184
		public float animatorTorqueScale;

		// Token: 0x04001059 RID: 4185
		private InputBankTest inputBank;

		// Token: 0x0400105A RID: 4186
		private Vector3 targetTorque;
	}
}
