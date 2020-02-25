using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Bison
{
	// Token: 0x020008DB RID: 2267
	public class Charge : BaseState
	{
		// Token: 0x060032C3 RID: 12995 RVA: 0x000DBEB8 File Offset: 0x000DA0B8
		public override void OnEnter()
		{
			base.OnEnter();
			this.animator = base.GetModelAnimator();
			this.childLocator = this.animator.GetComponent<ChildLocator>();
			FootstepHandler component = this.animator.GetComponent<FootstepHandler>();
			if (component)
			{
				this.baseFootstepString = component.baseFootstepString;
				component.baseFootstepString = Charge.footstepOverrideSoundString;
			}
			Util.PlaySound(Charge.startSoundString, base.gameObject);
			base.PlayCrossfade("Body", "ChargeForward", 0.2f);
			this.ResetOverlapAttack();
			this.SetSprintEffectActive(true);
			if (this.childLocator)
			{
				this.sphereCheckTransform = this.childLocator.FindChild("SphereCheckTransform");
			}
			if (!this.sphereCheckTransform && base.characterBody)
			{
				this.sphereCheckTransform = base.characterBody.coreTransform;
			}
			if (!this.sphereCheckTransform)
			{
				this.sphereCheckTransform = base.transform;
			}
		}

		// Token: 0x060032C4 RID: 12996 RVA: 0x000DBFAD File Offset: 0x000DA1AD
		private void SetSprintEffectActive(bool active)
		{
			if (this.childLocator)
			{
				Transform transform = this.childLocator.FindChild("SprintEffect");
				if (transform == null)
				{
					return;
				}
				transform.gameObject.SetActive(active);
			}
		}

		// Token: 0x060032C5 RID: 12997 RVA: 0x000DBFDC File Offset: 0x000DA1DC
		public override void OnExit()
		{
			base.OnExit();
			base.characterMotor.moveDirection = Vector3.zero;
			Util.PlaySound(Charge.endSoundString, base.gameObject);
			Util.PlaySound("stop_bison_charge_attack_loop", base.gameObject);
			this.SetSprintEffectActive(false);
			FootstepHandler component = this.animator.GetComponent<FootstepHandler>();
			if (component)
			{
				component.baseFootstepString = this.baseFootstepString;
			}
		}

		// Token: 0x060032C6 RID: 12998 RVA: 0x000DC048 File Offset: 0x000DA248
		public override void FixedUpdate()
		{
			this.targetMoveVector = Vector3.ProjectOnPlane(Vector3.SmoothDamp(this.targetMoveVector, base.inputBank.aimDirection, ref this.targetMoveVectorVelocity, Charge.turnSmoothTime, Charge.turnSpeed), Vector3.up).normalized;
			base.characterDirection.moveVector = this.targetMoveVector;
			Vector3 forward = base.characterDirection.forward;
			float value = this.moveSpeedStat * Charge.chargeMovementSpeedCoefficient;
			base.characterMotor.moveDirection = forward * Charge.chargeMovementSpeedCoefficient;
			this.animator.SetFloat("forwardSpeed", value);
			if (base.isAuthority && this.attack.Fire(null))
			{
				Util.PlaySound(Charge.headbuttImpactSound, base.gameObject);
			}
			if (this.overlapResetStopwatch >= 1f / Charge.overlapResetFrequency)
			{
				this.overlapResetStopwatch -= 1f / Charge.overlapResetFrequency;
			}
			if (base.isAuthority && Physics.OverlapSphere(this.sphereCheckTransform.position, Charge.overlapSphereRadius, LayerIndex.world.mask).Length != 0)
			{
				Util.PlaySound(Charge.headbuttImpactSound, base.gameObject);
				EffectManager.SimpleMuzzleFlash(Charge.hitEffectPrefab, base.gameObject, "SphereCheckTransform", true);
				base.healthComponent.TakeDamageForce(forward * Charge.selfStunForce, true, false);
				StunState stunState = new StunState();
				stunState.stunDuration = Charge.selfStunDuration;
				this.outer.SetNextState(stunState);
				return;
			}
			this.stopwatch += Time.fixedDeltaTime;
			this.overlapResetStopwatch += Time.fixedDeltaTime;
			if (this.stopwatch > Charge.chargeDuration)
			{
				this.outer.SetNextStateToMain();
			}
			base.FixedUpdate();
		}

		// Token: 0x060032C7 RID: 12999 RVA: 0x000DC210 File Offset: 0x000DA410
		private void ResetOverlapAttack()
		{
			if (!this.hitboxGroup)
			{
				Transform modelTransform = base.GetModelTransform();
				if (modelTransform)
				{
					this.hitboxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Charge");
				}
			}
			this.attack = new OverlapAttack();
			this.attack.attacker = base.gameObject;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = TeamComponent.GetObjectTeam(this.attack.attacker);
			this.attack.damage = Charge.damageCoefficient * this.damageStat;
			this.attack.hitEffectPrefab = Charge.hitEffectPrefab;
			this.attack.forceVector = Vector3.up * Charge.upwardForceMagnitude;
			this.attack.pushAwayForce = Charge.awayForceMagnitude;
			this.attack.hitBoxGroup = this.hitboxGroup;
		}

		// Token: 0x060032C8 RID: 13000 RVA: 0x0000B933 File Offset: 0x00009B33
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x040031F7 RID: 12791
		public static float chargeDuration;

		// Token: 0x040031F8 RID: 12792
		public static float chargeMovementSpeedCoefficient;

		// Token: 0x040031F9 RID: 12793
		public static float turnSpeed;

		// Token: 0x040031FA RID: 12794
		public static float turnSmoothTime;

		// Token: 0x040031FB RID: 12795
		public static float impactDamageCoefficient;

		// Token: 0x040031FC RID: 12796
		public static float impactForce;

		// Token: 0x040031FD RID: 12797
		public static float damageCoefficient;

		// Token: 0x040031FE RID: 12798
		public static float upwardForceMagnitude;

		// Token: 0x040031FF RID: 12799
		public static float awayForceMagnitude;

		// Token: 0x04003200 RID: 12800
		public static GameObject hitEffectPrefab;

		// Token: 0x04003201 RID: 12801
		public static float overlapResetFrequency;

		// Token: 0x04003202 RID: 12802
		public static float overlapSphereRadius;

		// Token: 0x04003203 RID: 12803
		public static float selfStunDuration;

		// Token: 0x04003204 RID: 12804
		public static float selfStunForce;

		// Token: 0x04003205 RID: 12805
		public static string startSoundString;

		// Token: 0x04003206 RID: 12806
		public static string endSoundString;

		// Token: 0x04003207 RID: 12807
		public static string footstepOverrideSoundString;

		// Token: 0x04003208 RID: 12808
		public static string headbuttImpactSound;

		// Token: 0x04003209 RID: 12809
		private float stopwatch;

		// Token: 0x0400320A RID: 12810
		private float overlapResetStopwatch;

		// Token: 0x0400320B RID: 12811
		private Animator animator;

		// Token: 0x0400320C RID: 12812
		private Vector3 targetMoveVector;

		// Token: 0x0400320D RID: 12813
		private Vector3 targetMoveVectorVelocity;

		// Token: 0x0400320E RID: 12814
		private ContactDamage contactDamage;

		// Token: 0x0400320F RID: 12815
		private OverlapAttack attack;

		// Token: 0x04003210 RID: 12816
		private HitBoxGroup hitboxGroup;

		// Token: 0x04003211 RID: 12817
		private ChildLocator childLocator;

		// Token: 0x04003212 RID: 12818
		private Transform sphereCheckTransform;

		// Token: 0x04003213 RID: 12819
		private string baseFootstepString;
	}
}
