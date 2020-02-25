using System;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;

namespace RoR2.Stats
{
	// Token: 0x0200049A RID: 1178
	public class StatDef
	{
		// Token: 0x06001C92 RID: 7314 RVA: 0x00079FF0 File Offset: 0x000781F0
		[CanBeNull]
		public static StatDef Find(string statName)
		{
			StatDef result;
			StatDef.nameToStatDef.TryGetValue(statName, out result);
			return result;
		}

		// Token: 0x06001C93 RID: 7315 RVA: 0x0007A00C File Offset: 0x0007820C
		private StatDef(string name, StatRecordType recordType, StatDataType dataType, double pointValue, StatDef.DisplayValueFormatterDelegate displayValueFormatter)
		{
			this.name = name;
			this.recordType = recordType;
			this.dataType = dataType;
			this.pointValue = pointValue;
			this.displayValueFormatter = displayValueFormatter;
			this.displayToken = "STATNAME_" + name.ToUpper(CultureInfo.InvariantCulture);
		}

		// Token: 0x06001C95 RID: 7317 RVA: 0x0007A508 File Offset: 0x00078708
		[SystemInitializer(new Type[]
		{

		})]
		private static void Init()
		{
			BodyCatalog.availability.CallWhenAvailable(new Action(PerBodyStatDef.RegisterStatDefs));
			ItemCatalog.availability.CallWhenAvailable(new Action(PerItemStatDef.RegisterStatDefs));
			EquipmentCatalog.availability.CallWhenAvailable(new Action(PerEquipmentStatDef.RegisterStatDefs));
			SceneCatalog.availability.CallWhenAvailable(new Action(PerStageStatDef.RegisterStatDefs));
		}

		// Token: 0x06001C96 RID: 7318 RVA: 0x0007A570 File Offset: 0x00078770
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

		// Token: 0x06001C97 RID: 7319 RVA: 0x0007A5C8 File Offset: 0x000787C8
		public static string DefaultDisplayValueFormatter(ref StatField statField)
		{
			return statField.ToLocalNumeric();
		}

		// Token: 0x06001C98 RID: 7320 RVA: 0x0007A5D0 File Offset: 0x000787D0
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

		// Token: 0x06001C99 RID: 7321 RVA: 0x0007A630 File Offset: 0x00078830
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

		// Token: 0x04001988 RID: 6536
		public static readonly List<StatDef> allStatDefs = new List<StatDef>();

		// Token: 0x04001989 RID: 6537
		private static readonly Dictionary<string, StatDef> nameToStatDef = new Dictionary<string, StatDef>();

		// Token: 0x0400198A RID: 6538
		public int index;

		// Token: 0x0400198B RID: 6539
		public readonly string name;

		// Token: 0x0400198C RID: 6540
		public readonly string displayToken;

		// Token: 0x0400198D RID: 6541
		public readonly StatRecordType recordType;

		// Token: 0x0400198E RID: 6542
		public readonly StatDataType dataType;

		// Token: 0x0400198F RID: 6543
		public double pointValue;

		// Token: 0x04001990 RID: 6544
		public readonly StatDef.DisplayValueFormatterDelegate displayValueFormatter;

