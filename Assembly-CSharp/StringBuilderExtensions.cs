using System;
using System.Text;

// Token: 0x02000073 RID: 115
public static class StringBuilderExtensions
{
	// Token: 0x060001B8 RID: 440 RVA: 0x0000957D File Offset: 0x0000777D
	public static void AppendInt(this StringBuilder stringBuilder, int value, uint minDigits = 0u, uint maxDigits = 4294967295u)
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
	}

	// Token: 0x060001B9 RID: 441 RVA: 0x000095A8 File Offset: 0x000077A8
	public static void AppendUint(this StringBuilder stringBuilder, uint value, uint minDigits = 0u, uint maxDigits = 4294967295u)
	{
		if (maxDigits < minDigits)
		{
			throw new ArgumentException("minDigits cannot be greater than maxDigits.");
		}
		uint num = 0u;
		uint num2 = (10u < maxDigits) ? 10u : maxDigits;
		uint num3 = 1u;
		while (num3 <= value && num < num2)
		{
			num3 *= 10u;
			num += 1u;
		}
		uint num4 = (minDigits < num) ? num : minDigits;
		if (num4 > 0u)
		{
			uint length = (uint)stringBuilder.Length;
			stringBuilder.Append('0', (int)num4);
			uint num5 = 0u;
			uint num6 = length + num4 - 1u;
			uint num7 = value;
			while (num5 < num)
			{
				uint num8 = num7 / 10u;
				uint num9 = (uint)((byte)(num7 - num8 * 10u));
				char value2 = (char)(48u + num9);
				stringBuilder[(int)num6] = value2;
				num7 = num8;
				num5 += 1u;
				num6 -= 1u;
			}
		}
	}

	// Token: 0x040001F1 RID: 497
	private const uint maxDigitsInUint = 10u;
}
