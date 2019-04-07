using System;
using UnityEngine;

// Token: 0x02000066 RID: 102
public struct NetworkLerpedFloatNew
{
	// Token: 0x06000194 RID: 404 RVA: 0x00008C58 File Offset: 0x00006E58
	public void SetValueImmediate(float value)
	{
		this.newestInterpPoint.time = Time.time;
		this.newestInterpPoint.value = value;
		this.highInterpPoint = this.newestInterpPoint;
		this.lowInterpPoint = this.newestInterpPoint;
		this.inverseLowHighTimespan = 0f;
	}

	// Token: 0x06000195 RID: 405 RVA: 0x00008CA4 File Offset: 0x00006EA4
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
		float t = (num - this.lowInterpPoint.time) * this.inverseLowHighTimespan;
		return Mathf.Lerp(this.lowInterpPoint.value, this.highInterpPoint.value, t);
	}

	// Token: 0x06000196 RID: 406 RVA: 0x00008D55 File Offset: 0x00006F55
	public float GetAuthoritativeValue()
	{
		return this.newestInterpPoint.value;
	}

	// Token: 0x06000197 RID: 407 RVA: 0x00008D62 File Offset: 0x00006F62
	public void PushValue(float value)
	{
		if (this.newestInterpPoint.value != value)
		{
			this.newestInterpPoint.time = Time.time;
			this.newestInterpPoint.value = value;
		}
	}

	// Token: 0x040001BC RID: 444
	private const float interpDelay = 0.1f;

	// Token: 0x040001BD RID: 445
	private NetworkLerpedFloatNew.InterpPoint lowInterpPoint;

	// Token: 0x040001BE RID: 446
	private NetworkLerpedFloatNew.InterpPoint highInterpPoint;

	// Token: 0x040001BF RID: 447
	private NetworkLerpedFloatNew.InterpPoint newestInterpPoint;

	// Token: 0x040001C0 RID: 448
	private float inverseLowHighTimespan;

	// Token: 0x02000067 RID: 103
	private struct InterpPoint
	{
		// Token: 0x040001C1 RID: 449
		public float time;

		// Token: 0x040001C2 RID: 450
		public float value;
	}
}
