using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200042F RID: 1071
	public static class GameModeCatalog
	{
		// Token: 0x060017D9 RID: 6105 RVA: 0x00071E0F File Offset: 0x0007000F
		static GameModeCatalog()
		{
			RoR2Application.onLoad = (Action)Delegate.Combine(RoR2Application.onLoad, new Action(GameModeCatalog.LoadGameModes));
		}

		// Token: 0x060017DA RID: 6106 RVA: 0x00071E48 File Offset: 0x00070048
		private static void LoadGameModes()
		{
			GameModeCatalog.indexToPrefabComponents = (from p in Resources.LoadAll<GameObject>("Prefabs/GameModes/")
			orderby p.name
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

		// Token: 0x060017DB RID: 6107 RVA: 0x00071F68 File Offset: 0x00070168
		public static Run FindGameModePrefabComponent(string name)
		{
			Run result;
			GameModeCatalog.nameToPrefabComponents.TryGetValue(name, out result);
			return result;
		}

		// Token: 0x060017DC RID: 6108 RVA: 0x00071F84 File Offset: 0x00070184
		public static Run GetGameModePrefabComponent(int index)
		{
			if (index < 0 || index >= GameModeCatalog.indexToPrefabComponents.Length)
			{
				return null;
			}
			return GameModeCatalog.indexToPrefabComponents[index];
		}

		// Token: 0x060017DD RID: 6109 RVA: 0x00071FA0 File Offset: 0x000701A0
		public static int FindGameModeIndex(string name)
		{
			int result;
			GameModeCatalog.nameToIndex.TryGetValue(name, out result);
			return result;
		}

		// Token: 0x04001B36 RID: 6966
		private static readonly Dictionary<string, int> nameToIndex = new Dictionary<string, int>();

		// Token: 0x04001B37 RID: 6967
		private static Run[] indexToPrefabComponents;

		// Token: 0x04001B38 RID: 6968
		private static readonly Dictionary<string, Run> nameToPrefabComponents = new Dictionary<string, Run>();

		// Token: 0x04001B39 RID: 6969
		public static ResourceAvailability availability;
	}
}
