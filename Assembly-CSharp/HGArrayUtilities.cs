using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

// Token: 0x0200005B RID: 91
public static class HGArrayUtilities
{
	// Token: 0x06000163 RID: 355 RVA: 0x00008760 File Offset: 0x00006960
	public static void ArrayInsertNoResize<T>(T[] array, int arraySize, int position, ref T value)
	{
		for (int i = arraySize - 1; i > position; i--)
		{
			array[i] = array[i - 1];
		}
		array[position] = value;
	}

	// Token: 0x06000164 RID: 356 RVA: 0x00008798 File Offset: 0x00006998
	public static void ArrayInsert<T>(ref T[] array, ref int arraySize, int position, ref T value)
	{
		arraySize++;
		if (arraySize > array.Length)
		{
			Array.Resize<T>(ref array, arraySize);
		}
		HGArrayUtilities.ArrayInsertNoResize<T>(array, arraySize, position, ref value);
	}

	// Token: 0x06000165 RID: 357 RVA: 0x000087BC File Offset: 0x000069BC
	public static void ArrayInsert<T>(ref T[] array, int position, ref T value)
	{
		int num = array.Length;
		HGArrayUtilities.ArrayInsert<T>(ref array, ref num, position, ref value);
	}

	// Token: 0x06000166 RID: 358 RVA: 0x000087D8 File Offset: 0x000069D8
	public static void ArrayAppend<T>(ref T[] array, ref int arraySize, ref T value)
	{
		HGArrayUtilities.ArrayInsert<T>(ref array, ref arraySize, arraySize, ref value);
	}

	// Token: 0x06000167 RID: 359 RVA: 0x000087E4 File Offset: 0x000069E4
	public static void ArrayAppend<T>(ref T[] array, ref T value)
	{
		int num = array.Length;
		HGArrayUtilities.ArrayAppend<T>(ref array, ref num, ref value);
	}

	// Token: 0x06000168 RID: 360 RVA: 0x00008800 File Offset: 0x00006A00
	public static void ArrayRemoveAt<T>(ref T[] array, ref int arraySize, int position, int count = 1)
	{
		int num = arraySize;
		arraySize -= count;
		int i = position;
		int num2 = arraySize;
		while (i < num2)
		{
			array[i] = array[i + count];
			i++;
		}
		for (int j = arraySize; j < num; j++)
		{
			array[j] = default(T);
		}
	}

	// Token: 0x06000169 RID: 361 RVA: 0x00008858 File Offset: 0x00006A58
	public static void ArrayRemoveAtAndResize<T>(ref T[] array, int position, int count = 1)
	{
		int newSize = array.Length;
		HGArrayUtilities.ArrayRemoveAt<T>(ref array, ref newSize, position, count);
		Array.Resize<T>(ref array, newSize);
	}

	// Token: 0x0600016A RID: 362 RVA: 0x0000887C File Offset: 0x00006A7C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T GetSafe<T>([NotNull] T[] array, int index)
	{
		if ((ulong)index >= (ulong)((long)array.Length))
		{
			return default(T);
		}
		return array[index];
	}

	// Token: 0x0600016B RID: 363 RVA: 0x000088A2 File Offset: 0x00006AA2
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T GetSafe<T>([NotNull] T[] array, int index, T defaultValue)
	{
		if ((ulong)index >= (ulong)((long)array.Length))
		{
			return defaultValue;
		}
		return array[index];
	}

	// Token: 0x0600016C RID: 364 RVA: 0x000088B8 File Offset: 0x00006AB8
	public static void SetAll<T>(T[] array, T value)
	{
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = value;
		}
	}

	// Token: 0x0600016D RID: 365 RVA: 0x000088DB File Offset: 0x00006ADB
	public static void EnsureCapacity<T>(ref T[] array, int capacity)
	{
		if (array.Length < capacity)
		{
			Array.Resize<T>(ref array, capacity);
		}
	}

	// Token: 0x0600016E RID: 366 RVA: 0x000088EC File Offset: 0x00006AEC
	public static void Swap<T>(T[] array, int a, int b)
	{
		ref T ptr = ref array[b];
		T t = array[a];
		array[a] = ptr;
		ptr = t;
	}

	// Token: 0x0600016F RID: 367 RVA: 0x00008921 File Offset: 0x00006B21
	public static void Clear<T>(T[] array, ref int count)
	{
		Array.Clear(array, 0, count);
		count = 0;
	}

	// Token: 0x06000170 RID: 368 RVA: 0x00008930 File Offset: 0x00006B30
	public static bool SequenceEquals<T>(T[] a, T[] b) where T : IEquatable<T>
	{
		if (a == null || b == null)
		{
			return a == null == (b == null);
		}
		if (a == b)
		{
			return true;
		}
		if (a.Length != b.Length)
		{
			return false;
		}
		for (int i = 0; i < a.Length; i++)
		{
			if (!a[i].Equals(b[i]))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000171 RID: 369 RVA: 0x0000898C File Offset: 0x00006B8C
	public static T[] Clone<T>(T[] src)
	{
		T[] array = new T[src.Length];
		Array.Copy(src, array, src.Length);
		return array;
	}
}
