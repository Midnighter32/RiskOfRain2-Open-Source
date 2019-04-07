using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000552 RID: 1362
	[RequireComponent(typeof(ProjectileDamage))]
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileMageFirewallController : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001E53 RID: 7763 RVA: 0x0008F07B File Offset: 0x0008D27B
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
		}

		// Token: 0x06001E54 RID: 7764 RVA: 0x0008F095 File Offset: 0x0008D295
		void IProjectileImpactBehavior.OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
			if (this.consumed)
			{
				return;
			}
			this.consumed = true;
			this.CreateWalkers();
			UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x06001E55 RID: 7765 RVA: 0x0008F0B8 File Offset: 0x0008D2B8
		private void CreateWalkers()
		{
			Vector3 forward = base.transform.forward;
			forward.y = 0f;
			forward.Normalize();
			Vector3 vector = Vector3.Cross(Vector3.up, forward);
			ProjectileManager.instance.FireProjectile(this.walkerPrefab, base.transform.position + Vector3.up, Util.QuaternionSafeLookRotation(vector), this.projectileController.owner, this.projectileDamage.damage, this.projectileDamage.force, this.projectileDamage.crit, this.projectileDamage.damageColorIndex, null, -1f);
			ProjectileManager.instance.FireProjectile(this.walkerPrefab, base.transform.position + Vector3.up, Util.QuaternionSafeLookRotation(-vector), this.projectileController.owner, this.projectileDamage.damage, this.projectileDamage.force, this.projectileDamage.crit, this.projectileDamage.damageColorIndex, null, -1f);
		}

		// Token: 0x040020F2 RID: 8434
		public GameObject walkerPrefab;

		// Token: 0x040020F3 RID: 8435
		private ProjectileController projectileController;

		// Token: 0x040020F4 RID: 8436
		private ProjectileDamage projectileDamage;

		// Token: 0x040020F5 RID: 8437
		private bool consumed;
	}
}
