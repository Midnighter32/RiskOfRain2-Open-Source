using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x020006CF RID: 1743
	[Serializable]
	public struct PhysicsMoverState
	{
		// Token: 0x04002949 RID: 10569
		public Vector3 Position;

		// Token: 0x0400294A RID: 10570
		public Quaternion Rotation;

		// Token: 0x0400294B RID: 10571
		public Vector3 Velocity;

		// Token: 0x0400294C RID: 10572
		public Vector3 AngularVelocity;
	}
}
