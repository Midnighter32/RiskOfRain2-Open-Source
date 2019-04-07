using System;
using System.Collections.Generic;
using System.Reflection;

namespace EntityStates
{
	// Token: 0x020000AE RID: 174
	internal static class StateIndexTable
	{
		// Token: 0x06000370 RID: 880 RVA: 0x0000DB48 File Offset: 0x0000BD48
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

		// Token: 0x06000371 RID: 881 RVA: 0x0000DC1D File Offset: 0x0000BE1D
		public static Type IndexToType(short stateTypeIndex)
		{
			if (stateTypeIndex >= 0 && (int)stateTypeIndex < StateIndexTable.stateIndexToType.Length)
			{
				return StateIndexTable.stateIndexToType[(int)stateTypeIndex];
			}
			return null;
		}

		// Token: 0x06000372 RID: 882 RVA: 0x0000DC38 File Offset: 0x0000BE38
		public static short TypeToIndex(Type stateType)
		{
			short result;
			if (StateIndexTable.stateTypeToIndex.TryGetValue(stateType, out result))
			{
				return result;
			}
			return -1;
		}

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x06000373 RID: 883 RVA: 0x0000DC57 File Offset: 0x0000BE57
		public static int typeCount
		{
			get
			{
				return StateIndexTable.stateIndexToType.Length;
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x06000374 RID: 884 RVA: 0x0000DC60 File Offset: 0x0000BE60
		public static IList<string> typeNames
		{
			get
			{
				return Array.AsReadOnly<string>(StateIndexTable.stateIndexToTypeName);
			}
		}

		// Token: 0x04000329 RID: 809
		private static Type[] stateIndexToType;

		// Token: 0x0400032A RID: 810
		private static string[] stateIndexToTypeName;

		// Token: 0x0400032B RID: 811
		private static Dictionary<Type, short> stateTypeToIndex = new Dictionary<Type, short>();
	}
}
