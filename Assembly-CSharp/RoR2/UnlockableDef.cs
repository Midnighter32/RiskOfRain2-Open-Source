using System;
using JetBrains.Annotations;

namespace RoR2
{
	// Token: 0x020004C8 RID: 1224
	public class UnlockableDef
	{
		// Token: 0x04001E0F RID: 7695
		public string name;

		// Token: 0x04001E10 RID: 7696
		public string nameToken = "???";

		// Token: 0x04001E11 RID: 7697
		public string displayModelPath = "Prefabs/NullModel";

		// Token: 0x04001E12 RID: 7698
		public UnlockableIndex index;

		// Token: 0x04001E13 RID: 7699
		public bool hidden;

		// Token: 0x04001E14 RID: 7700
		[NotNull]
		public Func<string> getHowToUnlockString = () => "???";

		// Token: 0x04001E15 RID: 7701
		[NotNull]
		public Func<string> getUnlockedString = () => "???";
	}
}
