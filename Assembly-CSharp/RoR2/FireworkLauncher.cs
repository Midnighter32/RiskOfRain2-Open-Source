using System;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002F3 RID: 755
	public class FireworkLauncher : MonoBehaviour
	{
		// Token: 0x06000F45 RID: 3909 RVA: 0x0004B6F4 File Offset: 0x000498F4
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

		// Token: 0x06000F46 RID: 3910 RVA: 0x0004B770 File Offset: 0x00049970
		private void FireMissile()
		{
			CharacterBody component = this.owner.GetComponent<CharacterBody>();
			if (component)
			{
				Vector2 vector = UnityEngine.Random.insideUnitCircle * this.randomCircleRange;
				ProjectileManager.instance.FireProjectile(this.projectilePrefab, base.transform.position + new Vector3(vector.x, 0f, vector.y), Util.QuaternionSafeLookRotation(Vector3.up + new Vector3(vector.x, 0f, vector.y)), this.owner, component.damage * this.damageCoefficient, 200f, this.crit, DamageColorIndex.Item, null, -1f);
			}
		}

		// Token: 0x04001362 RID: 4962
		public GameObject projectilePrefab;

		// Token: 0x04001363 RID: 4963
		public float launchInterval = 0.1f;

		// Token: 0x04001364 RID: 4964
		public float damageCoefficient = 3f;

		// Token: 0x04001365 RID: 4965
		public float coneAngle = 10f;

		// Token: 0x04001366 RID: 4966
		public float randomCircleRange;

		// Token: 0x04001367 RID: 4967
		[HideInInspector]
		public GameObject owner;

		// Token: 0x04001368 RID: 4968
		[HideInInspector]
		public TeamIndex team;

		// Token: 0x04001369 RID: 4969
		[HideInInspector]
		public int remaining;

		// Token: 0x0400136A RID: 4970
		[HideInInspector]
		public bool crit;

		// Token: 0x0400136B RID: 4971
		private float nextFireTimer;
	}
}
