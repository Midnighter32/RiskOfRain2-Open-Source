using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

// Token: 0x02000061 RID: 97
public static class HGArrayUtilities
{
	// Token: 0x06000186 RID: 390 RVA: 0x0000891C File Offset: 0x00006B1C
	public static void ArrayInsert<T>(ref T[] array, ref int arraySize, int position, ref T value)
	{
		arraySize++;
		if (arraySize > array.Length)
		{
			Array.Resize<T>(ref array, arraySize);
		}
		for (int i = arraySize - 1; i > position; i--)
		{
			array[i] = array[i - 1];
		}
		array[position] = value;
	}

	// Token: 0x06000187 RID: 391 RVA: 0x00008970 File Offset: 0x00006B70
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

	// Token: 0x06000188 RID: 392 RVA: 0x000089C8 File Offset: 0x00006BC8
	public static void ArrayRemoveAtAndResize<T>(ref T[] array, int position, int count = 1)
	{
		int newSize = array.Length;
		HGArrayUtilities.ArrayRemoveAt<T>(ref array, ref newSize, position, count);
		Array.Resize<T>(ref array, newSize);
	}

	// Token: 0x06000189 RID: 393 RVA: 0x000089EC File Offset: 0x00006BEC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T GetSafe<T>([NotNull] T[] array, int index)
	{
		if ((ulong)index >= (ulong)((long)array.Length))
		{
			return default(T);
		}
		return array[index];
	}
}
