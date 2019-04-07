using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Mage.Weapon
{
	// Token: 0x02000115 RID: 277
	internal class FireIceOrb : BaseState
	{
		// Token: 0x0600054F RID: 1359 RVA: 0x00017C50 File Offset: 0x00015E50
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireIceOrb.baseDuration / this.attackSpeedStat;
			FireIceOrb.Gauntlet gauntlet = this.gauntlet;
			if (gauntlet != FireIceOrb.Gauntlet.Left)
			{
				if (gauntlet == FireIceOrb.Gauntlet.Right)
				{
					this.muzzleString = "MuzzleRight";
					base.PlayAnimation("Gesture Right, Additive", "FireGauntletRight", "FireGauntlet.playbackRate", this.duration);
				}
			}
			else
			{
				this.muzzleString = "MuzzleLeft";
				base.PlayAnimation("Gesture Left, Additive", "FireGauntletLeft", "FireGauntlet.playbackRate", this.duration);
			}
			base.PlayAnimation("Gesture, Additive", "HoldGauntletsUp", "FireGauntlet.playbackRate", this.duration);
			Util.PlaySound(FireIceOrb.attackString, base.gameObject);
			this.animator = base.GetModelAnimator();
			base.characterBody.SetAimTimer(2f);
			this.FireGauntlet();
		}

		// Token: 0x06000550 RID: 1360 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000551 RID: 1361 RVA: 0x00017D24 File Offset: 0x00015F24
		private void FireGauntlet()
		{
			this.hasFiredGauntlet = true;
			Ray aimRay = base.GetAimRay();
			if (FireIceOrb.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireIceOrb.effectPrefab, base.gameObject, this.muzzleString, false);
			}
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(FireIceOrb.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireIceOrb.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x06000552 RID: 1362 RVA: 0x00017DC8 File Offset: 0x00015FC8
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (this.animator.GetFloat("FireGauntlet.fire") > 0f && !this.hasFiredGauntlet)
			{
				this.FireGauntlet();
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06000553 RID: 1363 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040005B0 RID: 1456
		public static GameObject projectilePrefab;

		// Token: 0x040005B1 RID: 1457
		public static GameObject effectPrefab;

		// Token: 0x040005B2 RID: 1458
		public static float baseDuration = 2f;

		// Token: 0x040005B3 RID: 1459
		public static float damageCoefficient = 1.2f;

		// Token: 0x040005B4 RID: 1460
		public static string attackString;

		// Token: 0x040005B5 RID: 1461
		private float duration;

		// Token: 0x040005B6 RID: 1462
		private bool hasFiredGauntlet;

		// Token: 0x040005B7 RID: 1463
		private string muzzleString;

		// Token: 0x040005B8 RID: 1464
		private Animator animator;

		// Token: 0x040005B9 RID: 1465
		public FireIceOrb.Gauntlet gauntlet;

		// Token: 0x02000116 RID: 278
		public enum Gauntlet
		{
			// Token: 0x040005BB RID: 1467
			Left,
			// Token: 0x040005BC RID: 1468
			Right
		}
	}
}
