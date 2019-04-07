using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Paladin.PaladinWeapon
{
	// Token: 0x020000FD RID: 253
	internal class FireBigRocket : BaseState
	{
		// Token: 0x060004E1 RID: 1249 RVA: 0x00014AFC File Offset: 0x00012CFC
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(FireBigRocket.soundEffectString, base.gameObject);
			this.duration = FireBigRocket.baseDuration / this.attackSpeedStat;
			base.characterBody.AddSpreadBloom(1f);
			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 2f, false);
			string muzzleName = "MuzzleCenter";
			if (FireBigRocket.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireBigRocket.effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(FireBigRocket.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireBigRocket.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x060004E2 RID: 1250 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x060004E3 RID: 1251 RVA: 0x00014BDD File Offset: 0x00012DDD
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060004E4 RID: 1252 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040004C0 RID: 1216
		public static GameObject projectilePrefab;

		// Token: 0x040004C1 RID: 1217
		public static GameObject effectPrefab;

		// Token: 0x040004C2 RID: 1218
		public static string soundEffectString;

		// Token: 0x040004C3 RID: 1219
		public static float damageCoefficient;

		// Token: 0x040004C4 RID: 1220
		public static float force;

		// Token: 0x040004C5 RID: 1221
		public static float baseDuration = 2f;

		// Token: 0x040004C6 RID: 1222
		private float duration;
	}
}
