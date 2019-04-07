using System;
using UnityEngine;

// Token: 0x02000068 RID: 104
public struct NetworkLerpedVector3
{
	// Token: 0x06000198 RID: 408 RVA: 0x00008D90 File Offset: 0x00006F90
	public void SetValueImmediate(Vector3 value)
	{
		this.newestInterpPoint.time = Time.time;
		this.newestInterpPoint.value = value;
		this.highInterpPoint = this.newestInterpPoint;
		this.lowInterpPoint = this.newestInterpPoint;
		this.inverseLowHighTimespan = 0f;
	}

	// Token: 0x06000199 RID: 409 RVA: 0x00008DDC File Offset: 0x00006FDC
	public Vector3 GetCurrentValue(bool hasAuthority)
	{
		if (hasAuthority)
		{
			return this.newestInterpPoint.value;
		}
		float num = Time.time - this.interpDelay;
		if (num > this.highInterpPoint.time)
		{
			this.lowInterpPoint = this.highInterpPoint;
			this.highInterpPoint = this.newestInterpPoint;
			float num2 = this.highInterpPoint.time - this.lowInterpPoint.time;
			this.inverseLowHighTimespan = ((num2 == 0f) ? 0f : (1f / num2));
		}
		float t = (num - this.lowInterpPoint.time) * this.inverseLowHighTimespan;
		return Vector3.Lerp(this.lowInterpPoint.value, this.highInterpPoint.value, t);
	}

	// Token: 0x0600019A RID: 410 RVA: 0x00008E8E File Offset: 0x0000708E
	public Vector3 GetAuthoritativeValue()
	{
		return this.newestInterpPoint.value;
	}

	// Token: 0x0600019B RID: 411 RVA: 0x00008E9B File Offset: 0x0000709B
	public void PushValue(Vector3 value)
	{
		if (this.newestInterpPoint.value != value)
		{
			this.newestInterpPoint.time = Time.time;
			this.newestInterpPoint.value = value;
		}
	}

	// Token: 0x040001C3 RID: 451
	public float interpDelay;

	// Token: 0x040001C4 RID: 452
	private NetworkLerpedVector3.InterpPoint lowInterpPoint;

	// Token: 0x040001C5 RID: 453
	private NetworkLerpedVector3.InterpPoint highInterpPoint;

	// Token: 0x040001C6 RID: 454
	private NetworkLerpedVector3.InterpPoint newestInterpPoint;

	// Token: 0x040001C7 RID: 455
	private float inverseLowHighTimespan;

	// Token: 0x02000069 RID: 105
	private struct InterpPoint
	{
		// Token: 0x040001C8 RID: 456
		public float time;

		// Token: 0x040001C9 RID: 457
		public Vector3 value;
	}
}
