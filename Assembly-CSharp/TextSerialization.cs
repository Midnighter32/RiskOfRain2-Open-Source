using System;
using System.Globalization;
using System.Runtime.CompilerServices;

// Token: 0x02000074 RID: 116
public static class TextSerialization
{
	// Token: 0x060001BA RID: 442 RVA: 0x0000964A File Offset: 0x0000784A
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseInvariant(string s, out int result)
	{
		return int.TryParse(s, NumberStyles.Integer, TextSerialization.invariantCulture, out result);
	}

	// Token: 0x060001BB RID: 443 RVA: 0x00009659 File Offset: 0x00007859
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseInvariant(string s, out uint result)
	{
		return uint.TryParse(s, NumberStyles.Integer, TextSerialization.invariantCulture, out result);
	}

	// Token: 0x060001BC RID: 444 RVA: 0x00009668 File Offset: 0x00007868
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseInvariant(string s, out long result)
	{
		return long.TryParse(s, NumberStyles.Integer, TextSerialization.invariantCulture, out result);
	}

	// Token: 0x060001BD RID: 445 RVA: 0x00009677 File Offset: 0x00007877
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseInvariant(string s, out ulong result)
	{
		return ulong.TryParse(s, NumberStyles.Integer, TextSerialization.invariantCulture, out result);
	}

	// Token: 0x060001BE RID: 446 RVA: 0x00009686 File Offset: 0x00007886
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseInvariant(string s, out short result)
	{
		return short.TryParse(s, NumberStyles.Integer, TextSerialization.invariantCulture, out result);
	}

	// Token: 0x060001BF RID: 447 RVA: 0x00009695 File Offset: 0x00007895
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseInvariant(string s, out ushort result)
	{
		return ushort.TryParse(s, NumberStyles.Integer, TextSerialization.invariantCulture, out result);
	}

	// Token: 0x060001C0 RID: 448 RVA: 0x000096A4 File Offset: 0x000078A4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseInvariant(string s, out float result)
	{
		return float.TryParse(s, NumberStyles.Float, TextSerialization.invariantCulture, out result);
	}

	// Token: 0x060001C1 RID: 449 RVA: 0x000096B7 File Offset: 0x000078B7
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseInvariant(string s, out double result)
	{
		return double.TryParse(s, NumberStyles.Float, TextSerialization.invariantCulture, out result);
	}

	// Token: 0x060001C2 RID: 450 RVA: 0x000096CA File Offset: 0x000078CA
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToStringInvariant(int value)
	{
		return value.ToString(TextSerialization.invariantCulture);
	}

	// Token: 0x060001C3 RID: 451 RVA: 0x000096D8 File Offset: 0x000078D8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToStringInvariant(uint value)
	{
		return value.ToString(TextSerialization.invariantCulture);
	}

	// Token: 0x060001C4 RID: 452 RVA: 0x000096E6 File Offset: 0x000078E6
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToStringInvariant(long value)
	{
		return value.ToString(TextSerialization.invariantCulture);
	}

	// Token: 0x060001C5 RID: 453 RVA: 0x000096F4 File Offset: 0x000078F4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToStringInvariant(ulong value)
	{
		return value.ToString(TextSerialization.invariantCulture);
	}

	// Token: 0x060001C6 RID: 454 RVA: 0x00009702 File Offset: 0x00007902
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToStringInvariant(short value)
	{
		return value.ToString(TextSerialization.invariantCulture);
	}

	// Token: 0x060001C7 RID: 455 RVA: 0x00009710 File Offset: 0x00007910
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToStringInvariant(ushort value)
	{
		return value.ToString(TextSerialization.invariantCulture);
	}

	// Token: 0x060001C8 RID: 456 RVA: 0x0000971E File Offset: 0x0000791E
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToStringInvariant(float value)
	{
		return value.ToString(TextSerialization.invariantCulture);
	}

	// Token: 0x060001C9 RID: 457 RVA: 0x0000972C File Offset: 0x0000792C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToStringInvariant(double value)
	{
		return value.ToString(TextSerialization.invariantCulture);
	}

	// Token: 0x040001F2 RID: 498
	private static readonly CultureInfo invariantCulture = CultureInfo.InvariantCulture;
}
