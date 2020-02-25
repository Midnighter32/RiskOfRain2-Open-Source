using System;
using UnityEngine;

// Token: 0x02000041 RID: 65
public class LightIntensityCurve : MonoBehaviour
{
	// Token: 0x06000114 RID: 276 RVA: 0x0000758C File Offset: 0x0000578C
	private void Start()
	{
		this.light = base.GetComponent<Light>();
		this.maxIntensity = this.light.intensity;
		this.light.intensity = 0f;
		if (this.randomStart)
		{
			this.time = UnityEngine.Random.Range(0f, this.timeMax);
		}
	}

	// Token: 0x06000115 RID: 277 RVA: 0x000075E4 File Offset: 0x000057E4
	private void Update()
	{
		this.time += Time.deltaTime;
		this.light.intensity = this.curve.Evaluate(this.time / this.timeMax) * this.maxIntensity;
		if (this.time >= this.timeMax && this.loop)
		{
			this.time = 0f;
		}
	}

	// Token: 0x04000140 RID: 320
	public AnimationCurve curve;

	// Token: 0x04000141 RID: 321
	public float timeMax = 5f;

	// Token: 0x04000142 RID: 322
	private float time;

	// Token: 0x04000143 RID: 323
	private Light light;

	// Token: 0x04000144 RID: 324
	private float maxIntensity;

	// Token: 0x04000145 RID: 325
	public bool loop;

	// Token: 0x04000146 RID: 326
	public bool randomStart;
}
