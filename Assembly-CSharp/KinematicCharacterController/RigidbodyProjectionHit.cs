using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x02000916 RID: 2326
	public struct RigidbodyProjectionHit
	{
		// Token: 0x04003374 RID: 13172
		public Rigidbody Rigidbody;

		// Token: 0x04003375 RID: 13173
		public Vector3 HitPoint;

		// Token: 0x04003376 RID: 13174
		public Vector3 EffectiveHitNormal;

		// Token: 0x04003377 RID: 13175
		public Vector3 HitVelocity;

		// Token: 0x04003378 RID: 13176
		public bool StableOnHit;
	}
}
