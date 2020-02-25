using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using RoR2.Orbs;
using UnityEngine;

namespace EntityStates.Croco
{
	// Token: 0x020008A6 RID: 2214
	public class Disease : BaseState
	{
		// Token: 0x060031A5 RID: 12709 RVA: 0x000D5D40 File Offset: 0x000D3F40
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = Disease.baseDuration / this.attackSpeedStat;
			Ray aimRay = base.GetAimRay();
			Transform transform = base.FindModelChild(Disease.muzzleString);
			BullseyeSearch bullseyeSearch = new BullseyeSearch();
			bullseyeSearch.searchOrigin = aimRay.origin;
			bullseyeSearch.searchDirection = aimRay.direction;
			bullseyeSearch.maxDistanceFilter = Disease.orbRange;
			bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
			bullseyeSearch.teamMaskFilter.RemoveTeam(TeamComponent.GetObjectTeam(base.gameObject));
			bullseyeSearch.sortMode = BullseyeSearch.SortMode.DistanceAndAngle;
			bullseyeSearch.RefreshCandidates();
			EffectManager.SimpleMuzzleFlash(Disease.muzzleflashEffectPrefab, base.gameObject, Disease.muzzleString, true);
			List<HurtBox> list = bullseyeSearch.GetResults().ToList<HurtBox>();
			if (list.Count > 0)
			{
				Debug.LogFormat("Shooting at {0}", new object[]
				{
					list[0]
				});
				HurtBox target = list.FirstOrDefault<HurtBox>();
				LightningOrb lightningOrb = new LightningOrb();
				lightningOrb.attacker = base.gameObject;
				lightningOrb.bouncedObjects = new List<HealthComponent>();
				lightningOrb.lightningType = LightningOrb.LightningType.CrocoDisease;
				lightningOrb.damageType = DamageType.PoisonOnHit;
				lightningOrb.damageValue = this.damageStat * Disease.damageCoefficient;
				lightningOrb.isCrit = base.RollCrit();
				lightningOrb.procCoefficient = Disease.procCoefficient;
				lightningOrb.bouncesRemaining = Disease.maxBounces;
				lightningOrb.origin = transform.position;
				lightningOrb.target = target;
				lightningOrb.teamIndex = base.GetTeam();
				lightningOrb.range = Disease.bounceRange;
				OrbManager.instance.AddOrb(lightningOrb);
			}
		}

		// Token: 0x060031A6 RID: 12710 RVA: 0x000D5EC4 File Offset: 0x000D40C4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x04003020 RID: 12320
		public static GameObject muzzleflashEffectPrefab;

		// Token: 0x04003021 RID: 12321
		public static string muzzleString;

		// Token: 0x04003022 RID: 12322
		public static float orbRange;

		// Token: 0x04003023 RID: 12323
		public static float baseDuration;

		// Token: 0x04003024 RID: 12324
		public static float damageCoefficient;

		// Token: 0x04003025 RID: 12325
		public static int maxBounces;

		// Token: 0x04003026 RID: 12326
		public static float bounceRange;

		// Token: 0x04003027 RID: 12327
		public static float procCoefficient;

		// Token: 0x04003028 RID: 12328
		private float duration;
	}
}
