using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000519 RID: 1305
	[RequireComponent(typeof(ProjectileDamage))]
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileMageFirewallController : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001ECB RID: 7883 RVA: 0x0008555B File Offset: 0x0008375B
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
		}

		// Token: 0x06001ECC RID: 7884 RVA: 0x00085575 File Offset: 0x00083775
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

		// Token: 0x06001ECD RID: 7885 RVA: 0x00085598 File Offset: 0x00083798
		private void CreateWalkers()
		{
			Vector3 forward = base.transform.forward;
			forward.y = 0f;
			forward.Normalize();
			Vector3 vector = Vector3.Cross(Vector3.up, forward);
			ProjectileManager.instance.FireProjectile(this.walkerPrefab, base.transform.position + Vector3.up, Util.QuaternionSafeLookRotation(vector), this.projectileController.owner, this.projectileDamage.damage, this.projectileDamage.force, this.projectileDamage.crit, this.projectileDamage.damageColorIndex, null, -1f);
			ProjectileManager.instance.FireProjectile(this.walkerPrefab, base.transform.position + Vector3.up, Util.QuaternionSafeLookRotation(-vector), this.projectileController.owner, this.projectileDamage.damage, this.projectileDamage.force, this.projectileDamage.crit, this.projectileDamage.damageColorIndex, null, -1f);
		}

		// Token: 0x04001C57 RID: 7255
		public GameObject walkerPrefab;

		// Token: 0x04001C58 RID: 7256
		private ProjectileController projectileController;

		// Token: 0x04001C59 RID: 7257
		private ProjectileDamage projectileDamage;

		// Token: 0x04001C5A RID: 7258
		private bool consumed;
	}
}
