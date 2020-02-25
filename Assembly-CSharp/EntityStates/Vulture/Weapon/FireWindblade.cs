using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Vulture.Weapon
{
	// Token: 0x0200073E RID: 1854
	public class FireWindblade : BaseSkillState
	{
		// Token: 0x06002B08 RID: 11016 RVA: 0x000B52A8 File Offset: 0x000B34A8
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireWindblade.baseDuration / this.attackSpeedStat;
			base.PlayAnimation("Gesture, Additive", "FireWindblade", "FireWindblade.playbackRate", this.duration);
			Util.PlaySound(FireWindblade.soundString, base.gameObject);
			if (FireWindblade.muzzleEffectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireWindblade.muzzleEffectPrefab, base.gameObject, FireWindblade.muzzleString, false);
			}
			Ray aimRay = base.GetAimRay();
			if (base.isAuthority)
			{
				Quaternion rhs = Util.QuaternionSafeLookRotation(aimRay.direction);
				Quaternion lhs = Quaternion.AngleAxis(UnityEngine.Random.Range(0f, 360f), aimRay.direction);
				ProjectileManager.instance.FireProjectile(FireWindblade.projectilePrefab, aimRay.origin, lhs * rhs, base.gameObject, this.damageStat * FireWindblade.damageCoefficient, FireWindblade.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x06002B09 RID: 11017 RVA: 0x000B53A4 File Offset: 0x000B35A4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x040026E1 RID: 9953
		public static float baseDuration;

		// Token: 0x040026E2 RID: 9954
		public static string muzzleString;

		// Token: 0x040026E3 RID: 9955
		public static GameObject muzzleEffectPrefab;

		// Token: 0x040026E4 RID: 9956
		public static GameObject projectilePrefab;

		// Token: 0x040026E5 RID: 9957
		public static float damageCoefficient = 1.2f;

		// Token: 0x040026E6 RID: 9958
		public static float force = 20f;

		// Token: 0x040026E7 RID: 9959
		public static string soundString;

		// Token: 0x040026E8 RID: 9960
		private float duration;
	}
}
