using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x020006CA RID: 1738
	public struct HitStabilityReport
	{
		// Token: 0x040028D0 RID: 10448
		public bool IsStable;

		// Token: 0x040028D1 RID: 10449
		public Vector3 InnerNormal;

		// Token: 0x040028D2 RID: 10450
		public Vector3 OuterNormal;

		// Token: 0x040028D3 RID: 10451
		public bool ValidStepDetected;

		// Token: 0x040028D4 RID: 10452
		public Collider SteppedCollider;

		// Token: 0x040028D5 RID: 10453
		public bool LedgeDetected;

		// Token: 0x040028D6 RID: 10454
		public bool IsOnEmptySideOfLedge;

		// Token: 0x040028D7 RID: 10455
		public float DistanceFromLedge;

		// Token: 0x040028D8 RID: 10456
		public Vector3 LedgeGroundNormal;

		// Token: 0x040028D9 RID: 10457
		public Vector3 LedgeRightDirection;

		// Token: 0x040028DA RID: 10458
		public Vector3 LedgeFacingDirection;
	}
}
