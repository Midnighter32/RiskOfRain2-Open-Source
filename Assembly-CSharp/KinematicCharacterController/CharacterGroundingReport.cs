using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x020006C8 RID: 1736
	public struct CharacterGroundingReport
	{
		// Token: 0x06002691 RID: 9873 RVA: 0x000B18EC File Offset: 0x000AFAEC
		public void CopyFrom(CharacterTransientGroundingReport transientGroundingReport)
		{
			this.FoundAnyGround = transientGroundingReport.FoundAnyGround;
			this.IsStableOnGround = transientGroundingReport.IsStableOnGround;
			this.SnappingPrevented = transientGroundingReport.SnappingPrevented;
			this.GroundNormal = transientGroundingReport.GroundNormal;
			this.InnerGroundNormal = transientGroundingReport.InnerGroundNormal;
			this.OuterGroundNormal = transientGroundingReport.OuterGroundNormal;
			this.GroundCollider = null;
			this.GroundPoint = Vector3.zero;
		}

		// Token: 0x040028C2 RID: 10434
		public bool FoundAnyGround;

		// Token: 0x040028C3 RID: 10435
		public bool IsStableOnGround;

		// Token: 0x040028C4 RID: 10436
		public bool SnappingPrevented;

		// Token: 0x040028C5 RID: 10437
		public Vector3 GroundNormal;

		// Token: 0x040028C6 RID: 10438
		public Vector3 InnerGroundNormal;

		// Token: 0x040028C7 RID: 10439
		public Vector3 OuterGroundNormal;

		// Token: 0x040028C8 RID: 10440
		public Collider GroundCollider;

		// Token: 0x040028C9 RID: 10441
		public Vector3 GroundPoint;
	}
}
