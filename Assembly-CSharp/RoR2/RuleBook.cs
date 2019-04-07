using System;
using System.Xml.Linq;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000476 RID: 1142
	public class RuleBook
	{
		// Token: 0x1700025C RID: 604
		// (get) Token: 0x0600196D RID: 6509 RVA: 0x000799F2 File Offset: 0x00077BF2
		public uint startingMoney
		{
			get
			{
				return (uint)this.GetRuleChoice(RuleBook.startingMoneyRule).extraData;
			}
		}

		// Token: 0x1700025D RID: 605
		// (get) Token: 0x0600196E RID: 6510 RVA: 0x00079A09 File Offset: 0x00077C09
		public StageOrder stageOrder
		{
			get
			{
				return (StageOrder)this.GetRuleChoice(RuleBook.stageOrderRule).extraData;
			}
		}

		// Token: 0x1700025E RID: 606
		// (get) Token: 0x0600196F RID: 6511 RVA: 0x00079A20 File Offset: 0x00077C20
		public bool keepMoneyBetweenStages
		{
			get
			{
				return (bool)this.GetRuleChoice(RuleBook.keepMoneyBetweenStagesRule).extraData;
			}
		}

		// Token: 0x06001970 RID: 6512 RVA: 0x00079A38 File Offset: 0x00077C38
		static RuleBook()
		{
			RuleCatalog.availability.CallWhenAvailable(delegate
			{
				RuleBook.defaultValues = new byte[RuleCatalog.ruleCount];
				for (int i = 0; i < RuleCatalog.ruleCount; i++)
				{
					RuleBook.defaultValues[i] = (byte)RuleCatalog.GetRuleDef(i).defaultChoiceIndex;
				}
			});
		}

		// Token: 0x06001971 RID: 6513 RVA: 0x00079A8C File Offset: 0x00077C8C
		public RuleBook()
		{
			HGXml.Register<RuleBook>(new HGXml.Serializer<RuleBook>(RuleBook.ToXml), new HGXml.Deserializer<RuleBook>(RuleBook.FromXml));
			this.SetToDefaults();
		}

		// Token: 0x06001972 RID: 6514 RVA: 0x00079AC7 File Offset: 0x00077CC7
		public void SetToDefaults()
		{
			Array.Copy(RuleBook.defaultValues, 0, this.ruleValues, 0, this.ruleValues.Length);
		}

		// Token: 0x06001973 RID: 6515 RVA: 0x00079AE3 File Offset: 0x00077CE3
		public void ApplyChoice(RuleChoiceDef choiceDef)
		{
			this.ruleValues[choiceDef.ruleDef.globalIndex] = (byte)choiceDef.localIndex;
		}

		// Token: 0x06001974 RID: 6516 RVA: 0x00079AFE File Offset: 0x00077CFE
		public int GetRuleChoiceIndex(int ruleIndex)
		{
			return (int)this.ruleValues[ruleIndex];
		}

		// Token: 0x06001975 RID: 6517 RVA: 0x00079B08 File Offset: 0x00077D08
		public int GetRuleChoiceIndex(RuleDef ruleDef)
		{
			return (int)this.ruleValues[ruleDef.globalIndex];
		}

		// Token: 0x06001976 RID: 6518 RVA: 0x00079B17 File Offset: 0x00077D17
		public RuleChoiceDef GetRuleChoice(int ruleIndex)
		{
			return RuleCatalog.GetRuleDef(ruleIndex).choices[(int)this.ruleValues[ruleIndex]];
		}

		// Token: 0x06001977 RID: 6519 RVA: 0x00079B31 File Offset: 0x00077D31
		public RuleChoiceDef GetRuleChoice(RuleDef ruleDef)
		{
			return ruleDef.choices[(int)this.ruleValues[ruleDef.globalIndex]];
		}

		// Token: 0x06001978 RID: 6520 RVA: 0x00079B4C File Offset: 0x00077D4C
		public void Serialize(NetworkWriter writer)
		{
			for (int i = 0; i < this.ruleValues.Length; i++)
			{
				writer.Write(this.ruleValues[i]);
			}
		}

		// Token: 0x06001979 RID: 6521 RVA: 0x00079B7C File Offset: 0x00077D7C
		public void Deserialize(NetworkReader reader)
		{
			for (int i = 0; i < this.ruleValues.Length; i++)
			{
				this.ruleValues[i] = reader.ReadByte();
			}
		}

		// Token: 0x0600197A RID: 6522 RVA: 0x00079BAC File Offset: 0x00077DAC
		public override bool Equals(object obj)
		{
			RuleBook ruleBook = obj as RuleBook;
			if (ruleBook != null)
			{
				for (int i = 0; i < this.ruleValues.Length; i++)
				{
					if (this.ruleValues[i] != ruleBook.ruleValues[i])
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x0600197B RID: 6523 RVA: 0x00079BEC File Offset: 0x00077DEC
		public override int GetHashCode()
		{
			int num = 0;
			for (int i = 0; i < this.ruleValues.Length; i++)
			{
				num += (int)this.ruleValues[i];
			}
			return num;
		}

		// Token: 0x0600197C RID: 6524 RVA: 0x00079C1A File Offset: 0x00077E1A
		public void Copy([NotNull] RuleBook src)
		{
			Array.Copy(src.ruleValues, this.ruleValues, this.ruleValues.Length);
		}

		// Token: 0x0600197D RID: 6525 RVA: 0x00079C38 File Offset: 0x00077E38
		public DifficultyIndex FindDifficulty()
		{
			for (int i = 0; i < this.ruleValues.Length; i++)
			{
				RuleChoiceDef ruleChoiceDef = RuleCatalog.GetRuleDef(i).choices[(int)this.ruleValues[i]];
				if (ruleChoiceDef.difficultyIndex != DifficultyIndex.Invalid)
				{
					return ruleChoiceDef.difficultyIndex;
				}
			}
			return DifficultyIndex.Invalid;
		}

		// Token: 0x0600197E RID: 6526 RVA: 0x00079C84 File Offset: 0x00077E84
		public ArtifactMask GenerateArtifactMask()
		{
			ArtifactMask result = default(ArtifactMask);
			for (int i = 0; i < this.ruleValues.Length; i++)
			{
				RuleChoiceDef ruleChoiceDef = RuleCatalog.GetRuleDef(i).choices[(int)this.ruleValues[i]];
				if (ruleChoiceDef.artifactIndex != ArtifactIndex.None)
				{
					result.AddArtifact(ruleChoiceDef.artifactIndex);
				}
			}
			return result;
		}

		// Token: 0x0600197F RID: 6527 RVA: 0x00079CDC File Offset: 0x00077EDC
		public ItemMask GenerateItemMask()
		{
			ItemMask result = default(ItemMask);
			for (int i = 0; i < this.ruleValues.Length; i++)
			{
				RuleChoiceDef ruleChoiceDef = RuleCatalog.GetRuleDef(i).choices[(int)this.ruleValues[i]];
				if (ruleChoiceDef.itemIndex != ItemIndex.None)
				{
					result.AddItem(ruleChoiceDef.itemIndex);
				}
			}
			return result;
		}

		// Token: 0x06001980 RID: 6528 RVA: 0x00079D34 File Offset: 0x00077F34
		public EquipmentMask GenerateEquipmentMask()
		{
			EquipmentMask result = default(EquipmentMask);
			for (int i = 0; i < this.ruleValues.Length; i++)
			{
				RuleChoiceDef ruleChoiceDef = RuleCatalog.GetRuleDef(i).choices[(int)this.ruleValues[i]];
				if (ruleChoiceDef.equipmentIndex != EquipmentIndex.None)
				{
					result.AddEquipment(ruleChoiceDef.equipmentIndex);
				}
			}
			return result;
		}

		// Token: 0x06001981 RID: 6529 RVA: 0x00079D8C File Offset: 0x00077F8C
		public static void ToXml(XElement element, RuleBook src)
		{
			byte[] array = src.ruleValues;
			RuleBook.<>c__DisplayClass28_0 CS$<>8__locals1;
			CS$<>8__locals1.choiceNamesBuffer = new string[array.Length];
			CS$<>8__locals1.choiceNamesCount = 0;
			for (int i = 0; i < array.Length; i++)
			{
				RuleDef ruleDef = RuleCatalog.GetRuleDef(i);
				byte b = array[i];
				if ((ulong)b < (ulong)((long)ruleDef.choices.Count))
				{
					RuleBook.<ToXml>g__AddChoice|28_0(ruleDef.choices[(int)b].globalName, ref CS$<>8__locals1);
				}
			}
			element.Value = string.Join(" ", CS$<>8__locals1.choiceNamesBuffer, 0, CS$<>8__locals1.choiceNamesCount);
		}

		// Token: 0x06001982 RID: 6530 RVA: 0x00079E18 File Offset: 0x00078018
		public static bool FromXml(XElement element, ref RuleBook dest)
		{
			dest.SetToDefaults();
			string[] array = element.Value.Split(new char[]
			{
				' '
			});
			for (int i = 0; i < array.Length; i++)
			{
				RuleChoiceDef ruleChoiceDef = RuleCatalog.FindChoiceDef(array[i]);
				if (ruleChoiceDef != null)
				{
					dest.ApplyChoice(ruleChoiceDef);
				}
			}
			return true;
		}

		// Token: 0x04001CED RID: 7405
		private readonly byte[] ruleValues = new byte[RuleCatalog.ruleCount];

		// Token: 0x04001CEE RID: 7406
		protected static readonly RuleDef startingMoneyRule = RuleCatalog.FindRuleDef("Misc.StartingMoney");

		// Token: 0x04001CEF RID: 7407
		protected static readonly RuleDef stageOrderRule = RuleCatalog.FindRuleDef("Misc.StageOrder");

		// Token: 0x04001CF0 RID: 7408
		protected static readonly RuleDef keepMoneyBetweenStagesRule = RuleCatalog.FindRuleDef("Misc.KeepMoneyBetweenStages");

		// Token: 0x04001CF1 RID: 7409
		private static byte[] defaultValues;
	}
}
