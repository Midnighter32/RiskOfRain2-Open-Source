using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.Mage.Weapon
{
	// Token: 0x02000112 RID: 274
	internal class ChargeNovabomb : BaseState
	{
		// Token: 0x0600053D RID: 1341 RVA: 0x00017460 File Offset: 0x00015660
		public override void OnEnter()
		{
			base.OnEnter();
			MageLastElementTracker component = base.GetComponent<MageLastElementTracker>();
			if (component)
			{
				component.ApplyElement(MageElement.Lightning);
			}
			this.stopwatch = 0f;
			this.windDownDuration = ChargeNovabomb.baseWinddownDuration / this.attackSpeedStat;
			this.chargeDuration = ChargeNovabomb.baseChargeDuration / this.attackSpeedStat;
			Util.PlayScaledSound(ChargeNovabomb.chargeSoundString, base.gameObject, this.attackSpeedStat);
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
				if (this.muzzleTransform && ChargeNovabomb.chargeEffectPrefab)
				{
					this.chargeEffectInstance = UnityEngine.Object.Instantiate<GameObject>(ChargeNovabomb.chargeEffectPrefab, this.muzzleTransform.position, this.muzzleTransform.rotation);
					this.chargeEffectInstance.transform.parent = this.muzzleTransform;
					ScaleParticleSystemDuration component2 = this.chargeEffectInstance.GetComponent<ScaleParticleSystemDuration>();
					ObjectScaleCurve component3 = this.chargeEffectInstance.GetComponent<ObjectScaleCurve>();
					if (component2)
					{
						component2.newDuration = this.chargeDuration;
					}
					if (component3)
					{
						component3.timeMax = this.chargeDuration;
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

		// Token: 0x0600053E RID: 1342 RVA: 0x00017629 File Offset: 0x00015829
		public override void Update()
		{
			base.Update();
			base.characterBody.SetSpreadBloom(Util.Remap(this.GetChargeProgress(), 0f, 1f, ChargeNovabomb.minRadius, ChargeNovabomb.maxRadius), true);
		}

		// Token: 0x0600053F RID: 1343 RVA: 0x0001765C File Offset: 0x0001585C
		public override void OnExit()
		{
			if (this.chargeEffectInstance)
			{
				EntityState.Destroy(this.chargeEffectInstance);
			}
			base.characterBody.crosshairPrefab = this.defaultCrosshairPrefab;
			base.OnExit();
		}

		// Token: 0x06000540 RID: 1344 RVA: 0x00017690 File Offset: 0x00015890
		private void FireNovaBomb()
		{
			this.hasFiredBomb = true;
			base.PlayAnimation("Gesture, Additive", "FireNovaBomb", "FireNovaBomb.playbackRate", this.windDownDuration);
			Ray aimRay = base.GetAimRay();
			if (this.chargeEffectInstance)
			{
				EntityState.Destroy(this.chargeEffectInstance);
			}
			if (ChargeNovabomb.muzzleflashEffectPrefab)
			{
				EffectManager.instance.SimpleMuzzleFlash(ChargeNovabomb.muzzleflashEffectPrefab, base.gameObject, "MuzzleLeft", false);
				EffectManager.instance.SimpleMuzzleFlash(ChargeNovabomb.muzzleflashEffectPrefab, base.gameObject, "MuzzleRight", false);
			}
			if (base.isAuthority)
			{
				float chargeProgress = this.GetChargeProgress();
				if (ChargeNovabomb.projectilePrefab != null)
				{
					float num = Util.Remap(chargeProgress, 0f, 1f, ChargeNovabomb.minDamageCoefficient, ChargeNovabomb.maxDamageCoefficient);
					float num2 = chargeProgress * ChargeNovabomb.force;
					Ray aimRay2 = base.GetAimRay();
					Vector3 direction = aimRay2.direction;
					Vector3 origin = aimRay2.origin;
					ProjectileManager.instance.FireProjectile(ChargeNovabomb.projectilePrefab, origin, Util.QuaternionSafeLookRotation(direction), base.gameObject, this.damageStat * num, num2, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
				}
				if (base.characterMotor)
				{
					base.characterMotor.ApplyForce(aimRay.direction * (-ChargeNovabomb.selfForce * chargeProgress), false);
				}
			}
			base.characterBody.crosshairPrefab = this.defaultCrosshairPrefab;
			this.stopwatch = 0f;
		}

		// Token: 0x06000541 RID: 1345 RVA: 0x0001780C File Offset: 0x00015A0C
		private float GetChargeProgress()
		{
			return Mathf.Clamp01(this.stopwatch / this.chargeDuration);
		}

		// Token: 0x06000542 RID: 1346 RVA: 0x00017820 File Offset: 0x00015A20
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

		// Token: 0x06000543 RID: 1347 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x04000583 RID: 1411
		public static GameObject projectilePrefab;

		// Token: 0x04000584 RID: 1412
		public static GameObject muzzleflashEffectPrefab;

		// Token: 0x04000585 RID: 1413
		public static GameObject chargeEffectPrefab;

		// Token: 0x04000586 RID: 1414
		public static string chargeSoundString;

		// Token: 0x04000587 RID: 1415
		public static float baseChargeDuration = 1.5f;

		// Token: 0x04000588 RID: 1416
		public static float baseWinddownDuration = 2f;

		// Token: 0x04000589 RID: 1417
		public static float minDamageCoefficient;

		// Token: 0x0400058A RID: 1418
		public static float maxDamageCoefficient;

		// Token: 0x0400058B RID: 1419
		public static float minRadius;

		// Token: 0x0400058C RID: 1420
		public static float maxRadius;

		// Token: 0x0400058D RID: 1421
		public static float force;

		// Token: 0x0400058E RID: 1422
		public static GameObject crosshairOverridePrefab;

		// Token: 0x0400058F RID: 1423
		public static float selfForce;

		// Token: 0x04000590 RID: 1424
		private const float minChargeDuration = 0.5f;

		// Token: 0x04000591 RID: 1425
		private float stopwatch;

		// Token: 0x04000592 RID: 1426
		private float windDownDuration;

		// Token: 0x04000593 RID: 1427
		private float chargeDuration;

		// Token: 0x04000594 RID: 1428
		private bool hasFiredBomb;

		// Token: 0x04000595 RID: 1429
		private string muzzleString;

		// Token: 0x04000596 RID: 1430
		private Transform muzzleTransform;

		// Token: 0x04000597 RID: 1431
		private Animator animator;

		// Token: 0x04000598 RID: 1432
		private ChildLocator childLocator;

		// Token: 0x04000599 RID: 1433
		private GameObject chargeEffectInstance;

		// Token: 0x0400059A RID: 1434
		private GameObject defaultCrosshairPrefab;
	}
}
