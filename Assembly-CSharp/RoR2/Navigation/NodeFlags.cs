using System;

namespace RoR2.Navigation
{
	// Token: 0x020004E2 RID: 1250
	[Flags]
	public enum NodeFlags : byte
	{
		// Token: 0x04001B03 RID: 6915
		None = 0,
		// Token: 0x04001B04 RID: 6916
		NoCeiling = 1,
		// Token: 0x04001B05 RID: 6917
		TeleporterOK = 2,
		// Token: 0x04001B06 RID: 6918
		NoCharacterSpawn = 4,
		// Token: 0x04001B07 RID: 6919
		NoChestSpawn = 8,
		// Token: 0x04001B08 RID: 6920
		NoShrineSpawn = 16
	}
}
