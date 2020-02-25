using System;
using System.Collections.Generic;
using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Huntress.HuntressWeapon
{
	// Token: 0x02000839 RID: 2105
	public class ThrowGlaive : BaseState
	{
		// Token: 0x06002FA6 RID: 12198 RVA: 0x000CC1E4 File Offset: 0x000CA3E4
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

		// Token: 0x06002FA7 RID: 12199 RVA: 0x000CC35C File Offset: 0x000CA55C
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
			if (!this.hasSuccessfullyThrownGlaive && NetworkServer.active)
			{
				base.skillLocator.secondary.AddOneStock();
			}
		}

		// Token: 0x06002FA8 RID: 12200 RVA: 0x000CC3F0 File Offset: 0x000CA5F0
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

		// Token: 0x06002FA9 RID: 12201 RVA: 0x000CC4A8 File Offset: 0x000CA6A8
		private void FireOrbGlaive()
		{
			if (!NetworkServer.active || this.hasTriedToThrowGlaive)
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
				EffectManager.SimpleMuzzleFlash(ThrowGlaive.muzzleFlashPrefab, base.gameObject, "HandR", true);
				lightningOrb.origin = transform.position;
				lightningOrb.target = hurtBox;
				OrbManager.instance.AddOrb(lightningOrb);
			}
		}

		// Token: 0x06002FAA RID: 12202 RVA: 0x0000BDAE File Offset: 0x00009FAE
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		// Token: 0x06002FAB RID: 12203 RVA: 0x000CC5CB File Offset: 0x000CA7CB
		public override void OnSerialize(NetworkWriter writer)
		{
			writer.Write(HurtBoxReference.FromHurtBox(this.initialOrbTarget));
		}

		// Token: 0x06002FAC RID: 12204 RVA: 0x000CC5E0 File Offset: 0x000CA7E0
		public override void OnDeserialize(NetworkReader reader)
		{
			this.initialOrbTarget = reader.ReadHurtBoxReference().ResolveHurtBox();
		}

		// Token: 0x04002D64 RID: 11620
		public static float baseDuration = 3f;

		// Token: 0x04002D65 RID: 11621
		public static GameObject chargePrefab;

		// Token: 0x04002D66 RID: 11622
		public static GameObject muzzleFlashPrefab;

		// Token: 0x04002D67 RID: 11623
		public static float smallHopStrength;

		// Token: 0x04002D68 RID: 11624
		public static float antigravityStrength;

		// Token: 0x04002D69 RID: 11625
		public static float damageCoefficient = 1.2f;

		// Token: 0x04002D6A RID: 11626
		public static float damageCoefficientPerBounce = 1.1f;

		// Token: 0x04002D6B RID: 11627
		public static float glaiveProcCoefficient;

		// Token: 0x04002D6C RID: 11628
		public static int maxBounceCount;

		// Token: 0x04002D6D RID: 11629
		public static float glaiveTravelSpeed;

		// Token: 0x04002D6E RID: 11630
		public static float glaiveBounceRange;

		// Token: 0x04002D6F RID: 11631
		public static string attackSoundString;

		// Token: 0x04002D70 RID: 11632
		private float duration;

		// Token: 0x04002D71 RID: 11633
		private float stopwatch;

		// Token: 0x04002D72 RID: 11634
		private Animator animator;

		// Token: 0x04002D73 RID: 11635
		private GameObject chargeEffect;

		// Token: 0x04002D74 RID: 11636
		private Transform modelTransform;

		// Token: 0x04002D75 RID: 11637
		private HuntressTracker huntressTracker;

		// Token: 0x04002D76 RID: 11638
		private ChildLocator childLocator;

		// Token: 0x04002D77 RID: 11639
		private bool hasTriedToThrowGlaive;

		// Token: 0x04002D78 RID: 11640
		private bool hasSuccessfullyThrownGlaive;

		// Token: 0x04002D79 RID: 11641
		private HurtBox initialOrbTarget;
	}
}
