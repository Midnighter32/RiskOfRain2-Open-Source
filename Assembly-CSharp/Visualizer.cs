using System;
using UnityEngine;

// Token: 0x02000055 RID: 85
public class Visualizer : MonoBehaviour
{
	// Token: 0x06000158 RID: 344 RVA: 0x00008582 File Offset: 0x00006782
	private void Start()
	{
		this.initialPos = this.particleObject.transform.localPosition;
	}

	// Token: 0x06000159 RID: 345 RVA: 0x0000859A File Offset: 0x0000679A
	private void Update()
	{
		this.particleObject.transform.localPosition = this.initialPos + new Vector3(0f, this.yvalue / this.yscale, 0f);
	}

	// Token: 0x04000190 RID: 400
	public float yscale;

	// Token: 0x04000191 RID: 401
	public GameObject particleObject;

	// Token: 0x04000192 RID: 402
	public float yvalue;

	// Token: 0x04000193 RID: 403
	private Vector3 initialPos;
}
