using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x020004FF RID: 1279
	public class ProjectileDamage : MonoBehaviour
	{
		// Token: 0x04001BA9 RID: 7081
		[HideInInspector]
		public float damage;

		// Token: 0x04001BAA RID: 7082
		[HideInInspector]
		public bool crit;

		// Token: 0x04001BAB RID: 7083
		[HideInInspector]
		public float force;

		// Token: 0x04001BAC RID: 7084
		[HideInInspector]
		public DamageColorIndex damageColorIndex;

		// Token: 0x04001BAD RID: 7085
		public DamageType damageType;
	}
}
