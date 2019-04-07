using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x020006C9 RID: 1737
	public struct CharacterTransientGroundingReport
	{
		// Token: 0x06002692 RID: 9874 RVA: 0x000B1954 File Offset: 0x000AFB54
		public void CopyFrom(CharacterGroundingReport groundingReport)
		{
			this.FoundAnyGround = groundingReport.FoundAnyGround;
			this.IsStableOnGround = groundingReport.IsStableOnGround;
			this.SnappingPrevented = groundingReport.SnappingPrevented;
			this.GroundNormal = groundingReport.GroundNormal;
			this.InnerGroundNormal = groundingReport.InnerGroundNormal;
			this.OuterGroundNormal = groundingReport.OuterGroundNormal;
		}

		// Token: 0x040028CA RID: 10442
		public bool FoundAnyGround;

		// Token: 0x040028CB RID: 10443
		public bool IsStableOnGround;

		// Token: 0x040028CC RID: 10444
		public bool SnappingPrevented;

		// Token: 0x040028CD RID: 10445
		public Vector3 GroundNormal;

		// Token: 0x040028CE RID: 10446
		public Vector3 InnerGroundNormal;

		// Token: 0x040028CF RID: 10447
		public Vector3 OuterGroundNormal;
	}
}
