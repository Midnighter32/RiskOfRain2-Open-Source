using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020008B5 RID: 2229
	public class FireBarrage : BaseState
	{
		// Token: 0x060031FB RID: 12795 RVA: 0x000D7C58 File Offset: 0x000D5E58
		public override void OnEnter()
		{
			base.OnEnter();
			base.characterBody.SetSpreadBloom(0.2f, false);
			this.duration = FireBarrage.totalDuration;
			this.durationBetweenShots = FireBarrage.baseDurationBetweenShots / this.attackSpeedStat;
			this.bulletCount = (int)((float)FireBarrage.baseBulletCount * this.attackSpeedStat);
			this.modelAnimator = base.GetModelAnimator();
			this.modelTransform = base.GetModelTransform();
			base.PlayCrossfade("Gesture, Additive", "FireBarrage", "FireBarrage.playbackRate", this.duration, 0.2f);
			base.PlayCrossfade("Gesture, Override", "FireBarrage", "FireBarrage.playbackRate", this.duration, 0.2f);
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(2f);
			}
			this.FireBullet();
		}

		// Token: 0x060031FC RID: 12796 RVA: 0x000D7D28 File Offset: 0x000D5F28
		private void FireBullet()
		{
			Ray aimRay = base.GetAimRay();
			string muzzleName = "MuzzleRight";
			if (this.modelAnimator)
			{
				if (FireBarrage.effectPrefab)
				{
					EffectManager.SimpleMuzzleFlash(FireBarrage.effectPrefab, base.gameObject, muzzleName, false);
				}
				base.PlayAnimation("Gesture Additive, Right", "FirePistol, Right");
			}
			base.AddRecoil(-0.8f * FireBarrage.recoilAmplitude, -1f * FireBarrage.recoilAmplitude, -0.1f * FireBarrage.recoilAmplitude, 0.15f * FireBarrage.recoilAmplitude);
			if (base.isAuthority)
			{
				new BulletAttack
				{
					owner = base.gameObject,
					weapon = base.gameObject,
					origin = aimRay.origin,
					aimVector = aimRay.direction,
					minSpread = FireBarrage.minSpread,
					maxSpread = FireBarrage.maxSpread,
					bulletCount = 1U,
					damage = FireBarrage.damageCoefficient * this.damageStat,
					force = FireBarrage.force,
					tracerEffectPrefab = FireBarrage.tracerEffectPrefab,
					muzzleName = muzzleName,
					hitEffectPrefab = FireBarrage.hitEffectPrefab,
					isCrit = Util.CheckRoll(this.critStat, base.characterBody.master),
					radius = FireBarrage.bulletRadius,
					smartCollision = true,
					damageType = DamageType.Stun1s
				}.Fire();
			}
			base.characterBody.AddSpreadBloom(FireBarrage.spreadBloomValue);
			this.totalBulletsFired++;
			Util.PlaySound(FireBarrage.fireBarrageSoundString, base.gameObject);
		}

		// Token: 0x060031FD RID: 12797 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x060031FE RID: 12798 RVA: 0x000D7EB4 File Offset: 0x000D60B4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatchBetweenShots += Time.fixedDeltaTime;
			if (this.stopwatchBetweenShots >= this.durationBetweenShots && this.totalBulletsFired < this.bulletCount)
			{
				this.stopwatchBetweenShots -= this.durationBetweenShots;
				this.FireBullet();
			}
			if (base.fixedAge >= this.duration && this.totalBulletsFired == this.bulletCount && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060031FF RID: 12799 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040030A0 RID: 12448
		public static GameObject effectPrefab;

		// Token: 0x040030A1 RID: 12449
		public static GameObject hitEffectPrefab;

		// Token: 0x040030A2 RID: 12450
		public static GameObject tracerEffectPrefab;

		// Token: 0x040030A3 RID: 12451
		public static float damageCoefficient;

		// Token: 0x040030A4 RID: 12452
		public static float force;

		// Token: 0x040030A5 RID: 12453
		public static float minSpread;

		// Token: 0x040030A6 RID: 12454
		public static float maxSpread;

		// Token: 0x040030A7 RID: 12455
		public static float baseDurationBetweenShots = 1f;

		// Token: 0x040030A8 RID: 12456
		public static float totalDuration = 2f;

		// Token: 0x040030A9 RID: 12457
		public static float bulletRadius = 1.5f;

		// Token: 0x040030AA RID: 12458
		public static int baseBulletCount = 1;

		// Token: 0x040030AB RID: 12459
		public static string fireBarrageSoundString;

		// Token: 0x040030AC RID: 12460
		public static float recoilAmplitude;

		// Token: 0x040030AD RID: 12461
		public static float spreadBloomValue;

		// Token: 0x040030AE RID: 12462
		private int totalBulletsFired;

		// Token: 0x040030AF RID: 12463
		private int bulletCount;

		// Token: 0x040030B0 RID: 12464
		public float stopwatchBetweenShots;

		// Token: 0x040030B1 RID: 12465
		private Animator modelAnimator;

		// Token: 0x040030B2 RID: 12466
		private Transform modelTransform;

		// Token: 0x040030B3 RID: 12467
		private float duration;

		// Token: 0x040030B4 RID: 12468
		private float durationBetweenShots;
	}
}
