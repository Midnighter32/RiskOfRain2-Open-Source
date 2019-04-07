using System;
using System.Collections.Generic;

namespace RoR2
{
	// Token: 0x0200047F RID: 1151
	public class RuleDef
	{
		// Token: 0x060019AF RID: 6575 RVA: 0x0007A930 File Offset: 0x00078B30
		public RuleChoiceDef AddChoice(string choiceName, object extraData = null, bool excludeByDefault = false)
		{
			RuleChoiceDef ruleChoiceDef = new RuleChoiceDef();
			ruleChoiceDef.ruleDef = this;
			ruleChoiceDef.localName = choiceName;
			ruleChoiceDef.globalName = this.globalName + "." + choiceName;
			ruleChoiceDef.localIndex = this.choices.Count;
			ruleChoiceDef.extraData = extraData;
			ruleChoiceDef.excludeByDefault = excludeByDefault;
			this.choices.Add(ruleChoiceDef);
			return ruleChoiceDef;
		}

		// Token: 0x060019B0 RID: 6576 RVA: 0x0007A994 File Offset: 0x00078B94
		public RuleChoiceDef FindChoice(string choiceLocalName)
		{
			int i = 0;
			int count = this.choices.Count;
			while (i < count)
			{
				if (this.choices[i].localName == choiceLocalName)
				{
					return this.choices[i];
				}
				i++;
			}
			return null;
		}

		// Token: 0x060019B1 RID: 6577 RVA: 0x0007A9E0 File Offset: 0x00078BE0
		public void MakeNewestChoiceDefault()
		{
			this.defaultChoiceIndex = this.choices.Count - 1;
		}

		// Token: 0x060019B2 RID: 6578 RVA: 0x0007A9F5 File Offset: 0x00078BF5
		public RuleDef(string globalName, string displayToken)
		{
			this.globalName = globalName;
			this.displayToken = displayToken;
		}

		// Token: 0x060019B3 RID: 6579 RVA: 0x0007AA18 File Offset: 0x00078C18
		public static RuleDef FromDifficulty()
		{
			RuleDef ruleDef = new RuleDef("Difficulty", "RULE_NAME_DIFFICULTY");
			for (DifficultyIndex difficultyIndex = DifficultyIndex.Easy; difficultyIndex < DifficultyIndex.Count; difficultyIndex++)
			{
				DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(difficultyIndex);
				RuleChoiceDef ruleChoiceDef = ruleDef.AddChoice(difficultyIndex.ToString(), null, false);
				ruleChoiceDef.spritePath = difficultyDef.iconPath;
				ruleChoiceDef.tooltipNameToken = difficultyDef.nameToken;
				ruleChoiceDef.tooltipNameColor = difficultyDef.color;
				ruleChoiceDef.tooltipBodyToken = difficultyDef.descriptionToken;
				ruleChoiceDef.difficultyIndex = difficultyIndex;
			}
			ruleDef.defaultChoiceIndex = 1;
			return ruleDef;
		}

		// Token: 0x060019B4 RID: 6580 RVA: 0x0007AA9C File Offset: 0x00078C9C
		public static RuleDef FromArtifact(ArtifactIndex artifactIndex)
		{
			ArtifactDef artifactDef = ArtifactCatalog.GetArtifactDef(artifactIndex);
			RuleDef ruleDef = new RuleDef("Artifacts." + artifactIndex.ToString(), artifactDef.nameToken);
			RuleChoiceDef ruleChoiceDef = ruleDef.AddChoice("On", null, false);
			ruleChoiceDef.spritePath = artifactDef.smallIconSelectedPath;
			ruleChoiceDef.tooltipBodyToken = artifactDef.descriptionToken;
			ruleChoiceDef.unlockableName = artifactDef.unlockableName;
			ruleChoiceDef.artifactIndex = artifactIndex;
			RuleChoiceDef ruleChoiceDef2 = ruleDef.AddChoice("Off", null, false);
			ruleChoiceDef2.spritePath = artifactDef.smallIconDeselectedPath;
			ruleChoiceDef2.materialPath = "Materials/UI/matRuleChoiceOff";
			ruleChoiceDef2.tooltipBodyToken = null;
			ruleDef.MakeNewestChoiceDefault();
			return ruleDef;
		}

		// Token: 0x060019B5 RID: 6581 RVA: 0x0007AB3C File Offset: 0x00078D3C
		public static RuleDef FromItem(ItemIndex itemIndex)
		{
			ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
			RuleDef ruleDef = new RuleDef("Items." + itemIndex.ToString(), itemDef.nameToken);
			RuleChoiceDef ruleChoiceDef = ruleDef.AddChoice("On", null, false);
			ruleChoiceDef.spritePath = itemDef.pickupIconPath;
			ruleChoiceDef.tooltipNameToken = itemDef.nameToken;
			ruleChoiceDef.unlockableName = itemDef.unlockableName;
			ruleChoiceDef.itemIndex = itemIndex;
			ruleDef.MakeNewestChoiceDefault();
			RuleChoiceDef ruleChoiceDef2 = ruleDef.AddChoice("Off", null, false);
			ruleChoiceDef2.spritePath = itemDef.pickupIconPath;
			ruleChoiceDef2.materialPath = "Materials/UI/matRuleChoiceOff";
			ruleChoiceDef2.tooltipNameToken = null;
			return ruleDef;
		}

		// Token: 0x060019B6 RID: 6582 RVA: 0x0007ABDC File Offset: 0x00078DDC
		public static RuleDef FromEquipment(EquipmentIndex equipmentIndex)
		{
			EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(equipmentIndex);
			RuleDef ruleDef = new RuleDef("Equipment." + equipmentIndex.ToString(), equipmentDef.nameToken);
			RuleChoiceDef ruleChoiceDef = ruleDef.AddChoice("On", null, false);
			ruleChoiceDef.spritePath = equipmentDef.pickupIconPath;
			ruleChoiceDef.tooltipBodyToken = equipmentDef.nameToken;
			ruleChoiceDef.unlockableName = equipmentDef.unlockableName;
			ruleChoiceDef.equipmentIndex = equipmentIndex;
			ruleChoiceDef.availableInMultiPlayer = equipmentDef.appearsInMultiPlayer;
			ruleChoiceDef.availableInSinglePlayer = equipmentDef.appearsInSinglePlayer;
			ruleDef.MakeNewestChoiceDefault();
			RuleChoiceDef ruleChoiceDef2 = ruleDef.AddChoice("Off", null, false);
			ruleChoiceDef2.spritePath = equipmentDef.pickupIconPath;
			ruleChoiceDef2.materialPath = "Materials/UI/matRuleChoiceOff";
			ruleChoiceDef2.tooltipBodyToken = null;
			return ruleDef;
		}

		// Token: 0x04001D18 RID: 7448
		public readonly string globalName;

		// Token: 0x04001D19 RID: 7449
		public int globalIndex;

		// Token: 0x04001D1A RID: 7450
		public readonly string displayToken;

		// Token: 0x04001D1B RID: 7451
		public readonly List<RuleChoiceDef> choices = new List<RuleChoiceDef>();

		// Token: 0x04001D1C RID: 7452
		public int defaultChoiceIndex;

		// Token: 0x04001D1D RID: 7453
		public RuleCategoryDef category;

		// Token: 0x04001D1E RID: 7454
		private const string pathToOffChoiceMaterial = "Materials/UI/matRuleChoiceOff";
	}
}
