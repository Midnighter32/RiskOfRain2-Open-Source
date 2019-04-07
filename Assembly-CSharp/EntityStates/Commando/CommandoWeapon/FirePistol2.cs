using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020001AA RID: 426
	internal class FirePistol2 : BaseState
	{
		// Token: 0x06000852 RID: 2130 RVA: 0x00029AF4 File Offset: 0x00027CF4
		private void FireBullet(string targetMuzzle)
		{
			Util.PlaySound(this.boosted ? FirePistol2.boostedFirePistolSoundString : FirePistol2.firePistolSoundString, base.gameObject);
			if (FirePistol2.muzzleEffectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(this.boosted ? FirePistol2.boostedMuzzleEffectPrefab : FirePistol2.muzzleEffectPrefab, base.gameObject, targetMuzzle, false);
			}
			base.AddRecoil(-0.4f * FirePistol2.recoilAmplitude, -0.8f * FirePistol2.recoilAmplitude, -0.3f * FirePistol2.recoilAmplitude, 0.3f * FirePistol2.recoilAmplitude);
			if (base.isAuthority)
			{
				new BulletAttack
				{
					owner = base.gameObject,
					weapon = base.gameObject,
					origin = this.aimRay.origin,
					aimVector = this.aimRay.direction,
					minSpread = 0f,
					maxSpread = base.characterBody.spreadBloomAngle,
					damage = FirePistol2.damageCoefficient * this.damageStat,
					force = FirePistol2.force,
					tracerEffectPrefab = (this.boosted ? FirePistol2.boostedTracerEffectPrefab : FirePistol2.tracerEffectPrefab),
					muzzleName = targetMuzzle,
					hitEffectPrefab = FirePistol2.hitEffectPrefab,
					isCrit = Util.CheckRoll(this.critStat, base.characterBody.master),
					radius = 0.1f,
					smartCollision = true
				}.Fire();
			}
			base.characterBody.AddSpreadBloom(FirePistol2.spreadBloomValue);
		}

		// Token: 0x06000853 RID: 2131 RVA: 0x00029C78 File Offset: 0x00027E78
		public override void OnEnter()
		{
			base.OnEnter();
			this.boosted = base.characterBody.HasBuff(BuffIndex.CommandoBoost);
			this.duration = FirePistol2.baseDuration / (this.attackSpeedStat * (this.boosted ? FirePistol2.commandoBoostBuffCoefficient : 1f));
			this.aimRay = base.GetAimRay();
			base.StartAimMode(this.aimRay, 3f, false);
			if (this.remainingShots % 2 == 0)
			{
				base.PlayAnimation("Gesture Additive, Left", "FirePistol, Left");
				this.FireBullet("MuzzleLeft");
				return;
			}
			base.PlayAnimation("Gesture Additive, Right", "FirePistol, Right");
			this.FireBullet("MuzzleRight");
		}

		// Token: 0x06000854 RID: 2132 RVA: 0x00029D24 File Offset: 0x00027F24
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge < this.duration || !base.isAuthority)
			{
				return;
			}
			this.remainingShots--;
			if (this.remainingShots == 0)
			{
				this.outer.SetNextStateToMain();
				return;
			}
			FirePistol2 firePistol = new FirePistol2();
			firePistol.remainingShots = this.remainingShots;
			this.outer.SetNextState(firePistol);
		}

		// Token: 0x06000855 RID: 2133 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000B0C RID: 2828
		public static GameObject muzzleEffectPrefab;

		// Token: 0x04000B0D RID: 2829
		public static GameObject boostedMuzzleEffectPrefab;

		// Token: 0x04000B0E RID: 2830
		public static GameObject hitEffectPrefab;

		// Token: 0x04000B0F RID: 2831
		public static GameObject tracerEffectPrefab;

		// Token: 0x04000B10 RID: 2832
		public static GameObject boostedTracerEffectPrefab;

		// Token: 0x04000B11 RID: 2833
		public static float damageCoefficient;

		// Token: 0x04000B12 RID: 2834
		public static float force;

		// Token: 0x04000B13 RID: 2835
		public static float baseDuration = 2f;

		// Token: 0x04000B14 RID: 2836
		public static string firePistolSoundString;

		// Token: 0x04000B15 RID: 2837
		public static string boostedFirePistolSoundString;

		// Token: 0x04000B16 RID: 2838
		public static float recoilAmplitude = 1f;

		// Token: 0x04000B17 RID: 2839
		public static float spreadBloomValue = 0.3f;

		// Token: 0x04000B18 RID: 2840
		public static float commandoBoostBuffCoefficient = 0.4f;

		// Token: 0x04000B19 RID: 2841
		public int remainingShots = 2;

		// Token: 0x04000B1A RID: 2842
		private Ray aimRay;

		// Token: 0x04000B1B RID: 2843
		private float duration;

		// Token: 0x04000B1C RID: 2844
		private bool boosted;
	}
}
