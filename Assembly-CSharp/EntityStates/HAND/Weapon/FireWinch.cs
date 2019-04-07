using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.HAND.Weapon
{
	// Token: 0x02000164 RID: 356
	internal class FireWinch : BaseState
	{
		// Token: 0x060006E9 RID: 1769 RVA: 0x00020F78 File Offset: 0x0001F178
		public override void OnEnter()
		{
			base.OnEnter();
			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 2f, false);
			string muzzleName = "WinchHole";
			if (FireWinch.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireWinch.effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(FireWinch.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireWinch.damageCoefficient, FireWinch.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x060006EA RID: 1770 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x060006EB RID: 1771 RVA: 0x00021026 File Offset: 0x0001F226
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= FireWinch.duration / this.attackSpeedStat && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060006EC RID: 1772 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000877 RID: 2167
		public static GameObject projectilePrefab;

		// Token: 0x04000878 RID: 2168
		public static GameObject effectPrefab;

		// Token: 0x04000879 RID: 2169
		public static float duration = 2f;

		// Token: 0x0400087A RID: 2170
		public static float baseDuration = 2f;

		// Token: 0x0400087B RID: 2171
		public static float damageCoefficient = 1.2f;

		// Token: 0x0400087C RID: 2172
		public static float force = 20f;
	}
}
