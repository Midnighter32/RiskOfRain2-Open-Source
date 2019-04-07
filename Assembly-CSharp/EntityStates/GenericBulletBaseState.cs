using System;
using RoR2;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x020000B3 RID: 179
	public abstract class GenericBulletBaseState : BaseState
	{
		// Token: 0x06000387 RID: 903 RVA: 0x0000E3B8 File Offset: 0x0000C5B8
		protected BulletAttack GenerateBulletAttack(Ray aimRay)
		{
			float num = 0f;
			if (base.characterBody)
			{
				num = base.characterBody.spreadBloomAngle;
			}
			return new BulletAttack
			{
				aimVector = aimRay.direction,
				origin = aimRay.origin,
				owner = base.gameObject,
				weapon = null,
				bulletCount = (uint)this.bulletCount,
				damage = this.damageStat * this.damageCoefficient,
				damageColorIndex = DamageColorIndex.Default,
				damageType = DamageType.Generic,
				falloffModel = BulletAttack.FalloffModel.Buckshot,
				force = this.force,
				HitEffectNormal = false,
				procChainMask = default(ProcChainMask),
				procCoefficient = this.procCoefficient,
				maxDistance = this.maxDistance,
				radius = this.bulletRadius,
				isCrit = base.RollCrit(),
				muzzleName = this.muzzleName,
				minSpread = this.minSpread + num,
				maxSpread = this.maxSpread + num,
				hitEffectPrefab = this.hitEffectPrefab,
				smartCollision = this.useSmartCollision,
				sniper = false,
				spreadPitchScale = this.spreadPitchScale,
				spreadYawScale = this.spreadYawScale,
				tracerEffectPrefab = this.tracerEffectPrefab
			};
		}

		// Token: 0x06000388 RID: 904 RVA: 0x00004507 File Offset: 0x00002707
		protected virtual void PlayFireAnimation()
		{
		}

		// Token: 0x06000389 RID: 905 RVA: 0x0000E504 File Offset: 0x0000C704
		protected virtual void DoFireEffects()
		{
			Util.PlaySound(this.fireSoundString, base.gameObject);
			if (this.muzzleTransform)
			{
				EffectManager.instance.SimpleMuzzleFlash(this.muzzleFlashPrefab, base.gameObject, this.muzzleName, false);
			}
		}

		// Token: 0x0600038A RID: 906 RVA: 0x00004507 File Offset: 0x00002707
		protected virtual void ModifyBullet(BulletAttack bulletAttack)
		{
		}

		// Token: 0x0600038B RID: 907 RVA: 0x0000E544 File Offset: 0x0000C744
		protected virtual void FireBullet(Ray aimRay)
		{
			base.StartAimMode(aimRay, 3f, false);
			this.DoFireEffects();
			this.PlayFireAnimation();
			if (base.isAuthority)
			{
				BulletAttack bulletAttack = this.GenerateBulletAttack(aimRay);
				this.ModifyBullet(bulletAttack);
				bulletAttack.Fire();
			}
		}

		// Token: 0x0600038C RID: 908 RVA: 0x0000E587 File Offset: 0x0000C787
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = this.baseDuration / this.attackSpeedStat;
			this.muzzleTransform = base.FindModelChild(this.muzzleName);
			this.FireBullet(base.GetAimRay());
		}

		// Token: 0x0600038D RID: 909 RVA: 0x0000E5C0 File Offset: 0x0000C7C0
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x0600038E RID: 910 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000341 RID: 833
		[SerializeField]
		public float baseDuration = 0.1f;

		// Token: 0x04000342 RID: 834
		[SerializeField]
		public int bulletCount = 1;

		// Token: 0x04000343 RID: 835
		[SerializeField]
		public float maxDistance = 50f;

		// Token: 0x04000344 RID: 836
		[SerializeField]
		public float bulletRadius;

		// Token: 0x04000345 RID: 837
		[SerializeField]
		public bool useSmartCollision;

		// Token: 0x04000346 RID: 838
		[SerializeField]
		public float damageCoefficient = 0.1f;

		// Token: 0x04000347 RID: 839
		[SerializeField]
		public float procCoefficient = 1f;

		// Token: 0x04000348 RID: 840
		[SerializeField]
		public float force = 100f;

		// Token: 0x04000349 RID: 841
		[SerializeField]
		public float minSpread;

		// Token: 0x0400034A RID: 842
		[SerializeField]
		public float maxSpread;

		// Token: 0x0400034B RID: 843
		[SerializeField]
		public float spreadPitchScale = 0.5f;

		// Token: 0x0400034C RID: 844
		[SerializeField]
		public float spreadYawScale = 1f;

		// Token: 0x0400034D RID: 845
		[SerializeField]
		public float spreadBloomValue = 0.2f;

		// Token: 0x0400034E RID: 846
		[SerializeField]
		public string muzzleName;

		// Token: 0x0400034F RID: 847
		[SerializeField]
		public string fireSoundString;

		// Token: 0x04000350 RID: 848
		[SerializeField]
		public GameObject muzzleFlashPrefab;

		// Token: 0x04000351 RID: 849
		[SerializeField]
		public GameObject tracerEffectPrefab;

		// Token: 0x04000352 RID: 850
		[SerializeField]
		public GameObject hitEffectPrefab;

		// Token: 0x04000353 RID: 851
		protected float duration;

		// Token: 0x04000354 RID: 852
		protected Transform muzzleTransform;
	}
}
