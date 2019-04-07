using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020001AC RID: 428
	internal class FireShotgun : BaseState
	{
		// Token: 0x0600085E RID: 2142 RVA: 0x00029EE8 File Offset: 0x000280E8
		public override void OnEnter()
		{
			base.OnEnter();
			base.AddRecoil(-1f * FireShotgun.recoilAmplitude, -2f * FireShotgun.recoilAmplitude, -0.5f * FireShotgun.recoilAmplitude, 0.5f * FireShotgun.recoilAmplitude);
			this.maxDuration = FireShotgun.baseMaxDuration / this.attackSpeedStat;
			this.minDuration = FireShotgun.baseMinDuration / this.attackSpeedStat;
			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 2f, false);
			Util.PlaySound(FireShotgun.attackSoundString, base.gameObject);
			base.PlayAnimation("Gesture, Additive", "FireShotgun", "FireShotgun.playbackRate", this.maxDuration * 1.1f);
			base.PlayAnimation("Gesture, Override", "FireShotgun", "FireShotgun.playbackRate", this.maxDuration * 1.1f);
			string muzzleName = "MuzzleShotgun";
			if (FireShotgun.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireShotgun.effectPrefab, base.gameObject, muzzleName, false);
			}
			if (base.isAuthority)
			{
				new BulletAttack
				{
					owner = base.gameObject,
					weapon = base.gameObject,
					origin = aimRay.origin,
					aimVector = aimRay.direction,
					minSpread = 0f,
					maxSpread = base.characterBody.spreadBloomAngle,
					bulletCount = (uint)((FireShotgun.bulletCount > 0) ? FireShotgun.bulletCount : 0),
					procCoefficient = 1f / (float)FireShotgun.bulletCount,
					damage = FireShotgun.damageCoefficient * this.damageStat / (float)FireShotgun.bulletCount,
					force = FireShotgun.force,
					falloffModel = BulletAttack.FalloffModel.DefaultBullet,
					tracerEffectPrefab = FireShotgun.tracerEffectPrefab,
					muzzleName = muzzleName,
					hitEffectPrefab = FireShotgun.hitEffectPrefab,
					isCrit = Util.CheckRoll(this.critStat, base.characterBody.master),
					HitEffectNormal = false,
					radius = 0f
				}.Fire();
			}
			base.characterBody.AddSpreadBloom(FireShotgun.spreadBloomValue);
		}

		// Token: 0x0600085F RID: 2143 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000860 RID: 2144 RVA: 0x0002A0F4 File Offset: 0x000282F4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.buttonReleased |= !base.inputBank.skill1.down;
			if (base.fixedAge >= this.maxDuration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06000861 RID: 2145 RVA: 0x0002A149 File Offset: 0x00028349
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			if (this.buttonReleased && base.fixedAge >= this.minDuration)
			{
				return InterruptPriority.Any;
			}
			return InterruptPriority.Skill;
		}

		// Token: 0x04000B24 RID: 2852
		public static GameObject effectPrefab;

		// Token: 0x04000B25 RID: 2853
		public static GameObject hitEffectPrefab;

		// Token: 0x04000B26 RID: 2854
		public static GameObject tracerEffectPrefab;

		// Token: 0x04000B27 RID: 2855
		public static float damageCoefficient;

		// Token: 0x04000B28 RID: 2856
		public static float force;

		// Token: 0x04000B29 RID: 2857
		public static int bulletCount;

		// Token: 0x04000B2A RID: 2858
		public static float baseMaxDuration = 2f;

		// Token: 0x04000B2B RID: 2859
		public static float baseMinDuration = 0.5f;

		// Token: 0x04000B2C RID: 2860
		public static string attackSoundString;

		// Token: 0x04000B2D RID: 2861
		public static float recoilAmplitude;

		// Token: 0x04000B2E RID: 2862
		public static float spreadBloomValue = 0.3f;

		// Token: 0x04000B2F RID: 2863
		private float maxDuration;

		// Token: 0x04000B30 RID: 2864
		private float minDuration;

		// Token: 0x04000B31 RID: 2865
		private bool buttonReleased;
	}
}
