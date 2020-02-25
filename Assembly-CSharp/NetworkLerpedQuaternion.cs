using System;
using UnityEngine;

// Token: 0x0200006B RID: 107
public struct NetworkLerpedQuaternion
{
	// Token: 0x060001BD RID: 445 RVA: 0x000095F0 File Offset: 0x000077F0
	public void SetValueImmediate(Quaternion value)
	{
		this.newestInterpPoint.time = Time.time;
		this.newestInterpPoint.value = value;
		this.highInterpPoint = this.newestInterpPoint;
		this.lowInterpPoint = this.newestInterpPoint;
		this.inverseLowHighTimespan = 0f;
	}

	// Token: 0x060001BE RID: 446 RVA: 0x0000963C File Offset: 0x0000783C
	public Quaternion GetCurrentValue(bool hasAuthority)
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
		return Quaternion.Slerp(this.lowInterpPoint.value, this.highInterpPoint.value, t);
	}

	// Token: 0x060001BF RID: 447 RVA: 0x000096ED File Offset: 0x000078ED
	public Quaternion GetAuthoritativeValue()
	{
		return this.newestInterpPoint.value;
	}

	// Token: 0x060001C0 RID: 448 RVA: 0x000096FA File Offset: 0x000078FA
	public void PushValue(Quaternion value)
	{
		if (this.newestInterpPoint.value != value)
		{
			this.newestInterpPoint.time = Time.time;
			this.newestInterpPoint.value = value;
		}
	}

	// Token: 0x040001CF RID: 463
	private const float interpDelay = 0.1f;

	// Token: 0x040001D0 RID: 464
	private NetworkLerpedQuaternion.InterpPoint lowInterpPoint;

	// Token: 0x040001D1 RID: 465
	private NetworkLerpedQuaternion.InterpPoint highInterpPoint;

	// Token: 0x040001D2 RID: 466
	private NetworkLerpedQuaternion.InterpPoint newestInterpPoint;

	// Token: 0x040001D3 RID: 467
	private float inverseLowHighTimespan;

	// Token: 0x0200006C RID: 108
	private struct InterpPoint
	{
		// Token: 0x040001D4 RID: 468
		public float time;

		// Token: 0x040001D5 RID: 469
		public Quaternion value;
	}
}
