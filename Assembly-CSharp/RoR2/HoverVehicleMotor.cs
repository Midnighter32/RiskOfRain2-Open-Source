using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000234 RID: 564
	[RequireComponent(typeof(Rigidbody))]
	public class HoverVehicleMotor : MonoBehaviour
	{
		// Token: 0x06000C8F RID: 3215 RVA: 0x0003898A File Offset: 0x00036B8A
		private void Start()
		{
			this.inputBank = base.GetComponent<InputBankTest>();
			this.rigidbody = base.GetComponent<Rigidbody>();
		}

		// Token: 0x06000C90 RID: 3216 RVA: 0x000389A4 File Offset: 0x00036BA4
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

		// Token: 0x06000C91 RID: 3217 RVA: 0x00038B25 File Offset: 0x00036D25
		private void UpdateCenterOfMass()
		{
			this.rigidbody.ResetCenterOfMass();
			this.rigidbody.centerOfMass = this.rigidbody.centerOfMass + this.centerOfMassOffset;
		}

		// Token: 0x06000C92 RID: 3218 RVA: 0x00038B54 File Offset: 0x00036D54
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

		// Token: 0x06000C93 RID: 3219 RVA: 0x00038C24 File Offset: 0x00036E24
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

		// Token: 0x06000C94 RID: 3220 RVA: 0x00038CCC File Offset: 0x00036ECC
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

		// Token: 0x06000C95 RID: 3221 RVA: 0x00038F21 File Offset: 0x00037121
		private void OnDrawGizmos()
		{
			if (this.rigidbody)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(base.transform.TransformPoint(this.rigidbody.centerOfMass), 0.3f);
			}
		}

		// Token: 0x04000C90 RID: 3216
		[HideInInspector]
		public Vector3 targetSteerVector;

		// Token: 0x04000C91 RID: 3217
		public Vector3 centerOfMassOffset;

		// Token: 0x04000C92 RID: 3218
		public HoverVehicleMotor.AxleGroup[] staticAxles;

		// Token: 0x04000C93 RID: 3219
		public HoverVehicleMotor.AxleGroup[] steerAxles;

		// Token: 0x04000C94 RID: 3220
		public float wheelWellDepth;

		// Token: 0x04000C95 RID: 3221
		public float wheelBase;

		// Token: 0x04000C96 RID: 3222
		public float trackWidth;

		// Token: 0x04000C97 RID: 3223
		public float rollingFrictionCoefficient;

		// Token: 0x04000C98 RID: 3224
		public float slidingTractionCoefficient;

		// Token: 0x04000C99 RID: 3225
		public float motorForce;

		// Token: 0x04000C9A RID: 3226
		public float maxSteerAngle;

		// Token: 0x04000C9B RID: 3227
		public float maxTurningRadius;

		// Token: 0x04000C9C RID: 3228
		public float hoverForce = 33f;

		// Token: 0x04000C9D RID: 3229
		public float hoverHeight = 2f;

		// Token: 0x04000C9E RID: 3230
		public float hoverDamping = 0.5f;

		// Token: 0x04000C9F RID: 3231
		public float hoverRadius = 0.5f;

		// Token: 0x04000CA0 RID: 3232
		public Vector3 hoverOffsetVector = Vector3.up;

		// Token: 0x04000CA1 RID: 3233
		private InputBankTest inputBank;

		// Token: 0x04000CA2 RID: 3234
		private Vector3 steerVector = Vector3.forward;

		// Token: 0x04000CA3 RID: 3235
		private Rigidbody rigidbody;

		// Token: 0x02000235 RID: 565
		private enum WheelLateralAxis
		{
			// Token: 0x04000CA5 RID: 3237
			Left,
			// Token: 0x04000CA6 RID: 3238
			Right
		}

		// Token: 0x02000236 RID: 566
		public enum WheelLongitudinalAxis
		{
			// Token: 0x04000CA8 RID: 3240
			Front,
			// Token: 0x04000CA9 RID: 3241
			Back
		}

		// Token: 0x02000237 RID: 567
		[Serializable]
		public struct AxleGroup
		{
			// Token: 0x04000CAA RID: 3242
			public HoverEngine leftWheel;

			// Token: 0x04000CAB RID: 3243
			public HoverEngine rightWheel;

			// Token: 0x04000CAC RID: 3244
			public HoverVehicleMotor.WheelLongitudinalAxis wheelLongitudinalAxis;

			// Token: 0x04000CAD RID: 3245
			public bool isDriven;

			// Token: 0x04000CAE RID: 3246
			public AnimationCurve slidingTractionCurve;
		}
	}
}
