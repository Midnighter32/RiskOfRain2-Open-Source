using System;
using System.Collections.Generic;

namespace RoR2.Stats
{
	// Token: 0x020004F9 RID: 1273
	public class PerItemStatDef
	{
		// Token: 0x06001CC8 RID: 7368 RVA: 0x00086318 File Offset: 0x00084518
		public static void RegisterStatDefs()
		{
			foreach (PerItemStatDef perItemStatDef in PerItemStatDef.instancesList)
			{
				foreach (ItemIndex itemIndex in ItemCatalog.allItems)
				{
					StatDef statDef = StatDef.Register(perItemStatDef.prefix + "." + itemIndex.ToString(), perItemStatDef.recordType, perItemStatDef.dataType, 0.0, null);
					perItemStatDef.keyToStatDef[(int)itemIndex] = statDef;
				}
			}
		}

		// Token: 0x06001CC9 RID: 7369 RVA: 0x000863E4 File Offset: 0x000845E4
		private PerItemStatDef(string prefix, StatRecordType recordType, StatDataType dataType)
		{
			this.prefix = prefix;
			this.recordType = recordType;
			this.dataType = dataType;
		}

		// Token: 0x06001CCA RID: 7370 RVA: 0x00086410 File Offset: 0x00084610
		private static PerItemStatDef Register(string prefix, StatRecordType recordType, StatDataType dataType)
		{
			PerItemStatDef perItemStatDef = new PerItemStatDef(prefix, recordType, dataType);
			PerItemStatDef.instancesList.Add(perItemStatDef);
			return perItemStatDef;
		}

		// Token: 0x06001CCB RID: 7371 RVA: 0x00086432 File Offset: 0x00084632
		public StatDef FindStatDef(ItemIndex key)
		{
			return this.keyToStatDef[(int)key];
		}

		// Token: 0x04001F05 RID: 7941
		private readonly string prefix;

		// Token: 0x04001F06 RID: 7942
		private readonly StatRecordType recordType;

		// Token: 0x04001F07 RID: 7943
		private readonly StatDataType dataType;

		// Token: 0x04001F08 RID: 7944
		private readonly StatDef[] keyToStatDef = new StatDef[78];

		// Token: 0x04001F09 RID: 7945
		private static readonly List<PerItemStatDef> instancesList = new List<PerItemStatDef>();

		// Token: 0x04001F0A RID: 7946
		public static readonly PerItemStatDef totalCollected = PerItemStatDef.Register("totalCollected", StatRecordType.Sum, StatDataType.ULong);

		// Token: 0x04001F0B RID: 7947
		public static readonly PerItemStatDef highestCollected = PerItemStatDef.Register("highestCollected", StatRecordType.Max, StatDataType.ULong);
	}
}
