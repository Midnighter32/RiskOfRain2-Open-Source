using System;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003F9 RID: 1017
	public class RuleBook
	{
		// Token: 0x170002DE RID: 734
		// (get) Token: 0x0600188F RID: 6287 RVA: 0x00069D8E File Offset: 0x00067F8E
		public uint startingMoney
		{
			get
			{
				return (uint)this.GetRuleChoice(RuleBook.startingMoneyRule).extraData;
			}
		}

		// Token: 0x170002DF RID: 735
		// (get) Token: 0x06001890 RID: 6288 RVA: 0x00069DA5 File Offset: 0x00067FA5
		public StageOrder stageOrder
		{
			get
			{
				return (StageOrder)this.GetRuleChoice(RuleBook.stageOrderRule).extraData;
			}
		}

		// Token: 0x170002E0 RID: 736
		// (get) Token: 0x06001891 RID: 6289 RVA: 0x00069DBC File Offset: 0x00067FBC
		public bool keepMoneyBetweenStages
		{
			get
			{
				return (bool)this.GetRuleChoice(RuleBook.keepMoneyBetweenStagesRule).extraData;
			}
		}

		// Token: 0x06001892 RID: 6290 RVA: 0x00069DD4 File Offset: 0x00067FD4
		[SystemInitializer(new Type[]
		{
			typeof(RuleCatalog)
		})]
		private static void Init()
		{
			RuleBook.defaultValues = new byte[RuleCatalog.ruleCount];
			for (int i = 0; i < RuleCatalog.ruleCount; i++)
			{
				RuleBook.defaultValues[i] = (byte)RuleCatalog.GetRuleDef(i).defaultChoiceIndex;
			}
		}

		// Token: 0x06001893 RID: 6291 RVA: 0x00069E13 File Offset: 0x00068013
		public RuleBook()
		{
			HGXml.Register<RuleBook>(new HGXml.Serializer<RuleBook>(RuleBook.ToXml), new HGXml.Deserializer<RuleBook>(RuleBook.FromXml));
			this.SetToDefaults();
		}

		// Token: 0x06001894 RID: 6292 RVA: 0x00069E4E File Offset: 0x0006804E
		public void SetToDefaults()
		{
			Array.Copy(RuleBook.defaultValues, 0, this.ruleValues, 0, this.ruleValues.Length);
		}

		// Token: 0x06001895 RID: 6293 RVA: 0x00069E6A File Offset: 0x0006806A
		public void ApplyChoice(RuleChoiceDef choiceDef)
		{
			this.ruleValues[choiceDef.ruleDef.globalIndex] = (byte)choiceDef.localIndex;
		}

		// Token: 0x06001896 RID: 6294 RVA: 0x00069E85 File Offset: 0x00068085
		public int GetRuleChoiceIndex(int ruleIndex)
		{
			return (int)this.ruleValues[ruleIndex];
		}

		// Token: 0x06001897 RID: 6295 RVA: 0x00069E8F File Offset: 0x0006808F
		public int GetRuleChoiceIndex(RuleDef ruleDef)
		{
			return (int)this.ruleValues[ruleDef.globalIndex];
		}

		// Token: 0x06001898 RID: 6296 RVA: 0x00069E9E File Offset: 0x0006809E
		public RuleChoiceDef GetRuleChoice(int ruleIndex)
		{
			return RuleCatalog.GetRuleDef(ruleIndex).choices[(int)this.ruleValues[ruleIndex]];
		}

		// Token: 0x06001899 RID: 6297 RVA: 0x00069EB8 File Offset: 0x000680B8
		public RuleChoiceDef GetRuleChoice(RuleDef ruleDef)
		{
			return ruleDef.choices[(int)this.ruleValues[ruleDef.globalIndex]];
		}

		// Token: 0x0600189A RID: 6298 RVA: 0x00069ED4 File Offset: 0x000680D4
		public void Serialize(NetworkWriter writer)
		{
			for (int i = 0; i < this.ruleValues.Length; i++)
			{
				writer.Write(this.ruleValues[i]);
			}
		}

		// Token: 0x0600189B RID: 6299 RVA: 0x00069F04 File Offset: 0x00068104
		public void Deserialize(NetworkReader reader)
		{
			for (int i = 0; i < this.ruleValues.Length; i++)
			{
				this.ruleValues[i] = reader.ReadByte();
			}
		}

		// Token: 0x0600189C RID: 6300 RVA: 0x00069F34 File Offset: 0x00068134
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

		// Token: 0x0600189D RID: 6301 RVA: 0x00069F74 File Offset: 0x00068174
		public override int GetHashCode()
		{
			int num = 0;
			for (int i = 0; i < this.ruleValues.Length; i++)
			{
				num += (int)this.ruleValues[i];
			}
			return num;
		}

		// Token: 0x0600189E RID: 6302 RVA: 0x00069FA2 File Offset: 0x000681A2
		public void Copy([NotNull] RuleBook src)
		{
			if (src == null)
			{
				throw new ArgumentException("Argument cannot be null.", "src");
			}
			Array.Copy(src.ruleValues, this.ruleValues, this.ruleValues.Length);
		}

		// Token: 0x0600189F RID: 6303 RVA: 0x00069FD0 File Offset: 0x000681D0
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

		// Token: 0x060018A0 RID: 6304 RVA: 0x0006A01C File Offset: 0x0006821C
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

		// Token: 0x060018A1 RID: 6305 RVA: 0x0006A074 File Offset: 0x00068274
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

		// Token: 0x060018A2 RID: 6306 RVA: 0x0006A0CC File Offset: 0x000682CC
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

		// Token: 0x060018A3 RID: 6307 RVA: 0x0006A124 File Offset: 0x00068324
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

		// Token: 0x060018A4 RID: 6308 RVA: 0x0006A1B0 File Offset: 0x000683B0
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

		// Token: 0x060018A6 RID: 6310 RVA: 0x0006A230 File Offset: 0x00068430
		[CompilerGenerated]
		internal static void <ToXml>g__AddChoice|28_0(string globalChoiceName, ref RuleBook.<>c__DisplayClass28_0 A_1)
		{
			string[] choiceNamesBuffer = A_1.choiceNamesBuffer;
			int choiceNamesCount = A_1.choiceNamesCount;
			A_1.choiceNamesCount = choiceNamesCount + 1;
			choiceNamesBuffer[choiceNamesCount] = globalChoiceName;
		}

		// Token: 0x04001716 RID: 5910
		private readonly byte[] ruleValues = new byte[RuleCatalog.ruleCount];

		// Token: 0x04001717 RID: 5911
		protected static readonly RuleDef startingMoneyRule = RuleCatalog.FindRuleDef("Misc.StartingMoney");

		// Token: 0x04001718 RID: 5912
		protected static readonly RuleDef stageOrderRule = RuleCatalog.FindRuleDef("Misc.StageOrder");

		// Token: 0x04001719 RID: 5913
		protected static readonly RuleDef keepMoneyBetweenStagesRule = RuleCatalog.FindRuleDef("Misc.KeepMoneyBetweenStages");

		// Token: 0x0400171A RID: 5914
		private static byte[] defaultValues;
	}
}
