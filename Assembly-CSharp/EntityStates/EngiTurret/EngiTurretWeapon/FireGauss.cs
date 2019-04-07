using System;
using RoR2;
using UnityEngine;

namespace EntityStates.EngiTurret.EngiTurretWeapon
{
	// Token: 0x02000181 RID: 385
	internal class FireGauss : BaseState
	{
		// Token: 0x06000765 RID: 1893 RVA: 0x000244D8 File Offset: 0x000226D8
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireGauss.baseDuration / this.attackSpeedStat;
			Util.PlaySound(FireGauss.attackSoundString, base.gameObject);
			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 2f, false);
			base.PlayAnimation("Gesture", "FireGauss", "FireGauss.playbackRate", this.duration);
			string muzzleName = "Muzzle";
			if (FireGauss.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireGauss.effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				new BulletAttack
				{
					owner = base.gameObject,
					weapon = base.gameObject,
					origin = aimRay.origin,
					aimVector = aimRay.direction,
					minSpread = FireGauss.minSpread,
					maxSpread = FireGauss.maxSpread,
					bulletCount = 1u,
					damage = FireGauss.damageCoefficient * this.damageStat,
					force = FireGauss.force,
					tracerEffectPrefab = FireGauss.tracerEffectPrefab,
					muzzleName = muzzleName,
					hitEffectPrefab = FireGauss.hitEffectPrefab,
					isCrit = Util.CheckRoll(this.critStat, base.characterBody.master),
					HitEffectNormal = false,
					radius = 0.15f
				}.Fire();
			}
		}

		// Token: 0x06000766 RID: 1894 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000767 RID: 1895 RVA: 0x00024632 File Offset: 0x00022832
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06000768 RID: 1896 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x0400096B RID: 2411
		public static GameObject effectPrefab;

		// Token: 0x0400096C RID: 2412
		public static GameObject hitEffectPrefab;

		// Token: 0x0400096D RID: 2413
		public static GameObject tracerEffectPrefab;

		// Token: 0x0400096E RID: 2414
		public static string attackSoundString;

		// Token: 0x0400096F RID: 2415
		public static float damageCoefficient;

		// Token: 0x04000970 RID: 2416
		public static float force;

		// Token: 0x04000971 RID: 2417
		public static float minSpread;

		// Token: 0x04000972 RID: 2418
		public static float maxSpread;

		// Token: 0x04000973 RID: 2419
		public static int bulletCount;

		// Token: 0x04000974 RID: 2420
		public static float baseDuration = 2f;

		// Token: 0x04000975 RID: 2421
		public int bulletCountCurrent = 1;

		// Token: 0x04000976 RID: 2422
		private float duration;
	}
}
