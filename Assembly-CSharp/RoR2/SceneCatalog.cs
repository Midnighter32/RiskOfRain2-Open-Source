using System;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RoR2
{
	// Token: 0x02000485 RID: 1157
	public static class SceneCatalog
	{
		// Token: 0x1700026B RID: 619
		// (get) Token: 0x060019E8 RID: 6632 RVA: 0x0007BC81 File Offset: 0x00079E81
		public static int sceneDefCount
		{
			get
			{
				return SceneCatalog.indexToSceneDef.Length;
			}
		}

		// Token: 0x1700026C RID: 620
		// (get) Token: 0x060019E9 RID: 6633 RVA: 0x0007BC8A File Offset: 0x00079E8A
		public static IEnumerable<SceneDef> allSceneDefs
		{
			get
			{
				return SceneCatalog.indexToSceneDef;
			}
		}

		// Token: 0x060019EA RID: 6634 RVA: 0x0007BC91 File Offset: 0x00079E91
		[NotNull]
		public static SceneDef GetSceneDef(int sceneIndex)
		{
			return SceneCatalog.indexToSceneDef[sceneIndex];
		}

		// Token: 0x1700026D RID: 621
		// (get) Token: 0x060019EB RID: 6635 RVA: 0x0007BC9A File Offset: 0x00079E9A
		// (set) Token: 0x060019EC RID: 6636 RVA: 0x0007BCA1 File Offset: 0x00079EA1
		[NotNull]
		public static SceneDef mostRecentSceneDef { get; private set; }

		// Token: 0x060019ED RID: 6637 RVA: 0x0007BCAC File Offset: 0x00079EAC
		[SystemInitializer(new Type[]
		{

		})]
		private static void Init()
		{
			SceneCatalog.indexToSceneDef = Resources.LoadAll<SceneDef>("SceneDefs/");
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
			SceneCatalog.availability.MakeAvailable();
		}

		// Token: 0x060019EE RID: 6638 RVA: 0x0007BD17 File Offset: 0x00079F17
		[NotNull]
		public static string GetUnlockableLogFromSceneName([NotNull] string name)
		{
			return string.Format(CultureInfo.InvariantCulture, "Logs.Stages.{0}", name);
		}

		// Token: 0x060019EF RID: 6639 RVA: 0x0007BD29 File Offset: 0x00079F29
		[CanBeNull]
		public static SceneDef GetSceneDefForCurrentScene()
		{
			return SceneCatalog.GetSceneDefFromScene(SceneManager.GetActiveScene());
		}

		// Token: 0x060019F0 RID: 6640 RVA: 0x0007BD38 File Offset: 0x00079F38
		[CanBeNull]
		public static SceneDef GetSceneDefFromSceneName([NotNull] string name)
		{
			for (int i = 0; i < SceneCatalog.indexToSceneDef.Length; i++)
			{
				if (SceneCatalog.indexToSceneDef[i].sceneName == name)
				{
					return SceneCatalog.indexToSceneDef[i];
				}
			}
			return null;
		}

		// Token: 0x060019F1 RID: 6641 RVA: 0x0007BD74 File Offset: 0x00079F74
		[CanBeNull]
		public static SceneDef GetSceneDefFromScene(Scene scene)
		{
			return SceneCatalog.GetSceneDefFromSceneName(scene.name);
		}

		// Token: 0x1400003B RID: 59
		// (add) Token: 0x060019F2 RID: 6642 RVA: 0x0007BD84 File Offset: 0x00079F84
		// (remove) Token: 0x060019F3 RID: 6643 RVA: 0x0007BDB8 File Offset: 0x00079FB8
		public static event Action<SceneDef> onMostRecentSceneDefChanged;

		// Token: 0x04001D3C RID: 7484
		private static SceneDef[] indexToSceneDef;

		// Token: 0x04001D3D RID: 7485
		private static string currentSceneName = string.Empty;

		// Token: 0x04001D3E RID: 7486
		private static SceneDef currentSceneDef;

		// Token: 0x04001D40 RID: 7488
		public static ResourceAvailability availability;
	}
}
