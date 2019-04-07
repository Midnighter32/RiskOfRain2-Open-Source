using System;
using System.Collections.Generic;
using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Huntress.HuntressWeapon
{
	// Token: 0x02000157 RID: 343
	internal class ThrowGlaive : BaseState
	{
		// Token: 0x060006A5 RID: 1701 RVA: 0x0001FB48 File Offset: 0x0001DD48
		public override void OnEnter()
		{
			base.OnEnter();
			this.stopwatch = 0f;
			this.duration = ThrowGlaive.baseDuration / this.attackSpeedStat;
			this.modelTransform = base.GetModelTransform();
			this.animator = base.GetModelAnimator();
			this.huntressTracker = base.GetComponent<HuntressTracker>();
			Util.PlayScaledSound(ThrowGlaive.attackSoundString, base.gameObject, this.attackSpeedStat);
			if (this.huntressTracker && base.isAuthority)
			{
				this.initialOrbTarget = this.huntressTracker.GetTrackingTarget();
			}
			if (base.characterMotor && ThrowGlaive.smallHopStrength != 0f)
			{
				base.characterMotor.velocity.y = ThrowGlaive.smallHopStrength;
			}
			base.PlayAnimation("FullBody, Override", "ThrowGlaive", "ThrowGlaive.playbackRate", this.duration);
			if (this.modelTransform)
			{
				this.childLocator = this.modelTransform.GetComponent<ChildLocator>();
				if (this.childLocator)
				{
					Transform transform = this.childLocator.FindChild("HandR");
					if (transform && ThrowGlaive.chargePrefab)
					{
						this.chargeEffect = UnityEngine.Object.Instantiate<GameObject>(ThrowGlaive.chargePrefab, transform.position, transform.rotation);
						this.chargeEffect.transform.parent = transform;
					}
				}
			}
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(this.duration);
			}
		}

		// Token: 0x060006A6 RID: 1702 RVA: 0x0001FCC0 File Offset: 0x0001DEC0
		public override void OnExit()
		{
			base.OnExit();
			if (this.chargeEffect)
			{
				EntityState.Destroy(this.chargeEffect);
			}
			int layerIndex = this.animator.GetLayerIndex("Impact");
			if (layerIndex >= 0)
			{
				this.animator.SetLayerWeight(layerIndex, 1.5f);
				this.animator.PlayInFixedTime("LightImpact", layerIndex, 0f);
			}
			if (!this.hasTriedToThrowGlaive)
			{
				this.FireOrbGlaive();
			}
			if (!this.hasSuccessfullyThrownGlaive && base.isServer)
			{
				base.skillLocator.secondary.AddOneStock();
			}
		}

		// Token: 0x060006A7 RID: 1703 RVA: 0x0001FD58 File Offset: 0x0001DF58
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (!this.hasTriedToThrowGlaive && this.animator.GetFloat("ThrowGlaive.fire") > 0f)
			{
				if (this.chargeEffect)
				{
					EntityState.Destroy(this.chargeEffect);
				}
				this.FireOrbGlaive();
			}
			CharacterMotor characterMotor = base.characterMotor;
			characterMotor.velocity.y = characterMotor.velocity.y + ThrowGlaive.antigravityStrength * Time.fixedDeltaTime * (1f - this.stopwatch / this.duration);
			if (this.stopwatch >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		// Token: 0x060006A8 RID: 1704 RVA: 0x0001FE10 File Offset: 0x0001E010
		private void FireOrbGlaive()
		{
			if (!base.isServer || this.hasTriedToThrowGlaive)
			{
				return;
			}
			this.hasTriedToThrowGlaive = true;
			LightningOrb lightningOrb = new LightningOrb();
			lightningOrb.lightningType = LightningOrb.LightningType.HuntressGlaive;
			lightningOrb.damageValue = base.characterBody.damage * ThrowGlaive.damageCoefficient;
			lightningOrb.isCrit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
			lightningOrb.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);
			lightningOrb.attacker = base.gameObject;
			lightningOrb.procCoefficient = ThrowGlaive.glaiveProcCoefficient;
			lightningOrb.bouncesRemaining = ThrowGlaive.maxBounceCount;
			lightningOrb.speed = ThrowGlaive.glaiveTravelSpeed;
			lightningOrb.bouncedObjects = new List<HealthComponent>();
			lightningOrb.range = ThrowGlaive.glaiveBounceRange;
			lightningOrb.damageCoefficientPerBounce = ThrowGlaive.damageCoefficientPerBounce;
			HurtBox hurtBox = this.initialOrbTarget;
			if (hurtBox)
			{
				this.hasSuccessfullyThrownGlaive = true;
				Transform transform = this.childLocator.FindChild("HandR");
				EffectManager.instance.SimpleMuzzleFlash(ThrowGlaive.muzzleFlashPrefab, base.gameObject, "HandR", true);
				lightningOrb.origin = transform.position;
				lightningOrb.target = hurtBox;
				OrbManager.instance.AddOrb(lightningOrb);
			}
		}

		// Token: 0x060006A9 RID: 1705 RVA: 0x0000B306 File Offset: 0x00009506
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x060006AA RID: 1706 RVA: 0x0001FF39 File Offset: 0x0001E139
		public override void OnSerialize(NetworkWriter writer)
		{
			writer.Write(HurtBoxReference.FromHurtBox(this.initialOrbTarget));
		}

		// Token: 0x060006AB RID: 1707 RVA: 0x0001FF4C File Offset: 0x0001E14C
		public override void OnDeserialize(NetworkReader reader)
		{
			this.initialOrbTarget = reader.ReadHurtBoxReference().ResolveHurtBox();
		}

		// Token: 0x0400081E RID: 2078
		public static float baseDuration = 3f;

		// Token: 0x0400081F RID: 2079
		public static GameObject chargePrefab;

		// Token: 0x04000820 RID: 2080
		public static GameObject muzzleFlashPrefab;

		// Token: 0x04000821 RID: 2081
		public static float smallHopStrength;

		// Token: 0x04000822 RID: 2082
		public static float antigravityStrength;

		// Token: 0x04000823 RID: 2083
		public static float damageCoefficient = 1.2f;

		// Token: 0x04000824 RID: 2084
		public static float damageCoefficientPerBounce = 1.1f;

		// Token: 0x04000825 RID: 2085
		public static float glaiveProcCoefficient;

		// Token: 0x04000826 RID: 2086
		public static int maxBounceCount;

		// Token: 0x04000827 RID: 2087
		public static float glaiveTravelSpeed;

		// Token: 0x04000828 RID: 2088
		public static float glaiveBounceRange;

		// Token: 0x04000829 RID: 2089
		public static string attackSoundString;

		// Token: 0x0400082A RID: 2090
		private float duration;

		// Token: 0x0400082B RID: 2091
		private float stopwatch;

		// Token: 0x0400082C RID: 2092
		private Animator animator;

		// Token: 0x0400082D RID: 2093
		private GameObject chargeEffect;

		// Token: 0x0400082E RID: 2094
		private Transform modelTransform;

		// Token: 0x0400082F RID: 2095
		private HuntressTracker huntressTracker;

		// Token: 0x04000830 RID: 2096
		private ChildLocator childLocator;

		// Token: 0x04000831 RID: 2097
		private bool hasTriedToThrowGlaive;

		// Token: 0x04000832 RID: 2098
		private bool hasSuccessfullyThrownGlaive;

		// Token: 0x04000833 RID: 2099
		private HurtBox initialOrbTarget;
	}
}
