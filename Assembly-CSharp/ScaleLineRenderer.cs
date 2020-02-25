using System;
using UnityEngine;

// Token: 0x0200004F RID: 79
public class ScaleLineRenderer : MonoBehaviour
{
	// Token: 0x06000144 RID: 324 RVA: 0x00007FA5 File Offset: 0x000061A5
	private void Start()
	{
		this.line = base.GetComponent<LineRenderer>();
		this.SetScale();
	}

	// Token: 0x06000145 RID: 325 RVA: 0x0000409B File Offset: 0x0000229B
	private void Update()
	{
	}

	// Token: 0x06000146 RID: 326 RVA: 0x00007FBC File Offset: 0x000061BC
	private void SetScale()
	{
		this.line.SetPosition(0, this.positions[0]);
		this.line.SetPosition(1, this.positions[1]);
		this.line.material.SetTextureScale("_MainTex", new Vector2(Vector3.Distance(this.positions[0], this.positions[1]) * this.scaleSize, 1f));
	}

	// Token: 0x04000174 RID: 372
	private LineRenderer line;

	// Token: 0x04000175 RID: 373
	public float scaleSize = 1f;

	// Token: 0x04000176 RID: 374
	public Vector3[] positions;
}
