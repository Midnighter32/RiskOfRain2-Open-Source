using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Mage.Weapon
{
	// Token: 0x020007D0 RID: 2000
	public class ChargeNovabomb : BaseState
	{
		// Token: 0x06002D9C RID: 11676 RVA: 0x000C179C File Offset: 0x000BF99C
		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			this.windDownDuration = this.baseWinddownDuration / this.attackSpeedStat;
			this.chargeDuration = this.baseChargeDuration / this.attackSpeedStat;
			this.soundID = Util.PlayScaledSound(this.chargeSoundString, base.gameObject, this.attackSpeedStat);
			base.characterBody.SetAimTimer(this.chargeDuration + this.windDownDuration + 2f);
			this.muzzleString = "MuzzleBetween";
			this.animator = base.GetModelAnimator();
			if (this.animator)
			{
				this.childLocator = this.animator.GetComponent<ChildLocator>();
			}
			if (this.childLocator)
			{
				this.muzzleTransform = this.childLocator.FindChild(this.muzzleString);
				if (this.muzzleTransform && this.chargeEffectPrefab)
				{
					this.chargeEffectInstance = UnityEngine.Object.Instantiate<GameObject>(this.chargeEffectPrefab, this.muzzleTransform.position, this.muzzleTransform.rotation);
					this.chargeEffectInstance.transform.parent = this.muzzleTransform;
					ScaleParticleSystemDuration component = this.chargeEffectInstance.GetComponent<ScaleParticleSystemDuration>();
					ObjectScaleCurve component2 = this.chargeEffectInstance.GetComponent<ObjectScaleCurve>();
					if (component)
					{
						component.newDuration = this.chargeDuration;
					}
					if (component2)
					{
						component2.timeMax = this.chargeDuration;
					}
				}
			}
			base.PlayAnimation("Gesture, Additive", "ChargeNovaBomb", "ChargeNovaBomb.playbackRate", this.chargeDuration);
			this.defaultCrosshairPrefab = base.characterBody.crosshairPrefab;
			if (ChargeNovabomb.crosshairOverridePrefab)
			{
				base.characterBody.crosshairPrefab = ChargeNovabomb.crosshairOverridePrefab;
			}
		}

		// Token: 0x06002D9D RID: 11677 RVA: 0x000C1959 File Offset: 0x000BFB59
		public override void Update()
		{
			base.Update();
			base.characterBody.SetSpreadBloom(Util.Remap(this.GetChargeProgress(), 0f, 1f, this.minRadius, this.maxRadius), true);
		}

		// Token: 0x06002D9E RID: 11678 RVA: 0x000C1990 File Offset: 0x000BFB90
		public override void OnExit()
		{
			if (!this.outer.destroying && !this.hasFiredBomb)
			{
				base.PlayAnimation("Gesture, Additive", "Empty");
			}
			if (this.chargeEffectInstance)
			{
				EntityState.Destroy(this.chargeEffectInstance);
			}
			AkSoundEngine.StopPlayingID(this.soundID);
			base.characterBody.crosshairPrefab = this.defaultCrosshairPrefab;
			base.OnExit();
		}

		// Token: 0x06002D9F RID: 11679 RVA: 0x000C19FC File Offset: 0x000BFBFC
		private void FireNovaBomb()
		{
			this.hasFiredBomb = true;
			base.PlayAnimation("Gesture, Additive", "FireNovaBomb", "FireNovaBomb.playbackRate", this.windDownDuration);
			Ray aimRay = base.GetAimRay();
			if (this.chargeEffectInstance)
			{
				EntityState.Destroy(this.chargeEffectInstance);
			}
			if (this.muzzleflashEffectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(this.muzzleflashEffectPrefab, base.gameObject, "MuzzleLeft", false);
				EffectManager.SimpleMuzzleFlash(this.muzzleflashEffectPrefab, base.gameObject, "MuzzleRight", false);
			}
			if (base.isAuthority)
			{
				float chargeProgress = this.GetChargeProgress();
				if (this.projectilePrefab != null)
				{
					float num = Util.Remap(chargeProgress, 0f, 1f, this.minDamageCoefficient, this.maxDamageCoefficient);
					float num2 = chargeProgress * this.force;
					Ray aimRay2 = base.GetAimRay();
					Vector3 direction = aimRay2.direction;
					Vector3 origin = aimRay2.origin;
					ProjectileManager.instance.FireProjectile(this.projectilePrefab, origin, Util.QuaternionSafeLookRotation(direction), base.gameObject, this.damageStat * num, num2, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
				}
				if (base.characterMotor)
				{
					base.characterMotor.ApplyForce(aimRay.direction * (-ChargeNovabomb.selfForce * chargeProgress), false, false);
				}
			}
			base.characterBody.crosshairPrefab = this.defaultCrosshairPrefab;
			this.stopwatch = 0f;
		}

		// Token: 0x06002DA0 RID: 11680 RVA: 0x000C1B77 File Offset: 0x000BFD77
		private float GetChargeProgress()
		{
			return Mathf.Clamp01(this.stopwatch / this.chargeDuration);
		}

		// Token: 0x06002DA1 RID: 11681 RVA: 0x000C1B8C File Offset: 0x000BFD8C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (!this.hasFiredBomb && (this.stopwatch >= this.chargeDuration || !base.inputBank.skill2.down) && !this.hasFiredBomb && this.stopwatch >= 0.5f)
			{
				this.FireNovaBomb();
			}
			if (this.stopwatch >= this.windDownDuration && this.hasFiredBomb && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x06002DA2 RID: 11682 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04002A3F RID: 10815
		[SerializeField]
		public GameObject projectilePrefab;

		// Token: 0x04002A40 RID: 10816
		[SerializeField]
		public GameObject muzzleflashEffectPrefab;

		// Token: 0x04002A41 RID: 10817
		[SerializeField]
		public GameObject chargeEffectPrefab;

		// Token: 0x04002A42 RID: 10818
		[SerializeField]
		public string chargeSoundString;

		// Token: 0x04002A43 RID: 10819
		[SerializeField]
		public float baseChargeDuration = 1.5f;

		// Token: 0x04002A44 RID: 10820
		[SerializeField]
		public float baseWinddownDuration = 2f;

		// Token: 0x04002A45 RID: 10821
		[SerializeField]
		public float minDamageCoefficient;

		// Token: 0x04002A46 RID: 10822
		[SerializeField]
		public float maxDamageCoefficient;

		// Token: 0x04002A47 RID: 10823
		[SerializeField]
		public float minRadius;

		// Token: 0x04002A48 RID: 10824
		[SerializeField]
		public float maxRadius;

		// Token: 0x04002A49 RID: 10825
		[SerializeField]
		public float force;

		// Token: 0x04002A4A RID: 10826
		public static GameObject crosshairOverridePrefab;

		// Token: 0x04002A4B RID: 10827
		public static float selfForce;

		// Token: 0x04002A4C RID: 10828
		private const float minChargeDuration = 0.5f;

		// Token: 0x04002A4D RID: 10829
		private float stopwatch;

		// Token: 0x04002A4E RID: 10830
		private float windDownDuration;

		// Token: 0x04002A4F RID: 10831
		private float chargeDuration;

		// Token: 0x04002A50 RID: 10832
		private bool hasFiredBomb;

		// Token: 0x04002A51 RID: 10833
		private string muzzleString;

		// Token: 0x04002A52 RID: 10834
		private Transform muzzleTransform;

		// Token: 0x04002A53 RID: 10835
		private Animator animator;

		// Token: 0x04002A54 RID: 10836
		private ChildLocator childLocator;

		// Token: 0x04002A55 RID: 10837
		private GameObject chargeEffectInstance;

		// Token: 0x04002A56 RID: 10838
		private GameObject defaultCrosshairPrefab;

		// Token: 0x04002A57 RID: 10839
		private uint soundID;
	}
}
