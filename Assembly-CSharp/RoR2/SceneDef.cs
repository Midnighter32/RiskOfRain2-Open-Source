using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200022D RID: 557
	[CreateAssetMenu]
	public class SceneDef : ScriptableObject
	{
		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000AAF RID: 2735 RVA: 0x00034D7E File Offset: 0x00032F7E
		public SceneField sceneField
		{
			get
			{
				return new SceneField(this.sceneName);
			}
		}

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000AB0 RID: 2736 RVA: 0x00034D8B File Offset: 0x00032F8B
		public string sceneName
		{
			get
			{
				if (this.cachedSceneName == null)
				{
					this.cachedSceneName = base.name;
				}
				return this.cachedSceneName;
			}
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000AB1 RID: 2737 RVA: 0x00034DA7 File Offset: 0x00032FA7
		[Obsolete("SceneDef.name should not be used due to unnecessary managed allocations. Use sceneName instead.")]
		public new string name
		{
			get
			{
				return base.name;
			}
		}

		// Token: 0x04000E3B RID: 3643
		public string nameToken;

		// Token: 0x04000E3C RID: 3644
		public string subtitleToken;

		// Token: 0x04000E3D RID: 3645
		public string loreToken;

		// Token: 0x04000E3E RID: 3646
		public int stageOrder;

		// Token: 0x04000E3F RID: 3647
		public Texture previewTexture;

		// Token: 0x04000E40 RID: 3648
		public GameObject dioramaPrefab;

		// Token: 0x04000E41 RID: 3649
		public SceneType sceneType;

		// Token: 0x04000E42 RID: 3650
		public string songName;

		// Token: 0x04000E43 RID: 3651
		public string bossSongName;

		// Token: 0x04000E44 RID: 3652
		public bool isOfflineScene;

		// Token: 0x04000E45 RID: 3653
		private string cachedSceneName;
	}
}
