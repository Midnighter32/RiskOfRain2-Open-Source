using System;
using UnityEngine;

// Token: 0x0200006A RID: 106
public struct NetworkLerpedQuaternion
{
	// Token: 0x0600019C RID: 412 RVA: 0x00008ECC File Offset: 0x000070CC
	public void SetValueImmediate(Quaternion value)
	{
		this.newestInterpPoint.time = Time.time;
		this.newestInterpPoint.value = value;
		this.highInterpPoint = this.newestInterpPoint;
		this.lowInterpPoint = this.newestInterpPoint;
		this.inverseLowHighTimespan = 0f;
	}

	// Token: 0x0600019D RID: 413 RVA: 0x00008F18 File Offset: 0x00007118
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

	// Token: 0x0600019E RID: 414 RVA: 0x00008FC9 File Offset: 0x000071C9
	public Quaternion GetAuthoritativeValue()
	{
		return this.newestInterpPoint.value;
	}

	// Token: 0x0600019F RID: 415 RVA: 0x00008FD6 File Offset: 0x000071D6
	public void PushValue(Quaternion value)
	{
		if (this.newestInterpPoint.value != value)
		{
			this.newestInterpPoint.time = Time.time;
			this.newestInterpPoint.value = value;
		}
	}

	// Token: 0x040001CA RID: 458
	private const float interpDelay = 0.1f;

	// Token: 0x040001CB RID: 459
	private NetworkLerpedQuaternion.InterpPoint lowInterpPoint;

	// Token: 0x040001CC RID: 460
	private NetworkLerpedQuaternion.InterpPoint highInterpPoint;

	// Token: 0x040001CD RID: 461
	private NetworkLerpedQuaternion.InterpPoint newestInterpPoint;

	// Token: 0x040001CE RID: 462
	private float inverseLowHighTimespan;

	// Token: 0x0200006B RID: 107
	private struct InterpPoint
	{
		// Token: 0x040001CF RID: 463
		public float time;

		// Token: 0x040001D0 RID: 464
		public Quaternion value;
	}
}
