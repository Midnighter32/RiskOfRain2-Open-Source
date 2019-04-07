using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020001A4 RID: 420
	internal class FireBarrage : BaseState
	{
		// Token: 0x06000829 RID: 2089 RVA: 0x00028CF8 File Offset: 0x00026EF8
		public override void OnEnter()
		{
			base.OnEnter();
			base.characterBody.SetSpreadBloom(0.2f, false);
			this.duration = FireBarrage.totalDuration / this.attackSpeedStat;
			this.durationBetweenShots = FireBarrage.baseDurationBetweenShots / this.attackSpeedStat;
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

		// Token: 0x0600082A RID: 2090 RVA: 0x00028DBC File Offset: 0x00026FBC
		private void FireBullet()
		{
			Ray aimRay = base.GetAimRay();
			string muzzleName = "MuzzleRight";
			if (this.modelAnimator)
			{
				if (FireBarrage.effectPrefab)
				{
					EffectManager.instance.SimpleMuzzleFlash(FireBarrage.effectPrefab, base.gameObject, muzzleName, false);
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
					bulletCount = 1u,
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

		// Token: 0x0600082B RID: 2091 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x0600082C RID: 2092 RVA: 0x00028F4C File Offset: 0x0002714C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatchBetweenShots += Time.fixedDeltaTime;
			if (this.stopwatchBetweenShots >= this.durationBetweenShots && this.totalBulletsFired < FireBarrage.bulletCount)
			{
				this.stopwatchBetweenShots -= this.durationBetweenShots;
				this.FireBullet();
			}
			if (base.fixedAge >= this.duration && this.totalBulletsFired == FireBarrage.bulletCount && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x0600082D RID: 2093 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000AB8 RID: 2744
		public static GameObject effectPrefab;

		// Token: 0x04000AB9 RID: 2745
		public static GameObject hitEffectPrefab;

		// Token: 0x04000ABA RID: 2746
		public static GameObject tracerEffectPrefab;

		// Token: 0x04000ABB RID: 2747
		public static float damageCoefficient;

		// Token: 0x04000ABC RID: 2748
		public static float force;

		// Token: 0x04000ABD RID: 2749
		public static float minSpread;

		// Token: 0x04000ABE RID: 2750
		public static float maxSpread;

		// Token: 0x04000ABF RID: 2751
		public static float baseDurationBetweenShots = 1f;

		// Token: 0x04000AC0 RID: 2752
		public static float totalDuration = 2f;

		// Token: 0x04000AC1 RID: 2753
		public static float bulletRadius = 1.5f;

		// Token: 0x04000AC2 RID: 2754
		public static int bulletCount = 1;

		// Token: 0x04000AC3 RID: 2755
		public static string fireBarrageSoundString;

		// Token: 0x04000AC4 RID: 2756
		public static float recoilAmplitude;

		// Token: 0x04000AC5 RID: 2757
		public static float spreadBloomValue;

		// Token: 0x04000AC6 RID: 2758
		private int totalBulletsFired;

		// Token: 0x04000AC7 RID: 2759
		public float stopwatchBetweenShots;

		// Token: 0x04000AC8 RID: 2760
		private Animator modelAnimator;

		// Token: 0x04000AC9 RID: 2761
		private Transform modelTransform;

		// Token: 0x04000ACA RID: 2762
		private float duration;

		// Token: 0x04000ACB RID: 2763
		private float durationBetweenShots;
	}
}
