using System;
using System.IO;
using JetBrains.Annotations;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003ED RID: 1005
	public class AssetCheckArgs
	{
		// Token: 0x170002D6 RID: 726
		// (get) Token: 0x0600185F RID: 6239 RVA: 0x00069436 File Offset: 0x00067636
		[CanBeNull]
		public Component assetComponent
		{
			get
			{
				return this.asset as Component;
			}
		}

		// Token: 0x170002D7 RID: 727
		// (get) Token: 0x06001860 RID: 6240 RVA: 0x00069443 File Offset: 0x00067643
		[CanBeNull]
		public GameObject gameObject
		{
			get
			{
				Component assetComponent = this.assetComponent;
				if (assetComponent == null)
				{
					return null;
				}
				return assetComponent.gameObject;
			}
		}

		// Token: 0x170002D8 RID: 728
		// (get) Token: 0x06001861 RID: 6241 RVA: 0x00069456 File Offset: 0x00067656
		[CanBeNull]
		public GameObject gameObjectRoot
		{
			get
			{
				GameObject gameObject = this.gameObject;
				if (gameObject == null)
				{
					return null;
				}
				Transform root = gameObject.transform.root;
				if (root == null)
				{
					return null;
				}
				return root.gameObject;
			}
		}

		// Token: 0x170002D9 RID: 729
		// (get) Token: 0x06001862 RID: 6242 RVA: 0x00069479 File Offset: 0x00067679
		public bool isPrefab
		{
			get
			{
				return AssetCheckArgs.GameObjectIsPrefab(this.gameObjectRoot);
			}
		}

		// Token: 0x170002DA RID: 730
		// (get) Token: 0x06001863 RID: 6243 RVA: 0x00069486 File Offset: 0x00067686
		public bool isPrefabVariant
		{
			get
			{
				return AssetCheckArgs.GameObjectIsPrefabVariant(this.gameObjectRoot);
			}
		}

		// Token: 0x06001864 RID: 6244 RVA: 0x0000AC89 File Offset: 0x00008E89
		private static bool GameObjectIsPrefab(GameObject gameObject)
		{
			return false;
		}

		// Token: 0x06001865 RID: 6245 RVA: 0x0000AC89 File Offset: 0x00008E89
		private static bool GameObjectIsPrefabVariant(GameObject gameObject)
		{
			return false;
		}

		// Token: 0x170002DB RID: 731
		// (get) Token: 0x06001866 RID: 6246 RVA: 0x00069494 File Offset: 0x00067694
		[CanBeNull]
		public GameObject prefabRoot
		{
			get
			{
				GameObject gameObjectRoot = this.gameObjectRoot;
				if (AssetCheckArgs.GameObjectIsPrefab(gameObjectRoot))
				{
					return gameObjectRoot;
				}
				return null;
			}
		}

		// Token: 0x06001867 RID: 6247 RVA: 0x000694B3 File Offset: 0x000676B3
		public void UpdatePrefab()
		{
			this.prefabRoot;
		}

		// Token: 0x06001868 RID: 6248 RVA: 0x000694C1 File Offset: 0x000676C1
		public void Log(string str, UnityEngine.Object context = null)
		{
			this.projectIssueChecker.Log(str, context);
		}

		// Token: 0x06001869 RID: 6249 RVA: 0x000694D0 File Offset: 0x000676D0
		public void LogError(string str, UnityEngine.Object context = null)
		{
			this.projectIssueChecker.LogError(str, context);
		}

		// Token: 0x0600186A RID: 6250 RVA: 0x000694DF File Offset: 0x000676DF
		public void LogFormat(UnityEngine.Object context, string format, params object[] formatArgs)
		{
			this.projectIssueChecker.LogFormat(context, format, formatArgs);
		}

		// Token: 0x0600186B RID: 6251 RVA: 0x000694EF File Offset: 0x000676EF
		public void LogErrorFormat(UnityEngine.Object context, string format, params object[] formatArgs)
		{
			this.projectIssueChecker.LogErrorFormat(context, format, formatArgs);
		}

		// Token: 0x0600186C RID: 6252 RVA: 0x000694FF File Offset: 0x000676FF
		public void EnsurePath(string path)
		{
			Directory.Exists(path);
		}

		// Token: 0x0600186D RID: 6253 RVA: 0x00069508 File Offset: 0x00067708
		public T LoadAsset<T>(string fullFilePath) where T : UnityEngine.Object
		{
			File.Exists(fullFilePath);
			return default(T);
		}

		// Token: 0x0600186E RID: 6254 RVA: 0x0000409B File Offset: 0x0000229B
		public static void CreateAsset<T>(string path)
		{
		}

		// Token: 0x040016EC RID: 5868
		[NotNull]
		public ProjectIssueChecker projectIssueChecker;

		// Token: 0x040016ED RID: 5869
		[NotNull]
		public UnityEngine.Object asset;
	}
}
