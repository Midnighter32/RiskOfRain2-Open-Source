using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000145 RID: 325
	public class AmmoPickup : MonoBehaviour
	{
		// Token: 0x060005C6 RID: 1478 RVA: 0x00017EB8 File Offset: 0x000160B8
		private void OnTriggerStay(Collider other)
		{
			if (NetworkServer.active && this.alive && TeamComponent.GetObjectTeam(other.gameObject) == this.teamFilter.teamIndex)
			{
				SkillLocator component = other.GetComponent<SkillLocator>();
				if (component)
				{
					this.alive = false;
					component.ApplyAmmoPack();
					EffectManager.SimpleEffect(this.pickupEffect, base.transform.position, Quaternion.identity, true);
					UnityEngine.Object.Destroy(this.baseObject);
				}
			}
		}

		// Token: 0x04000641 RID: 1601
		[Tooltip("The base object to destroy when this pickup is consumed.")]
		public GameObject baseObject;

		// Token: 0x04000642 RID: 1602
		[Tooltip("The team filter object which determines who can pick up this pack.")]
		public TeamFilter teamFilter;

		// Token: 0x04000643 RID: 1603
		public GameObject pickupEffect;

		// Token: 0x04000644 RID: 1604
		private bool alive = true;
	}
}
