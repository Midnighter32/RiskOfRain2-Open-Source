using System;
using UnityEngine;

// Token: 0x0200004D RID: 77
[ExecuteInEditMode]
public class RenderDepth : MonoBehaviour
{
	// Token: 0x0600014C RID: 332 RVA: 0x00007E0E File Offset: 0x0000600E
	private void OnEnable()
	{
		base.GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
	}
}
