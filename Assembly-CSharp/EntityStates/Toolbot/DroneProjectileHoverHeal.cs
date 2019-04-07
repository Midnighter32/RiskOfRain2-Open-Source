using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Toolbot
{
	// Token: 0x020000E4 RID: 228
	public class DroneProjectileHoverHeal : DroneProjectileHover
	{
		// Token: 0x06000471 RID: 1137 RVA: 0x00012AD4 File Offset: 0x00010CD4
		protected override void Pulse()
		{
			float num = 1f;
			ProjectileDamage component = base.GetComponent<ProjectileDamage>();
			if (component)
			{
				num = component.damage;
			}
			this.HealOccupants(DroneProjectileHover.pulseRadius, DroneProjectileHoverHeal.healPointsCoefficient * num, DroneProjectileHoverHeal.healFraction);
			EffectData effectData = new EffectData();
			effectData.origin = base.transform.position;
			effectData.scale = DroneProjectileHover.pulseRadius;
			EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/ExplosionVFX"), effectData, true);
		}

		// Token: 0x06000472 RID: 1138 RVA: 0x00012B4C File Offset: 0x00010D4C
		private static HealthComponent SelectHealthComponent(Collider collider)
		{
			HurtBox component = collider.GetComponent<HurtBox>();
			if (component && component.healthComponent)
			{
				return component.healthComponent;
			}
			return null;
		}

		// Token: 0x06000473 RID: 1139 RVA: 0x00012B80 File Offset: 0x00010D80
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

		// Token: 0x0400043D RID: 1085
		public static float healPointsCoefficient;

		// Token: 0x0400043E RID: 1086
		public static float healFraction;
	}
}
