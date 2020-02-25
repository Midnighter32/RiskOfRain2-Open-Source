using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200038C RID: 908
	public static class GameModeCatalog
	{
		// Token: 0x1400004E RID: 78
		// (add) Token: 0x06001621 RID: 5665 RVA: 0x0005F460 File Offset: 0x0005D660
		// (remove) Token: 0x06001622 RID: 5666 RVA: 0x0005F494 File Offset: 0x0005D694
		public static event Action<List<GameObject>> getAdditionalEntries;

		// Token: 0x06001623 RID: 5667 RVA: 0x0005F4C8 File Offset: 0x0005D6C8
		[SystemInitializer(new Type[]
		{
			typeof(RuleCatalog)
		})]
		private static void LoadGameModes()
		{
			IEnumerable<GameObject> first = Resources.LoadAll<GameObject>("Prefabs/GameModes/");
			List<GameObject> list = new List<GameObject>();
			Action<List<GameObject>> action = GameModeCatalog.getAdditionalEntries;
			if (action != null)
			{
				action(list);
			}
			GameModeCatalog.indexToPrefabComponents = (from p in first.Concat(list.OrderBy((GameObject v) => v.name, StringComparer.Ordinal))
			select p.GetComponent<Run>()).ToArray<Run>();
			GameModeCatalog.nameToIndex.Clear();
			GameModeCatalog.nameToPrefabComponents.Clear();
			int i = 0;
			int num = GameModeCatalog.indexToPrefabComponents.Length;
			while (i < num)
			{
				Run run = GameModeCatalog.indexToPrefabComponents[i];
				string name = run.gameObject.name;
				string key = name + "(Clone)";
				GameModeCatalog.nameToIndex.Add(name, i);
				GameModeCatalog.nameToIndex.Add(key, i);
				GameModeCatalog.nameToPrefabComponents.Add(name, run);
				GameModeCatalog.nameToPrefabComponents.Add(key, run);
				Debug.LogFormat("Registered gamemode {0} {1}", new object[]
				{
					run.gameObject.name,
					run
				});
				i++;
			}
			GameModeCatalog.availability.MakeAvailable();
		}

		// Token: 0x06001624 RID: 5668 RVA: 0x0005F610 File Offset: 0x0005D810
		public static Run FindGameModePrefabComponent(string name)
		{
			Run result;
			GameModeCatalog.nameToPrefabComponents.TryGetValue(name, out result);
			return result;
		}

		// Token: 0x06001625 RID: 5669 RVA: 0x0005F62C File Offset: 0x0005D82C
		public static Run GetGameModePrefabComponent(int index)
		{
			if (index < 0 || index >= GameModeCatalog.indexToPrefabComponents.Length)
			{
				return null;
			}
			return GameModeCatalog.indexToPrefabComponents[index];
		}

		// Token: 0x06001626 RID: 5670 RVA: 0x0005F648 File Offset: 0x0005D848
		public static int FindGameModeIndex(string name)
		{
			int result;
			GameModeCatalog.nameToIndex.TryGetValue(name, out result);
			return result;
		}

		// Token: 0x040014D6 RID: 5334
		private static readonly Dictionary<string, int> nameToIndex = new Dictionary<string, int>();

		// Token: 0x040014D7 RID: 5335
		private static Run[] indexToPrefabComponents;

		// Token: 0x040014D8 RID: 5336
		private static readonly Dictionary<string, Run> nameToPrefabComponents = new Dictionary<string, Run>();

		// Token: 0x040014DA RID: 5338
		public static ResourceAvailability availability;
	}
}
