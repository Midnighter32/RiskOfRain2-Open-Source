using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x0200050E RID: 1294
	public class BounceOrb : Orb
	{
		// Token: 0x06001D36 RID: 7478 RVA: 0x0008809C File Offset: 0x0008629C
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
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/BounceOrbEffect"), effectData, true);
		}

		// Token: 0x06001D37 RID: 7479 RVA: 0x00088108 File Offset: 0x00086308
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

		// Token: 0x06001D38 RID: 7480 RVA: 0x00088200 File Offset: 0x00086400
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

		// Token: 0x04001F59 RID: 8025
		private const float speed = 70f;

		// Token: 0x04001F5A RID: 8026
		public float damageValue;

		// Token: 0x04001F5B RID: 8027
		public GameObject attacker;

		// Token: 0x04001F5C RID: 8028
		public TeamIndex teamIndex;

		// Token: 0x04001F5D RID: 8029
		public List<HealthComponent> bouncedObjects;

		// Token: 0x04001F5E RID: 8030
		public bool isCrit;

		// Token: 0x04001F5F RID: 8031
		public float scale;

		// Token: 0x04001F60 RID: 8032
		public ProcChainMask procChainMask;

		// Token: 0x04001F61 RID: 8033
		public float procCoefficient = 0.2f;

		// Token: 0x04001F62 RID: 8034
		public DamageColorIndex damageColorIndex;
	}
}
