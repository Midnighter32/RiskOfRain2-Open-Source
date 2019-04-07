using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000062 RID: 98
public static class HGMath
{
	// Token: 0x0600018A RID: 394 RVA: 0x00008A14 File Offset: 0x00006C14
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

	// Token: 0x0600018B RID: 395 RVA: 0x00008A8C File Offset: 0x00006C8C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IntDivCeil(int a, int b)
	{
		return (a - 1) / b + 1;
	}

	// Token: 0x0600018C RID: 396 RVA: 0x00008A95 File Offset: 0x00006C95
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint UintSafeSubtact(uint a, uint b)
	{
		if (b <= a)
		{
			return a - b;
		}
		return 0u;
	}

	// Token: 0x0600018D RID: 397 RVA: 0x00008AA0 File Offset: 0x00006CA0
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
}
