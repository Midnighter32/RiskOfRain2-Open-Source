using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000168 RID: 360
	public class BuffPickup : MonoBehaviour
	{
		// Token: 0x060006AF RID: 1711 RVA: 0x0001B440 File Offset: 0x00019640
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

		// Token: 0x040006F7 RID: 1783
		[Tooltip("The base object to destroy when this pickup is consumed.")]
		public GameObject baseObject;

		// Token: 0x040006F8 RID: 1784
		[Tooltip("The team filter object which determines who can pick up this pack.")]
		public TeamFilter teamFilter;

		// Token: 0x040006F9 RID: 1785
		public GameObject pickupEffect;

		// Token: 0x040006FA RID: 1786
		public BuffIndex buffIndex;

		// Token: 0x040006FB RID: 1787
		public float buffDuration;

		// Token: 0x040006FC RID: 1788
		private bool alive = true;
	}
}
