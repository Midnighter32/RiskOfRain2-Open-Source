using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200005C RID: 92
public static class HGMath
{
	// Token: 0x06000172 RID: 370 RVA: 0x000089B0 File Offset: 0x00006BB0
	public static Vector3 Average<T>(T entries) where T : ICollection<Vector3>
	{
		int count = entries.Count;
		float d = 1f / (float)count;
		Vector3 vector = Vector3.zero;
		foreach (Vector3 a in entries)
		{
			vector += d * a;
		}
		return vector;
	}

	// Token: 0x06000173 RID: 371 RVA: 0x00008A28 File Offset: 0x00006C28
	public static Vector3 Average(in Vector3 a, in Vector3 b)
	{
		return (a + b) * 0.5f;
	}

	// Token: 0x06000174 RID: 372 RVA: 0x00008A45 File Offset: 0x00006C45
	public static Vector3 Average(in Vector3 a, in Vector3 b, in Vector3 c)
	{
		return (a + b + c) * 0.33333334f;
	}

	// Token: 0x06000175 RID: 373 RVA: 0x00008A6D File Offset: 0x00006C6D
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IntDivCeil(int a, int b)
	{
		return (a - 1) / b + 1;
	}

	// Token: 0x06000176 RID: 374 RVA: 0x00008A76 File Offset: 0x00006C76
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint UintSafeSubtact(uint a, uint b)
	{
		if (b <= a)
		{
			return a - b;
		}
		return 0U;
	}

	// Token: 0x06000177 RID: 375 RVA: 0x00008A84 File Offset: 0x00006C84
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint UintSafeAdd(uint a, uint b)
	{
		uint num = a + b;
		if (num >= a)
		{
			return num;
		}
		return uint.MaxValue;
	}

	// Token: 0x06000178 RID: 376 RVA: 0x00008A9C File Offset: 0x00006C9C
	public static Vector3 Remap(Vector3 value, Vector3 inMin, Vector3 inMax, Vector3 outMin, Vector3 outMax)
	{
		return new Vector3(outMin.x + (value.x - inMin.x) / (inMax.x - inMin.x) * (outMax.x - outMin.x), outMin.y + (value.y - inMin.y) / (inMax.y - inMin.y) * (outMax.y - outMin.y), outMin.z + (value.z - inMin.z) / (inMax.z - inMin.z) * (outMax.z - outMin.z));
	}

	// Token: 0x06000179 RID: 377 RVA: 0x00008B41 File Offset: 0x00006D41
	public static Vector3 Remap(Vector3 value, float inMin, float inMax, float outMin, float outMax)
	{
		return new Vector3(outMin + (value.x - inMin) / (inMax - inMin) * (outMax - outMin), outMin + (value.y - inMin) / (inMax - inMin) * (outMax - outMin), outMin + (value.z - inMin) / (inMax - inMin) * (outMax - outMin));
	}

	// Token: 0x0600017A RID: 378 RVA: 0x00008B81 File Offset: 0x00006D81
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float Clamp(float value, float min, float max)
	{
		if (value <= min)
		{
			return min;
		}
		if (value >= max)
		{
			return max;
		}
		return value;
	}

	// Token: 0x0600017B RID: 379 RVA: 0x00008B81 File Offset: 0x00006D81
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Clamp(int value, int min, int max)
	{
		if (value <= min)
		{
			return min;
		}
		if (value >= max)
		{
			return max;
		}
		return value;
	}

	// Token: 0x0600017C RID: 380 RVA: 0x00008B90 File Offset: 0x00006D90
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint Clamp(uint value, uint min, uint max)
	{
		if (value <= min)
		{
			return min;
		}
		if (value >= max)
		{
			return max;
		}
		return value;
	}

	// Token: 0x0600017D RID: 381 RVA: 0x00008BA0 File Offset: 0x00006DA0
	public static Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max)
	{
		return new Vector3(HGMath.Clamp(value.x, min.x, max.x), HGMath.Clamp(value.y, min.y, max.y), HGMath.Clamp(value.z, min.z, max.z));
	}

	// Token: 0x0600017E RID: 382 RVA: 0x00008BF7 File Offset: 0x00006DF7
	public static Vector3 Clamp(Vector3 value, float min, float max)
	{
		return new Vector3(HGMath.Clamp(value.x, min, max), HGMath.Clamp(value.y, min, max), HGMath.Clamp(value.z, min, max));
	}

	// Token: 0x0600017F RID: 383 RVA: 0x00008C25 File Offset: 0x00006E25
	public static bool IsVectorNaN(Vector3 value)
	{
		return float.IsNaN(value.x) || float.IsNaN(value.y) || float.IsNaN(value.z);
	}

	// Token: 0x06000180 RID: 384 RVA: 0x00008C50 File Offset: 0x00006E50
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsVectorValid(ref Vector3 vector3)
	{
		float f = vector3.x + vector3.y + vector3.z;
		return !float.IsInfinity(f) && !float.IsNaN(f);
	}

	// Token: 0x06000181 RID: 385 RVA: 0x00008C88 File Offset: 0x00006E88
	public static bool Overshoots(Vector3 startPosition, Vector3 endPosition, Vector3 targetPosition)
	{
		Vector3 lhs = endPosition - startPosition;
		Vector3 rhs = targetPosition - endPosition;
		return Vector3.Dot(lhs, rhs) <= 0f;
	}

	// Token: 0x06000182 RID: 386 RVA: 0x00008CB4 File Offset: 0x00006EB4
	public static float TriangleArea(in Vector3 a, in Vector3 b, in Vector3 c)
	{
		return 0.5f * Vector3.Cross(b - a, c - a).magnitude;
	}

	// Token: 0x06000183 RID: 387 RVA: 0x00008CF6 File Offset: 0x00006EF6
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float CircleRadiusToArea(float radius)
	{
		return 3.1415927f * (radius * radius);
	}

	// Token: 0x06000184 RID: 388 RVA: 0x00008D01 File Offset: 0x00006F01
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float CircleAreaToRadius(float area)
	{
		return Mathf.Sqrt(area * 0.31830987f);
	}
}
