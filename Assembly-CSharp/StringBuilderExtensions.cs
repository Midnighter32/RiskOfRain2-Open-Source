using System;
using System.Text;
using UnityEngine;

// Token: 0x02000077 RID: 119
public static class StringBuilderExtensions
{
	// Token: 0x060001E6 RID: 486 RVA: 0x00009E89 File Offset: 0x00008089
	public static StringBuilder AppendInt(this StringBuilder stringBuilder, int value, uint minDigits = 0U, uint maxDigits = 4294967295U)
	{
		if (maxDigits < minDigits)
		{
			throw new ArgumentException("minDigits cannot be greater than maxDigits.");
		}
		if (value < 0)
		{
			stringBuilder.Append('-');
			value = -value;
		}
		stringBuilder.AppendUint((uint)value, minDigits, maxDigits);
		return stringBuilder;
	}

	// Token: 0x060001E7 RID: 487 RVA: 0x00009EB8 File Offset: 0x000080B8
	public static StringBuilder AppendUint(this StringBuilder stringBuilder, uint value, uint minDigits = 0U, uint maxDigits = 4294967295U)
	{
		if (maxDigits < minDigits)
		{
			throw new ArgumentException("minDigits cannot be greater than maxDigits.");
		}
		uint num = 0U;
		uint num2 = (10U < maxDigits) ? 10U : maxDigits;
		uint num3 = 1U;
		while (num3 <= value && num < num2)
		{
			num3 *= 10U;
			num += 1U;
		}
		uint num4 = (minDigits < num) ? num : minDigits;
		if (num4 > 0U)
		{
			uint length = (uint)stringBuilder.Length;
			stringBuilder.Append('0', (int)num4);
			uint num5 = 0U;
			uint num6 = length + num4 - 1U;
			uint num7 = value;
			while (num5 < num)
			{
				uint num8 = num7 / 10U;
				uint num9 = (uint)((byte)(num7 - num8 * 10U));
				char value2 = (char)(48U + num9);
				stringBuilder[(int)num6] = value2;
				num7 = num8;
				num5 += 1U;
				num6 -= 1U;
			}
		}
		return stringBuilder;
	}

	// Token: 0x060001E8 RID: 488 RVA: 0x00009F5B File Offset: 0x0000815B
	private static void GetByteHexChars(byte byteValue, out char char1, out char char2)
	{
		char1 = StringBuilderExtensions.hexLookup[byteValue >> 4 & 15];
		char2 = StringBuilderExtensions.hexLookup[(int)(byteValue & 15)];
	}

	// Token: 0x060001E9 RID: 489 RVA: 0x00009F78 File Offset: 0x00008178
	public static StringBuilder AppendColor32RGBHexValues(this StringBuilder stringBuilder, Color32 color)
	{
		int num = stringBuilder.Length + 6;
		if (stringBuilder.Capacity < num)
		{
			stringBuilder.Capacity = num;
		}
		char value;
		char value2;
		StringBuilderExtensions.GetByteHexChars(color.r, out value, out value2);
		char value3;
		char value4;
		StringBuilderExtensions.GetByteHexChars(color.g, out value3, out value4);
		char value5;
		char value6;
		StringBuilderExtensions.GetByteHexChars(color.b, out value5, out value6);
		return stringBuilder.Append(value).Append(value2).Append(value3).Append(value4).Append(value5).Append(value6);
	}

	// Token: 0x060001EA RID: 490 RVA: 0x00009FF5 File Offset: 0x000081F5
	public static string Take(this StringBuilder stringBuilder)
	{
		string result = stringBuilder.ToString();
		stringBuilder.Clear();
		return result;
	}

	// Token: 0x040001F8 RID: 504
	private const uint maxDigitsInUint = 10U;

	// Token: 0x040001F9 RID: 505
	private static readonly char[] hexLookup = new char[]
	{
		'0',
		'1',
		'2',
		'3',
		'4',
		'5',
		'6',
		'7',
		'8',
		'9',
		'A',
		'B',
		'C',
		'D',
		'E',
		'F'
	};
}
