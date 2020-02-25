using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Commando.CommandoWeapon
{
	// Token: 0x020008B6 RID: 2230
	public class FireFMJ : BaseState
	{
		// Token: 0x06003202 RID: 12802 RVA: 0x000D7F64 File Offset: 0x000D6164
		public override void OnEnter()
		{
			base.OnEnter();
			base.characterBody.SetSpreadBloom(0f, false);
			this.stopwatch = 0f;
			this.duration = this.baseDuration / this.attackSpeedStat;
			this.delayBeforeFiringProjectile = this.baseDelayBeforeFiringProjectile / this.attackSpeedStat;
			this.PlayAnimation(this.duration);
			Util.PlaySound(this.attackSoundString, base.gameObject);
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(2f);
			}
		}

		// Token: 0x06003203 RID: 12803 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06003204 RID: 12804 RVA: 0x000D7FF4 File Offset: 0x000D61F4
		protected virtual void PlayAnimation(float duration)
		{
			if (base.GetModelAnimator())
			{
				base.PlayAnimation("Gesture, Additive", "FireFMJ", "FireFMJ.playbackRate", duration);
				base.PlayAnimation("Gesture, Override", "FireFMJ", "FireFMJ.playbackRate", duration);
			}
		}

		// Token: 0x06003205 RID: 12805 RVA: 0x000D8030 File Offset: 0x000D6230
		protected virtual void Fire()
		{
			string muzzleName = "MuzzleCenter";
			base.AddRecoil(-2f * this.recoilAmplitude, -3f * this.recoilAmplitude, -1f * this.recoilAmplitude, 1f * this.recoilAmplitude);
			if (this.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(this.effectPrefab, base.gameObject, muzzleName, false);
			}
			this.firedProjectile = true;
			if (base.isAuthority)
			{
				Ray aimRay = base.GetAimRay();
				aimRay.direction = Util.ApplySpread(aimRay.direction, this.minSpread, this.maxSpread, 1f, 1f, 0f, this.projectilePitchBonus);
				ProjectileManager.instance.FireProjectile(this.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * this.damageCoefficient, this.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x06003206 RID: 12806 RVA: 0x000D8140 File Offset: 0x000D6340
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.delayBeforeFiringProjectile && !this.firedProjectile)
			{
				this.Fire();
			}
			if (this.stopwatch >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06003207 RID: 12807 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x040030B5 RID: 12469
		[SerializeField]
		public GameObject effectPrefab;

		// Token: 0x040030B6 RID: 12470
		[SerializeField]
		public GameObject projectilePrefab;

		// Token: 0x040030B7 RID: 12471
		[SerializeField]
		public float damageCoefficient;

		// Token: 0x040030B8 RID: 12472
		[SerializeField]
		public float force;

		// Token: 0x040030B9 RID: 12473
		[SerializeField]
		public float minSpread;

		// Token: 0x040030BA RID: 12474
		[SerializeField]
		public float maxSpread;

		// Token: 0x040030BB RID: 12475
		[SerializeField]
		public float baseDuration = 2f;

		// Token: 0x040030BC RID: 12476
		[SerializeField]
		public float recoilAmplitude = 1f;

		// Token: 0x040030BD RID: 12477
		[SerializeField]
		public string attackSoundString;

		// Token: 0x040030BE RID: 12478
		[SerializeField]
		public float projectilePitchBonus;

		// Token: 0x040030BF RID: 12479
		[SerializeField]
		public float baseDelayBeforeFiringProjectile;

		// Token: 0x040030C0 RID: 12480
		private float stopwatch;

		// Token: 0x040030C1 RID: 12481
		private float duration;

		// Token: 0x040030C2 RID: 12482
		private float delayBeforeFiringProjectile;

		// Token: 0x040030C3 RID: 12483
		private bool firedProjectile;
	}
}
