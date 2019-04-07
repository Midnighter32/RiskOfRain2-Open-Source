using System;
using UnityEngine;

// Token: 0x02000034 RID: 52
public class DetachTrailOnDestroy : MonoBehaviour
{
	// Token: 0x060000ED RID: 237 RVA: 0x00005A30 File Offset: 0x00003C30
	private void OnDestroy()
	{
		foreach (TrailRenderer trailRenderer in this.trail)
		{
			trailRenderer.transform.parent = null;
			trailRenderer.autodestruct = true;
		}
		this.trail = null;
	}

	// Token: 0x040000D8 RID: 216
	public TrailRenderer[] trail;
}
