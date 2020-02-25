using System;

namespace RoR2
{
	// Token: 0x020001B5 RID: 437
	[Flags]
	public enum ConVarFlags
	{
		// Token: 0x040009B8 RID: 2488
		None = 0,
		// Token: 0x040009B9 RID: 2489
		ExecuteOnServer = 1,
		// Token: 0x040009BA RID: 2490
		SenderMustBeServer = 2,
		// Token: 0x040009BB RID: 2491
		Archive = 4,
		// Token: 0x040009BC RID: 2492
		Cheat = 8,
		// Token: 0x040009BD RID: 2493
		Engine = 16
	}
}
