using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000550 RID: 1360
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileImpaleOnEnemy : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001E4C RID: 7756 RVA: 0x0008EE98 File Offset: 0x0008D098
		private void Awake()
		{
			this.rigidbody = base.GetComponent<Rigidbody>();
		}

		// Token: 0x06001E4D RID: 7757 RVA: 0x0008EEA8 File Offset: 0x0008D0A8
		public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
		{
			if (!this.alive)
			{
				return;
			}
			Collider collider = impactInfo.collider;
			if (collider)
			{
				HurtBox component = collider.GetComponent<HurtBox>();
				if (component)
				{
					Vector3 position = base.transform.position;
					Vector3 estimatedPointOfImpact = impactInfo.estimatedPointOfImpact;
					Quaternion identity = Quaternion.identity;
					if (this.rigidbody)
					{
						Util.QuaternionSafeLookRotation(this.rigidbody.velocity);
					}
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.impalePrefab, component.transform);
					gameObject.transform.position = estimatedPointOfImpact;
					gameObject.transform.rotation = base.transform.rotation;
				}
			}
		}

		// Token: 0x040020E9 RID: 8425
		private bool alive = true;

		// Token: 0x040020EA RID: 8426
		public GameObject impalePrefab;

		// Token: 0x040020EB RID: 8427
		private Rigidbody rigidbody;
	}
}
