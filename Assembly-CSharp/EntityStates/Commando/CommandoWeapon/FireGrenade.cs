using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020001A6 RID: 422
	internal class FireGrenade : BaseState
	{
		// Token: 0x06000836 RID: 2102 RVA: 0x000291DC File Offset: 0x000273DC
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireGrenade.baseDuration / this.attackSpeedStat;
			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 2f, false);
			base.PlayAnimation("Gesture", "FireFMJ", "FireFMJ.playbackRate", this.duration);
			string muzzleName = "MuzzleCenter";
			if (FireGrenade.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireGrenade.effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(FireGrenade.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireGrenade.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
			if (base.characterMotor && !base.characterMotor.isGrounded)
			{
				Vector3 vector = -aimRay.direction * FireGrenade.selfForce;
				vector.y *= 0.5f;
				base.characterMotor.ApplyForce(vector, true);
			}
		}

		// Token: 0x06000837 RID: 2103 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000838 RID: 2104 RVA: 0x00029305 File Offset: 0x00027505
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06000839 RID: 2105 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000ADB RID: 2779
		public static GameObject effectPrefab;

		// Token: 0x04000ADC RID: 2780
		public static GameObject projectilePrefab;

		// Token: 0x04000ADD RID: 2781
		public static float damageCoefficient;

		// Token: 0x04000ADE RID: 2782
		public static float force;

		// Token: 0x04000ADF RID: 2783
		public static float selfForce;

		// Token: 0x04000AE0 RID: 2784
		public static float baseDuration = 2f;

		// Token: 0x04000AE1 RID: 2785
		private float duration;

		// Token: 0x04000AE2 RID: 2786
		public int bulletCountCurrent = 1;
	}
}
