using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RoR2
{
	// Token: 0x02000407 RID: 1031
	public static class SceneCatalog
	{
		// Token: 0x170002EE RID: 750
		// (get) Token: 0x0600190A RID: 6410 RVA: 0x0006C0C5 File Offset: 0x0006A2C5
		public static int sceneDefCount
		{
			get
			{
				return SceneCatalog.indexToSceneDef.Length;
			}
		}

		// Token: 0x170002EF RID: 751
		// (get) Token: 0x0600190B RID: 6411 RVA: 0x0006C0CE File Offset: 0x0006A2CE
		public static IEnumerable<SceneDef> allSceneDefs
		{
			get
			{
				return SceneCatalog.indexToSceneDef;
			}
		}

		// Token: 0x0600190C RID: 6412 RVA: 0x0006C0D5 File Offset: 0x0006A2D5
		[NotNull]
		public static SceneDef GetSceneDef(int sceneIndex)
		{
			return SceneCatalog.indexToSceneDef[sceneIndex];
		}

		// Token: 0x170002F0 RID: 752
		// (get) Token: 0x0600190D RID: 6413 RVA: 0x0006C0DE File Offset: 0x0006A2DE
		// (set) Token: 0x0600190E RID: 6414 RVA: 0x0006C0E5 File Offset: 0x0006A2E5
		[NotNull]
		public static SceneDef mostRecentSceneDef { get; private set; }

		// Token: 0x0600190F RID: 6415 RVA: 0x0006C0F0 File Offset: 0x0006A2F0
		[SystemInitializer(new Type[]
		{

		})]
		private static void Init()
		{
			IEnumerable<SceneDef> first = Resources.LoadAll<SceneDef>("SceneDefs/");
			List<SceneDef> list = new List<SceneDef>();
			Action<List<SceneDef>> action = SceneCatalog.getAdditionalEntries;
			if (action != null)
			{
				action(list);
			}
			SceneCatalog.SetSceneDefs(first.Concat(list.OrderBy((SceneDef v) => v.name, StringComparer.Ordinal)).ToArray<SceneDef>());
			SceneCatalog.availability.MakeAvailable();
		}

		// Token: 0x06001910 RID: 6416 RVA: 0x0006C164 File Offset: 0x0006A364
		private static void SetSceneDefs(SceneDef[] newSceneDefs)
		{
			SceneCatalog.indexToSceneDef = HGArrayUtilities.Clone<SceneDef>(newSceneDefs);
			for (int i = 0; i < SceneCatalog.indexToSceneDef.Length; i++)
			{
				SceneCatalog.indexToSceneDef[i].sceneDefIndex = i;
			}
			SceneManager.activeSceneChanged += delegate(Scene oldScene, Scene newScene)
			{
				SceneCatalog.currentSceneDef = SceneCatalog.GetSceneDefFromSceneName(newScene.name);
				if (SceneCatalog.currentSceneDef != null)
				{
					SceneCatalog.mostRecentSceneDef = SceneCatalog.currentSceneDef;
					Action<SceneDef> action = SceneCatalog.onMostRecentSceneDefChanged;
					if (action == null)
					{
						return;
					}
					action(SceneCatalog.mostRecentSceneDef);
				}
			};
			SceneCatalog.currentSceneDef = SceneCatalog.GetSceneDefFromSceneName(SceneManager.GetActiveScene().name);
			SceneCatalog.mostRecentSceneDef = SceneCatalog.currentSceneDef;
		}

		// Token: 0x06001911 RID: 6417 RVA: 0x0006C1E0 File Offset: 0x0006A3E0
		[NotNull]
		public static string GetUnlockableLogFromSceneName([NotNull] string baseSceneName)
		{
			return string.Format(CultureInfo.InvariantCulture, "Logs.Stages.{0}", baseSceneName);
		}

		// Token: 0x06001912 RID: 6418 RVA: 0x0006C1F2 File Offset: 0x0006A3F2
		[CanBeNull]
		public static SceneDef GetSceneDefForCurrentScene()
		{
			return SceneCatalog.GetSceneDefFromScene(SceneManager.GetActiveScene());
		}

		// Token: 0x06001913 RID: 6419 RVA: 0x0006C200 File Offset: 0x0006A400
		[CanBeNull]
		public static SceneDef GetSceneDefFromSceneName([NotNull] string name)
		{
			for (int i = 0; i < SceneCatalog.indexToSceneDef.Length; i++)
			{
				SceneDef sceneDef = SceneCatalog.indexToSceneDef[i];
				List<string> sceneNameOverrides = sceneDef.sceneNameOverrides;
				if (sceneNameOverrides.Count == 0)
				{
					if (sceneDef.baseSceneName.Equals(name, StringComparison.OrdinalIgnoreCase))
					{
						return sceneDef;
					}
				}
				else
				{
					for (int j = 0; j < sceneNameOverrides.Count; j++)
					{
						if (sceneNameOverrides[j].Equals(name, StringComparison.OrdinalIgnoreCase))
						{
							return sceneDef;
						}
					}
				}
			}
			return null;
		}

		// Token: 0x06001914 RID: 6420 RVA: 0x0006C26B File Offset: 0x0006A46B
		[CanBeNull]
		public static SceneDef GetSceneDefFromScene(Scene scene)
		{
			return SceneCatalog.GetSceneDefFromSceneName(scene.name);
		}

		// Token: 0x14000059 RID: 89
		// (add) Token: 0x06001915 RID: 6421 RVA: 0x0006C27C File Offset: 0x0006A47C
		// (remove) Token: 0x06001916 RID: 6422 RVA: 0x0006C2B0 File Offset: 0x0006A4B0
		public static event Action<SceneDef> onMostRecentSceneDefChanged;

		// Token: 0x04001768 RID: 5992
		private static SceneDef[] indexToSceneDef;

		// Token: 0x04001769 RID: 5993
		private static string currentSceneName = string.Empty;

		// Token: 0x0400176A RID: 5994
		private static SceneDef currentSceneDef;

		// Token: 0x0400176C RID: 5996
		public static Action<List<SceneDef>> getAdditionalEntries;

		// Token: 0x0400176D RID: 5997
		public static ResourceAvailability availability;
	}
}
