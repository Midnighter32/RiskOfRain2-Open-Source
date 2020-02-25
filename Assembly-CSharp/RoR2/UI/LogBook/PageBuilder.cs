using System;
using System.Collections.Generic;
using System.Text;
using RoR2.Stats;
using TMPro;
using UnityEngine;

namespace RoR2.UI.LogBook
{
	// Token: 0x02000676 RID: 1654
	public class PageBuilder
	{
		// Token: 0x170003E9 RID: 1001
		// (get) Token: 0x060026C1 RID: 9921 RVA: 0x000A8FDF File Offset: 0x000A71DF
		private StatSheet statSheet
		{
			get
			{
				return this.userProfile.statSheet;
			}
		}

		// Token: 0x060026C2 RID: 9922 RVA: 0x000A8FEC File Offset: 0x000A71EC
		public void Destroy()
		{
			foreach (GameObject obj in this.managedObjects)
			{
				UnityEngine.Object.Destroy(obj);
			}
		}

		// Token: 0x060026C3 RID: 9923 RVA: 0x000A903C File Offset: 0x000A723C
		public void AddSimpleTextPanel(string text)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/Logbook/SimpleTextPanel"), this.container);
			gameObject.GetComponent<ChildLocator>().FindChild("MainLabel").GetComponent<TextMeshProUGUI>().text = text;
			this.managedObjects.Add(gameObject);
		}

		// Token: 0x060026C4 RID: 9924 RVA: 0x000A9086 File Offset: 0x000A7286
		public void AddSimpleTextPanel(params string[] textLines)
		{
			this.AddSimpleTextPanel(string.Join("\n", textLines));
		}

