using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x02000915 RID: 2325
	public struct HitStabilityReport
	{
		// Token: 0x04003369 RID: 13161
		public bool IsStable;

		// Token: 0x0400336A RID: 13162
		public Vector3 InnerNormal;

		// Token: 0x0400336B RID: 13163
		public Vector3 OuterNormal;

		// Token: 0x0400336C RID: 13164
		public bool ValidStepDetected;

		// Token: 0x0400336D RID: 13165
		public Collider SteppedCollider;

		// Token: 0x0400336E RID: 13166
		public bool LedgeDetected;

		// Token: 0x0400336F RID: 13167
		public bool IsOnEmptySideOfLedge;

		// Token: 0x04003370 RID: 13168
		public float DistanceFromLedge;

		// Token: 0x04003371 RID: 13169
		public Vector3 LedgeGroundNormal;

		// Token: 0x04003372 RID: 13170
		public Vector3 LedgeRightDirection;

		// Token: 0x04003373 RID: 13171
		public Vector3 LedgeFacingDirection;
	}
}
