using System;
using UnityEngine;

// Token: 0x02000004 RID: 4
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class CameraDepthMode : MonoBehaviour
{
	// Token: 0x06000003 RID: 3 RVA: 0x00002058 File Offset: 0x00000258
	private void Start()
	{
		base.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
	}

	// Token: 0x06000004 RID: 4 RVA: 0x00002058 File Offset: 0x00000258
	private void OnEnable()
	{
		base.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
	}

	// Token: 0x06000005 RID: 5 RVA: 0x00002066 File Offset: 0x00000266
	private void OnDisable()
	{
		base.GetComponent<Camera>().depthTextureMode = DepthTextureMode.None;
	}
}
