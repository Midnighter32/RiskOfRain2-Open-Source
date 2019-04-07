using System;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;
using UnityEngine.Networking;

namespace RoR2.Stats
{
	// Token: 0x02000508 RID: 1288
	public class StatSheet
	{
		// Token: 0x06001D09 RID: 7433 RVA: 0x0008770F File Offset: 0x0008590F
		public void SetStatValueFromString([CanBeNull] StatDef statDef, string value)
		{
			if (statDef == null)
			{
				return;
			}
			this.fields[statDef.index].SetFromString(value);
		}

		// Token: 0x06001D0A RID: 7434 RVA: 0x0008772C File Offset: 0x0008592C
		public void PushStatValue([CanBeNull] StatDef statDef, ulong statValue)
		{
			if (statDef == null)
			{
				return;
			}
			this.fields[statDef.index].PushStatValue(statValue);
		}

		// Token: 0x06001D0B RID: 7435 RVA: 0x00087749 File Offset: 0x00085949
		public void PushStatValue([CanBeNull] StatDef statDef, double statValue)
		{
			if (statDef == null)
			{
				return;
			}
			this.fields[statDef.index].PushStatValue(statValue);
		}

		// Token: 0x06001D0C RID: 7436 RVA: 0x00087766 File Offset: 0x00085966
		public void PushStatValue([NotNull] PerBodyStatDef perBodyStatDef, [NotNull] string bodyName, ulong statValue)
		{
			this.PushStatValue(perBodyStatDef.FindStatDef(bodyName), statValue);
		}

		// Token: 0x06001D0D RID: 7437 RVA: 0x00087776 File Offset: 0x00085976
		public void PushStatValue([NotNull] PerBodyStatDef perBodyStatDef, [NotNull] string bodyName, double statValue)
		{
			this.PushStatValue(perBodyStatDef.FindStatDef(bodyName), statValue);
		}

		// Token: 0x06001D0E RID: 7438 RVA: 0x00087786 File Offset: 0x00085986
		public ulong GetStatValueULong([CanBeNull] StatDef statDef)
		{
			if (statDef == null)
			{
				return 0UL;
			}
			return this.fields[statDef.index].GetULongValue();
		}

		// Token: 0x06001D0F RID: 7439 RVA: 0x000877A4 File Offset: 0x000859A4
		public double GetStatValueDouble([CanBeNull] StatDef statDef)
		{
			if (statDef == null)
			{
				return 0.0;
			}
			return this.fields[statDef.index].GetDoubleValue();
		}

		// Token: 0x06001D10 RID: 7440 RVA: 0x000877C9 File Offset: 0x000859C9
		[NotNull]
		public string GetStatValueString([CanBeNull] StatDef statDef)
		{
			if (statDef == null)
			{
				return "INVALID_STAT";
			}
			return this.fields[statDef.index].ToString();
		}

		// Token: 0x06001D11 RID: 7441 RVA: 0x000877F0 File Offset: 0x000859F0
		[NotNull]
		public string GetStatDisplayValue([CanBeNull] StatDef statDef)
		{
			if (statDef == null)
			{
				return "INVALID_STAT";
			}
			return statDef.displayValueFormatter(ref this.fields[statDef.index]);
		}

		// Token: 0x06001D12 RID: 7442 RVA: 0x00087817 File Offset: 0x00085A17
		public ulong GetStatPointValue([NotNull] StatDef statDef)
		{
			return this.fields[statDef.index].GetPointValue(statDef.pointValue);
		}

		// Token: 0x06001D13 RID: 7443 RVA: 0x00087835 File Offset: 0x00085A35
		public ulong GetStatValueULong([NotNull] PerBodyStatDef perBodyStatDef, [NotNull] string bodyName)
		{
			return this.GetStatValueULong(perBodyStatDef.FindStatDef(bodyName));
		}

		// Token: 0x06001D14 RID: 7444 RVA: 0x00087844 File Offset: 0x00085A44
		public double GetStatValueDouble([NotNull] PerBodyStatDef perBodyStatDef, [NotNull] string bodyName)
		{
			return this.GetStatValueDouble(perBodyStatDef.FindStatDef(bodyName));
		}

		// Token: 0x06001D15 RID: 7445 RVA: 0x00087853 File Offset: 0x00085A53
		[NotNull]
		public string GetStatValueString([NotNull] PerBodyStatDef perBodyStatDef, [NotNull] string bodyName)
		{
			return this.GetStatValueString(perBodyStatDef.FindStatDef(bodyName));
		}

