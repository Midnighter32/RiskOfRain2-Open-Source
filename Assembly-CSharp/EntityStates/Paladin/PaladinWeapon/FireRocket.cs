using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Paladin.PaladinWeapon
{
	// Token: 0x020007AF RID: 1967
	public class FireRocket : BaseState
	{
		// Token: 0x06002CFB RID: 11515 RVA: 0x000BDE4C File Offset: 0x000BC04C
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
				EffectManager.SimpleMuzzleFlash(FireRocket.effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(FireRocket.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireRocket.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x06002CFC RID: 11516 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002CFD RID: 11517 RVA: 0x000BDF28 File Offset: 0x000BC128
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002CFE RID: 11518 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04002930 RID: 10544
		public static GameObject projectilePrefab;

		// Token: 0x04002931 RID: 10545
		public static GameObject effectPrefab;

		// Token: 0x04002932 RID: 10546
		public static string soundEffectString;

		// Token: 0x04002933 RID: 10547
		public static float damageCoefficient;

		// Token: 0x04002934 RID: 10548
		public static float force;

		// Token: 0x04002935 RID: 10549
		public static float baseDuration = 2f;

		// Token: 0x04002936 RID: 10550
		private float duration;

		// Token: 0x04002937 RID: 10551
		public int bulletCountCurrent = 1;
	}
}
