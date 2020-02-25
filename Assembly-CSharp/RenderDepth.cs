using System;
using UnityEngine;

// Token: 0x02000049 RID: 73
[ExecuteInEditMode]
public class RenderDepth : MonoBehaviour
{
	// Token: 0x06000131 RID: 305 RVA: 0x00007D22 File Offset: 0x00005F22
	private void OnEnable()
	{
		base.GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
	}
}
