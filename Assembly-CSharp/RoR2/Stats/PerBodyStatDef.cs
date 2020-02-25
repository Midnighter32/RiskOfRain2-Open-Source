using System;
using System.Collections.Generic;

namespace RoR2.Stats
{
	// Token: 0x0200049C RID: 1180
	public class PerBodyStatDef
	{
		// Token: 0x06001C9F RID: 7327 RVA: 0x0007A7C0 File Offset: 0x000789C0
		public static void RegisterStatDefs()
		{
			foreach (PerBodyStatDef perBodyStatDef in PerBodyStatDef.instancesList)
			{
				perBodyStatDef.bodyIndexToStatDef = new StatDef[BodyCatalog.bodyCount];
				for (int i = 0; i < BodyCatalog.bodyCount; i++)
				{
					string bodyName = BodyCatalog.GetBodyName(i);
					StatDef statDef = StatDef.Register(perBodyStatDef.prefix + "." + bodyName, perBodyStatDef.recordType, perBodyStatDef.dataType, 0.0, perBodyStatDef.displayValueFormatter);
					perBodyStatDef.bodyNameToStatDefDictionary.Add(bodyName, statDef);
					perBodyStatDef.bodyNameToStatDefDictionary.Add(bodyName + "(Clone)", statDef);
					perBodyStatDef.bodyIndexToStatDef[i] = statDef;
				}
			}
		}

		// Token: 0x06001CA0 RID: 7328 RVA: 0x0007A89C File Offset: 0x00078A9C
		private PerBodyStatDef(string prefix, StatRecordType recordType, StatDataType dataType, StatDef.DisplayValueFormatterDelegate displayValueFormatter = null)
		{
			this.prefix = prefix;
			this.recordType = recordType;
			this.dataType = dataType;
			this.displayValueFormatter = displayValueFormatter;
		}

		// Token: 0x06001CA1 RID: 7329 RVA: 0x0007A8CC File Offset: 0x00078ACC
		private static PerBodyStatDef Register(string prefix, StatRecordType recordType, StatDataType dataType, StatDef.DisplayValueFormatterDelegate displayValueFormatter = null)
		{
			PerBodyStatDef perBodyStatDef = new PerBodyStatDef(prefix, recordType, dataType, displayValueFormatter);
			PerBodyStatDef.instancesList.Add(perBodyStatDef);
			return perBodyStatDef;
		}

		// Token: 0x06001CA2 RID: 7330 RVA: 0x0007A8F0 File Offset: 0x00078AF0
		public StatDef FindStatDef(string bodyName)
		{
			StatDef result;
			this.bodyNameToStatDefDictionary.TryGetValue(bodyName, out result);
			return result;
		}

		// Token: 0x06001CA3 RID: 7331 RVA: 0x0007A90D File Offset: 0x00078B0D
		public StatDef FindStatDef(int bodyIndex)
		{
			return HGArrayUtilities.GetSafe<StatDef>(this.bodyIndexToStatDef, bodyIndex);
		}

		// Token: 0x040019BC RID: 6588
		private readonly string prefix;

		// Token: 0x040019BD RID: 6589
		private readonly StatRecordType recordType;

		// Token: 0x040019BE RID: 6590
		private readonly StatDataType dataType;

		// Token: 0x040019BF RID: 6591
		private readonly StatDef.DisplayValueFormatterDelegate displayValueFormatter;

		// Token: 0x040019C0 RID: 6592
		private readonly Dictionary<string, StatDef> bodyNameToStatDefDictionary = new Dictionary<string, StatDef>();

		// Token: 0x040019C1 RID: 6593
		private StatDef[] bodyIndexToStatDef;

		// Token: 0x040019C2 RID: 6594
		private static readonly List<PerBodyStatDef> instancesList = new List<PerBodyStatDef>();

		// Token: 0x040019C3 RID: 6595
		public static readonly PerBodyStatDef totalTimeAlive = PerBodyStatDef.Register("totalTimeAlive", StatRecordType.Sum, StatDataType.Double, null);

		// Token: 0x040019C4 RID: 6596
		public static readonly PerBodyStatDef totalWins = PerBodyStatDef.Register("totalWins", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x040019C5 RID: 6597
		public static readonly PerBodyStatDef longestRun = PerBodyStatDef.Register("longestRun", StatRecordType.Max, StatDataType.Double, new StatDef.DisplayValueFormatterDelegate(StatDef.TimeMMSSDisplayValueFormatter));

		// Token: 0x040019C6 RID: 6598
		public static readonly PerBodyStatDef damageDealtTo = PerBodyStatDef.Register("damageDealtTo", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x040019C7 RID: 6599
		public static readonly PerBodyStatDef damageDealtAs = PerBodyStatDef.Register("damageDealtAs", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x040019C8 RID: 6600
		public static readonly PerBodyStatDef minionDamageDealtAs = PerBodyStatDef.Register("minionDamageDealtAs", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x040019C9 RID: 6601
		public static readonly PerBodyStatDef damageTakenFrom = PerBodyStatDef.Register("damageTakenFrom", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x040019CA RID: 6602
		public static readonly PerBodyStatDef damageTakenAs = PerBodyStatDef.Register("damageTakenAs", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x040019CB RID: 6603
		public static readonly PerBodyStatDef killsAgainst = PerBodyStatDef.Register("killsAgainst", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x040019CC RID: 6604
		public static readonly PerBodyStatDef killsAgainstElite = PerBodyStatDef.Register("killsAgainstElite", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x040019CD RID: 6605
		public static readonly PerBodyStatDef deathsFrom = PerBodyStatDef.Register("deathsFrom", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x040019CE RID: 6606
		public static readonly PerBodyStatDef killsAs = PerBodyStatDef.Register("killsAs", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x040019CF RID: 6607
		public static readonly PerBodyStatDef minionKillsAs = PerBodyStatDef.Register("minionKillsAs", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x040019D0 RID: 6608
		public static readonly PerBodyStatDef deathsAs = PerBodyStatDef.Register("deathsAs", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x040019D1 RID: 6609
		public static readonly PerBodyStatDef timesPicked = PerBodyStatDef.Register("timesPicked", StatRecordType.Sum, StatDataType.ULong, null);
	}
}
