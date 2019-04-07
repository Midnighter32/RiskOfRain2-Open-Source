using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000240 RID: 576
	public static class EliteCatalog
	{
		// Token: 0x06000ADC RID: 2780 RVA: 0x0003599C File Offset: 0x00033B9C
		static EliteCatalog()
		{
			EliteCatalog.RegisterElite(EliteIndex.Fire, new EliteDef
			{
				eliteEquipmentIndex = EquipmentIndex.AffixRed,
				color = Color.red,
				prefix = "Blazing "
			});
			EliteCatalog.RegisterElite(EliteIndex.Lightning, new EliteDef
			{
				eliteEquipmentIndex = EquipmentIndex.AffixBlue,
				color = Color.blue,
				prefix = "Overloading "
			});
			EliteCatalog.RegisterElite(EliteIndex.Ice, new EliteDef
			{
				eliteEquipmentIndex = EquipmentIndex.AffixWhite,
				color = Color.white,
				prefix = "Glacial "
			});
		}

		// Token: 0x06000ADD RID: 2781 RVA: 0x00035A46 File Offset: 0x00033C46
		private static void RegisterElite(EliteIndex eliteIndex, EliteDef eliteDef)
		{
			eliteDef.eliteIndex = eliteIndex;
			EliteCatalog.eliteList.Add(eliteIndex);
			EliteCatalog.eliteDefs[(int)eliteIndex] = eliteDef;
		}

		// Token: 0x06000ADE RID: 2782 RVA: 0x00035A64 File Offset: 0x00033C64
		public static EliteIndex IsEquipmentElite(EquipmentIndex equipmentIndex)
		{
			if (equipmentIndex < EquipmentIndex.CommandMissile || equipmentIndex >= EquipmentIndex.Count)
			{
				return EliteIndex.None;
			}
			foreach (EliteDef eliteDef in EliteCatalog.eliteDefs)
			{
				if (eliteDef.eliteEquipmentIndex == equipmentIndex)
				{
					return eliteDef.eliteIndex;
				}
			}
			return EliteIndex.None;
		}

		// Token: 0x06000ADF RID: 2783 RVA: 0x00035AA5 File Offset: 0x00033CA5
		public static EliteDef GetEliteDef(EliteIndex eliteIndex)
		{
			if (eliteIndex < EliteIndex.Fire || eliteIndex >= EliteIndex.Count)
			{
				return null;
			}
			return EliteCatalog.eliteDefs[(int)eliteIndex];
		}

		// Token: 0x04000EA8 RID: 3752
		public static List<EliteIndex> eliteList = new List<EliteIndex>();

		// Token: 0x04000EA9 RID: 3753
		private static EliteDef[] eliteDefs = new EliteDef[3];
	}
}
