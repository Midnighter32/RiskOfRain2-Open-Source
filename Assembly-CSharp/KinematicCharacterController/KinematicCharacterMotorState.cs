using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x020006C6 RID: 1734
	[Serializable]
	public struct KinematicCharacterMotorState
	{
		// Token: 0x040028B8 RID: 10424
		public Vector3 Position;

		// Token: 0x040028B9 RID: 10425
		public Quaternion Rotation;

		// Token: 0x040028BA RID: 10426
		public Vector3 BaseVelocity;

		// Token: 0x040028BB RID: 10427
		public bool MustUnground;

		// Token: 0x040028BC RID: 10428
		public bool LastMovementIterationFoundAnyGround;

		// Token: 0x040028BD RID: 10429
		public CharacterTransientGroundingReport GroundingStatus;

		// Token: 0x040028BE RID: 10430
		public Rigidbody AttachedRigidbody;

		// Token: 0x040028BF RID: 10431
		public Vector3 AttachedRigidbodyVelocity;
	}
}
