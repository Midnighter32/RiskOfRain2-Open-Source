using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace RoR2.Stats
{
	// Token: 0x020004A4 RID: 1188
	internal class StatManager
	{
		// Token: 0x06001CD0 RID: 7376 RVA: 0x0007B250 File Offset: 0x00079450
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			GlobalEventManager.onServerDamageDealt += StatManager.OnDamageDealt;
			GlobalEventManager.onCharacterDeathGlobal += StatManager.OnCharacterDeath;
			GlobalEventManager.onServerCharacterExecuted += StatManager.OnCharacterExecute;
			HealthComponent.onCharacterHealServer += StatManager.OnCharacterHeal;
			Run.onPlayerFirstCreatedServer += StatManager.OnPlayerFirstCreatedServer;
			Run.OnServerGameOver += StatManager.OnServerGameOver;
			Stage.onServerStageComplete += StatManager.OnServerStageComplete;
			Stage.onServerStageBegin += StatManager.OnServerStageBegin;
			Inventory.onServerItemGiven += StatManager.OnServerItemGiven;
			RoR2Application.onFixedUpdate += StatManager.ProcessEvents;
			EquipmentSlot.onServerEquipmentActivated += StatManager.OnEquipmentActivated;
		}

		// Token: 0x06001CD1 RID: 7377 RVA: 0x0007B318 File Offset: 0x00079518
		private static void OnServerGameOver(Run run, GameResultType result)
		{
			if (result != GameResultType.Lost)
			{
				foreach (PlayerStatsComponent playerStatsComponent in PlayerStatsComponent.instancesList)
				{
					if (playerStatsComponent.playerCharacterMasterController.isConnected)
					{
						StatSheet currentStats = playerStatsComponent.currentStats;
						PerBodyStatDef totalWins = PerBodyStatDef.totalWins;
						GameObject bodyPrefab = playerStatsComponent.characterMaster.bodyPrefab;
						currentStats.PushStatValue(totalWins.FindStatDef(((bodyPrefab != null) ? bodyPrefab.name : null) ?? ""), 1UL);
					}
				}
			}
		}

		// Token: 0x06001CD2 RID: 7378 RVA: 0x0007B3AC File Offset: 0x000795AC
		private static void OnPlayerFirstCreatedServer(Run run, PlayerCharacterMasterController playerCharacterMasterController)
		{
			playerCharacterMasterController.master.onBodyStart += StatManager.OnBodyFirstStart;
		}

		// Token: 0x06001CD3 RID: 7379 RVA: 0x0007B3C8 File Offset: 0x000795C8
		private static void OnBodyFirstStart(CharacterBody body)
		{
			CharacterMaster master = body.master;
			if (master)
			{
				master.onBodyStart -= StatManager.OnBodyFirstStart;
				PlayerCharacterMasterController component = master.GetComponent<PlayerCharacterMasterController>();
				if (component)
				{
					PlayerStatsComponent component2 = component.GetComponent<PlayerStatsComponent>();
					if (component2)
					{
						StatSheet currentStats = component2.currentStats;
						currentStats.PushStatValue(PerBodyStatDef.timesPicked.FindStatDef(body.name), 1UL);
						currentStats.PushStatValue(StatDef.totalGamesPlayed, 1UL);
					}
				}
			}
		}

		// Token: 0x06001CD4 RID: 7380 RVA: 0x0007B43E File Offset: 0x0007963E
		private static void ProcessEvents()
		{
			StatManager.ProcessDamageEvents();
			StatManager.ProcessDeathEvents();
			StatManager.ProcessHealingEvents();
			StatManager.ProcessGoldEvents();
			StatManager.ProcessItemCollectedEvents();
			StatManager.ProcessCharacterUpdateEvents();
		}

		// Token: 0x06001CD5 RID: 7381 RVA: 0x0007B460 File Offset: 0x00079660
		public static void OnCharacterHeal(HealthComponent healthComponent, float amount)
		{
			StatManager.healingEvents.Enqueue(new StatManager.HealingEvent
			{
				healee = healthComponent.gameObject,
				healAmount = amount
			});
		}

		// Token: 0x06001CD6 RID: 7382 RVA: 0x0007B498 File Offset: 0x00079698
		public static void OnDamageDealt(DamageReport damageReport)
		{
			StatManager.damageEvents.Enqueue(new StatManager.DamageEvent
			{
				attackerMaster = damageReport.attackerMaster,
				attackerBodyIndex = damageReport.attackerBodyIndex,
				attackerOwnerMaster = damageReport.attackerOwnerMaster,
				attackerOwnerBodyIndex = damageReport.attackerOwnerBodyIndex,
				victimMaster = damageReport.victimMaster,
				victimBodyIndex = damageReport.victimBodyIndex,
				victimIsElite = damageReport.victimIsElite,
				damageDealt = damageReport.damageDealt,
				dotType = damageReport.dotType
			});
		}

		// Token: 0x06001CD7 RID: 7383 RVA: 0x0007B530 File Offset: 0x00079730
		public static void OnCharacterExecute(DamageReport damageReport, float executionHealthLost)
		{
			StatManager.damageEvents.Enqueue(new StatManager.DamageEvent
			{
				attackerMaster = damageReport.attackerMaster,
				attackerBodyIndex = damageReport.attackerBodyIndex,
				attackerOwnerMaster = damageReport.attackerOwnerMaster,
				attackerOwnerBodyIndex = damageReport.attackerOwnerBodyIndex,
				victimMaster = damageReport.victimMaster,
				victimBodyIndex = damageReport.victimBodyIndex,
				victimIsElite = damageReport.victimIsElite,
				damageDealt = executionHealthLost,
				dotType = damageReport.dotType
			});
		}

		// Token: 0x06001CD8 RID: 7384 RVA: 0x0007B5C0 File Offset: 0x000797C0
		public static void OnCharacterDeath(DamageReport damageReport)
		{
			DotController dotController = DotController.FindDotController(damageReport.victim.gameObject);
			bool victimWasBurning = false;
			if (dotController)
			{
				victimWasBurning = (dotController.HasDotActive(DotController.DotIndex.Burn) | dotController.HasDotActive(DotController.DotIndex.PercentBurn) | dotController.HasDotActive(DotController.DotIndex.Helfire));
			}
			StatManager.deathEvents.Enqueue(new StatManager.DeathEvent
			{
				damageReport = damageReport,
				victimWasBurning = victimWasBurning
			});
		}

		// Token: 0x06001CD9 RID: 7385 RVA: 0x0007B624 File Offset: 0x00079824
		private static void ProcessHealingEvents()
		{
			while (StatManager.healingEvents.Count > 0)
			{
				StatManager.HealingEvent healingEvent = StatManager.healingEvents.Dequeue();
				ulong statValue = (ulong)healingEvent.healAmount;
				StatSheet statSheet = PlayerStatsComponent.FindBodyStatSheet(healingEvent.healee);
				if (statSheet != null)
				{
					statSheet.PushStatValue(StatDef.totalHealthHealed, statValue);
				}
			}
		}

		// Token: 0x06001CDA RID: 7386 RVA: 0x0007B66C File Offset: 0x0007986C
		private static void ProcessDamageEvents()
		{
			while (StatManager.damageEvents.Count > 0)
			{
				StatManager.DamageEvent damageEvent = StatManager.damageEvents.Dequeue();
				ulong statValue = (ulong)damageEvent.damageDealt;
				StatSheet statSheet = PlayerStatsComponent.FindMasterStatSheet(damageEvent.victimMaster);
				StatSheet statSheet2 = PlayerStatsComponent.FindMasterStatSheet(damageEvent.attackerMaster);
				StatSheet statSheet3 = PlayerStatsComponent.FindMasterStatSheet(damageEvent.attackerOwnerMaster);
				if (statSheet != null)
				{
					statSheet.PushStatValue(StatDef.totalDamageTaken, statValue);
					if (damageEvent.attackerBodyIndex != -1)
					{
						statSheet.PushStatValue(PerBodyStatDef.damageTakenFrom, damageEvent.attackerBodyIndex, statValue);
					}
					if (damageEvent.victimBodyIndex != -1)
					{
						statSheet.PushStatValue(PerBodyStatDef.damageTakenAs, damageEvent.victimBodyIndex, statValue);
					}
				}
				if (statSheet2 != null)
				{
					statSheet2.PushStatValue(StatDef.totalDamageDealt, statValue);
					statSheet2.PushStatValue(StatDef.highestDamageDealt, statValue);
					if (damageEvent.attackerBodyIndex != -1)
					{
						statSheet2.PushStatValue(PerBodyStatDef.damageDealtAs, damageEvent.attackerBodyIndex, statValue);
					}
					if (damageEvent.victimBodyIndex != -1)
					{
						statSheet2.PushStatValue(PerBodyStatDef.damageDealtTo, damageEvent.victimBodyIndex, statValue);
					}
				}
				if (statSheet3 != null)
				{
					statSheet3.PushStatValue(StatDef.totalMinionDamageDealt, statValue);
					if (damageEvent.attackerOwnerBodyIndex != -1)
					{
						statSheet3.PushStatValue(PerBodyStatDef.minionDamageDealtAs, damageEvent.attackerOwnerBodyIndex, statValue);
					}
				}
			}
		}

		// Token: 0x06001CDB RID: 7387 RVA: 0x0007B78C File Offset: 0x0007998C
		private static void ProcessDeathEvents()
		{
			while (StatManager.deathEvents.Count > 0)
			{
				StatManager.DeathEvent deathEvent = StatManager.deathEvents.Dequeue();
				DamageReport damageReport = deathEvent.damageReport;
				StatSheet statSheet = PlayerStatsComponent.FindMasterStatSheet(damageReport.victimMaster);
				StatSheet statSheet2 = PlayerStatsComponent.FindMasterStatSheet(damageReport.attackerMaster);
				StatSheet statSheet3 = PlayerStatsComponent.FindMasterStatSheet(damageReport.attackerOwnerMaster);
				if (statSheet != null)
				{
					statSheet.PushStatValue(StatDef.totalDeaths, 1UL);
					statSheet.PushStatValue(PerBodyStatDef.deathsAs, damageReport.victimBodyIndex, 1UL);
					if (damageReport.attackerBodyIndex != -1)
					{
						statSheet.PushStatValue(PerBodyStatDef.deathsFrom, damageReport.attackerBodyIndex, 1UL);
					}
					if (damageReport.dotType != DotController.DotIndex.None)
					{
						DotController.DotIndex dotType = damageReport.dotType;
						if (dotType - DotController.DotIndex.Burn <= 2)
						{
							statSheet.PushStatValue(StatDef.totalBurnDeaths, 1UL);
						}
					}
					if (deathEvent.victimWasBurning)
					{
						statSheet.PushStatValue(StatDef.totalDeathsWhileBurning, 1UL);
					}
				}
				if (statSheet2 != null)
				{
					statSheet2.PushStatValue(StatDef.totalKills, 1UL);
					statSheet2.PushStatValue(PerBodyStatDef.killsAs, damageReport.attackerBodyIndex, 1UL);
					if (damageReport.victimBodyIndex != -1)
					{
						statSheet2.PushStatValue(PerBodyStatDef.killsAgainst, damageReport.victimBodyIndex, 1UL);
						if (damageReport.victimIsElite)
						{
							statSheet2.PushStatValue(StatDef.totalEliteKills, 1UL);
							statSheet2.PushStatValue(PerBodyStatDef.killsAgainstElite, damageReport.victimBodyIndex, 1UL);
						}
					}
				}
				if (statSheet3 != null)
				{
					statSheet3.PushStatValue(StatDef.totalMinionKills, 1UL);
					if (damageReport.attackerOwnerBodyIndex != -1)
					{
						statSheet3.PushStatValue(PerBodyStatDef.minionKillsAs, damageReport.attackerOwnerBodyIndex, 1UL);
					}
				}
				if (damageReport.victimIsBoss)
				{
					int i = 0;
					int count = PlayerStatsComponent.instancesList.Count;
					while (i < count)
					{
						PlayerStatsComponent playerStatsComponent = PlayerStatsComponent.instancesList[i];
						if (playerStatsComponent.characterMaster.alive)
						{
							playerStatsComponent.currentStats.PushStatValue(StatDef.totalTeleporterBossKillsWitnessed, 1UL);
						}
						i++;
					}
				}
			}
		}

		// Token: 0x06001CDC RID: 7388 RVA: 0x0007B948 File Offset: 0x00079B48
		public static void OnGoldCollected(CharacterMaster characterMaster, ulong amount)
		{
			StatManager.goldCollectedEvents.Enqueue(new StatManager.GoldEvent
			{
				characterMaster = characterMaster,
				amount = amount
			});
		}

		// Token: 0x06001CDD RID: 7389 RVA: 0x0007B978 File Offset: 0x00079B78
		private static void ProcessGoldEvents()
		{
			while (StatManager.goldCollectedEvents.Count > 0)
			{
				StatManager.GoldEvent goldEvent = StatManager.goldCollectedEvents.Dequeue();
				CharacterMaster characterMaster = goldEvent.characterMaster;
				StatSheet statSheet;
				if (characterMaster == null)
				{
					statSheet = null;
				}
				else
				{
					PlayerStatsComponent component = characterMaster.GetComponent<PlayerStatsComponent>();
					statSheet = ((component != null) ? component.currentStats : null);
				}
				StatSheet statSheet2 = statSheet;
				if (statSheet2 != null)
				{
					statSheet2.PushStatValue(StatDef.goldCollected, goldEvent.amount);
					statSheet2.PushStatValue(StatDef.maxGoldCollected, statSheet2.GetStatValueULong(StatDef.goldCollected));
				}
			}
		}

		// Token: 0x06001CDE RID: 7390 RVA: 0x0007B9E8 File Offset: 0x00079BE8
		public static void OnPurchase<T>(CharacterBody characterBody, CostTypeIndex costType, T statDefsToIncrement) where T : IEnumerable<StatDef>
		{
			StatSheet statSheet = PlayerStatsComponent.FindBodyStatSheet(characterBody);
			if (statSheet == null)
			{
				return;
			}
			StatDef statDef = null;
			StatDef statDef2 = null;
			switch (costType)
			{
			case CostTypeIndex.Money:
				statDef = StatDef.totalGoldPurchases;
				statDef2 = StatDef.highestGoldPurchases;
				break;
			case CostTypeIndex.PercentHealth:
				statDef = StatDef.totalBloodPurchases;
				statDef2 = StatDef.highestBloodPurchases;
				break;
			case CostTypeIndex.LunarCoin:
				statDef = StatDef.totalLunarPurchases;
				statDef2 = StatDef.highestLunarPurchases;
				break;
			case CostTypeIndex.WhiteItem:
				statDef = StatDef.totalTier1Purchases;
				statDef2 = StatDef.highestTier1Purchases;
				break;
			case CostTypeIndex.GreenItem:
				statDef = StatDef.totalTier2Purchases;
				statDef2 = StatDef.highestTier2Purchases;
				break;
			case CostTypeIndex.RedItem:
				statDef = StatDef.totalTier3Purchases;
				statDef2 = StatDef.highestTier3Purchases;
				break;
			}
			statSheet.PushStatValue(StatDef.totalPurchases, 1UL);
			statSheet.PushStatValue(StatDef.highestPurchases, statSheet.GetStatValueULong(StatDef.totalPurchases));
			if (statDef != null)
			{
				statSheet.PushStatValue(statDef, 1UL);
				if (statDef2 != null)
				{
					statSheet.PushStatValue(statDef2, statSheet.GetStatValueULong(statDef));
				}
			}
			if (statDefsToIncrement != null)
			{
				foreach (StatDef statDef3 in statDefsToIncrement)
				{
					if (statDef3 != null)
					{
						statSheet.PushStatValue(statDef3, 1UL);
					}
				}
			}
		}

		// Token: 0x06001CDF RID: 7391 RVA: 0x0007BB0C File Offset: 0x00079D0C
		public static void OnEquipmentActivated(EquipmentSlot activator, EquipmentIndex equipmentIndex)
		{
			StatSheet statSheet = PlayerStatsComponent.FindBodyStatSheet(activator.characterBody);
			if (statSheet == null)
			{
				return;
			}
			statSheet.PushStatValue(PerEquipmentStatDef.totalTimesFired.FindStatDef(equipmentIndex), 1UL);
		}

		// Token: 0x06001CE0 RID: 7392 RVA: 0x0007BB30 File Offset: 0x00079D30
		public static void PushCharacterUpdateEvent(StatManager.CharacterUpdateEvent e)
		{
			StatManager.characterUpdateEvents.Enqueue(e);
		}

		// Token: 0x06001CE1 RID: 7393 RVA: 0x0007BB40 File Offset: 0x00079D40
		private static void ProcessCharacterUpdateEvents()
		{
			while (StatManager.characterUpdateEvents.Count > 0)
			{
				StatManager.CharacterUpdateEvent characterUpdateEvent = StatManager.characterUpdateEvents.Dequeue();
				if (characterUpdateEvent.statsComponent)
				{
					StatSheet currentStats = characterUpdateEvent.statsComponent.currentStats;
					if (currentStats != null)
					{
						CharacterBody body = characterUpdateEvent.statsComponent.characterMaster.GetBody();
						int num = (body != null) ? body.bodyIndex : -1;
						currentStats.PushStatValue(StatDef.totalTimeAlive, (double)characterUpdateEvent.additionalTimeAlive);
						currentStats.PushStatValue(StatDef.highestLevel, (ulong)((long)characterUpdateEvent.level));
						currentStats.PushStatValue(StatDef.totalDistanceTraveled, (double)characterUpdateEvent.additionalDistanceTraveled);
						if (num != -1)
						{
							currentStats.PushStatValue(PerBodyStatDef.totalTimeAlive, num, (double)characterUpdateEvent.additionalTimeAlive);
							currentStats.PushStatValue(PerBodyStatDef.longestRun, num, (double)characterUpdateEvent.runTime);
						}
						EquipmentIndex currentEquipmentIndex = characterUpdateEvent.statsComponent.characterMaster.inventory.currentEquipmentIndex;
						if (currentEquipmentIndex != EquipmentIndex.None)
						{
							currentStats.PushStatValue(PerEquipmentStatDef.totalTimeHeld.FindStatDef(currentEquipmentIndex), (double)characterUpdateEvent.additionalTimeAlive);
						}
					}
				}
			}
		}

		// Token: 0x06001CE2 RID: 7394 RVA: 0x0007BC40 File Offset: 0x00079E40
		private static void OnServerItemGiven(Inventory inventory, ItemIndex itemIndex, int quantity)
		{
			StatManager.itemCollectedEvents.Enqueue(new StatManager.ItemCollectedEvent
			{
				inventory = inventory,
				itemIndex = itemIndex,
				quantity = quantity,
				newCount = inventory.GetItemCount(itemIndex)
			});
		}

		// Token: 0x06001CE3 RID: 7395 RVA: 0x0007BC88 File Offset: 0x00079E88
		private static void ProcessItemCollectedEvents()
		{
			while (StatManager.itemCollectedEvents.Count > 0)
			{
				StatManager.ItemCollectedEvent itemCollectedEvent = StatManager.itemCollectedEvents.Dequeue();
				if (itemCollectedEvent.inventory)
				{
					PlayerStatsComponent component = itemCollectedEvent.inventory.GetComponent<PlayerStatsComponent>();
					StatSheet statSheet = (component != null) ? component.currentStats : null;
					if (statSheet != null)
					{
						statSheet.PushStatValue(StatDef.totalItemsCollected, (ulong)((long)itemCollectedEvent.quantity));
						statSheet.PushStatValue(StatDef.highestItemsCollected, statSheet.GetStatValueULong(StatDef.totalItemsCollected));
						statSheet.PushStatValue(PerItemStatDef.totalCollected.FindStatDef(itemCollectedEvent.itemIndex), (ulong)((long)itemCollectedEvent.quantity));
						statSheet.PushStatValue(PerItemStatDef.highestCollected.FindStatDef(itemCollectedEvent.itemIndex), (ulong)((long)itemCollectedEvent.newCount));
					}
				}
			}
		}

		// Token: 0x06001CE4 RID: 7396 RVA: 0x0007BD40 File Offset: 0x00079F40
		private static void OnServerStageBegin(Stage stage)
		{
			foreach (PlayerStatsComponent playerStatsComponent in PlayerStatsComponent.instancesList)
			{
				if (playerStatsComponent.playerCharacterMasterController.isConnected)
				{
					StatSheet currentStats = playerStatsComponent.currentStats;
					StatDef statDef = PerStageStatDef.totalTimesVisited.FindStatDef(stage.sceneDef ? stage.sceneDef.baseSceneName : string.Empty);
					if (statDef != null)
					{
						currentStats.PushStatValue(statDef, 1UL);
					}
				}
			}
		}

		// Token: 0x06001CE5 RID: 7397 RVA: 0x0007BDD8 File Offset: 0x00079FD8
		private static void OnServerStageComplete(Stage stage)
		{
			foreach (PlayerStatsComponent playerStatsComponent in PlayerStatsComponent.instancesList)
			{
				if (playerStatsComponent.playerCharacterMasterController.isConnected)
				{
					StatSheet currentStats = playerStatsComponent.currentStats;
					if (SceneInfo.instance.countsAsStage)
					{
						currentStats.PushStatValue(StatDef.totalStagesCompleted, 1UL);
						currentStats.PushStatValue(StatDef.highestStagesCompleted, currentStats.GetStatValueULong(StatDef.totalStagesCompleted));
					}
					StatDef statDef = PerStageStatDef.totalTimesCleared.FindStatDef(stage.sceneDef ? stage.sceneDef.baseSceneName : string.Empty);
					if (statDef != null)
					{
						currentStats.PushStatValue(statDef, 1UL);
					}
				}
			}
		}

		// Token: 0x040019F7 RID: 6647
		private static readonly Queue<StatManager.DamageEvent> damageEvents = new Queue<StatManager.DamageEvent>();

		// Token: 0x040019F8 RID: 6648
		private static readonly Queue<StatManager.DeathEvent> deathEvents = new Queue<StatManager.DeathEvent>();

		// Token: 0x040019F9 RID: 6649
		private static readonly Queue<StatManager.HealingEvent> healingEvents = new Queue<StatManager.HealingEvent>();

		// Token: 0x040019FA RID: 6650
		private static readonly Queue<StatManager.GoldEvent> goldCollectedEvents = new Queue<StatManager.GoldEvent>();

		// Token: 0x040019FB RID: 6651
		private static readonly Queue<StatManager.PurchaseStatEvent> purchaseStatEvents = new Queue<StatManager.PurchaseStatEvent>();

		// Token: 0x040019FC RID: 6652
		private static readonly Queue<StatManager.CharacterUpdateEvent> characterUpdateEvents = new Queue<StatManager.CharacterUpdateEvent>();

		// Token: 0x040019FD RID: 6653
		private static readonly Queue<StatManager.ItemCollectedEvent> itemCollectedEvents = new Queue<StatManager.ItemCollectedEvent>();

		// Token: 0x020004A5 RID: 1189
		private struct DamageEvent
		{
			// Token: 0x040019FE RID: 6654
			[CanBeNull]
			public CharacterMaster attackerMaster;

			// Token: 0x040019FF RID: 6655
			public int attackerBodyIndex;

			// Token: 0x04001A00 RID: 6656
			[CanBeNull]
			public CharacterMaster attackerOwnerMaster;

			// Token: 0x04001A01 RID: 6657
			public int attackerOwnerBodyIndex;

			// Token: 0x04001A02 RID: 6658
			[CanBeNull]
			public CharacterMaster victimMaster;

			// Token: 0x04001A03 RID: 6659
			public int victimBodyIndex;

			// Token: 0x04001A04 RID: 6660
			public bool victimIsElite;

			// Token: 0x04001A05 RID: 6661
			public float damageDealt;

			// Token: 0x04001A06 RID: 6662
			public DotController.DotIndex dotType;
		}

		// Token: 0x020004A6 RID: 1190
		private struct DeathEvent
		{
			// Token: 0x04001A07 RID: 6663
			public DamageReport damageReport;

			// Token: 0x04001A08 RID: 6664
			public bool victimWasBurning;
		}

		// Token: 0x020004A7 RID: 1191
		private struct HealingEvent
		{
			// Token: 0x04001A09 RID: 6665
			[CanBeNull]
			public GameObject healee;

			// Token: 0x04001A0A RID: 6666
			public float healAmount;
		}

		// Token: 0x020004A8 RID: 1192
		private struct GoldEvent
		{
			// Token: 0x04001A0B RID: 6667
			[CanBeNull]
			public CharacterMaster characterMaster;

			// Token: 0x04001A0C RID: 6668
			public ulong amount;
		}

		// Token: 0x020004A9 RID: 1193
		private struct PurchaseStatEvent
		{
		}

		// Token: 0x020004AA RID: 1194
		public struct CharacterUpdateEvent
		{
			// Token: 0x04001A0D RID: 6669
			public PlayerStatsComponent statsComponent;

			// Token: 0x04001A0E RID: 6670
			public float additionalDistanceTraveled;

			// Token: 0x04001A0F RID: 6671
			public float additionalTimeAlive;

			// Token: 0x04001A10 RID: 6672
			public int level;

			// Token: 0x04001A11 RID: 6673
			public float runTime;
		}

		// Token: 0x020004AB RID: 1195
		private struct ItemCollectedEvent
		{
			// Token: 0x04001A12 RID: 6674
			[CanBeNull]
			public Inventory inventory;

			// Token: 0x04001A13 RID: 6675
			public ItemIndex itemIndex;

			// Token: 0x04001A14 RID: 6676
			public int quantity;

			// Token: 0x04001A15 RID: 6677
			public int newCount;
		}
	}
}
