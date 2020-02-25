using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001E7 RID: 487
	public class DisableCollisionsBetweenColliders : MonoBehaviour
	{
		// Token: 0x06000A31 RID: 2609 RVA: 0x0002C89C File Offset: 0x0002AA9C
		public void Awake()
		{
			foreach (Collider collider in this.collidersA)
			{
				foreach (Collider collider2 in this.collidersB)
				{
					Physics.IgnoreCollision(collider, collider2);
				}
			}
		}

		// Token: 0x04000A91 RID: 2705
		public Collider[] collidersA;

		// Token: 0x04000A92 RID: 2706
		public Collider[] collidersB;
	}
}
