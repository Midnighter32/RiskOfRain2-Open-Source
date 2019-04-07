using System;
using UnityEngine;

// Token: 0x02000043 RID: 67
public class InterpolatedTransformUpdater : MonoBehaviour
{
	// Token: 0x06000127 RID: 295 RVA: 0x00007584 File Offset: 0x00005784
	private void Awake()
	{
		this.m_interpolatedTransform = base.GetComponent<InterpolatedTransform>();
	}

	// Token: 0x06000128 RID: 296 RVA: 0x00007592 File Offset: 0x00005792
	private void FixedUpdate()
	{
		this.m_interpolatedTransform.LateFixedUpdate();
	}

	// Token: 0x04000137 RID: 311
	private InterpolatedTransform m_interpolatedTransform;
}
