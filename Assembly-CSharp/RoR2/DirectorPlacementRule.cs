using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001E4 RID: 484
	public class DirectorPlacementRule
	{
		// Token: 0x17000148 RID: 328
		// (get) Token: 0x06000A2C RID: 2604 RVA: 0x0002C7E0 File Offset: 0x0002A9E0
		public Vector3 targetPosition
		{
			get
			{
				if (!this.spawnOnTarget)
				{
					return this.position;
				}
				return this.spawnOnTarget.position;
			}
		}

		// Token: 0x04000A7D RID: 2685
		public Transform spawnOnTarget;

		// Token: 0x04000A7E RID: 2686
		public Vector3 position;

		// Token: 0x04000A7F RID: 2687
		public DirectorPlacementRule.PlacementMode placementMode;

		// Token: 0x04000A80 RID: 2688
		public float minDistance;

		// Token: 0x04000A81 RID: 2689
		public float maxDistance;

		// Token: 0x04000A82 RID: 2690
		public bool preventOverhead;

		// Token: 0x020001E5 RID: 485
		public enum PlacementMode
		{
			// Token: 0x04000A84 RID: 2692
			Direct,
			// Token: 0x04000A85 RID: 2693
			Approximate,
			// Token: 0x04000A86 RID: 2694
			ApproximateSimple,
			// Token: 0x04000A87 RID: 2695
			NearestNode,
			// Token: 0x04000A88 RID: 2696
			Random
		}
	}
}
