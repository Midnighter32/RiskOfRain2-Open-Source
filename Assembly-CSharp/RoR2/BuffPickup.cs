using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000270 RID: 624
	public class BuffPickup : MonoBehaviour
	{
		// Token: 0x06000BBC RID: 3004 RVA: 0x000395E8 File Offset: 0x000377E8
		private void OnTriggerStay(Collider other)
		{
			if (NetworkServer.active && this.alive && TeamComponent.GetObjectTeam(other.gameObject) == this.teamFilter.teamIndex)
			{
				CharacterBody component = other.GetComponent<CharacterBody>();
				if (component)
				{
					component.AddTimedBuff(this.buffIndex, this.buffDuration);
					UnityEngine.Object.Instantiate<GameObject>(this.pickupEffect, other.transform.position, Quaternion.identity);
					UnityEngine.Object.Destroy(this.baseObject);
				}
			}
		}

		// Token: 0x04000FA5 RID: 4005
		[Tooltip("The base object to destroy when this pickup is consumed.")]
		public GameObject baseObject;

		// Token: 0x04000FA6 RID: 4006
		[Tooltip("The team filter object which determines who can pick up this pack.")]
		public TeamFilter teamFilter;

		// Token: 0x04000FA7 RID: 4007
		public GameObject pickupEffect;

		// Token: 0x04000FA8 RID: 4008
		public BuffIndex buffIndex;

		// Token: 0x04000FA9 RID: 4009
		public float buffDuration;

		// Token: 0x04000FAA RID: 4010
		private bool alive = true;
	}
}
