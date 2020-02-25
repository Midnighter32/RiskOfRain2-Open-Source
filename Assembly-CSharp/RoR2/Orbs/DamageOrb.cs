using System;
using UnityEngine;

namespace RoR2.Orbs
{
	// Token: 0x020004C7 RID: 1223
	public class DamageOrb : Orb
	{
		// Token: 0x06001D69 RID: 7529 RVA: 0x0007D4C0 File Offset: 0x0007B6C0
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
			EffectManager.SpawnEffect(effectPrefab, effectData, true);
		}

		// Token: 0x06001D6A RID: 7530 RVA: 0x0007D540 File Offset: 0x0007B740
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

		// Token: 0x04001A70 RID: 6768
		private float speed = 60f;

		// Token: 0x04001A71 RID: 6769
		public float damageValue;

		// Token: 0x04001A72 RID: 6770
		public GameObject attacker;

		// Token: 0x04001A73 RID: 6771
		public TeamIndex teamIndex;

		// Token: 0x04001A74 RID: 6772
		public bool isCrit;

		// Token: 0x04001A75 RID: 6773
		public ProcChainMask procChainMask;

		// Token: 0x04001A76 RID: 6774
		public float procCoefficient = 0.2f;

		// Token: 0x04001A77 RID: 6775
		public DamageColorIndex damageColorIndex;

		// Token: 0x04001A78 RID: 6776
		public DamageOrb.DamageOrbType damageOrbType;

		// Token: 0x04001A79 RID: 6777
		private DamageType orbDamageType;

		// Token: 0x020004C8 RID: 1224
		public enum DamageOrbType
		{
			// Token: 0x04001A7B RID: 6779
			ClayGooOrb
		}
	}
}
