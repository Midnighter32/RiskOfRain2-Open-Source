using System;
using JetBrains.Annotations;

namespace RoR2
{
	// Token: 0x02000460 RID: 1120
	public class UnlockableDef
	{
		// Token: 0x0400188D RID: 6285
		public string name;

		// Token: 0x0400188E RID: 6286
		public string nameToken = "???";

		// Token: 0x0400188F RID: 6287
		public string displayModelPath = "Prefabs/NullModel";

		// Token: 0x04001890 RID: 6288
		public UnlockableIndex index;

		// Token: 0x04001891 RID: 6289
		public bool hidden;

		// Token: 0x04001892 RID: 6290
		[NotNull]
		public Func<string> getHowToUnlockString = () => "???";

		// Token: 0x04001893 RID: 6291
		[NotNull]
		public Func<string> getUnlockedString = () => "???";

		// Token: 0x04001894 RID: 6292
		public int sortScore;
	}
}
