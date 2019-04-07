using System;
using RoR2;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x020000A6 RID: 166
	public class BaseCharacterMain : BaseState
	{
		// Token: 0x17000078 RID: 120
		// (get) Token: 0x0600031A RID: 794 RVA: 0x0000CA5F File Offset: 0x0000AC5F
		// (set) Token: 0x0600031B RID: 795 RVA: 0x0000CA67 File Offset: 0x0000AC67
		private protected Animator modelAnimator { protected get; private set; }

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600031C RID: 796 RVA: 0x0000CA70 File Offset: 0x0000AC70
		// (set) Token: 0x0600031D RID: 797 RVA: 0x0000CA78 File Offset: 0x0000AC78
		private protected Vector3 estimatedVelocity { protected get; private set; }

		// Token: 0x0600031E RID: 798 RVA: 0x0000CA84 File Offset: 0x0000AC84
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
				this.modelAnimator.CrossFadeInFixedTime("Idle", 0.1f, layerIndex);
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

		// Token: 0x0600031F RID: 799 RVA: 0x0000CC04 File Offset: 0x0000AE04
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

		// Token: 0x06000320 RID: 800 RVA: 0x0000CC80 File Offset: 0x0000AE80
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

		// Token: 0x06000321 RID: 801 RVA: 0x0000CD0C File Offset: 0x0000AF0C
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

		// Token: 0x06000322 RID: 802 RVA: 0x0000CE54 File Offset: 0x0000B054
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

		// Token: 0x04000302 RID: 770
		private RootMotionAccumulator rootMotionAccumulator;

		// Token: 0x04000304 RID: 772
		private Vector3 previousPosition;

		// Token: 0x04000306 RID: 774
		protected CharacterAnimParamAvailability characterAnimParamAvailability;

		// Token: 0x04000307 RID: 775
		private CharacterAnimatorWalkParamCalculator animatorWalkParamCalculator;

		// Token: 0x04000308 RID: 776
		protected BodyAnimatorSmoothingParameters.SmoothingParameters smoothingParameters;

		// Token: 0x04000309 RID: 777
		protected bool useRootMotion;

		// Token: 0x0400030A RID: 778
		private bool wasGrounded;

		// Token: 0x0400030B RID: 779
		private float lastYSpeed;

		// Token: 0x0400030C RID: 780
		protected bool hasCharacterMotor;

		// Token: 0x0400030D RID: 781
		protected bool hasCharacterDirection;

		// Token: 0x0400030E RID: 782
		protected bool hasCharacterBody;

		// Token: 0x0400030F RID: 783
		protected bool hasRailMotor;

		// Token: 0x04000310 RID: 784
		protected bool hasCameraTargetParams;

		// Token: 0x04000311 RID: 785
		protected bool hasSkillLocator;

		// Token: 0x04000312 RID: 786
		protected bool hasModelAnimator;

		// Token: 0x04000313 RID: 787
		protected bool hasInputBank;

		// Token: 0x04000314 RID: 788
		protected bool hasRootMotionAccumulator;
	}
}
