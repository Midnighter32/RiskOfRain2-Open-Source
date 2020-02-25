using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2.Projectile
{
	// Token: 0x02000529 RID: 1321
	[RequireComponent(typeof(Collider))]
	public class SlowDownProjectiles : MonoBehaviour
	{
		// Token: 0x06001F49 RID: 8009 RVA: 0x00087E38 File Offset: 0x00086038
		private void Start()
		{
			this.affectedRigidbodies = new List<Rigidbody>();
		}

		// Token: 0x06001F4A RID: 8010 RVA: 0x0000409B File Offset: 0x0000229B
		private void Update()
		{
		}

		// Token: 0x06001F4B RID: 8011 RVA: 0x00087E48 File Offset: 0x00086048
		private void OnTriggerEnter(Collider other)
		{
			other.GetComponent<TeamFilter>();
			Rigidbody component = other.GetComponent<Rigidbody>();
			if (component)
			{
				this.affectedRigidbodies.Add(component);
			}
		}

		// Token: 0x06001F4C RID: 8012 RVA: 0x00087E78 File Offset: 0x00086078
		private void OnTriggerExit(Collider other)
		{
			Rigidbody component = other.GetComponent<Rigidbody>();
			ProjectileSimple component2 = other.GetComponent<ProjectileSimple>();
			if (component)
			{
				this.affectedRigidbodies.Remove(component);
			}
			if (component2 && !component2.enableVelocityOverLifetime)
			{
				component.velocity = component.transform.forward * component2.velocity;
			}
		}

		// Token: 0x06001F4D RID: 8013 RVA: 0x00087ED4 File Offset: 0x000860D4
		private void FixedUpdate()
		{
			for (int i = 0; i < this.affectedRigidbodies.Count; i++)
			{
				Rigidbody rigidbody = this.affectedRigidbodies[i];
				if (rigidbody)
				{
					if (rigidbody.velocity.magnitude > this.maxVelocityMagnitude)
					{
						rigidbody.velocity = rigidbody.velocity.normalized * this.maxVelocityMagnitude;
					}
					if (rigidbody.useGravity)
					{
						rigidbody.velocity += Physics.gravity * this.antiGravity * Time.fixedDeltaTime;
					}
				}
				else
				{
					this.affectedRigidbodies.Remove(rigidbody);
				}
			}
		}

		// Token: 0x04001CF3 RID: 7411
		public float maxVelocityMagnitude;

		// Token: 0x04001CF4 RID: 7412
		public TeamFilter teamFilter;

		// Token: 0x04001CF5 RID: 7413
		public float antiGravity;

		// Token: 0x04001CF6 RID: 7414
		private List<Rigidbody> affectedRigidbodies;
	}
}
