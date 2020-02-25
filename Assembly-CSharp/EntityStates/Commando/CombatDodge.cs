using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using UnityEngine;

namespace EntityStates.Commando
{
	// Token: 0x020008B1 RID: 2225
	public class CombatDodge : DodgeState
	{
		// Token: 0x060031E2 RID: 12770 RVA: 0x000D6F4C File Offset: 0x000D514C
		public override void OnEnter()
		{
			base.OnEnter();
			this.search = new BullseyeSearch();
			this.search.searchDirection = Vector3.zero;
			this.search.teamMaskFilter = TeamMask.allButNeutral;
			this.search.teamMaskFilter.RemoveTeam(base.characterBody.teamComponent.teamIndex);
			this.search.filterByLoS = true;
			this.search.sortMode = BullseyeSearch.SortMode.Distance;
			this.search.maxDistanceFilter = CombatDodge.range;
		}

		// Token: 0x060031E3 RID: 12771 RVA: 0x000D6FD4 File Offset: 0x000D51D4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			float num = base.fixedAge / CombatDodge.durationToFire;
			if (this.bulletsFired < CombatDodge.bulletCount && num > (float)this.bulletsFired / (float)CombatDodge.bulletCount)
			{
				if (this.bulletsFired % 2 == 0)
				{
					base.PlayAnimation("Gesture Additive, Left", "FirePistol, Left");
					this.FireBullet("MuzzleLeft");
					return;
				}
				base.PlayAnimation("Gesture Additive, Right", "FirePistol, Right");
				this.FireBullet("MuzzleRight");
			}
		}

		// Token: 0x060031E4 RID: 12772 RVA: 0x000D7054 File Offset: 0x000D5254
		private HurtBox PickNextTarget()
		{
			this.search.searchOrigin = base.GetAimRay().origin;
			this.search.RefreshCandidates();
			List<HurtBox> list = this.search.GetResults().ToList<HurtBox>();
			if (list.Count <= 0)
			{
				return null;
			}
			return list[UnityEngine.Random.Range(0, list.Count)];
		}

		// Token: 0x060031E5 RID: 12773 RVA: 0x000D70B4 File Offset: 0x000D52B4
		private void FireBullet(string targetMuzzle)
		{
			this.bulletsFired++;
			base.AddRecoil(-0.4f * CombatDodge.recoilAmplitude, -0.8f * CombatDodge.recoilAmplitude, -0.3f * CombatDodge.recoilAmplitude, 0.3f * CombatDodge.recoilAmplitude);
			if (base.isAuthority)
			{
				Ray aimRay = base.GetAimRay();
				aimRay.direction = UnityEngine.Random.onUnitSphere;
				HurtBox hurtBox = this.PickNextTarget();
				if (hurtBox)
				{
					aimRay.direction = hurtBox.transform.position - aimRay.origin;
				}
				Util.PlaySound(CombatDodge.firePistolSoundString, base.gameObject);
				if (CombatDodge.muzzleEffectPrefab)
				{
					EffectManager.SimpleMuzzleFlash(CombatDodge.muzzleEffectPrefab, base.gameObject, targetMuzzle, false);
				}
				new BulletAttack
				{
					owner = base.gameObject,
					weapon = base.gameObject,
					origin = aimRay.origin,
					aimVector = aimRay.direction,
					minSpread = 0f,
					maxSpread = base.characterBody.spreadBloomAngle,
					damage = CombatDodge.damageCoefficient * this.damageStat,
					force = CombatDodge.force,
					tracerEffectPrefab = CombatDodge.tracerEffectPrefab,
					muzzleName = targetMuzzle,
					hitEffectPrefab = CombatDodge.hitEffectPrefab,
					isCrit = Util.CheckRoll(this.critStat, base.characterBody.master),
					radius = 0.1f,
					smartCollision = true
				}.Fire();
			}
		}

		// Token: 0x0400306F RID: 12399
		public static float durationToFire;

		// Token: 0x04003070 RID: 12400
		public static int bulletCount;

		// Token: 0x04003071 RID: 12401
		public static GameObject muzzleEffectPrefab;

		// Token: 0x04003072 RID: 12402
		public static GameObject tracerEffectPrefab;

		// Token: 0x04003073 RID: 12403
		public static GameObject hitEffectPrefab;

		// Token: 0x04003074 RID: 12404
		public static float damageCoefficient;

		// Token: 0x04003075 RID: 12405
		public static float force;

		// Token: 0x04003076 RID: 12406
		public static string firePistolSoundString;

		// Token: 0x04003077 RID: 12407
		public static float recoilAmplitude = 1f;

		// Token: 0x04003078 RID: 12408
		public static float range;

		// Token: 0x04003079 RID: 12409
		private int bulletsFired;

		// Token: 0x0400307A RID: 12410
		private BullseyeSearch search;
	}
}
