using System;
using UnityEngine;

// Token: 0x0200005F RID: 95
public class Wind : MonoBehaviour
{
	// Token: 0x0600017C RID: 380 RVA: 0x00008793 File Offset: 0x00006993
	private void Start()
	{
		this.rend = base.GetComponent<Renderer>();
		this.props = new MaterialPropertyBlock();
		this.SetWind(this.props, this.windVector);
	}

	// Token: 0x0600017D RID: 381 RVA: 0x000087C0 File Offset: 0x000069C0
	private void Update()
	{
		this.time += Time.deltaTime;
		this.windVector.x = (0.5f + 0.5f * Mathf.Sin(this.MainWindSpeed * this.time * 0.017453292f)) * this.MainWindAmplitude;
		this.SetWind(this.props, this.windVector);
	}

	// Token: 0x0600017E RID: 382 RVA: 0x00008827 File Offset: 0x00006A27
	private void SetWind(MaterialPropertyBlock block, Vector4 input)
	{
		this.rend.GetPropertyBlock(block);
		block.Clear();
		block.SetVector("_Wind", input);
		this.rend.SetPropertyBlock(block);
	}

	// Token: 0x040001A5 RID: 421
	private Renderer rend;

	// Token: 0x040001A6 RID: 422
	private MaterialPropertyBlock props;

	// Token: 0x040001A7 RID: 423
	public Vector4 windVector;

	// Token: 0x040001A8 RID: 424
	public float MainWindAmplitude = 1f;

	// Token: 0x040001A9 RID: 425
	public float MainWindSpeed = 3f;

	// Token: 0x040001AA RID: 426
	private float time;
}
