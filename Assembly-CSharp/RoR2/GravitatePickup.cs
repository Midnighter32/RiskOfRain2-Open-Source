using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000305 RID: 773
	public class GravitatePickup : MonoBehaviour
	{
		// Token: 0x06000FE7 RID: 4071 RVA: 0x0004F9AB File Offset: 0x0004DBAB
		private void Start()
		{
			this.teamFilter.teamIndex = TeamIndex.Player;
		}

		// Token: 0x06000FE8 RID: 4072 RVA: 0x0004F9B9 File Offset: 0x0004DBB9
		private void OnTriggerEnter(Collider other)
		{
			if (NetworkServer.active && !this.gravitateTarget && TeamComponent.GetObjectTeam(other.gameObject) == this.teamFilter.teamIndex)
			{
				this.gravitateTarget = other.gameObject.transform;
			}
		}

		// Token: 0x06000FE9 RID: 4073 RVA: 0x0004F9F8 File Offset: 0x0004DBF8
		private void FixedUpdate()
		{
			if (this.gravitateTarget)
			{
				this.rigidbody.velocity = Vector3.MoveTowards(this.rigidbody.velocity, (this.gravitateTarget.transform.position - base.transform.position).normalized * this.maxSpeed, this.acceleration);
			}
		}

		// Token: 0x040013EA RID: 5098
		private Transform gravitateTarget;

		// Token: 0x040013EB RID: 5099
		[Tooltip("The rigidbody to set the velocity of.")]
		public Rigidbody rigidbody;

		// Token: 0x040013EC RID: 5100
		[Tooltip("The TeamFilter which controls which team can activate this trigger.")]
		public TeamFilter teamFilter;

		// Token: 0x040013ED RID: 5101
		public float acceleration;

		// Token: 0x040013EE RID: 5102
		public float maxSpeed;
	}
}
