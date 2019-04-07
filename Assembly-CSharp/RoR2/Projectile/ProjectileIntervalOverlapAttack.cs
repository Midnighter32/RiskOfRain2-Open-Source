using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000551 RID: 1361
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(ProjectileDamage))]
	public class ProjectileIntervalOverlapAttack : MonoBehaviour
	{
		// Token: 0x06001E4F RID: 7759 RVA: 0x0008EF54 File Offset: 0x0008D154
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
		}

		// Token: 0x06001E50 RID: 7760 RVA: 0x0008EF6E File Offset: 0x0008D16E
		private void Start()
		{
			this.countdown = 0f;
		}

		// Token: 0x06001E51 RID: 7761 RVA: 0x0008EF7C File Offset: 0x0008D17C
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.countdown -= Time.fixedDeltaTime;
				if (this.countdown <= 0f)
				{
					this.countdown += this.interval;
					if (this.hitBoxGroup)
					{
						new OverlapAttack
						{
							attacker = this.projectileController.owner,
							inflictor = base.gameObject,
							teamIndex = this.projectileController.teamFilter.teamIndex,
							damage = this.projectileDamage.damage * this.damageCoefficient,
							hitBoxGroup = this.hitBoxGroup,
							isCrit = this.projectileDamage.crit,
							procCoefficient = 0f,
							damageType = this.projectileDamage.damageType
						}.Fire(null);
					}
				}
			}
		}

		// Token: 0x040020EC RID: 8428
		public HitBoxGroup hitBoxGroup;

		// Token: 0x040020ED RID: 8429
		public float interval;

		// Token: 0x040020EE RID: 8430
		public float damageCoefficient = 1f;

		// Token: 0x040020EF RID: 8431
		private float countdown;

		// Token: 0x040020F0 RID: 8432
		private ProjectileController projectileController;

		// Token: 0x040020F1 RID: 8433
		private ProjectileDamage projectileDamage;
	}
}
