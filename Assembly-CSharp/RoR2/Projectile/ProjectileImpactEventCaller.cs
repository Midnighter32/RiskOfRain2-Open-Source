using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x0200054C RID: 1356
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileImpactEventCaller : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001E42 RID: 7746 RVA: 0x0008E825 File Offset: 0x0008CA25
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
			ProjectileImpactEvent projectileImpactEvent = this.impactEvent;
			if (projectileImpactEvent == null)
			{
				return;
			}
			projectileImpactEvent.Invoke(impactInfo);
		}

		// Token: 0x040020C5 RID: 8389
		public ProjectileImpactEvent impactEvent;
	}
}
