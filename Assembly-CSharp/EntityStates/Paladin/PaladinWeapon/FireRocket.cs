using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Paladin.PaladinWeapon
{
	// Token: 0x020000FE RID: 254
	internal class FireRocket : BaseState
	{
		// Token: 0x060004E7 RID: 1255 RVA: 0x00014C14 File Offset: 0x00012E14
		public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(FireRocket.soundEffectString, base.gameObject);
			this.duration = FireRocket.baseDuration / this.attackSpeedStat;
			base.characterBody.AddSpreadBloom(0.3f);
			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 2f, false);
			string muzzleName = "MuzzleCenter";
			if (FireRocket.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireRocket.effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(FireRocket.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireRocket.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x060004E8 RID: 1256 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x060004E9 RID: 1257 RVA: 0x00014CF5 File Offset: 0x00012EF5
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060004EA RID: 1258 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040004C7 RID: 1223
		public static GameObject projectilePrefab;

		// Token: 0x040004C8 RID: 1224
		public static GameObject effectPrefab;

		// Token: 0x040004C9 RID: 1225
		public static string soundEffectString;

		// Token: 0x040004CA RID: 1226
		public static float damageCoefficient;

		// Token: 0x040004CB RID: 1227
		public static float force;

		// Token: 0x040004CC RID: 1228
		public static float baseDuration = 2f;

		// Token: 0x040004CD RID: 1229
		private float duration;

		// Token: 0x040004CE RID: 1230
		public int bulletCountCurrent = 1;
	}
}
