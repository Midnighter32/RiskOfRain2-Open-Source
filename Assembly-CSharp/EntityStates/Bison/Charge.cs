using System;
using RoR2;
using UnityEngine;

namespace EntityStates.Bison
{
	// Token: 0x020001C0 RID: 448
	public class Charge : BaseState
	{
		// Token: 0x060008C4 RID: 2244 RVA: 0x0002C288 File Offset: 0x0002A488
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

		// Token: 0x060008C5 RID: 2245 RVA: 0x0002C37D File Offset: 0x0002A57D
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

		// Token: 0x060008C6 RID: 2246 RVA: 0x0002C3AC File Offset: 0x0002A5AC
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

		// Token: 0x060008C7 RID: 2247 RVA: 0x0002C418 File Offset: 0x0002A618
		public override void FixedUpdate()
		{
			this.targetMoveVector = Vector3.ProjectOnPlane(Vector3.SmoothDamp(this.targetMoveVector, base.inputBank.aimDirection, ref this.targetMoveVectorVelocity, Charge.turnSmoothTime, Charge.turnSpeed), Vector3.up).normalized;
			base.characterDirection.moveVector = this.targetMoveVector;
			Vector3 forward = base.characterDirection.forward;
			float value = this.moveSpeedStat * Charge.chargeMovementSpeedCoefficient;
			base.characterMotor.moveDirection = forward * Charge.chargeMovementSpeedCoefficient;
			this.animator.SetFloat("forwardSpeed", value);
			if (base.isAuthority)
			{
				this.attack.Fire(null);
			}
			if (this.overlapResetStopwatch >= 1f / Charge.overlapResetFrequency)
			{
				this.overlapResetStopwatch -= 1f / Charge.overlapResetFrequency;
			}
			if (base.isAuthority && Physics.OverlapSphere(this.sphereCheckTransform.position, Charge.overlapSphereRadius, LayerIndex.entityPrecise.mask | LayerIndex.world.mask).Length != 0)
			{
				base.healthComponent.TakeDamageForce(forward * Charge.selfStunForce, true);
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

		// Token: 0x060008C8 RID: 2248 RVA: 0x0002C5B4 File Offset: 0x0002A7B4
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

		// Token: 0x060008C9 RID: 2249 RVA: 0x0000AE8B File Offset: 0x0000908B
		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Skill;
		}

		// Token: 0x04000BCF RID: 3023
		public static float chargeDuration;

		// Token: 0x04000BD0 RID: 3024
		public static float chargeMovementSpeedCoefficient;

		// Token: 0x04000BD1 RID: 3025
		public static float turnSpeed;

		// Token: 0x04000BD2 RID: 3026
		public static float turnSmoothTime;

		// Token: 0x04000BD3 RID: 3027
		public static float impactDamageCoefficient;

		// Token: 0x04000BD4 RID: 3028
		public static float impactForce;

		// Token: 0x04000BD5 RID: 3029
		public static float damageCoefficient;

		// Token: 0x04000BD6 RID: 3030
		public static float upwardForceMagnitude;

		// Token: 0x04000BD7 RID: 3031
		public static float awayForceMagnitude;

		// Token: 0x04000BD8 RID: 3032
		public static GameObject hitEffectPrefab;

		// Token: 0x04000BD9 RID: 3033
		public static float overlapResetFrequency;

		// Token: 0x04000BDA RID: 3034
		public static float overlapSphereRadius;

		// Token: 0x04000BDB RID: 3035
		public static float selfStunDuration;

		// Token: 0x04000BDC RID: 3036
		public static float selfStunForce;

		// Token: 0x04000BDD RID: 3037
		public static string startSoundString;

		// Token: 0x04000BDE RID: 3038
		public static string endSoundString;

		// Token: 0x04000BDF RID: 3039
		public static string footstepOverrideSoundString;

		// Token: 0x04000BE0 RID: 3040
		private float stopwatch;

		// Token: 0x04000BE1 RID: 3041
		private float overlapResetStopwatch;

		// Token: 0x04000BE2 RID: 3042
		private Animator animator;

		// Token: 0x04000BE3 RID: 3043
		private Vector3 targetMoveVector;

		// Token: 0x04000BE4 RID: 3044
		private Vector3 targetMoveVectorVelocity;

		// Token: 0x04000BE5 RID: 3045
		private ContactDamage contactDamage;

		// Token: 0x04000BE6 RID: 3046
		private OverlapAttack attack;

		// Token: 0x04000BE7 RID: 3047
		private HitBoxGroup hitboxGroup;

		// Token: 0x04000BE8 RID: 3048
		private ChildLocator childLocator;

		// Token: 0x04000BE9 RID: 3049
		private Transform sphereCheckTransform;

		// Token: 0x04000BEA RID: 3050
		private string baseFootstepString;
	}
}
