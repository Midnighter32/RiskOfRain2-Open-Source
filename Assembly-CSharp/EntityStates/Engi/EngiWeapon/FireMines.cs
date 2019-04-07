using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Engi.EngiWeapon
{
	// Token: 0x02000188 RID: 392
	internal class FireMines : BaseState
	{
		// Token: 0x06000791 RID: 1937 RVA: 0x000254F8 File Offset: 0x000236F8
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(FireMines.throwMineSoundString, base.gameObject);
			this.duration = FireMines.baseDuration / this.attackSpeedStat;
			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 2f, false);
			if (base.GetModelAnimator())
			{
				float num = this.duration * 0.3f;
				base.PlayCrossfade("Gesture, Additive", "FireMineRight", "FireMine.playbackRate", this.duration + num, 0.05f);
			}
			string muzzleName = "MuzzleCenter";
			if (FireMines.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireMines.effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(this.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireMines.damageCoefficient, FireMines.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x06000792 RID: 1938 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000793 RID: 1939 RVA: 0x00025606 File Offset: 0x00023806
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06000794 RID: 1940 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x040009B3 RID: 2483
		public static GameObject effectPrefab;

		// Token: 0x040009B4 RID: 2484
		public static GameObject hitEffectPrefab;

		// Token: 0x040009B5 RID: 2485
		[SerializeField]
		public GameObject projectilePrefab;

		// Token: 0x040009B6 RID: 2486
		public static float damageCoefficient;

		// Token: 0x040009B7 RID: 2487
		public static float force;

		// Token: 0x040009B8 RID: 2488
		public static int bulletCount;

		// Token: 0x040009B9 RID: 2489
		public static float baseDuration = 2f;

		// Token: 0x040009BA RID: 2490
		public static string throwMineSoundString;

		// Token: 0x040009BB RID: 2491
		private float duration;

		// Token: 0x040009BC RID: 2492
		public int bulletCountCurrent = 1;
	}
}
