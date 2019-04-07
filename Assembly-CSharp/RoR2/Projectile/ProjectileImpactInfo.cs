using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x0200053B RID: 1339
	public struct ProjectileImpactInfo
	{
		// Token: 0x04002053 RID: 8275
		public Collider collider;

		// Token: 0x04002054 RID: 8276
		public Vector3 estimatedPointOfImpact;

		// Token: 0x04002055 RID: 8277
		public Vector3 estimatedImpactNormal;
	}
}
