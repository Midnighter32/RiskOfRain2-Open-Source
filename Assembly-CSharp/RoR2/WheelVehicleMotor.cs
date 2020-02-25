using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200037E RID: 894
	public class WheelVehicleMotor : MonoBehaviour
	{
		// Token: 0x060015D0 RID: 5584 RVA: 0x0005CE46 File Offset: 0x0005B046
		private void Start()
		{
			this.inputBank = base.GetComponent<InputBankTest>();
		}

		// Token: 0x060015D1 RID: 5585 RVA: 0x0005CE54 File Offset: 0x0005B054
		private void UpdateWheelParameter(WheelCollider wheel)
		{
			wheel.mass = this.wheelMass;
			wheel.radius = this.wheelRadius;
			wheel.suspensionDistance = this.wheelSuspensionDistance;
			wheel.forceAppPointDistance = this.wheelForceAppPointDistance;
			wheel.transform.localPosition = new Vector3(wheel.transform.localPosition.x, -this.wheelWellDistance, wheel.transform.localPosition.z);
			wheel.suspensionSpring = new JointSpring
			{
				spring = this.wheelSuspensionSpringSpring,
				damper = this.wheelSuspensionSpringDamper,
				targetPosition = this.wheelSuspensionSpringTargetPosition
			};
			wheel.forwardFriction = new WheelFrictionCurve
			{
				extremumSlip = this.forwardFrictionExtremumSlip,
				extremumValue = this.forwardFrictionValue,
				asymptoteSlip = this.forwardFrictionAsymptoticSlip,
				asymptoteValue = this.forwardFrictionAsymptoticValue,
				stiffness = this.forwardFrictionStiffness
			};
			wheel.sidewaysFriction = new WheelFrictionCurve
			{
				extremumSlip = this.sidewaysFrictionExtremumSlip,
				extremumValue = this.sidewaysFrictionValue,
				asymptoteSlip = this.sidewaysFrictionAsymptoticSlip,
				asymptoteValue = this.sidewaysFrictionAsymptoticValue,
				stiffness = this.sidewaysFrictionStiffness
			};
		}

		// Token: 0x060015D2 RID: 5586 RVA: 0x0005CFA0 File Offset: 0x0005B1A0
		private void UpdateAllWheelParameters()
		{
			foreach (WheelCollider wheel in this.driveWheels)
			{
				this.UpdateWheelParameter(wheel);
			}
			foreach (WheelCollider wheel2 in this.steerWheels)
			{
				this.UpdateWheelParameter(wheel2);
			}
		}

		// Token: 0x060015D3 RID: 5587 RVA: 0x0005CFF0 File Offset: 0x0005B1F0
		private void FixedUpdate()
		{
			this.UpdateAllWheelParameters();
			if (this.inputBank)
			{
				this.moveVector = this.inputBank.moveVector;
				float f = 0f;
				if (this.moveVector.sqrMagnitude > 0f)
				{
					f = Util.AngleSigned(base.transform.forward, this.moveVector, Vector3.up);
				}
				WheelCollider[] array = this.steerWheels;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].steerAngle = Mathf.Min(this.maxSteerAngle, Mathf.Abs(f)) * Mathf.Sign(f);
				}
				array = this.driveWheels;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].motorTorque = this.moveVector.magnitude * this.motorTorque;
				}
			}
		}

		// Token: 0x04001449 RID: 5193
		[HideInInspector]
		public Vector3 moveVector;

		// Token: 0x0400144A RID: 5194
		public WheelCollider[] driveWheels;

		// Token: 0x0400144B RID: 5195
		public WheelCollider[] steerWheels;

		// Token: 0x0400144C RID: 5196
		public float motorTorque;

		// Token: 0x0400144D RID: 5197
		public float maxSteerAngle;

		// Token: 0x0400144E RID: 5198
		public float wheelMass = 20f;

		// Token: 0x0400144F RID: 5199
		public float wheelRadius = 0.5f;

		// Token: 0x04001450 RID: 5200
		public float wheelWellDistance = 2.7f;

		// Token: 0x04001451 RID: 5201
		public float wheelSuspensionDistance = 0.3f;

		// Token: 0x04001452 RID: 5202
		public float wheelForceAppPointDistance;

		// Token: 0x04001453 RID: 5203
		public float wheelSuspensionSpringSpring = 35000f;

		// Token: 0x04001454 RID: 5204
		public float wheelSuspensionSpringDamper = 4500f;

		// Token: 0x04001455 RID: 5205
		public float wheelSuspensionSpringTargetPosition = 0.5f;

		// Token: 0x04001456 RID: 5206
		public float forwardFrictionExtremumSlip = 0.4f;

		// Token: 0x04001457 RID: 5207
		public float forwardFrictionValue = 1f;

		// Token: 0x04001458 RID: 5208
		public float forwardFrictionAsymptoticSlip = 0.8f;

		// Token: 0x04001459 RID: 5209
		public float forwardFrictionAsymptoticValue = 0.5f;

		// Token: 0x0400145A RID: 5210
		public float forwardFrictionStiffness = 1f;

		// Token: 0x0400145B RID: 5211
		public float sidewaysFrictionExtremumSlip = 0.2f;

		// Token: 0x0400145C RID: 5212
		public float sidewaysFrictionValue = 1f;

		// Token: 0x0400145D RID: 5213
		public float sidewaysFrictionAsymptoticSlip = 0.5f;

		// Token: 0x0400145E RID: 5214
		public float sidewaysFrictionAsymptoticValue = 0.75f;

		// Token: 0x0400145F RID: 5215
		public float sidewaysFrictionStiffness = 1f;

		// Token: 0x04001460 RID: 5216
		private InputBankTest inputBank;
	}
}
