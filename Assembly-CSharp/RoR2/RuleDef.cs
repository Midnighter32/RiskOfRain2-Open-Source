using System;
using System.Collections.Generic;

namespace RoR2
{
	// Token: 0x02000401 RID: 1025
	public class RuleDef
	{
		// Token: 0x060018D1 RID: 6353 RVA: 0x0006AD0C File Offset: 0x00068F0C
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

		// Token: 0x060018D2 RID: 6354 RVA: 0x0006AD70 File Offset: 0x00068F70
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

		// Token: 0x060018D3 RID: 6355 RVA: 0x0006ADBC File Offset: 0x00068FBC
		public void MakeNewestChoiceDefault()
		{
			this.defaultChoiceIndex = this.choices.Count - 1;
		}

		// Token: 0x060018D4 RID: 6356 RVA: 0x0006ADD1 File Offset: 0x00068FD1
		public RuleDef(string globalName, string displayToken)
		{
			this.globalName = globalName;
			this.displayToken = displayToken;
		}

		// Token: 0x060018D5 RID: 6357 RVA: 0x0006ADF4 File Offset: 0x00068FF4
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

		// Token: 0x060018D6 RID: 6358 RVA: 0x0006AE78 File Offset: 0x00069078
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

		// Token: 0x060018D7 RID: 6359 RVA: 0x0006AF18 File Offset: 0x00069118
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

		// Token: 0x060018D8 RID: 6360 RVA: 0x0006AFB8 File Offset: 0x000691B8
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

		// Token: 0x04001743 RID: 5955
		public readonly string globalName;

		// Token: 0x04001744 RID: 5956
		public int globalIndex;

		// Token: 0x04001745 RID: 5957
		public readonly string displayToken;

		// Token: 0x04001746 RID: 5958
		public readonly List<RuleChoiceDef> choices = new List<RuleChoiceDef>();

		// Token: 0x04001747 RID: 5959
		public int defaultChoiceIndex;

		// Token: 0x04001748 RID: 5960
		public RuleCategoryDef category;

		// Token: 0x04001749 RID: 5961
		private const string pathToOffChoiceMaterial = "Materials/UI/matRuleChoiceOff";
	}
}
