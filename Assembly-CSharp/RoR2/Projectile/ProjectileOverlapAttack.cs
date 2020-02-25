using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000521 RID: 1313
	[RequireComponent(typeof(HitBoxGroup))]
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(ProjectileDamage))]
	public class ProjectileOverlapAttack : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001F07 RID: 7943 RVA: 0x000866D4 File Offset: 0x000848D4
		private void Start()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.attack = new OverlapAttack();
			this.attack.procChainMask = this.projectileController.procChainMask;
			this.attack.procCoefficient = this.projectileController.procCoefficient * this.overlapProcCoefficient;
			this.attack.attacker = this.projectileController.owner;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = this.projectileController.teamFilter.teamIndex;
			this.attack.damage = this.damageCoefficient * this.projectileDamage.damage;
			this.attack.forceVector = this.forceVector + this.projectileDamage.force * base.transform.forward;
			this.attack.hitEffectPrefab = this.impactEffect;
			this.attack.isCrit = this.projectileDamage.crit;
			this.attack.damageColorIndex = this.projectileDamage.damageColorIndex;
			this.attack.damageType = this.projectileDamage.damageType;
			this.attack.procChainMask = this.projectileController.procChainMask;
			this.attack.maximumOverlapTargets = this.maximumOverlapTargets;
			this.attack.hitBoxGroup = base.GetComponent<HitBoxGroup>();
		}

		// Token: 0x06001F08 RID: 7944 RVA: 0x0000409B File Offset: 0x0000229B
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
		}

		// Token: 0x06001F09 RID: 7945 RVA: 0x00086854 File Offset: 0x00084A54
		public void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				if (this.resetInterval >= 0f)
				{
					this.resetTimer -= Time.fixedDeltaTime;
					if (this.resetTimer <= 0f)
					{
						this.resetTimer = this.resetInterval;
						this.attack.ResetIgnoredHealthComponents();
					}
				}
				this.fireTimer -= Time.fixedDeltaTime;
				if (this.fireTimer <= 0f)
				{
					this.fireTimer = 1f / this.fireFrequency;
					this.attack.Fire(null);
				}
			}
		}

		// Token: 0x04001C91 RID: 7313
		private ProjectileController projectileController;

		// Token: 0x04001C92 RID: 7314
		private ProjectileDamage projectileDamage;

		// Token: 0x04001C93 RID: 7315
		public float damageCoefficient;

		// Token: 0x04001C94 RID: 7316
		public GameObject impactEffect;

		// Token: 0x04001C95 RID: 7317
		public Vector3 forceVector;

		// Token: 0x04001C96 RID: 7318
		public float overlapProcCoefficient = 1f;

		// Token: 0x04001C97 RID: 7319
		public int maximumOverlapTargets = 100;

		// Token: 0x04001C98 RID: 7320
		private OverlapAttack attack;

		// Token: 0x04001C99 RID: 7321
		public float fireFrequency = 60f;

		// Token: 0x04001C9A RID: 7322
		[Tooltip("If non-negative, the attack clears its hit memory at the specified interval.")]
		public float resetInterval = -1f;

		// Token: 0x04001C9B RID: 7323
		private float resetTimer;

		// Token: 0x04001C9C RID: 7324
		private float fireTimer;
	}
}
