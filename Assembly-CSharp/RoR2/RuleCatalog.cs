using System;
using System.Collections.Generic;
using System.Linq;
using RoR2.ConVar;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200047B RID: 1147
	public static class RuleCatalog
	{
		// Token: 0x17000260 RID: 608
		// (get) Token: 0x06001995 RID: 6549 RVA: 0x0007A115 File Offset: 0x00078315
		public static int ruleCount
		{
			get
			{
				return RuleCatalog.allRuleDefs.Count;
			}
		}

		// Token: 0x17000261 RID: 609
		// (get) Token: 0x06001996 RID: 6550 RVA: 0x0007A121 File Offset: 0x00078321
		public static int choiceCount
		{
			get
			{
				return RuleCatalog.allChoicesDefs.Count;
			}
		}

		// Token: 0x17000262 RID: 610
		// (get) Token: 0x06001997 RID: 6551 RVA: 0x0007A12D File Offset: 0x0007832D
		public static int categoryCount
		{
			get
			{
				return RuleCatalog.allCategoryDefs.Count;
			}
		}

		// Token: 0x17000263 RID: 611
		// (get) Token: 0x06001998 RID: 6552 RVA: 0x0007A139 File Offset: 0x00078339
		// (set) Token: 0x06001999 RID: 6553 RVA: 0x0007A140 File Offset: 0x00078340
		public static int highestLocalChoiceCount { get; private set; }

		// Token: 0x0600199A RID: 6554 RVA: 0x0007A148 File Offset: 0x00078348
		public static RuleDef GetRuleDef(int ruleDefIndex)
		{
			return RuleCatalog.allRuleDefs[ruleDefIndex];
		}

		// Token: 0x0600199B RID: 6555 RVA: 0x0007A158 File Offset: 0x00078358
		public static RuleDef FindRuleDef(string ruleDefGlobalName)
		{
			RuleDef result;
			RuleCatalog.ruleDefsByGlobalName.TryGetValue(ruleDefGlobalName, out result);
			return result;
		}

		// Token: 0x0600199C RID: 6556 RVA: 0x0007A174 File Offset: 0x00078374
		public static RuleChoiceDef FindChoiceDef(string ruleChoiceDefGlobalName)
		{
			RuleChoiceDef result;
			RuleCatalog.ruleChoiceDefsByGlobalName.TryGetValue(ruleChoiceDefGlobalName, out result);
			return result;
		}

		// Token: 0x0600199D RID: 6557 RVA: 0x0007A190 File Offset: 0x00078390
		public static RuleChoiceDef GetChoiceDef(int ruleChoiceDefIndex)
		{
			return RuleCatalog.allChoicesDefs[ruleChoiceDefIndex];
		}

		// Token: 0x0600199E RID: 6558 RVA: 0x0007A19D File Offset: 0x0007839D
		public static RuleCategoryDef GetCategoryDef(int ruleCategoryDefIndex)
		{
			return RuleCatalog.allCategoryDefs[ruleCategoryDefIndex];
		}

		// Token: 0x0600199F RID: 6559 RVA: 0x0000AE8B File Offset: 0x0000908B
		private static bool HiddenTestTrue()
		{
			return true;
		}

		// Token: 0x060019A0 RID: 6560 RVA: 0x0000A1ED File Offset: 0x000083ED
		private static bool HiddenTestFalse()
		{
			return false;
		}

		// Token: 0x060019A1 RID: 6561 RVA: 0x0007A1AA File Offset: 0x000783AA
		private static bool HiddenTestItemsConvar()
		{
			return !RuleCatalog.ruleShowItems.value;
		}

		// Token: 0x060019A2 RID: 6562 RVA: 0x0007A1B9 File Offset: 0x000783B9
		private static void AddCategory(string displayToken, Color color)
		{
			RuleCatalog.AddCategory(displayToken, color, null, new Func<bool>(RuleCatalog.HiddenTestFalse));
		}

		// Token: 0x060019A3 RID: 6563 RVA: 0x0007A1CF File Offset: 0x000783CF
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

		// Token: 0x060019A4 RID: 6564 RVA: 0x0007A20C File Offset: 0x0007840C
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

		// Token: 0x060019A5 RID: 6565 RVA: 0x0007A2F4 File Offset: 0x000784F4
		static RuleCatalog()
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
			for (ItemIndex itemIndex = ItemIndex.Syringe; itemIndex < ItemIndex.Count; itemIndex++)
			{
				list.Add(itemIndex);
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
			for (EquipmentIndex equipmentIndex = EquipmentIndex.CommandMissile; equipmentIndex < EquipmentIndex.Count; equipmentIndex++)
			{
				list2.Add(equipmentIndex);
			}
			foreach (EquipmentIndex equipmentIndex2 in from i in list2
			where EquipmentCatalog.GetEquipmentDef(i).canDrop
			select i)
			{
				RuleCatalog.AddRule(RuleDef.FromEquipment(equipmentIndex2));
			}
			RuleCatalog.AddCategory("RULE_HEADER_MISC", new Color32(192, 192, 192, byte.MaxValue), null, new Func<bool>(RuleCatalog.HiddenTestFalse));
			RuleDef ruleDef = new RuleDef("Misc.StartingMoney", "RULE_MISC_STARTING_MONEY");
			RuleChoiceDef ruleChoiceDef = ruleDef.AddChoice("0", 0u, true);
			ruleChoiceDef.tooltipNameToken = "RULE_STARTINGMONEY_CHOICE_0_NAME";
			ruleChoiceDef.tooltipBodyToken = "RULE_STARTINGMONEY_CHOICE_0_DESC";
			ruleChoiceDef.tooltipNameColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarCoin);
			RuleChoiceDef ruleChoiceDef2 = ruleDef.AddChoice("15", 15u, true);
			ruleChoiceDef2.tooltipNameToken = "RULE_STARTINGMONEY_CHOICE_15_NAME";
			ruleChoiceDef2.tooltipBodyToken = "RULE_STARTINGMONEY_CHOICE_15_DESC";
			ruleChoiceDef2.tooltipNameColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarCoin);
			ruleDef.MakeNewestChoiceDefault();
			RuleChoiceDef ruleChoiceDef3 = ruleDef.AddChoice("50", 50u, true);
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

		// Token: 0x060019A6 RID: 6566 RVA: 0x0007A79C File Offset: 0x0007899C
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

		// Token: 0x04001CF5 RID: 7413
		private static readonly List<RuleDef> allRuleDefs = new List<RuleDef>();

		// Token: 0x04001CF6 RID: 7414
		private static readonly List<RuleChoiceDef> allChoicesDefs = new List<RuleChoiceDef>();

		// Token: 0x04001CF7 RID: 7415
		public static readonly List<RuleCategoryDef> allCategoryDefs = new List<RuleCategoryDef>();

		// Token: 0x04001CF8 RID: 7416
		private static readonly Dictionary<string, RuleDef> ruleDefsByGlobalName = new Dictionary<string, RuleDef>();

		// Token: 0x04001CF9 RID: 7417
		private static readonly Dictionary<string, RuleChoiceDef> ruleChoiceDefsByGlobalName = new Dictionary<string, RuleChoiceDef>();

		// Token: 0x04001CFA RID: 7418
		public static ResourceAvailability availability;

		// Token: 0x04001CFC RID: 7420
		private static readonly BoolConVar ruleShowItems = new BoolConVar("rule_show_items", ConVarFlags.Cheat, "0", "Whether or not to allow voting on items in the pregame rules.");
	}
}
