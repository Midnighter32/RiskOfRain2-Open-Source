using System;
using UnityEngine;

namespace KinematicCharacterController
{
	// Token: 0x02000912 RID: 2322
	public struct OverlapResult
	{
		// Token: 0x060033DA RID: 13274 RVA: 0x000E0D22 File Offset: 0x000DEF22
		public OverlapResult(Vector3 normal, Collider collider)
		{
			this.Normal = normal;
			this.Collider = collider;
		}

		// Token: 0x04003359 RID: 13145
		public Vector3 Normal;

		// Token: 0x0400335A RID: 13146
		public Collider Collider;
	}
}
