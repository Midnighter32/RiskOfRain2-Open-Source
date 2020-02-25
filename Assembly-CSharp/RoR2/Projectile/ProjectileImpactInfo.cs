using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x020004F6 RID: 1270
	public struct ProjectileImpactInfo
	{
		// Token: 0x04001B70 RID: 7024
		public Collider collider;

		// Token: 0x04001B71 RID: 7025
		public Vector3 estimatedPointOfImpact;

		// Token: 0x04001B72 RID: 7026
		public Vector3 estimatedImpactNormal;
	}
}
