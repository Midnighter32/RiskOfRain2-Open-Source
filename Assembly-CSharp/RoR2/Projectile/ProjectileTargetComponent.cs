using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000528 RID: 1320
	public class ProjectileTargetComponent : MonoBehaviour
	{
		// Token: 0x06001F47 RID: 8007 RVA: 0x00087E10 File Offset: 0x00086010
		private void FixedUpdate()
		{
			if (this.target && !this.target.gameObject.activeSelf)
			{
				this.target = null;
			}
		}

		// Token: 0x04001CF2 RID: 7410
		[NonSerialized]
		public Transform target;
	}
}
