using System;

// Token: 0x02000072 RID: 114
public class SplitMix64
{
	// Token: 0x060001B6 RID: 438 RVA: 0x00009534 File Offset: 0x00007734
	public ulong Next()
	{
		ulong num = this.x += 11400714819323198485UL;
		ulong num2 = (num ^ num >> 30) * 13787848793156543929UL;
		ulong num3 = (num2 ^ num2 >> 27) * 10723151780598845931UL;
		return num3 ^ num3 >> 31;
	}

	// Token: 0x040001F0 RID: 496
	public ulong x;
}
