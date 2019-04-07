using System;
using UnityEngine;

// Token: 0x02000048 RID: 72
public class ObjectScaleCurve : MonoBehaviour
{
	// Token: 0x06000139 RID: 313 RVA: 0x00007775 File Offset: 0x00005975
	private void Awake()
	{
		if (!this.hasCachedMaxSize)
		{
			this.maxSize = base.transform.localScale;
			this.hasCachedMaxSize = true;
		}
		this.Reset();
	}

	// Token: 0x0600013A RID: 314 RVA: 0x0000779D File Offset: 0x0000599D
	private void OnEnable()
	{
		this.Reset();
	}

	// Token: 0x0600013B RID: 315 RVA: 0x000077A5 File Offset: 0x000059A5
	public void Reset()
	{
		this.time = 0f;
		this.updateScale(0f);
	}

	// Token: 0x0600013C RID: 316 RVA: 0x000077BD File Offset: 0x000059BD
	private void Update()
	{
		this.time += Time.deltaTime;
		this.updateScale(this.time);
	}

	// Token: 0x0600013D RID: 317 RVA: 0x000077E0 File Offset: 0x000059E0
	private void updateScale(float time)
	{
		float d = 1f;
		if (this.overallCurve != null)
		{
			d = this.overallCurve.Evaluate(time / this.timeMax);
		}
		Vector3 a = new Vector3(this.curveX.Evaluate(time / this.timeMax) * this.maxSize.x, this.curveY.Evaluate(time / this.timeMax) * this.maxSize.y, this.curveZ.Evaluate(time / this.timeMax) * this.maxSize.z);
		base.transform.localScale = a * d;
	}

	// Token: 0x04000146 RID: 326
	public AnimationCurve curveX;

	// Token: 0x04000147 RID: 327
	public AnimationCurve curveY;

	// Token: 0x04000148 RID: 328
	public AnimationCurve curveZ;

	// Token: 0x04000149 RID: 329
	public AnimationCurve overallCurve;

	// Token: 0x0400014A RID: 330
	public float timeMax = 5f;

	// Token: 0x0400014B RID: 331
	[HideInInspector]
	public float time;

	// Token: 0x0400014C RID: 332
	private Vector3 maxSize;

	// Token: 0x0400014D RID: 333
	private bool hasCachedMaxSize;
}
