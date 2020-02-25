using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020008C3 RID: 2243
	public class FireThermite : BaseState
	{
		// Token: 0x0600324C RID: 12876 RVA: 0x000D9968 File Offset: 0x000D7B68
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
				EffectManager.SimpleMuzzleFlash(FireThermite.effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(FireThermite.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireThermite.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
			if (base.characterMotor && !base.characterMotor.isGrounded)
			{
				Vector3 vector = -aimRay.direction * FireThermite.selfForce;
				vector.y *= 0.5f;
				base.characterMotor.ApplyForce(vector, true, false);
			}
		}

		// Token: 0x0600324D RID: 12877 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x0600324E RID: 12878 RVA: 0x000D9A8D File Offset: 0x000D7C8D
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x0600324F RID: 12879 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04003141 RID: 12609
		public static GameObject effectPrefab;

		// Token: 0x04003142 RID: 12610
		public static GameObject projectilePrefab;

		// Token: 0x04003143 RID: 12611
		public static float damageCoefficient;

		// Token: 0x04003144 RID: 12612
		public static float force;

		// Token: 0x04003145 RID: 12613
		public static float selfForce;

		// Token: 0x04003146 RID: 12614
		public static float baseDuration = 2f;

		// Token: 0x04003147 RID: 12615
		private float duration;

		// Token: 0x04003148 RID: 12616
		public int bulletCountCurrent = 1;
	}
}
