using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000505 RID: 1285
	public class ProjectileFireEffects : MonoBehaviour
	{
		// Token: 0x06001E7E RID: 7806 RVA: 0x00083AD0 File Offset: 0x00081CD0
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
					EffectManager.SimpleImpactEffect(this.effectPrefab, base.transform.position + b, Vector3.forward, true);
				}
			}
		}

		// Token: 0x04001BE1 RID: 7137
		public float duration = 5f;

		// Token: 0x04001BE2 RID: 7138
		public int count = 5;

		// Token: 0x04001BE3 RID: 7139
		public GameObject effectPrefab;

		// Token: 0x04001BE4 RID: 7140
		public Vector3 randomOffset;

		// Token: 0x04001BE5 RID: 7141
		private float timer;

		// Token: 0x04001BE6 RID: 7142
		private float nextSpawnTimer;
	}
}
