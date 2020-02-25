using System;
using UnityEngine;

// Token: 0x0200005A RID: 90
public class Wind : MonoBehaviour
{
	// Token: 0x0600015F RID: 351 RVA: 0x0000867F File Offset: 0x0000687F
	private void Start()
	{
		this.rend = base.GetComponent<Renderer>();
		this.props = new MaterialPropertyBlock();
		this.SetWind(this.props, this.windVector);
	}

	// Token: 0x06000160 RID: 352 RVA: 0x000086AC File Offset: 0x000068AC
	private void Update()
	{
		this.time += Time.deltaTime;
		this.windVector.x = (0.5f + 0.5f * Mathf.Sin(this.MainWindSpeed * this.time * 0.017453292f)) * this.MainWindAmplitude;
		this.SetWind(this.props, this.windVector);
	}

	// Token: 0x06000161 RID: 353 RVA: 0x00008713 File Offset: 0x00006913
	private void SetWind(MaterialPropertyBlock block, Vector4 input)
	{
		this.rend.GetPropertyBlock(block);
		block.Clear();
		block.SetVector("_Wind", input);
		this.rend.SetPropertyBlock(block);
	}

	// Token: 0x040001A4 RID: 420
	private Renderer rend;

	// Token: 0x040001A5 RID: 421
	private MaterialPropertyBlock props;

	// Token: 0x040001A6 RID: 422
	public Vector4 windVector;

	// Token: 0x040001A7 RID: 423
	public float MainWindAmplitude = 1f;

	// Token: 0x040001A8 RID: 424
	public float MainWindSpeed = 3f;

	// Token: 0x040001A9 RID: 425
	private float time;
}
