using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.LemurianBruiserMonster
{
	// Token: 0x020007EE RID: 2030
	public class FireMegaFireball : BaseState
	{
		// Token: 0x06002E2E RID: 11822 RVA: 0x000C4660 File Offset: 0x000C2860
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireMegaFireball.baseDuration / this.attackSpeedStat;
			this.fireDuration = FireMegaFireball.baseFireDuration / this.attackSpeedStat;
			base.PlayAnimation("Gesture, Additive", "FireMegaFireball", "FireMegaFireball.playbackRate", this.duration);
			Util.PlaySound(FireMegaFireball.attackString, base.gameObject);
		}

		// Token: 0x06002E2F RID: 11823 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002E30 RID: 11824 RVA: 0x000C46C4 File Offset: 0x000C28C4
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
						EffectManager.SimpleMuzzleFlash(FireMegaFireball.muzzleflashEffectPrefab, base.gameObject, muzzleName, false);
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

		// Token: 0x06002E31 RID: 11825 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002B37 RID: 11063
		public static GameObject projectilePrefab;

		// Token: 0x04002B38 RID: 11064
		public static GameObject muzzleflashEffectPrefab;

		// Token: 0x04002B39 RID: 11065
		public static int projectileCount = 3;

		// Token: 0x04002B3A RID: 11066
		public static float totalYawSpread = 5f;

		// Token: 0x04002B3B RID: 11067
		public static float baseDuration = 2f;

		// Token: 0x04002B3C RID: 11068
		public static float baseFireDuration = 0.2f;

		// Token: 0x04002B3D RID: 11069
		public static float damageCoefficient = 1.2f;

		// Token: 0x04002B3E RID: 11070
		public static float projectileSpeed;

		// Token: 0x04002B3F RID: 11071
		public static float force = 20f;

		// Token: 0x04002B40 RID: 11072
		public static string attackString;

		// Token: 0x04002B41 RID: 11073
		private float duration;

		// Token: 0x04002B42 RID: 11074
		private float fireDuration;

		// Token: 0x04002B43 RID: 11075
		private int projectilesFired;
	}
}
