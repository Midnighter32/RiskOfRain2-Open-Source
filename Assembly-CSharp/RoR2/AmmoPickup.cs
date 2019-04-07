using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000254 RID: 596
	public class AmmoPickup : MonoBehaviour
	{
		// Token: 0x06000B1E RID: 2846 RVA: 0x0003738C File Offset: 0x0003558C
		private void OnTriggerStay(Collider other)
		{
			if (NetworkServer.active && this.alive && TeamComponent.GetObjectTeam(other.gameObject) == this.teamFilter.teamIndex)
			{
				SkillLocator component = other.GetComponent<SkillLocator>();
				if (component)
				{
					this.alive = false;
					component.ApplyAmmoPack();
					EffectManager.instance.SimpleEffect(this.pickupEffect, base.transform.position, Quaternion.identity, true);
					UnityEngine.Object.Destroy(this.baseObject);
				}
			}
		}

		// Token: 0x04000F24 RID: 3876
		[Tooltip("The base object to destroy when this pickup is consumed.")]
		public GameObject baseObject;

		// Token: 0x04000F25 RID: 3877
		[Tooltip("The team filter object which determines who can pick up this pack.")]
		public TeamFilter teamFilter;

		// Token: 0x04000F26 RID: 3878
		public GameObject pickupEffect;

		// Token: 0x04000F27 RID: 3879
		private bool alive = true;
	}
}
