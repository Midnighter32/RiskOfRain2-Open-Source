using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x020004C6 RID: 1222
	public class BounceOrb : Orb
	{
		// Token: 0x06001D64 RID: 7524 RVA: 0x0007D284 File Offset: 0x0007B484
		public override void Begin()
		{
			base.duration = base.distanceToTarget / 70f;
			EffectData effectData = new EffectData
			{
				scale = this.scale,
				origin = this.origin,
				genericFloat = base.duration
			};
			effectData.SetHurtBoxReference(this.target);
			EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/BounceOrbEffect"), effectData, true);
		}

		// Token: 0x06001D65 RID: 7525 RVA: 0x0007D2EC File Offset: 0x0007B4EC
		public override void OnArrival()
		{
			if (this.target)
			{
				HealthComponent healthComponent = this.target.healthComponent;
				if (healthComponent)
				{
					DamageInfo damageInfo = new DamageInfo();
					damageInfo.damage = this.damageValue;
					damageInfo.attacker = this.attacker;
					damageInfo.inflictor = null;
					damageInfo.force = Vector3.Normalize(this.target.transform.position - this.origin) * -1000f;
					damageInfo.crit = this.isCrit;
					damageInfo.procChainMask = this.procChainMask;
					damageInfo.procCoefficient = this.procCoefficient;
					damageInfo.position = this.target.transform.position;
					damageInfo.damageColorIndex = this.damageColorIndex;
					healthComponent.TakeDamage(damageInfo);
					GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
					GlobalEventManager.instance.OnHitAll(damageInfo, healthComponent.gameObject);
				}
			}
		}

		// Token: 0x06001D66 RID: 7526 RVA: 0x0007D3E4 File Offset: 0x0007B5E4
		public HurtBox PickNextTarget(Vector3 position, float range)
		{
			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			bullseyeSearch.searchOrigin = position;
			bullseyeSearch.searchDirection = Vector3.zero;
			bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
			bullseyeSearch.teamMaskFilter.RemoveTeam(this.teamIndex);
			bullseyeSearch.filterByLoS = false;
			bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
			bullseyeSearch.maxDistanceFilter = range;
			bullseyeSearch.RefreshCandidates();
			List<HurtBox> list = (from v in bullseyeSearch.GetResults()
			where !this.bouncedObjects.Contains(v.healthComponent)
			select v).ToList<HurtBox>();
			HurtBox hurtBox = (list.Count > 0) ? list[UnityEngine.Random.Range(0, list.Count)] : null;
			if (hurtBox)
			{
				this.bouncedObjects.Add(hurtBox.healthComponent);
			}
			return hurtBox;
		}

		// Token: 0x04001A66 RID: 6758
		private const float speed = 70f;

		// Token: 0x04001A67 RID: 6759
		public float damageValue;

		// Token: 0x04001A68 RID: 6760
		public GameObject attacker;

		// Token: 0x04001A69 RID: 6761
		public TeamIndex teamIndex;

		// Token: 0x04001A6A RID: 6762
		public List<HealthComponent> bouncedObjects;

		// Token: 0x04001A6B RID: 6763
		public bool isCrit;

		// Token: 0x04001A6C RID: 6764
		public float scale;

		// Token: 0x04001A6D RID: 6765
		public ProcChainMask procChainMask;

		// Token: 0x04001A6E RID: 6766
		public float procCoefficient = 0.2f;

		// Token: 0x04001A6F RID: 6767
		public DamageColorIndex damageColorIndex;
	}
}
