using System;
using RoR2.Orbs;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x0200052B RID: 1323
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileHealOwnerOnDamageInflicted : MonoBehaviour, IOnDamageInflictedServerReceiver
	{
		// Token: 0x06001F51 RID: 8017 RVA: 0x00088079 File Offset: 0x00086279
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
		}

		// Token: 0x06001F52 RID: 8018 RVA: 0x00088088 File Offset: 0x00086288
		public void OnDamageInflictedServer(DamageReport damageReport)
		{
			if (this.projectileController.owner)
			{
				HealthComponent component = this.projectileController.owner.GetComponent<HealthComponent>();
				if (component)
				{
					HealOrb healOrb = new HealOrb();
					healOrb.origin = base.transform.position;
					healOrb.target = component.body.mainHurtBox;
					healOrb.healValue = damageReport.damageDealt * this.fractionOfDamage;
					healOrb.overrideDuration = 0.3f;
					OrbManager.instance.AddOrb(healOrb);
				}
			}
		}

		// Token: 0x04001CFA RID: 7418
		public float fractionOfDamage;

		// Token: 0x04001CFB RID: 7419
		private ProjectileController projectileController;
	}
}
