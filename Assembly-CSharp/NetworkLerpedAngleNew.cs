using System;
using UnityEngine;

// Token: 0x02000065 RID: 101
public struct NetworkLerpedAngleNew
{
	// Token: 0x060001B1 RID: 433 RVA: 0x00009240 File Offset: 0x00007440
	public void SetValueImmediate(float value)
	{
		this.newestInterpPoint.time = Time.time;
		this.newestInterpPoint.value = value;
		this.highInterpPoint = this.newestInterpPoint;
		this.lowInterpPoint = this.newestInterpPoint;
		this.inverseLowHighTimespan = 0f;
	}

	// Token: 0x060001B2 RID: 434 RVA: 0x0000928C File Offset: 0x0000748C
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

	// Token: 0x060001B3 RID: 435 RVA: 0x00009342 File Offset: 0x00007542
	public float GetAuthoritativeValue()
	{
		return this.newestInterpPoint.value;
	}

	// Token: 0x060001B4 RID: 436 RVA: 0x0000934F File Offset: 0x0000754F
	public void PushValue(float value)
	{
		if (this.newestInterpPoint.value != value)
		{
			this.newestInterpPoint.time = Time.time;
			this.newestInterpPoint.value = value;
		}
	}

	// Token: 0x040001BA RID: 442
	private const float interpDelay = 0.1f;

	// Token: 0x040001BB RID: 443
	private NetworkLerpedAngleNew.InterpPoint lowInterpPoint;

	// Token: 0x040001BC RID: 444
	private NetworkLerpedAngleNew.InterpPoint highInterpPoint;

	// Token: 0x040001BD RID: 445
	private NetworkLerpedAngleNew.InterpPoint newestInterpPoint;

	// Token: 0x040001BE RID: 446
	private float inverseLowHighTimespan;

	// Token: 0x02000066 RID: 102
	private struct InterpPoint
	{
		// Token: 0x040001BF RID: 447
		public float time;

		// Token: 0x040001C0 RID: 448
		public float value;
	}
}
