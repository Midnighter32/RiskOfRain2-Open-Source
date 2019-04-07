using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020001AE RID: 430
	internal class FireThermite : BaseState
	{
		// Token: 0x0600086A RID: 2154 RVA: 0x0002A3E4 File Offset: 0x000285E4
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireThermite.baseDuration / this.attackSpeedStat;
			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 2f, false);
			base.PlayAnimation("Gesture", "FireFMJ", "FireFMJ.playbackRate", this.duration);
			string muzzleName = "MuzzleCenter";
			if (FireThermite.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireThermite.effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(FireThermite.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireThermite.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
			if (base.characterMotor && !base.characterMotor.isGrounded)
			{
				Vector3 vector = -aimRay.direction * FireThermite.selfForce;
				vector.y *= 0.5f;
				base.characterMotor.ApplyForce(vector, true);
			}
		}

		// Token: 0x0600086B RID: 2155 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x0600086C RID: 2156 RVA: 0x0002A50D File Offset: 0x0002870D
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x0600086D RID: 2157 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000B40 RID: 2880
		public static GameObject effectPrefab;

		// Token: 0x04000B41 RID: 2881
		public static GameObject projectilePrefab;

		// Token: 0x04000B42 RID: 2882
		public static float damageCoefficient;

		// Token: 0x04000B43 RID: 2883
		public static float force;

		// Token: 0x04000B44 RID: 2884
		public static float selfForce;

		// Token: 0x04000B45 RID: 2885
		public static float baseDuration = 2f;

		// Token: 0x04000B46 RID: 2886
		private float duration;

		// Token: 0x04000B47 RID: 2887
		public int bulletCountCurrent = 1;
	}
}
