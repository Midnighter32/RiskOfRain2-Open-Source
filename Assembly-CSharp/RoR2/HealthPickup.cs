using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200022C RID: 556
	public class HealthPickup : MonoBehaviour
	{
		// Token: 0x06000C72 RID: 3186 RVA: 0x0003822C File Offset: 0x0003642C
		private void OnTriggerStay(Collider other)
		{
			if (NetworkServer.active && this.alive && TeamComponent.GetObjectTeam(other.gameObject) == this.teamFilter.teamIndex)
			{
				CharacterBody component = other.GetComponent<CharacterBody>();
				if (component)
				{
					HealthComponent healthComponent = component.healthComponent;
					if (healthComponent)
					{
						component.healthComponent.Heal(this.flatHealing + healthComponent.fullHealth * this.fractionalHealing, default(ProcChainMask), true);
						EffectManager.SpawnEffect(this.pickupEffect, new EffectData
						{
							origin = base.transform.position
						}, true);
					}
					UnityEngine.Object.Destroy(this.baseObject);
				}
			}
		}

		// Token: 0x04000C5F RID: 3167
		[Tooltip("The base object to destroy when this pickup is consumed.")]
		public GameObject baseObject;

		// Token: 0x04000C60 RID: 3168
		[Tooltip("The team filter object which determines who can pick up this pack.")]
		public TeamFilter teamFilter;

		// Token: 0x04000C61 RID: 3169
		public GameObject pickupEffect;

		// Token: 0x04000C62 RID: 3170
		public float flatHealing;

		// Token: 0x04000C63 RID: 3171
		public float fractionalHealing;

		// Token: 0x04000C64 RID: 3172
		private bool alive = true;
	}
}