		// Token: 0x06001D16 RID: 7446 RVA: 0x00087862 File Offset: 0x00085A62
		[SystemInitializer(new Type[]
		{
			typeof(StatDef)
		})]
		private static void Init()
		{
			StatSheet.OnFieldsFinalized();
		}

		// Token: 0x06001D17 RID: 7447 RVA: 0x00087869 File Offset: 0x00085A69
		static StatSheet()
		{
			HGXml.Register<StatSheet>(new HGXml.Serializer<StatSheet>(StatSheet.ToXml), new HGXml.Deserializer<StatSheet>(StatSheet.FromXml));
		}

		// Token: 0x06001D18 RID: 7448 RVA: 0x00087888 File Offset: 0x00085A88
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

		// Token: 0x06001D19 RID: 7449 RVA: 0x000878FC File Offset: 0x00085AFC
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

		// Token: 0x06001D1A RID: 7450 RVA: 0x00087964 File Offset: 0x00085B64
		private static void OnFieldsFinalized()
		{
			StatSheet.fieldsTemplate = (from v in StatDef.allStatDefs
			select new StatField
			{
				statDef = v
			}).ToArray<StatField>();
			StatSheet.nonDefaultFieldsBuffer = new bool[StatSheet.fieldsTemplate.Length];
		}

		// Token: 0x06001D1B RID: 7451 RVA: 0x000879B5 File Offset: 0x00085BB5
		private StatSheet([NotNull] StatField[] fields)
		{
			this.fields = fields;
		}

		// Token: 0x06001D1C RID: 7452 RVA: 0x000879D0 File Offset: 0x00085BD0
		public static StatSheet New()
		{
			StatField[] array = new StatField[StatSheet.fieldsTemplate.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = StatSheet.fieldsTemplate[i];
			}
			return new StatSheet(array);
		}

		// Token: 0x06001D1D RID: 7453 RVA: 0x00087A10 File Offset: 0x00085C10
		public int GetUnlockableCount()
		{
			return this.unlockables.Length;
		}

		// Token: 0x06001D1E RID: 7454 RVA: 0x00087A1A File Offset: 0x00085C1A
		public UnlockableIndex GetUnlockableIndex(int index)
		{
			return this.unlockables[index];
		}

		// Token: 0x06001D1F RID: 7455 RVA: 0x00087A28 File Offset: 0x00085C28
		public UnlockableDef GetUnlockable(int index)
		{
			return UnlockableCatalog.GetUnlockableDef(this.unlockables[index]);
		}

		// Token: 0x06001D20 RID: 7456 RVA: 0x00087A3C File Offset: 0x00085C3C
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

		// Token: 0x06001D21 RID: 7457 RVA: 0x00087A78 File Offset: 0x00085C78
		private void AllocateUnlockables(int desiredCount)
		{
			Array.Resize<UnlockableIndex>(ref this.unlockables, desiredCount);
		}

		// Token: 0x06001D22 RID: 7458 RVA: 0x00087A86 File Offset: 0x00085C86
		public void AddUnlockable([NotNull] UnlockableDef unlockableDef)
		{
			this.AddUnlockable(unlockableDef.index);
		}

		// Token: 0x06001D23 RID: 7459 RVA: 0x00087A94 File Offset: 0x00085C94
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

		// Token: 0x06001D24 RID: 7460 RVA: 0x00087AF4 File Offset: 0x00085CF4
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

		// Token: 0x06001D25 RID: 7461 RVA: 0x00087B38 File Offset: 0x00085D38
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

		// Token: 0x06001D26 RID: 7462 RVA: 0x00087BE0 File Offset: 0x00085DE0
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

		// Token: 0x06001D27 RID: 7463 RVA: 0x00087C68 File Offset: 0x00085E68
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

		// Token: 0x06001D28 RID: 7464 RVA: 0x00087D8C File Offset: 0x00085F8C
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

		// Token: 0x06001D29 RID: 7465 RVA: 0x00087DF0 File Offset: 0x00085FF0
		public static void Copy([NotNull] StatSheet src, [NotNull] StatSheet dest)
		{
			Array.Copy(src.fields, dest.fields, src.fields.Length);
			dest.AllocateUnlockables(src.unlockables.Length);
			Array.Copy(src.unlockables, dest.unlockables, src.unlockables.Length);
		}

		// Token: 0x04001F48 RID: 8008
		private static StatField[] fieldsTemplate;

		// Token: 0x04001F49 RID: 8009
		private static bool[] nonDefaultFieldsBuffer;

		// Token: 0x04001F4A RID: 8010
		public readonly StatField[] fields;

		// Token: 0x04001F4B RID: 8011
		private UnlockableIndex[] unlockables = Array.Empty<UnlockableIndex>();
	}
}
