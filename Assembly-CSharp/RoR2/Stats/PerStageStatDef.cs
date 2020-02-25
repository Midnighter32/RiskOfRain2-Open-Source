using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace RoR2.Stats
{
	// Token: 0x0200049F RID: 1183
	public class PerStageStatDef
	{
		// Token: 0x06001CAF RID: 7343 RVA: 0x0007AC1C File Offset: 0x00078E1C
		public static void RegisterStatDefs()
		{
			foreach (PerStageStatDef perStageStatDef in PerStageStatDef.instancesList)
			{
				foreach (SceneDef sceneDef in SceneCatalog.allSceneDefs)
				{
					string baseSceneName = sceneDef.baseSceneName;
					string text = perStageStatDef.prefix + "." + baseSceneName;
					Debug.LogFormat("Registering key '{0}' with stat name '{1}'", new object[]
					{
						baseSceneName,
						text
					});
					StatDef value = StatDef.Register(text, perStageStatDef.recordType, perStageStatDef.dataType, 0.0, perStageStatDef.displayValueFormatter);
					perStageStatDef.keyToStatDef[baseSceneName] = value;
				}
			}
		}

		// Token: 0x06001CB0 RID: 7344 RVA: 0x0007AD04 File Offset: 0x00078F04
		private PerStageStatDef(string prefix, StatRecordType recordType, StatDataType dataType, StatDef.DisplayValueFormatterDelegate displayValueFormatter)
		{
			this.prefix = prefix;
			this.recordType = recordType;
			this.dataType = dataType;
			this.displayValueFormatter = (displayValueFormatter ?? new StatDef.DisplayValueFormatterDelegate(StatDef.DefaultDisplayValueFormatter));
		}

		// Token: 0x06001CB1 RID: 7345 RVA: 0x0007AD44 File Offset: 0x00078F44
		[NotNull]
		private static PerStageStatDef Register(string prefix, StatRecordType recordType, StatDataType dataType, StatDef.DisplayValueFormatterDelegate displayValueFormatter = null)
		{
			PerStageStatDef perStageStatDef = new PerStageStatDef(prefix, recordType, dataType, displayValueFormatter);
			PerStageStatDef.instancesList.Add(perStageStatDef);
			return perStageStatDef;
		}

		// Token: 0x06001CB2 RID: 7346 RVA: 0x0007AD68 File Offset: 0x00078F68
		[CanBeNull]
		public StatDef FindStatDef(string key)
		{
			StatDef result;
			if (this.keyToStatDef.TryGetValue(key, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x040019E1 RID: 6625
		private readonly string prefix;

		// Token: 0x040019E2 RID: 6626
		private readonly StatRecordType recordType;

		// Token: 0x040019E3 RID: 6627
		private readonly StatDataType dataType;

		// Token: 0x040019E4 RID: 6628
		private readonly Dictionary<string, StatDef> keyToStatDef = new Dictionary<string, StatDef>();

		// Token: 0x040019E5 RID: 6629
		private StatDef.DisplayValueFormatterDelegate displayValueFormatter;

		// Token: 0x040019E6 RID: 6630
		private static readonly List<PerStageStatDef> instancesList = new List<PerStageStatDef>();

		// Token: 0x040019E7 RID: 6631
		public static readonly PerStageStatDef totalTimesVisited = PerStageStatDef.Register("totalTimesVisited", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x040019E8 RID: 6632
		public static readonly PerStageStatDef totalTimesCleared = PerStageStatDef.Register("totalTimesCleared", StatRecordType.Sum, StatDataType.ULong, null);
	}
}
