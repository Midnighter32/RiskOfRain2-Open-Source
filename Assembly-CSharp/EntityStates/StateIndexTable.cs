using System;
using System.Collections.Generic;
using System.Reflection;

namespace EntityStates
{
	// Token: 0x02000705 RID: 1797
	internal static class StateIndexTable
	{
		// Token: 0x060029EA RID: 10730 RVA: 0x000B0070 File Offset: 0x000AE270
		static StateIndexTable()
		{
			List<Type> list = new List<Type>();
			foreach (Type type in Assembly.GetAssembly(typeof(EntityState)).GetTypes())
			{
				if (type.IsSubclassOf(typeof(EntityState)) && !type.IsAbstract)
				{
					list.Add(type);
				}
			}
			list.Sort((Type a, Type b) => string.CompareOrdinal(a.Name, b.Name));
			StateIndexTable.stateIndexToType = list.ToArray();
			StateIndexTable.stateIndexToTypeName = new string[StateIndexTable.stateIndexToType.Length];
			short num = 0;
			while ((int)num < StateIndexTable.stateIndexToType.Length)
			{
				Type type2 = StateIndexTable.stateIndexToType[(int)num];
				StateIndexTable.stateIndexToTypeName[(int)num] = type2.FullName;
				StateIndexTable.stateTypeToIndex[type2] = num;
				num += 1;
			}
		}

		// Token: 0x060029EB RID: 10731 RVA: 0x000B0145 File Offset: 0x000AE345
		public static Type IndexToType(short stateTypeIndex)
		{
			if (stateTypeIndex >= 0 && (int)stateTypeIndex < StateIndexTable.stateIndexToType.Length)
			{
				return StateIndexTable.stateIndexToType[(int)stateTypeIndex];
			}
			return null;
		}

		// Token: 0x060029EC RID: 10732 RVA: 0x000B0160 File Offset: 0x000AE360
		public static short TypeToIndex(Type stateType)
		{
			short result;
			if (StateIndexTable.stateTypeToIndex.TryGetValue(stateType, out result))
			{
				return result;
			}
			return -1;
		}

		// Token: 0x1700041B RID: 1051
		// (get) Token: 0x060029ED RID: 10733 RVA: 0x000B017F File Offset: 0x000AE37F
		public static int typeCount
		{
			get
			{
				return StateIndexTable.stateIndexToType.Length;
			}
		}

		// Token: 0x1700041C RID: 1052
		// (get) Token: 0x060029EE RID: 10734 RVA: 0x000B0188 File Offset: 0x000AE388
		public static IList<string> typeNames
		{
			get
			{
				return Array.AsReadOnly<string>(StateIndexTable.stateIndexToTypeName);
			}
		}

		// Token: 0x040025BC RID: 9660
		private static Type[] stateIndexToType;

		// Token: 0x040025BD RID: 9661
		private static string[] stateIndexToTypeName;

		// Token: 0x040025BE RID: 9662
		private static Dictionary<Type, short> stateTypeToIndex = new Dictionary<Type, short>();
	}
}
