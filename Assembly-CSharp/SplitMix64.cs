using System;

// Token: 0x02000076 RID: 118
public class SplitMix64
{
	// Token: 0x060001E4 RID: 484 RVA: 0x00009E40 File Offset: 0x00008040
	public ulong Next()
	{
		ulong num = this.x += 11400714819323198485UL;
		ulong num2 = (num ^ num >> 30) * 13787848793156543929UL;
		ulong num3 = (num2 ^ num2 >> 27) * 10723151780598845931UL;
		return num3 ^ num3 >> 31;
	}

	// Token: 0x040001F7 RID: 503
	public ulong x;
}
