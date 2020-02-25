using System;
using UnityEngine;

// Token: 0x02000069 RID: 105
public struct NetworkLerpedVector3
{
	// Token: 0x060001B9 RID: 441 RVA: 0x000094B4 File Offset: 0x000076B4
	public void SetValueImmediate(Vector3 value)
	{
		this.newestInterpPoint.time = Time.time;
		this.newestInterpPoint.value = value;
		this.highInterpPoint = this.newestInterpPoint;
		this.lowInterpPoint = this.newestInterpPoint;
		this.inverseLowHighTimespan = 0f;
	}

	// Token: 0x060001BA RID: 442 RVA: 0x00009500 File Offset: 0x00007700
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

	// Token: 0x060001BB RID: 443 RVA: 0x000095B2 File Offset: 0x000077B2
	public Vector3 GetAuthoritativeValue()
	{
		return this.newestInterpPoint.value;
	}

	// Token: 0x060001BC RID: 444 RVA: 0x000095BF File Offset: 0x000077BF
	public void PushValue(Vector3 value)
	{
		if (this.newestInterpPoint.value != value)
		{
			this.newestInterpPoint.time = Time.time;
			this.newestInterpPoint.value = value;
		}
	}

	// Token: 0x040001C8 RID: 456
	public float interpDelay;

	// Token: 0x040001C9 RID: 457
	private NetworkLerpedVector3.InterpPoint lowInterpPoint;

	// Token: 0x040001CA RID: 458
	private NetworkLerpedVector3.InterpPoint highInterpPoint;

	// Token: 0x040001CB RID: 459
	private NetworkLerpedVector3.InterpPoint newestInterpPoint;

	// Token: 0x040001CC RID: 460
	private float inverseLowHighTimespan;

	// Token: 0x0200006A RID: 106
	private struct InterpPoint
	{
		// Token: 0x040001CD RID: 461
		public float time;

		// Token: 0x040001CE RID: 462
		public Vector3 value;
	}
}
