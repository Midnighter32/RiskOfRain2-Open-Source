using System;
using UnityEngine;

// Token: 0x0200004A RID: 74
public class RenderInFrontOfParticles : MonoBehaviour
{
	// Token: 0x06000133 RID: 307 RVA: 0x00007D30 File Offset: 0x00005F30
	private void Start()
	{
		this.rend = base.GetComponent<Renderer>();
		this.rend.material.renderQueue = this.renderOrder;
	}

	// Token: 0x06000134 RID: 308 RVA: 0x0000409B File Offset: 0x0000229B
	private void Update()
	{
	}

	// Token: 0x04000165 RID: 357
	public int renderOrder;

	// Token: 0x04000166 RID: 358
	private Renderer rend;
}
