using System;
using UnityEngine;

// Token: 0x02000072 RID: 114
[Serializable]
public class SceneField
{
	// Token: 0x1700001E RID: 30
	// (get) Token: 0x060001D4 RID: 468 RVA: 0x00009C13 File Offset: 0x00007E13
	public string SceneName
	{
		get
		{
			return this.sceneName;
		}
	}

	// Token: 0x060001D5 RID: 469 RVA: 0x00009C1B File Offset: 0x00007E1B
	public SceneField(string sceneName)
	{
		this.sceneName = sceneName;
	}

	// Token: 0x060001D6 RID: 470 RVA: 0x00009C13 File Offset: 0x00007E13
	public static implicit operator string(SceneField sceneField)
	{
		return sceneField.sceneName;
	}

	// Token: 0x060001D7 RID: 471 RVA: 0x00009C35 File Offset: 0x00007E35
	public static implicit operator bool(SceneField sceneField)
	{
		return !string.IsNullOrEmpty(sceneField.sceneName);
	}

	// Token: 0x040001F2 RID: 498
	[SerializeField]
	private UnityEngine.Object sceneAsset;

	// Token: 0x040001F3 RID: 499
	[SerializeField]
	private string sceneName = "";
}
