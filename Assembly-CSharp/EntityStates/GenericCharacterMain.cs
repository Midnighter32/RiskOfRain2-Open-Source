using System;
using System.Runtime.CompilerServices;
using RoR2;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x0200070C RID: 1804
	public class GenericCharacterMain : BaseCharacterMain
	{
		// Token: 0x06002A1D RID: 10781 RVA: 0x000B0F68 File Offset: 0x000AF168
		public override void OnEnter()
		{
			base.OnEnter();
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				this.aimAnimator = modelTransform.GetComponent<AimAnimator>();
				if (this.aimAnimator)
				{
					this.aimAnimator.enabled = true;
				}
			}
			this.hasAimAnimator = this.aimAnimator;
		}

		// Token: 0x06002A1E RID: 10782 RVA: 0x000B0FC0 File Offset: 0x000AF1C0
		public override void OnExit()
		{
			Transform modelTransform = base.GetModelTransform();
			if (modelTransform)
			{
				AimAnimator component = modelTransform.GetComponent<AimAnimator>();
				if (component)
				{
					component.enabled = false;
				}
			}
			if (base.isAuthority)
			{
				if (base.characterMotor)
				{
					base.characterMotor.moveDirection = Vector3.zero;
				}
				if (base.railMotor)
				{
					base.railMotor.inputMoveVector = Vector3.zero;
				}
			}
			base.OnExit();
		}

		// Token: 0x06002A1F RID: 10783 RVA: 0x000B103A File Offset: 0x000AF23A
		public override void Update()
		{
			base.Update();
		}

		// Token: 0x06002A20 RID: 10784 RVA: 0x000B1042 File Offset: 0x000AF242
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.GatherInputs();
			this.HandleMovements();
			this.PerformInputs();
		}

		// Token: 0x06002A21 RID: 10785 RVA: 0x000B105C File Offset: 0x000AF25C
		public virtual void HandleMovements()
		{
			if (this.useRootMotion)
			{
				if (this.hasCharacterMotor)
				{
					base.characterMotor.moveDirection = Vector3.zero;
				}
				if (this.hasRailMotor)
				{
					base.railMotor.inputMoveVector = this.moveVector;
				}
			}
			else
			{
				if (this.hasCharacterMotor)
				{
					base.characterMotor.moveDirection = this.moveVector;
				}
				if (this.hasRailMotor)
				{
					base.railMotor.inputMoveVector = this.moveVector;
				}
			}
			bool isGrounded = base.isGrounded;
			if (!this.hasRailMotor && this.hasCharacterDirection && this.hasCharacterBody)
			{
				if (this.hasAimAnimator && this.aimAnimator.aimType == AimAnimator.AimType.Smart)
				{
					Vector3 vector = (this.moveVector == Vector3.zero) ? base.characterDirection.forward : this.moveVector;
					float num = Vector3.Angle(this.aimDirection, vector);
					float num2 = Mathf.Max(this.aimAnimator.pitchRangeMax + this.aimAnimator.pitchGiveupRange, this.aimAnimator.yawRangeMax + this.aimAnimator.yawGiveupRange);
					base.characterDirection.moveVector = ((base.characterBody && base.characterBody.shouldAim && num > num2) ? this.aimDirection : vector);
				}
				else
				{
					base.characterDirection.moveVector = ((base.characterBody && base.characterBody.shouldAim) ? this.aimDirection : this.moveVector);
				}
			}
			if (base.isAuthority)
			{
				this.ProcessJump();
				if (this.hasCharacterBody)
				{
					bool flag = this.sprintInputReceived;
					if (this.moveVector.magnitude <= 0.5f)
					{
						flag = false;
					}
					if (this.hasCameraTargetParams)
					{
						if (flag)
						{
							if (base.cameraTargetParams.aimMode == CameraTargetParams.AimType.Standard)
							{
								base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Sprinting;
							}
						}
						else if (base.cameraTargetParams.aimMode == CameraTargetParams.AimType.Sprinting)
						{
							base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;
						}
					}
					base.characterBody.isSprinting = flag;
				}
			}
		}

		// Token: 0x06002A22 RID: 10786 RVA: 0x000B1268 File Offset: 0x000AF468
		public static void ApplyJumpVelocity(CharacterMotor characterMotor, CharacterBody characterBody, float horizontalBonus, float verticalBonus)
		{
			Vector3 moveDirection = characterMotor.moveDirection;
			moveDirection.y = 0f;
			moveDirection.Normalize();
			characterMotor.velocity = moveDirection * characterBody.moveSpeed * horizontalBonus;
			characterMotor.velocity.y = characterBody.jumpPower * verticalBonus;
			characterMotor.Motor.ForceUnground();
		}

		// Token: 0x06002A23 RID: 10787 RVA: 0x000B12C8 File Offset: 0x000AF4C8
		public virtual void ProcessJump()
		{
			if (this.hasCharacterMotor)
			{
				bool flag = false;
				bool flag2 = false;
				if (this.jumpInputReceived && base.characterBody && base.characterMotor.jumpCount < base.characterBody.maxJumpCount)
				{
					int itemCount = base.characterBody.inventory.GetItemCount(ItemIndex.JumpBoost);
					float horizontalBonus = 1f;
					float verticalBonus = 1f;
					if (base.characterMotor.jumpCount >= base.characterBody.baseJumpCount)
					{
						flag = true;
						horizontalBonus = 1.5f;
						verticalBonus = 1.5f;
					}
					else if ((float)itemCount > 0f && base.characterBody.isSprinting)
					{
						flag2 = true;
						float num = Mathf.Sqrt(10f * (float)itemCount / (base.characterBody.acceleration * base.characterMotor.airControl));
						float num2 = base.characterBody.moveSpeed / (base.characterBody.acceleration * base.characterMotor.airControl);
						horizontalBonus = (num + num2) / num2;
					}
					GenericCharacterMain.ApplyJumpVelocity(base.characterMotor, base.characterBody, horizontalBonus, verticalBonus);
					if (this.hasModelAnimator)
					{
						int layerIndex = base.modelAnimator.GetLayerIndex("Body");
						if (layerIndex >= 0)
						{
							if (base.characterMotor.jumpCount == 0 || base.characterBody.baseJumpCount == 1)
							{
								base.modelAnimator.CrossFade("Jump", this.smoothingParameters.intoJumpTransitionTime, layerIndex);
							}
							else
							{
								base.modelAnimator.CrossFade("BonusJump", this.smoothingParameters.intoJumpTransitionTime, layerIndex);
							}
						}
					}
					if (flag)
					{
						EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/FeatherEffect"), new EffectData
						{
							origin = base.characterBody.footPosition
						}, true);
					}
					else if (base.characterMotor.jumpCount > 0)
					{
						EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/CharacterLandImpact"), new EffectData
						{
							origin = base.characterBody.footPosition,
							scale = base.characterBody.radius
						}, true);
					}
					if (flag2)
					{
						EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/BoostJumpEffect"), new EffectData
						{
							origin = base.characterBody.footPosition,
							rotation = Util.QuaternionSafeLookRotation(base.characterMotor.velocity)
						}, true);
					}
					base.characterMotor.jumpCount++;
				}
			}
		}

		// Token: 0x06002A24 RID: 10788 RVA: 0x0000B933 File Offset: 0x00009B33
		protected virtual bool CanExecuteSkill(GenericSkill skillSlot)
		{
			return true;
		}

		// Token: 0x06002A25 RID: 10789 RVA: 0x000B1518 File Offset: 0x000AF718
		protected void PerformInputs()
		{
			if (base.isAuthority)
			{
				if (this.hasSkillLocator)
				{
					this.<PerformInputs>g__HandleSkill|19_0(base.skillLocator.primary, ref this.skill1InputReceived, base.inputBank.skill1.justPressed);
					this.<PerformInputs>g__HandleSkill|19_0(base.skillLocator.secondary, ref this.skill2InputReceived, base.inputBank.skill2.justPressed);
					this.<PerformInputs>g__HandleSkill|19_0(base.skillLocator.utility, ref this.skill3InputReceived, base.inputBank.skill3.justPressed);
					this.<PerformInputs>g__HandleSkill|19_0(base.skillLocator.special, ref this.skill4InputReceived, base.inputBank.skill4.justPressed);
				}
				this.jumpInputReceived = false;
				this.sprintInputReceived = false;
			}
		}

		// Token: 0x06002A26 RID: 10790 RVA: 0x000B15E8 File Offset: 0x000AF7E8
		protected void GatherInputs()
		{
			if (this.hasInputBank)
			{
				this.moveVector = base.inputBank.moveVector;
				this.aimDirection = base.inputBank.aimDirection;
				this.emoteRequest = base.inputBank.emoteRequest;
				base.inputBank.emoteRequest = -1;
				this.skill1InputReceived |= base.inputBank.skill1.down;
				this.skill2InputReceived |= base.inputBank.skill2.down;
				this.skill3InputReceived |= base.inputBank.skill3.down;
				this.skill4InputReceived |= base.inputBank.skill4.down;
				this.jumpInputReceived = base.inputBank.jump.justPressed;
				this.sprintInputReceived |= base.inputBank.sprint.down;
			}
		}

		// Token: 0x06002A28 RID: 10792 RVA: 0x000B170C File Offset: 0x000AF90C
		[CompilerGenerated]
		private void <PerformInputs>g__HandleSkill|19_0(GenericSkill skillSlot, ref bool inputReceived, bool justPressed)
		{
			bool flag = inputReceived;
			inputReceived = false;
			if (!skillSlot)
			{
				return;
			}
			if ((justPressed || (flag && !skillSlot.mustKeyPress)) && this.CanExecuteSkill(skillSlot))
			{
				skillSlot.ExecuteIfReady();
			}
		}

		// Token: 0x040025F2 RID: 9714
		private AimAnimator aimAnimator;

		// Token: 0x040025F3 RID: 9715
		protected bool skill1InputReceived;

		// Token: 0x040025F4 RID: 9716
		protected bool skill2InputReceived;

		// Token: 0x040025F5 RID: 9717
		protected bool skill3InputReceived;

		// Token: 0x040025F6 RID: 9718
		protected bool skill4InputReceived;

		// Token: 0x040025F7 RID: 9719
		protected bool jumpInputReceived;

		// Token: 0x040025F8 RID: 9720
		protected bool sprintInputReceived;

		// Token: 0x040025F9 RID: 9721
		private Vector3 moveVector = Vector3.zero;

		// Token: 0x040025FA RID: 9722
		private Vector3 aimDirection = Vector3.forward;

		// Token: 0x040025FB RID: 9723
		private int emoteRequest = -1;

		// Token: 0x040025FC RID: 9724
		private bool hasAimAnimator;
	}
}
