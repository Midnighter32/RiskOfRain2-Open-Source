using System;
using System.Linq;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.NullifierMonster
{
	// Token: 0x020007B2 RID: 1970
	public class FirePortalBomb : BaseState
	{
		// Token: 0x06002D0C RID: 11532 RVA: 0x000BE294 File Offset: 0x000BC494
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FirePortalBomb.baseDuration / this.attackSpeedStat;
			base.StartAimMode(4f, false);
			if (base.isAuthority)
			{
				BullseyeSearch bullseyeSearch = new BullseyeSearch();
				bullseyeSearch.searchOrigin = base.characterBody.corePosition;
				bullseyeSearch.searchDirection = base.characterBody.corePosition;
				bullseyeSearch.maxDistanceFilter = FirePortalBomb.maxDistance;
				bullseyeSearch.teamMaskFilter = TeamMask.allButNeutral;
				bullseyeSearch.teamMaskFilter.RemoveTeam(TeamComponent.GetObjectTeam(base.gameObject));
				bullseyeSearch.sortMode = BullseyeSearch.SortMode.DistanceAndAngle;
				bullseyeSearch.RefreshCandidates();
				EffectManager.SimpleMuzzleFlash(FirePortalBomb.muzzleflashEffectPrefab, base.gameObject, FirePortalBomb.muzzleString, true);
				HurtBox hurtBox = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
				if (hurtBox)
				{
					for (int i = 0; i < FirePortalBomb.portalBombCount; i++)
					{
						Vector3 b = UnityEngine.Random.insideUnitSphere * FirePortalBomb.randomRadius;
						b.y = 0f;
						if (i == 0)
						{
							b = Vector3.zero;
						}
						Vector3 vector = hurtBox.transform.position + Vector3.up * 10f + b;
						RaycastHit raycastHit;
						if (Physics.Raycast(vector, Vector3.down, out raycastHit, float.PositiveInfinity, LayerIndex.world.mask))
						{
							vector = raycastHit.point;
						}
						ProjectileManager.instance.FireProjectile(FirePortalBomb.portalBombProjectileEffect, vector, Quaternion.identity, base.gameObject, this.damageStat * FirePortalBomb.damageCoefficient, FirePortalBomb.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
					}
				}
			}
		}

		// Token: 0x06002D0D RID: 11533 RVA: 0x000BE435 File Offset: 0x000BC635
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x04002944 RID: 10564
		public static GameObject portalBombProjectileEffect;

		// Token: 0x04002945 RID: 10565
		public static GameObject muzzleflashEffectPrefab;

		// Token: 0x04002946 RID: 10566
		public static string muzzleString;

		// Token: 0x04002947 RID: 10567
		public static int portalBombCount;

		// Token: 0x04002948 RID: 10568
		public static float baseDuration;

		// Token: 0x04002949 RID: 10569
		public static float maxDistance;

		// Token: 0x0400294A RID: 10570
		public static float damageCoefficient;

		// Token: 0x0400294B RID: 10571
		public static float procCoefficient;

		// Token: 0x0400294C RID: 10572
		public static float randomRadius;

		// Token: 0x0400294D RID: 10573
		public static float force;

		// Token: 0x0400294E RID: 10574
		private float duration;
	}
}
