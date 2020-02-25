using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Loader
{
	// Token: 0x020007EC RID: 2028
	public class ThrowPylon : BaseState
	{
		// Token: 0x06002E23 RID: 11811 RVA: 0x000C43E4 File Offset: 0x000C25E4
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = ThrowPylon.baseDuration / this.attackSpeedStat;
			if (base.isAuthority)
			{
				Ray aimRay = base.GetAimRay();
				FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
				{
					crit = base.RollCrit(),
					damage = this.damageStat * ThrowPylon.damageCoefficient,
					damageColorIndex = DamageColorIndex.Default,
					force = 0f,
					owner = base.gameObject,
					position = aimRay.origin,
					procChainMask = default(ProcChainMask),
					projectilePrefab = ThrowPylon.projectilePrefab,
					rotation = Quaternion.LookRotation(aimRay.direction),
					target = null
				};
				ProjectileManager.instance.FireProjectile(fireProjectileInfo);
			}
			EffectManager.SimpleMuzzleFlash(ThrowPylon.muzzleflashObject, base.gameObject, ThrowPylon.muzzleString, false);
			Util.PlaySound(ThrowPylon.soundString, base.gameObject);
		}

		// Token: 0x06002E24 RID: 11812 RVA: 0x000C44DA File Offset: 0x000C26DA
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && this.duration <= base.age)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002E25 RID: 11813 RVA: 0x0000C68F File Offset: 0x0000A88F
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Frozen;
		}

		// Token: 0x04002B2B RID: 11051
		public static GameObject projectilePrefab;

		// Token: 0x04002B2C RID: 11052
		public static float baseDuration;

		// Token: 0x04002B2D RID: 11053
		public static float damageCoefficient;

		// Token: 0x04002B2E RID: 11054
		public static string muzzleString;

		// Token: 0x04002B2F RID: 11055
		public static GameObject muzzleflashObject;

		// Token: 0x04002B30 RID: 11056
		public static string soundString;

		// Token: 0x04002B31 RID: 11057
		private float duration;
	}
}
