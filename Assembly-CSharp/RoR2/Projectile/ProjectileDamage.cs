using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000542 RID: 1346
	public class ProjectileDamage : MonoBehaviour
	{
		// Token: 0x04002081 RID: 8321
		[HideInInspector]
		public float damage;

		// Token: 0x04002082 RID: 8322
		[HideInInspector]
		public bool crit;

		// Token: 0x04002083 RID: 8323
		[HideInInspector]
		public float force;

		// Token: 0x04002084 RID: 8324
		[HideInInspector]
		public DamageColorIndex damageColorIndex;

		// Token: 0x04002085 RID: 8325
		public DamageType damageType;
	}
}
