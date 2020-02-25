using System;
using System.Collections.Generic;
using System.Linq;

namespace RoR2.Orbs
{
	// Token: 0x020004D4 RID: 1236
	public static class OrbCatalog
	{
		// Token: 0x06001D8B RID: 7563 RVA: 0x0007E180 File Offset: 0x0007C380
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

		// Token: 0x06001D8C RID: 7564 RVA: 0x0007E22A File Offset: 0x0007C42A
		static OrbCatalog()
		{
			OrbCatalog.GenerateCatalog();
		}

		// Token: 0x06001D8D RID: 7565 RVA: 0x0007E248 File Offset: 0x0007C448
		public static int FindIndex(Type type)
		{
			int result;
			if (OrbCatalog.typeToIndex.TryGetValue(type, out result))
			{
				return result;
			}
			return -1;
		}

		// Token: 0x06001D8E RID: 7566 RVA: 0x0007E267 File Offset: 0x0007C467
		public static Type FindType(int index)
		{
			if (index < 0 || index >= OrbCatalog.indexToType.Length)
			{
				return null;
			}
			return OrbCatalog.indexToType[index];
		}

		// Token: 0x06001D8F RID: 7567 RVA: 0x0007E280 File Offset: 0x0007C480
		public static Orb Instantiate(int index)
		{
			return OrbCatalog.Instantiate(OrbCatalog.FindType(index));
		}

		// Token: 0x06001D90 RID: 7568 RVA: 0x0007E28D File Offset: 0x0007C48D
		public static Orb Instantiate(Type type)
		{
			if (type == null)
			{
				return null;
			}
			return (Orb)Activator.CreateInstance(type);
		}

		// Token: 0x04001ABD RID: 6845
		private static readonly Dictionary<Type, int> typeToIndex = new Dictionary<Type, int>();

		// Token: 0x04001ABE RID: 6846
		private static Type[] indexToType = Array.Empty<Type>();
	}
}
