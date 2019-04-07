using System;
using System.Collections.Generic;

namespace RoR2.Stats
{
	// Token: 0x020004FA RID: 1274
	public class PerEquipmentStatDef
	{
		// Token: 0x06001CCD RID: 7373 RVA: 0x00086478 File Offset: 0x00084678
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

		// Token: 0x06001CCE RID: 7374 RVA: 0x00086548 File Offset: 0x00084748
		private PerEquipmentStatDef(string prefix, StatRecordType recordType, StatDataType dataType, StatDef.DisplayValueFormatterDelegate displayValueFormatter)
		{
			this.prefix = prefix;
			this.recordType = recordType;
			this.dataType = dataType;
			this.displayValueFormatter = (displayValueFormatter ?? new StatDef.DisplayValueFormatterDelegate(StatDef.DefaultDisplayValueFormatter));
		}

		// Token: 0x06001CCF RID: 7375 RVA: 0x00086598 File Offset: 0x00084798
		private static PerEquipmentStatDef Register(string prefix, StatRecordType recordType, StatDataType dataType, StatDef.DisplayValueFormatterDelegate displayValueFormatter = null)
		{
			PerEquipmentStatDef perEquipmentStatDef = new PerEquipmentStatDef(prefix, recordType, dataType, displayValueFormatter);
			PerEquipmentStatDef.instancesList.Add(perEquipmentStatDef);
			return perEquipmentStatDef;
		}

		// Token: 0x06001CD0 RID: 7376 RVA: 0x000865BB File Offset: 0x000847BB
		public StatDef FindStatDef(EquipmentIndex key)
		{
			return this.keyToStatDef[(int)key];
		}

		// Token: 0x04001F0C RID: 7948
		private readonly string prefix;

		// Token: 0x04001F0D RID: 7949
		private readonly StatRecordType recordType;

		// Token: 0x04001F0E RID: 7950
		private readonly StatDataType dataType;

		// Token: 0x04001F0F RID: 7951
		private readonly StatDef[] keyToStatDef = new StatDef[27];

		// Token: 0x04001F10 RID: 7952
		private StatDef.DisplayValueFormatterDelegate displayValueFormatter;

		// Token: 0x04001F11 RID: 7953
		private static readonly List<PerEquipmentStatDef> instancesList = new List<PerEquipmentStatDef>();

		// Token: 0x04001F12 RID: 7954
		public static readonly PerEquipmentStatDef totalTimeHeld = PerEquipmentStatDef.Register("totalTimeHeld", StatRecordType.Sum, StatDataType.Double, new StatDef.DisplayValueFormatterDelegate(StatDef.TimeMMSSDisplayValueFormatter));

		// Token: 0x04001F13 RID: 7955
		public static readonly PerEquipmentStatDef totalTimesFired = PerEquipmentStatDef.Register("totalTimesFired", StatRecordType.Sum, StatDataType.ULong, null);
	}
}
