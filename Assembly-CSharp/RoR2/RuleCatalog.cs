using System;
using System.Collections.Generic;
using System.Linq;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003FD RID: 1021
	public static class RuleCatalog
	{
		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x060018B5 RID: 6325 RVA: 0x0006A491 File Offset: 0x00068691
		public static int ruleCount
		{
			get
			{
				return RuleCatalog.allRuleDefs.Count;
			}
		}

		// Token: 0x170002E3 RID: 739
		// (get) Token: 0x060018B6 RID: 6326 RVA: 0x0006A49D File Offset: 0x0006869D
		public static int choiceCount
		{
			get
			{
				return RuleCatalog.allChoicesDefs.Count;
			}
		}

		// Token: 0x170002E4 RID: 740
		// (get) Token: 0x060018B7 RID: 6327 RVA: 0x0006A4A9 File Offset: 0x000686A9
		public static int categoryCount
		{
			get
			{
				return RuleCatalog.allCategoryDefs.Count;
			}
		}

		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x060018B8 RID: 6328 RVA: 0x0006A4B5 File Offset: 0x000686B5
		// (set) Token: 0x060018B9 RID: 6329 RVA: 0x0006A4BC File Offset: 0x000686BC
		public static int highestLocalChoiceCount { get; private set; }

		// Token: 0x060018BA RID: 6330 RVA: 0x0006A4C4 File Offset: 0x000686C4
		public static RuleDef GetRuleDef(int ruleDefIndex)
		{
			return RuleCatalog.allRuleDefs[ruleDefIndex];
		}

		// Token: 0x060018BB RID: 6331 RVA: 0x0006A4D4 File Offset: 0x000686D4
		public static RuleDef FindRuleDef(string ruleDefGlobalName)
		{
			RuleDef result;
			RuleCatalog.ruleDefsByGlobalName.TryGetValue(ruleDefGlobalName, out result);
			return result;
		}

		// Token: 0x060018BC RID: 6332 RVA: 0x0006A4F0 File Offset: 0x000686F0
		public static RuleChoiceDef FindChoiceDef(string ruleChoiceDefGlobalName)
		{
			RuleChoiceDef result;
			RuleCatalog.ruleChoiceDefsByGlobalName.TryGetValue(ruleChoiceDefGlobalName, out result);
			return result;
		}

		// Token: 0x060018BD RID: 6333 RVA: 0x0006A50C File Offset: 0x0006870C
		public static RuleChoiceDef GetChoiceDef(int ruleChoiceDefIndex)
		{
			return RuleCatalog.allChoicesDefs[ruleChoiceDefIndex];
		}

		// Token: 0x060018BE RID: 6334 RVA: 0x0006A519 File Offset: 0x00068719
		public static RuleCategoryDef GetCategoryDef(int ruleCategoryDefIndex)
		{
			return RuleCatalog.allCategoryDefs[ruleCategoryDefIndex];
		}

		// Token: 0x060018BF RID: 6335 RVA: 0x0000B933 File Offset: 0x00009B33
		private static bool HiddenTestTrue()
		{
			return true;
		}

		// Token: 0x060018C0 RID: 6336 RVA: 0x0000AC89 File Offset: 0x00008E89
		private static bool HiddenTestFalse()
		{
			return false;
		}

		// Token: 0x060018C1 RID: 6337 RVA: 0x0006A526 File Offset: 0x00068726
		private static bool HiddenTestItemsConvar()
		{
			return !RuleCatalog.ruleShowItems.value;
		}

		// Token: 0x060018C2 RID: 6338 RVA: 0x0006A535 File Offset: 0x00068735
		private static void AddCategory(string displayToken, Color color)
		{
			RuleCatalog.AddCategory(displayToken, color, null, new Func<bool>(RuleCatalog.HiddenTestFalse));
		}

		// Token: 0x060018C3 RID: 6339 RVA: 0x0006A54B File Offset: 0x0006874B
		private static void AddCategory(string displayToken, Color color, string emptyTipToken, Func<bool> hiddenTest)
		{
			RuleCatalog.allCategoryDefs.Add(new RuleCategoryDef
			{
				position = RuleCatalog.allRuleDefs.Count,
				displayToken = displayToken,
				color = color,
				emptyTipToken = emptyTipToken,
				hiddenTest = hiddenTest
			});
		}

		// Token: 0x060018C4 RID: 6340 RVA: 0x0006A588 File Offset: 0x00068788
		private static void AddRule(RuleDef ruleDef)
		{
			if (RuleCatalog.allCategoryDefs.Count > 0)
			{
				ruleDef.category = RuleCatalog.allCategoryDefs[RuleCatalog.allCategoryDefs.Count - 1];
				RuleCatalog.allCategoryDefs[RuleCatalog.allCategoryDefs.Count - 1].children.Add(ruleDef);
			}
			RuleCatalog.allRuleDefs.Add(ruleDef);
			if (RuleCatalog.highestLocalChoiceCount < ruleDef.choices.Count)
			{
				RuleCatalog.highestLocalChoiceCount = ruleDef.choices.Count;
			}
			RuleCatalog.ruleDefsByGlobalName[ruleDef.globalName] = ruleDef;
			foreach (RuleChoiceDef ruleChoiceDef in ruleDef.choices)
			{
				RuleCatalog.ruleChoiceDefsByGlobalName[ruleChoiceDef.globalName] = ruleChoiceDef;
			}
		}

		// Token: 0x060018C5 RID: 6341 RVA: 0x0006A670 File Offset: 0x00068870
		[SystemInitializer(new Type[]
		{
			typeof(ItemCatalog),
			typeof(EquipmentCatalog)
		})]
		private static void Init()
		{
			RuleCatalog.AddCategory("RULE_HEADER_DIFFICULTY", new Color32(28, 99, 150, byte.MaxValue));
			RuleCatalog.AddRule(RuleDef.FromDifficulty());
			RuleCatalog.AddCategory("RULE_HEADER_ARTIFACTS", new Color32(74, 50, 149, byte.MaxValue), "RULE_ARTIFACTS_EMPTY_TIP", new Func<bool>(RuleCatalog.HiddenTestFalse));
			for (ArtifactIndex artifactIndex = ArtifactIndex.Command; artifactIndex < ArtifactIndex.Count; artifactIndex++)
			{
				RuleCatalog.AddRule(RuleDef.FromArtifact(artifactIndex));
			}
			RuleCatalog.AddCategory("RULE_HEADER_ITEMS", new Color32(147, 225, 128, byte.MaxValue), null, new Func<bool>(RuleCatalog.HiddenTestItemsConvar));
			List<ItemIndex> list = new List<ItemIndex>();
			ItemIndex itemIndex = ItemIndex.Syringe;
			ItemIndex itemCount = (ItemIndex)ItemCatalog.itemCount;
			while (itemIndex < itemCount)
			{
				list.Add(itemIndex);
				itemIndex++;
			}
			foreach (ItemIndex itemIndex2 in from i in list
			where ItemCatalog.GetItemDef(i).inDroppableTier
			orderby ItemCatalog.GetItemDef(i).tier
			select i)
			{
				RuleCatalog.AddRule(RuleDef.FromItem(itemIndex2));
			}
			RuleCatalog.AddCategory("RULE_HEADER_EQUIPMENT", new Color32(byte.MaxValue, 128, 0, byte.MaxValue), null, new Func<bool>(RuleCatalog.HiddenTestItemsConvar));
			List<EquipmentIndex> list2 = new List<EquipmentIndex>();
			EquipmentIndex equipmentIndex = EquipmentIndex.CommandMissile;
			EquipmentIndex equipmentCount = (EquipmentIndex)EquipmentCatalog.equipmentCount;
			while (equipmentIndex < equipmentCount)
			{
				list2.Add(equipmentIndex);
				equipmentIndex++;
			}
			foreach (EquipmentIndex equipmentIndex2 in from i in list2
			where EquipmentCatalog.GetEquipmentDef(i).canDrop
			select i)
			{
				RuleCatalog.AddRule(RuleDef.FromEquipment(equipmentIndex2));
			}
			RuleCatalog.AddCategory("RULE_HEADER_MISC", new Color32(192, 192, 192, byte.MaxValue), null, new Func<bool>(RuleCatalog.HiddenTestFalse));
			RuleDef ruleDef = new RuleDef("Misc.StartingMoney", "RULE_MISC_STARTING_MONEY");
			RuleChoiceDef ruleChoiceDef = ruleDef.AddChoice("0", 0U, true);
			ruleChoiceDef.tooltipNameToken = "RULE_STARTINGMONEY_CHOICE_0_NAME";
			ruleChoiceDef.tooltipBodyToken = "RULE_STARTINGMONEY_CHOICE_0_DESC";
			ruleChoiceDef.tooltipNameColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarCoin);
			RuleChoiceDef ruleChoiceDef2 = ruleDef.AddChoice("15", 15U, true);
			ruleChoiceDef2.tooltipNameToken = "RULE_STARTINGMONEY_CHOICE_15_NAME";
			ruleChoiceDef2.tooltipBodyToken = "RULE_STARTINGMONEY_CHOICE_15_DESC";
			ruleChoiceDef2.tooltipNameColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarCoin);
			ruleDef.MakeNewestChoiceDefault();
			RuleChoiceDef ruleChoiceDef3 = ruleDef.AddChoice("50", 50U, true);
			ruleChoiceDef3.tooltipNameToken = "RULE_STARTINGMONEY_CHOICE_50_NAME";
			ruleChoiceDef3.tooltipBodyToken = "RULE_STARTINGMONEY_CHOICE_50_DESC";
			ruleChoiceDef3.spritePath = "Textures/MiscIcons/texRuleBonusStartingMoney";
			ruleChoiceDef3.tooltipNameColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarCoin);
			RuleCatalog.AddRule(ruleDef);
			RuleDef ruleDef2 = new RuleDef("Misc.StageOrder", "RULE_MISC_STAGE_ORDER");
			RuleChoiceDef ruleChoiceDef4 = ruleDef2.AddChoice("Normal", StageOrder.Normal, true);
			ruleChoiceDef4.tooltipNameToken = "RULE_STAGEORDER_CHOICE_NORMAL_NAME";
			ruleChoiceDef4.tooltipBodyToken = "RULE_STAGEORDER_CHOICE_NORMAL_DESC";
			ruleChoiceDef4.tooltipNameColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarCoin);
			ruleDef2.MakeNewestChoiceDefault();
			RuleChoiceDef ruleChoiceDef5 = ruleDef2.AddChoice("Random", StageOrder.Random, true);
			ruleChoiceDef5.tooltipNameToken = "RULE_STAGEORDER_CHOICE_RANDOM_NAME";
			ruleChoiceDef5.tooltipBodyToken = "RULE_STAGEORDER_CHOICE_RANDOM_DESC";
			ruleChoiceDef5.spritePath = "Textures/MiscIcons/texRuleMapIsRandom";
			ruleChoiceDef5.tooltipNameColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarCoin);
			RuleCatalog.AddRule(ruleDef2);
			RuleDef ruleDef3 = new RuleDef("Misc.KeepMoneyBetweenStages", "RULE_MISC_KEEP_MONEY_BETWEEN_STAGES");
			ruleDef3.AddChoice("On", true, true).tooltipBodyToken = "RULE_KEEPMONEYBETWEENSTAGES_CHOICE_ON_DESC";
			ruleDef3.AddChoice("Off", false, true).tooltipBodyToken = "RULE_KEEPMONEYBETWEENSTAGES_CHOICE_OFF_DESC";
			ruleDef3.MakeNewestChoiceDefault();
			RuleCatalog.AddRule(ruleDef3);
			for (int k = 0; k < RuleCatalog.allRuleDefs.Count; k++)
			{
				RuleDef ruleDef4 = RuleCatalog.allRuleDefs[k];
				ruleDef4.globalIndex = k;
				for (int j = 0; j < ruleDef4.choices.Count; j++)
				{
					RuleChoiceDef ruleChoiceDef6 = ruleDef4.choices[j];
					ruleChoiceDef6.localIndex = j;
					ruleChoiceDef6.globalIndex = RuleCatalog.allChoicesDefs.Count;
					RuleCatalog.allChoicesDefs.Add(ruleChoiceDef6);
				}
			}
			RuleCatalog.availability.MakeAvailable();
		}

		// Token: 0x060018C6 RID: 6342 RVA: 0x0006AB08 File Offset: 0x00068D08
		[ConCommand(commandName = "rules_dump", flags = ConVarFlags.None, helpText = "Dump information about the rules system.")]
		private static void CCRulesDump(ConCommandArgs args)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			for (int i = 0; i < RuleCatalog.ruleCount; i++)
			{
				RuleDef ruleDef = RuleCatalog.GetRuleDef(i);
				for (int j = 0; j < ruleDef.choices.Count; j++)
				{
					RuleChoiceDef ruleChoiceDef = ruleDef.choices[j];
					string item = string.Format("  {{localChoiceIndex={0} globalChoiceIndex={1} localName={2}}}", ruleChoiceDef.localIndex, ruleChoiceDef.globalIndex, ruleChoiceDef.localName);
					list2.Add(item);
				}
				string str = string.Join("\n", list2);
				list2.Clear();
				string str2 = string.Format("[{0}] {1} defaultChoiceIndex={2}\n", i, ruleDef.globalName, ruleDef.defaultChoiceIndex);
				list.Add(str2 + str);
			}
			Debug.Log(string.Join("\n", list));
		}

		// Token: 0x0400171D RID: 5917
		private static readonly List<RuleDef> allRuleDefs = new List<RuleDef>();

		// Token: 0x0400171E RID: 5918
		private static readonly List<RuleChoiceDef> allChoicesDefs = new List<RuleChoiceDef>();

		// Token: 0x0400171F RID: 5919
		public static readonly List<RuleCategoryDef> allCategoryDefs = new List<RuleCategoryDef>();

		// Token: 0x04001720 RID: 5920
		private static readonly Dictionary<string, RuleDef> ruleDefsByGlobalName = new Dictionary<string, RuleDef>();

		// Token: 0x04001721 RID: 5921
		private static readonly Dictionary<string, RuleChoiceDef> ruleChoiceDefsByGlobalName = new Dictionary<string, RuleChoiceDef>();

		// Token: 0x04001722 RID: 5922
		public static ResourceAvailability availability;

		// Token: 0x04001724 RID: 5924
		private static readonly BoolConVar ruleShowItems = new BoolConVar("rule_show_items", ConVarFlags.Cheat, "0", "Whether or not to allow voting on items in the pregame rules.");
	}
}
