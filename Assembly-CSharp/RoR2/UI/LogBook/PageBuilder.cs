using System;
using System.Collections.Generic;
using System.Text;
using RoR2.Stats;
using TMPro;
using UnityEngine;

namespace RoR2.UI.LogBook
{
	// Token: 0x02000681 RID: 1665
	public class PageBuilder
	{
		// Token: 0x17000329 RID: 809
		// (get) Token: 0x0600251C RID: 9500 RVA: 0x000AE4D3 File Offset: 0x000AC6D3
		private StatSheet statSheet
		{
			get
			{
				return this.userProfile.statSheet;
			}
		}

		// Token: 0x0600251D RID: 9501 RVA: 0x000AE4E0 File Offset: 0x000AC6E0
		public void Destroy()
		{
			foreach (GameObject obj in this.managedObjects)
			{
				UnityEngine.Object.Destroy(obj);
			}
		}

		// Token: 0x0600251E RID: 9502 RVA: 0x000AE530 File Offset: 0x000AC730
		public void AddSimpleTextPanel(string text)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/Logbook/SimpleTextPanel"), this.container);
			gameObject.GetComponent<ChildLocator>().FindChild("MainLabel").GetComponent<TextMeshProUGUI>().text = text;
			this.managedObjects.Add(gameObject);
		}

		// Token: 0x0600251F RID: 9503 RVA: 0x000AE57A File Offset: 0x000AC77A
		public void AddSimpleTextPanel(params string[] textLines)
		{
			this.AddSimpleTextPanel(string.Join("\n", textLines));
		}

		// Token: 0x06002520 RID: 9504 RVA: 0x000AE590 File Offset: 0x000AC790
		public void AddSimplePickup(PickupIndex pickupIndex)
		{
			ItemIndex itemIndex = pickupIndex.itemIndex;
			EquipmentIndex equipmentIndex = pickupIndex.equipmentIndex;
			string token = null;
			if (itemIndex != ItemIndex.None)
			{
				ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
				this.AddDescriptionPanel(Language.GetString(itemDef.descriptionToken));
				token = itemDef.loreToken;
				ulong statValueULong = this.statSheet.GetStatValueULong(PerItemStatDef.totalCollected.FindStatDef(itemIndex));
				ulong statValueULong2 = this.statSheet.GetStatValueULong(PerItemStatDef.highestCollected.FindStatDef(itemIndex));
				string stringFormatted = Language.GetStringFormatted("GENERIC_PREFIX_FOUND", new object[]
				{
					statValueULong
				});
				string stringFormatted2 = Language.GetStringFormatted("ITEM_PREFIX_STACKCOUNT", new object[]
				{
					statValueULong2
				});
				this.AddSimpleTextPanel(new string[]
				{
					stringFormatted,
					stringFormatted2
				});
			}
			else if (equipmentIndex != EquipmentIndex.None)
			{
				EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(equipmentIndex);
				this.AddDescriptionPanel(Language.GetString(equipmentDef.descriptionToken));
				token = equipmentDef.loreToken;
				string stringFormatted3 = Language.GetStringFormatted("EQUIPMENT_PREFIX_TOTALTIMEHELD", new object[]
				{
					this.statSheet.GetStatDisplayValue(PerEquipmentStatDef.totalTimeHeld.FindStatDef(equipmentIndex))
				});
				string stringFormatted4 = Language.GetStringFormatted("EQUIPMENT_PREFIX_USECOUNT", new object[]
				{
					this.statSheet.GetStatDisplayValue(PerEquipmentStatDef.totalTimesFired.FindStatDef(equipmentIndex))
				});
				this.AddSimpleTextPanel(new string[]
				{
					stringFormatted3,
					stringFormatted4
				});
			}
			this.AddNotesPanel(Language.IsTokenInvalid(token) ? Language.GetString("EARLY_ACCESS_LORE") : Language.GetString(token));
		}

		// Token: 0x06002521 RID: 9505 RVA: 0x000AE70D File Offset: 0x000AC90D
		public void AddDescriptionPanel(string content)
		{
			this.AddSimpleTextPanel(Language.GetStringFormatted("DESCRIPTION_PREFIX_FORMAT", new object[]
			{
				content
			}));
		}

		// Token: 0x06002522 RID: 9506 RVA: 0x000AE729 File Offset: 0x000AC929
		public void AddNotesPanel(string content)
		{
			this.AddSimpleTextPanel(Language.GetStringFormatted("NOTES_PREFIX_FORMAT", new object[]
			{
				content
			}));
		}

