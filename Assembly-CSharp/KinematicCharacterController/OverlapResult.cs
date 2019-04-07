using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x020006C7 RID: 1735
	public struct OverlapResult
	{
		// Token: 0x06002690 RID: 9872 RVA: 0x000B18DA File Offset: 0x000AFADA
		public OverlapResult(Vector3 normal, Collider collider)
		{
			this.Normal = normal;
			this.Collider = collider;
		}

		// Token: 0x040028C0 RID: 10432
		public Vector3 Normal;

		// Token: 0x040028C1 RID: 10433
		public Collider Collider;
	}
}
