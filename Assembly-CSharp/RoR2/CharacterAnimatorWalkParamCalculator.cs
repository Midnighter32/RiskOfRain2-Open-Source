using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020000DC RID: 220
	public struct CharacterAnimatorWalkParamCalculator
	{
		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000448 RID: 1096 RVA: 0x00011883 File Offset: 0x0000FA83
		// (set) Token: 0x06000449 RID: 1097 RVA: 0x0001188B File Offset: 0x0000FA8B
		public Vector2 animatorWalkSpeed { get; private set; }

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x0600044A RID: 1098 RVA: 0x00011894 File Offset: 0x0000FA94
		// (set) Token: 0x0600044B RID: 1099 RVA: 0x0001189C File Offset: 0x0000FA9C
		public float remainingTurnAngle { get; private set; }

		// Token: 0x0600044C RID: 1100 RVA: 0x000118A8 File Offset: 0x0000FAA8
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

		// Token: 0x04000419 RID: 1049
		private float animatorReferenceMagnitudeVelocity;

		// Token: 0x0400041A RID: 1050
		private float animatorReferenceAngleVelocity;
	}
}
