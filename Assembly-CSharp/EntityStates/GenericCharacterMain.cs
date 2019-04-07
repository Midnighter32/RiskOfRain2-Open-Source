using System;
using RoR2;
using UnityEngine;

namespace EntityStates
{
	// Token: 0x020000B5 RID: 181
	public class GenericCharacterMain : BaseCharacterMain
	{
		// Token: 0x06000397 RID: 919 RVA: 0x0000E774 File Offset: 0x0000C974
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

		// Token: 0x06000398 RID: 920 RVA: 0x0000E7CC File Offset: 0x0000C9CC
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

		// Token: 0x06000399 RID: 921 RVA: 0x0000E846 File Offset: 0x0000CA46
		public override void Update()
		{
			base.Update();
		}

		// Token: 0x0600039A RID: 922 RVA: 0x0000E850 File Offset: 0x0000CA50
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.GatherInputs();
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
				if (this.hasCharacterMotor)
				{
					bool flag = false;
					bool flag2 = false;
					if (this.jumpInputReceived && base.characterBody && base.characterMotor.jumpCount < base.characterBody.maxJumpCount)
					{
						int itemCount = base.characterBody.inventory.GetItemCount(ItemIndex.JumpBoost);
						float d = 1f;
						float num3 = 1f;
						if (base.characterMotor.jumpCount >= base.characterBody.baseJumpCount)
						{
							flag = true;
							d = 1.5f;
							num3 = 1.5f;
						}
						else if ((float)itemCount > 0f && base.characterBody.isSprinting)
						{
							flag2 = true;
							float num4 = Mathf.Sqrt(10f * (float)itemCount / (base.characterBody.acceleration * base.characterMotor.airControl));
							float num5 = base.characterBody.moveSpeed / (base.characterBody.acceleration * base.characterMotor.airControl);
							d = (num4 + num5) / num5;
						}
						Vector3 moveDirection = base.characterMotor.moveDirection;
						moveDirection.y = 0f;
						moveDirection.Normalize();
						base.characterMotor.velocity = moveDirection * base.characterBody.moveSpeed * d;
						base.characterMotor.velocity.y = base.characterBody.jumpPower * num3;
						base.characterMotor.Motor.ForceUnground();
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
							EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/FeatherEffect"), new EffectData
							{
								origin = base.characterBody.footPosition
							}, true);
						}
						else if (base.characterMotor.jumpCount > 0)
						{
							EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/CharacterLandImpact"), new EffectData
							{
								origin = base.characterBody.footPosition,
								scale = base.characterBody.radius
							}, true);
						}
						if (flag2)
						{
							EffectManager.instance.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/BoostJumpEffect"), new EffectData
							{
								origin = base.characterBody.footPosition,
								rotation = Util.QuaternionSafeLookRotation(base.characterMotor.velocity)
							}, true);
						}
						base.characterMotor.jumpCount++;
					}
				}
				if (this.hasCharacterBody)
				{
					bool flag3 = this.sprintInputReceived;
					if (this.moveVector.magnitude <= 0.5f)
					{
						flag3 = false;
					}
					if (this.hasCameraTargetParams)
					{
						if (flag3)
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
					base.characterBody.isSprinting = flag3;
				}
				if (this.hasSkillLocator)
				{
					if (base.skillLocator.primary && ((this.skill1InputReceived && !base.skillLocator.primary.mustKeyPress) || base.inputBank.skill1.justPressed))
					{
						base.skillLocator.primary.ExecuteIfReady();
					}
					if (base.skillLocator.secondary && ((this.skill2InputReceived && !base.skillLocator.secondary.mustKeyPress) || base.inputBank.skill2.justPressed))
					{
						base.skillLocator.secondary.ExecuteIfReady();
					}
					if (base.skillLocator.utility && ((this.skill3InputReceived && !base.skillLocator.utility.mustKeyPress) || base.inputBank.skill3.justPressed))
					{
						base.skillLocator.utility.ExecuteIfReady();
					}
					if (base.skillLocator.special && ((this.skill4InputReceived && !base.skillLocator.special.mustKeyPress) || base.inputBank.skill4.justPressed))
					{
						base.skillLocator.special.ExecuteIfReady();
					}
				}
				this.skill1InputReceived = false;
				this.skill2InputReceived = false;
				this.skill3InputReceived = false;
				this.skill4InputReceived = false;
				this.jumpInputReceived = false;
				this.sprintInputReceived = false;
			}
		}

		// Token: 0x0600039B RID: 923 RVA: 0x0000EE90 File Offset: 0x0000D090
		private void GatherInputs()
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

		// Token: 0x04000357 RID: 855
		private AimAnimator aimAnimator;

		// Token: 0x04000358 RID: 856
		private bool skill1InputReceived;

		// Token: 0x04000359 RID: 857
		private bool skill2InputReceived;

		// Token: 0x0400035A RID: 858
		private bool skill3InputReceived;

		// Token: 0x0400035B RID: 859
		private bool skill4InputReceived;

		// Token: 0x0400035C RID: 860
		private bool jumpInputReceived;

		// Token: 0x0400035D RID: 861
		private bool sprintInputReceived;

		// Token: 0x0400035E RID: 862
		private Vector3 moveVector = Vector3.zero;

		// Token: 0x0400035F RID: 863
		private Vector3 aimDirection = Vector3.forward;

		// Token: 0x04000360 RID: 864
		private int emoteRequest = -1;

		// Token: 0x04000361 RID: 865
		private bool hasAimAnimator;
	}
}
