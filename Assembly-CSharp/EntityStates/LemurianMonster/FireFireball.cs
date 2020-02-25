using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.LemurianMonster
{
	// Token: 0x020007F4 RID: 2036
	public class FireFireball : BaseState
	{
		// Token: 0x06002E4E RID: 11854 RVA: 0x000C5120 File Offset: 0x000C3320
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireFireball.baseDuration / this.attackSpeedStat;
			base.PlayAnimation("Gesture", "FireFireball", "FireFireball.playbackRate", this.duration);
			Util.PlaySound(FireFireball.attackString, base.gameObject);
			Ray aimRay = base.GetAimRay();
			string muzzleName = "MuzzleMouth";
			if (FireFireball.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireFireball.effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(FireFireball.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireFireball.damageCoefficient, FireFireball.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x06002E4F RID: 11855 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002E50 RID: 11856 RVA: 0x000C51FA File Offset: 0x000C33FA
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002E51 RID: 11857 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002B75 RID: 11125
		public static GameObject projectilePrefab;

		// Token: 0x04002B76 RID: 11126
		public static GameObject effectPrefab;

		// Token: 0x04002B77 RID: 11127
		public static float baseDuration = 2f;

		// Token: 0x04002B78 RID: 11128
		public static float damageCoefficient = 1.2f;

		// Token: 0x04002B79 RID: 11129
		public static float force = 20f;

		// Token: 0x04002B7A RID: 11130
		public static string attackString;

		// Token: 0x04002B7B RID: 11131
		private float duration;
	}
}
