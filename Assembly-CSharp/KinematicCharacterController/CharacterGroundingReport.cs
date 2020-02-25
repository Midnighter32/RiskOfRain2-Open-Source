using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x02000913 RID: 2323
	public struct CharacterGroundingReport
	{
		// Token: 0x060033DB RID: 13275 RVA: 0x000E0D34 File Offset: 0x000DEF34
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

		// Token: 0x0400335B RID: 13147
		public bool FoundAnyGround;

		// Token: 0x0400335C RID: 13148
		public bool IsStableOnGround;

		// Token: 0x0400335D RID: 13149
		public bool SnappingPrevented;

		// Token: 0x0400335E RID: 13150
		public Vector3 GroundNormal;

		// Token: 0x0400335F RID: 13151
		public Vector3 InnerGroundNormal;

		// Token: 0x04003360 RID: 13152
		public Vector3 OuterGroundNormal;

		// Token: 0x04003361 RID: 13153
		public Collider GroundCollider;

		// Token: 0x04003362 RID: 13154
		public Vector3 GroundPoint;
	}
}
