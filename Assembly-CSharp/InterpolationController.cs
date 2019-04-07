using System;
using UnityEngine;

// Token: 0x02000044 RID: 68
public class InterpolationController : MonoBehaviour
{
	// Token: 0x17000018 RID: 24
	// (get) Token: 0x0600012A RID: 298 RVA: 0x0000759F File Offset: 0x0000579F
	public static float InterpolationFactor
	{
		get
		{
			return InterpolationController.m_interpolationFactor;
		}
	}

	// Token: 0x0600012B RID: 299 RVA: 0x000075A6 File Offset: 0x000057A6
	public void Start()
	{
		this.m_lastFixedUpdateTimes = new float[2];
		this.m_newTimeIndex = 0;
	}

	// Token: 0x0600012C RID: 300 RVA: 0x000075BB File Offset: 0x000057BB
	public void FixedUpdate()
	{
		this.m_newTimeIndex = this.OldTimeIndex();
		this.m_lastFixedUpdateTimes[this.m_newTimeIndex] = Time.fixedTime;
	}

	// Token: 0x0600012D RID: 301 RVA: 0x000075DC File Offset: 0x000057DC
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

	// Token: 0x0600012E RID: 302 RVA: 0x00007624 File Offset: 0x00005824
	private int OldTimeIndex()
	{
		if (this.m_newTimeIndex != 0)
		{
			return 0;
		}
		return 1;
	}

	// Token: 0x04000138 RID: 312
	private float[] m_lastFixedUpdateTimes;

	// Token: 0x04000139 RID: 313
	private int m_newTimeIndex;

	// Token: 0x0400013A RID: 314
	private static float m_interpolationFactor;
}
