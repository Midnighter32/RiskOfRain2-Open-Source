using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002D6 RID: 726
	[RequireComponent(typeof(SphereCollider))]
	public class DisableCollisionsIfInTrigger : MonoBehaviour
	{
		// Token: 0x06000E87 RID: 3719 RVA: 0x000479B5 File Offset: 0x00045BB5
		public void Awake()
		{
			this.trigger = base.GetComponent<SphereCollider>();
		}

		// Token: 0x06000E88 RID: 3720 RVA: 0x000479C4 File Offset: 0x00045BC4
		private void OnTriggerEnter(Collider other)
		{
			if (this.trigger)
			{
				Vector3 position = base.transform.position;
				Vector3 position2 = other.transform.position;
				float num = this.trigger.radius * this.trigger.radius;
				if ((position - position2).sqrMagnitude < num)
				{
					Physics.IgnoreCollision(this.colliderToIgnore, other);
				}
			}
		}

		// Token: 0x0400129E RID: 4766
		public Collider colliderToIgnore;

		// Token: 0x0400129F RID: 4767
		private SphereCollider trigger;
	}
}
