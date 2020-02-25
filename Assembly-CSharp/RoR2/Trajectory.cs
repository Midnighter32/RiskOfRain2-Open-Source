using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200045E RID: 1118
	public struct Trajectory
	{
		// Token: 0x17000313 RID: 787
		// (get) Token: 0x06001B08 RID: 6920 RVA: 0x00072A89 File Offset: 0x00070C89
		private static float defaultGravity
		{
			get
			{
				return Physics.gravity.y;
			}
		}

		// Token: 0x06001B09 RID: 6921 RVA: 0x00072A95 File Offset: 0x00070C95
		public static float CalculateApex(float initialSpeed)
		{
			return Trajectory.CalculateApex(initialSpeed, Trajectory.defaultGravity);
		}

		// Token: 0x06001B0A RID: 6922 RVA: 0x00072AA2 File Offset: 0x00070CA2
		public static float CalculateApex(float initialSpeed, float gravity)
		{
			return initialSpeed * initialSpeed / (2f * -gravity);
		}

		// Token: 0x06001B0B RID: 6923 RVA: 0x00072AB0 File Offset: 0x00070CB0
		public static float CalculateGroundSpeed(float time, float distance)
		{
			return distance / time;
		}

		// Token: 0x06001B0C RID: 6924 RVA: 0x00072AB0 File Offset: 0x00070CB0
		public static float CalculateGroundTravelTime(float hSpeed, float hDistance)
		{
			return hDistance / hSpeed;
		}

		// Token: 0x06001B0D RID: 6925 RVA: 0x00072AB5 File Offset: 0x00070CB5
		public static float CalculateInitialYSpeed(float timeToTarget, float destinationYOffset)
		{
			return Trajectory.CalculateInitialYSpeed(timeToTarget, destinationYOffset, Trajectory.defaultGravity);
		}

		// Token: 0x06001B0E RID: 6926 RVA: 0x00072AC3 File Offset: 0x00070CC3
		public static float CalculateInitialYSpeed(float timeToTarget, float destinationYOffset, float gravity)
		{
			return (destinationYOffset + 0.5f * -gravity * timeToTarget * timeToTarget) / timeToTarget;
		}

		// Token: 0x06001B0F RID: 6927 RVA: 0x00072AD5 File Offset: 0x00070CD5
		public static float CalculateInitialYSpeedForHeight(float height)
		{
			return Trajectory.CalculateInitialYSpeedForHeight(height, Trajectory.defaultGravity);
		}

		// Token: 0x06001B10 RID: 6928 RVA: 0x00072AE2 File Offset: 0x00070CE2
		public static float CalculateInitialYSpeedForHeight(float height, float gravity)
		{
			return Mathf.Sqrt(height * (2f * -gravity));
		}

		// Token: 0x06001B11 RID: 6929 RVA: 0x00072AF3 File Offset: 0x00070CF3
		public static Vector3 CalculatePositionAtTime(Vector3 origin, Vector3 initialVelocity, float t)
		{
			return Trajectory.CalculatePositionAtTime(origin, initialVelocity, t, Trajectory.defaultGravity);
		}

		// Token: 0x06001B12 RID: 6930 RVA: 0x00072B04 File Offset: 0x00070D04
		public static Vector3 CalculatePositionAtTime(Vector3 origin, Vector3 initialVelocity, float t, float gravity)
		{
			Vector3 result = origin + initialVelocity * t;
			result.y += 0.5f * gravity * t * t;
			return result;
		}

		// Token: 0x06001B13 RID: 6931 RVA: 0x00072B36 File Offset: 0x00070D36
		public static float CalculatePositionYAtTime(float originY, float initialVelocityY, float t)
		{
			return Trajectory.CalculatePositionYAtTime(originY, initialVelocityY, t, Trajectory.defaultGravity);
		}

		// Token: 0x06001B14 RID: 6932 RVA: 0x00072B45 File Offset: 0x00070D45
		public static float CalculatePositionYAtTime(float originY, float initialVelocityY, float t, float gravity)
		{
			return originY + initialVelocityY * t + 0.5f * gravity * t * t;
		}

		// Token: 0x06001B15 RID: 6933 RVA: 0x00072B58 File Offset: 0x00070D58
		public static float CalculateInitialYSpeedForFlightDuration(float duration)
		{
			return Trajectory.CalculateInitialYSpeedForFlightDuration(duration, Trajectory.defaultGravity);
		}

		// Token: 0x06001B16 RID: 6934 RVA: 0x00072B65 File Offset: 0x00070D65
		public static float CalculateInitialYSpeedForFlightDuration(float duration, float gravity)
		{
			return duration * gravity * -0.5f;
		}

		// Token: 0x06001B17 RID: 6935 RVA: 0x00072B70 File Offset: 0x00070D70
		public static float CalculateFlightDuration(float vSpeed)
		{
			return Trajectory.CalculateFlightDuration(vSpeed, Trajectory.defaultGravity);
		}

		// Token: 0x06001B18 RID: 6936 RVA: 0x00072B7D File Offset: 0x00070D7D
		public static float CalculateFlightDuration(float vSpeed, float gravity)
		{
			return 2f * vSpeed / -gravity;
		}

		// Token: 0x06001B19 RID: 6937 RVA: 0x00072B89 File Offset: 0x00070D89
		public static float CalculateGroundSpeedToClearDistance(float vSpeed, float distance)
		{
			return Trajectory.CalculateGroundSpeedToClearDistance(vSpeed, distance, Trajectory.defaultGravity);
		}

		// Token: 0x06001B1A RID: 6938 RVA: 0x00072B97 File Offset: 0x00070D97
		public static float CalculateGroundSpeedToClearDistance(float vSpeed, float distance, float gravity)
		{
			return distance / Trajectory.CalculateFlightDuration(vSpeed, gravity);
		}
	}
}
