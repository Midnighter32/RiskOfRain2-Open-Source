using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200030B RID: 779
	public class HealthPickup : MonoBehaviour
	{
		// Token: 0x0600102C RID: 4140 RVA: 0x000513EC File Offset: 0x0004F5EC
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
						EffectManager.instance.SpawnEffect(this.pickupEffect, new EffectData
						{
							origin = base.transform.position
						}, true);
					}
					UnityEngine.Object.Destroy(this.baseObject);
				}
			}
		}

		// Token: 0x0400141D RID: 5149
		[Tooltip("The base object to destroy when this pickup is consumed.")]
		public GameObject baseObject;

		// Token: 0x0400141E RID: 5150
		[Tooltip("The team filter object which determines who can pick up this pack.")]
		public TeamFilter teamFilter;

		// Token: 0x0400141F RID: 5151
		public GameObject pickupEffect;

		// Token: 0x04001420 RID: 5152
		public float flatHealing;

		// Token: 0x04001421 RID: 5153
		public float fractionalHealing;

		// Token: 0x04001422 RID: 5154
		private bool alive = true;
	}
}
