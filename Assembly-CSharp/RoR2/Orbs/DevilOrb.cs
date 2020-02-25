using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x020004C9 RID: 1225
	public class DevilOrb : Orb
	{
		// Token: 0x06001D6C RID: 7532 RVA: 0x0007D67C File Offset: 0x0007B87C
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
			GameObject effectPrefab = null;
			DevilOrb.EffectType effectType = this.effectType;
			if (effectType != DevilOrb.EffectType.Skull)
			{
				if (effectType == DevilOrb.EffectType.Wisp)
				{
					effectPrefab = Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/WispOrbEffect");
				}
			}
			else
			{
				effectPrefab = Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/DevilOrbEffect");
			}
			EffectManager.SpawnEffect(effectPrefab, effectData, true);
		}

		// Token: 0x06001D6D RID: 7533 RVA: 0x0007D704 File Offset: 0x0007B904
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

		// Token: 0x06001D6E RID: 7534 RVA: 0x0007D7D8 File Offset: 0x0007B9D8
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

		// Token: 0x04001A7C RID: 6780
		private const float speed = 30f;

		// Token: 0x04001A7D RID: 6781
		public float damageValue;

		// Token: 0x04001A7E RID: 6782
		public GameObject attacker;

		// Token: 0x04001A7F RID: 6783
		public TeamIndex teamIndex;

		// Token: 0x04001A80 RID: 6784
		public bool isCrit;

		// Token: 0x04001A81 RID: 6785
		public float scale;

		// Token: 0x04001A82 RID: 6786
		public ProcChainMask procChainMask;

		// Token: 0x04001A83 RID: 6787
		public float procCoefficient = 0.2f;

		// Token: 0x04001A84 RID: 6788
		public DamageColorIndex damageColorIndex;

		// Token: 0x04001A85 RID: 6789
		public DevilOrb.EffectType effectType;

		// Token: 0x020004CA RID: 1226
		public enum EffectType
		{
			// Token: 0x04001A87 RID: 6791
			Skull,
			// Token: 0x04001A88 RID: 6792
			Wisp
		}
	}
}
