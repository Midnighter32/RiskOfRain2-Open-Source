using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.UrchinTurret.Weapon
{
	// Token: 0x02000908 RID: 2312
	public class MinigunFire : MinigunState
	{
		// Token: 0x06003394 RID: 13204 RVA: 0x000DFD94 File Offset: 0x000DDF94
		public override void OnEnter()
		{
			base.OnEnter();
			if (this.muzzleTransform && MinigunFire.muzzleVfxPrefab)
			{
				this.muzzleVfxTransform = UnityEngine.Object.Instantiate<GameObject>(MinigunFire.muzzleVfxPrefab, this.muzzleTransform).transform;
			}
			this.baseFireRate = 1f / MinigunFire.baseFireInterval;
			this.baseBulletsPerSecond = this.baseFireRate;
			this.critEndTime = Run.FixedTimeStamp.negativeInfinity;
			this.lastCritCheck = Run.FixedTimeStamp.negativeInfinity;
			Util.PlaySound(MinigunFire.startSound, base.gameObject);
			base.PlayCrossfade("Gesture, Additive", "ShootLoop", 0.2f);
		}

		// Token: 0x06003395 RID: 13205 RVA: 0x000DFE35 File Offset: 0x000DE035
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

		// Token: 0x06003396 RID: 13206 RVA: 0x000DFE74 File Offset: 0x000DE074
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

		// Token: 0x06003397 RID: 13207 RVA: 0x000DFED1 File Offset: 0x000DE0D1
		private void OnFireShared()
		{
			Util.PlaySound(MinigunFire.fireSound, base.gameObject);
			if (base.isAuthority)
			{
				this.OnFireAuthority();
			}
		}

		// Token: 0x06003398 RID: 13208 RVA: 0x000DFEF4 File Offset: 0x000DE0F4
		private void OnFireAuthority()
		{
			this.UpdateCrits();
			bool crit = !this.critEndTime.hasPassed;
			float damage = MinigunFire.baseDamagePerSecondCoefficient / this.baseBulletsPerSecond * this.damageStat;
			Ray aimRay = base.GetAimRay();
			Vector3 forward = Util.ApplySpread(aimRay.direction, MinigunFire.bulletMinSpread, MinigunFire.bulletMaxSpread, 1f, 1f, 0f, 0f);
			ProjectileManager.instance.FireProjectile(MinigunFire.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(forward), base.gameObject, damage, 0f, crit, DamageColorIndex.Default, null, -1f);
		}

		// Token: 0x06003399 RID: 13209 RVA: 0x000DFF8C File Offset: 0x000DE18C
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

		// Token: 0x04003311 RID: 13073
		public static GameObject muzzleVfxPrefab;

		// Token: 0x04003312 RID: 13074
		public static GameObject projectilePrefab;

		// Token: 0x04003313 RID: 13075
		public static float baseFireInterval;

		// Token: 0x04003314 RID: 13076
		public static float baseDamagePerSecondCoefficient;

		// Token: 0x04003315 RID: 13077
		public static float bulletMinSpread;

		// Token: 0x04003316 RID: 13078
		public static float bulletMaxSpread;

		// Token: 0x04003317 RID: 13079
		public static string fireSound;

		// Token: 0x04003318 RID: 13080
		public static string startSound;

		// Token: 0x04003319 RID: 13081
		public static string endSound;

		// Token: 0x0400331A RID: 13082
		private float fireTimer;

		// Token: 0x0400331B RID: 13083
		private Transform muzzleVfxTransform;

		// Token: 0x0400331C RID: 13084
		private float baseFireRate;

		// Token: 0x0400331D RID: 13085
		private float baseBulletsPerSecond;

		// Token: 0x0400331E RID: 13086
		private Run.FixedTimeStamp critEndTime;

		// Token: 0x0400331F RID: 13087
		private Run.FixedTimeStamp lastCritCheck;
	}
}
