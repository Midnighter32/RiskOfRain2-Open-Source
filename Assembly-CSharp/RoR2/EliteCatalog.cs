using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200012D RID: 301
	public static class EliteCatalog
	{
		// Token: 0x0600056C RID: 1388 RVA: 0x00015FA0 File Offset: 0x000141A0
		[SystemInitializer(new Type[]
		{

		})]
		private static void Init()
		{
			EliteCatalog.eliteDefs = new EliteDef[6];
			EliteCatalog.RegisterElite(EliteIndex.Fire, new EliteDef
			{
				eliteEquipmentIndex = EquipmentIndex.AffixRed,
				color = Color.red,
				modifierToken = "ELITE_MODIFIER_FIRE"
			});
			EliteCatalog.RegisterElite(EliteIndex.Lightning, new EliteDef
			{
				eliteEquipmentIndex = EquipmentIndex.AffixBlue,
				color = Color.blue,
				modifierToken = "ELITE_MODIFIER_LIGHTNING"
			});
			EliteCatalog.RegisterElite(EliteIndex.Ice, new EliteDef
			{
				eliteEquipmentIndex = EquipmentIndex.AffixWhite,
				color = Color.white,
				modifierToken = "ELITE_MODIFIER_ICE"
			});
			EliteCatalog.RegisterElite(EliteIndex.Poison, new EliteDef
			{
				eliteEquipmentIndex = EquipmentIndex.AffixPoison,
				color = Color.black,
				modifierToken = "ELITE_MODIFIER_POISON"
			});
			EliteCatalog.RegisterElite(EliteIndex.Haunted, new EliteDef
			{
				eliteEquipmentIndex = EquipmentIndex.AffixHaunted,
				color = Color.white,
				modifierToken = "ELITE_MODIFIER_HAUNTED"
			});
			EliteCatalog.RegisterElite(EliteIndex.Gold, new EliteDef
			{
				eliteEquipmentIndex = EquipmentIndex.AffixGold,
				color = Color.yellow,
				modifierToken = "ELITE_MODIFIER_GOLD"
			});
			EliteCatalog.modHelper.CollectAndRegisterAdditionalEntries(ref EliteCatalog.eliteDefs);
		}

		// Token: 0x0600056D RID: 1389 RVA: 0x000160D8 File Offset: 0x000142D8
		private static void RegisterElite(EliteIndex eliteIndex, EliteDef eliteDef)
		{
			if (eliteIndex < EliteIndex.Count)
			{
				eliteDef.name = eliteIndex.ToString();
			}
			eliteDef.eliteIndex = eliteIndex;
			EliteCatalog.eliteList.Add(eliteIndex);
			EliteCatalog.eliteDefs[(int)eliteIndex] = eliteDef;
		}

		// Token: 0x0600056E RID: 1390 RVA: 0x0001610C File Offset: 0x0001430C
		public static EliteIndex GetEquipmentEliteIndex(EquipmentIndex equipmentIndex)
		{
			if (!EquipmentCatalog.IsIndexValid(equipmentIndex))
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

		// Token: 0x0600056F RID: 1391 RVA: 0x0001614D File Offset: 0x0001434D
		public static EliteDef GetEliteDef(EliteIndex eliteIndex)
		{
			return HGArrayUtilities.GetSafe<EliteDef>(EliteCatalog.eliteDefs, (int)eliteIndex);
		}

		// Token: 0x040005B1 RID: 1457
		public static List<EliteIndex> eliteList = new List<EliteIndex>();

		// Token: 0x040005B2 RID: 1458
		private static EliteDef[] eliteDefs;

		// Token: 0x040005B3 RID: 1459
		public static readonly CatalogModHelper<EliteDef> modHelper = new CatalogModHelper<EliteDef>(delegate(int i, EliteDef def)
		{
			EliteCatalog.RegisterElite((EliteIndex)i, def);
		}, (EliteDef v) => v.name);
	}
}
