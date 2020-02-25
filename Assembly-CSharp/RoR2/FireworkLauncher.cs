using System;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000202 RID: 514
	public class FireworkLauncher : MonoBehaviour
	{
		// Token: 0x06000AFC RID: 2812 RVA: 0x00030B14 File Offset: 0x0002ED14
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				if (this.remaining <= 0 || !this.owner)
				{
					UnityEngine.Object.Destroy(base.gameObject);
					return;
				}
				this.nextFireTimer -= Time.fixedDeltaTime;
				if (this.nextFireTimer <= 0f)
				{
					this.remaining--;
					this.nextFireTimer += this.launchInterval;
					this.FireMissile();
				}
			}
		}

		// Token: 0x06000AFD RID: 2813 RVA: 0x00030B90 File Offset: 0x0002ED90
		private void FireMissile()
		{
			CharacterBody component = this.owner.GetComponent<CharacterBody>();
			if (component)
			{
				Vector2 vector = UnityEngine.Random.insideUnitCircle * this.randomCircleRange;
				ProjectileManager.instance.FireProjectile(this.projectilePrefab, base.transform.position + new Vector3(vector.x, 0f, vector.y), Util.QuaternionSafeLookRotation(Vector3.up + new Vector3(vector.x, 0f, vector.y)), this.owner, component.damage * this.damageCoefficient, 200f, this.crit, DamageColorIndex.Item, null, -1f);
			}
		}

		// Token: 0x04000B61 RID: 2913
		public GameObject projectilePrefab;

		// Token: 0x04000B62 RID: 2914
		public float launchInterval = 0.1f;

		// Token: 0x04000B63 RID: 2915
		public float damageCoefficient = 3f;

		// Token: 0x04000B64 RID: 2916
		public float coneAngle = 10f;

		// Token: 0x04000B65 RID: 2917
		public float randomCircleRange;

		// Token: 0x04000B66 RID: 2918
		[HideInInspector]
		public GameObject owner;

		// Token: 0x04000B67 RID: 2919
		[HideInInspector]
		public TeamIndex team;

		// Token: 0x04000B68 RID: 2920
		[HideInInspector]
		public int remaining;

		// Token: 0x04000B69 RID: 2921
		[HideInInspector]
		public bool crit;

		// Token: 0x04000B6A RID: 2922
		private float nextFireTimer;
	}
}
