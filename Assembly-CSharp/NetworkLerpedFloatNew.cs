using System;
using UnityEngine;

// Token: 0x02000067 RID: 103
public struct NetworkLerpedFloatNew
{
	// Token: 0x060001B5 RID: 437 RVA: 0x0000937C File Offset: 0x0000757C
	public void SetValueImmediate(float value)
	{
		this.newestInterpPoint.time = Time.time;
		this.newestInterpPoint.value = value;
		this.highInterpPoint = this.newestInterpPoint;
		this.lowInterpPoint = this.newestInterpPoint;
		this.inverseLowHighTimespan = 0f;
	}

	// Token: 0x060001B6 RID: 438 RVA: 0x000093C8 File Offset: 0x000075C8
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

	// Token: 0x060001B7 RID: 439 RVA: 0x00009479 File Offset: 0x00007679
	public float GetAuthoritativeValue()
	{
		return this.newestInterpPoint.value;
	}

	// Token: 0x060001B8 RID: 440 RVA: 0x00009486 File Offset: 0x00007686
	public void PushValue(float value)
	{
		if (this.newestInterpPoint.value != value)
		{
			this.newestInterpPoint.time = Time.time;
			this.newestInterpPoint.value = value;
		}
	}

	// Token: 0x040001C1 RID: 449
	private const float interpDelay = 0.1f;

	// Token: 0x040001C2 RID: 450
	private NetworkLerpedFloatNew.InterpPoint lowInterpPoint;

	// Token: 0x040001C3 RID: 451
	private NetworkLerpedFloatNew.InterpPoint highInterpPoint;

	// Token: 0x040001C4 RID: 452
	private NetworkLerpedFloatNew.InterpPoint newestInterpPoint;

	// Token: 0x040001C5 RID: 453
	private float inverseLowHighTimespan;

	// Token: 0x02000068 RID: 104
	private struct InterpPoint
	{
		// Token: 0x040001C6 RID: 454
		public float time;

		// Token: 0x040001C7 RID: 455
		public float value;
	}
}
