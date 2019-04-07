using System;
using UnityEngine;

// Token: 0x02000064 RID: 100
public struct NetworkLerpedAngleNew
{
	// Token: 0x06000190 RID: 400 RVA: 0x00008B1C File Offset: 0x00006D1C
	public void SetValueImmediate(float value)
	{
		this.newestInterpPoint.time = Time.time;
		this.newestInterpPoint.value = value;
		this.highInterpPoint = this.newestInterpPoint;
		this.lowInterpPoint = this.newestInterpPoint;
		this.inverseLowHighTimespan = 0f;
	}

	// Token: 0x06000191 RID: 401 RVA: 0x00008B68 File Offset: 0x00006D68
	public float GetCurrentValue(bool hasAuthority)
	{
		if (hasAuthority)
		{
			return this.newestInterpPoint.value;
		}
		float num = Time.time - 0.1f;
		if (num > this.highInterpPoint.time)
		{
			this.lowInterpPoint = this.highInterpPoint;
			this.highInterpPoint = this.newestInterpPoint;
			float num2 = this.highInterpPoint.time - this.lowInterpPoint.time;
			this.inverseLowHighTimespan = ((num2 == 0f) ? 0f : (1f / num2));
		}
		float t = Mathf.Clamp01((num - this.lowInterpPoint.time) * this.inverseLowHighTimespan);
		return Mathf.LerpAngle(this.lowInterpPoint.value, this.highInterpPoint.value, t);
	}

	// Token: 0x06000192 RID: 402 RVA: 0x00008C1E File Offset: 0x00006E1E
	public float GetAuthoritativeValue()
	{
		return this.newestInterpPoint.value;
	}

	// Token: 0x06000193 RID: 403 RVA: 0x00008C2B File Offset: 0x00006E2B
	public void PushValue(float value)
	{
		if (this.newestInterpPoint.value != value)
		{
			this.newestInterpPoint.time = Time.time;
			this.newestInterpPoint.value = value;
		}
	}

	// Token: 0x040001B5 RID: 437
	private const float interpDelay = 0.1f;

	// Token: 0x040001B6 RID: 438
	private NetworkLerpedAngleNew.InterpPoint lowInterpPoint;

	// Token: 0x040001B7 RID: 439
	private NetworkLerpedAngleNew.InterpPoint highInterpPoint;

	// Token: 0x040001B8 RID: 440
	private NetworkLerpedAngleNew.InterpPoint newestInterpPoint;

	// Token: 0x040001B9 RID: 441
	private float inverseLowHighTimespan;

	// Token: 0x02000065 RID: 101
	private struct InterpPoint
	{
		// Token: 0x040001BA RID: 442
		public float time;

		// Token: 0x040001BB RID: 443
		public float value;
	}
}
