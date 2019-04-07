using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000546 RID: 1350
	public class ProjectileFireChildren : MonoBehaviour
	{
		// Token: 0x06001E28 RID: 7720 RVA: 0x0008E0AE File Offset: 0x0008C2AE
		private void Start()
		{
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.projectileController = base.GetComponent<ProjectileController>();
		}

		// Token: 0x06001E29 RID: 7721 RVA: 0x0008E0C8 File Offset: 0x0008C2C8
		private void Update()
		{
			this.timer += Time.deltaTime;
			this.nextSpawnTimer += Time.deltaTime;
			if (this.timer >= this.duration)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			if (this.nextSpawnTimer >= this.duration / (float)this.count)
			{
				this.nextSpawnTimer -= this.duration / (float)this.count;
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.childProjectilePrefab, base.transform.position, Util.QuaternionSafeLookRotation(base.transform.forward));
				ProjectileController component = gameObject.GetComponent<ProjectileController>();
				if (component)
				{
					component.procChainMask = this.projectileController.procChainMask;
					component.procCoefficient = this.projectileController.procCoefficient * this.childProcCoefficient;
					component.Networkowner = this.projectileController.owner;
				}
				gameObject.GetComponent<TeamFilter>().teamIndex = base.GetComponent<TeamFilter>().teamIndex;
				ProjectileDamage component2 = gameObject.GetComponent<ProjectileDamage>();
				if (component2)
				{
					component2.damage = this.projectileDamage.damage * this.childDamageCoefficient;
					component2.crit = this.projectileDamage.crit;
					component2.force = this.projectileDamage.force;
					component2.damageColorIndex = this.projectileDamage.damageColorIndex;
				}
				if (!this.ignoreParentForChainController)
				{
					ChainController component3 = gameObject.GetComponent<ChainController>();
					if (component3)
					{
						component3.pastTargetList.Add(base.transform.parent);
					}
				}
				NetworkServer.Spawn(gameObject);
			}
		}

		// Token: 0x040020A3 RID: 8355
		public float duration = 5f;

		// Token: 0x040020A4 RID: 8356
		public int count = 5;

		// Token: 0x040020A5 RID: 8357
		public GameObject childProjectilePrefab;

		// Token: 0x040020A6 RID: 8358
		private float timer;

		// Token: 0x040020A7 RID: 8359
		private float nextSpawnTimer;

		// Token: 0x040020A8 RID: 8360
		public float childDamageCoefficient = 1f;

		// Token: 0x040020A9 RID: 8361
		public float childProcCoefficient = 1f;

		// Token: 0x040020AA RID: 8362
		private ProjectileDamage projectileDamage;

		// Token: 0x040020AB RID: 8363
		private ProjectileController projectileController;

		// Token: 0x040020AC RID: 8364
		public bool ignoreParentForChainController;
	}
}
