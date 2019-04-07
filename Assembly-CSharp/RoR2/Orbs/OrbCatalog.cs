using System;
using System.Collections.Generic;
using System.Linq;

namespace RoR2.Orbs
{
	// Token: 0x0200051A RID: 1306
	public static class OrbCatalog
	{
		// Token: 0x06001D5A RID: 7514 RVA: 0x00088E40 File Offset: 0x00087040
		private static void GenerateCatalog()
		{
			OrbCatalog.indexToType = (from t in typeof(Orb).Assembly.GetTypes()
			where t.IsSubclassOf(typeof(Orb))
			orderby t.Name
			select t).ToArray<Type>();
			OrbCatalog.typeToIndex.Clear();
			foreach (Type key in OrbCatalog.indexToType)
			{
				OrbCatalog.typeToIndex[key] = OrbCatalog.typeToIndex.Count;
			}
		}

		// Token: 0x06001D5B RID: 7515 RVA: 0x00088EEA File Offset: 0x000870EA
		static OrbCatalog()
		{
			OrbCatalog.GenerateCatalog();
		}

		// Token: 0x06001D5C RID: 7516 RVA: 0x00088F08 File Offset: 0x00087108
		public static int FindIndex(Type type)
		{
			int result;
			if (OrbCatalog.typeToIndex.TryGetValue(type, out result))
			{
				return result;
			}
			return -1;
		}

		// Token: 0x06001D5D RID: 7517 RVA: 0x00088F27 File Offset: 0x00087127
		public static Type FindType(int index)
		{
			if (index < 0 || index >= OrbCatalog.indexToType.Length)
			{
				return null;
			}
			return OrbCatalog.indexToType[index];
		}

		// Token: 0x06001D5E RID: 7518 RVA: 0x00088F40 File Offset: 0x00087140
		public static Orb Instantiate(int index)
		{
			return OrbCatalog.Instantiate(OrbCatalog.FindType(index));
		}

		// Token: 0x06001D5F RID: 7519 RVA: 0x00088F4D File Offset: 0x0008714D
		public static Orb Instantiate(Type type)
		{
			if (type == null)
			{
				return null;
			}
			return (Orb)Activator.CreateInstance(type);
		}

		// Token: 0x04001FA5 RID: 8101
		private static readonly Dictionary<Type, int> typeToIndex = new Dictionary<Type, int>();

		// Token: 0x04001FA6 RID: 8102
		private static Type[] indexToType = Array.Empty<Type>();
	}
}
