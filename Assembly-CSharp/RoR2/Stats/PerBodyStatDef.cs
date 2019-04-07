using System;
using System.Collections.Generic;

namespace RoR2.Stats
{
	// Token: 0x020004F8 RID: 1272
	public class PerBodyStatDef
	{
		// Token: 0x06001CC3 RID: 7363 RVA: 0x000861B8 File Offset: 0x000843B8
		public static void RegisterStatDefs(string[] bodyNames)
		{
			foreach (PerBodyStatDef perBodyStatDef in PerBodyStatDef.instancesList)
			{
				foreach (string text in bodyNames)
				{
					StatDef value = StatDef.Register(perBodyStatDef.prefix + "." + text, perBodyStatDef.recordType, perBodyStatDef.dataType, 0.0, perBodyStatDef.displayValueFormatter);
					perBodyStatDef.bodyNameToStatDefDictionary.Add(text, value);
					perBodyStatDef.bodyNameToStatDefDictionary.Add(text + "(Clone)", value);
				}
			}
		}

		// Token: 0x06001CC4 RID: 7364 RVA: 0x00086278 File Offset: 0x00084478
		private PerBodyStatDef(string prefix, StatRecordType recordType, StatDataType dataType, StatDef.DisplayValueFormatterDelegate displayValueFormatter = null)
		{
			this.prefix = prefix;
			this.recordType = recordType;
			this.dataType = dataType;
			this.displayValueFormatter = displayValueFormatter;
		}

		// Token: 0x06001CC5 RID: 7365 RVA: 0x000862A8 File Offset: 0x000844A8
		private static PerBodyStatDef Register(string prefix, StatRecordType recordType, StatDataType dataType, StatDef.DisplayValueFormatterDelegate displayValueFormatter = null)
		{
			PerBodyStatDef perBodyStatDef = new PerBodyStatDef(prefix, recordType, dataType, displayValueFormatter);
			PerBodyStatDef.instancesList.Add(perBodyStatDef);
			return perBodyStatDef;
		}

		// Token: 0x06001CC6 RID: 7366 RVA: 0x000862CC File Offset: 0x000844CC
		public StatDef FindStatDef(string bodyName)
		{
			StatDef result;
			this.bodyNameToStatDefDictionary.TryGetValue(bodyName, out result);
			return result;
		}

		// Token: 0x04001EF2 RID: 7922
		private readonly string prefix;

		// Token: 0x04001EF3 RID: 7923
		private readonly StatRecordType recordType;

		// Token: 0x04001EF4 RID: 7924
		private readonly StatDataType dataType;

		// Token: 0x04001EF5 RID: 7925
		private readonly StatDef.DisplayValueFormatterDelegate displayValueFormatter;

		// Token: 0x04001EF6 RID: 7926
		private readonly Dictionary<string, StatDef> bodyNameToStatDefDictionary = new Dictionary<string, StatDef>();

		// Token: 0x04001EF7 RID: 7927
		private static readonly List<PerBodyStatDef> instancesList = new List<PerBodyStatDef>();

		// Token: 0x04001EF8 RID: 7928
		public static readonly PerBodyStatDef totalTimeAlive = PerBodyStatDef.Register("totalTimeAlive", StatRecordType.Sum, StatDataType.Double, null);

		// Token: 0x04001EF9 RID: 7929
		public static readonly PerBodyStatDef totalWins = PerBodyStatDef.Register("totalWins", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001EFA RID: 7930
		public static readonly PerBodyStatDef longestRun = PerBodyStatDef.Register("longestRun", StatRecordType.Max, StatDataType.Double, new StatDef.DisplayValueFormatterDelegate(StatDef.TimeMMSSDisplayValueFormatter));

		// Token: 0x04001EFB RID: 7931
		public static readonly PerBodyStatDef damageDealtTo = PerBodyStatDef.Register("damageDealtTo", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001EFC RID: 7932
		public static readonly PerBodyStatDef damageDealtAs = PerBodyStatDef.Register("damageDealtAs", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001EFD RID: 7933
		public static readonly PerBodyStatDef damageTakenFrom = PerBodyStatDef.Register("damageTakenFrom", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001EFE RID: 7934
		public static readonly PerBodyStatDef damageTakenAs = PerBodyStatDef.Register("damageTakenAs", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001EFF RID: 7935
		public static readonly PerBodyStatDef killsAgainst = PerBodyStatDef.Register("killsAgainst", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001F00 RID: 7936
		public static readonly PerBodyStatDef killsAgainstElite = PerBodyStatDef.Register("killsAgainstElite", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001F01 RID: 7937
		public static readonly PerBodyStatDef deathsFrom = PerBodyStatDef.Register("deathsFrom", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001F02 RID: 7938
		public static readonly PerBodyStatDef killsAs = PerBodyStatDef.Register("killsAs", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001F03 RID: 7939
		public static readonly PerBodyStatDef deathsAs = PerBodyStatDef.Register("deathsAs", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001F04 RID: 7940
		public static readonly PerBodyStatDef timesPicked = PerBodyStatDef.Register("timesPicked", StatRecordType.Sum, StatDataType.ULong, null);
	}
}
