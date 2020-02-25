using System;
using System.Collections.Generic;

namespace RoR2.Stats
{
	// Token: 0x0200049D RID: 1181
	public class PerItemStatDef
	{
		// Token: 0x06001CA5 RID: 7333 RVA: 0x0007A94C File Offset: 0x00078B4C
		public static void RegisterStatDefs()
		{
			foreach (PerItemStatDef perItemStatDef in PerItemStatDef.instancesList)
			{
				foreach (ItemIndex itemIndex in ItemCatalog.allItems)
				{
					ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
					StatDef statDef = StatDef.Register(perItemStatDef.prefix + "." + itemDef.name, perItemStatDef.recordType, perItemStatDef.dataType, 0.0, null);
					perItemStatDef.keyToStatDef[(int)itemIndex] = statDef;
				}
			}
		}

		// Token: 0x06001CA6 RID: 7334 RVA: 0x0007AA1C File Offset: 0x00078C1C
		private PerItemStatDef(string prefix, StatRecordType recordType, StatDataType dataType)
		{
			this.prefix = prefix;
			this.recordType = recordType;
			this.dataType = dataType;
		}

		// Token: 0x06001CA7 RID: 7335 RVA: 0x0007AA44 File Offset: 0x00078C44
		private static PerItemStatDef Register(string prefix, StatRecordType recordType, StatDataType dataType)
		{
			PerItemStatDef perItemStatDef = new PerItemStatDef(prefix, recordType, dataType);
			PerItemStatDef.instancesList.Add(perItemStatDef);
			return perItemStatDef;
		}

		// Token: 0x06001CA8 RID: 7336 RVA: 0x0007AA66 File Offset: 0x00078C66
		public StatDef FindStatDef(ItemIndex key)
		{
			return this.keyToStatDef[(int)key];
		}

		// Token: 0x040019D2 RID: 6610
		private readonly string prefix;

		// Token: 0x040019D3 RID: 6611
		private readonly StatRecordType recordType;

		// Token: 0x040019D4 RID: 6612
		private readonly StatDataType dataType;

		// Token: 0x040019D5 RID: 6613
		private readonly StatDef[] keyToStatDef = ItemCatalog.GetPerItemBuffer<StatDef>();

		// Token: 0x040019D6 RID: 6614
		private static readonly List<PerItemStatDef> instancesList = new List<PerItemStatDef>();

		// Token: 0x040019D7 RID: 6615
		public static readonly PerItemStatDef totalCollected = PerItemStatDef.Register("totalCollected", StatRecordType.Sum, StatDataType.ULong);

		// Token: 0x040019D8 RID: 6616
		public static readonly PerItemStatDef highestCollected = PerItemStatDef.Register("highestCollected", StatRecordType.Max, StatDataType.ULong);
	}
}
