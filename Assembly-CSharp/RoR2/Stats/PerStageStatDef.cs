using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace RoR2.Stats
{
	// Token: 0x020004FB RID: 1275
	public class PerStageStatDef
	{
		// Token: 0x06001CD2 RID: 7378 RVA: 0x000865F8 File Offset: 0x000847F8
		public static void RegisterStatDefs()
		{
			foreach (PerStageStatDef perStageStatDef in PerStageStatDef.instancesList)
			{
				foreach (SceneDef sceneDef in SceneCatalog.allSceneDefs)
				{
					string sceneName = sceneDef.sceneName;
					StatDef value = StatDef.Register(perStageStatDef.prefix + "." + sceneName, perStageStatDef.recordType, perStageStatDef.dataType, 0.0, perStageStatDef.displayValueFormatter);
					perStageStatDef.keyToStatDef[sceneName] = value;
				}
			}
		}

		// Token: 0x06001CD3 RID: 7379 RVA: 0x000866C0 File Offset: 0x000848C0
		private PerStageStatDef(string prefix, StatRecordType recordType, StatDataType dataType, StatDef.DisplayValueFormatterDelegate displayValueFormatter)
		{
			this.prefix = prefix;
			this.recordType = recordType;
			this.dataType = dataType;
			this.displayValueFormatter = (displayValueFormatter ?? new StatDef.DisplayValueFormatterDelegate(StatDef.DefaultDisplayValueFormatter));
		}

		// Token: 0x06001CD4 RID: 7380 RVA: 0x00086700 File Offset: 0x00084900
		[NotNull]
		private static PerStageStatDef Register(string prefix, StatRecordType recordType, StatDataType dataType, StatDef.DisplayValueFormatterDelegate displayValueFormatter = null)
		{
			PerStageStatDef perStageStatDef = new PerStageStatDef(prefix, recordType, dataType, displayValueFormatter);
			PerStageStatDef.instancesList.Add(perStageStatDef);
			return perStageStatDef;
		}

		// Token: 0x06001CD5 RID: 7381 RVA: 0x00086724 File Offset: 0x00084924
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

		// Token: 0x04001F14 RID: 7956
		private readonly string prefix;

		// Token: 0x04001F15 RID: 7957
		private readonly StatRecordType recordType;

		// Token: 0x04001F16 RID: 7958
		private readonly StatDataType dataType;

		// Token: 0x04001F17 RID: 7959
		private readonly Dictionary<string, StatDef> keyToStatDef = new Dictionary<string, StatDef>();

		// Token: 0x04001F18 RID: 7960
		private StatDef.DisplayValueFormatterDelegate displayValueFormatter;

		// Token: 0x04001F19 RID: 7961
		private static readonly List<PerStageStatDef> instancesList = new List<PerStageStatDef>();

		// Token: 0x04001F1A RID: 7962
		public static readonly PerStageStatDef totalTimesVisited = PerStageStatDef.Register("totalTimesVisited", StatRecordType.Sum, StatDataType.ULong, null);

		// Token: 0x04001F1B RID: 7963
		public static readonly PerStageStatDef totalTimesCleared = PerStageStatDef.Register("totalTimesCleared", StatRecordType.Sum, StatDataType.ULong, null);
	}
}
