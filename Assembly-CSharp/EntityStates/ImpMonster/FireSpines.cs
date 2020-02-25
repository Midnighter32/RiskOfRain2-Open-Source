using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.ImpMonster
{
	// Token: 0x02000826 RID: 2086
	public class FireSpines : BaseState
	{
		// Token: 0x06002F42 RID: 12098 RVA: 0x000C9D6F File Offset: 0x000C7F6F
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireSpines.baseDuration / this.attackSpeedStat;
			base.PlayAnimation("Gesture", "FireSpines", "FireSpines.playbackRate", this.duration);
		}

		// Token: 0x06002F43 RID: 12099 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002F44 RID: 12100 RVA: 0x000C9DA4 File Offset: 0x000C7FA4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.spineTimer += Time.fixedDeltaTime;
			if (this.spineTimer >= FireSpines.durationBetweenThrows / this.attackSpeedStat && this.spineCount < FireSpines.spineCountMax)
			{
				this.spineCount++;
				Ray aimRay = base.GetAimRay();
				string muzzleName = "MuzzleMouth";
				this.spineTimer -= FireSpines.durationBetweenThrows / this.attackSpeedStat;
				if (FireSpines.effectPrefab)
				{
					EffectManager.SimpleMuzzleFlash(FireSpines.effectPrefab, base.gameObject, muzzleName, false);
				}
				if (base.isAuthority)
				{
					ProjectileManager.instance.FireProjectile(FireSpines.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireSpines.damageCoefficient, FireSpines.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
				}
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002F45 RID: 12101 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002CC7 RID: 11463
		public static GameObject projectilePrefab;

		// Token: 0x04002CC8 RID: 11464
		public static GameObject effectPrefab;

		// Token: 0x04002CC9 RID: 11465
		public static float baseDuration = 2f;

		// Token: 0x04002CCA RID: 11466
		public static float durationBetweenThrows = 0.1f;

		// Token: 0x04002CCB RID: 11467
		public static int spineCountMax = 3;

		// Token: 0x04002CCC RID: 11468
		public static float damageCoefficient = 1.2f;

		// Token: 0x04002CCD RID: 11469
		public static float force = 20f;

		// Token: 0x04002CCE RID: 11470
		private int spineCount;

		// Token: 0x04002CCF RID: 11471
		private float spineTimer;

		// Token: 0x04002CD0 RID: 11472
		private float duration;
	}
}
