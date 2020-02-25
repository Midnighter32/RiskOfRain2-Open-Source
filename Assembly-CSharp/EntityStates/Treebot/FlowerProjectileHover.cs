using System;
using System.Collections.Generic;
using EntityStates.Toolbot;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Treebot
{
	// Token: 0x02000749 RID: 1865
	public class FlowerProjectileHover : DroneProjectileHover
	{
		// Token: 0x06002B3D RID: 11069 RVA: 0x000B6378 File Offset: 0x000B4578
		public override void OnEnter()
		{
			base.OnEnter();
			ProjectileController component = base.GetComponent<ProjectileController>();
			if (component)
			{
				this.owner = component.owner;
				this.procChainMask = component.procChainMask;
				this.procCoefficient = component.procCoefficient;
				this.teamIndex = component.teamFilter.teamIndex;
			}
			ProjectileDamage component2 = base.GetComponent<ProjectileDamage>();
			if (component2)
			{
				this.damage = component2.damage;
				this.damageType = component2.damageType;
				this.crit = component2.crit;
			}
		}

		// Token: 0x06002B3E RID: 11070 RVA: 0x000B6404 File Offset: 0x000B4604
		private void FirstPulse()
		{
			Vector3 position = base.transform.position;
			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			bullseyeSearch.searchOrigin = position;
			bullseyeSearch.teamMaskFilter = TeamMask.AllExcept(this.teamIndex);
			bullseyeSearch.maxDistanceFilter = this.pulseRadius;
			bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
			bullseyeSearch.filterByLoS = true;
			bullseyeSearch.filterByDistinctEntity = true;
			bullseyeSearch.RefreshCandidates();
			IEnumerable<HurtBox> results = bullseyeSearch.GetResults();
			int num = 0;
			foreach (HurtBox hurtBox in results)
			{
				num++;
				Vector3 a = hurtBox.transform.position - position;
				float magnitude = a.magnitude;
				Vector3 a2 = a / magnitude;
				Rigidbody component = hurtBox.healthComponent.GetComponent<Rigidbody>();
				float num2 = component ? component.mass : 1f;
				float num3 = FlowerProjectileHover.yankSuitabilityCurve.Evaluate(num2);
				Vector3 force = a2 * (num2 * num3 * -FlowerProjectileHover.yankSpeed);
				DamageInfo damageInfo = new DamageInfo
				{
					attacker = this.owner,
					inflictor = base.gameObject,
					crit = this.crit,
					damage = this.damage,
					damageColorIndex = DamageColorIndex.Default,
					damageType = this.damageType,
					force = force,
					position = hurtBox.transform.position,
					procChainMask = this.procChainMask,
					procCoefficient = this.procCoefficient
				};
				hurtBox.healthComponent.TakeDamage(damageInfo);
			}
			this.healPulseHealthFractionValue = (float)num * FlowerProjectileHover.healthFractionYieldPerHit / (float)(this.pulseCount - 1);
		}

		// Token: 0x06002B3F RID: 11071 RVA: 0x000B65C0 File Offset: 0x000B47C0
		private void HealPulse()
		{
			if (this.owner)
			{
				HealthComponent component = this.owner.GetComponent<HealthComponent>();
				if (component)
				{
					component.HealFraction(this.healPulseHealthFractionValue, this.procChainMask);
				}
			}
		}

		// Token: 0x06002B40 RID: 11072 RVA: 0x000B6601 File Offset: 0x000B4801
		protected override void Pulse()
		{
			if (this.pulses == 1)
			{
				this.FirstPulse();
				return;
			}
			this.HealPulse();
		}

		// Token: 0x0400272B RID: 10027
		public static float yankSpeed;

		// Token: 0x0400272C RID: 10028
		public static AnimationCurve yankSuitabilityCurve;

		// Token: 0x0400272D RID: 10029
		public static float healthFractionYieldPerHit;

		// Token: 0x0400272E RID: 10030
		private GameObject owner;

		// Token: 0x0400272F RID: 10031
		private ProcChainMask procChainMask;

		// Token: 0x04002730 RID: 10032
		private float procCoefficient;

		// Token: 0x04002731 RID: 10033
		private float damage;

		// Token: 0x04002732 RID: 10034
		private DamageType damageType;

		// Token: 0x04002733 RID: 10035
		private bool crit;

		// Token: 0x04002734 RID: 10036
		private TeamIndex teamIndex = TeamIndex.None;

		// Token: 0x04002735 RID: 10037
		private float healPulseHealthFractionValue;
	}
}
