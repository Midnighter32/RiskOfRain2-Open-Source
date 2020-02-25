using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RoR2.CharacterAI;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003CB RID: 971
	public static class MasterCatalog
	{
		// Token: 0x170002BE RID: 702
		// (get) Token: 0x060017A6 RID: 6054 RVA: 0x00066CBF File Offset: 0x00064EBF
		public static IEnumerable<CharacterMaster> allMasters
		{
			get
			{
				return MasterCatalog.masterPrefabMasterComponents;
			}
		}

		// Token: 0x170002BF RID: 703
		// (get) Token: 0x060017A7 RID: 6055 RVA: 0x00066CC6 File Offset: 0x00064EC6
		public static IEnumerable<CharacterMaster> allAiMasters
		{
			get
			{
				return MasterCatalog.aiMasterPrefabs;
			}
		}

		// Token: 0x060017A8 RID: 6056 RVA: 0x00066CCD File Offset: 0x00064ECD
		public static GameObject GetMasterPrefab(MasterCatalog.MasterIndex masterIndex)
		{
			return HGArrayUtilities.GetSafe<GameObject>(MasterCatalog.masterPrefabs, (int)masterIndex);
		}

		// Token: 0x060017A9 RID: 6057 RVA: 0x00066CE0 File Offset: 0x00064EE0
		public static MasterCatalog.MasterIndex FindMasterIndex([NotNull] string masterName)
		{
			MasterCatalog.MasterIndex result;
			if (MasterCatalog.nameToIndexMap.TryGetValue(masterName, out result))
			{
				return result;
			}
			return MasterCatalog.MasterIndex.none;
		}

		// Token: 0x060017AA RID: 6058 RVA: 0x00066D03 File Offset: 0x00064F03
		public static MasterCatalog.MasterIndex FindMasterIndex(GameObject masterObject)
		{
			if (!masterObject)
			{
				return MasterCatalog.MasterIndex.none;
			}
			return MasterCatalog.FindMasterIndex(masterObject.name);
		}

		// Token: 0x060017AB RID: 6059 RVA: 0x00066D20 File Offset: 0x00064F20
		public static GameObject FindMasterPrefab([NotNull] string bodyName)
		{
			MasterCatalog.MasterIndex masterIndex = MasterCatalog.FindMasterIndex(bodyName);
			if (masterIndex.isValid)
			{
				return MasterCatalog.GetMasterPrefab(masterIndex);
			}
			return null;
		}

		// Token: 0x14000057 RID: 87
		// (add) Token: 0x060017AC RID: 6060 RVA: 0x00066D48 File Offset: 0x00064F48
		// (remove) Token: 0x060017AD RID: 6061 RVA: 0x00066D7C File Offset: 0x00064F7C
		public static event Action<List<GameObject>> getAdditionalEntries;

		// Token: 0x060017AE RID: 6062 RVA: 0x00066DB0 File Offset: 0x00064FB0
		[SystemInitializer(new Type[]
		{

		})]
		private static void Init()
		{
			IEnumerable<GameObject> first = Resources.LoadAll<GameObject>("Prefabs/CharacterMasters/");
			List<GameObject> list = new List<GameObject>();
			Action<List<GameObject>> action = MasterCatalog.getAdditionalEntries;
			if (action != null)
			{
				action(list);
			}
			MasterCatalog.SetEntries(first.Concat(list.OrderBy((GameObject v) => v.name, StringComparer.Ordinal)).ToArray<GameObject>());
		}

		// Token: 0x060017AF RID: 6063 RVA: 0x00066E18 File Offset: 0x00065018
		private static void SetEntries(GameObject[] newEntries)
		{
			MasterCatalog.masterPrefabs = HGArrayUtilities.Clone<GameObject>(newEntries);
			MasterCatalog.masterPrefabMasterComponents = new CharacterMaster[MasterCatalog.masterPrefabs.Length];
			for (int i = 0; i < MasterCatalog.masterPrefabs.Length; i++)
			{
				MasterCatalog.MasterIndex value = new MasterCatalog.MasterIndex(i);
				MasterCatalog.nameToIndexMap.Add(MasterCatalog.masterPrefabs[i].name, value);
				MasterCatalog.nameToIndexMap.Add(MasterCatalog.masterPrefabs[i].name + "(Clone)", value);
				MasterCatalog.masterPrefabMasterComponents[i] = MasterCatalog.masterPrefabs[i].GetComponent<CharacterMaster>();
			}
			MasterCatalog.aiMasterPrefabs = (from master in MasterCatalog.masterPrefabMasterComponents
			where master.GetComponent<BaseAI>()
			select master).ToArray<CharacterMaster>();
		}

		// Token: 0x0400164D RID: 5709
		private static GameObject[] masterPrefabs;

		// Token: 0x0400164E RID: 5710
		private static CharacterMaster[] masterPrefabMasterComponents;

		// Token: 0x0400164F RID: 5711
		private static CharacterMaster[] aiMasterPrefabs;

		// Token: 0x04001650 RID: 5712
		private static readonly Dictionary<string, MasterCatalog.MasterIndex> nameToIndexMap = new Dictionary<string, MasterCatalog.MasterIndex>();

		// Token: 0x020003CC RID: 972
		public struct MasterIndex
		{
			// Token: 0x060017B1 RID: 6065 RVA: 0x00066EE5 File Offset: 0x000650E5
			public MasterIndex(int i)
			{
				this.i = i;
			}

			// Token: 0x170002C0 RID: 704
			// (get) Token: 0x060017B2 RID: 6066 RVA: 0x00066EEE File Offset: 0x000650EE
			public bool isValid
			{
				get
				{
					return this.i >= 0;
				}
			}

			// Token: 0x060017B3 RID: 6067 RVA: 0x00066EFC File Offset: 0x000650FC
			public static explicit operator int(MasterCatalog.MasterIndex masterIndex)
			{
				return masterIndex.i;
			}

			// Token: 0x04001652 RID: 5714
			private readonly int i;

			// Token: 0x04001653 RID: 5715
			public static readonly MasterCatalog.MasterIndex none = new MasterCatalog.MasterIndex(-1);
		}

		// Token: 0x020003CD RID: 973
		[Serializable]
		public struct NetworkMasterIndex
		{
			// Token: 0x060017B5 RID: 6069 RVA: 0x00066F14 File Offset: 0x00065114
			public static implicit operator MasterCatalog.NetworkMasterIndex(MasterCatalog.MasterIndex masterIndex)
			{
				return new MasterCatalog.NetworkMasterIndex
				{
					i = (uint)((int)masterIndex + 1)
				};
			}

			// Token: 0x060017B6 RID: 6070 RVA: 0x00066F39 File Offset: 0x00065139
			public static implicit operator MasterCatalog.MasterIndex(MasterCatalog.NetworkMasterIndex networkMasterIndex)
			{
				return new MasterCatalog.MasterIndex((int)(networkMasterIndex.i - 1U));
			}

			// Token: 0x04001654 RID: 5716
			public uint i;
		}
	}
}
