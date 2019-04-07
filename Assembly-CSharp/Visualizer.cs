using System;
using UnityEngine;

// Token: 0x0200005A RID: 90
public class Visualizer : MonoBehaviour
{
	// Token: 0x06000175 RID: 373 RVA: 0x00008696 File Offset: 0x00006896
	private void Start()
	{
		this.initialPos = this.particleObject.transform.localPosition;
	}

	// Token: 0x06000176 RID: 374 RVA: 0x000086AE File Offset: 0x000068AE
	private void Update()
	{
		this.particleObject.transform.localPosition = this.initialPos + new Vector3(0f, this.yvalue / this.yscale, 0f);
	}

	// Token: 0x04000191 RID: 401
	public float yscale;

	// Token: 0x04000192 RID: 402
	public GameObject particleObject;

	// Token: 0x04000193 RID: 403
	public float yvalue;

	// Token: 0x04000194 RID: 404
	private Vector3 initialPos;
}
