using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x020006CB RID: 1739
	public struct RigidbodyProjectionHit
	{
		// Token: 0x040028DB RID: 10459
		public Rigidbody Rigidbody;

		// Token: 0x040028DC RID: 10460
		public Vector3 HitPoint;

		// Token: 0x040028DD RID: 10461
		public Vector3 EffectiveHitNormal;

		// Token: 0x040028DE RID: 10462
		public Vector3 HitVelocity;

		// Token: 0x040028DF RID: 10463
		public bool StableOnHit;
	}
}
