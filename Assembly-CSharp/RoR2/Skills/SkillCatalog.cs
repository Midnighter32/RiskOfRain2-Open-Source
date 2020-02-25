using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2.Skills
{
	// Token: 0x020004BA RID: 1210
	public static class SkillCatalog
	{
		// Token: 0x06001D2B RID: 7467 RVA: 0x0007C9E9 File Offset: 0x0007ABE9
		public static SkillDef GetSkillDef(int skillDefIndex)
		{
			return HGArrayUtilities.GetSafe<SkillDef>(SkillCatalog._allSkillDefs, skillDefIndex);
		}

		// Token: 0x06001D2C RID: 7468 RVA: 0x0007C9F6 File Offset: 0x0007ABF6
		public static string GetSkillName(int skillDefIndex)
		{
			return HGArrayUtilities.GetSafe<string>(SkillCatalog._allSkillNames, skillDefIndex);
		}

		// Token: 0x06001D2D RID: 7469 RVA: 0x0007CA03 File Offset: 0x0007AC03
		public static SkillFamily GetSkillFamily(int skillFamilyIndex)
		{
			return HGArrayUtilities.GetSafe<SkillFamily>(SkillCatalog._allSkillFamilies, skillFamilyIndex);
		}

		// Token: 0x06001D2E RID: 7470 RVA: 0x0007CA10 File Offset: 0x0007AC10
		public static string GetSkillFamilyName(int skillFamilyIndex)
		{
			return HGArrayUtilities.GetSafe<string>(SkillCatalog._allSkillFamilyNames, skillFamilyIndex);
		}

		// Token: 0x17000327 RID: 807
		// (get) Token: 0x06001D2F RID: 7471 RVA: 0x0007CA1D File Offset: 0x0007AC1D
		public static IEnumerable<SkillDef> allSkillDefs
		{
			get
			{
				return SkillCatalog._allSkillDefs;
			}
		}

		// Token: 0x17000328 RID: 808
		// (get) Token: 0x06001D30 RID: 7472 RVA: 0x0007CA24 File Offset: 0x0007AC24
		public static IEnumerable<SkillFamily> allSkillFamilies
		{
			get
			{
				return SkillCatalog._allSkillFamilies;
			}
		}

		// Token: 0x06001D31 RID: 7473 RVA: 0x0007CA2C File Offset: 0x0007AC2C
		public static int FindSkillIndexByName(string skillName)
		{
			for (int i = 0; i < SkillCatalog._allSkillDefs.Length; i++)
			{
				if (SkillCatalog._allSkillDefs[i].skillName == skillName)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x1400006D RID: 109
		// (add) Token: 0x06001D32 RID: 7474 RVA: 0x0007CA64 File Offset: 0x0007AC64
		// (remove) Token: 0x06001D33 RID: 7475 RVA: 0x0007CA98 File Offset: 0x0007AC98
		public static event Action<List<SkillDef>> getAdditionalSkillDefs;

		// Token: 0x1400006E RID: 110
		// (add) Token: 0x06001D34 RID: 7476 RVA: 0x0007CACC File Offset: 0x0007ACCC
		// (remove) Token: 0x06001D35 RID: 7477 RVA: 0x0007CB00 File Offset: 0x0007AD00
		public static event Action<List<SkillFamily>> getAdditionalSkillFamilies;

		// Token: 0x06001D36 RID: 7478 RVA: 0x0007CB34 File Offset: 0x0007AD34
		[SystemInitializer(new Type[]
		{
			typeof(BodyCatalog)
		})]
		private static void Init()
		{
			SkillCatalog._allSkillDefs = Resources.LoadAll<SkillDef>("SkillDefs");
			List<SkillDef> list = new List<SkillDef>();
			Action<List<SkillDef>> action = SkillCatalog.getAdditionalSkillDefs;
			if (action != null)
			{
				action(list);
			}
			SkillCatalog._allSkillDefs = SkillCatalog._allSkillDefs.Concat(list.OrderBy((SkillDef v) => v.name, StringComparer.Ordinal)).ToArray<SkillDef>();
			SkillCatalog._allSkillFamilies = Resources.LoadAll<SkillFamily>("SkillDefs");
			List<SkillFamily> list2 = new List<SkillFamily>();
			Action<List<SkillFamily>> action2 = SkillCatalog.getAdditionalSkillFamilies;
			if (action2 != null)
			{
				action2(list2);
			}
			SkillCatalog._allSkillFamilies = SkillCatalog._allSkillFamilies.Concat(list2.OrderBy((SkillFamily v) => v.name, StringComparer.Ordinal)).ToArray<SkillFamily>();
			SkillCatalog._allSkillNames = new string[SkillCatalog._allSkillDefs.Length];
			SkillCatalog._allSkillFamilyNames = new string[SkillCatalog._allSkillFamilies.Length];
			for (int i = 0; i < SkillCatalog._allSkillDefs.Length; i++)
			{
				SkillCatalog._allSkillDefs[i].skillIndex = i;
				SkillCatalog._allSkillNames[i] = SkillCatalog._allSkillDefs[i].name;
			}
			for (int j = 0; j < SkillCatalog._allSkillFamilies.Length; j++)
			{
				SkillCatalog._allSkillFamilies[j].catalogIndex = j;
				SkillCatalog._allSkillFamilyNames[j] = SkillCatalog._allSkillFamilies[j].name;
			}
			SkillCatalog.skillsDefined.MakeAvailable();
		}

		// Token: 0x04001A2D RID: 6701
		private static SkillDef[] _allSkillDefs;

		// Token: 0x04001A2E RID: 6702
		private static string[] _allSkillNames;

		// Token: 0x04001A2F RID: 6703
		private static SkillFamily[] _allSkillFamilies;

		// Token: 0x04001A30 RID: 6704
		private static string[] _allSkillFamilyNames;

		// Token: 0x04001A33 RID: 6707
		public static ResourceAvailability skillsDefined;
	}
}
