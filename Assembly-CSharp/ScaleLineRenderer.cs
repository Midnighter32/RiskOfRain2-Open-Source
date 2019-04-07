using System;
using UnityEngine;

// Token: 0x02000053 RID: 83
public class ScaleLineRenderer : MonoBehaviour
{
	// Token: 0x0600015F RID: 351 RVA: 0x00008091 File Offset: 0x00006291
	private void Start()
	{
		this.line = base.GetComponent<LineRenderer>();
		this.SetScale();
	}

	// Token: 0x06000160 RID: 352 RVA: 0x00004507 File Offset: 0x00002707
	private void Update()
	{
	}

	// Token: 0x06000161 RID: 353 RVA: 0x000080A8 File Offset: 0x000062A8
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
