using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000518 RID: 1304
	[RequireComponent(typeof(ProjectileController))]
	[RequireComponent(typeof(ProjectileDamage))]
	public class ProjectileIntervalOverlapAttack : MonoBehaviour
	{
		// Token: 0x06001EC7 RID: 7879 RVA: 0x00085432 File Offset: 0x00083632
		private void Awake()
		{
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
		}

		// Token: 0x06001EC8 RID: 7880 RVA: 0x0008544C File Offset: 0x0008364C
		private void Start()
		{
			this.countdown = 0f;
		}

		// Token: 0x06001EC9 RID: 7881 RVA: 0x0008545C File Offset: 0x0008365C
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

		// Token: 0x04001C51 RID: 7249
		public HitBoxGroup hitBoxGroup;

		// Token: 0x04001C52 RID: 7250
		public float interval;

		// Token: 0x04001C53 RID: 7251
		public float damageCoefficient = 1f;

		// Token: 0x04001C54 RID: 7252
		private float countdown;

		// Token: 0x04001C55 RID: 7253
		private ProjectileController projectileController;

		// Token: 0x04001C56 RID: 7254
		private ProjectileDamage projectileDamage;
	}
}
