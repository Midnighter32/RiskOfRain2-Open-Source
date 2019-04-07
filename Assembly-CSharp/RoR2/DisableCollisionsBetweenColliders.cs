using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002D5 RID: 725
	public class DisableCollisionsBetweenColliders : MonoBehaviour
	{
		// Token: 0x06000E85 RID: 3717 RVA: 0x00047968 File Offset: 0x00045B68
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

		// Token: 0x0400129C RID: 4764
		public Collider[] collidersA;

		// Token: 0x0400129D RID: 4765
		public Collider[] collidersB;
	}
}