		// Token: 0x04001991 RID: 6545
		public static readonly StatDef totalGamesPlayed = StatDef.Register("totalGamesPlayed", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001992 RID: 6546
		public static readonly StatDef totalTimeAlive = StatDef.Register("totalTimeAlive", StatRecordType.Sum, StatDataType.Double, 1.0, new StatDef.DisplayValueFormatterDelegate(StatDef.TimeMMSSDisplayValueFormatter));

		// Token: 0x04001993 RID: 6547
		public static readonly StatDef totalKills = StatDef.Register("totalKills", StatRecordType.Sum, StatDataType.ULong, 10.0, null);

		// Token: 0x04001994 RID: 6548
		public static readonly StatDef totalMinionKills = StatDef.Register("totalMinionKills", StatRecordType.Sum, StatDataType.ULong, 10.0, null);

		// Token: 0x04001995 RID: 6549
		public static readonly StatDef totalDeaths = StatDef.Register("totalDeaths", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001996 RID: 6550
		public static readonly StatDef totalDamageDealt = StatDef.Register("totalDamageDealt", StatRecordType.Sum, StatDataType.ULong, 0.01, null);

		// Token: 0x04001997 RID: 6551
		public static readonly StatDef totalMinionDamageDealt = StatDef.Register("totalMinionDamageDealt", StatRecordType.Sum, StatDataType.ULong, 0.01, null);

		// Token: 0x04001998 RID: 6552
		public static readonly StatDef totalDamageTaken = StatDef.Register("totalDamageTaken", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x04001999 RID: 6553
		public static readonly StatDef totalHealthHealed = StatDef.Register("totalHealthHealed", StatRecordType.Sum, StatDataType.ULong, 0.01, null);

		// Token: 0x0400199A RID: 6554
		public static readonly StatDef highestDamageDealt = StatDef.Register("highestDamageDealt", StatRecordType.Max, StatDataType.ULong, 1.0, null);

		// Token: 0x0400199B RID: 6555
		public static readonly StatDef highestLevel = StatDef.Register("highestLevel", StatRecordType.Max, StatDataType.ULong, 100.0, null);

		// Token: 0x0400199C RID: 6556
		public static readonly StatDef goldCollected = StatDef.Register("totalGoldCollected", StatRecordType.Sum, StatDataType.ULong, 1.0, null);

		// Token: 0x0400199D RID: 6557
		public static readonly StatDef maxGoldCollected = StatDef.Register("maxGoldCollected", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x0400199E RID: 6558
		public static readonly StatDef totalDistanceTraveled = StatDef.Register("totalDistanceTraveled", StatRecordType.Sum, StatDataType.Double, 0.01, new StatDef.DisplayValueFormatterDelegate(StatDef.DistanceMarathonsDisplayValueFormatter));

		// Token: 0x0400199F RID: 6559
		public static readonly StatDef totalItemsCollected = StatDef.Register("totalItemsCollected", StatRecordType.Sum, StatDataType.ULong, 110.0, null);

		// Token: 0x040019A0 RID: 6560
		public static readonly StatDef highestItemsCollected = StatDef.Register("highestItemsCollected", StatRecordType.Max, StatDataType.ULong, 10.0, null);

		// Token: 0x040019A1 RID: 6561
		public static readonly StatDef totalStagesCompleted = StatDef.Register("totalStagesCompleted", StatRecordType.Sum, StatDataType.ULong, 100.0, null);

		// Token: 0x040019A2 RID: 6562
		public static readonly StatDef highestStagesCompleted = StatDef.Register("highestStagesCompleted", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x040019A3 RID: 6563
		public static readonly StatDef totalPurchases = StatDef.Register("totalPurchases", StatRecordType.Sum, StatDataType.ULong, 35.0, null);

		// Token: 0x040019A4 RID: 6564
		public static readonly StatDef highestPurchases = StatDef.Register("highestPurchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x040019A5 RID: 6565
		public static readonly StatDef totalGoldPurchases = StatDef.Register("totalGoldPurchases", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x040019A6 RID: 6566
		public static readonly StatDef highestGoldPurchases = StatDef.Register("highestGoldPurchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x040019A7 RID: 6567
		public static readonly StatDef totalBloodPurchases = StatDef.Register("totalBloodPurchases", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x040019A8 RID: 6568
		public static readonly StatDef highestBloodPurchases = StatDef.Register("highestBloodPurchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x040019A9 RID: 6569
		public static readonly StatDef totalLunarPurchases = StatDef.Register("totalLunarPurchases", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x040019AA RID: 6570
		public static readonly StatDef highestLunarPurchases = StatDef.Register("highestLunarPurchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x040019AB RID: 6571
		public static readonly StatDef totalTier1Purchases = StatDef.Register("totalTier1Purchases", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x040019AC RID: 6572
		public static readonly StatDef highestTier1Purchases = StatDef.Register("highestTier1Purchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x040019AD RID: 6573
		public static readonly StatDef totalTier2Purchases = StatDef.Register("totalTier2Purchases", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x040019AE RID: 6574
		public static readonly StatDef highestTier2Purchases = StatDef.Register("highestTier2Purchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x040019AF RID: 6575
		public static readonly StatDef totalTier3Purchases = StatDef.Register("totalTier3Purchases", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x040019B0 RID: 6576
		public static readonly StatDef highestTier3Purchases = StatDef.Register("highestTier3Purchases", StatRecordType.Max, StatDataType.ULong, 0.0, null);

		// Token: 0x040019B1 RID: 6577
		public static readonly StatDef totalDronesPurchased = StatDef.Register("totalDronesPurchased", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x040019B2 RID: 6578
		public static readonly StatDef totalGreenSoupsPurchased = StatDef.Register("totalGreenSoupsPurchased", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x040019B3 RID: 6579
		public static readonly StatDef totalRedSoupsPurchased = StatDef.Register("totalRedSoupsPurchased", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x040019B4 RID: 6580
		public static readonly StatDef suicideHermitCrabsAchievementProgress = StatDef.Register("suicideHermitCrabsAchievementProgress", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x040019B5 RID: 6581
		public static readonly StatDef firstTeleporterCompleted = StatDef.Register("firstTeleporterCompleted", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x040019B6 RID: 6582
		public static readonly StatDef totalEliteKills = StatDef.Register("totalEliteKills", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x040019B7 RID: 6583
		public static readonly StatDef totalBurnDeaths = StatDef.Register("totalBurnDeaths", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x040019B8 RID: 6584
		public static readonly StatDef totalDeathsWhileBurning = StatDef.Register("totalDeathsWhileBurning", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x040019B9 RID: 6585
		public static readonly StatDef totalTeleporterBossKillsWitnessed = StatDef.Register("totalTeleporterBossKillsWitnessed", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x040019BA RID: 6586
		public static readonly StatDef totalCrocoInfectionsInflicted = StatDef.Register("totalCrocoInfectionsInflicted", StatRecordType.Sum, StatDataType.ULong, 0.0, null);

		// Token: 0x040019BB RID: 6587
		private static string[] bodyNames;

		// Token: 0x0200049B RID: 1179
		// (Invoke) Token: 0x06001C9B RID: 7323
		public delegate string DisplayValueFormatterDelegate(ref StatField statField);
	}
}
