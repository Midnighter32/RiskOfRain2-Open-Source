using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.TitanMonster
{
	// Token: 0x02000858 RID: 2136
	public class FireGoldMegaLaser : FireMegaLaser
	{
		// Token: 0x0600303B RID: 12347 RVA: 0x000CF430 File Offset: 0x000CD630
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				this.projectileStopwatch += Time.fixedDeltaTime * this.attackSpeedStat;
				if (this.projectileStopwatch >= 1f / FireGoldMegaLaser.projectileFireFrequency)
				{
					Ray aimRay = base.GetAimRay();
					if (this.muzzleTransform)
					{
						aimRay.origin = this.muzzleTransform.transform.position;
					}
					aimRay.direction = Util.ApplySpread(aimRay.direction, FireGoldMegaLaser.projectileMinSpread, FireGoldMegaLaser.projectileMaxSpread, 1f, 1f, 0f, 0f);
					this.projectileStopwatch -= 1f / FireGoldMegaLaser.projectileFireFrequency;
					ProjectileManager.instance.FireProjectile(FireGoldMegaLaser.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireMegaLaser.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
				}
			}
		}

		// Token: 0x04002E47 RID: 11847
		public static GameObject projectilePrefab;

		// Token: 0x04002E48 RID: 11848
		public static float projectileFireFrequency;

		// Token: 0x04002E49 RID: 11849
		public static float projectileDamageCoefficient;

		// Token: 0x04002E4A RID: 11850
		public static float projectileMinSpread;

		// Token: 0x04002E4B RID: 11851
		public static float projectileMaxSpread;

		// Token: 0x04002E4C RID: 11852
		private float projectileStopwatch;
	}
}
