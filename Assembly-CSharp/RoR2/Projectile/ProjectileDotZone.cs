using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000503 RID: 1283
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(HitBoxGroup))]
	public class ProjectileDotZone : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001E76 RID: 7798 RVA: 0x00083674 File Offset: 0x00081874
		private void Start()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.ResetOverlap();
			this.onBegin.Invoke();
		}

		// Token: 0x06001E77 RID: 7799 RVA: 0x000836A0 File Offset: 0x000818A0
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

		// Token: 0x06001E78 RID: 7800 RVA: 0x0000409B File Offset: 0x0000229B
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
		}

		// Token: 0x06001E79 RID: 7801 RVA: 0x000837E0 File Offset: 0x000819E0
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

		// Token: 0x04001BC8 RID: 7112
		private ProjectileController projectileController;

		// Token: 0x04001BC9 RID: 7113
		private ProjectileDamage projectileDamage;

		// Token: 0x04001BCA RID: 7114
		public float damageCoefficient;

		// Token: 0x04001BCB RID: 7115
		public GameObject impactEffect;

		// Token: 0x04001BCC RID: 7116
		public Vector3 forceVector;

		// Token: 0x04001BCD RID: 7117
		public float overlapProcCoefficient = 1f;

		// Token: 0x04001BCE RID: 7118
		[Tooltip("The frequency (1/time) at which the overlap attack is tested. Higher values are more accurate but more expensive.")]
		public float fireFrequency = 1f;

		// Token: 0x04001BCF RID: 7119
		[Tooltip("The frequency  (1/time) at which the overlap attack is reset. Higher values means more frequent ticks of damage.")]
		public float resetFrequency = 20f;

		// Token: 0x04001BD0 RID: 7120
		public float lifetime = 30f;

		// Token: 0x04001BD1 RID: 7121
		[Tooltip("The event that runs at the start.")]
		public UnityEvent onBegin;

		// Token: 0x04001BD2 RID: 7122
		[Tooltip("The event that runs at the start.")]
		public UnityEvent onEnd;

		// Token: 0x04001BD3 RID: 7123
		private OverlapAttack attack;

		// Token: 0x04001BD4 RID: 7124
		private float fireStopwatch;

		// Token: 0x04001BD5 RID: 7125
		private float resetStopwatch;

		// Token: 0x04001BD6 RID: 7126
		private float totalStopwatch;
	}
}
