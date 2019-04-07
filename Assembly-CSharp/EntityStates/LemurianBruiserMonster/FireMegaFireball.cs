using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.LemurianBruiserMonster
{
	// Token: 0x0200011F RID: 287
	internal class FireMegaFireball : BaseState
	{
		// Token: 0x06000588 RID: 1416 RVA: 0x0001935C File Offset: 0x0001755C
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireMegaFireball.baseDuration / this.attackSpeedStat;
			this.fireDuration = FireMegaFireball.baseFireDuration / this.attackSpeedStat;
			base.PlayAnimation("Gesture, Additive", "FireMegaFireball", "FireMegaFireball.playbackRate", this.duration);
			Util.PlaySound(FireMegaFireball.attackString, base.gameObject);
		}

		// Token: 0x06000589 RID: 1417 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x0600058A RID: 1418 RVA: 0x000193C0 File Offset: 0x000175C0
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			string muzzleName = "MuzzleMouth";
			if (base.isAuthority)
			{
				int num = Mathf.FloorToInt(base.fixedAge / this.fireDuration * (float)FireMegaFireball.projectileCount);
				if (this.projectilesFired <= num && this.projectilesFired < FireMegaFireball.projectileCount)
				{
					if (FireMegaFireball.muzzleflashEffectPrefab)
					{
						EffectManager.instance.SimpleMuzzleFlash(FireMegaFireball.muzzleflashEffectPrefab, base.gameObject, muzzleName, false);
					}
					Ray aimRay = base.GetAimRay();
					float speedOverride = FireMegaFireball.projectileSpeed;
					float bonusYaw = (float)Mathf.FloorToInt((float)this.projectilesFired - (float)(FireMegaFireball.projectileCount - 1) / 2f) / (float)(FireMegaFireball.projectileCount - 1) * FireMegaFireball.totalYawSpread;
					Vector3 forward = Util.ApplySpread(aimRay.direction, 0f, 0f, 1f, 1f, bonusYaw, 0f);
					ProjectileManager.instance.FireProjectile(FireMegaFireball.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * FireMegaFireball.damageCoefficient, FireMegaFireball.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, speedOverride);
					this.projectilesFired++;
				}
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x0600058B RID: 1419 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000636 RID: 1590
		public static GameObject projectilePrefab;

		// Token: 0x04000637 RID: 1591
		public static GameObject muzzleflashEffectPrefab;

		// Token: 0x04000638 RID: 1592
		public static int projectileCount = 3;

		// Token: 0x04000639 RID: 1593
		public static float totalYawSpread = 5f;

		// Token: 0x0400063A RID: 1594
		public static float baseDuration = 2f;

		// Token: 0x0400063B RID: 1595
		public static float baseFireDuration = 0.2f;

		// Token: 0x0400063C RID: 1596
		public static float damageCoefficient = 1.2f;

		// Token: 0x0400063D RID: 1597
		public static float projectileSpeed;

		// Token: 0x0400063E RID: 1598
		public static float force = 20f;

		// Token: 0x0400063F RID: 1599
		public static string attackString;

		// Token: 0x04000640 RID: 1600
		private float duration;

		// Token: 0x04000641 RID: 1601
		private float fireDuration;

		// Token: 0x04000642 RID: 1602
		private int projectilesFired;
	}
}
