using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200007A RID: 122
[Serializable]
public struct Wave
{
	// Token: 0x17000022 RID: 34
	// (get) Token: 0x06000204 RID: 516 RVA: 0x0000A1A8 File Offset: 0x000083A8
	// (set) Token: 0x06000205 RID: 517 RVA: 0x0000A1B6 File Offset: 0x000083B6
	public float period
	{
		get
		{
			return 1f / this.frequency;
		}
		set
		{
			this.frequency = 1f / value;
		}
	}

	// Token: 0x06000206 RID: 518 RVA: 0x0000A1C5 File Offset: 0x000083C5
	public float Evaluate(float t)
	{
		return Mathf.Sin(6.2831855f * (this.frequency * t + this.cycleOffset)) * this.amplitude;
	}

	// Token: 0x040001FE RID: 510
	public float amplitude;

	// Token: 0x040001FF RID: 511
	public float frequency;

	// Token: 0x04000200 RID: 512
	[FormerlySerializedAs("phase")]
	public float cycleOffset;
}
