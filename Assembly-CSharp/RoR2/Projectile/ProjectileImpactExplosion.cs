using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000514 RID: 1300
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileImpactExplosion : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001EBB RID: 7867 RVA: 0x00084CF4 File Offset: 0x00082EF4
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.lifetime += UnityEngine.Random.Range(0f, this.lifetimeRandomOffset);
		}

		// Token: 0x06001EBC RID: 7868 RVA: 0x00084D2C File Offset: 0x00082F2C
		public void FixedUpdate()
		{
			this.stopwatch += Time.fixedDeltaTime;
			if (this.timerAfterImpact && this.hasImpact)
			{
				this.stopwatchAfterImpact += Time.fixedDeltaTime;
			}
			if ((this.stopwatch >= this.lifetime && (!this.timerAfterImpact || !this.hasImpact)) || this.stopwatchAfterImpact > this.lifetimeAfterImpact || (this.projectileHealthComponent && !this.projectileHealthComponent.alive))
			{
				this.alive = false;
			}
			if (this.alive && !this.hasPlayedLifetimeExpiredSound && (this.stopwatch > this.lifetime - this.offsetForLifetimeExpiredSound || (this.timerAfterImpact && this.stopwatchAfterImpact > this.lifetimeAfterImpact - this.offsetForLifetimeExpiredSound)))
			{
				this.hasPlayedLifetimeExpiredSound = true;
				Util.PlaySound(this.lifetimeExpiredSoundString, base.gameObject);
			}
			if (!this.alive)
			{
				if (NetworkServer.active)
				{
					if (this.impactEffect)
					{
						EffectManager.SpawnEffect(this.impactEffect, new EffectData
						{
							origin = base.transform.position,
							scale = this.blastRadius
						}, true);
					}
					if (this.projectileDamage)
					{
						new BlastAttack
						{
							position = base.transform.position,
							baseDamage = this.projectileDamage.damage * this.blastDamageCoefficient,
							baseForce = this.projectileDamage.force * this.blastDamageCoefficient,
							radius = this.blastRadius,
							attacker = (this.projectileController.owner ? this.projectileController.owner.gameObject : null),
							inflictor = base.gameObject,
							teamIndex = this.projectileController.teamFilter.teamIndex,
							crit = this.projectileDamage.crit,
							procChainMask = this.projectileController.procChainMask,
							procCoefficient = this.projectileController.procCoefficient * this.blastProcCoefficient,
							bonusForce = this.bonusBlastForce,
							falloffModel = this.falloffModel,
							damageColorIndex = this.projectileDamage.damageColorIndex,
							damageType = this.projectileDamage.damageType
						}.Fire();
					}
					if (this.explosionSoundString.Length > 0)
					{
						Util.PlaySound(this.explosionSoundString, base.gameObject);
					}
					if (this.fireChildren)
					{
						for (int i = 0; i < this.childrenCount; i++)
						{
							Vector3 vector = new Vector3(UnityEngine.Random.Range(this.minAngleOffset.x, this.maxAngleOffset.x), UnityEngine.Random.Range(this.minAngleOffset.z, this.maxAngleOffset.z), UnityEngine.Random.Range(this.minAngleOffset.z, this.maxAngleOffset.z));
							switch (this.transformSpace)
							{
							case ProjectileImpactExplosion.TransformSpace.World:
								this.FireChild(vector);
								break;
							case ProjectileImpactExplosion.TransformSpace.Local:
								this.FireChild(base.transform.forward + base.transform.TransformDirection(vector));
								break;
							case ProjectileImpactExplosion.TransformSpace.Normal:
								this.FireChild(this.impactNormal + vector);
								break;
							}
						}
					}
				}
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x06001EBD RID: 7869 RVA: 0x00085090 File Offset: 0x00083290
		private void FireChild(Vector3 direction)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.childrenProjectilePrefab, base.transform.position, Util.QuaternionSafeLookRotation(direction));
			ProjectileController component = gameObject.GetComponent<ProjectileController>();
			if (component)
			{
				component.procChainMask = this.projectileController.procChainMask;
				component.procCoefficient = this.projectileController.procCoefficient;
				component.Networkowner = this.projectileController.owner;
			}
			gameObject.GetComponent<TeamFilter>().teamIndex = base.GetComponent<TeamFilter>().teamIndex;
			ProjectileDamage component2 = gameObject.GetComponent<ProjectileDamage>();
			if (component2)
			{
				component2.damage = this.projectileDamage.damage * this.childrenDamageCoefficient;
				component2.crit = this.projectileDamage.crit;
				component2.force = this.projectileDamage.force;
				component2.damageColorIndex = this.projectileDamage.damageColorIndex;
			}
			NetworkServer.Spawn(gameObject);
		}

		// Token: 0x06001EBE RID: 7870 RVA: 0x00085170 File Offset: 0x00083370
		public void SetExplosionRadius(float newRadius)
		{
			this.blastRadius = newRadius;
		}

		// Token: 0x06001EBF RID: 7871 RVA: 0x0008517C File Offset: 0x0008337C
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
			if (!this.alive)
			{
				return;
			}
			Collider collider = impactInfo.collider;
			this.impactNormal = impactInfo.estimatedImpactNormal;
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
				}
				else
				{
					Debug.Log("No projectile damage component!");
				}
				HurtBox component = collider.GetComponent<HurtBox>();
				if (component)
				{
					if (this.destroyOnEnemy)
					{
						HealthComponent healthComponent = component.healthComponent;
						if (healthComponent)
						{
							if (healthComponent.gameObject == this.projectileController.owner)
							{
								return;
							}
							if (this.projectileHealthComponent && healthComponent == this.projectileHealthComponent)
							{
								return;
							}
							this.alive = false;
						}
					}
				}
				else if (this.destroyOnWorld)
				{
					this.alive = false;
				}
				this.hasImpact = true;
				if (NetworkServer.active)
				{
					GlobalEventManager.instance.OnHitAll(damageInfo, collider.gameObject);
				}
			}
		}

		// Token: 0x06001EC0 RID: 7872 RVA: 0x0008530C File Offset: 0x0008350C
		public void SetAlive(bool newAlive)
		{
			this.alive = newAlive;
		}

		// Token: 0x04001C29 RID: 7209
		private ProjectileController projectileController;

		// Token: 0x04001C2A RID: 7210
		private ProjectileDamage projectileDamage;

		// Token: 0x04001C2B RID: 7211
		private bool alive = true;

		// Token: 0x04001C2C RID: 7212
		private Vector3 impactNormal = Vector3.up;

		// Token: 0x04001C2D RID: 7213
		public GameObject impactEffect;

		// Token: 0x04001C2E RID: 7214
		public string explosionSoundString;

		// Token: 0x04001C2F RID: 7215
		public string lifetimeExpiredSoundString;

		// Token: 0x04001C30 RID: 7216
		public float offsetForLifetimeExpiredSound;

		// Token: 0x04001C31 RID: 7217
		public bool destroyOnEnemy = true;

		// Token: 0x04001C32 RID: 7218
		public bool destroyOnWorld;

		// Token: 0x04001C33 RID: 7219
		public bool timerAfterImpact;

		// Token: 0x04001C34 RID: 7220
		public BlastAttack.FalloffModel falloffModel = BlastAttack.FalloffModel.Linear;

		// Token: 0x04001C35 RID: 7221
		public float lifetime;

		// Token: 0x04001C36 RID: 7222
		public float lifetimeAfterImpact;

		// Token: 0x04001C37 RID: 7223
		public float lifetimeRandomOffset;

		// Token: 0x04001C38 RID: 7224
		public float blastRadius;

		// Token: 0x04001C39 RID: 7225
		[Tooltip("The percentage of the damage, proc coefficient, and force of the initial projectile. Ranges from 0-1")]
		public float blastDamageCoefficient;

		// Token: 0x04001C3A RID: 7226
		public float blastProcCoefficient = 1f;

		// Token: 0x04001C3B RID: 7227
		public Vector3 bonusBlastForce;

		// Token: 0x04001C3C RID: 7228
		[Tooltip("Does this projectile release children on death?")]
		public bool fireChildren;

		// Token: 0x04001C3D RID: 7229
		public GameObject childrenProjectilePrefab;

		// Token: 0x04001C3E RID: 7230
		public int childrenCount;

		// Token: 0x04001C3F RID: 7231
		[Tooltip("What percentage of our damage does the children get?")]
		public float childrenDamageCoefficient;

		// Token: 0x04001C40 RID: 7232
		public Vector3 minAngleOffset;

		// Token: 0x04001C41 RID: 7233
		public Vector3 maxAngleOffset;

		// Token: 0x04001C42 RID: 7234
		public ProjectileImpactExplosion.TransformSpace transformSpace;

		// Token: 0x04001C43 RID: 7235
		public HealthComponent projectileHealthComponent;

		// Token: 0x04001C44 RID: 7236
		private float stopwatch;

		// Token: 0x04001C45 RID: 7237
		private float stopwatchAfterImpact;

		// Token: 0x04001C46 RID: 7238
		private bool hasImpact;

		// Token: 0x04001C47 RID: 7239
		private bool hasPlayedLifetimeExpiredSound;

		// Token: 0x02000515 RID: 1301
		public enum TransformSpace
		{
			// Token: 0x04001C49 RID: 7241
			World,
			// Token: 0x04001C4A RID: 7242
			Local,
			// Token: 0x04001C4B RID: 7243
			Normal
		}
	}
}
