using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200052A RID: 1322
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileGrantOnKillOnDestroy : MonoBehaviour
	{
		// Token: 0x06001F4F RID: 8015 RVA: 0x00087F8C File Offset: 0x0008618C
		private void OnDestroy()
		{
			this.healthComponent = base.GetComponent<HealthComponent>();
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			if (NetworkServer.active && this.projectileController.owner)
			{
				DamageInfo damageInfo = new DamageInfo
				{
					attacker = this.projectileController.owner,
					crit = this.projectileDamage.crit,
					damage = this.projectileDamage.damage,
					position = base.transform.position,
					procCoefficient = this.projectileController.procCoefficient,
					damageType = this.projectileDamage.damageType,
					damageColorIndex = this.projectileDamage.damageColorIndex
				};
				HealthComponent victim = this.healthComponent;
				DamageReport damageReport = new DamageReport(damageInfo, victim, damageInfo.damage);
				GlobalEventManager.instance.OnCharacterDeath(damageReport);
			}
		}

		// Token: 0x04001CF7 RID: 7415
		private ProjectileController projectileController;

		// Token: 0x04001CF8 RID: 7416
		private ProjectileDamage projectileDamage;

		// Token: 0x04001CF9 RID: 7417
		private HealthComponent healthComponent;
	}
}
