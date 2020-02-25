using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x020001DF RID: 479
	public class DetachTrailOnDestroy : MonoBehaviour
	{
		// Token: 0x06000A1C RID: 2588 RVA: 0x0002C264 File Offset: 0x0002A464
		private void OnDestroy()
		{
			for (int i = 0; i < this.targetTrailRenderers.Length; i++)
			{
				TrailRenderer trailRenderer = this.targetTrailRenderers[i];
				if (trailRenderer)
				{
					trailRenderer.transform.SetParent(null);
					trailRenderer.autodestruct = true;
				}
			}
			this.targetTrailRenderers = null;
		}

		// Token: 0x04000A6D RID: 2669
		[FormerlySerializedAs("trail")]
		public TrailRenderer[] targetTrailRenderers;
	}
}
