using System;

namespace KinematicCharacterController
{
	// Token: 0x020006C5 RID: 1733
	public enum MovementSweepState
	{
		// Token: 0x040028B4 RID: 10420
		Initial,
		// Token: 0x040028B5 RID: 10421
		AfterFirstHit,
		// Token: 0x040028B6 RID: 10422
		FoundBlockingCrease,
		// Token: 0x040028B7 RID: 10423
		FoundBlockingCorner
	}
}
