using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001FD RID: 509
	public class ExplodeRigidbodiesOnStart : MonoBehaviour
	{
		// Token: 0x06000ADF RID: 2783 RVA: 0x0002FFF8 File Offset: 0x0002E1F8
		private void Start()
		{
			Vector3 position = base.transform.position;
			for (int i = 0; i < this.bodies.Length; i++)
			{
				this.bodies[i].AddExplosionForce(this.force, position, this.explosionRadius);
			}
		}

		// Token: 0x04000B29 RID: 2857
		public Rigidbody[] bodies;

		// Token: 0x04000B2A RID: 2858
		public float force;

		// Token: 0x04000B2B RID: 2859
		public float explosionRadius;
	}
}
