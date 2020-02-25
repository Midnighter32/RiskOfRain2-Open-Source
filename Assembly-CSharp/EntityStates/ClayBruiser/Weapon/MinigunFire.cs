using System;
using RoR2;
using UnityEngine;

namespace EntityStates.ClayBruiser.Weapon
{
	// Token: 0x020008CB RID: 2251
	public class MinigunFire : MinigunState
	{
		// Token: 0x06003276 RID: 12918 RVA: 0x000DA2FC File Offset: 0x000D84FC
		public override void OnEnter()
		{
			base.OnEnter();
			if (this.muzzleTransform && MinigunFire.muzzleVfxPrefab)
			{
				this.muzzleVfxTransform = UnityEngine.Object.Instantiate<GameObject>(MinigunFire.muzzleVfxPrefab, this.muzzleTransform).transform;
			}
			this.baseFireRate = 1f / MinigunFire.baseFireInterval;
			this.baseBulletsPerSecond = (float)MinigunFire.baseBulletCount * this.baseFireRate;
			this.critEndTime = Run.FixedTimeStamp.negativeInfinity;
			this.lastCritCheck = Run.FixedTimeStamp.negativeInfinity;
			Util.PlaySound(MinigunFire.startSound, base.gameObject);
			base.PlayCrossfade("Gesture, Additive", "FireMinigun", 0.2f);
		}

		// Token: 0x06003277 RID: 12919 RVA: 0x000DA3A3 File Offset: 0x000D85A3
		private void UpdateCrits()
		{
			if (this.lastCritCheck.timeSince >= 1f)
			{
				this.lastCritCheck = Run.FixedTimeStamp.now;
				if (base.RollCrit())
				{
					this.critEndTime = Run.FixedTimeStamp.now + 2f;
				}
			}
		}

		// Token: 0x06003278 RID: 12920 RVA: 0x000DA3E0 File Offset: 0x000D85E0
		public override void OnExit()
		{
			Util.PlaySound(MinigunFire.endSound, base.gameObject);
			if (this.muzzleVfxTransform)
			{
				EntityState.Destroy(this.muzzleVfxTransform.gameObject);
				this.muzzleVfxTransform = null;
			}
			base.PlayCrossfade("Gesture, Additive", "BufferEmpty", 0.2f);
			base.OnExit();
		}

		// Token: 0x06003279 RID: 12921 RVA: 0x000DA43D File Offset: 0x000D863D
		private void OnFireShared()
		{
			Util.PlaySound(MinigunFire.fireSound, base.gameObject);
			if (base.isAuthority)
			{
				this.OnFireAuthority();
			}
		}

		// Token: 0x0600327A RID: 12922 RVA: 0x000DA460 File Offset: 0x000D8660
		private void OnFireAuthority()
		{
			this.UpdateCrits();
			bool isCrit = !this.critEndTime.hasPassed;
			float damage = MinigunFire.baseDamagePerSecondCoefficient / this.baseBulletsPerSecond * this.damageStat;
			float force = MinigunFire.baseForcePerSecond / this.baseBulletsPerSecond;
			float procCoefficient = MinigunFire.baseProcCoefficientPerSecond / this.baseBulletsPerSecond;
			Ray aimRay = base.GetAimRay();
			new BulletAttack
			{
				bulletCount = (uint)MinigunFire.baseBulletCount,
				aimVector = aimRay.direction,
				origin = aimRay.origin,
				damage = damage,
				damageColorIndex = DamageColorIndex.Default,
				damageType = DamageType.Generic,
				falloffModel = BulletAttack.FalloffModel.None,
				maxDistance = MinigunFire.bulletMaxDistance,
				force = force,
				hitMask = LayerIndex.CommonMasks.bullet,
				minSpread = MinigunFire.bulletMinSpread,
				maxSpread = MinigunFire.bulletMaxSpread,
				isCrit = isCrit,
				owner = base.gameObject,
				muzzleName = MinigunState.muzzleName,
				smartCollision = false,
				procChainMask = default(ProcChainMask),
				procCoefficient = procCoefficient,
				radius = 0f,
				sniper = false,
				stopperMask = LayerIndex.CommonMasks.bullet,
				weapon = null,
				tracerEffectPrefab = MinigunFire.bulletTracerEffectPrefab,
				spreadPitchScale = 1f,
				spreadYawScale = 1f,
				queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
				hitEffectPrefab = MinigunFire.bulletHitEffectPrefab,
				HitEffectNormal = MinigunFire.bulletHitEffectNormal
			}.Fire();
		}

		// Token: 0x0600327B RID: 12923 RVA: 0x000DA5D0 File Offset: 0x000D87D0
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.fireTimer -= Time.fixedDeltaTime;
			if (this.fireTimer <= 0f)
			{
				float num = MinigunFire.baseFireInterval / this.attackSpeedStat;
				this.fireTimer += num;
				this.OnFireShared();
			}
			if (base.isAuthority && !base.skillButtonState.down)
			{
				this.outer.SetNextState(new MinigunSpinDown());
				return;
			}
		}

		// Token: 0x0400316C RID: 12652
		public static GameObject muzzleVfxPrefab;

		// Token: 0x0400316D RID: 12653
		public static float baseFireInterval;

		// Token: 0x0400316E RID: 12654
		public static int baseBulletCount;

		// Token: 0x0400316F RID: 12655
		public static float baseDamagePerSecondCoefficient;

		// Token: 0x04003170 RID: 12656
		public static float baseForcePerSecond;

		// Token: 0x04003171 RID: 12657
		public static float baseProcCoefficientPerSecond;

		// Token: 0x04003172 RID: 12658
		public static float bulletMinSpread;

		// Token: 0x04003173 RID: 12659
		public static float bulletMaxSpread;

		// Token: 0x04003174 RID: 12660
		public static GameObject bulletTracerEffectPrefab;

		// Token: 0x04003175 RID: 12661
		public static GameObject bulletHitEffectPrefab;

		// Token: 0x04003176 RID: 12662
		public static bool bulletHitEffectNormal;

		// Token: 0x04003177 RID: 12663
		public static float bulletMaxDistance;

		// Token: 0x04003178 RID: 12664
		public static string fireSound;

		// Token: 0x04003179 RID: 12665
		public static string startSound;

		// Token: 0x0400317A RID: 12666
		public static string endSound;

		// Token: 0x0400317B RID: 12667
		private float fireTimer;

		// Token: 0x0400317C RID: 12668
		private Transform muzzleVfxTransform;

		// Token: 0x0400317D RID: 12669
		private float baseFireRate;

		// Token: 0x0400317E RID: 12670
		private float baseBulletsPerSecond;

		// Token: 0x0400317F RID: 12671
		private Run.FixedTimeStamp critEndTime;

		// Token: 0x04003180 RID: 12672
		private Run.FixedTimeStamp lastCritCheck;
	}
}
