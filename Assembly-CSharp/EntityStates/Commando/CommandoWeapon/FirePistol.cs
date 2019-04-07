using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020001A9 RID: 425
	internal class FirePistol : BaseState
	{
		// Token: 0x0600084B RID: 2123 RVA: 0x000298E0 File Offset: 0x00027AE0
		private void FireBullet(string targetMuzzle)
		{
			if (FirePistol.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FirePistol.effectPrefab, base.gameObject, targetMuzzle, false);
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

		// Token: 0x0600084C RID: 2124 RVA: 0x00029A00 File Offset: 0x00027C00
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

		// Token: 0x0600084D RID: 2125 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x0600084E RID: 2126 RVA: 0x00029A78 File Offset: 0x00027C78
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

		// Token: 0x0600084F RID: 2127 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000AFE RID: 2814
		public static GameObject effectPrefab;

		// Token: 0x04000AFF RID: 2815
		public static GameObject hitEffectPrefab;

		// Token: 0x04000B00 RID: 2816
		public static GameObject tracerEffectPrefab;

		// Token: 0x04000B01 RID: 2817
		public static float damageCoefficient;

		// Token: 0x04000B02 RID: 2818
		public static float force;

		// Token: 0x04000B03 RID: 2819
		public static float minSpread;

		// Token: 0x04000B04 RID: 2820
		public static float maxSpread;

		// Token: 0x04000B05 RID: 2821
		public static float baseDuration = 2f;

		// Token: 0x04000B06 RID: 2822
		public static string firePistolSoundString;

		// Token: 0x04000B07 RID: 2823
		public static float recoilAmplitude = 1f;

		// Token: 0x04000B08 RID: 2824
		private Ray aimRay;

		// Token: 0x04000B09 RID: 2825
		private Animator modelAnimator;

		// Token: 0x04000B0A RID: 2826
		private bool hasFiredSecondBullet;

		// Token: 0x04000B0B RID: 2827
		private float duration;
	}
}
