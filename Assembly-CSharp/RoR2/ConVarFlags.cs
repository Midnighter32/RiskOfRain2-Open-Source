using System;

namespace RoR2
{
	// Token: 0x020002A6 RID: 678
	[Flags]
	public enum ConVarFlags
	{
		// Token: 0x040011CF RID: 4559
		None = 0,
		// Token: 0x040011D0 RID: 4560
		ExecuteOnServer = 1,
		// Token: 0x040011D1 RID: 4561
		SenderMustBeServer = 2,
		// Token: 0x040011D2 RID: 4562
		Archive = 4,
		// Token: 0x040011D3 RID: 4563
		Cheat = 8,
		// Token: 0x040011D4 RID: 4564
		Engine = 16
	}
}
