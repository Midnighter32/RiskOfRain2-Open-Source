using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200021D RID: 541
	public class GravitatePickup : MonoBehaviour
	{
		// Token: 0x06000BF4 RID: 3060 RVA: 0x0000409B File Offset: 0x0000229B
		private void Start()
		{
		}

		// Token: 0x06000BF5 RID: 3061 RVA: 0x000357D4 File Offset: 0x000339D4
		private void OnTriggerEnter(Collider other)
		{
			if (NetworkServer.active && !this.gravitateTarget && this.teamFilter.teamIndex != TeamIndex.None && TeamComponent.GetObjectTeam(other.gameObject) == this.teamFilter.teamIndex)
			{
				this.gravitateTarget = other.gameObject.transform;
			}
		}

		// Token: 0x06000BF6 RID: 3062 RVA: 0x0003582C File Offset: 0x00033A2C
		private void FixedUpdate()
		{
			if (this.gravitateTarget)
			{
				this.rigidbody.velocity = Vector3.MoveTowards(this.rigidbody.velocity, (this.gravitateTarget.transform.position - base.transform.position).normalized * this.maxSpeed, this.acceleration);
			}
		}

		// Token: 0x04000C00 RID: 3072
		private Transform gravitateTarget;

		// Token: 0x04000C01 RID: 3073
		[Tooltip("The rigidbody to set the velocity of.")]
		public Rigidbody rigidbody;

		// Token: 0x04000C02 RID: 3074
		[Tooltip("The TeamFilter which controls which team can activate this trigger.")]
		public TeamFilter teamFilter;

		// Token: 0x04000C03 RID: 3075
		public float acceleration;

		// Token: 0x04000C04 RID: 3076
		public float maxSpeed;
	}
}
