using System;

namespace RoR2.Navigation
{
	// Token: 0x02000528 RID: 1320
	[Flags]
	public enum NodeFlags : byte
	{
		// Token: 0x04001FEA RID: 8170
		None = 0,
		// Token: 0x04001FEB RID: 8171
		NoCeiling = 1,
		// Token: 0x04001FEC RID: 8172
		TeleporterOK = 2,
		// Token: 0x04001FED RID: 8173
		NoCharacterSpawn = 4,
		// Token: 0x04001FEE RID: 8174
		NoChestSpawn = 8,
		// Token: 0x04001FEF RID: 8175
		NoShrineSpawn = 16
	}
}
