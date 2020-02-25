using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001E8 RID: 488
	[RequireComponent(typeof(SphereCollider))]
	public class DisableCollisionsIfInTrigger : MonoBehaviour
	{
		// Token: 0x06000A33 RID: 2611 RVA: 0x0002C8E9 File Offset: 0x0002AAE9
		public void Awake()
		{
			this.trigger = base.GetComponent<SphereCollider>();
		}

		// Token: 0x06000A34 RID: 2612 RVA: 0x0002C8F8 File Offset: 0x0002AAF8
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

		// Token: 0x04000A93 RID: 2707
		public Collider colliderToIgnore;

		// Token: 0x04000A94 RID: 2708
		private SphereCollider trigger;
	}
}
