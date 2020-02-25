using System;
using RoR2;
using UnityEngine;

namespace EntityStates.EngiTurret.EngiTurretWeapon
{
	// Token: 0x02000869 RID: 2153
	public class FireGauss : BaseState
	{
		// Token: 0x0600308D RID: 12429 RVA: 0x000D15B0 File Offset: 0x000CF7B0
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
				EffectManager.SimpleMuzzleFlash(FireGauss.effectPrefab, base.gameObject, muzzleName, false);
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
					bulletCount = 1U,
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

		// Token: 0x0600308E RID: 12430 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x0600308F RID: 12431 RVA: 0x000D1705 File Offset: 0x000CF905
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06003090 RID: 12432 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002EE7 RID: 12007
		public static GameObject effectPrefab;

		// Token: 0x04002EE8 RID: 12008
		public static GameObject hitEffectPrefab;

		// Token: 0x04002EE9 RID: 12009
		public static GameObject tracerEffectPrefab;

		// Token: 0x04002EEA RID: 12010
		public static string attackSoundString;

		// Token: 0x04002EEB RID: 12011
		public static float damageCoefficient;

		// Token: 0x04002EEC RID: 12012
		public static float force;

		// Token: 0x04002EED RID: 12013
		public static float minSpread;

		// Token: 0x04002EEE RID: 12014
		public static float maxSpread;

		// Token: 0x04002EEF RID: 12015
		public static int bulletCount;

		// Token: 0x04002EF0 RID: 12016
		public static float baseDuration = 2f;

		// Token: 0x04002EF1 RID: 12017
		public int bulletCountCurrent = 1;

		// Token: 0x04002EF2 RID: 12018
		private float duration;
	}
}
