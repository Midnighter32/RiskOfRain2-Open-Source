using System;
using RoR2;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x0200070A RID: 1802
	public abstract class GenericBulletBaseState : BaseState
	{
		// Token: 0x06002A02 RID: 10754 RVA: 0x000B089C File Offset: 0x000AEA9C
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

		// Token: 0x06002A03 RID: 10755 RVA: 0x0000409B File Offset: 0x0000229B
		protected virtual void PlayFireAnimation()
		{
		}

		// Token: 0x06002A04 RID: 10756 RVA: 0x000B09E8 File Offset: 0x000AEBE8
		protected virtual void DoFireEffects()
		{
			Util.PlaySound(this.fireSoundString, base.gameObject);
			if (this.muzzleTransform)
			{
				EffectManager.SimpleMuzzleFlash(this.muzzleFlashPrefab, base.gameObject, this.muzzleName, false);
			}
		}

		// Token: 0x06002A05 RID: 10757 RVA: 0x0000409B File Offset: 0x0000229B
		protected virtual void ModifyBullet(BulletAttack bulletAttack)
		{
		}

		// Token: 0x06002A06 RID: 10758 RVA: 0x000B0A24 File Offset: 0x000AEC24
		protected virtual void FireBullet(Ray aimRay)
		{
			base.StartAimMode(aimRay, 3f, false);
			this.DoFireEffects();
			this.PlayFireAnimation();
			base.AddRecoil(-1f * this.recoilAmplitudeY, -1.5f * this.recoilAmplitudeY, -1f * this.recoilAmplitudeX, 1f * this.recoilAmplitudeX);
			if (base.isAuthority)
			{
				BulletAttack bulletAttack = this.GenerateBulletAttack(aimRay);
				this.ModifyBullet(bulletAttack);
				bulletAttack.Fire();
			}
		}

		// Token: 0x06002A07 RID: 10759 RVA: 0x000B0A9D File Offset: 0x000AEC9D
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = this.baseDuration / this.attackSpeedStat;
			this.muzzleTransform = base.FindModelChild(this.muzzleName);
			this.FireBullet(base.GetAimRay());
		}

		// Token: 0x06002A08 RID: 10760 RVA: 0x000B0AD6 File Offset: 0x000AECD6
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06002A09 RID: 10761 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040025D3 RID: 9683
		[SerializeField]
		public float baseDuration = 0.1f;

		// Token: 0x040025D4 RID: 9684
		[SerializeField]
		public int bulletCount = 1;

		// Token: 0x040025D5 RID: 9685
		[SerializeField]
		public float maxDistance = 50f;

		// Token: 0x040025D6 RID: 9686
		[SerializeField]
		public float bulletRadius;

		// Token: 0x040025D7 RID: 9687
		[SerializeField]
		public bool useSmartCollision;

		// Token: 0x040025D8 RID: 9688
		[SerializeField]
		public float damageCoefficient = 0.1f;

		// Token: 0x040025D9 RID: 9689
		[SerializeField]
		public float procCoefficient = 1f;

		// Token: 0x040025DA RID: 9690
		[SerializeField]
		public float force = 100f;

		// Token: 0x040025DB RID: 9691
		[SerializeField]
		public float minSpread;

		// Token: 0x040025DC RID: 9692
		[SerializeField]
		public float maxSpread;

		// Token: 0x040025DD RID: 9693
		[SerializeField]
		public float spreadPitchScale = 0.5f;

		// Token: 0x040025DE RID: 9694
		[SerializeField]
		public float spreadYawScale = 1f;

		// Token: 0x040025DF RID: 9695
		[SerializeField]
		public float spreadBloomValue = 0.2f;

		// Token: 0x040025E0 RID: 9696
		[SerializeField]
		public float recoilAmplitudeY;

		// Token: 0x040025E1 RID: 9697
		[SerializeField]
		public float recoilAmplitudeX;

		// Token: 0x040025E2 RID: 9698
		[SerializeField]
		public string muzzleName;

		// Token: 0x040025E3 RID: 9699
		[SerializeField]
		public string fireSoundString;

		// Token: 0x040025E4 RID: 9700
		[SerializeField]
		public GameObject muzzleFlashPrefab;

		// Token: 0x040025E5 RID: 9701
		[SerializeField]
		public GameObject tracerEffectPrefab;

		// Token: 0x040025E6 RID: 9702
		[SerializeField]
		public GameObject hitEffectPrefab;

		// Token: 0x040025E7 RID: 9703
		protected float duration;

		// Token: 0x040025E8 RID: 9704
		protected Transform muzzleTransform;
	}
}
