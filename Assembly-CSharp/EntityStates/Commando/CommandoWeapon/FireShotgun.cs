using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020008BF RID: 2239
	public class FireShotgun : BaseState
	{
		// Token: 0x06003237 RID: 12855 RVA: 0x000D8F78 File Offset: 0x000D7178
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
				EffectManager.SimpleMuzzleFlash(FireShotgun.effectPrefab, base.gameObject, muzzleName, false);
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

		// Token: 0x06003238 RID: 12856 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06003239 RID: 12857 RVA: 0x000D9180 File Offset: 0x000D7380
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

		// Token: 0x0600323A RID: 12858 RVA: 0x000D91D5 File Offset: 0x000D73D5
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			if (this.buttonReleased && base.fixedAge >= this.minDuration)
			{
				return InterruptPriority.Any;
			}
			return InterruptPriority.Skill;
		}

		// Token: 0x0400310A RID: 12554
		public static GameObject effectPrefab;

		// Token: 0x0400310B RID: 12555
		public static GameObject hitEffectPrefab;

		// Token: 0x0400310C RID: 12556
		public static GameObject tracerEffectPrefab;

		// Token: 0x0400310D RID: 12557
		public static float damageCoefficient;

		// Token: 0x0400310E RID: 12558
		public static float force;

		// Token: 0x0400310F RID: 12559
		public static int bulletCount;

		// Token: 0x04003110 RID: 12560
		public static float baseMaxDuration = 2f;

		// Token: 0x04003111 RID: 12561
		public static float baseMinDuration = 0.5f;

		// Token: 0x04003112 RID: 12562
		public static string attackSoundString;

		// Token: 0x04003113 RID: 12563
		public static float recoilAmplitude;

		// Token: 0x04003114 RID: 12564
		public static float spreadBloomValue = 0.3f;

		// Token: 0x04003115 RID: 12565
		private float maxDuration;

		// Token: 0x04003116 RID: 12566
		private float minDuration;

		// Token: 0x04003117 RID: 12567
		private bool buttonReleased;
	}
}
