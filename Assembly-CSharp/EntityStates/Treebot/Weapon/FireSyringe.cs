using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Treebot.Weapon
{
	// Token: 0x02000756 RID: 1878
	public class FireSyringe : BaseState
	{
		// Token: 0x06002B76 RID: 11126 RVA: 0x000B74DE File Offset: 0x000B56DE
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireSyringe.baseDuration / this.attackSpeedStat;
			this.fireDuration = FireSyringe.baseFireDuration / this.attackSpeedStat;
		}

		// Token: 0x06002B77 RID: 11127 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002B78 RID: 11128 RVA: 0x000B750C File Offset: 0x000B570C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			int num = Mathf.FloorToInt(base.fixedAge / this.fireDuration * (float)FireSyringe.projectileCount);
			if (this.projectilesFired <= num && this.projectilesFired < FireSyringe.projectileCount)
			{
				GameObject prefab = FireSyringe.projectilePrefab;
				string soundString = FireSyringe.attackSound;
				if (this.projectilesFired == FireSyringe.projectileCount - 1)
				{
					prefab = FireSyringe.finalProjectilePrefab;
					soundString = FireSyringe.finalAttackSound;
				}
				base.PlayAnimation("Gesture, Additive", "FireSyringe");
				Util.PlaySound(soundString, base.gameObject);
				base.characterBody.SetAimTimer(3f);
				if (FireSyringe.muzzleflashEffectPrefab)
				{
					EffectManager.SimpleMuzzleFlash(FireSyringe.muzzleflashEffectPrefab, base.gameObject, FireSyringe.muzzleName, false);
				}
				if (base.isAuthority)
				{
					Ray aimRay = base.GetAimRay();
					float bonusYaw = (float)Mathf.FloorToInt((float)this.projectilesFired - (float)(FireSyringe.projectileCount - 1) / 2f) / (float)(FireSyringe.projectileCount - 1) * FireSyringe.totalYawSpread;
					Vector3 forward = Util.ApplySpread(aimRay.direction, 0f, 0f, 1f, 1f, bonusYaw, 0f);
					ProjectileManager.instance.FireProjectile(prefab, aimRay.origin, Util.QuaternionSafeLookRotation(forward), base.gameObject, this.damageStat * FireSyringe.damageCoefficient, FireSyringe.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
				}
				this.projectilesFired++;
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002B79 RID: 11129 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002777 RID: 10103
		public static GameObject projectilePrefab;

		// Token: 0x04002778 RID: 10104
		public static GameObject finalProjectilePrefab;

		// Token: 0x04002779 RID: 10105
		public static GameObject muzzleflashEffectPrefab;

		// Token: 0x0400277A RID: 10106
		public static int projectileCount = 3;

		// Token: 0x0400277B RID: 10107
		public static float totalYawSpread = 5f;

		// Token: 0x0400277C RID: 10108
		public static float baseDuration = 2f;

		// Token: 0x0400277D RID: 10109
		public static float baseFireDuration = 0.2f;

		// Token: 0x0400277E RID: 10110
		public static float damageCoefficient = 1.2f;

		// Token: 0x0400277F RID: 10111
		public static float force = 20f;

		// Token: 0x04002780 RID: 10112
		public static string attackSound;

		// Token: 0x04002781 RID: 10113
		public static string finalAttackSound;

		// Token: 0x04002782 RID: 10114
		public static string muzzleName;

		// Token: 0x04002783 RID: 10115
		private float duration;

		// Token: 0x04002784 RID: 10116
		private float fireDuration;

		// Token: 0x04002785 RID: 10117
		private int projectilesFired;
	}
}
