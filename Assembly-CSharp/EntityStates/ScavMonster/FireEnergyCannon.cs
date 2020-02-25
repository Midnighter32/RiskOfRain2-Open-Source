using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.ScavMonster
{
	// Token: 0x0200078F RID: 1935
	public class FireEnergyCannon : EnergyCannonState
	{
		// Token: 0x06002C66 RID: 11366 RVA: 0x000BB52C File Offset: 0x000B972C
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireEnergyCannon.baseDuration / this.attackSpeedStat;
			this.refireDuration = FireEnergyCannon.baseRefireDuration / this.attackSpeedStat;
			Util.PlayScaledSound(FireEnergyCannon.sound, base.gameObject, this.attackSpeedStat);
			base.PlayCrossfade("Body", "FireEnergyCannon", "FireEnergyCannon.playbackRate", this.duration, 0.1f);
			base.AddRecoil(-2f * FireEnergyCannon.recoilAmplitude, -3f * FireEnergyCannon.recoilAmplitude, -1f * FireEnergyCannon.recoilAmplitude, 1f * FireEnergyCannon.recoilAmplitude);
			if (FireEnergyCannon.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireEnergyCannon.effectPrefab, base.gameObject, EnergyCannonState.muzzleName, false);
			}
			if (base.isAuthority)
			{
				float num = (float)((this.currentRefire % 2 == 0) ? 1 : -1);
				float num2 = Mathf.Ceil((float)this.currentRefire / 2f) * FireEnergyCannon.projectileYawBonusPerRefire;
				for (int i = 0; i < FireEnergyCannon.projectileCount; i++)
				{
					Ray aimRay = base.GetAimRay();
					aimRay.direction = Util.ApplySpread(aimRay.direction, FireEnergyCannon.minSpread, FireEnergyCannon.maxSpread, 1f, 1f, num * num2, FireEnergyCannon.projectilePitchBonus);
					ProjectileManager.instance.FireProjectile(FireEnergyCannon.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireEnergyCannon.damageCoefficient, FireEnergyCannon.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
				}
			}
		}

		// Token: 0x06002C67 RID: 11367 RVA: 0x000BB6C4 File Offset: 0x000B98C4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.refireDuration && this.currentRefire + 1 < FireEnergyCannon.maxRefireCount && base.isAuthority)
			{
				FireEnergyCannon fireEnergyCannon = new FireEnergyCannon();
				fireEnergyCannon.currentRefire = this.currentRefire + 1;
				this.outer.SetNextState(fireEnergyCannon);
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x04002868 RID: 10344
		public static float baseDuration;

		// Token: 0x04002869 RID: 10345
		public static float baseRefireDuration;

		// Token: 0x0400286A RID: 10346
		public static string sound;

		// Token: 0x0400286B RID: 10347
		public static GameObject effectPrefab;

		// Token: 0x0400286C RID: 10348
		public static GameObject projectilePrefab;

		// Token: 0x0400286D RID: 10349
		public static float damageCoefficient;

		// Token: 0x0400286E RID: 10350
		public static float force;

		// Token: 0x0400286F RID: 10351
		public static float minSpread;

		// Token: 0x04002870 RID: 10352
		public static float maxSpread;

		// Token: 0x04002871 RID: 10353
		public static float recoilAmplitude = 1f;

		// Token: 0x04002872 RID: 10354
		public static float projectilePitchBonus;

		// Token: 0x04002873 RID: 10355
		public static float projectileYawBonusPerRefire;

		// Token: 0x04002874 RID: 10356
		public static int projectileCount;

		// Token: 0x04002875 RID: 10357
		public static int maxRefireCount;

		// Token: 0x04002876 RID: 10358
		public int currentRefire;

		// Token: 0x04002877 RID: 10359
		private float duration;

		// Token: 0x04002878 RID: 10360
		private float refireDuration;
	}
}
