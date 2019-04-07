using System;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Navigation;
using UnityEngine;

namespace EntityStates.AI.Walker
{
	// Token: 0x020001E4 RID: 484
	public class Combat : BaseAIState
	{
		// Token: 0x06000972 RID: 2418 RVA: 0x0002F2D9 File Offset: 0x0002D4D9
		public override void OnEnter()
		{
			base.OnEnter();
			this.pathUpdateTimer = 0f;
			this.activeSoundTimer = UnityEngine.Random.Range(3f, 8f);
		}

		// Token: 0x06000973 RID: 2419 RVA: 0x0002F301 File Offset: 0x0002D501
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06000974 RID: 2420 RVA: 0x0002F30C File Offset: 0x0002D50C
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.ai && base.body)
			{
				Vector3 footPosition = base.body.footPosition;
				base.ai.pathFollower.UpdatePosition(footPosition);
				this.updateTimer -= Time.fixedDeltaTime;
				this.pathUpdateTimer -= Time.fixedDeltaTime;
				this.strafeTimer -= Time.fixedDeltaTime;
				if (this.updateTimer <= 0f)
				{
					this.updateTimer = UnityEngine.Random.Range(0.16666667f, 0.2f);
					AISkillDriver.MovementType movementType = AISkillDriver.MovementType.Stop;
					SkillSlot skillSlot = SkillSlot.None;
					bool flag = false;
					BaseAI.SkillDriverEvaluation skillDriverEvaluation = base.ai.skillDriverEvaluation;
					this.dominantSkillDriver = skillDriverEvaluation.dominantSkillDriver;
					BaseAI.Target target = skillDriverEvaluation.target;
					if (this.dominantSkillDriver)
					{
						movementType = this.dominantSkillDriver.movementType;
						skillSlot = this.dominantSkillDriver.skillSlot;
						flag = this.dominantSkillDriver.activationRequiresTargetLoS;
					}
					Vector3 position = base.body.transform.position;
					Vector3 b = base.bodyInputBank ? base.bodyInputBank.aimOrigin : position;
					Vector3 vector = position;
					Vector3 vector2 = vector;
					this.moveTargetPositionFound = (target != null && target.GetBullseyePosition(out vector));
					if (this.moveTargetPositionFound)
					{
						CharacterBody characterBody = target.characterBody;
						if (characterBody)
						{
							vector2 = characterBody.footPosition;
						}
						else
						{
							vector2 = vector;
						}
						if (this.pathUpdateTimer <= 0f)
						{
							base.ai.RefreshPath(footPosition, vector2);
							this.pathUpdateTimer = this.pathUpdateInterval;
						}
					}
					Vector3 normalized = (vector - b).normalized;
					bool flag2 = true;
					if (this.dominantSkillDriver && this.dominantSkillDriver.activationRequiresAimConfirmation)
					{
						flag2 = false;
						if (base.bodyInputBank && Vector3.Dot(base.bodyInputBank.aimDirection.normalized, normalized) >= 0.95f)
						{
							flag2 = true;
						}
					}
					Vector3 normalized2 = (((this.dominantSkillDriver && this.dominantSkillDriver.ignoreNodeGraph) ? ((base.ai.nodegraphType == MapNodeGroup.GraphType.Ground) ? vector2 : vector) : base.ai.pathFollower.GetNextPosition()) - footPosition).normalized;
					Vector3 vector3 = Vector3.Cross(Vector3.up, normalized2);
					Vector3 vector4 = position;
					if (movementType == AISkillDriver.MovementType.ChaseMoveTarget)
					{
						vector4 += normalized2;
					}
					else if (movementType == AISkillDriver.MovementType.FleeMoveTarget)
					{
						vector4 -= normalized2;
					}
					else if (movementType == AISkillDriver.MovementType.StrafeMovetarget)
					{
						if (this.strafeTimer <= 0f)
						{
							this.strafeDirection = 0f;
						}
						if (this.strafeDirection == 0f)
						{
							LayerMask mask = LayerIndex.world.mask | LayerIndex.defaultLayer.mask;
							float num = 0.25f * base.body.moveSpeed;
							float num2 = num;
							float num3 = num;
							HullDef hullDef = HullDef.Find(base.body.hullClassification);
							float radius = hullDef.radius;
							float height = hullDef.height;
							Vector3 b2 = Vector3.up * (height * 0.5f - radius);
							RaycastHit raycastHit;
							if (Physics.CapsuleCast(position + b2, position - b2, radius, vector3, out raycastHit, num, mask))
							{
								num2 = raycastHit.distance;
							}
							if (Physics.CapsuleCast(position + b2, position - b2, radius, -vector3, out raycastHit, num, mask))
							{
								num3 = raycastHit.distance;
							}
							if (num3 == num2)
							{
								this.strafeDirection = ((UnityEngine.Random.Range(0, 1) == 0) ? -1f : 1f);
							}
							else
							{
								this.strafeDirection = ((num3 < num2) ? 1f : -1f);
							}
							if (this.strafeDirection != 0f)
							{
								this.strafeTimer = 0.25f;
							}
						}
						vector4 += vector3 * this.strafeDirection;
					}
					base.ai.localNavigator.targetPosition = vector4;
					base.ai.localNavigator.Update(Time.fixedDeltaTime);
					bool newState = false;
					bool newState2 = false;
					bool newState3 = false;
					bool newState4 = false;
					if (((target != null) ? target.gameObject : null) && flag2 && (!flag || base.ai.HasLOS(position, vector)))
					{
						switch (skillSlot)
						{
						case SkillSlot.Primary:
							newState = true;
							break;
						case SkillSlot.Secondary:
							newState2 = true;
							break;
						case SkillSlot.Utility:
							newState3 = true;
							break;
						case SkillSlot.Special:
							newState4 = true;
							break;
						}
					}
					Vector3 vector5 = base.ai.localNavigator.moveVector;
					if (base.bodyCharacterMotor && movementType == AISkillDriver.MovementType.ChaseMoveTarget && Vector3.Dot(vector5, base.bodyCharacterMotor.velocity.normalized) <= 0.5f)
					{
						vector5 *= 0.5f;
					}
					if (this.dominantSkillDriver)
					{
						vector5 *= this.dominantSkillDriver.moveInputScale;
					}
					if (base.bodyInputBank)
					{
						base.bodyInputBank.skill1.PushState(newState);
						base.bodyInputBank.skill2.PushState(newState2);
						base.bodyInputBank.skill3.PushState(newState3);
						base.bodyInputBank.skill4.PushState(newState4);
						bool newState5 = false;
						if (base.bodyCharacterMotor && base.bodyCharacterMotor.isGrounded && Mathf.Max(base.ai.pathFollower.CalculateJumpVelocityNeededToReachNextWaypoint(base.body.moveSpeed), base.ai.localNavigator.jumpSpeed) > 0f)
						{
							newState5 = true;
						}
						base.bodyInputBank.jump.PushState(newState5);
						base.bodyInputBank.moveVector = vector5;
					}
					if (!this.dominantSkillDriver)
					{
						this.outer.SetNextState(new LookBusy());
						return;
					}
				}
				this.activeSoundTimer -= Time.fixedDeltaTime;
				if (this.activeSoundTimer <= 0f)
				{
					this.activeSoundTimer = UnityEngine.Random.Range(3f, 8f);
					base.body.CallRpcBark();
				}
			}
		}

		// Token: 0x04000CC0 RID: 3264
		private float pathUpdateTimer;

		// Token: 0x04000CC1 RID: 3265
		private float pathUpdateInterval = 1f;

		// Token: 0x04000CC2 RID: 3266
		private float strafeDirection;

		// Token: 0x04000CC3 RID: 3267
		private const float strafeDuration = 0.25f;

		// Token: 0x04000CC4 RID: 3268
		private float strafeTimer;

		// Token: 0x04000CC5 RID: 3269
		private float activeSoundTimer;

		// Token: 0x04000CC6 RID: 3270
		private float updateTimer;

		// Token: 0x04000CC7 RID: 3271
		private const float minUpdateInterval = 0.16666667f;

		// Token: 0x04000CC8 RID: 3272
		private const float maxUpdateInterval = 0.2f;

		// Token: 0x04000CC9 RID: 3273
		private AISkillDriver dominantSkillDriver;

		// Token: 0x04000CCA RID: 3274
		public bool moveTargetPositionFound;
	}
}
