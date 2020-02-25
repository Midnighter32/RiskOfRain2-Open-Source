using System;
using System.Globalization;
using System.Runtime.CompilerServices;

// Token: 0x02000078 RID: 120
public static class TextSerialization
{
	// Token: 0x060001EC RID: 492 RVA: 0x0000A01D File Offset: 0x0000821D
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseInvariant(string s, out int result)
	{
		return int.TryParse(s, NumberStyles.Integer, TextSerialization.invariantCulture, out result);
	}

	// Token: 0x060001ED RID: 493 RVA: 0x0000A02C File Offset: 0x0000822C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseInvariant(string s, out uint result)
	{
		return uint.TryParse(s, NumberStyles.Integer, TextSerialization.invariantCulture, out result);
	}

	// Token: 0x060001EE RID: 494 RVA: 0x0000A03B File Offset: 0x0000823B
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseInvariant(string s, out long result)
	{
		return long.TryParse(s, NumberStyles.Integer, TextSerialization.invariantCulture, out result);
	}

	// Token: 0x060001EF RID: 495 RVA: 0x0000A04A File Offset: 0x0000824A
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseInvariant(string s, out ulong result)
	{
		return ulong.TryParse(s, NumberStyles.Integer, TextSerialization.invariantCulture, out result);
	}

	// Token: 0x060001F0 RID: 496 RVA: 0x0000A059 File Offset: 0x00008259
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseInvariant(string s, out short result)
	{
		return short.TryParse(s, NumberStyles.Integer, TextSerialization.invariantCulture, out result);
	}

	// Token: 0x060001F1 RID: 497 RVA: 0x0000A068 File Offset: 0x00008268
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseInvariant(string s, out ushort result)
	{
		return ushort.TryParse(s, NumberStyles.Integer, TextSerialization.invariantCulture, out result);
	}

	// Token: 0x060001F2 RID: 498 RVA: 0x0000A077 File Offset: 0x00008277
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseInvariant(string s, out float result)
	{
		return float.TryParse(s, NumberStyles.Float, TextSerialization.invariantCulture, out result);
	}

	// Token: 0x060001F3 RID: 499 RVA: 0x0000A08A File Offset: 0x0000828A
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryParseInvariant(string s, out double result)
	{
		return double.TryParse(s, NumberStyles.Float, TextSerialization.invariantCulture, out result);
	}

	// Token: 0x060001F4 RID: 500 RVA: 0x0000A09D File Offset: 0x0000829D
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToStringInvariant(int value)
	{
		return value.ToString(TextSerialization.invariantCulture);
	}

	// Token: 0x060001F5 RID: 501 RVA: 0x0000A0AB File Offset: 0x000082AB
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToStringInvariant(uint value)
	{
		return value.ToString(TextSerialization.invariantCulture);
	}

	// Token: 0x060001F6 RID: 502 RVA: 0x0000A0B9 File Offset: 0x000082B9
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToStringInvariant(long value)
	{
		return value.ToString(TextSerialization.invariantCulture);
	}

	// Token: 0x060001F7 RID: 503 RVA: 0x0000A0C7 File Offset: 0x000082C7
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToStringInvariant(ulong value)
	{
		return value.ToString(TextSerialization.invariantCulture);
	}

	// Token: 0x060001F8 RID: 504 RVA: 0x0000A0D5 File Offset: 0x000082D5
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToStringInvariant(short value)
	{
		return value.ToString(TextSerialization.invariantCulture);
	}

	// Token: 0x060001F9 RID: 505 RVA: 0x0000A0E3 File Offset: 0x000082E3
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToStringInvariant(ushort value)
	{
		return value.ToString(TextSerialization.invariantCulture);
	}

	// Token: 0x060001FA RID: 506 RVA: 0x0000A0F1 File Offset: 0x000082F1
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToStringInvariant(float value)
	{
		return value.ToString(TextSerialization.invariantCulture);
	}

	// Token: 0x060001FB RID: 507 RVA: 0x0000A0FF File Offset: 0x000082FF
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToStringInvariant(double value)
	{
		return value.ToString(TextSerialization.invariantCulture);
	}

	// Token: 0x060001FC RID: 508 RVA: 0x0000A10D File Offset: 0x0000830D
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToStringNumeric(double value)
	{
		return value.ToString("N0");
	}

	// Token: 0x060001FD RID: 509 RVA: 0x0000A11B File Offset: 0x0000831B
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToStringNumeric(ulong value)
	{
		return value.ToString("N0");
	}

	// Token: 0x040001FA RID: 506
	private static readonly CultureInfo invariantCulture = CultureInfo.InvariantCulture;
}
