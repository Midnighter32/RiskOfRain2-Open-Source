using System;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Loader
{
	// Token: 0x020007EA RID: 2026
	public class SwingZapFist : BaseSwingChargedFist
	{
		// Token: 0x06002E19 RID: 11801 RVA: 0x000C4224 File Offset: 0x000C2424
		protected override void OnMeleeHitAuthority()
		{
			if (this.hasHit)
			{
				return;
			}
			base.OnMeleeHitAuthority();
			this.hasHit = true;
			if (base.FindModelChild(this.swingEffectMuzzleString))
			{
				FireProjectileInfo fireProjectileInfo = default(FireProjectileInfo);
				fireProjectileInfo.position = base.FindModelChild(this.swingEffectMuzzleString).position;
				fireProjectileInfo.rotation = Quaternion.LookRotation(this.punchVelocity);
				fireProjectileInfo.crit = base.isCritAuthority;
				fireProjectileInfo.damage = 1f * this.damageStat;
				fireProjectileInfo.owner = base.gameObject;
				fireProjectileInfo.projectilePrefab = Resources.Load<GameObject>("Prefabs/Projectiles/LoaderZapCone");
				ProjectileManager.instance.FireProjectile(fireProjectileInfo);
			}
		}

		// Token: 0x06002E1A RID: 11802 RVA: 0x000C42D8 File Offset: 0x000C24D8
		protected override void AuthorityExitHitPause()
		{
			base.AuthorityExitHitPause();
			if (base.healthComponent)
			{
				Vector3 force = this.punchVelocity.normalized * -SwingZapFist.selfKnockback;
				base.healthComponent.TakeDamageForce(force, true, false);
			}
			this.outer.SetNextStateToMain();
		}

		// Token: 0x04002B28 RID: 11048
		public static float selfKnockback;

		// Token: 0x04002B29 RID: 11049
		private bool hasHit;
	}
}