		// Token: 0x06002523 RID: 9507 RVA: 0x000AE748 File Offset: 0x000AC948
		public void AddBodyStatsPanel(CharacterBody bodyPrefabComponent)
		{
			float baseMaxHealth = bodyPrefabComponent.baseMaxHealth;
			float levelMaxHealth = bodyPrefabComponent.levelMaxHealth;
			float baseDamage = bodyPrefabComponent.baseDamage;
			float levelDamage = bodyPrefabComponent.levelDamage;
			float baseMoveSpeed = bodyPrefabComponent.baseMoveSpeed;
			this.AddSimpleTextPanel(new string[]
			{
				Language.GetStringFormatted("BODY_HEALTH_FORMAT", new object[]
				{
					Language.GetStringFormatted("BODY_STATS_FORMAT", new object[]
					{
						baseMaxHealth.ToString(),
						levelMaxHealth.ToString()
					})
				}),
				Language.GetStringFormatted("BODY_DAMAGE_FORMAT", new object[]
				{
					Language.GetStringFormatted("BODY_STATS_FORMAT", new object[]
					{
						baseDamage.ToString(),
						levelDamage.ToString()
					})
				}),
				Language.GetStringFormatted("BODY_MOVESPEED_FORMAT", new object[]
				{
					baseMoveSpeed
				})
			});
		}

		// Token: 0x06002524 RID: 9508 RVA: 0x000AE818 File Offset: 0x000ACA18
		public void AddMonsterPanel(CharacterBody bodyPrefabComponent)
		{
			ulong statValueULong = this.statSheet.GetStatValueULong(PerBodyStatDef.killsAgainst, bodyPrefabComponent.gameObject.name);
			ulong statValueULong2 = this.statSheet.GetStatValueULong(PerBodyStatDef.killsAgainstElite, bodyPrefabComponent.gameObject.name);
			ulong statValueULong3 = this.statSheet.GetStatValueULong(PerBodyStatDef.deathsFrom, bodyPrefabComponent.gameObject.name);
			string stringFormatted = Language.GetStringFormatted("MONSTER_PREFIX_KILLED", new object[]
			{
				statValueULong
			});
			string stringFormatted2 = Language.GetStringFormatted("MONSTER_PREFIX_ELITESKILLED", new object[]
			{
				statValueULong2
			});
			string stringFormatted3 = Language.GetStringFormatted("MONSTER_PREFIX_DEATH", new object[]
			{
				statValueULong3
			});
			this.AddSimpleTextPanel(new string[]
			{
				stringFormatted,
				stringFormatted2,
				stringFormatted3
			});
		}

		// Token: 0x06002525 RID: 9509 RVA: 0x000AE8E4 File Offset: 0x000ACAE4
		public void AddSurvivorPanel(CharacterBody bodyPrefabComponent)
		{
			string statDisplayValue = this.statSheet.GetStatDisplayValue(PerBodyStatDef.longestRun.FindStatDef(bodyPrefabComponent.name));
			ulong statValueULong = this.statSheet.GetStatValueULong(PerBodyStatDef.timesPicked.FindStatDef(bodyPrefabComponent.name));
			ulong statValueULong2 = this.statSheet.GetStatValueULong(StatDef.totalGamesPlayed);
			double num = 0.0;
			if (statValueULong2 != 0UL)
			{
				num = statValueULong / statValueULong2 * 100.0;
			}
			PageBuilder.sharedStringBuilder.Clear();
			PageBuilder.sharedStringBuilder.AppendLine(Language.GetStringFormatted("SURVIVOR_PREFIX_LONGESTRUN", new object[]
			{
				statDisplayValue
			}));
			PageBuilder.sharedStringBuilder.AppendLine(Language.GetStringFormatted("SURVIVOR_PREFIX_TIMESPICKED", new object[]
			{
				statValueULong
			}));
			PageBuilder.sharedStringBuilder.AppendLine(Language.GetStringFormatted("SURVIVOR_PREFIX_PICKPERCENTAGE", new object[]
			{
				num
			}));
			this.AddSimpleTextPanel(PageBuilder.sharedStringBuilder.ToString());
		}

		// Token: 0x06002526 RID: 9510 RVA: 0x000AE9DB File Offset: 0x000ACBDB
		public void AddSimpleBody(CharacterBody bodyPrefabComponent)
		{
			this.AddBodyStatsPanel(bodyPrefabComponent);
		}

