using System;
using RoR2;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x020000A7 RID: 167
	public class BaseState : EntityState
	{
		// Token: 0x06000324 RID: 804 RVA: 0x0000D0D8 File Offset: 0x0000B2D8
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.characterBody)
			{
				this.attackSpeedStat = base.characterBody.attackSpeed;
				this.damageStat = base.characterBody.damage;
				this.critStat = base.characterBody.crit;
				this.moveSpeedStat = base.characterBody.moveSpeed;
			}
		}

		// Token: 0x06000325 RID: 805 RVA: 0x0000D13C File Offset: 0x0000B33C
		protected Ray GetAimRay()
		{
			if (base.inputBank)
			{
				return new Ray(base.inputBank.aimOrigin, base.inputBank.aimDirection);
			}
			return new Ray(base.transform.position, base.transform.forward);
		}

		// Token: 0x06000326 RID: 806 RVA: 0x0000D18D File Offset: 0x0000B38D
		protected void AddRecoil(float verticalMin, float verticalMax, float horizontalMin, float horizontalMax)
		{
			base.cameraTargetParams.AddRecoil(verticalMin, verticalMax, horizontalMin, horizontalMax);
		}

		// Token: 0x06000327 RID: 807 RVA: 0x0000D1A0 File Offset: 0x0000B3A0
		public OverlapAttack InitMeleeOverlap(float damageCoefficient, GameObject hitEffectPrefab, Transform modelTransform, string hitboxGroupName)
		{
			OverlapAttack overlapAttack = new OverlapAttack();
			overlapAttack.attacker = base.gameObject;
			overlapAttack.inflictor = base.gameObject;
			overlapAttack.teamIndex = TeamComponent.GetObjectTeam(overlapAttack.attacker);
			overlapAttack.damage = damageCoefficient * this.damageStat;
			overlapAttack.hitEffectPrefab = hitEffectPrefab;
			overlapAttack.isCrit = this.RollCrit();
			if (modelTransform)
			{
				overlapAttack.hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == hitboxGroupName);
			}
			return overlapAttack;
		}

		// Token: 0x06000328 RID: 808 RVA: 0x0000D234 File Offset: 0x0000B434
		public bool FireMeleeOverlap(OverlapAttack attack, Animator animator, string mecanimHitboxActiveParameter, float forceMagnitude)
		{
			bool result = false;
			if (animator && animator.GetFloat(mecanimHitboxActiveParameter) > 0.1f)
			{
				attack.forceVector = base.transform.forward * forceMagnitude;
				result = attack.Fire(null);
			}
			return result;
		}

		// Token: 0x06000329 RID: 809 RVA: 0x0000D27A File Offset: 0x0000B47A
		public void SmallHop(CharacterMotor characterMotor, float smallHopVelocity)
		{
			if (characterMotor)
			{
				characterMotor.velocity = new Vector3(characterMotor.velocity.x, Mathf.Max(characterMotor.velocity.y, smallHopVelocity), characterMotor.velocity.z);
			}
		}

		// Token: 0x0600032A RID: 810 RVA: 0x0000D2B8 File Offset: 0x0000B4B8
		protected BaseState.HitStopCachedState CreateHitStopCachedState(CharacterMotor characterMotor, Animator animator, string playbackRateAnimationParameter)
		{
			BaseState.HitStopCachedState hitStopCachedState = default(BaseState.HitStopCachedState);
			hitStopCachedState.characterVelocity = new Vector3(characterMotor.velocity.x, Mathf.Max(0f, characterMotor.velocity.y), characterMotor.velocity.z);
			hitStopCachedState.playbackName = playbackRateAnimationParameter;
			hitStopCachedState.playbackRate = animator.GetFloat(hitStopCachedState.playbackName);
			return hitStopCachedState;
		}

		// Token: 0x0600032B RID: 811 RVA: 0x0000D320 File Offset: 0x0000B520
		protected void ConsumeHitStopCachedState(BaseState.HitStopCachedState hitStopCachedState, CharacterMotor characterMotor, Animator animator)
		{
			characterMotor.velocity = hitStopCachedState.characterVelocity;
			animator.SetFloat(hitStopCachedState.playbackName, hitStopCachedState.playbackRate);
		}

		// Token: 0x0600032C RID: 812 RVA: 0x0000D340 File Offset: 0x0000B540
		protected void StartAimMode(float duration = 2f, bool snap = false)
		{
			this.StartAimMode(this.GetAimRay(), duration, snap);
		}

		// Token: 0x0600032D RID: 813 RVA: 0x0000D350 File Offset: 0x0000B550
		protected void StartAimMode(Ray aimRay, float duration = 2f, bool snap = false)
		{
			if (base.characterDirection && aimRay.direction != Vector3.zero && snap)
			{
				base.characterDirection.forward = aimRay.direction;
			}
			if (base.characterBody)
			{
				base.characterBody.SetAimTimer(duration);
			}
			if (base.modelLocator)
			{
				Transform modelTransform = base.modelLocator.modelTransform;
				if (modelTransform)
				{
					AimAnimator component = modelTransform.GetComponent<AimAnimator>();
					if (component && snap)
					{
						component.AimImmediate();
					}
				}
			}
		}

		// Token: 0x0600032E RID: 814 RVA: 0x0000D3E5 File Offset: 0x0000B5E5
		protected bool RollCrit()
		{
			return base.characterBody && base.characterBody.master && Util.CheckRoll(this.critStat, base.characterBody.master);
		}

		// Token: 0x0600032F RID: 815 RVA: 0x0000D420 File Offset: 0x0000B620
		protected Transform FindModelChild(string childName)
		{
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				ChildLocator component = modelTransform.GetComponent<ChildLocator>();
				if (component)
				{
					return component.FindChild(childName);
				}
			}
			return null;
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000330 RID: 816 RVA: 0x0000D454 File Offset: 0x0000B654
		protected bool isGrounded
		{
			get
			{
				return base.characterMotor && base.characterMotor.isGrounded;
			}
		}

		// Token: 0x04000315 RID: 789
		protected float attackSpeedStat = 1f;

		// Token: 0x04000316 RID: 790
		protected float damageStat;

		// Token: 0x04000317 RID: 791
		protected float critStat;

		// Token: 0x04000318 RID: 792
		protected float moveSpeedStat;

		// Token: 0x04000319 RID: 793
		private const float defaultAimDuration = 2f;

		// Token: 0x020000A8 RID: 168
		protected struct HitStopCachedState
		{
			// Token: 0x0400031A RID: 794
			public Vector3 characterVelocity;

			// Token: 0x0400031B RID: 795
			public string playbackName;

			// Token: 0x0400031C RID: 796
			public float playbackRate;
		}
	}
}
