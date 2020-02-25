using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x0200091A RID: 2330
	[Serializable]
	public struct PhysicsMoverState
	{
		// Token: 0x040033E2 RID: 13282
		public Vector3 Position;

		// Token: 0x040033E3 RID: 13283
		public Quaternion Rotation;

		// Token: 0x040033E4 RID: 13284
		public Vector3 Velocity;

		// Token: 0x040033E5 RID: 13285
		public Vector3 AngularVelocity;
	}
}
