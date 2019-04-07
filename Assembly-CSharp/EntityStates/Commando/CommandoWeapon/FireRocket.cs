using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020001AB RID: 427
	internal class FireRocket : BaseState
	{
		// Token: 0x06000858 RID: 2136 RVA: 0x00029DC8 File Offset: 0x00027FC8
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireRocket.baseDuration / this.attackSpeedStat;
			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 2f, false);
			string muzzleName = "MuzzleCenter";
			base.PlayAnimation("Gesture", "FireFMJ", "FireFMJ.playbackRate", this.duration);
			if (FireRocket.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireRocket.effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(FireRocket.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireRocket.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x06000859 RID: 2137 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x0600085A RID: 2138 RVA: 0x00029EA3 File Offset: 0x000280A3
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x0600085B RID: 2139 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000B1D RID: 2845
		public static GameObject projectilePrefab;

		// Token: 0x04000B1E RID: 2846
		public static GameObject effectPrefab;

		// Token: 0x04000B1F RID: 2847
		public static float damageCoefficient;

		// Token: 0x04000B20 RID: 2848
		public static float force;

		// Token: 0x04000B21 RID: 2849
		public static float baseDuration = 2f;

		// Token: 0x04000B22 RID: 2850
		private float duration;

		// Token: 0x04000B23 RID: 2851
		public int bulletCountCurrent = 1;
	}
}
