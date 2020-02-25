using System;
using RoR2;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x0200071E RID: 1822
	public class FireNailgun : BaseState
	{
		// Token: 0x06002A6F RID: 10863 RVA: 0x000B27AC File Offset: 0x000B09AC
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireNailgun.baseDuration / this.attackSpeedStat;
			this.FireBullet(base.GetAimRay(), FireNailgun.initialBurstCount, FireNailgun.shotgunSpreadScale);
			this.soundID = Util.PlaySound(FireNailgun.spinUpSoundString, base.gameObject);
			Util.PlaySound(FireNailgun.burstSoundString, base.gameObject);
			base.PlayAnimation("Gesture, Additive", "FireNailgun");
			base.GetModelAnimator().SetBool("isFiringNailgun", true);
			EffectManager.SimpleMuzzleFlash(FireNailgun.burstMuzzleFlashPrefab, base.gameObject, FireNailgun.muzzleName, false);
			this.stopwatchBetweenShots = -FireNailgun.delayBetweenShotgunAndNailgun / this.attackSpeedStat;
		}

		// Token: 0x06002A70 RID: 10864 RVA: 0x000B2858 File Offset: 0x000B0A58
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
					EffectManager.SimpleMuzzleFlash(FireNailgun.muzzleFlashPrefab, base.gameObject, FireNailgun.muzzleName, false);
				}
			}
			if ((!base.inputBank.skill1.down || base.characterBody.isSprinting) && !this.beginToCooldown)
			{
				this.beginToCooldown = true;
				Util.PlaySound(FireNailgun.spinDownSoundString, base.gameObject);
			}
		}

		// Token: 0x06002A71 RID: 10865 RVA: 0x000B2956 File Offset: 0x000B0B56
		private void RefreshCombatStats()
		{
			this.attackSpeedStat = base.characterBody.attackSpeed;
			this.critStat = base.characterBody.crit;
			this.duration = FireNailgun.baseDuration / this.attackSpeedStat;
		}

		// Token: 0x06002A72 RID: 10866 RVA: 0x000B298C File Offset: 0x000B0B8C
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

		// Token: 0x06002A73 RID: 10867 RVA: 0x000B2AE8 File Offset: 0x000B0CE8
		public override void OnExit()
		{
			base.OnExit();
			if (base.GetModelAnimator())
			{
				base.GetModelAnimator().SetBool("isFiringNailgun", false);
			}
			AkSoundEngine.StopPlayingID(this.soundID);
		}

		// Token: 0x06002A74 RID: 10868 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002637 RID: 9783
		public static float baseDuration = 0.1f;

		// Token: 0x04002638 RID: 9784
		public static float damageCoefficient = 0.1f;

		// Token: 0x04002639 RID: 9785
		public static float procCoefficient = 1f;

		// Token: 0x0400263A RID: 9786
		public static float baseCooldownDuration;

		// Token: 0x0400263B RID: 9787
		public static int bulletCount = 1;

		// Token: 0x0400263C RID: 9788
		public static float force = 100f;

		// Token: 0x0400263D RID: 9789
		public static float maxDistance = 50f;

		// Token: 0x0400263E RID: 9790
		public static string muzzleName;

		// Token: 0x0400263F RID: 9791
		public static GameObject hitEffectPrefab;

		// Token: 0x04002640 RID: 9792
		public static float spreadPitchScale = 0.5f;

		// Token: 0x04002641 RID: 9793
		public static float spreadYawScale = 1f;

		// Token: 0x04002642 RID: 9794
		public static GameObject tracerEffectPrefab;

		// Token: 0x04002643 RID: 9795
		public static string fireSoundString;

		// Token: 0x04002644 RID: 9796
		public static string burstSoundString;

		// Token: 0x04002645 RID: 9797
		public static string spinUpSoundString;

		// Token: 0x04002646 RID: 9798
		public static string spinDownSoundString;

		// Token: 0x04002647 RID: 9799
		public static float shotgunSpreadScale;

		// Token: 0x04002648 RID: 9800
		public static float delayBetweenShotgunAndNailgun;

		// Token: 0x04002649 RID: 9801
		public static GameObject muzzleFlashPrefab;

		// Token: 0x0400264A RID: 9802
		public static GameObject burstMuzzleFlashPrefab;

		// Token: 0x0400264B RID: 9803
		public static float spreadBloomValue = 0.2f;

		// Token: 0x0400264C RID: 9804
		public static int initialBurstCount;

		// Token: 0x0400264D RID: 9805
		private float stopwatchBetweenShots;

		// Token: 0x0400264E RID: 9806
		private float cooldownStopwatch;

		// Token: 0x0400264F RID: 9807
		private float duration;

		// Token: 0x04002650 RID: 9808
		private uint soundID;

		// Token: 0x04002651 RID: 9809
		private bool beginToCooldown;
	}
}
