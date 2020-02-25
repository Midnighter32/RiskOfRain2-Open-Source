using System;

namespace RoR2.Stats
{
	// Token: 0x020004A0 RID: 1184
	public struct StatEvent
	{
		// Token: 0x040019E9 RID: 6633
		public string stringValue;

		// Token: 0x040019EA RID: 6634
		public double doubleValue;

		// Token: 0x040019EB RID: 6635
		public ulong ulongValue;

		// Token: 0x020004A1 RID: 1185
		public enum Type
		{
			// Token: 0x040019ED RID: 6637
			Damage,
			// Token: 0x040019EE RID: 6638
			Kill,
			// Token: 0x040019EF RID: 6639
			Walk,
			// Token: 0x040019F0 RID: 6640
			Die,
			// Token: 0x040019F1 RID: 6641
			Lose,
			// Token: 0x040019F2 RID: 6642
			Win
		}
	}
}
