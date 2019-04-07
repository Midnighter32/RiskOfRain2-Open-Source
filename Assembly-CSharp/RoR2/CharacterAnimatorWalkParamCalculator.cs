using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000210 RID: 528
	public struct CharacterAnimatorWalkParamCalculator
	{
		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x06000A4B RID: 2635 RVA: 0x00033930 File Offset: 0x00031B30
		// (set) Token: 0x06000A4C RID: 2636 RVA: 0x00033938 File Offset: 0x00031B38
		public Vector2 animatorWalkSpeed { get; private set; }

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x06000A4D RID: 2637 RVA: 0x00033941 File Offset: 0x00031B41
		// (set) Token: 0x06000A4E RID: 2638 RVA: 0x00033949 File Offset: 0x00031B49
		public float remainingTurnAngle { get; private set; }

		// Token: 0x06000A4F RID: 2639 RVA: 0x00033954 File Offset: 0x00031B54
		public void Update(Vector3 worldMoveVector, Vector3 animatorForward, in BodyAnimatorSmoothingParameters.SmoothingParameters smoothingParameters, float deltaTime)
		{
			ref Vector3 ptr = ref animatorForward;
			Vector3 rhs = Vector3.Cross(Vector3.up, ptr);
			float x = Vector3.Dot(worldMoveVector, ptr);
			float y = Vector3.Dot(worldMoveVector, rhs);
			Vector2 to = new Vector2(x, y);
			float magnitude = to.magnitude;
			float num = (magnitude > 0f) ? Vector2.SignedAngle(Vector2.right, to) : 0f;
			float magnitude2 = this.animatorWalkSpeed.magnitude;
			float current = (magnitude2 > 0f) ? Vector2.SignedAngle(Vector2.right, this.animatorWalkSpeed) : 0f;
			float d = Mathf.SmoothDamp(magnitude2, magnitude, ref this.animatorReferenceMagnitudeVelocity, smoothingParameters.walkMagnitudeSmoothDamp, float.PositiveInfinity, deltaTime);
			float num2 = Mathf.SmoothDampAngle(current, num, ref this.animatorReferenceAngleVelocity, smoothingParameters.walkAngleSmoothDamp, float.PositiveInfinity, deltaTime);
			this.remainingTurnAngle = num2 - num;
			this.animatorWalkSpeed = new Vector2(Mathf.Cos(num2 * 0.017453292f), Mathf.Sin(num2 * 0.017453292f)) * d;
		}

		// Token: 0x04000DC0 RID: 3520
		private float animatorReferenceMagnitudeVelocity;

		// Token: 0x04000DC1 RID: 3521
		private float animatorReferenceAngleVelocity;
	}
}
