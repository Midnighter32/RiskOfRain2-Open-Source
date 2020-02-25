using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x02000911 RID: 2321
	[Serializable]
	public struct KinematicCharacterMotorState
	{
		// Token: 0x04003351 RID: 13137
		public Vector3 Position;

		// Token: 0x04003352 RID: 13138
		public Quaternion Rotation;

		// Token: 0x04003353 RID: 13139
		public Vector3 BaseVelocity;

		// Token: 0x04003354 RID: 13140
		public bool MustUnground;

		// Token: 0x04003355 RID: 13141
		public bool LastMovementIterationFoundAnyGround;

		// Token: 0x04003356 RID: 13142
		public CharacterTransientGroundingReport GroundingStatus;

		// Token: 0x04003357 RID: 13143
		public Rigidbody AttachedRigidbody;

		// Token: 0x04003358 RID: 13144
		public Vector3 AttachedRigidbodyVelocity;
	}
}
