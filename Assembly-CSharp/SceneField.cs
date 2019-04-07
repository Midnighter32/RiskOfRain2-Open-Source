using System;
using UnityEngine;

// Token: 0x0200006E RID: 110
[Serializable]
public class SceneField
{
	// Token: 0x17000019 RID: 25
	// (get) Token: 0x060001A6 RID: 422 RVA: 0x00009307 File Offset: 0x00007507
	public string SceneName
	{
		get
		{
			return this.sceneName;
		}
	}

	// Token: 0x060001A7 RID: 423 RVA: 0x0000930F File Offset: 0x0000750F
	public SceneField(string sceneName)
	{
		this.sceneName = sceneName;
	}

	// Token: 0x060001A8 RID: 424 RVA: 0x00009307 File Offset: 0x00007507
	public static implicit operator string(SceneField sceneField)
	{
		return sceneField.sceneName;
	}

	// Token: 0x060001A9 RID: 425 RVA: 0x00009329 File Offset: 0x00007529
	public static implicit operator bool(SceneField sceneField)
	{
		return !string.IsNullOrEmpty(sceneField.sceneName);
	}

	// Token: 0x040001EB RID: 491
	[SerializeField]
	private UnityEngine.Object sceneAsset;

	// Token: 0x040001EC RID: 492
	[SerializeField]
	private string sceneName = "";
}
