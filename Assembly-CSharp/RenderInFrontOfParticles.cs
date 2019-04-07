using System;
using UnityEngine;

// Token: 0x0200004E RID: 78
public class RenderInFrontOfParticles : MonoBehaviour
{
	// Token: 0x0600014E RID: 334 RVA: 0x00007E1C File Offset: 0x0000601C
	private void Start()
	{
		this.rend = base.GetComponent<Renderer>();
		this.rend.material.renderQueue = this.renderOrder;
	}

	// Token: 0x0600014F RID: 335 RVA: 0x00004507 File Offset: 0x00002707
	private void Update()
	{
	}

	// Token: 0x04000165 RID: 357
	public int renderOrder;

	// Token: 0x04000166 RID: 358
	private Renderer rend;
}
