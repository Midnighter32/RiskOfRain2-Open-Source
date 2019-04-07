using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.TitanMonster
{
	// Token: 0x02000172 RID: 370
	internal class FireGoldMegaLaser : FireMegaLaser
	{
		// Token: 0x06000720 RID: 1824 RVA: 0x00022640 File Offset: 0x00020840
		public override void FixedUpdate()
		{
			base.FixedUpdate();
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

		// Token: 0x040008DB RID: 2267
		public static GameObject projectilePrefab;

		// Token: 0x040008DC RID: 2268
		public static float projectileFireFrequency;

		// Token: 0x040008DD RID: 2269
		public static float projectileDamageCoefficient;

		// Token: 0x040008DE RID: 2270
		public static float projectileMinSpread;

		// Token: 0x040008DF RID: 2271
		public static float projectileMaxSpread;

		// Token: 0x040008E0 RID: 2272
		private float projectileStopwatch;
	}
}
