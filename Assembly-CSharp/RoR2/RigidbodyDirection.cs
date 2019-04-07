using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003A5 RID: 933
	[RequireComponent(typeof(QuaternionPID))]
	[RequireComponent(typeof(VectorPID))]
	public class RigidbodyDirection : MonoBehaviour
	{
		// Token: 0x060013C3 RID: 5059 RVA: 0x00060AB8 File Offset: 0x0005ECB8
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

		// Token: 0x060013C4 RID: 5060 RVA: 0x00060B0C File Offset: 0x0005ED0C
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

		// Token: 0x060013C5 RID: 5061 RVA: 0x00060C2C File Offset: 0x0005EE2C
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

		// Token: 0x0400177A RID: 6010
		public Vector3 aimDirection = Vector3.one;

		// Token: 0x0400177B RID: 6011
		public Rigidbody rigid;

		// Token: 0x0400177C RID: 6012
		public QuaternionPID angularVelocityPID;

		// Token: 0x0400177D RID: 6013
		public VectorPID torquePID;

		// Token: 0x0400177E RID: 6014
		public bool freezeXRotation;

		// Token: 0x0400177F RID: 6015
		public bool freezeYRotation;

		// Token: 0x04001780 RID: 6016
		public bool freezeZRotation;

		// Token: 0x04001781 RID: 6017
		private ModelLocator modelLocator;

		// Token: 0x04001782 RID: 6018
		private Animator animator;

		// Token: 0x04001783 RID: 6019
		public string animatorXCycle;

		// Token: 0x04001784 RID: 6020
		public string animatorYCycle;

		// Token: 0x04001785 RID: 6021
		public string animatorZCycle;

		// Token: 0x04001786 RID: 6022
		public float animatorTorqueScale;

		// Token: 0x04001787 RID: 6023
		private InputBankTest inputBank;

		// Token: 0x04001788 RID: 6024
		private Vector3 targetTorque;
	}
}
