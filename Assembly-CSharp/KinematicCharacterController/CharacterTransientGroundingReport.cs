using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x02000914 RID: 2324
	public struct CharacterTransientGroundingReport
	{
		// Token: 0x060033DC RID: 13276 RVA: 0x000E0D9C File Offset: 0x000DEF9C
		public void CopyFrom(CharacterGroundingReport groundingReport)
		{
			this.FoundAnyGround = groundingReport.FoundAnyGround;
			this.IsStableOnGround = groundingReport.IsStableOnGround;
			this.SnappingPrevented = groundingReport.SnappingPrevented;
			this.GroundNormal = groundingReport.GroundNormal;
			this.InnerGroundNormal = groundingReport.InnerGroundNormal;
			this.OuterGroundNormal = groundingReport.OuterGroundNormal;
		}

		// Token: 0x04003363 RID: 13155
		public bool FoundAnyGround;

		// Token: 0x04003364 RID: 13156
		public bool IsStableOnGround;

		// Token: 0x04003365 RID: 13157
		public bool SnappingPrevented;

		// Token: 0x04003366 RID: 13158
		public Vector3 GroundNormal;

		// Token: 0x04003367 RID: 13159
		public Vector3 InnerGroundNormal;

		// Token: 0x04003368 RID: 13160
		public Vector3 OuterGroundNormal;
	}
}
