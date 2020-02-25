using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x02000710 RID: 1808
	public class GenericProjectileBaseState : BaseState
	{
		// Token: 0x06002A32 RID: 10802 RVA: 0x000B182C File Offset: 0x000AFA2C
		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			this.duration = this.baseDuration / this.attackSpeedStat;
			this.delayBeforeFiringProjectile = this.baseDelayBeforeFiringProjectile / this.attackSpeedStat;
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(2f);
			}
			this.PlayAnimation(this.duration);
		}

		// Token: 0x06002A33 RID: 10803 RVA: 0x000B1899 File Offset: 0x000AFA99
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06002A34 RID: 10804 RVA: 0x0000409B File Offset: 0x0000229B
		protected virtual void PlayAnimation(float duration)
		{
		}

		// Token: 0x06002A35 RID: 10805 RVA: 0x000B18A4 File Offset: 0x000AFAA4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.delayBeforeFiringProjectile && !this.firedProjectile)
			{
				this.firedProjectile = true;
				this.FireProjectile();
				this.DoFireEffects();
			}
			if (this.stopwatch >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002A36 RID: 10806 RVA: 0x000B1914 File Offset: 0x000AFB14
		protected virtual void FireProjectile()
		{
			if (base.isAuthority)
			{
				Ray aimRay = base.GetAimRay();
				aimRay = this.ModifyProjectileAimRay(aimRay);
				aimRay.direction = Util.ApplySpread(aimRay.direction, this.minSpread, this.maxSpread, 1f, 1f, 0f, this.projectilePitchBonus);
				ProjectileManager.instance.FireProjectile(this.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * this.damageCoefficient, this.force, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
			}
		}

		// Token: 0x06002A37 RID: 10807 RVA: 0x0000AC9A File Offset: 0x00008E9A
		protected virtual Ray ModifyProjectileAimRay(Ray aimRay)
		{
			return aimRay;
		}

		// Token: 0x06002A38 RID: 10808 RVA: 0x000B19C8 File Offset: 0x000AFBC8
		protected virtual void DoFireEffects()
		{
			Util.PlaySound(this.attackSoundString, base.gameObject);
			base.AddRecoil(-2f * this.recoilAmplitude, -3f * this.recoilAmplitude, -1f * this.recoilAmplitude, 1f * this.recoilAmplitude);
			if (this.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(this.effectPrefab, base.gameObject, this.targetMuzzle, false);
			}
			base.characterBody.AddSpreadBloom(this.bloom);
		}

		// Token: 0x06002A39 RID: 10809 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x040025FF RID: 9727
		[SerializeField]
		public GameObject effectPrefab;

		// Token: 0x04002600 RID: 9728
		[SerializeField]
		public GameObject projectilePrefab;

		// Token: 0x04002601 RID: 9729
		[SerializeField]
		public float damageCoefficient;

		// Token: 0x04002602 RID: 9730
		[SerializeField]
		public float force;

		// Token: 0x04002603 RID: 9731
		[SerializeField]
		public float minSpread;

		// Token: 0x04002604 RID: 9732
		[SerializeField]
		public float maxSpread;

		// Token: 0x04002605 RID: 9733
		[SerializeField]
		public float baseDuration = 2f;

		// Token: 0x04002606 RID: 9734
		[SerializeField]
		public float recoilAmplitude = 1f;

		// Token: 0x04002607 RID: 9735
		[SerializeField]
		public string attackSoundString;

		// Token: 0x04002608 RID: 9736
		[SerializeField]
		public float projectilePitchBonus;

		// Token: 0x04002609 RID: 9737
		[SerializeField]
		public float baseDelayBeforeFiringProjectile;

		// Token: 0x0400260A RID: 9738
		[SerializeField]
		public string targetMuzzle;

		// Token: 0x0400260B RID: 9739
		[SerializeField]
		public float bloom;

		// Token: 0x0400260C RID: 9740
		private float stopwatch;

		// Token: 0x0400260D RID: 9741
		private float duration;

		// Token: 0x0400260E RID: 9742
		private float delayBeforeFiringProjectile;

		// Token: 0x0400260F RID: 9743
		private bool firedProjectile;
	}
}
