using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.RoboBallBoss.Weapon
{
	// Token: 0x020007A1 RID: 1953
	public class FireDelayKnockup : BaseState
	{
		// Token: 0x06002CB4 RID: 11444 RVA: 0x000BC980 File Offset: 0x000BAB80
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireDelayKnockup.baseDuration / this.attackSpeedStat;
			base.PlayCrossfade("Gesture, Additive", "FireDelayKnockup", 0.1f);
			if (FireDelayKnockup.muzzleEffectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireDelayKnockup.muzzleEffectPrefab, base.gameObject, "EyeballMuzzle1", false);
				EffectManager.SimpleMuzzleFlash(FireDelayKnockup.muzzleEffectPrefab, base.gameObject, "EyeballMuzzle2", false);
				EffectManager.SimpleMuzzleFlash(FireDelayKnockup.muzzleEffectPrefab, base.gameObject, "EyeballMuzzle3", false);
			}
			if (NetworkServer.active)
			{
				BullseyeSearch bullseyeSearch = new BullseyeSearch();
				bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
				if (base.teamComponent)
				{
					bullseyeSearch.teamMaskFilter.RemoveTeam(base.teamComponent.teamIndex);
				}
				bullseyeSearch.maxDistanceFilter = FireDelayKnockup.maxDistance;
				bullseyeSearch.maxAngleFilter = 360f;
				Ray aimRay = base.GetAimRay();
				bullseyeSearch.searchOrigin = aimRay.origin;
				bullseyeSearch.searchDirection = aimRay.direction;
				bullseyeSearch.filterByLoS = false;
				bullseyeSearch.sortMode = BullseyeSearch.SortMode.Angle;
				bullseyeSearch.RefreshCandidates();
				List<HurtBox> list = bullseyeSearch.GetResults().ToList<HurtBox>();
				int num = 0;
				for (int i = 0; i < this.knockupCount; i++)
				{
					if (num >= list.Count)
					{
						num = 0;
					}
					HurtBox hurtBox = list[num];
					if (hurtBox)
					{
						Vector2 vector = UnityEngine.Random.insideUnitCircle * this.randomPositionRadius;
						Vector3 vector2 = hurtBox.transform.position + new Vector3(vector.x, 0f, vector.y);
						RaycastHit raycastHit;
						if (Physics.Raycast(new Ray(vector2 + Vector3.up * 1f, Vector3.down), out raycastHit, 200f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
						{
							vector2 = raycastHit.point;
						}
						ProjectileManager.instance.FireProjectile(FireDelayKnockup.projectilePrefab, vector2, Quaternion.identity, base.gameObject, this.damageStat, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
					}
					num++;
				}
			}
		}

		// Token: 0x06002CB5 RID: 11445 RVA: 0x000BCBAA File Offset: 0x000BADAA
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x040028D9 RID: 10457
		[SerializeField]
		public int knockupCount;

		// Token: 0x040028DA RID: 10458
		[SerializeField]
		public float randomPositionRadius;

		// Token: 0x040028DB RID: 10459
		public static float baseDuration;

		// Token: 0x040028DC RID: 10460
		public static GameObject projectilePrefab;

		// Token: 0x040028DD RID: 10461
		public static GameObject muzzleEffectPrefab;

		// Token: 0x040028DE RID: 10462
		public static float maxDistance;

		// Token: 0x040028DF RID: 10463
		private float duration;
	}
}
