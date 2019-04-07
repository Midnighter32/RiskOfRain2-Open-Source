using System;
using UnityEngine;

// Token: 0x02000045 RID: 69
public class LightIntensityCurve : MonoBehaviour
{
	// Token: 0x06000130 RID: 304 RVA: 0x00007634 File Offset: 0x00005834
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

	// Token: 0x06000131 RID: 305 RVA: 0x0000768C File Offset: 0x0000588C
	private void Update()
	{
		this.time += Time.deltaTime;
		this.light.intensity = this.curve.Evaluate(this.time / this.timeMax) * this.maxIntensity;
		if (this.time >= this.timeMax && this.loop)
		{
			this.time = 0f;
		}
	}

	// Token: 0x0400013B RID: 315
	public AnimationCurve curve;

	// Token: 0x0400013C RID: 316
	public float timeMax = 5f;

	// Token: 0x0400013D RID: 317
	private float time;

	// Token: 0x0400013E RID: 318
	private Light light;

	// Token: 0x0400013F RID: 319
	private float maxIntensity;

	// Token: 0x04000140 RID: 320
	public bool loop;

	// Token: 0x04000141 RID: 321
	public bool randomStart;
}
