using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x020004C4 RID: 1220
	public class ArrowOrb : Orb
	{
		// Token: 0x06001D5E RID: 7518 RVA: 0x0007D074 File Offset: 0x0007B274
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
			EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/ArrowOrbEffect"), effectData, true);
		}

		// Token: 0x06001D5F RID: 7519 RVA: 0x0007D0DC File Offset: 0x0007B2DC
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

		// Token: 0x04001A5C RID: 6748
		private const float speed = 60f;

		// Token: 0x04001A5D RID: 6749
		public float damageValue;

		// Token: 0x04001A5E RID: 6750
		public GameObject attacker;

		// Token: 0x04001A5F RID: 6751
		public TeamIndex teamIndex;

		// Token: 0x04001A60 RID: 6752
		public bool isCrit;

		// Token: 0x04001A61 RID: 6753
		public float scale;

		// Token: 0x04001A62 RID: 6754
		public ProcChainMask procChainMask;

		// Token: 0x04001A63 RID: 6755
		public float procCoefficient = 0.2f;

		// Token: 0x04001A64 RID: 6756
		public DamageColorIndex damageColorIndex;
	}
}
