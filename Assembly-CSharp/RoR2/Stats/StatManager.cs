using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace RoR2.Stats
{
	// Token: 0x02000500 RID: 1280
	internal class StatManager
	{
		// Token: 0x06001CF2 RID: 7410 RVA: 0x00086BC8 File Offset: 0x00084DC8
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			GlobalEventManager.onServerDamageDealt += StatManager.OnDamageDealt;
			GlobalEventManager.onCharacterDeathGlobal += StatManager.OnCharacterDeath;
			HealthComponent.onCharacterHealServer += StatManager.OnCharacterHeal;
			Run.onPlayerFirstCreatedServer += StatManager.OnPlayerFirstCreatedServer;
			Run.OnServerGameOver += StatManager.OnServerGameOver;
			Stage.onServerStageComplete += StatManager.OnServerStageComplete;
			Stage.onServerStageBegin += StatManager.OnServerStageBegin;
			Inventory.onServerItemGiven += StatManager.OnServerItemGiven;
			RoR2Application.onFixedUpdate += StatManager.ProcessEvents;
			EquipmentSlot.onServerEquipmentActivated += StatManager.OnEquipmentActivated;
		}

		// Token: 0x06001CF3 RID: 7411 RVA: 0x00086C80 File Offset: 0x00084E80
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

		// Token: 0x06001CF4 RID: 7412 RVA: 0x00086D14 File Offset: 0x00084F14
		private static void OnPlayerFirstCreatedServer(Run run, PlayerCharacterMasterController playerCharacterMasterController)
		{
			playerCharacterMasterController.master.onBodyStart += StatManager.OnBodyFirstStart;
		}

		// Token: 0x06001CF5 RID: 7413 RVA: 0x00086D30 File Offset: 0x00084F30
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

		// Token: 0x06001CF6 RID: 7414 RVA: 0x00086DA6 File Offset: 0x00084FA6
		private static void ProcessEvents()
		{
			StatManager.ProcessDamageEvents();
			StatManager.ProcessDeathEvents();
			StatManager.ProcessHealingEvents();
			StatManager.ProcessGoldEvents();
			StatManager.ProcessItemCollectedEvents();
			StatManager.ProcessCharacterUpdateEvents();
		}

		// Token: 0x06001CF7 RID: 7415 RVA: 0x00086DC8 File Offset: 0x00084FC8
		public static void OnCharacterHeal(HealthComponent healthComponent, float amount)
		{
			StatManager.healingEvents.Enqueue(new StatManager.HealingEvent
			{
				healee = healthComponent.gameObject,
				healAmount = amount
			});
		}

		// Token: 0x06001CF8 RID: 7416 RVA: 0x00086E00 File Offset: 0x00085000
		public static void OnDamageDealt(DamageReport damageReport)
		{
			DamageInfo damageInfo = damageReport.damageInfo;
			StatManager.damageEvents.Enqueue(new StatManager.DamageEvent
			{
				attacker = damageInfo.attacker,
				attackerName = (damageInfo.attacker ? damageInfo.attacker.name : null),
				victim = damageReport.victim.gameObject,
				victimName = damageReport.victim.gameObject.name,
				victimIsElite = (damageReport.victimBody && damageReport.victimBody.isElite),
				damageDealt = damageReport.damageInfo.damage
			});
		}

		// Token: 0x06001CF9 RID: 7417 RVA: 0x00086EB4 File Offset: 0x000850B4
		public static void OnCharacterDeath(DamageReport damageReport)
		{
			DamageInfo damageInfo = damageReport.damageInfo;
			StatManager.deathEvents.Enqueue(new StatManager.DamageEvent
			{
				attacker = damageInfo.attacker,
				attackerName = (damageInfo.attacker ? damageInfo.attacker.name : null),
				victim = damageReport.victim.gameObject,
				victimName = damageReport.victim.gameObject.name,
				victimIsElite = (damageReport.victimBody && damageReport.victimBody.isElite),
				damageDealt = damageReport.damageInfo.damage
			});
		}

		// Token: 0x06001CFA RID: 7418 RVA: 0x00086F68 File Offset: 0x00085168
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

		// Token: 0x06001CFB RID: 7419 RVA: 0x00086FB0 File Offset: 0x000851B0
		private static void ProcessDamageEvents()
		{
			while (StatManager.damageEvents.Count > 0)
			{
				StatManager.DamageEvent damageEvent = StatManager.damageEvents.Dequeue();
				ulong statValue = (ulong)damageEvent.damageDealt;
				StatSheet statSheet = PlayerStatsComponent.FindBodyStatSheet(damageEvent.victim);
				StatSheet statSheet2 = PlayerStatsComponent.FindBodyStatSheet(damageEvent.attacker);
				if (statSheet != null)
				{
					statSheet.PushStatValue(StatDef.totalDamageTaken, statValue);
					if (damageEvent.attackerName != null)
					{
						statSheet.PushStatValue(PerBodyStatDef.damageTakenFrom, damageEvent.attackerName, statValue);
					}
					if (damageEvent.victimName != null)
					{
						statSheet.PushStatValue(PerBodyStatDef.damageTakenAs, damageEvent.victimName, statValue);
					}
				}
				if (statSheet2 != null)
				{
					statSheet2.PushStatValue(StatDef.totalDamageDealt, statValue);
					statSheet2.PushStatValue(StatDef.highestDamageDealt, statValue);
					if (damageEvent.attackerName != null)
					{
						statSheet2.PushStatValue(PerBodyStatDef.damageDealtAs, damageEvent.attackerName, statValue);
					}
					if (damageEvent.victimName != null)
					{
						statSheet2.PushStatValue(PerBodyStatDef.damageDealtTo, damageEvent.victimName, statValue);
					}
				}
			}
		}

		// Token: 0x06001CFC RID: 7420 RVA: 0x00087090 File Offset: 0x00085290
		private static void ProcessDeathEvents()
		{
			while (StatManager.deathEvents.Count > 0)
			{
				StatManager.DamageEvent damageEvent = StatManager.deathEvents.Dequeue();
				StatSheet statSheet = PlayerStatsComponent.FindBodyStatSheet(damageEvent.victim);
				StatSheet statSheet2 = PlayerStatsComponent.FindBodyStatSheet(damageEvent.attacker);
				if (statSheet != null)
				{
					statSheet.PushStatValue(StatDef.totalDeaths, 1UL);
					statSheet.PushStatValue(PerBodyStatDef.deathsAs, damageEvent.victimName, 1UL);
					if (damageEvent.attackerName != null)
					{
						statSheet.PushStatValue(PerBodyStatDef.deathsFrom, damageEvent.attackerName, 1UL);
					}
				}
				if (statSheet2 != null)
				{
					statSheet2.PushStatValue(StatDef.totalKills, 1UL);
					statSheet2.PushStatValue(PerBodyStatDef.killsAs, damageEvent.attackerName, 1UL);
					if (damageEvent.victimName != null)
					{
						statSheet2.PushStatValue(PerBodyStatDef.killsAgainst, damageEvent.victimName, 1UL);
						if (damageEvent.victimIsElite)
						{
							statSheet2.PushStatValue(PerBodyStatDef.killsAgainstElite, damageEvent.victimName, 1UL);
						}
					}
				}
			}
		}

		// Token: 0x06001CFD RID: 7421 RVA: 0x0008716C File Offset: 0x0008536C
		public static void OnGoldCollected(CharacterMaster characterMaster, ulong amount)
		{
			StatManager.goldCollectedEvents.Enqueue(new StatManager.GoldEvent
			{
				characterMaster = characterMaster,
				amount = amount
			});
		}

		// Token: 0x06001CFE RID: 7422 RVA: 0x0008719C File Offset: 0x0008539C
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

		// Token: 0x06001CFF RID: 7423 RVA: 0x0008720C File Offset: 0x0008540C
		public static void OnPurchase<T>(CharacterBody characterBody, CostType costType, T statDefsToIncrement) where T : IEnumerable<StatDef>
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
			case CostType.Money:
				statDef = StatDef.totalGoldPurchases;
				statDef2 = StatDef.highestGoldPurchases;
				break;
			case CostType.PercentHealth:
				statDef = StatDef.totalBloodPurchases;
				statDef2 = StatDef.highestBloodPurchases;
				break;
			case CostType.Lunar:
				statDef = StatDef.totalLunarPurchases;
				statDef2 = StatDef.highestLunarPurchases;
				break;
			case CostType.WhiteItem:
				statDef = StatDef.totalTier1Purchases;
				statDef2 = StatDef.highestTier1Purchases;
				break;
			case CostType.GreenItem:
				statDef = StatDef.totalTier2Purchases;
				statDef2 = StatDef.highestTier2Purchases;
				break;
			case CostType.RedItem:
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

		// Token: 0x06001D00 RID: 7424 RVA: 0x00087330 File Offset: 0x00085530
		public static void OnEquipmentActivated(EquipmentSlot activator, EquipmentIndex equipmentIndex)
		{
			StatSheet statSheet = PlayerStatsComponent.FindBodyStatSheet(activator.characterBody);
			if (statSheet == null)
			{
				return;
			}
			statSheet.PushStatValue(PerEquipmentStatDef.totalTimesFired.FindStatDef(equipmentIndex), 1UL);
		}

		// Token: 0x06001D01 RID: 7425 RVA: 0x00087354 File Offset: 0x00085554
		public static void PushCharacterUpdateEvent(StatManager.CharacterUpdateEvent e)
		{
			StatManager.characterUpdateEvents.Enqueue(e);
		}

		// Token: 0x06001D02 RID: 7426 RVA: 0x00087364 File Offset: 0x00085564
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
						GameObject bodyObject = characterUpdateEvent.statsComponent.characterMaster.GetBodyObject();
						string text = null;
						if (bodyObject)
						{
							text = bodyObject.name;
						}
						currentStats.PushStatValue(StatDef.totalTimeAlive, (double)characterUpdateEvent.additionalTimeAlive);
						currentStats.PushStatValue(StatDef.highestLevel, (ulong)((long)characterUpdateEvent.level));
						currentStats.PushStatValue(StatDef.totalDistanceTraveled, (double)characterUpdateEvent.additionalDistanceTraveled);
						if (text != null)
						{
							currentStats.PushStatValue(PerBodyStatDef.totalTimeAlive, text, (double)characterUpdateEvent.additionalTimeAlive);
							currentStats.PushStatValue(PerBodyStatDef.longestRun, text, (double)characterUpdateEvent.runTime);
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

		// Token: 0x06001D03 RID: 7427 RVA: 0x0008746C File Offset: 0x0008566C
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

		// Token: 0x06001D04 RID: 7428 RVA: 0x000874B4 File Offset: 0x000856B4
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

		// Token: 0x06001D05 RID: 7429 RVA: 0x0008756C File Offset: 0x0008576C
		private static void OnServerStageBegin(Stage stage)
		{
			foreach (PlayerStatsComponent playerStatsComponent in PlayerStatsComponent.instancesList)
			{
				if (playerStatsComponent.playerCharacterMasterController.isConnected)
				{
					StatSheet currentStats = playerStatsComponent.currentStats;
					StatDef statDef = PerStageStatDef.totalTimesVisited.FindStatDef(stage.sceneDef ? stage.sceneDef.sceneName : string.Empty);
					if (statDef != null)
					{
						currentStats.PushStatValue(statDef, 1UL);
					}
				}
			}
		}

		// Token: 0x06001D06 RID: 7430 RVA: 0x00087604 File Offset: 0x00085804
		private static void OnServerStageComplete(Stage stage)
		{
			foreach (PlayerStatsComponent playerStatsComponent in PlayerStatsComponent.instancesList)
			{
				if (playerStatsComponent.playerCharacterMasterController.isConnected)
				{
					StatSheet currentStats = playerStatsComponent.currentStats;
					currentStats.PushStatValue(StatDef.totalStagesCompleted, 1UL);
					currentStats.PushStatValue(StatDef.highestStagesCompleted, currentStats.GetStatValueULong(StatDef.totalStagesCompleted));
					StatDef statDef = PerStageStatDef.totalTimesCleared.FindStatDef(stage.sceneDef ? stage.sceneDef.sceneName : string.Empty);
					if (statDef != null)
					{
						currentStats.PushStatValue(statDef, 1UL);
					}
				}
			}
		}

		// Token: 0x04001F2A RID: 7978
		private static readonly Queue<StatManager.DamageEvent> damageEvents = new Queue<StatManager.DamageEvent>();

		// Token: 0x04001F2B RID: 7979
		private static readonly Queue<StatManager.DamageEvent> deathEvents = new Queue<StatManager.DamageEvent>();

		// Token: 0x04001F2C RID: 7980
		private static readonly Queue<StatManager.HealingEvent> healingEvents = new Queue<StatManager.HealingEvent>();

		// Token: 0x04001F2D RID: 7981
		private static readonly Queue<StatManager.GoldEvent> goldCollectedEvents = new Queue<StatManager.GoldEvent>();

		// Token: 0x04001F2E RID: 7982
		private static readonly Queue<StatManager.PurchaseStatEvent> purchaseStatEvents = new Queue<StatManager.PurchaseStatEvent>();

		// Token: 0x04001F2F RID: 7983
		private static readonly Queue<StatManager.CharacterUpdateEvent> characterUpdateEvents = new Queue<StatManager.CharacterUpdateEvent>();

		// Token: 0x04001F30 RID: 7984
		private static readonly Queue<StatManager.ItemCollectedEvent> itemCollectedEvents = new Queue<StatManager.ItemCollectedEvent>();

		// Token: 0x02000501 RID: 1281
		private struct DamageEvent
		{
			// Token: 0x04001F31 RID: 7985
			[CanBeNull]
			public GameObject attacker;

			// Token: 0x04001F32 RID: 7986
			[CanBeNull]
			public string attackerName;

			// Token: 0x04001F33 RID: 7987
			[CanBeNull]
			public GameObject victim;

			// Token: 0x04001F34 RID: 7988
			[NotNull]
			public string victimName;

			// Token: 0x04001F35 RID: 7989
			public bool victimIsElite;

			// Token: 0x04001F36 RID: 7990
			public float damageDealt;
		}

		// Token: 0x02000502 RID: 1282
		private struct HealingEvent
		{
			// Token: 0x04001F37 RID: 7991
			[CanBeNull]
			public GameObject healee;

			// Token: 0x04001F38 RID: 7992
			public float healAmount;
		}

		// Token: 0x02000503 RID: 1283
		private struct GoldEvent
		{
			// Token: 0x04001F39 RID: 7993
			[CanBeNull]
			public CharacterMaster characterMaster;

			// Token: 0x04001F3A RID: 7994
			public ulong amount;
		}

		// Token: 0x02000504 RID: 1284
		private struct PurchaseStatEvent
		{
		}

		// Token: 0x02000505 RID: 1285
		public struct CharacterUpdateEvent
		{
			// Token: 0x04001F3B RID: 7995
			public PlayerStatsComponent statsComponent;

			// Token: 0x04001F3C RID: 7996
			public float additionalDistanceTraveled;

			// Token: 0x04001F3D RID: 7997
			public float additionalTimeAlive;

			// Token: 0x04001F3E RID: 7998
			public int level;

			// Token: 0x04001F3F RID: 7999
			public float runTime;
		}

		// Token: 0x02000506 RID: 1286
		private struct ItemCollectedEvent
		{
			// Token: 0x04001F40 RID: 8000
			[CanBeNull]
			public Inventory inventory;

			// Token: 0x04001F41 RID: 8001
			public ItemIndex itemIndex;

			// Token: 0x04001F42 RID: 8002
			public int quantity;

			// Token: 0x04001F43 RID: 8003
			public int newCount;
		}
	}
}
