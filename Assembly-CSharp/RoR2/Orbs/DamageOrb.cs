using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x0200050F RID: 1295
	public class DamageOrb : Orb
	{
		// Token: 0x06001D3B RID: 7483 RVA: 0x000882DC File Offset: 0x000864DC
		public override void Begin()
		{
			GameObject effectPrefab = null;
			if (this.damageOrbType == DamageOrb.DamageOrbType.ClayGooOrb)
			{
				this.speed = 5f;
				effectPrefab = Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/ClayGooOrbEffect");
				this.orbDamageType = DamageType.ClayGoo;
			}
			base.duration = base.distanceToTarget / this.speed;
			EffectData effectData = new EffectData
			{
				origin = this.origin,
				genericFloat = base.duration
			};
			effectData.SetHurtBoxReference(this.target);
			EffectManager.instance.SpawnEffect(effectPrefab, effectData, true);
		}

		// Token: 0x06001D3C RID: 7484 RVA: 0x00088360 File Offset: 0x00086560
		public override void OnArrival()
		{
			if (this.target)
			{
				HealthComponent healthComponent = this.target.healthComponent;
				if (healthComponent)
				{
					if (this.damageOrbType == DamageOrb.DamageOrbType.ClayGooOrb)
					{
						CharacterBody component = healthComponent.GetComponent<CharacterBody>();
						if (component && (component.bodyFlags & CharacterBody.BodyFlags.ImmuneToGoo) != CharacterBody.BodyFlags.None)
						{
							healthComponent.Heal(this.damageValue, default(ProcChainMask), true);
							return;
						}
					}
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
					damageInfo.damageType = this.orbDamageType;
					healthComponent.TakeDamage(damageInfo);
					GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
					GlobalEventManager.instance.OnHitAll(damageInfo, healthComponent.gameObject);
				}
			}
		}

		// Token: 0x04001F63 RID: 8035
		private float speed = 60f;

		// Token: 0x04001F64 RID: 8036
		public float damageValue;

		// Token: 0x04001F65 RID: 8037
		public GameObject attacker;

		// Token: 0x04001F66 RID: 8038
		public TeamIndex teamIndex;

		// Token: 0x04001F67 RID: 8039
		public bool isCrit;

		// Token: 0x04001F68 RID: 8040
		public ProcChainMask procChainMask;

		// Token: 0x04001F69 RID: 8041
		public float procCoefficient = 0.2f;

		// Token: 0x04001F6A RID: 8042
		public DamageColorIndex damageColorIndex;

		// Token: 0x04001F6B RID: 8043
		public DamageOrb.DamageOrbType damageOrbType;

		// Token: 0x04001F6C RID: 8044
		private DamageType orbDamageType;

		// Token: 0x02000510 RID: 1296
		public enum DamageOrbType
		{
			// Token: 0x04001F6E RID: 8046
			ClayGooOrb
		}
	}
}
