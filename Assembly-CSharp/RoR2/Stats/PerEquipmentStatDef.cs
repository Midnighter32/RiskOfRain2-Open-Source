using System;
using System.Collections.Generic;

namespace RoR2.Stats
{
	// Token: 0x0200049E RID: 1182
	public class PerEquipmentStatDef
	{
		// Token: 0x06001CAA RID: 7338 RVA: 0x0007AAAC File Offset: 0x00078CAC
		public static void RegisterStatDefs()
		{
			foreach (PerEquipmentStatDef perEquipmentStatDef in PerEquipmentStatDef.instancesList)
			{
				foreach (EquipmentIndex equipmentIndex in EquipmentCatalog.allEquipment)
				{
					StatDef statDef = StatDef.Register(perEquipmentStatDef.prefix + "." + equipmentIndex.ToString(), perEquipmentStatDef.recordType, perEquipmentStatDef.dataType, 0.0, perEquipmentStatDef.displayValueFormatter);
					perEquipmentStatDef.keyToStatDef[(int)equipmentIndex] = statDef;
				}
			}
		}

		// Token: 0x06001CAB RID: 7339 RVA: 0x0007AB7C File Offset: 0x00078D7C
		private PerEquipmentStatDef(string prefix, StatRecordType recordType, StatDataType dataType, StatDef.DisplayValueFormatterDelegate displayValueFormatter)
		{
			this.prefix = prefix;
			this.recordType = recordType;
			this.dataType = dataType;
			this.displayValueFormatter = (displayValueFormatter ?? new StatDef.DisplayValueFormatterDelegate(StatDef.DefaultDisplayValueFormatter));
		}

		// Token: 0x06001CAC RID: 7340 RVA: 0x0007ABBC File Offset: 0x00078DBC
		private static PerEquipmentStatDef Register(string prefix, StatRecordType recordType, StatDataType dataType, StatDef.DisplayValueFormatterDelegate displayValueFormatter = null)
		{
			PerEquipmentStatDef perEquipmentStatDef = new PerEquipmentStatDef(prefix, recordType, dataType, displayValueFormatter);
			PerEquipmentStatDef.instancesList.Add(perEquipmentStatDef);
			return perEquipmentStatDef;
		}

		// Token: 0x06001CAD RID: 7341 RVA: 0x0007ABDF File Offset: 0x00078DDF
		public StatDef FindStatDef(EquipmentIndex key)
		{
			return this.keyToStatDef[(int)key];
		}

		// Token: 0x040019D9 RID: 6617
		private readonly string prefix;

		// Token: 0x040019DA RID: 6618
		private readonly StatRecordType recordType;

		// Token: 0x040019DB RID: 6619
		private readonly StatDataType dataType;

		// Token: 0x040019DC RID: 6620
		private readonly StatDef[] keyToStatDef = EquipmentCatalog.GetPerEquipmentBuffer<StatDef>();

		// Token: 0x040019DD RID: 6621
		private StatDef.DisplayValueFormatterDelegate displayValueFormatter;

		// Token: 0x040019DE RID: 6622
		private static readonly List<PerEquipmentStatDef> instancesList = new List<PerEquipmentStatDef>();

		// Token: 0x040019DF RID: 6623
		public static readonly PerEquipmentStatDef totalTimeHeld = PerEquipmentStatDef.Register("totalTimeHeld", StatRecordType.Sum, StatDataType.Double, new StatDef.DisplayValueFormatterDelegate(StatDef.TimeMMSSDisplayValueFormatter));

		// Token: 0x040019E0 RID: 6624
		public static readonly PerEquipmentStatDef totalTimesFired = PerEquipmentStatDef.Register("totalTimesFired", StatRecordType.Sum, StatDataType.ULong, null);
	}
}
