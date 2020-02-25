using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.HAND.Weapon
{
	// Token: 0x02000846 RID: 2118
	public class FireWinch : BaseState
	{
		// Token: 0x06002FEE RID: 12270 RVA: 0x000CD688 File Offset: 0x000CB888
		public override void OnEnter()
		{
			base.OnEnter();
			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 2f, false);
			string muzzleName = "WinchHole";
			if (FireWinch.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireWinch.effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(FireWinch.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireWinch.damageCoefficient, FireWinch.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x06002FEF RID: 12271 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002FF0 RID: 12272 RVA: 0x000CD731 File Offset: 0x000CB931
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= FireWinch.duration / this.attackSpeedStat && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002FF1 RID: 12273 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002DBD RID: 11709
		public static GameObject projectilePrefab;

		// Token: 0x04002DBE RID: 11710
		public static GameObject effectPrefab;

		// Token: 0x04002DBF RID: 11711
		public static float duration = 2f;

		// Token: 0x04002DC0 RID: 11712
		public static float baseDuration = 2f;

		// Token: 0x04002DC1 RID: 11713
		public static float damageCoefficient = 1.2f;

		// Token: 0x04002DC2 RID: 11714
		public static float force = 20f;
	}
}
