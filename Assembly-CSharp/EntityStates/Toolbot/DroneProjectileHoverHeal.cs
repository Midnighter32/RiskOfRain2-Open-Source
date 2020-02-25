using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Toolbot
{
	// Token: 0x0200076B RID: 1899
	public class DroneProjectileHoverHeal : DroneProjectileHover
	{
		// Token: 0x06002BCB RID: 11211 RVA: 0x000B93C8 File Offset: 0x000B75C8
		protected override void Pulse()
		{
			float num = 1f;
			ProjectileDamage component = base.GetComponent<ProjectileDamage>();
			if (component)
			{
				num = component.damage;
			}
			this.HealOccupants(this.pulseRadius, DroneProjectileHoverHeal.healPointsCoefficient * num, DroneProjectileHoverHeal.healFraction);
			EffectData effectData = new EffectData();
			effectData.origin = base.transform.position;
			effectData.scale = this.pulseRadius;
			EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/ExplosionVFX"), effectData, true);
		}

		// Token: 0x06002BCC RID: 11212 RVA: 0x000B9440 File Offset: 0x000B7640
		private static HealthComponent SelectHealthComponent(Collider collider)
		{
			HurtBox component = collider.GetComponent<HurtBox>();
			if (component && component.healthComponent)
			{
				return component.healthComponent;
			}
			return null;
		}

		// Token: 0x06002BCD RID: 11213 RVA: 0x000B9474 File Offset: 0x000B7674
		private void HealOccupants(float radius, float healPoints, float healFraction)
		{
			IEnumerable<Collider> source = Physics.OverlapSphere(base.transform.position, radius, LayerIndex.entityPrecise.mask);
			TeamIndex teamIndex = this.teamFilter ? this.teamFilter.teamIndex : TeamIndex.None;
			IEnumerable<HealthComponent> source2 = source.Select(new Func<Collider, HealthComponent>(DroneProjectileHoverHeal.SelectHealthComponent));
			Func<HealthComponent, bool> <>9__0;
			Func<HealthComponent, bool> predicate;
			if ((predicate = <>9__0) == null)
			{
				predicate = (<>9__0 = ((HealthComponent healthComponent) => healthComponent && healthComponent.body.teamComponent.teamIndex == teamIndex));
			}
			foreach (HealthComponent healthComponent2 in source2.Where(predicate).Distinct<HealthComponent>())
			{
				float num = healPoints + healthComponent2.fullHealth * healFraction;
				if (num > 0f)
				{
					healthComponent2.Heal(num, default(ProcChainMask), true);
				}
			}
		}

		// Token: 0x040027F7 RID: 10231
		public static float healPointsCoefficient;

		// Token: 0x040027F8 RID: 10232
		public static float healFraction;
	}
}
