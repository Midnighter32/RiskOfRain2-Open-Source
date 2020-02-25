using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000110 RID: 272
	[CreateAssetMenu]
	public class SceneDef : ScriptableObject
	{
		// Token: 0x1700009C RID: 156
		// (get) Token: 0x0600050D RID: 1293 RVA: 0x00014422 File Offset: 0x00012622
		// (set) Token: 0x0600050E RID: 1294 RVA: 0x0001442A File Offset: 0x0001262A
		public int sceneDefIndex { get; set; }

		// Token: 0x0600050F RID: 1295 RVA: 0x00014433 File Offset: 0x00012633
		private void Awake()
		{
			this.baseSceneName = base.name;
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x06000510 RID: 1296 RVA: 0x00014441 File Offset: 0x00012641
		// (set) Token: 0x06000511 RID: 1297 RVA: 0x00014449 File Offset: 0x00012649
		public string baseSceneName { get; private set; }

		// Token: 0x06000512 RID: 1298 RVA: 0x00014452 File Offset: 0x00012652
		public string ChooseSceneName()
		{
			if (Run.instance && this.sceneNameOverrides.Count > 0)
			{
				return Run.instance.nextStageRng.NextElementUniform<string>(this.sceneNameOverrides);
			}
			return this.baseSceneName;
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x06000513 RID: 1299 RVA: 0x0001448A File Offset: 0x0001268A
		[Obsolete("SceneDef.name should not be used due to unnecessary managed allocations. Use sceneName instead.")]
		public new string name
		{
			get
			{
				return base.name;
			}
		}

		// Token: 0x040004F0 RID: 1264
		public string nameToken;

		// Token: 0x040004F1 RID: 1265
		public string subtitleToken;

		// Token: 0x040004F2 RID: 1266
		public string loreToken;

		// Token: 0x040004F3 RID: 1267
		public int stageOrder;

		// Token: 0x040004F4 RID: 1268
		public Texture previewTexture;

		// Token: 0x040004F5 RID: 1269
		public Material portalMaterial;

		// Token: 0x040004F6 RID: 1270
		public string portalSelectionMessageString;

		// Token: 0x040004F7 RID: 1271
		public GameObject dioramaPrefab;

		// Token: 0x040004F8 RID: 1272
		public SceneType sceneType;

		// Token: 0x040004F9 RID: 1273
		public string songName;

		// Token: 0x040004FA RID: 1274
		public string bossSongName;

		// Token: 0x040004FB RID: 1275
		public bool isOfflineScene;

		// Token: 0x040004FC RID: 1276
		public bool suppressNpcEntry;

		// Token: 0x040004FD RID: 1277
		[Tooltip("Stages that can be destinations of the teleporter.")]
		public SceneDef[] destinations;

		// Token: 0x040004FE RID: 1278
		public List<string> sceneNameOverrides;
	}
}
