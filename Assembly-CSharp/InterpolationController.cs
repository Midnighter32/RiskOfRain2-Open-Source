using System;
using UnityEngine;

// Token: 0x02000040 RID: 64
public class InterpolationController : MonoBehaviour
{
	// Token: 0x1700001C RID: 28
	// (get) Token: 0x0600010E RID: 270 RVA: 0x000074F7 File Offset: 0x000056F7
	public static float InterpolationFactor
	{
		get
		{
			return InterpolationController.m_interpolationFactor;
		}
	}

	// Token: 0x0600010F RID: 271 RVA: 0x000074FE File Offset: 0x000056FE
	public void Start()
	{
		this.m_lastFixedUpdateTimes = new float[2];
		this.m_newTimeIndex = 0;
	}

	// Token: 0x06000110 RID: 272 RVA: 0x00007513 File Offset: 0x00005713
	public void FixedUpdate()
	{
		this.m_newTimeIndex = this.OldTimeIndex();
		this.m_lastFixedUpdateTimes[this.m_newTimeIndex] = Time.fixedTime;
	}

	// Token: 0x06000111 RID: 273 RVA: 0x00007534 File Offset: 0x00005734
	public void Update()
	{
		float num = this.m_lastFixedUpdateTimes[this.m_newTimeIndex];
		float num2 = this.m_lastFixedUpdateTimes[this.OldTimeIndex()];
		if (num != num2)
		{
			InterpolationController.m_interpolationFactor = (Time.time - num) / (num - num2);
			return;
		}
		InterpolationController.m_interpolationFactor = 1f;
	}

	// Token: 0x06000112 RID: 274 RVA: 0x0000757C File Offset: 0x0000577C
	private int OldTimeIndex()
	{
		if (this.m_newTimeIndex != 0)
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x0400013D RID: 317
	private float[] m_lastFixedUpdateTimes;

	// Token: 0x0400013E RID: 318
	private int m_newTimeIndex;

	// Token: 0x0400013F RID: 319
	private static float m_interpolationFactor;
}
