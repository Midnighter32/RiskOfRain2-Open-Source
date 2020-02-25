using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020008BC RID: 2236
	public class FirePistol : BaseState
	{
		// Token: 0x06003224 RID: 12836 RVA: 0x000D89D4 File Offset: 0x000D6BD4
		private void FireBullet(string targetMuzzle)
		{
			if (FirePistol.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FirePistol.effectPrefab, base.gameObject, targetMuzzle, false);
			}
			base.AddRecoil(-0.4f * FirePistol.recoilAmplitude, -0.8f * FirePistol.recoilAmplitude, -0.3f * FirePistol.recoilAmplitude, 0.3f * FirePistol.recoilAmplitude);
			if (base.isAuthority)
			{
				new BulletAttack
				{
					owner = base.gameObject,
					weapon = base.gameObject,
					origin = this.aimRay.origin,
					aimVector = this.aimRay.direction,
					minSpread = FirePistol.minSpread,
					maxSpread = FirePistol.maxSpread,
					damage = FirePistol.damageCoefficient * this.damageStat,
					force = FirePistol.force,
					tracerEffectPrefab = FirePistol.tracerEffectPrefab,
					muzzleName = targetMuzzle,
					hitEffectPrefab = FirePistol.hitEffectPrefab,
					isCrit = Util.CheckRoll(this.critStat, base.characterBody.master)
				}.Fire();
			}
		}

		// Token: 0x06003225 RID: 12837 RVA: 0x000D8AEC File Offset: 0x000D6CEC
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FirePistol.baseDuration / this.attackSpeedStat;
			this.aimRay = base.GetAimRay();
			base.StartAimMode(this.aimRay, 2f, false);
			this.modelAnimator = base.GetModelAnimator();
			base.PlayAnimation("Gesture", "FirePistolJoint", "FirePistol.playbackRate", this.duration);
			this.FireBullet("MuzzleLeft");
		}

		// Token: 0x06003226 RID: 12838 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06003227 RID: 12839 RVA: 0x000D8B64 File Offset: 0x000D6D64
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (!this.hasFiredSecondBullet && this.modelAnimator.GetFloat("FireSecondBullet") > 0.1f)
			{
				this.FireBullet("MuzzleRight");
				this.hasFiredSecondBullet = true;
			}
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06003228 RID: 12840 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040030E8 RID: 12520
		public static GameObject effectPrefab;

		// Token: 0x040030E9 RID: 12521
		public static GameObject hitEffectPrefab;

		// Token: 0x040030EA RID: 12522
		public static GameObject tracerEffectPrefab;

		// Token: 0x040030EB RID: 12523
		public static float damageCoefficient;

		// Token: 0x040030EC RID: 12524
		public static float force;

		// Token: 0x040030ED RID: 12525
		public static float minSpread;

		// Token: 0x040030EE RID: 12526
		public static float maxSpread;

		// Token: 0x040030EF RID: 12527
		public static float baseDuration = 2f;

		// Token: 0x040030F0 RID: 12528
		public static string firePistolSoundString;

		// Token: 0x040030F1 RID: 12529
		public static float recoilAmplitude = 1f;

		// Token: 0x040030F2 RID: 12530
		private Ray aimRay;

		// Token: 0x040030F3 RID: 12531
		private Animator modelAnimator;

		// Token: 0x040030F4 RID: 12532
		private bool hasFiredSecondBullet;

		// Token: 0x040030F5 RID: 12533
		private float duration;
	}
}
