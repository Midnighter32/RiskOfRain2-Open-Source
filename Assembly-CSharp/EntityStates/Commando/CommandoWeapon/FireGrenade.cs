using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020008B9 RID: 2233
	public class FireGrenade : BaseState
	{
		// Token: 0x0600320F RID: 12815 RVA: 0x000D82DC File Offset: 0x000D64DC
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
				EffectManager.SimpleMuzzleFlash(FireGrenade.effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(FireGrenade.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireGrenade.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
			if (base.characterMotor && !base.characterMotor.isGrounded)
			{
				Vector3 vector = -aimRay.direction * FireGrenade.selfForce;
				vector.y *= 0.5f;
				base.characterMotor.ApplyForce(vector, true, false);
			}
		}

		// Token: 0x06003210 RID: 12816 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06003211 RID: 12817 RVA: 0x000D8401 File Offset: 0x000D6601
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06003212 RID: 12818 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040030C5 RID: 12485
		public static GameObject effectPrefab;

		// Token: 0x040030C6 RID: 12486
		public static GameObject projectilePrefab;

		// Token: 0x040030C7 RID: 12487
		public static float damageCoefficient;

		// Token: 0x040030C8 RID: 12488
		public static float force;

		// Token: 0x040030C9 RID: 12489
		public static float selfForce;

		// Token: 0x040030CA RID: 12490
		public static float baseDuration = 2f;

		// Token: 0x040030CB RID: 12491
		private float duration;

		// Token: 0x040030CC RID: 12492
		public int bulletCountCurrent = 1;
	}
}
