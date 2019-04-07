using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000075 RID: 117
[Serializable]
public struct Wave
{
	// Token: 0x1700001C RID: 28
	// (get) Token: 0x060001CB RID: 459 RVA: 0x00009746 File Offset: 0x00007946
	// (set) Token: 0x060001CC RID: 460 RVA: 0x00009754 File Offset: 0x00007954
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

	// Token: 0x060001CD RID: 461 RVA: 0x00009763 File Offset: 0x00007963
	public float Evaluate(float t)
	{
		return Mathf.Sin(6.2831855f * (this.frequency * t + this.cycleOffset)) * this.amplitude;
	}

	// Token: 0x040001F3 RID: 499
	public float amplitude;

	// Token: 0x040001F4 RID: 500
	public float frequency;

	// Token: 0x040001F5 RID: 501
	[FormerlySerializedAs("phase")]
	public float cycleOffset;
}
