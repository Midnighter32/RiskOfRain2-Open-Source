using System;

namespace KinematicCharacterController
{
	// Token: 0x02000910 RID: 2320
	public enum MovementSweepState
	{
		// Token: 0x0400334D RID: 13133
		Initial,
		// Token: 0x0400334E RID: 13134
		AfterFirstHit,
		// Token: 0x0400334F RID: 13135
		FoundBlockingCrease,
		// Token: 0x04003350 RID: 13136
		FoundBlockingCorner
	}
}
