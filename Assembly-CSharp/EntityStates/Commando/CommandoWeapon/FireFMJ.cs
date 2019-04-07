using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020001A5 RID: 421
	internal class FireFMJ : BaseState
	{
		// Token: 0x06000830 RID: 2096 RVA: 0x00028FFC File Offset: 0x000271FC
		public override void OnEnter()
		{
			base.OnEnter();
			base.characterBody.SetSpreadBloom(0f, false);
			this.stopwatch = 0f;
			this.duration = FireFMJ.baseDuration / this.attackSpeedStat;
			Ray aimRay = base.GetAimRay();
			UnityEngine.Object modelAnimator = base.GetModelAnimator();
			base.GetModelTransform();
			Util.PlaySound(FireFMJ.attackSoundString, base.gameObject);
			string muzzleName = "";
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(2f);
			}
			if (modelAnimator)
			{
				base.PlayAnimation("Gesture, Additive", "FireFMJ", "FireFMJ.playbackRate", this.duration);
				base.PlayAnimation("Gesture, Override", "FireFMJ", "FireFMJ.playbackRate", this.duration);
				muzzleName = "MuzzleCenter";
			}
			if (FireFMJ.effectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(FireFMJ.effectPrefab, base.gameObject, muzzleName, false);
			}
			base.AddRecoil(-2f * FireFMJ.recoilAmplitude, -3f * FireFMJ.recoilAmplitude, -1f * FireFMJ.recoilAmplitude, 1f * FireFMJ.recoilAmplitude);
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(FireFMJ.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireFMJ.damageCoefficient, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x06000831 RID: 2097 RVA: 0x00010288 File Offset: 0x0000E488
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000832 RID: 2098 RVA: 0x0002917B File Offset: 0x0002737B
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06000833 RID: 2099 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000ACC RID: 2764
		public static GameObject effectPrefab;

		// Token: 0x04000ACD RID: 2765
		public static GameObject hitEffectPrefab;

		// Token: 0x04000ACE RID: 2766
		public static GameObject tracerEffectPrefab;

		// Token: 0x04000ACF RID: 2767
		public static GameObject projectilePrefab;

		// Token: 0x04000AD0 RID: 2768
		public static float damageCoefficient;

		// Token: 0x04000AD1 RID: 2769
		public static float force;

		// Token: 0x04000AD2 RID: 2770
		public static float minSpread;

		// Token: 0x04000AD3 RID: 2771
		public static float maxSpread;

		// Token: 0x04000AD4 RID: 2772
		public static int bulletCount;

		// Token: 0x04000AD5 RID: 2773
		public static float baseDuration = 2f;

		// Token: 0x04000AD6 RID: 2774
		public static float recoilAmplitude = 1f;

		// Token: 0x04000AD7 RID: 2775
		public static string attackSoundString;

		// Token: 0x04000AD8 RID: 2776
		private float stopwatch;

		// Token: 0x04000AD9 RID: 2777
		private float duration;

		// Token: 0x04000ADA RID: 2778
		public int bulletCountCurrent = 1;
	}
}
