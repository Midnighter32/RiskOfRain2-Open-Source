using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x0200050C RID: 1292
	public class ArrowOrb : Orb
	{
		// Token: 0x06001D30 RID: 7472 RVA: 0x00087E84 File Offset: 0x00086084
		public override void Begin()
		{
			base.duration = base.distanceToTarget / 60f;
			EffectData effectData = new EffectData
			{
				scale = this.scale,
				origin = this.origin,
				genericFloat = base.duration
			};
			effectData.SetHurtBoxReference(this.target);
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/ArrowOrbEffect"), effectData, true);
		}

		// Token: 0x06001D31 RID: 7473 RVA: 0x00087EF0 File Offset: 0x000860F0
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

		// Token: 0x04001F4F RID: 8015
		private const float speed = 60f;

		// Token: 0x04001F50 RID: 8016
		public float damageValue;

		// Token: 0x04001F51 RID: 8017
		public GameObject attacker;

		// Token: 0x04001F52 RID: 8018
		public TeamIndex teamIndex;

		// Token: 0x04001F53 RID: 8019
		public bool isCrit;

		// Token: 0x04001F54 RID: 8020
		public float scale;

		// Token: 0x04001F55 RID: 8021
		public ProcChainMask procChainMask;

		// Token: 0x04001F56 RID: 8022
		public float procCoefficient = 0.2f;

		// Token: 0x04001F57 RID: 8023
		public DamageColorIndex damageColorIndex;
	}
}
