using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000512 RID: 1298
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileImpactEventCaller : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001EB8 RID: 7864 RVA: 0x00084CD9 File Offset: 0x00082ED9
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
			ProjectileImpactEvent projectileImpactEvent = this.impactEvent;
			if (projectileImpactEvent == null)
			{
				return;
			}
			projectileImpactEvent.Invoke(impactInfo);
		}

		// Token: 0x04001C28 RID: 7208
		public ProjectileImpactEvent impactEvent;
	}
}
