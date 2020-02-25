using System;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace RoR2.Stats
{
	// Token: 0x020004AD RID: 1197
	public class StatSheet
	{
		// Token: 0x06001CE8 RID: 7400 RVA: 0x0007BEF7 File Offset: 0x0007A0F7
		public void SetStatValueFromString([CanBeNull] StatDef statDef, string value)
		{
			if (statDef == null)
			{
				return;
			}
			this.fields[statDef.index].SetFromString(value);
		}

		// Token: 0x06001CE9 RID: 7401 RVA: 0x0007BF14 File Offset: 0x0007A114
		public void PushStatValue([CanBeNull] StatDef statDef, ulong statValue)
		{
			if (statDef == null)
			{
				return;
			}
			this.fields[statDef.index].PushStatValue(statValue);
		}

		// Token: 0x06001CEA RID: 7402 RVA: 0x0007BF31 File Offset: 0x0007A131
		public void PushStatValue([CanBeNull] StatDef statDef, double statValue)
		{
			if (statDef == null)
			{
				return;
			}
			this.fields[statDef.index].PushStatValue(statValue);
		}

		// Token: 0x06001CEB RID: 7403 RVA: 0x0007BF4E File Offset: 0x0007A14E
		public void PushStatValue([NotNull] PerBodyStatDef perBodyStatDef, int bodyIndex, ulong statValue)
		{
			this.PushStatValue(perBodyStatDef.FindStatDef(bodyIndex), statValue);
		}

		// Token: 0x06001CEC RID: 7404 RVA: 0x0007BF5E File Offset: 0x0007A15E
		public void PushStatValue([NotNull] PerBodyStatDef perBodyStatDef, int bodyIndex, double statValue)
		{
			this.PushStatValue(perBodyStatDef.FindStatDef(bodyIndex), statValue);
		}

		// Token: 0x06001CED RID: 7405 RVA: 0x0007BF6E File Offset: 0x0007A16E
		public ulong GetStatValueULong([CanBeNull] StatDef statDef)
		{
			if (statDef == null)
			{
				return 0UL;
			}
			return this.fields[statDef.index].GetULongValue();
		}

		// Token: 0x06001CEE RID: 7406 RVA: 0x0007BF8C File Offset: 0x0007A18C
		public double GetStatValueDouble([CanBeNull] StatDef statDef)
		{
			if (statDef == null)
			{
				return 0.0;
			}
			return this.fields[statDef.index].GetDoubleValue();
		}

		// Token: 0x06001CEF RID: 7407 RVA: 0x0007BFB1 File Offset: 0x0007A1B1
		[NotNull]
		public string GetStatValueString([CanBeNull] StatDef statDef)
		{
			if (statDef == null)
			{
				return "INVALID_STAT";
			}
			return this.fields[statDef.index].ToString();
		}

		// Token: 0x06001CF0 RID: 7408 RVA: 0x0007BFD8 File Offset: 0x0007A1D8
		[NotNull]
		public string GetStatDisplayValue([CanBeNull] StatDef statDef)
		{
			if (statDef == null)
			{
				return "INVALID_STAT";
			}
			return statDef.displayValueFormatter(ref this.fields[statDef.index]);
		}

		// Token: 0x06001CF1 RID: 7409 RVA: 0x0007BFFF File Offset: 0x0007A1FF
		public ulong GetStatPointValue([NotNull] StatDef statDef)
		{
			return this.fields[statDef.index].GetPointValue(statDef.pointValue);
		}

		// Token: 0x06001CF2 RID: 7410 RVA: 0x0007C01D File Offset: 0x0007A21D
		public ulong GetStatValueULong([NotNull] PerBodyStatDef perBodyStatDef, [NotNull] string bodyName)
		{
			return this.GetStatValueULong(perBodyStatDef.FindStatDef(bodyName));
		}

		// Token: 0x06001CF3 RID: 7411 RVA: 0x0007C02C File Offset: 0x0007A22C
		public double GetStatValueDouble([NotNull] PerBodyStatDef perBodyStatDef, [NotNull] string bodyName)
		{
			return this.GetStatValueDouble(perBodyStatDef.FindStatDef(bodyName));
		}

		// Token: 0x06001CF4 RID: 7412 RVA: 0x0007C03B File Offset: 0x0007A23B
		[NotNull]
		public string GetStatValueString([NotNull] PerBodyStatDef perBodyStatDef, [NotNull] string bodyName)
		{
			return this.GetStatValueString(perBodyStatDef.FindStatDef(bodyName));
		}

		// Token: 0x06001CF5 RID: 7413 RVA: 0x0007C04A File Offset: 0x0007A24A
		[SystemInitializer(new Type[]
		{
			typeof(StatDef)
		})]
		private static void Init()
		{
			StatSheet.OnFieldsFinalized();
		}

		// Token: 0x06001CF6 RID: 7414 RVA: 0x0007C051 File Offset: 0x0007A251
		static StatSheet()
		{
			HGXml.Register<StatSheet>(new HGXml.Serializer<StatSheet>(StatSheet.ToXml), new HGXml.Deserializer<StatSheet>(StatSheet.FromXml));
		}

		// Token: 0x06001CF7 RID: 7415 RVA: 0x0007C070 File Offset: 0x0007A270
		public static void ToXml(XElement element, StatSheet statSheet)
		{
			element.RemoveAll();
			XElement xelement = new XElement("fields");
			element.Add(xelement);
			StatField[] array = statSheet.fields;
			for (int i = 0; i < array.Length; i++)
			{
				ref StatField ptr = ref array[i];
				if (!ptr.IsDefault())
				{
					xelement.Add(new XElement(ptr.name, ptr.ToString()));
				}
			}
		}

		// Token: 0x06001CF8 RID: 7416 RVA: 0x0007C0E4 File Offset: 0x0007A2E4
		public static bool FromXml(XElement element, ref StatSheet statSheet)
		{
			XElement xelement = element.Element("fields");
			if (xelement == null)
			{
				return false;
			}
			StatField[] array = statSheet.fields;
			for (int i = 0; i < array.Length; i++)
			{
				ref StatField ptr = ref array[i];
				XElement xelement2 = xelement.Element(ptr.name);
				if (xelement2 != null)
				{
					ptr.SetFromString(xelement2.Value);
				}
			}
			return true;
		}

		// Token: 0x06001CF9 RID: 7417 RVA: 0x0007C14C File Offset: 0x0007A34C
		private static void OnFieldsFinalized()
		{
			StatSheet.fieldsTemplate = (from v in StatDef.allStatDefs
			select new StatField
			{
				statDef = v
			}).ToArray<StatField>();
			StatSheet.nonDefaultFieldsBuffer = new bool[StatSheet.fieldsTemplate.Length];
		}

		// Token: 0x06001CFA RID: 7418 RVA: 0x0007C19D File Offset: 0x0007A39D
		private StatSheet([NotNull] StatField[] fields)
		{
			this.fields = fields;
		}

		// Token: 0x06001CFB RID: 7419 RVA: 0x0007C1B8 File Offset: 0x0007A3B8
		public static StatSheet New()
		{
			StatField[] array = new StatField[StatSheet.fieldsTemplate.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = StatSheet.fieldsTemplate[i];
			}
			return new StatSheet(array);
		}

		// Token: 0x06001CFC RID: 7420 RVA: 0x0007C1F8 File Offset: 0x0007A3F8
		public int GetUnlockableCount()
		{
			return this.unlockables.Length;
		}

		// Token: 0x06001CFD RID: 7421 RVA: 0x0007C202 File Offset: 0x0007A402
		public UnlockableIndex GetUnlockableIndex(int index)
		{
			return this.unlockables[index];
		}

		// Token: 0x06001CFE RID: 7422 RVA: 0x0007C210 File Offset: 0x0007A410
		public UnlockableDef GetUnlockable(int index)
		{
			return UnlockableCatalog.GetUnlockableDef(this.unlockables[index]);
		}

		// Token: 0x06001CFF RID: 7423 RVA: 0x0007C224 File Offset: 0x0007A424
		public bool HasUnlockable(UnlockableDef unlockableDef)
		{
			for (int i = 0; i < this.unlockables.Length; i++)
			{
				if (this.unlockables[i] == unlockableDef.index)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001D00 RID: 7424 RVA: 0x0007C260 File Offset: 0x0007A460
		private void AllocateUnlockables(int desiredCount)
		{
			Array.Resize<UnlockableIndex>(ref this.unlockables, desiredCount);
		}

		// Token: 0x06001D01 RID: 7425 RVA: 0x0007C26E File Offset: 0x0007A46E
		public void AddUnlockable([NotNull] UnlockableDef unlockableDef)
		{
			this.AddUnlockable(unlockableDef.index);
		}

		// Token: 0x06001D02 RID: 7426 RVA: 0x0007C27C File Offset: 0x0007A47C
		public void AddUnlockable(UnlockableIndex unlockIndex)
		{
			for (int i = 0; i < this.unlockables.Length; i++)
			{
				if (this.unlockables[i] == unlockIndex)
				{
					return;
				}
			}
			Array.Resize<UnlockableIndex>(ref this.unlockables, this.unlockables.Length + 1);
			this.unlockables[this.unlockables.Length - 1] = unlockIndex;
		}

		// Token: 0x06001D03 RID: 7427 RVA: 0x0007C2DC File Offset: 0x0007A4DC
		public void RemoveUnlockable(UnlockableIndex unlockIndex)
		{
			int num = Array.IndexOf<UnlockableIndex>(this.unlockables, unlockIndex);
			if (num == -1)
			{
				return;
			}
			int newSize = this.unlockables.Length;
			HGArrayUtilities.ArrayRemoveAt<UnlockableIndex>(ref this.unlockables, ref newSize, num, 1);
			Array.Resize<UnlockableIndex>(ref this.unlockables, newSize);
		}

		// Token: 0x06001D04 RID: 7428 RVA: 0x0007C320 File Offset: 0x0007A520
		public void Write(NetworkWriter writer)
		{
			for (int i = 0; i < this.fields.Length; i++)
			{
				StatSheet.nonDefaultFieldsBuffer[i] = !this.fields[i].IsDefault();
			}
			writer.WriteBitArray(StatSheet.nonDefaultFieldsBuffer);
			for (int j = 0; j < this.fields.Length; j++)
			{
				if (StatSheet.nonDefaultFieldsBuffer[j])
				{
					this.fields[j].Write(writer);
				}
			}
			writer.Write((byte)this.unlockables.Length);
			for (int k = 0; k < this.unlockables.Length; k++)
			{
				writer.Write(this.unlockables[k]);
			}
		}

		// Token: 0x06001D05 RID: 7429 RVA: 0x0007C3C8 File Offset: 0x0007A5C8
		public void Read(NetworkReader reader)
		{
			reader.ReadBitArray(StatSheet.nonDefaultFieldsBuffer);
			for (int i = 0; i < this.fields.Length; i++)
			{
				if (StatSheet.nonDefaultFieldsBuffer[i])
				{
					this.fields[i].Read(reader);
				}
				else
				{
					this.fields[i].SetDefault();
				}
			}
			int num = (int)reader.ReadByte();
			this.AllocateUnlockables(num);
			for (int j = 0; j < num; j++)
			{
				this.unlockables[j] = reader.ReadUnlockableIndex();
			}
		}

		// Token: 0x06001D06 RID: 7430 RVA: 0x0007C450 File Offset: 0x0007A650
		public static void GetDelta(StatSheet result, StatSheet newerStats, StatSheet olderStats)
		{
			StatField[] array = result.fields;
			StatField[] array2 = newerStats.fields;
			StatField[] array3 = olderStats.fields;
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = StatField.GetDelta(ref array2[i], ref array3[i]);
			}
			UnlockableIndex[] array4 = newerStats.unlockables;
			UnlockableIndex[] array5 = olderStats.unlockables;
			int num = 0;
			foreach (UnlockableIndex b in array4)
			{
				bool flag = false;
				for (int k = 0; k < array5.Length; k++)
				{
					if (array5[k] == b)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					num++;
				}
			}
			result.AllocateUnlockables(num);
			UnlockableIndex[] array6 = result.unlockables;
			int num2 = 0;
			foreach (UnlockableIndex unlockableIndex in array4)
			{
				bool flag2 = false;
				for (int m = 0; m < array5.Length; m++)
				{
					if (array5[m] == unlockableIndex)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					array6[num2++] = unlockableIndex;
				}
			}
		}

		// Token: 0x06001D07 RID: 7431 RVA: 0x0007C574 File Offset: 0x0007A774
		public void ApplyDelta(StatSheet deltaSheet)
		{
			StatField[] array = deltaSheet.fields;
			for (int i = 0; i < this.fields.Length; i++)
			{
				this.fields[i].PushDelta(ref array[i]);
			}
			for (int j = 0; j < deltaSheet.unlockables.Length; j++)
			{
				this.AddUnlockable(deltaSheet.unlockables[j]);
			}
		}

		// Token: 0x06001D08 RID: 7432 RVA: 0x0007C5D8 File Offset: 0x0007A7D8
		public static void Copy([NotNull] StatSheet src, [NotNull] StatSheet dest)
		{
			Array.Copy(src.fields, dest.fields, src.fields.Length);
			dest.AllocateUnlockables(src.unlockables.Length);
			Array.Copy(src.unlockables, dest.unlockables, src.unlockables.Length);
		}

		// Token: 0x04001A1A RID: 6682
		private static StatField[] fieldsTemplate;

		// Token: 0x04001A1B RID: 6683
		private static bool[] nonDefaultFieldsBuffer;

		// Token: 0x04001A1C RID: 6684
		public readonly StatField[] fields;

		// Token: 0x04001A1D RID: 6685
		private UnlockableIndex[] unlockables = Array.Empty<UnlockableIndex>();
	}
}
