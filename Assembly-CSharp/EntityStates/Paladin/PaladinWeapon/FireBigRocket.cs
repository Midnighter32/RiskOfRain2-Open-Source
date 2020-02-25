using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Paladin.PaladinWeapon
{
	// Token: 0x020007AE RID: 1966
	public class FireBigRocket : BaseState
	{
		// Token: 0x06002CF5 RID: 11509 RVA: 0x000BDD38 File Offset: 0x000BBF38
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
				EffectManager.SimpleMuzzleFlash(FireBigRocket.effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(FireBigRocket.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireBigRocket.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x06002CF6 RID: 11510 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002CF7 RID: 11511 RVA: 0x000BDE14 File Offset: 0x000BC014
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002CF8 RID: 11512 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002929 RID: 10537
		public static GameObject projectilePrefab;

		// Token: 0x0400292A RID: 10538
		public static GameObject effectPrefab;

		// Token: 0x0400292B RID: 10539
		public static string soundEffectString;

		// Token: 0x0400292C RID: 10540
		public static float damageCoefficient;

		// Token: 0x0400292D RID: 10541
		public static float force;

		// Token: 0x0400292E RID: 10542
		public static float baseDuration = 2f;

		// Token: 0x0400292F RID: 10543
		private float duration;
	}
}
