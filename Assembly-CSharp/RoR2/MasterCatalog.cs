using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RoR2.CharacterAI;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000458 RID: 1112
	public static class MasterCatalog
	{
		// Token: 0x1700024A RID: 586
		// (get) Token: 0x060018E0 RID: 6368 RVA: 0x00077787 File Offset: 0x00075987
		public static IEnumerable<CharacterMaster> allMasters
		{
			get
			{
				return MasterCatalog.masterPrefabMasterComponents;
			}
		}

		// Token: 0x1700024B RID: 587
		// (get) Token: 0x060018E1 RID: 6369 RVA: 0x0007778E File Offset: 0x0007598E
		public static IEnumerable<CharacterMaster> allAiMasters
		{
			get
			{
				return MasterCatalog.aiMasterPrefabs;
			}
		}

		// Token: 0x060018E2 RID: 6370 RVA: 0x00077795 File Offset: 0x00075995
		public static GameObject GetMasterPrefab(int index)
		{
			if (index < 0)
			{
				return null;
			}
			return MasterCatalog.masterPrefabs[index];
		}

		// Token: 0x060018E3 RID: 6371 RVA: 0x000777A4 File Offset: 0x000759A4
		public static int FindMasterIndex([NotNull] string bodyName)
		{
			int result;
			if (MasterCatalog.nameToIndexMap.TryGetValue(bodyName, out result))
			{
				return result;
			}
			return -1;
		}

		// Token: 0x060018E4 RID: 6372 RVA: 0x000777C3 File Offset: 0x000759C3
		public static int FindMasterIndex(GameObject bodyObject)
		{
			if (!bodyObject)
			{
				return -1;
			}
			return MasterCatalog.FindMasterIndex(bodyObject.name);
		}

		// Token: 0x060018E5 RID: 6373 RVA: 0x000777DC File Offset: 0x000759DC
		public static GameObject FindMasterPrefab([NotNull] string bodyName)
		{
			int num = MasterCatalog.FindMasterIndex(bodyName);
			if (num != -1)
			{
				return MasterCatalog.GetMasterPrefab(num);
			}
			return null;
		}

		// Token: 0x060018E6 RID: 6374 RVA: 0x000777FC File Offset: 0x000759FC
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			MasterCatalog.masterPrefabs = Resources.LoadAll<GameObject>("Prefabs/CharacterMasters/");
			MasterCatalog.masterPrefabMasterComponents = new CharacterMaster[MasterCatalog.masterPrefabs.Length];
			for (int i = 0; i < MasterCatalog.masterPrefabs.Length; i++)
			{
				MasterCatalog.nameToIndexMap.Add(MasterCatalog.masterPrefabs[i].name, i);
				MasterCatalog.nameToIndexMap.Add(MasterCatalog.masterPrefabs[i].name + "(Clone)", i);
				MasterCatalog.masterPrefabMasterComponents[i] = MasterCatalog.masterPrefabs[i].GetComponent<CharacterMaster>();
			}
			MasterCatalog.aiMasterPrefabs = (from master in MasterCatalog.masterPrefabMasterComponents
			where master.GetComponent<BaseAI>()
			select master).ToArray<CharacterMaster>();
			MasterCatalog.availability.MakeAvailable();
		}

		// Token: 0x04001C5A RID: 7258
		public static ResourceAvailability availability = default(ResourceAvailability);

		// Token: 0x04001C5B RID: 7259
		private static GameObject[] masterPrefabs;

		// Token: 0x04001C5C RID: 7260
		private static CharacterMaster[] masterPrefabMasterComponents;

		// Token: 0x04001C5D RID: 7261
		private static CharacterMaster[] aiMasterPrefabs;

		// Token: 0x04001C5E RID: 7262
		private static readonly Dictionary<string, int> nameToIndexMap = new Dictionary<string, int>();
	}
}
