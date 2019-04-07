using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace RoR2.Stats
{
	// Token: 0x020004F5 RID: 1269
	public class StatDef
	{
		// Token: 0x06001CB2 RID: 7346 RVA: 0x00085A9C File Offset: 0x00083C9C
		[CanBeNull]
		public static StatDef Find(string statName)
		{
			StatDef result;
			StatDef.nameToStatDef.TryGetValue(statName, out result);
			return result;
		}

		// Token: 0x06001CB3 RID: 7347 RVA: 0x00085AB8 File Offset: 0x00083CB8
		private StatDef(string name, StatRecordType recordType, StatDataType dataType, double pointValue, StatDef.DisplayValueFormatterDelegate displayValueFormatter)
		{
			this.name = name;
			this.recordType = recordType;
			this.dataType = dataType;
			this.pointValue = pointValue;
			this.displayValueFormatter = displayValueFormatter;
			this.displayToken = "STATNAME_" + name.ToUpper();
		}

		// Token: 0x06001CB5 RID: 7349 RVA: 0x00085EF0 File Offset: 0x000840F0
		[SystemInitializer(new Type[]
		{
			typeof(BodyCatalog),
			typeof(SceneCatalog)
		})]
		private static void Init()
		{
			BodyCatalog.availability.CallWhenAvailable(delegate
			{
				StatDef.bodyNames = (from gameObject in BodyCatalog.allBodyPrefabs
				select gameObject.name).ToArray<string>();
				PerBodyStatDef.RegisterStatDefs(StatDef.bodyNames);
				PerItemStatDef.RegisterStatDefs();
				PerEquipmentStatDef.RegisterStatDefs();
				PerStageStatDef.RegisterStatDefs();
			});
		}

		// Token: 0x06001CB6 RID: 7350 RVA: 0x00085F1C File Offset: 0x0008411C
		public static StatDef Register(string name, StatRecordType recordType, StatDataType dataType, double pointValue, StatDef.DisplayValueFormatterDelegate displayValueFormatter = null)
		{
			if (displayValueFormatter == null)
			{
				displayValueFormatter = new StatDef.DisplayValueFormatterDelegate(StatDef.DefaultDisplayValueFormatter);
			}
			StatDef statDef = new StatDef(name, recordType, dataType, pointValue, displayValueFormatter)
			{
				index = StatDef.allStatDefs.Count
			};
			StatDef.allStatDefs.Add(statDef);
			StatDef.nameToStatDef.Add(statDef.name, statDef);
			return statDef;
		}

		// Token: 0x06001CB7 RID: 7351 RVA: 0x00085F74 File Offset: 0x00084174
		public static string DefaultDisplayValueFormatter(ref StatField statField)
		{
			return statField.ToString();
		}

		// Token: 0x06001CB8 RID: 7352 RVA: 0x00085F84 File Offset: 0x00084184
		public static string TimeMMSSDisplayValueFormatter(ref StatField statField)
		{
			StatDataType statDataType = statField.dataType;
			ulong num;
			if (statDataType != StatDataType.ULong)
			{
				if (statDataType != StatDataType.Double)
				{
					throw new ArgumentOutOfRangeException();
				}
				num = (ulong)statField.GetDoubleValue();
			}
			else
			{
				num = statField.GetULongValue();
			}
			ulong num2 = num / 60UL;
			ulong num3 = num - num2 * 60UL;
			return string.Format("{0:00}:{1:00}", num2, num3);
		}

		// Token: 0x06001CB9 RID: 7353 RVA: 0x00085FE4 File Offset: 0x000841E4
		public static string DistanceMarathonsDisplayValueFormatter(ref StatField statField)
		{
			StatDataType statDataType = statField.dataType;
			double num;
			if (statDataType != StatDataType.ULong)
			{
				if (statDataType != StatDataType.Double)
				{
					throw new ArgumentOutOfRangeException();
				}
				num = statField.GetDoubleValue();
			}
			else
			{
				num = statField.GetULongValue();
			}
			return string.Format(Language.GetString("STAT_VALUE_MARATHONS_FORMAT"), num * 2.3699E-05);
		}

		// Token: 0x04001EC2 RID: 7874
		public static readonly List<StatDef> allStatDefs = new List<StatDef>();

		// Token: 0x04001EC3 RID: 7875
		private static readonly Dictionary<string, StatDef> nameToStatDef = new Dictionary<string, StatDef>();

		// Token: 0x04001EC4 RID: 7876
		public int index;

		// Token: 0x04001EC5 RID: 7877
		public readonly string name;

		// Token: 0x04001EC6 RID: 7878
		public readonly string displayToken;

		// Token: 0x04001EC7 RID: 7879
		public readonly StatRecordType recordType;

		// Token: 0x04001EC8 RID: 7880
		public readonly StatDataType dataType;

		// Token: 0x04001EC9 RID: 7881
		public double pointValue;

		// Token: 0x04001ECA RID: 7882
		public readonly StatDef.DisplayValueFormatterDelegate displayValueFormatter;

		// Token: 0x04001ECB RID: 7883
		public static readonly StatDef totalGamesPlayed = StatDef.Register("totalGamesPlayed", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001ECC RID: 7884
		public static readonly StatDef totalTimeAlive = StatDef.Register("totalTimeAlive", StatRecordType.Sum, StatDataType.Double, 1.0, new StatDef.DisplayValueFormatterDelegate(StatDef.TimeMMSSDisplayValueFormatter));

		// Token: 0x04001ECD RID: 7885
		public static readonly StatDef totalKills = StatDef.Register("totalKills", StatRecordType.Sum, StatDataType.ULong, 10.0, null);

		// Token: 0x04001ECE RID: 7886
		public static readonly StatDef totalDeaths = StatDef.Register("totalDeaths", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001ECF RID: 7887
		public static readonly StatDef totalDamageDealt = StatDef.Register("totalDamageDealt", StatRecordType.Sum, StatDataType.ULong, 0.01, null);

		// Token: 0x04001ED0 RID: 7888
		public static readonly StatDef totalDamageTaken = StatDef.Register("totalDamageTaken", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001ED1 RID: 7889
		public static readonly StatDef totalHealthHealed = StatDef.Register("totalHealthHealed", StatRecordType.Sum, StatDataType.ULong, 0.01, null);

		// Token: 0x04001ED2 RID: 7890
		public static readonly StatDef highestDamageDealt = StatDef.Register("highestDamageDealt", StatRecordType.Max, StatDataType.ULong, 1.0, null);

		// Token: 0x04001ED3 RID: 7891
		public static readonly StatDef highestLevel = StatDef.Register("highestLevel", StatRecordType.Max, StatDataType.ULong, 100.0, null);

		// Token: 0x04001ED4 RID: 7892
		public static readonly StatDef goldCollected = StatDef.Register("totalGoldCollected", StatRecordType.Sum, StatDataType.ULong, 1.0, null);

		// Token: 0x04001ED5 RID: 7893
		public static readonly StatDef maxGoldCollected = StatDef.Register("maxGoldCollected", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x04001ED6 RID: 7894
		public static readonly StatDef totalDistanceTraveled = StatDef.Register("totalDistanceTraveled", StatRecordType.Sum, StatDataType.Double, 0.01, new StatDef.DisplayValueFormatterDelegate(StatDef.DistanceMarathonsDisplayValueFormatter));

		// Token: 0x04001ED7 RID: 7895
		public static readonly StatDef totalItemsCollected = StatDef.Register("totalItemsCollected", StatRecordType.Sum, StatDataType.ULong, 110.0, null);

		// Token: 0x04001ED8 RID: 7896
		public static readonly StatDef highestItemsCollected = StatDef.Register("highestItemsCollected", StatRecordType.Max, StatDataType.ULong, 10.0, null);

		// Token: 0x04001ED9 RID: 7897
		public static readonly StatDef totalStagesCompleted = StatDef.Register("totalStagesCompleted", StatRecordType.Sum, StatDataType.ULong, 100.0, null);

		// Token: 0x04001EDA RID: 7898
		public static readonly StatDef highestStagesCompleted = StatDef.Register("highestStagesCompleted", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EDB RID: 7899
		public static readonly StatDef totalPurchases = StatDef.Register("totalPurchases", StatRecordType.Sum, StatDataType.ULong, 35.0, null);

		// Token: 0x04001EDC RID: 7900
		public static readonly StatDef highestPurchases = StatDef.Register("highestPurchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EDD RID: 7901
		public static readonly StatDef totalGoldPurchases = StatDef.Register("totalGoldPurchases", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EDE RID: 7902
		public static readonly StatDef highestGoldPurchases = StatDef.Register("highestGoldPurchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EDF RID: 7903
		public static readonly StatDef totalBloodPurchases = StatDef.Register("totalBloodPurchases", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EE0 RID: 7904
		public static readonly StatDef highestBloodPurchases = StatDef.Register("highestBloodPurchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EE1 RID: 7905
		public static readonly StatDef totalLunarPurchases = StatDef.Register("totalLunarPurchases", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EE2 RID: 7906
		public static readonly StatDef highestLunarPurchases = StatDef.Register("highestLunarPurchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EE3 RID: 7907
		public static readonly StatDef totalTier1Purchases = StatDef.Register("totalTier1Purchases", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EE4 RID: 7908
		public static readonly StatDef highestTier1Purchases = StatDef.Register("highestTier1Purchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EE5 RID: 7909
		public static readonly StatDef totalTier2Purchases = StatDef.Register("totalTier2Purchases", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EE6 RID: 7910
		public static readonly StatDef highestTier2Purchases = StatDef.Register("highestTier2Purchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EE7 RID: 7911
		public static readonly StatDef totalTier3Purchases = StatDef.Register("totalTier3Purchases", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EE8 RID: 7912
		public static readonly StatDef highestTier3Purchases = StatDef.Register("highestTier3Purchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EE9 RID: 7913
		public static readonly StatDef totalDronesPurchased = StatDef.Register("totalDronesPurchased", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EEA RID: 7914
		public static readonly StatDef totalGreenSoupsPurchased = StatDef.Register("totalGreenSoupsPurchased", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EEB RID: 7915
		public static readonly StatDef totalRedSoupsPurchased = StatDef.Register("totalRedSoupsPurchased", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EEC RID: 7916
		public static readonly StatDef suicideHermitCrabsAchievementProgress = StatDef.Register("suicideHermitCrabsAchievementProgress", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EED RID: 7917
		public static readonly StatDef firstTeleporterCompleted = StatDef.Register("firstTeleporterCompleted", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001EEE RID: 7918
		private static string[] bodyNames;

		// Token: 0x020004F6 RID: 1270
		// (Invoke) Token: 0x06001CBB RID: 7355
		public delegate string DisplayValueFormatterDelegate(ref StatField statField);
	}
}
