using System;
using UnityEngine;

// Token: 0x02000063 RID: 99
public struct NetworkLerpedAngle
{
	// Token: 0x0600018E RID: 398 RVA: 0x00008AB8 File Offset: 0x00006CB8
	public float GetCurrentValue(bool hasAuthority)
	{
		if (hasAuthority)
		{
			return this.authoritativeValue;
		}
		float t = Mathf.Clamp01((Time.time - this.interpStartTime) * 20f);
		return Mathf.LerpAngle(this.interpStartValue, this.authoritativeValue, t);
	}

	// Token: 0x0600018F RID: 399 RVA: 0x00008AF9 File Offset: 0x00006CF9
	public void SetAuthoritativeValue(float newValue, bool hasAuthority)
	{
		this.interpStartValue = this.GetCurrentValue(hasAuthority);
		this.interpStartTime = Time.time;
		this.authoritativeValue = newValue;
	}

	// Token: 0x040001B0 RID: 432
	private const float interpDuration = 0.05f;

	// Token: 0x040001B1 RID: 433
	private const float invInterpDuration = 20f;

	// Token: 0x040001B2 RID: 434
	public float authoritativeValue;

	// Token: 0x040001B3 RID: 435
	private float interpStartValue;

	// Token: 0x040001B4 RID: 436
	private float interpStartTime;
}
