using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000547 RID: 1351
	public class ProjectileFireEffects : MonoBehaviour
	{
		// Token: 0x06001E2B RID: 7723 RVA: 0x0008E28C File Offset: 0x0008C48C
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
				if (this.effectPrefab)
				{
					Vector3 b = new Vector3(UnityEngine.Random.Range(-this.randomOffset.x, this.randomOffset.x), UnityEngine.Random.Range(-this.randomOffset.y, this.randomOffset.y), UnityEngine.Random.Range(-this.randomOffset.z, this.randomOffset.z));
					EffectManager.instance.SimpleImpactEffect(this.effectPrefab, base.transform.position + b, Vector3.forward, true);
				}
			}
		}

		// Token: 0x040020AD RID: 8365
		public float duration = 5f;

		// Token: 0x040020AE RID: 8366
		public int count = 5;

		// Token: 0x040020AF RID: 8367
		public GameObject effectPrefab;

		// Token: 0x040020B0 RID: 8368
		public Vector3 randomOffset;

		// Token: 0x040020B1 RID: 8369
		private float timer;

		// Token: 0x040020B2 RID: 8370
		private float nextSpawnTimer;
	}
}
