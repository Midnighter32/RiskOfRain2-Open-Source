using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200028C RID: 652
	public class MoneyPickup : MonoBehaviour
	{
		// Token: 0x06000E83 RID: 3715 RVA: 0x000406F8 File Offset: 0x0003E8F8
		private void Start()
		{
			if (NetworkServer.active)
			{
				this.goldReward = (this.shouldScale ? Run.instance.GetDifficultyScaledCost(this.baseGoldReward) : this.baseGoldReward);
			}
		}

		// Token: 0x06000E84 RID: 3716 RVA: 0x00040728 File Offset: 0x0003E928
		private void OnTriggerStay(Collider other)
		{
			if (NetworkServer.active && this.alive)
			{
				TeamIndex objectTeam = TeamComponent.GetObjectTeam(other.gameObject);
				if (objectTeam == this.teamFilter.teamIndex)
				{
					this.alive = false;
					Vector3 position = base.transform.position;
					TeamManager.instance.GiveTeamMoney(objectTeam, (uint)this.goldReward);
					if (this.pickupEffectPrefab)
					{
						EffectManager.SimpleEffect(this.pickupEffectPrefab, position, Quaternion.identity, true);
					}
					UnityEngine.Object.Destroy(this.baseObject);
				}
			}
		}

		// Token: 0x04000E66 RID: 3686
		[Tooltip("The base object to destroy when this pickup is consumed.")]
		public GameObject baseObject;

		// Token: 0x04000E67 RID: 3687
		[Tooltip("The team filter object which determines who can pick up this pack.")]
		public TeamFilter teamFilter;

		// Token: 0x04000E68 RID: 3688
		public GameObject pickupEffectPrefab;

		// Token: 0x04000E69 RID: 3689
		public int baseGoldReward;

		// Token: 0x04000E6A RID: 3690
		public bool shouldScale;

		// Token: 0x04000E6B RID: 3691
		private bool alive = true;

		// Token: 0x04000E6C RID: 3692
		private int goldReward;
	}
}
