using System;
using UnityEngine;

// Token: 0x02000064 RID: 100
public struct NetworkLerpedAngle
{
	// Token: 0x060001AF RID: 431 RVA: 0x000091DC File Offset: 0x000073DC
	public float GetCurrentValue(bool hasAuthority)
	{
		if (hasAuthority)
		{
			return this.authoritativeValue;
		}
		float t = Mathf.Clamp01((Time.time - this.interpStartTime) * 20f);
		return Mathf.LerpAngle(this.interpStartValue, this.authoritativeValue, t);
	}

	// Token: 0x060001B0 RID: 432 RVA: 0x0000921D File Offset: 0x0000741D
	public void SetAuthoritativeValue(float newValue, bool hasAuthority)
	{
		this.interpStartValue = this.GetCurrentValue(hasAuthority);
		this.interpStartTime = Time.time;
		this.authoritativeValue = newValue;
	}

	// Token: 0x040001B5 RID: 437
	private const float interpDuration = 0.05f;

	// Token: 0x040001B6 RID: 438
	private const float invInterpDuration = 20f;

	// Token: 0x040001B7 RID: 439
	public float authoritativeValue;

	// Token: 0x040001B8 RID: 440
	private float interpStartValue;

	// Token: 0x040001B9 RID: 441
	private float interpStartTime;
}
