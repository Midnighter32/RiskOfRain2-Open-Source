using System;
using UnityEngine;

// Token: 0x0200003F RID: 63
public class InterpolatedTransformUpdater : MonoBehaviour
{
	// Token: 0x0600010B RID: 267 RVA: 0x000074DC File Offset: 0x000056DC
	private void Awake()
	{
		this.m_interpolatedTransform = base.GetComponent<InterpolatedTransform>();
	}

	// Token: 0x0600010C RID: 268 RVA: 0x000074EA File Offset: 0x000056EA
	private void FixedUpdate()
	{
		this.m_interpolatedTransform.LateFixedUpdate();
	}

	// Token: 0x0400013C RID: 316
	private InterpolatedTransform m_interpolatedTransform;
}
