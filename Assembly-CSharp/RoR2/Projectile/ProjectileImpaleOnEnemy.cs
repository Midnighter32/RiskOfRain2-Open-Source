using System;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000516 RID: 1302
	[RequireComponent(typeof(ProjectileController))]
	public class ProjectileImpaleOnEnemy : MonoBehaviour, IProjectileImpactBehavior
	{
		// Token: 0x06001EC2 RID: 7874 RVA: 0x00085348 File Offset: 0x00083548
		private void Awake()
		{
			this.rigidbody = base.GetComponent<Rigidbody>();
		}

		// Token: 0x06001EC3 RID: 7875 RVA: 0x00085358 File Offset: 0x00083558
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

		// Token: 0x04001C4C RID: 7244
		private bool alive = true;

		// Token: 0x04001C4D RID: 7245
		public GameObject impalePrefab;

		// Token: 0x04001C4E RID: 7246
		private Rigidbody rigidbody;
	}
}
