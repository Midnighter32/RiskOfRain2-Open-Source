using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020004C6 RID: 1222
	public struct Trajectory
	{
		// Token: 0x06001B71 RID: 7025 RVA: 0x0008038D File Offset: 0x0007E58D
		public static float CalculateApex(float initialSpeed)
		{
			return Trajectory.CalculateApex(initialSpeed, Physics.gravity.y);
		}

		// Token: 0x06001B72 RID: 7026 RVA: 0x0008039F File Offset: 0x0007E59F
		public static float CalculateApex(float initialSpeed, float gravity)
		{
			return initialSpeed * initialSpeed / (2f * -gravity);
		}

		// Token: 0x06001B73 RID: 7027 RVA: 0x000803AD File Offset: 0x0007E5AD
		public static float CalculateGroundSpeed(float time, float distance)
		{
			return distance / time;
		}

		// Token: 0x06001B74 RID: 7028 RVA: 0x000803AD File Offset: 0x0007E5AD
		public static float CalculateGroundTravelTime(float hSpeed, float hDistance)
		{
			return hDistance / hSpeed;
		}

		// Token: 0x06001B75 RID: 7029 RVA: 0x000803B2 File Offset: 0x0007E5B2
		public static float CalculateInitialYSpeed(float timeToTarget, float destinationYOffset)
		{
			return Trajectory.CalculateInitialYSpeed(timeToTarget, destinationYOffset, Physics.gravity.y);
		}

		// Token: 0x06001B76 RID: 7030 RVA: 0x000803C5 File Offset: 0x0007E5C5
		public static float CalculateInitialYSpeed(float timeToTarget, float destinationYOffset, float gravity)
		{
			return (destinationYOffset + 0.5f * -gravity * timeToTarget * timeToTarget) / timeToTarget;
		}

		// Token: 0x06001B77 RID: 7031 RVA: 0x000803D7 File Offset: 0x0007E5D7
		public static float CalculateInitialYSpeedForHeight(float height)
		{
			return Trajectory.CalculateInitialYSpeedForHeight(height, Physics.gravity.y);
		}

		// Token: 0x06001B78 RID: 7032 RVA: 0x000803E9 File Offset: 0x0007E5E9
		public static float CalculateInitialYSpeedForHeight(float height, float gravity)
		{
			return Mathf.Sqrt(height * (2f * -gravity));
		}

		// Token: 0x06001B79 RID: 7033 RVA: 0x000803FA File Offset: 0x0007E5FA
		public static Vector3 CalculatePositionAtTime(Vector3 origin, Vector3 initialVelocity, float t)
		{
			return Trajectory.CalculatePositionAtTime(origin, initialVelocity, t, Physics.gravity.y);
		}

		// Token: 0x06001B7A RID: 7034 RVA: 0x00080410 File Offset: 0x0007E610
		public static Vector3 CalculatePositionAtTime(Vector3 origin, Vector3 initialVelocity, float t, float gravity)
		{
			Vector3 result = origin + initialVelocity * t;
			result.y += 0.5f * gravity * t * t;
			return result;
		}

		// Token: 0x06001B7B RID: 7035 RVA: 0x00080442 File Offset: 0x0007E642
		public static float CalculateInitialYSpeedForFlightDuration(float duration)
		{
			return Trajectory.CalculateInitialYSpeedForFlightDuration(duration, Physics.gravity.y);
		}

		// Token: 0x06001B7C RID: 7036 RVA: 0x00080454 File Offset: 0x0007E654
		public static float CalculateInitialYSpeedForFlightDuration(float duration, float gravity)
		{
			return duration * gravity * -0.5f;
		}
	}
}
