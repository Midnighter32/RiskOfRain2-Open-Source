using System;

namespace RoR2.Stats
{
	// Token: 0x020004FC RID: 1276
	public struct StatEvent
	{
		// Token: 0x04001F1C RID: 7964
		public string stringValue;

		// Token: 0x04001F1D RID: 7965
		public double doubleValue;

		// Token: 0x04001F1E RID: 7966
		public ulong ulongValue;

		// Token: 0x020004FD RID: 1277
		public enum Type
		{
			// Token: 0x04001F20 RID: 7968
			Damage,
			// Token: 0x04001F21 RID: 7969
			Kill,
			// Token: 0x04001F22 RID: 7970
			Walk,
			// Token: 0x04001F23 RID: 7971
			Die,
			// Token: 0x04001F24 RID: 7972
			Lose,
			// Token: 0x04001F25 RID: 7973
			Win
		}
	}
}
