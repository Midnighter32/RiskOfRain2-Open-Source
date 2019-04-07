using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000314 RID: 788
	[RequireComponent(typeof(Rigidbody))]
	public class HoverVehicleMotor : MonoBehaviour
	{
		// Token: 0x0600104D RID: 4173 RVA: 0x00051BCE File Offset: 0x0004FDCE
		private void Start()
		{
			this.inputBank = base.GetComponent<InputBankTest>();
			this.rigidbody = base.GetComponent<Rigidbody>();
		}

		// Token: 0x0600104E RID: 4174 RVA: 0x00051BE8 File Offset: 0x0004FDE8
		private void ApplyWheelForces(HoverEngine wheel, float gas, bool driveWheel, AnimationCurve slidingWheelTractionCurve)
		{
			if (wheel.isGrounded)
			{
				float d = 0.005f;
				Transform transform = wheel.transform;
				float d2 = 1f;
				Vector3 position = transform.position;
				Vector3 pointVelocity = this.rigidbody.GetPointVelocity(position);
				Vector3 a = Vector3.Project(pointVelocity, transform.right);
				Vector3 a2 = Vector3.Project(pointVelocity, transform.forward);
				Vector3 up = Vector3.up;
				Debug.DrawRay(position, pointVelocity, Color.blue);
				Vector3 a3 = Vector3.zero;
				if (driveWheel)
				{
					a3 = transform.forward * gas * this.motorForce;
					this.rigidbody.AddForceAtPosition(transform.forward * gas * this.motorForce * d2, position);
					Debug.DrawRay(position, a3 * d, Color.yellow);
				}
				Vector3 vector = Vector3.ProjectOnPlane(-a2 * this.rollingFrictionCoefficient * d2, up);
				this.rigidbody.AddForceAtPosition(vector, position);
				Debug.DrawRay(position, vector * d, Color.red);
				Vector3 vector2 = Vector3.ProjectOnPlane(-a * slidingWheelTractionCurve.Evaluate(pointVelocity.magnitude) * this.slidingTractionCoefficient * d2, up);
				this.rigidbody.AddForceAtPosition(vector2, position);
				Debug.DrawRay(position, vector2 * d, Color.red);
				Debug.DrawRay(position, (a3 + vector + vector2) * d, Color.green);
			}
		}

		// Token: 0x0600104F RID: 4175 RVA: 0x00051D69 File Offset: 0x0004FF69
		private void UpdateCenterOfMass()
		{
			this.rigidbody.ResetCenterOfMass();
			this.rigidbody.centerOfMass = this.rigidbody.centerOfMass + this.centerOfMassOffset;
		}

		// Token: 0x06001050 RID: 4176 RVA: 0x00051D98 File Offset: 0x0004FF98
		private void UpdateWheelParameter(HoverEngine wheel, HoverVehicleMotor.WheelLateralAxis wheelLateralAxis, HoverVehicleMotor.WheelLongitudinalAxis wheelLongitudinalAxis)
		{
			wheel.hoverForce = this.hoverForce;
			wheel.hoverDamping = this.hoverDamping;
			wheel.hoverHeight = this.hoverHeight;
			wheel.offsetVector = this.hoverOffsetVector;
			wheel.hoverRadius = this.hoverRadius;
			Vector3 zero = Vector3.zero;
			zero.y = -this.wheelWellDepth;
			if (wheelLateralAxis != HoverVehicleMotor.WheelLateralAxis.Left)
			{
				if (wheelLateralAxis == HoverVehicleMotor.WheelLateralAxis.Right)
				{
					zero.x = this.trackWidth / 2f;
				}
			}
			else
			{
				zero.x = -this.trackWidth / 2f;
			}
			if (wheelLongitudinalAxis != HoverVehicleMotor.WheelLongitudinalAxis.Front)
			{
				if (wheelLongitudinalAxis == HoverVehicleMotor.WheelLongitudinalAxis.Back)
				{
					zero.z = -this.wheelBase / 2f;
				}
			}
			else
			{
				zero.z = this.wheelBase / 2f;
			}
			wheel.transform.localPosition = zero;
		}

		// Token: 0x06001051 RID: 4177 RVA: 0x00051E68 File Offset: 0x00050068
		private void UpdateAllWheelParameters()
		{
			foreach (HoverVehicleMotor.AxleGroup axleGroup in this.staticAxles)
			{
				HoverEngine leftWheel = axleGroup.leftWheel;
				HoverEngine rightWheel = axleGroup.rightWheel;
				this.UpdateWheelParameter(leftWheel, HoverVehicleMotor.WheelLateralAxis.Left, axleGroup.wheelLongitudinalAxis);
				this.UpdateWheelParameter(rightWheel, HoverVehicleMotor.WheelLateralAxis.Right, axleGroup.wheelLongitudinalAxis);
			}
			foreach (HoverVehicleMotor.AxleGroup axleGroup2 in this.steerAxles)
			{
				HoverEngine leftWheel2 = axleGroup2.leftWheel;
				HoverEngine rightWheel2 = axleGroup2.rightWheel;
				this.UpdateWheelParameter(leftWheel2, HoverVehicleMotor.WheelLateralAxis.Left, axleGroup2.wheelLongitudinalAxis);
				this.UpdateWheelParameter(rightWheel2, HoverVehicleMotor.WheelLateralAxis.Right, axleGroup2.wheelLongitudinalAxis);
			}
		}

		// Token: 0x06001052 RID: 4178 RVA: 0x00051F10 File Offset: 0x00050110
		private void FixedUpdate()
		{
			this.UpdateCenterOfMass();
			this.UpdateAllWheelParameters();
			if (this.inputBank)
			{
				Vector3 moveVector = this.inputBank.moveVector;
				Vector3 normalized = Vector3.ProjectOnPlane(this.inputBank.aimDirection, base.transform.up).normalized;
				float num = Mathf.Clamp(Util.AngleSigned(base.transform.forward, normalized, base.transform.up), -this.maxSteerAngle, this.maxSteerAngle);
				float magnitude = moveVector.magnitude;
				foreach (HoverVehicleMotor.AxleGroup axleGroup in this.staticAxles)
				{
					HoverEngine leftWheel = axleGroup.leftWheel;
					HoverEngine rightWheel = axleGroup.rightWheel;
					this.ApplyWheelForces(leftWheel, magnitude, axleGroup.isDriven, axleGroup.slidingTractionCurve);
					this.ApplyWheelForces(rightWheel, magnitude, axleGroup.isDriven, axleGroup.slidingTractionCurve);
				}
				foreach (HoverVehicleMotor.AxleGroup axleGroup2 in this.steerAxles)
				{
					HoverEngine leftWheel2 = axleGroup2.leftWheel;
					HoverEngine rightWheel2 = axleGroup2.rightWheel;
					float num2 = this.maxTurningRadius / Mathf.Abs(num / this.maxSteerAngle);
					float num3 = Mathf.Atan(this.wheelBase / (num2 - this.trackWidth / 2f)) * 57.29578f;
					float num4 = Mathf.Atan(this.wheelBase / (num2 + this.trackWidth / 2f)) * 57.29578f;
					Quaternion localRotation = Quaternion.Euler(0f, num3 * Mathf.Sign(num), 0f);
					Quaternion localRotation2 = Quaternion.Euler(0f, num4 * Mathf.Sign(num), 0f);
					if (num <= 0f)
					{
						leftWheel2.transform.localRotation = localRotation;
						rightWheel2.transform.localRotation = localRotation2;
					}
					else
					{
						leftWheel2.transform.localRotation = localRotation2;
						rightWheel2.transform.localRotation = localRotation;
					}
					this.ApplyWheelForces(leftWheel2, magnitude, axleGroup2.isDriven, axleGroup2.slidingTractionCurve);
					this.ApplyWheelForces(rightWheel2, magnitude, axleGroup2.isDriven, axleGroup2.slidingTractionCurve);
				}
				Debug.DrawRay(base.transform.position, normalized * 5f, Color.blue);
			}
		}

		// Token: 0x06001053 RID: 4179 RVA: 0x00052165 File Offset: 0x00050365
		private void OnDrawGizmos()
		{
			if (this.rigidbody)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(base.transform.TransformPoint(this.rigidbody.centerOfMass), 0.3f);
			}
		}

		// Token: 0x04001452 RID: 5202
		[HideInInspector]
		public Vector3 targetSteerVector;

		// Token: 0x04001453 RID: 5203
		public Vector3 centerOfMassOffset;

		// Token: 0x04001454 RID: 5204
		public HoverVehicleMotor.AxleGroup[] staticAxles;

		// Token: 0x04001455 RID: 5205
		public HoverVehicleMotor.AxleGroup[] steerAxles;

		// Token: 0x04001456 RID: 5206
		public float wheelWellDepth;

		// Token: 0x04001457 RID: 5207
		public float wheelBase;

		// Token: 0x04001458 RID: 5208
		public float trackWidth;

		// Token: 0x04001459 RID: 5209
		public float rollingFrictionCoefficient;

		// Token: 0x0400145A RID: 5210
		public float slidingTractionCoefficient;

		// Token: 0x0400145B RID: 5211
		public float motorForce;

		// Token: 0x0400145C RID: 5212
		public float maxSteerAngle;

		// Token: 0x0400145D RID: 5213
		public float maxTurningRadius;

		// Token: 0x0400145E RID: 5214
		public float hoverForce = 33f;

		// Token: 0x0400145F RID: 5215
		public float hoverHeight = 2f;

		// Token: 0x04001460 RID: 5216
		public float hoverDamping = 0.5f;

		// Token: 0x04001461 RID: 5217
		public float hoverRadius = 0.5f;

		// Token: 0x04001462 RID: 5218
		public Vector3 hoverOffsetVector = Vector3.up;

		// Token: 0x04001463 RID: 5219
		private InputBankTest inputBank;

		// Token: 0x04001464 RID: 5220
		private Vector3 steerVector = Vector3.forward;

		// Token: 0x04001465 RID: 5221
		private Rigidbody rigidbody;

		// Token: 0x02000315 RID: 789
		private enum WheelLateralAxis
		{
			// Token: 0x04001467 RID: 5223
			Left,
			// Token: 0x04001468 RID: 5224
			Right
		}

		// Token: 0x02000316 RID: 790
		public enum WheelLongitudinalAxis
		{
			// Token: 0x0400146A RID: 5226
			Front,
			// Token: 0x0400146B RID: 5227
			Back
		}

		// Token: 0x02000317 RID: 791
		[Serializable]
		public struct AxleGroup
		{
			// Token: 0x0400146C RID: 5228
			public HoverEngine leftWheel;

			// Token: 0x0400146D RID: 5229
			public HoverEngine rightWheel;

			// Token: 0x0400146E RID: 5230
			public HoverVehicleMotor.WheelLongitudinalAxis wheelLongitudinalAxis;

			// Token: 0x0400146F RID: 5231
			public bool isDriven;

			// Token: 0x04001470 RID: 5232
			public AnimationCurve slidingTractionCurve;
		}
	}
}