		// Token: 0x060026C5 RID: 9925 RVA: 0x000A909C File Offset: 0x000A729C
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
				string stringFormatted3 = Language.GetStringFormatted("EQUIPMENT_PREFIX_COOLDOWN", new object[]
				{
					equipmentDef.cooldown
				});
				string stringFormatted4 = Language.GetStringFormatted("EQUIPMENT_PREFIX_TOTALTIMEHELD", new object[]
				{
					this.statSheet.GetStatDisplayValue(PerEquipmentStatDef.totalTimeHeld.FindStatDef(equipmentIndex))
				});
				string stringFormatted5 = Language.GetStringFormatted("EQUIPMENT_PREFIX_USECOUNT", new object[]
				{
					this.statSheet.GetStatDisplayValue(PerEquipmentStatDef.totalTimesFired.FindStatDef(equipmentIndex))
				});
				this.AddSimpleTextPanel(stringFormatted3);
				this.AddSimpleTextPanel(new string[]
				{
					stringFormatted4,
					stringFormatted5
				});
			}
			this.AddNotesPanel(Language.IsTokenInvalid(token) ? Language.GetString("EARLY_ACCESS_LORE") : Language.GetString(token));
		}

		// Token: 0x060026C6 RID: 9926 RVA: 0x000A9242 File Offset: 0x000A7442
		public void AddDescriptionPanel(string content)
		{
			this.AddSimpleTextPanel(Language.GetStringFormatted("DESCRIPTION_PREFIX_FORMAT", new object[]
			{
				content
			}));
		}

		// Token: 0x060026C7 RID: 9927 RVA: 0x000A925E File Offset: 0x000A745E
		public void AddNotesPanel(string content)
		{
			this.AddSimpleTextPanel(Language.GetStringFormatted("NOTES_PREFIX_FORMAT", new object[]
			{
				content
			}));
		}

		// Token: 0x060026C8 RID: 9928 RVA: 0x000A927C File Offset: 0x000A747C
		public void AddBodyStatsPanel(CharacterBody bodyPrefabComponent)
		{
			float baseMaxHealth = bodyPrefabComponent.baseMaxHealth;
			float levelMaxHealth = bodyPrefabComponent.levelMaxHealth;
			float baseDamage = bodyPrefabComponent.baseDamage;
			float levelDamage = bodyPrefabComponent.levelDamage;
			float baseArmor = bodyPrefabComponent.baseArmor;
			float baseRegen = bodyPrefabComponent.baseRegen;
			float levelRegen = bodyPrefabComponent.levelRegen;
			float baseMoveSpeed = bodyPrefabComponent.baseMoveSpeed;
			this.AddSimpleTextPanel(string.Concat(new string[]
			{
				Language.GetStringFormatted("BODY_HEALTH_FORMAT", new object[]
				{
					Language.GetStringFormatted("BODY_STATS_FORMAT", new object[]
					{
						baseMaxHealth.ToString(),
						levelMaxHealth.ToString()
					})
				}),
				"\n",
				Language.GetStringFormatted("BODY_DAMAGE_FORMAT", new object[]
				{
					Language.GetStringFormatted("BODY_STATS_FORMAT", new object[]
					{
						baseDamage.ToString(),
						levelDamage.ToString()
					})
				}),
				"\n",
				(baseRegen >= Mathf.Epsilon) ? (Language.GetStringFormatted("BODY_REGEN_FORMAT", new object[]
				{
					Language.GetStringFormatted("BODY_STATS_FORMAT", new object[]
					{
						baseRegen.ToString(),
						levelRegen.ToString()
					})
				}) + "\n") : "",
				Language.GetStringFormatted("BODY_MOVESPEED_FORMAT", new object[]
				{
					baseMoveSpeed
				}),
				"\n",
				Language.GetStringFormatted("BODY_ARMOR_FORMAT", new object[]
				{
					baseArmor.ToString()
				})
			}));
		}

		// Token: 0x060026C9 RID: 9929 RVA: 0x000A93F0 File Offset: 0x000A75F0
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

		// Token: 0x060026CA RID: 9930 RVA: 0x000A94BC File Offset: 0x000A76BC
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

		// Token: 0x060026CB RID: 9931 RVA: 0x000A95B3 File Offset: 0x000A77B3
		public void AddSimpleBody(CharacterBody bodyPrefabComponent)
		{
			this.AddBodyStatsPanel(bodyPrefabComponent);
		}

		// Token: 0x060026CC RID: 9932 RVA: 0x000A95BC File Offset: 0x000A77BC
		public void AddBodyLore(CharacterBody characterBody)
		{
			bool flag = false;
			string token = "";
			string baseNameToken = characterBody.baseNameToken;
			if (!string.IsNullOrEmpty(baseNameToken))
			{
				token = baseNameToken.Replace("_NAME", "_LORE");
				if (!Language.IsTokenInvalid(token))
				{
					flag = true;
				}
			}
			if (flag)
			{
				this.AddNotesPanel(Language.GetString(token));
				return;
			}
			this.AddNotesPanel(Language.GetString("EARLY_ACCESS_LORE"));
		}

		// Token: 0x060026CD RID: 9933 RVA: 0x000A961C File Offset: 0x000A781C
		public void AddStagePanel(SceneDef sceneDef)
		{
			string statDisplayValue = this.userProfile.statSheet.GetStatDisplayValue(PerStageStatDef.totalTimesVisited.FindStatDef(sceneDef.baseSceneName));
			string statDisplayValue2 = this.userProfile.statSheet.GetStatDisplayValue(PerStageStatDef.totalTimesCleared.FindStatDef(sceneDef.baseSceneName));
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

		// Token: 0x060026CE RID: 9934 RVA: 0x000A96D8 File Offset: 0x000A78D8
		public static void Stage(PageBuilder builder)
		{
			SceneDef sceneDef = (SceneDef)builder.entry.extraData;
			builder.AddStagePanel(sceneDef);
			builder.AddNotesPanel(Language.IsTokenInvalid(sceneDef.loreToken) ? Language.GetString("EARLY_ACCESS_LORE") : Language.GetString(sceneDef.loreToken));
		}

		// Token: 0x060026CF RID: 9935 RVA: 0x000A9727 File Offset: 0x000A7927
		public static void SimplePickup(PageBuilder builder)
		{
			builder.AddSimplePickup((PickupIndex)builder.entry.extraData);
		}

		// Token: 0x060026D0 RID: 9936 RVA: 0x000A973F File Offset: 0x000A793F
		public static void SimpleBody(PageBuilder builder)
		{
			builder.AddSimpleBody((CharacterBody)builder.entry.extraData);
		}

		// Token: 0x060026D1 RID: 9937 RVA: 0x000A9758 File Offset: 0x000A7958
		public static void MonsterBody(PageBuilder builder)
		{
			CharacterBody characterBody = (CharacterBody)builder.entry.extraData;
			builder.AddSimpleBody(characterBody);
			builder.AddMonsterPanel(characterBody);
			builder.AddBodyLore(characterBody);
		}

		// Token: 0x060026D2 RID: 9938 RVA: 0x000A978C File Offset: 0x000A798C
		public static void SurvivorBody(PageBuilder builder)
		{
			CharacterBody characterBody = (CharacterBody)builder.entry.extraData;
			builder.AddSimpleBody(characterBody);
			builder.AddSurvivorPanel(characterBody);
			builder.AddBodyLore(characterBody);
		}

		// Token: 0x060026D3 RID: 9939 RVA: 0x000A97C0 File Offset: 0x000A79C0
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

		// Token: 0x060026D4 RID: 9940 RVA: 0x000A981F File Offset: 0x000A7A1F
		public static void RunReportPanel(PageBuilder builder)
		{
			builder.AddRunReportPanel(RunReport.Load("PreviousRun"));
		}

		// Token: 0x040024B0 RID: 9392
		private static readonly StringBuilder sharedStringBuilder = new StringBuilder();

		// Token: 0x040024B1 RID: 9393
		public UserProfile userProfile;

		// Token: 0x040024B2 RID: 9394
		public RectTransform container;

		// Token: 0x040024B3 RID: 9395
		public Entry entry;

		// Token: 0x040024B4 RID: 9396
		public readonly List<GameObject> managedObjects = new List<GameObject>();
	}
}
