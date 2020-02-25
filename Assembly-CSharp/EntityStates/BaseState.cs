using System;
using System.Collections.Generic;
using RoR2;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x020006FB RID: 1787
	public class BaseState : EntityState
	{
		// Token: 0x06002980 RID: 10624 RVA: 0x000AEDE0 File Offset: 0x000ACFE0
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

		// Token: 0x06002981 RID: 10625 RVA: 0x000AEE44 File Offset: 0x000AD044
		protected Ray GetAimRay()
		{
			if (base.inputBank)
			{
				return new Ray(base.inputBank.aimOrigin, base.inputBank.aimDirection);
			}
			return new Ray(base.transform.position, base.transform.forward);
		}

		// Token: 0x06002982 RID: 10626 RVA: 0x000AEE95 File Offset: 0x000AD095
		protected void AddRecoil(float verticalMin, float verticalMax, float horizontalMin, float horizontalMax)
		{
			base.cameraTargetParams.AddRecoil(verticalMin, verticalMax, horizontalMin, horizontalMax);
		}

		// Token: 0x06002983 RID: 10627 RVA: 0x000AEEA8 File Offset: 0x000AD0A8
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

		// Token: 0x06002984 RID: 10628 RVA: 0x000AEF3C File Offset: 0x000AD13C
		public bool FireMeleeOverlap(OverlapAttack attack, Animator animator, string mecanimHitboxActiveParameter, float forceMagnitude, bool calculateForceVector = true)
		{
			bool result = false;
			if (animator && animator.GetFloat(mecanimHitboxActiveParameter) > 0.1f)
			{
				if (calculateForceVector)
				{
					attack.forceVector = base.transform.forward * forceMagnitude;
				}
				result = attack.Fire(null);
			}
			return result;
		}

		// Token: 0x06002985 RID: 10629 RVA: 0x000AEF88 File Offset: 0x000AD188
		public void SmallHop(CharacterMotor characterMotor, float smallHopVelocity)
		{
			if (characterMotor)
			{
				characterMotor.Motor.ForceUnground();
				characterMotor.velocity = new Vector3(characterMotor.velocity.x, Mathf.Max(characterMotor.velocity.y, smallHopVelocity), characterMotor.velocity.z);
			}
		}

		// Token: 0x06002986 RID: 10630 RVA: 0x000AEFDC File Offset: 0x000AD1DC
		protected BaseState.HitStopCachedState CreateHitStopCachedState(CharacterMotor characterMotor, Animator animator, string playbackRateAnimationParameter)
		{
			BaseState.HitStopCachedState hitStopCachedState = default(BaseState.HitStopCachedState);
			hitStopCachedState.characterVelocity = new Vector3(characterMotor.velocity.x, Mathf.Max(0f, characterMotor.velocity.y), characterMotor.velocity.z);
			hitStopCachedState.playbackName = playbackRateAnimationParameter;
			hitStopCachedState.playbackRate = animator.GetFloat(hitStopCachedState.playbackName);
			return hitStopCachedState;
		}

		// Token: 0x06002987 RID: 10631 RVA: 0x000AF044 File Offset: 0x000AD244
		protected void ConsumeHitStopCachedState(BaseState.HitStopCachedState hitStopCachedState, CharacterMotor characterMotor, Animator animator)
		{
			characterMotor.velocity = hitStopCachedState.characterVelocity;
			animator.SetFloat(hitStopCachedState.playbackName, hitStopCachedState.playbackRate);
		}

		// Token: 0x06002988 RID: 10632 RVA: 0x000AF064 File Offset: 0x000AD264
		protected void StartAimMode(float duration = 2f, bool snap = false)
		{
			this.StartAimMode(this.GetAimRay(), duration, snap);
		}

		// Token: 0x06002989 RID: 10633 RVA: 0x000AF074 File Offset: 0x000AD274
		protected void StartAimMode(Ray aimRay, float duration = 2f, bool snap = false)
		{
			if (base.characterDirection && aimRay.direction != Vector3.zero)
			{
				if (snap)
				{
					base.characterDirection.forward = aimRay.direction;
				}
				else
				{
					base.characterDirection.moveVector = aimRay.direction;
				}
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

		// Token: 0x0600298A RID: 10634 RVA: 0x000AF11B File Offset: 0x000AD31B
		protected bool RollCrit()
		{
			return base.characterBody && base.characterBody.master && Util.CheckRoll(this.critStat, base.characterBody.master);
		}

		// Token: 0x0600298B RID: 10635 RVA: 0x000AF154 File Offset: 0x000AD354
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

		// Token: 0x170003FD RID: 1021
		// (get) Token: 0x0600298C RID: 10636 RVA: 0x000AF188 File Offset: 0x000AD388
		protected bool isGrounded
		{
			get
			{
				return base.characterMotor && base.characterMotor.isGrounded;
			}
		}

		// Token: 0x0600298D RID: 10637 RVA: 0x000AF1A4 File Offset: 0x000AD3A4
		public TeamIndex GetTeam()
		{
			return TeamComponent.GetObjectTeam(base.gameObject);
		}

		// Token: 0x0600298E RID: 10638 RVA: 0x000AF1B1 File Offset: 0x000AD3B1
		public bool HasBuff(BuffIndex buffType)
		{
			return base.characterBody && base.characterBody.HasBuff(buffType);
		}

		// Token: 0x0600298F RID: 10639 RVA: 0x000AF1CE File Offset: 0x000AD3CE
		public int GetBuffCount(BuffIndex buffType)
		{
			if (!base.characterBody)
			{
				return 0;
			}
			return base.characterBody.GetBuffCount(buffType);
		}

		// Token: 0x06002990 RID: 10640 RVA: 0x000AF1EB File Offset: 0x000AD3EB
		protected void AttemptToStartSprint()
		{
			if (base.inputBank)
			{
				base.inputBank.sprint.down = true;
			}
		}

		// Token: 0x06002991 RID: 10641 RVA: 0x000AF20C File Offset: 0x000AD40C
		protected HitBoxGroup FindHitBoxGroup(string groupName)
		{
			Transform modelTransform = base.GetModelTransform();
			if (!modelTransform)
			{
				return null;
			}
			HitBoxGroup result = null;
			List<HitBoxGroup> gameObjectComponents = GetComponentsCache<HitBoxGroup>.GetGameObjectComponents(modelTransform.gameObject);
			int i = 0;
			int count = gameObjectComponents.Count;
			while (i < count)
			{
				if (gameObjectComponents[i].groupName == groupName)
				{
					result = gameObjectComponents[i];
					break;
				}
				i++;
			}
			GetComponentsCache<HitBoxGroup>.ReturnBuffer(gameObjectComponents);
			return result;
		}

		// Token: 0x04002587 RID: 9607
		protected float attackSpeedStat = 1f;

		// Token: 0x04002588 RID: 9608
		protected float damageStat;

		// Token: 0x04002589 RID: 9609
		protected float critStat;

		// Token: 0x0400258A RID: 9610
		protected float moveSpeedStat;

		// Token: 0x0400258B RID: 9611
		private const float defaultAimDuration = 2f;

		// Token: 0x020006FC RID: 1788
		protected struct HitStopCachedState
		{
			// Token: 0x0400258C RID: 9612
			public Vector3 characterVelocity;

			// Token: 0x0400258D RID: 9613
			public string playbackName;

			// Token: 0x0400258E RID: 9614
			public float playbackRate;
		}
	}
}
