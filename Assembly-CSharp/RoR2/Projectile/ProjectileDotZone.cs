using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000545 RID: 1349
	[RequireComponent(typeof(HitBoxGroup))]
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileDotZone : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001E23 RID: 7715 RVA: 0x0008DE2E File Offset: 0x0008C02E
		private void Start()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.ResetOverlap();
			this.onBegin.Invoke();
		}

		// Token: 0x06001E24 RID: 7716 RVA: 0x0008DE5C File Offset: 0x0008C05C
		private void ResetOverlap()
		{
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
			this.attack.hitBoxGroup = base.GetComponent<HitBoxGroup>();
		}

		// Token: 0x06001E25 RID: 7717 RVA: 0x00004507 File Offset: 0x00002707
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
		}

		// Token: 0x06001E26 RID: 7718 RVA: 0x0008DF9C File Offset: 0x0008C19C
		public void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.totalStopwatch += Time.fixedDeltaTime;
				this.resetStopwatch += Time.fixedDeltaTime;
				this.fireStopwatch += Time.fixedDeltaTime;
				if (this.resetStopwatch >= 1f / this.resetFrequency)
				{
					this.ResetOverlap();
					this.resetStopwatch -= 1f / this.resetFrequency;
				}
				if (this.fireStopwatch >= 1f / this.fireFrequency)
				{
					this.attack.Fire(null);
					this.fireStopwatch -= 1f / this.fireFrequency;
				}
				if (this.totalStopwatch >= this.lifetime)
				{
					this.onEnd.Invoke();
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x04002094 RID: 8340
		private ProjectileController projectileController;

		// Token: 0x04002095 RID: 8341
		private ProjectileDamage projectileDamage;

		// Token: 0x04002096 RID: 8342
		public float damageCoefficient;

		// Token: 0x04002097 RID: 8343
		public GameObject impactEffect;

		// Token: 0x04002098 RID: 8344
		public Vector3 forceVector;

		// Token: 0x04002099 RID: 8345
		public float overlapProcCoefficient = 1f;

		// Token: 0x0400209A RID: 8346
		[Tooltip("The frequency (1/time) at which the overlap attack is tested. Higher values are more accurate but more expensive.")]
		public float fireFrequency = 1f;

		// Token: 0x0400209B RID: 8347
		[Tooltip("The frequency  (1/time) at which the overlap attack is reset. Higher values means more frequent ticks of damage.")]
		public float resetFrequency = 20f;

		// Token: 0x0400209C RID: 8348
		public float lifetime = 30f;

		// Token: 0x0400209D RID: 8349
		[Tooltip("The event that runs at the start.")]
		public UnityEvent onBegin;

		// Token: 0x0400209E RID: 8350
		[Tooltip("The event that runs at the start.")]
		public UnityEvent onEnd;

		// Token: 0x0400209F RID: 8351
		private OverlapAttack attack;

		// Token: 0x040020A0 RID: 8352
		private float fireStopwatch;

		// Token: 0x040020A1 RID: 8353
		private float resetStopwatch;

		// Token: 0x040020A2 RID: 8354
		private float totalStopwatch;
	}
}
