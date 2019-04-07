using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200055F RID: 1375
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileSingleTargetImpact : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001EAF RID: 7855 RVA: 0x00090CF5 File Offset: 0x0008EEF5
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
		}

		// Token: 0x06001EB0 RID: 7856 RVA: 0x00090D10 File Offset: 0x0008EF10
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
						if (NetworkServer.active && !flag)
						{
							damageInfo.ModifyDamageInfo(component.damageModifier);
							healthComponent.TakeDamage(damageInfo);
							GlobalEventManager.instance.OnHitEnemy(damageInfo, component.healthComponent.gameObject);
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
				if (NetworkServer.active)
				{
					if (this.impactEffect)
					{
						EffectManager.instance.SimpleImpactEffect(this.impactEffect, impactInfo.estimatedPointOfImpact, -base.transform.forward, !this.projectileController.isPrediction);
					}
					if (this.hitSoundString.Length > 0)
					{
						Util.PlaySound(this.hitSoundString, base.gameObject);
					}
				}
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x0400215D RID: 8541
		private ProjectileController projectileController;

		// Token: 0x0400215E RID: 8542
		private ProjectileDamage projectileDamage;

		// Token: 0x0400215F RID: 8543
		private bool alive = true;

		// Token: 0x04002160 RID: 8544
		public bool destroyOnWorld;

		// Token: 0x04002161 RID: 8545
		public GameObject impactEffect;

		// Token: 0x04002162 RID: 8546
		public string hitSoundString;
	}
}
