using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000525 RID: 1317
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileSingleTargetImpact : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001F20 RID: 7968 RVA: 0x00087149 File Offset: 0x00085349
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
		}

		// Token: 0x06001F21 RID: 7969 RVA: 0x00087164 File Offset: 0x00085364
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
			if (!this.alive)
			{
				return;
			}
			Collider collider = impactInfo.collider;
			if (collider)
			{
				DamageInfo damageInfo = new DamageInfo();
				if (this.projectileDamage)
				{
					damageInfo.damage = this.projectileDamage.damage;
					damageInfo.crit = this.projectileDamage.crit;
					damageInfo.attacker = (this.projectileController.owner ? this.projectileController.owner.gameObject : null);
					damageInfo.inflictor = base.gameObject;
					damageInfo.position = impactInfo.estimatedPointOfImpact;
					damageInfo.force = this.projectileDamage.force * base.transform.forward;
					damageInfo.procChainMask = this.projectileController.procChainMask;
					damageInfo.procCoefficient = this.projectileController.procCoefficient;
					damageInfo.damageColorIndex = this.projectileDamage.damageColorIndex;
					damageInfo.damageType = this.projectileDamage.damageType;
				}
				else
				{
					Debug.Log("No projectile damage component!");
				}
				HurtBox component = collider.GetComponent<HurtBox>();
				if (component)
				{
					HealthComponent healthComponent = component.healthComponent;
					if (healthComponent)
					{
						if (healthComponent.gameObject == this.projectileController.owner)
						{
							return;
						}
						TeamComponent component2 = healthComponent.GetComponent<TeamComponent>();
						TeamFilter component3 = base.GetComponent<TeamFilter>();
						bool flag = false;
						if (component2 && component3)
						{
							flag = (component2.teamIndex == component3.teamIndex);
						}
						if (!flag)
						{
							Util.PlaySound(this.enemyHitSoundString, base.gameObject);
							if (NetworkServer.active)
							{
								damageInfo.ModifyDamageInfo(component.damageModifier);
								healthComponent.TakeDamage(damageInfo);
								GlobalEventManager.instance.OnHitEnemy(damageInfo, component.healthComponent.gameObject);
							}
						}
						this.alive = false;
					}
				}
				else if (this.destroyOnWorld)
				{
					this.alive = false;
				}
				damageInfo.position = base.transform.position;
				if (NetworkServer.active)
				{
					GlobalEventManager.instance.OnHitAll(damageInfo, collider.gameObject);
				}
			}
			if (!this.alive)
			{
				if (NetworkServer.active && this.impactEffect)
				{
					EffectManager.SimpleImpactEffect(this.impactEffect, impactInfo.estimatedPointOfImpact, -base.transform.forward, !this.projectileController.isPrediction);
				}
				Util.PlaySound(this.hitSoundString, base.gameObject);
				if (this.destroyWhenNotAlive)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x04001CC6 RID: 7366
		private ProjectileController projectileController;

		// Token: 0x04001CC7 RID: 7367
		private ProjectileDamage projectileDamage;

		// Token: 0x04001CC8 RID: 7368
		private bool alive = true;

		// Token: 0x04001CC9 RID: 7369
		public bool destroyWhenNotAlive = true;

		// Token: 0x04001CCA RID: 7370
		public bool destroyOnWorld;

		// Token: 0x04001CCB RID: 7371
		public GameObject impactEffect;

		// Token: 0x04001CCC RID: 7372
		public string hitSoundString;

		// Token: 0x04001CCD RID: 7373
		public string enemyHitSoundString;
	}
}
