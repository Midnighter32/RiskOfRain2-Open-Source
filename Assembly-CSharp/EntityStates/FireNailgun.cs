using System;
using RoR2;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x020000C4 RID: 196
	public class FireNailgun : BaseState
	{
		// Token: 0x060003D1 RID: 977 RVA: 0x0000F9E4 File Offset: 0x0000DBE4
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireNailgun.baseDuration / this.attackSpeedStat;
			this.FireBullet(base.GetAimRay(), FireNailgun.initialBurstCount, FireNailgun.shotgunSpreadScale);
			this.soundID = Util.PlaySound(FireNailgun.spinUpSoundString, base.gameObject);
			Util.PlaySound(FireNailgun.burstSoundString, base.gameObject);
			base.PlayAnimation("Gesture, Additive", "FireNailgun");
			base.GetModelAnimator().SetBool("isFiringNailgun", true);
			EffectManager.instance.SimpleMuzzleFlash(FireNailgun.burstMuzzleFlashPrefab, base.gameObject, FireNailgun.muzzleName, false);
			this.stopwatchBetweenShots = -FireNailgun.delayBetweenShotgunAndNailgun / this.attackSpeedStat;
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x0000FA98 File Offset: 0x0000DC98
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (this.beginToCooldown)
			{
				this.cooldownStopwatch += Time.fixedDeltaTime;
				if (this.cooldownStopwatch > FireNailgun.baseCooldownDuration / this.attackSpeedStat)
				{
					this.outer.SetNextStateToMain();
					return;
				}
			}
			else
			{
				this.stopwatchBetweenShots += Time.fixedDeltaTime;
				if (this.stopwatchBetweenShots >= this.duration)
				{
					this.RefreshCombatStats();
					this.stopwatchBetweenShots -= this.duration;
					Util.PlaySound(FireNailgun.fireSoundString, base.gameObject);
					this.FireBullet(base.GetAimRay(), 1, 1f);
					EffectManager.instance.SimpleMuzzleFlash(FireNailgun.muzzleFlashPrefab, base.gameObject, FireNailgun.muzzleName, false);
				}
			}
			if ((!base.inputBank.skill1.down || base.characterBody.isSprinting) && !this.beginToCooldown)
			{
				this.beginToCooldown = true;
				Util.PlaySound(FireNailgun.spinDownSoundString, base.gameObject);
			}
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x0000FB9E File Offset: 0x0000DD9E
		private void RefreshCombatStats()
		{
			this.attackSpeedStat = base.characterBody.attackSpeed;
			this.critStat = base.characterBody.crit;
			this.duration = FireNailgun.baseDuration / this.attackSpeedStat;
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x0000FBD4 File Offset: 0x0000DDD4
		private void FireBullet(Ray aimRay, int bulletCount, float spreadScale)
		{
			base.StartAimMode(aimRay, 3f, false);
			if (base.isAuthority)
			{
				new BulletAttack
				{
					aimVector = aimRay.direction,
					origin = aimRay.origin,
					owner = base.gameObject,
					weapon = base.gameObject,
					bulletCount = (uint)bulletCount,
					damage = this.damageStat * FireNailgun.damageCoefficient,
					damageColorIndex = DamageColorIndex.Default,
					damageType = DamageType.Generic,
					falloffModel = BulletAttack.FalloffModel.DefaultBullet,
					force = FireNailgun.force,
					HitEffectNormal = false,
					procChainMask = default(ProcChainMask),
					procCoefficient = FireNailgun.procCoefficient,
					maxDistance = FireNailgun.maxDistance,
					radius = 0f,
					isCrit = base.RollCrit(),
					muzzleName = FireNailgun.muzzleName,
					minSpread = 0f,
					hitEffectPrefab = FireNailgun.hitEffectPrefab,
					maxSpread = base.characterBody.spreadBloomAngle,
					smartCollision = false,
					sniper = false,
					spreadPitchScale = FireNailgun.spreadPitchScale * spreadScale,
					spreadYawScale = FireNailgun.spreadYawScale * spreadScale,
					tracerEffectPrefab = FireNailgun.tracerEffectPrefab
				}.Fire();
			}
			if (base.characterBody)
			{
				base.characterBody.AddSpreadBloom(FireNailgun.spreadBloomValue);
			}
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x0000FD30 File Offset: 0x0000DF30
		public override void OnExit()
		{
			base.OnExit();
			if (base.GetModelAnimator())
			{
				base.GetModelAnimator().SetBool("isFiringNailgun", false);
			}
			AkSoundEngine.StopPlayingID(this.soundID);
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x0400037D RID: 893
		public static float baseDuration = 0.1f;

		// Token: 0x0400037E RID: 894
		public static float damageCoefficient = 0.1f;

		// Token: 0x0400037F RID: 895
		public static float procCoefficient = 1f;

		// Token: 0x04000380 RID: 896
		public static float baseCooldownDuration;

		// Token: 0x04000381 RID: 897
		public static int bulletCount = 1;

		// Token: 0x04000382 RID: 898
		public static float force = 100f;

		// Token: 0x04000383 RID: 899
		public static float maxDistance = 50f;

		// Token: 0x04000384 RID: 900
		public static string muzzleName;

		// Token: 0x04000385 RID: 901
		public static GameObject hitEffectPrefab;

		// Token: 0x04000386 RID: 902
		public static float spreadPitchScale = 0.5f;

		// Token: 0x04000387 RID: 903
		public static float spreadYawScale = 1f;

		// Token: 0x04000388 RID: 904
		public static GameObject tracerEffectPrefab;

		// Token: 0x04000389 RID: 905
		public static string fireSoundString;

		// Token: 0x0400038A RID: 906
		public static string burstSoundString;

		// Token: 0x0400038B RID: 907
		public static string spinUpSoundString;

		// Token: 0x0400038C RID: 908
		public static string spinDownSoundString;

		// Token: 0x0400038D RID: 909
		public static float shotgunSpreadScale;

		// Token: 0x0400038E RID: 910
		public static float delayBetweenShotgunAndNailgun;

		// Token: 0x0400038F RID: 911
		public static GameObject muzzleFlashPrefab;

		// Token: 0x04000390 RID: 912
		public static GameObject burstMuzzleFlashPrefab;

		// Token: 0x04000391 RID: 913
		public static float spreadBloomValue = 0.2f;

		// Token: 0x04000392 RID: 914
		public static int initialBurstCount;

		// Token: 0x04000393 RID: 915
		private float stopwatchBetweenShots;

		// Token: 0x04000394 RID: 916
		private float cooldownStopwatch;

		// Token: 0x04000395 RID: 917
		private float duration;

		// Token: 0x04000396 RID: 918
		private uint soundID;

		// Token: 0x04000397 RID: 919
		private bool beginToCooldown;
	}
}
