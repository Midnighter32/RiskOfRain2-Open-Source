using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Projectile
{
	// Token: 0x02000504 RID: 1284
	public class ProjectileFireChildren : MonoBehaviour
	{
		// Token: 0x06001E7B RID: 7803 RVA: 0x000838F2 File Offset: 0x00081AF2
		private void Start()
		{
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.projectileController = base.GetComponent<ProjectileController>();
		}

		// Token: 0x06001E7C RID: 7804 RVA: 0x0008390C File Offset: 0x00081B0C
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

		// Token: 0x04001BD7 RID: 7127
		public float duration = 5f;

		// Token: 0x04001BD8 RID: 7128
		public int count = 5;

		// Token: 0x04001BD9 RID: 7129
		public GameObject childProjectilePrefab;

		// Token: 0x04001BDA RID: 7130
		private float timer;

		// Token: 0x04001BDB RID: 7131
		private float nextSpawnTimer;

		// Token: 0x04001BDC RID: 7132
		public float childDamageCoefficient = 1f;

		// Token: 0x04001BDD RID: 7133
		public float childProcCoefficient = 1f;

		// Token: 0x04001BDE RID: 7134
		private ProjectileDamage projectileDamage;

		// Token: 0x04001BDF RID: 7135
		private ProjectileController projectileController;

		// Token: 0x04001BE0 RID: 7136
		public bool ignoreParentForChainController;
	}
}
