using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x02000511 RID: 1297
	public class DevilOrb : Orb
	{
		// Token: 0x06001D3E RID: 7486 RVA: 0x0008849C File Offset: 0x0008669C
		public override void Begin()
		{
			base.duration = base.distanceToTarget / 30f;
			EffectData effectData = new EffectData
			{
				scale = this.scale,
				origin = this.origin,
				genericFloat = base.duration
			};
			effectData.SetHurtBoxReference(this.target);
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/DevilOrbEffect"), effectData, true);
		}

		// Token: 0x06001D3F RID: 7487 RVA: 0x00088508 File Offset: 0x00086708
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
					damageInfo.force = Vector3.zero;
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

		// Token: 0x06001D40 RID: 7488 RVA: 0x000885DC File Offset: 0x000867DC
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
			List<HurtBox> list = bullseyeSearch.GetResults().ToList<HurtBox>();
			if (list.Count <= 0)
			{
				return null;
			}
			return list[UnityEngine.Random.Range(0, list.Count)];
		}

		// Token: 0x04001F6F RID: 8047
		private const float speed = 30f;

		// Token: 0x04001F70 RID: 8048
		public float damageValue;

		// Token: 0x04001F71 RID: 8049
		public GameObject attacker;

		// Token: 0x04001F72 RID: 8050
		public TeamIndex teamIndex;

		// Token: 0x04001F73 RID: 8051
		public bool isCrit;

		// Token: 0x04001F74 RID: 8052
		public float scale;

		// Token: 0x04001F75 RID: 8053
		public ProcChainMask procChainMask;

		// Token: 0x04001F76 RID: 8054
		public float procCoefficient = 0.2f;

		// Token: 0x04001F77 RID: 8055
		public DamageColorIndex damageColorIndex;
	}
}
