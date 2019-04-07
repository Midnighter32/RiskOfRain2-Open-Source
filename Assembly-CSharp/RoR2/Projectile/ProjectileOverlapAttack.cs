using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x0200055B RID: 1371
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(ProjectileDamage))]
	[RequireComponent(typeof(HitBoxGroup))]
	public class ProjectileOverlapAttack : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001E96 RID: 7830 RVA: 0x00090358 File Offset: 0x0008E558
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
			this.attack.hitBoxGroup = base.GetComponent<HitBoxGroup>();
		}

		// Token: 0x06001E97 RID: 7831 RVA: 0x00004507 File Offset: 0x00002707
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
		}

		// Token: 0x06001E98 RID: 7832 RVA: 0x000904C8 File Offset: 0x0008E6C8
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
				this.attack.Fire(null);
			}
		}

		// Token: 0x0400212F RID: 8495
		private ProjectileController projectileController;

		// Token: 0x04002130 RID: 8496
		private ProjectileDamage projectileDamage;

		// Token: 0x04002131 RID: 8497
		public float damageCoefficient;

		// Token: 0x04002132 RID: 8498
		public GameObject impactEffect;

		// Token: 0x04002133 RID: 8499
		public Vector3 forceVector;

		// Token: 0x04002134 RID: 8500
		public float overlapProcCoefficient = 1f;

		// Token: 0x04002135 RID: 8501
		private OverlapAttack attack;

		// Token: 0x04002136 RID: 8502
		[Tooltip("If non-negative, the attack clears its hit memory at the specified interval.")]
		public float resetInterval = -1f;

		// Token: 0x04002137 RID: 8503
		private float resetTimer;
	}
}