		// Token: 0x06002527 RID: 9511 RVA: 0x000AE9E4 File Offset: 0x000ACBE4
		public void AddStagePanel(SceneDef sceneDef)
		{
			string statDisplayValue = this.userProfile.statSheet.GetStatDisplayValue(PerStageStatDef.totalTimesVisited.FindStatDef(sceneDef.sceneName));
			string statDisplayValue2 = this.userProfile.statSheet.GetStatDisplayValue(PerStageStatDef.totalTimesCleared.FindStatDef(sceneDef.sceneName));
			string stringFormatted = Language.GetStringFormatted("STAGE_PREFIX_TOTALTIMESVISITED", new object[]
			{
				statDisplayValue
			});
			string stringFormatted2 = Language.GetStringFormatted("STAGE_PREFIX_TOTALTIMESCLEARED", new object[]
			{
				statDisplayValue2
			});
			PageBuilder.sharedStringBuilder.Clear();
			PageBuilder.sharedStringBuilder.Append(stringFormatted);
			PageBuilder.sharedStringBuilder.Append("\n");
			PageBuilder.sharedStringBuilder.Append(stringFormatted2);
			this.AddSimpleTextPanel(PageBuilder.sharedStringBuilder.ToString());
		}

		// Token: 0x06002528 RID: 9512 RVA: 0x000AEAA0 File Offset: 0x000ACCA0
		public static void Stage(PageBuilder builder)
		{
			SceneDef sceneDef = (SceneDef)builder.entry.extraData;
			builder.AddStagePanel(sceneDef);
			builder.AddNotesPanel(Language.IsTokenInvalid(sceneDef.loreToken) ? Language.GetString("EARLY_ACCESS_LORE") : Language.GetString(sceneDef.loreToken));
		}

		// Token: 0x06002529 RID: 9513 RVA: 0x000AEAEF File Offset: 0x000ACCEF
		public static void SimplePickup(PageBuilder builder)
		{
			builder.AddSimplePickup((PickupIndex)builder.entry.extraData);
		}

		// Token: 0x0600252A RID: 9514 RVA: 0x000AEB07 File Offset: 0x000ACD07
		public static void SimpleBody(PageBuilder builder)
		{
			builder.AddSimpleBody((CharacterBody)builder.entry.extraData);
		}

		// Token: 0x0600252B RID: 9515 RVA: 0x000AEB20 File Offset: 0x000ACD20
		public static void MonsterBody(PageBuilder builder)
		{
			CharacterBody bodyPrefabComponent = (CharacterBody)builder.entry.extraData;
			builder.AddSimpleBody(bodyPrefabComponent);
			builder.AddMonsterPanel(bodyPrefabComponent);
			builder.AddNotesPanel(Language.GetString("EARLY_ACCESS_LORE"));
		}

		// Token: 0x0600252C RID: 9516 RVA: 0x000AEB5C File Offset: 0x000ACD5C
		public static void SurvivorBody(PageBuilder builder)
		{
			CharacterBody bodyPrefabComponent = (CharacterBody)builder.entry.extraData;
			builder.AddSimpleBody(bodyPrefabComponent);
			builder.AddSurvivorPanel(bodyPrefabComponent);
			builder.AddNotesPanel(Language.GetString("EARLY_ACCESS_LORE"));
		}

		// Token: 0x0600252D RID: 9517 RVA: 0x000AEB98 File Offset: 0x000ACD98
		public void AddRunReportPanel(RunReport runReport)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/GameEndReportPanel"), this.container);
			gameObject.GetComponent<GameEndReportPanelController>().SetDisplayData(new GameEndReportPanelController.DisplayData
			{
				runReport = runReport,
				playerIndex = 0
			});
			gameObject.GetComponent<MPEventSystemProvider>().fallBackToMainEventSystem = true;
			this.managedObjects.Add(gameObject);
		}

		// Token: 0x0600252E RID: 9518 RVA: 0x000AEBF7 File Offset: 0x000ACDF7
		public static void RunReportPanel(PageBuilder builder)
		{
			builder.AddRunReportPanel(RunReport.Load("PreviousRun"));
		}

		// Token: 0x04002849 RID: 10313
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();

		// Token: 0x0400284A RID: 10314
		public UserProfile userProfile;

		// Token: 0x0400284B RID: 10315
		public RectTransform container;

		// Token: 0x0400284C RID: 10316
		public Entry entry;

		// Token: 0x0400284D RID: 10317
		public readonly List<GameObject> managedObjects = new List<GameObject>();
	}
}
