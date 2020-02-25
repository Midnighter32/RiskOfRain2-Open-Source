using System;
using RoR2;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x020006F9 RID: 1785
	public class BaseCharacterMain : BaseState
	{
		// Token: 0x170003FB RID: 1019
		// (get) Token: 0x06002972 RID: 10610 RVA: 0x000AE63A File Offset: 0x000AC83A
		// (set) Token: 0x06002973 RID: 10611 RVA: 0x000AE642 File Offset: 0x000AC842
		private protected Animator modelAnimator { protected get; private set; }

		// Token: 0x170003FC RID: 1020
		// (get) Token: 0x06002974 RID: 10612 RVA: 0x000AE64B File Offset: 0x000AC84B
		// (set) Token: 0x06002975 RID: 10613 RVA: 0x000AE653 File Offset: 0x000AC853
		private protected Vector3 estimatedVelocity { protected get; private set; }

		// Token: 0x06002976 RID: 10614 RVA: 0x000AE65C File Offset: 0x000AC85C
		public override void OnEnter()
		{
			base.OnEnter();
			this.modelAnimator = base.GetModelAnimator();
			this.rootMotionAccumulator = base.GetModelRootMotionAccumulator();
			if (this.modelAnimator)
			{
				this.characterAnimParamAvailability = CharacterAnimParamAvailability.FromAnimator(this.modelAnimator);
				int layerIndex = this.modelAnimator.GetLayerIndex("Body");
				if (this.characterAnimParamAvailability.isGrounded)
				{
					this.wasGrounded = base.isGrounded;
					this.modelAnimator.SetBool(AnimationParameters.isGrounded, this.wasGrounded);
				}
				if (base.isGrounded)
				{
					this.modelAnimator.CrossFadeInFixedTime("Idle", 0.1f, layerIndex);
				}
				else
				{
					this.modelAnimator.CrossFadeInFixedTime("AscendDescend", 0.1f, layerIndex);
				}
				this.modelAnimator.Update(0f);
			}
			if (this.rootMotionAccumulator)
			{
				this.rootMotionAccumulator.ExtractRootMotion();
			}
			base.GetBodyAnimatorSmoothingParameters(out this.smoothingParameters);
			this.previousPosition = base.transform.position;
			this.hasCharacterMotor = base.characterMotor;
			this.hasCharacterDirection = base.characterDirection;
			this.hasCharacterBody = base.characterBody;
			this.hasRailMotor = base.railMotor;
			this.hasCameraTargetParams = base.cameraTargetParams;
			this.hasSkillLocator = base.skillLocator;
			this.hasModelAnimator = this.modelAnimator;
			this.hasInputBank = base.inputBank;
			this.hasRootMotionAccumulator = this.rootMotionAccumulator;
		}

		// Token: 0x06002977 RID: 10615 RVA: 0x000AE800 File Offset: 0x000ACA00
		public override void OnExit()
		{
			if (this.rootMotionAccumulator)
			{
				this.rootMotionAccumulator.ExtractRootMotion();
			}
			if (this.modelAnimator)
			{
				if (this.characterAnimParamAvailability.isMoving)
				{
					this.modelAnimator.SetBool(AnimationParameters.isMoving, false);
				}
				if (this.characterAnimParamAvailability.turnAngle)
				{
					this.modelAnimator.SetFloat(AnimationParameters.turnAngle, 0f);
				}
			}
			base.OnExit();
		}

		// Token: 0x06002978 RID: 10616 RVA: 0x000AE87C File Offset: 0x000ACA7C
		public override void Update()
		{
			base.Update();
			if (Time.deltaTime <= 0f)
			{
				return;
			}
			Vector3 position = base.transform.position;
			this.estimatedVelocity = (position - this.previousPosition) / Time.deltaTime;
			this.previousPosition = position;
			this.useRootMotion = ((base.characterBody && base.characterBody.rootMotionInMainState && base.isGrounded) || base.railMotor);
			this.UpdateAnimationParameters();
		}

		// Token: 0x06002979 RID: 10617 RVA: 0x000AE908 File Offset: 0x000ACB08
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (this.hasCharacterMotor)
			{
				float num = this.estimatedVelocity.y - this.lastYSpeed;
				if (base.isGrounded && !this.wasGrounded && this.hasModelAnimator)
				{
					int layerIndex = this.modelAnimator.GetLayerIndex("Impact");
					if (layerIndex >= 0)
					{
						this.modelAnimator.SetLayerWeight(layerIndex, Mathf.Clamp01(Mathf.Max(new float[]
						{
							0.3f,
							num / 5f,
							this.modelAnimator.GetLayerWeight(layerIndex)
						})));
						this.modelAnimator.PlayInFixedTime("LightImpact", layerIndex, 0f);
					}
				}
				this.wasGrounded = base.isGrounded;
				this.lastYSpeed = this.estimatedVelocity.y;
			}
			if (this.hasRootMotionAccumulator)
			{
				Vector3 vector = this.rootMotionAccumulator.ExtractRootMotion();
				if (this.useRootMotion && vector != Vector3.zero && base.isAuthority)
				{
					if (base.characterMotor)
					{
						base.characterMotor.rootMotion += vector;
					}
					if (base.railMotor)
					{
						base.railMotor.rootMotion += vector;
					}
				}
			}
		}

		// Token: 0x0600297A RID: 10618 RVA: 0x000AEA50 File Offset: 0x000ACC50
		protected virtual void UpdateAnimationParameters()
		{
			if (this.hasRailMotor || !this.hasModelAnimator)
			{
				return;
			}
			Vector3 vector = base.inputBank ? base.inputBank.moveVector : Vector3.zero;
			bool value = vector != Vector3.zero;
			this.animatorWalkParamCalculator.Update(vector, base.characterDirection ? base.characterDirection.animatorForward : base.transform.forward, this.smoothingParameters, Time.fixedDeltaTime);
			if (this.useRootMotion)
			{
				if (this.characterAnimParamAvailability.mainRootPlaybackRate)
				{
					float num = 1f;
					if (base.modelLocator && base.modelLocator.modelTransform)
					{
						num = base.modelLocator.modelTransform.localScale.x;
					}
					float value2 = base.characterBody.moveSpeed / (base.characterBody.mainRootSpeed * num);
					this.modelAnimator.SetFloat(AnimationParameters.mainRootPlaybackRate, value2);
				}
			}
			else if (this.characterAnimParamAvailability.walkSpeed)
			{
				this.modelAnimator.SetFloat(AnimationParameters.walkSpeed, base.characterBody.moveSpeed);
			}
			if (this.characterAnimParamAvailability.isGrounded)
			{
				this.modelAnimator.SetBool(AnimationParameters.isGrounded, base.isGrounded);
			}
			if (this.characterAnimParamAvailability.isMoving)
			{
				this.modelAnimator.SetBool(AnimationParameters.isMoving, value);
			}
			if (this.characterAnimParamAvailability.turnAngle)
			{
				this.modelAnimator.SetFloat(AnimationParameters.turnAngle, this.animatorWalkParamCalculator.remainingTurnAngle, this.smoothingParameters.turnAngleSmoothDamp, Time.fixedDeltaTime);
			}
			if (this.characterAnimParamAvailability.isSprinting)
			{
				this.modelAnimator.SetBool(AnimationParameters.isSprinting, base.characterBody.isSprinting);
			}
			if (this.characterAnimParamAvailability.forwardSpeed)
			{
				this.modelAnimator.SetFloat(AnimationParameters.forwardSpeed, this.animatorWalkParamCalculator.animatorWalkSpeed.x, this.smoothingParameters.forwardSpeedSmoothDamp, Time.deltaTime);
			}
			if (this.characterAnimParamAvailability.rightSpeed)
			{
				this.modelAnimator.SetFloat(AnimationParameters.rightSpeed, this.animatorWalkParamCalculator.animatorWalkSpeed.y, this.smoothingParameters.rightSpeedSmoothDamp, Time.deltaTime);
			}
			if (this.characterAnimParamAvailability.upSpeed)
			{
				this.modelAnimator.SetFloat(AnimationParameters.upSpeed, this.estimatedVelocity.y, 0.1f, Time.deltaTime);
			}
		}

		// Token: 0x04002573 RID: 9587
		private RootMotionAccumulator rootMotionAccumulator;

		// Token: 0x04002575 RID: 9589
		private Vector3 previousPosition;

		// Token: 0x04002577 RID: 9591
		protected CharacterAnimParamAvailability characterAnimParamAvailability;

		// Token: 0x04002578 RID: 9592
		private CharacterAnimatorWalkParamCalculator animatorWalkParamCalculator;

		// Token: 0x04002579 RID: 9593
		protected BodyAnimatorSmoothingParameters.SmoothingParameters smoothingParameters;

		// Token: 0x0400257A RID: 9594
		protected bool useRootMotion;

		// Token: 0x0400257B RID: 9595
		private bool wasGrounded;

		// Token: 0x0400257C RID: 9596
		private float lastYSpeed;

		// Token: 0x0400257D RID: 9597
		protected bool hasCharacterMotor;

		// Token: 0x0400257E RID: 9598
		protected bool hasCharacterDirection;

		// Token: 0x0400257F RID: 9599
		protected bool hasCharacterBody;

		// Token: 0x04002580 RID: 9600
		protected bool hasRailMotor;

		// Token: 0x04002581 RID: 9601
		protected bool hasCameraTargetParams;

		// Token: 0x04002582 RID: 9602
		protected bool hasSkillLocator;

		// Token: 0x04002583 RID: 9603
		protected bool hasModelAnimator;

		// Token: 0x04002584 RID: 9604
		protected bool hasInputBank;

		// Token: 0x04002585 RID: 9605
		protected bool hasRootMotionAccumulator;
	}
}
