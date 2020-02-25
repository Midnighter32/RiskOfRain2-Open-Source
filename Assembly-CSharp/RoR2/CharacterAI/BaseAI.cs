using System;
using System.Linq;
using EntityStates;
using JetBrains.Annotations;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

namespace RoR2.CharacterAI
{
	// Token: 0x0200056D RID: 1389
	[RequireComponent(typeof(CharacterMaster))]
	public class BaseAI : MonoBehaviour
	{
		// Token: 0x17000378 RID: 888
		// (get) Token: 0x06002106 RID: 8454 RVA: 0x0008EA86 File Offset: 0x0008CC86
		// (set) Token: 0x06002107 RID: 8455 RVA: 0x0008EA8E File Offset: 0x0008CC8E
		public CharacterMaster master { get; protected set; }

		// Token: 0x17000379 RID: 889
		// (get) Token: 0x06002108 RID: 8456 RVA: 0x0008EA97 File Offset: 0x0008CC97
		// (set) Token: 0x06002109 RID: 8457 RVA: 0x0008EA9F File Offset: 0x0008CC9F
		public CharacterBody body { get; protected set; }

		// Token: 0x1700037A RID: 890
		// (get) Token: 0x0600210A RID: 8458 RVA: 0x0008EAA8 File Offset: 0x0008CCA8
		// (set) Token: 0x0600210B RID: 8459 RVA: 0x0008EAB0 File Offset: 0x0008CCB0
		public Transform bodyTransform { get; protected set; }

		// Token: 0x1700037B RID: 891
		// (get) Token: 0x0600210C RID: 8460 RVA: 0x0008EAB9 File Offset: 0x0008CCB9
		// (set) Token: 0x0600210D RID: 8461 RVA: 0x0008EAC1 File Offset: 0x0008CCC1
		public CharacterDirection bodyCharacterDirection { get; protected set; }

		// Token: 0x1700037C RID: 892
		// (get) Token: 0x0600210E RID: 8462 RVA: 0x0008EACA File Offset: 0x0008CCCA
		// (set) Token: 0x0600210F RID: 8463 RVA: 0x0008EAD2 File Offset: 0x0008CCD2
		public CharacterMotor bodyCharacterMotor { get; protected set; }

		// Token: 0x1700037D RID: 893
		// (get) Token: 0x06002110 RID: 8464 RVA: 0x0008EADB File Offset: 0x0008CCDB
		// (set) Token: 0x06002111 RID: 8465 RVA: 0x0008EAE3 File Offset: 0x0008CCE3
		public InputBankTest bodyInputBank { get; protected set; }

		// Token: 0x1700037E RID: 894
		// (get) Token: 0x06002112 RID: 8466 RVA: 0x0008EAEC File Offset: 0x0008CCEC
		// (set) Token: 0x06002113 RID: 8467 RVA: 0x0008EAF4 File Offset: 0x0008CCF4
		public HealthComponent bodyHealthComponent { get; protected set; }

		// Token: 0x1700037F RID: 895
		// (get) Token: 0x06002114 RID: 8468 RVA: 0x0008EAFD File Offset: 0x0008CCFD
		// (set) Token: 0x06002115 RID: 8469 RVA: 0x0008EB05 File Offset: 0x0008CD05
		public SkillLocator bodySkillLocator { get; protected set; }

		// Token: 0x17000380 RID: 896
		// (get) Token: 0x06002116 RID: 8470 RVA: 0x0008EB0E File Offset: 0x0008CD0E
		// (set) Token: 0x06002117 RID: 8471 RVA: 0x0008EB16 File Offset: 0x0008CD16
		public NetworkIdentity networkIdentity { get; protected set; }

		// Token: 0x17000381 RID: 897
		// (get) Token: 0x06002118 RID: 8472 RVA: 0x0008EB1F File Offset: 0x0008CD1F
		// (set) Token: 0x06002119 RID: 8473 RVA: 0x0008EB27 File Offset: 0x0008CD27
		public AISkillDriver[] skillDrivers { get; protected set; }

		// Token: 0x0600211A RID: 8474 RVA: 0x0008EB30 File Offset: 0x0008CD30
		private void Awake()
		{
			this.targetRefreshTimer = 0.5f;
			this.master = base.GetComponent<CharacterMaster>();
			this.stateMachine = base.GetComponent<EntityStateMachine>();
			this.stateMachine.enabled = false;
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
			this.skillDrivers = base.GetComponents<AISkillDriver>();
			this.currentEnemy = new BaseAI.Target(this);
			this.leader = new BaseAI.Target(this);
			this.buddy = new BaseAI.Target(this);
			this.customTarget = new BaseAI.Target(this);
		}

		// Token: 0x0600211B RID: 8475 RVA: 0x0008EBB4 File Offset: 0x0008CDB4
		public void Start()
		{
			if (!Util.HasEffectiveAuthority(this.networkIdentity))
			{
				base.enabled = false;
				if (this.stateMachine)
				{
					this.stateMachine.enabled = false;
				}
			}
			if (NetworkServer.active)
			{
				this.skillDriverUpdateTimer = UnityEngine.Random.value;
			}
		}

		// Token: 0x0600211C RID: 8476 RVA: 0x0008EC00 File Offset: 0x0008CE00
		public void FixedUpdate()
		{
			if (this.drawAIPath)
			{
				this.DebugDrawPath(Color.red, Time.fixedDeltaTime);
			}
			this.enemyAttention -= Time.fixedDeltaTime;
			if (this.currentEnemy.characterBody && this.body && this.currentEnemy.characterBody.GetVisibilityLevel(this.body) < VisibilityLevel.Revealed)
			{
				this.currentEnemy.Reset();
			}
			if (this.pendingPath != null && this.pendingPath.status == PathTask.TaskStatus.Complete)
			{
				this.pathFollower.SetPath(this.pendingPath.path);
				this.pendingPath.path.Dispose();
				this.pendingPath = null;
			}
			if (this.body)
			{
				this.targetRefreshTimer -= Time.fixedDeltaTime;
				this.skillDriverUpdateTimer -= Time.fixedDeltaTime;
				if (this.skillDriverUpdateTimer <= 0f)
				{
					if (this.skillDriverEvaluation.dominantSkillDriver)
					{
						this.selectedSkilldriverName = this.skillDriverEvaluation.dominantSkillDriver.customName;
						if (this.skillDriverEvaluation.dominantSkillDriver.resetCurrentEnemyOnNextDriverSelection)
						{
							this.currentEnemy.Reset();
							this.targetRefreshTimer = 0f;
						}
					}
					if (!this.currentEnemy.gameObject && this.targetRefreshTimer <= 0f)
					{
						this.targetRefreshTimer = 0.5f;
						HurtBox hurtBox = this.FindEnemyHurtBox(float.PositiveInfinity, this.fullVision, true);
						if (hurtBox && hurtBox.healthComponent)
						{
							this.currentEnemy.gameObject = hurtBox.healthComponent.gameObject;
							this.currentEnemy.bestHurtBox = hurtBox;
						}
						if (this.currentEnemy.gameObject)
						{
							this.enemyAttention = this.enemyAttentionDuration;
						}
					}
					this.BeginSkillDriver(this.EvaluateSkillDrivers());
				}
			}
			this.PickCurrentNodeGraph();
			if (this.bodyInputBank)
			{
				bool newState = false;
				bool newState2 = false;
				if (this.skillDriverEvaluation.dominantSkillDriver)
				{
					AISkillDriver.AimType aimType = this.skillDriverEvaluation.dominantSkillDriver.aimType;
					if (aimType != AISkillDriver.AimType.None)
					{
						BaseAI.Target target = null;
						switch (aimType)
						{
						case AISkillDriver.AimType.AtMoveTarget:
							target = this.skillDriverEvaluation.target;
							break;
						case AISkillDriver.AimType.AtCurrentEnemy:
							target = this.currentEnemy;
							break;
						case AISkillDriver.AimType.AtCurrentLeader:
							target = this.leader;
							break;
						}
						if (target != null)
						{
							Vector3 a;
							if (target.GetBullseyePosition(out a))
							{
								this.desiredAimDirection = (a - this.bodyInputBank.aimOrigin).normalized;
							}
							newState = (this.skillDriverEvaluation.dominantSkillDriver.shouldFireEquipment && !this.bodyInputBank.activateEquipment.down);
						}
						else if (this.bodyInputBank.moveVector != Vector3.zero)
						{
							this.desiredAimDirection = this.bodyInputBank.moveVector;
						}
					}
					newState2 = this.skillDriverEvaluation.dominantSkillDriver.shouldSprint;
				}
				this.bodyInputBank.activateEquipment.PushState(newState);
				this.bodyInputBank.sprint.PushState(newState2);
				Vector3 aimDirection = this.bodyInputBank.aimDirection;
				Vector3 eulerAngles = Util.QuaternionSafeLookRotation(this.desiredAimDirection).eulerAngles;
				Vector3 eulerAngles2 = Util.QuaternionSafeLookRotation(aimDirection).eulerAngles;
				float fixedDeltaTime = Time.fixedDeltaTime;
				float x = Mathf.SmoothDampAngle(eulerAngles2.x, eulerAngles.x, ref this.aimVelocity.x, this.aimVectorDampTime, this.aimVectorMaxSpeed, fixedDeltaTime);
				float y = Mathf.SmoothDampAngle(eulerAngles2.y, eulerAngles.y, ref this.aimVelocity.y, this.aimVectorDampTime, this.aimVectorMaxSpeed, fixedDeltaTime);
				float z = Mathf.SmoothDampAngle(eulerAngles2.z, eulerAngles.z, ref this.aimVelocity.z, this.aimVectorDampTime, this.aimVectorMaxSpeed, fixedDeltaTime);
				this.bodyInputBank.aimDirection = Quaternion.Euler(x, y, z) * Vector3.forward;
			}
			this.debugEnemyHurtBox = this.currentEnemy.bestHurtBox;
		}

		// Token: 0x0600211D RID: 8477 RVA: 0x0008F014 File Offset: 0x0008D214
		private void PickCurrentNodeGraph()
		{
			bool flag = true;
			if (this.bodyCharacterMotor)
			{
				flag = this.bodyCharacterMotor.isFlying;
			}
			this.SetCurrentBodyNodeGraph(SceneInfo.instance.GetNodeGraph(flag ? MapNodeGroup.GraphType.Air : MapNodeGroup.GraphType.Ground));
		}

		// Token: 0x0600211E RID: 8478 RVA: 0x0008F054 File Offset: 0x0008D254
		private void BeginSkillDriver(BaseAI.SkillDriverEvaluation newSkillDriverEvaluation)
		{
			this.skillDriverEvaluation = newSkillDriverEvaluation;
			if (this.skillDriverEvaluation.dominantSkillDriver && this.skillDriverEvaluation.dominantSkillDriver.driverUpdateTimerOverride >= 0f)
			{
				this.skillDriverUpdateTimer = this.skillDriverEvaluation.dominantSkillDriver.driverUpdateTimerOverride;
				return;
			}
			this.skillDriverUpdateTimer = UnityEngine.Random.Range(0.16666667f, 0.2f);
		}

		// Token: 0x0600211F RID: 8479 RVA: 0x0008F0C0 File Offset: 0x0008D2C0
		public virtual void OnBodyStart(CharacterBody newBody)
		{
			this.body = newBody;
			this.bodyTransform = newBody.transform;
			this.bodyCharacterDirection = newBody.GetComponent<CharacterDirection>();
			this.bodyCharacterMotor = newBody.GetComponent<CharacterMotor>();
			this.bodyInputBank = newBody.GetComponent<InputBankTest>();
			this.bodyHealthComponent = newBody.GetComponent<HealthComponent>();
			this.bodySkillLocator = newBody.GetComponent<SkillLocator>();
			this.localNavigator.SetBody(newBody);
			this.PickCurrentNodeGraph();
			if (this.stateMachine && Util.HasEffectiveAuthority(this.networkIdentity))
			{
				this.stateMachine.enabled = true;
				this.stateMachine.SetNextState(EntityState.Instantiate(this.scanState));
			}
			base.enabled = true;
			if (this.bodyInputBank)
			{
				this.desiredAimDirection = this.bodyInputBank.aimDirection;
			}
			Action<CharacterBody> action = this.onBodyDiscovered;
			if (action == null)
			{
				return;
			}
			action(newBody);
		}

		// Token: 0x14000080 RID: 128
		// (add) Token: 0x06002120 RID: 8480 RVA: 0x0008F1A0 File Offset: 0x0008D3A0
		// (remove) Token: 0x06002121 RID: 8481 RVA: 0x0008F1D8 File Offset: 0x0008D3D8
		public event Action<CharacterBody> onBodyDiscovered;

		// Token: 0x14000081 RID: 129
		// (add) Token: 0x06002122 RID: 8482 RVA: 0x0008F210 File Offset: 0x0008D410
		// (remove) Token: 0x06002123 RID: 8483 RVA: 0x0008F248 File Offset: 0x0008D448
		public event Action<CharacterBody> onBodyLost;

		// Token: 0x06002124 RID: 8484 RVA: 0x0008F27D File Offset: 0x0008D47D
		public virtual void OnBodyDeath(CharacterBody characterBody)
		{
			this.OnBodyLost(characterBody);
		}

		// Token: 0x06002125 RID: 8485 RVA: 0x0008F27D File Offset: 0x0008D47D
		public virtual void OnBodyDestroyed(CharacterBody characterBody)
		{
			this.OnBodyLost(characterBody);
		}

		// Token: 0x06002126 RID: 8486 RVA: 0x0008F288 File Offset: 0x0008D488
		public virtual void OnBodyLost(CharacterBody characterBody)
		{
			if (this.body != null)
			{
				base.enabled = false;
				this.body = null;
				this.bodyTransform = null;
				this.bodyCharacterDirection = null;
				this.bodyCharacterMotor = null;
				this.bodyInputBank = null;
				this.bodyHealthComponent = null;
				this.bodySkillLocator = null;
				this.localNavigator.SetBody(null);
				if (this.stateMachine)
				{
					this.stateMachine.enabled = false;
					this.stateMachine.SetNextState(new Idle());
				}
				this.SetCurrentBodyNodeGraph(null);
				Action<CharacterBody> action = this.onBodyLost;
				if (action == null)
				{
					return;
				}
				action(characterBody);
			}
		}

		// Token: 0x06002127 RID: 8487 RVA: 0x0008F328 File Offset: 0x0008D528
		public virtual void OnBodyDamaged(DamageReport damageReport)
		{
			DamageInfo damageInfo = damageReport.damageInfo;
			if (!damageInfo.attacker)
			{
				return;
			}
			if (!this.body)
			{
				return;
			}
			if ((!this.currentEnemy.gameObject || this.enemyAttention <= 0f) && damageInfo.attacker != this.body.gameObject)
			{
				this.currentEnemy.gameObject = damageInfo.attacker;
				this.enemyAttention = this.enemyAttentionDuration;
			}
		}

		// Token: 0x06002128 RID: 8488 RVA: 0x0008F3AC File Offset: 0x0008D5AC
		public virtual bool HasLOS(Vector3 start, Vector3 end)
		{
			RaycastHit raycastHit;
			return !Physics.Raycast(new Ray
			{
				origin = start,
				direction = end - start
			}, out raycastHit, Vector3.Magnitude(end - start), LayerIndex.world.mask);
		}

		// Token: 0x06002129 RID: 8489 RVA: 0x0008F400 File Offset: 0x0008D600
		public virtual bool HasLOS(Vector3 end)
		{
			if (!this.bodyInputBank)
			{
				return false;
			}
			Vector3 aimOrigin = this.bodyInputBank.aimOrigin;
			RaycastHit raycastHit;
			return !Physics.Raycast(new Ray
			{
				origin = aimOrigin,
				direction = end - aimOrigin
			}, out raycastHit, Vector3.Magnitude(end - aimOrigin), LayerIndex.world.mask);
		}

		// Token: 0x0600212A RID: 8490 RVA: 0x0008F46F File Offset: 0x0008D66F
		private Vector3 GetNavigationStartPos()
		{
			return this.body.footPosition;
		}

		// Token: 0x0600212B RID: 8491 RVA: 0x0008F47C File Offset: 0x0008D67C
		private NodeGraph.PathRequest GeneratePathRequest(Vector3 endPos)
		{
			return new NodeGraph.PathRequest
			{
				startPos = this.GetNavigationStartPos(),
				endPos = endPos,
				maxJumpHeight = this.body.maxJumpHeight,
				maxSpeed = this.body.moveSpeed,
				hullClassification = this.body.hullClassification,
				path = new Path(this.GetNodeGraph())
			};
		}

		// Token: 0x0600212C RID: 8492 RVA: 0x0008F4E8 File Offset: 0x0008D6E8
		private void SetCurrentBodyNodeGraph(NodeGraph nodeGraph)
		{
			if (this.currentBodyNodegraph == nodeGraph)
			{
				return;
			}
			this.currentBodyNodegraph = nodeGraph;
			if (this.lastRequestedPathStart != null && this.lastRequestedPathEnd != null && this.currentBodyNodegraph)
			{
				this.RefreshPath(this.lastRequestedPathStart.Value, this.lastRequestedPathEnd.Value);
			}
		}

		// Token: 0x0600212D RID: 8493 RVA: 0x0008F549 File Offset: 0x0008D749
		public NodeGraph GetNodeGraph()
		{
			return this.currentBodyNodegraph;
		}

		// Token: 0x0600212E RID: 8494 RVA: 0x0008F551 File Offset: 0x0008D751
		public NodeGraph GetDesiredSpawnNodeGraph()
		{
			return SceneInfo.instance.GetNodeGraph(this.desiredSpawnNodeGraphType);
		}

		// Token: 0x0600212F RID: 8495 RVA: 0x0008F564 File Offset: 0x0008D764
		public void RefreshPath(Vector3 startVec, Vector3 endVec)
		{
			this.lastRequestedPathStart = new Vector3?(startVec);
			this.lastRequestedPathEnd = new Vector3?(endVec);
			NodeGraph.PathRequest pathRequest = this.GeneratePathRequest(endVec);
			NodeGraph nodeGraph = this.GetNodeGraph();
			if (nodeGraph)
			{
				this.pendingPath = nodeGraph.ComputePath(pathRequest);
			}
		}

		// Token: 0x06002130 RID: 8496 RVA: 0x0008F5AD File Offset: 0x0008D7AD
		public void DebugDrawPath(Color color, float duration)
		{
			this.pathFollower.DebugDrawPath(color, duration);
		}

		// Token: 0x06002131 RID: 8497 RVA: 0x0008F5BC File Offset: 0x0008D7BC
		private static bool CheckLoS(Vector3 start, Vector3 end)
		{
			Vector3 direction = end - start;
			RaycastHit raycastHit;
			return !Physics.Raycast(start, direction, out raycastHit, direction.magnitude, LayerIndex.world.mask, QueryTriggerInteraction.Ignore);
		}

		// Token: 0x06002132 RID: 8498 RVA: 0x0008F5F8 File Offset: 0x0008D7F8
		public HurtBox GetBestHurtBox(GameObject target)
		{
			CharacterBody component = target.GetComponent<CharacterBody>();
			HurtBoxGroup hurtBoxGroup = (component != null) ? component.hurtBoxGroup : null;
			if (hurtBoxGroup && hurtBoxGroup.bullseyeCount > 1 && this.bodyInputBank)
			{
				Vector3 aimOrigin = this.bodyInputBank.aimOrigin;
				HurtBox hurtBox = null;
				float num = float.PositiveInfinity;
				foreach (HurtBox hurtBox2 in hurtBoxGroup.hurtBoxes)
				{
					if (hurtBox2.isBullseye)
					{
						Vector3 position = hurtBox2.transform.position;
						if (BaseAI.CheckLoS(aimOrigin, hurtBox2.transform.position))
						{
							float sqrMagnitude = (position - aimOrigin).sqrMagnitude;
							if (sqrMagnitude < num)
							{
								num = sqrMagnitude;
								hurtBox = hurtBox2;
							}
						}
					}
				}
				if (hurtBox)
				{
					return hurtBox;
				}
			}
			return Util.FindBodyMainHurtBox(target);
		}

		// Token: 0x06002133 RID: 8499 RVA: 0x0008F6D0 File Offset: 0x0008D8D0
		public void ForceAcquireNearestEnemyIfNoCurrentEnemy()
		{
			if (this.currentEnemy.gameObject)
			{
				return;
			}
			if (!this.body)
			{
				Debug.LogErrorFormat("BaseAI.ForceAcquireNearestEnemyIfNoCurrentEnemy for CharacterMaster '{0}' failed: AI has no body to search from.", new object[]
				{
					base.gameObject.name
				});
				return;
			}
			HurtBox hurtBox = this.FindEnemyHurtBox(float.PositiveInfinity, true, false);
			if (hurtBox && hurtBox.healthComponent)
			{
				this.currentEnemy.gameObject = hurtBox.healthComponent.gameObject;
				this.currentEnemy.bestHurtBox = hurtBox;
			}
		}

		// Token: 0x06002134 RID: 8500 RVA: 0x0008F764 File Offset: 0x0008D964
		private HurtBox FindEnemyHurtBox(float maxDistance, bool full360Vision, bool filterByLoS)
		{
			if (!this.body)
			{
				return null;
			}
			this.enemySearch.viewer = this.body;
			this.enemySearch.teamMaskFilter = TeamMask.allButNeutral;
			this.enemySearch.teamMaskFilter.RemoveTeam(this.master.teamIndex);
			this.enemySearch.sortMode = BullseyeSearch.SortMode.Distance;
			this.enemySearch.minDistanceFilter = 0f;
			this.enemySearch.maxDistanceFilter = maxDistance;
			this.enemySearch.searchOrigin = this.bodyInputBank.aimOrigin;
			this.enemySearch.searchDirection = this.bodyInputBank.aimDirection;
			this.enemySearch.maxAngleFilter = (full360Vision ? 180f : 90f);
			this.enemySearch.filterByLoS = filterByLoS;
			this.enemySearch.RefreshCandidates();
			return this.enemySearch.GetResults().FirstOrDefault<HurtBox>();
		}

		// Token: 0x06002135 RID: 8501 RVA: 0x0008F854 File Offset: 0x0008DA54
		public bool GameObjectPassesSkillDriverFilters(BaseAI.Target target, AISkillDriver skillDriver, out float separationSqrMagnitude)
		{
			separationSqrMagnitude = 0f;
			if (!target.gameObject)
			{
				return false;
			}
			float num = 1f;
			if (target.healthComponent)
			{
				num = target.healthComponent.combinedHealthFraction;
			}
			if (num < skillDriver.minTargetHealthFraction || num > skillDriver.maxTargetHealthFraction)
			{
				return false;
			}
			float num2 = 0f;
			if (this.body)
			{
				num2 = this.body.radius;
			}
			float num3 = 0f;
			if (target.characterBody)
			{
				num3 = target.characterBody.radius;
			}
			Vector3 b = this.bodyInputBank ? this.bodyInputBank.aimOrigin : this.bodyTransform.position;
			Vector3 a;
			target.GetBullseyePosition(out a);
			float sqrMagnitude = (a - b).sqrMagnitude;
			separationSqrMagnitude = sqrMagnitude - num3 * num3 - num2 * num2;
			return separationSqrMagnitude >= skillDriver.minDistanceSqr && separationSqrMagnitude <= skillDriver.maxDistanceSqr && (!skillDriver.selectionRequiresTargetLoS || target.hasLoS);
		}

		// Token: 0x06002136 RID: 8502 RVA: 0x0008F95F File Offset: 0x0008DB5F
		private void UpdateTargets()
		{
			this.currentEnemy.Update();
			this.leader.Update();
		}

		// Token: 0x06002137 RID: 8503 RVA: 0x0008F978 File Offset: 0x0008DB78
		public BaseAI.SkillDriverEvaluation EvaluateSkillDrivers()
		{
			this.UpdateTargets();
			BaseAI.SkillDriverEvaluation result = default(BaseAI.SkillDriverEvaluation);
			float num = 1f;
			if (this.bodyHealthComponent)
			{
				num = this.bodyHealthComponent.combinedHealthFraction;
			}
			float positiveInfinity = float.PositiveInfinity;
			if (this.bodySkillLocator)
			{
				for (int i = 0; i < this.skillDrivers.Length; i++)
				{
					AISkillDriver aiskillDriver = this.skillDrivers[i];
					if (!aiskillDriver.noRepeat || !(this.skillDriverEvaluation.dominantSkillDriver == aiskillDriver))
					{
						BaseAI.Target target = null;
						if (!aiskillDriver.requireEquipmentReady || this.body.equipmentSlot.stock > 0)
						{
							if (aiskillDriver.skillSlot != SkillSlot.None)
							{
								GenericSkill skill = this.bodySkillLocator.GetSkill(aiskillDriver.skillSlot);
								if ((aiskillDriver.requireSkillReady && (!skill || !skill.IsReady())) || (aiskillDriver.requiredSkill && (!skill || !(skill.skillDef == aiskillDriver.requiredSkill))))
								{
									goto IL_327;
								}
							}
							if (aiskillDriver.minUserHealthFraction <= num && aiskillDriver.maxUserHealthFraction >= num)
							{
								switch (aiskillDriver.moveTargetType)
								{
								case AISkillDriver.TargetType.CurrentEnemy:
									if (this.GameObjectPassesSkillDriverFilters(this.currentEnemy, aiskillDriver, out positiveInfinity))
									{
										target = this.currentEnemy;
									}
									break;
								case AISkillDriver.TargetType.NearestFriendlyInSkillRange:
									if (this.bodyInputBank)
									{
										this.buddySearch.teamMaskFilter = TeamMask.none;
										this.buddySearch.teamMaskFilter.AddTeam(this.master.teamIndex);
										this.buddySearch.sortMode = BullseyeSearch.SortMode.Distance;
										this.buddySearch.minDistanceFilter = aiskillDriver.minDistanceSqr;
										this.buddySearch.maxDistanceFilter = aiskillDriver.maxDistance;
										this.buddySearch.searchOrigin = this.bodyInputBank.aimOrigin;
										this.buddySearch.searchDirection = this.bodyInputBank.aimDirection;
										this.buddySearch.maxAngleFilter = 180f;
										this.buddySearch.filterByLoS = aiskillDriver.activationRequiresTargetLoS;
										this.buddySearch.RefreshCandidates();
										if (this.body)
										{
											this.buddySearch.FilterOutGameObject(this.body.gameObject);
										}
										this.buddySearch.FilterCandidatesByHealthFraction(aiskillDriver.minTargetHealthFraction, aiskillDriver.maxTargetHealthFraction);
										HurtBox hurtBox = this.buddySearch.GetResults().FirstOrDefault<HurtBox>();
										if (hurtBox && hurtBox.healthComponent)
										{
											this.buddy.gameObject = hurtBox.healthComponent.gameObject;
											this.buddy.bestHurtBox = hurtBox;
										}
										if (this.GameObjectPassesSkillDriverFilters(this.buddy, aiskillDriver, out positiveInfinity))
										{
											target = this.buddy;
										}
									}
									break;
								case AISkillDriver.TargetType.CurrentLeader:
									if (this.GameObjectPassesSkillDriverFilters(this.leader, aiskillDriver, out positiveInfinity))
									{
										target = this.leader;
									}
									break;
								case AISkillDriver.TargetType.Custom:
									if (this.GameObjectPassesSkillDriverFilters(this.customTarget, aiskillDriver, out positiveInfinity))
									{
										target = this.customTarget;
									}
									break;
								}
								if (target != null)
								{
									result.dominantSkillDriver = aiskillDriver;
									result.target = target;
									result.separationSqrMagnitude = positiveInfinity;
									break;
								}
							}
						}
					}
					IL_327:;
				}
			}
			return result;
		}

		// Token: 0x04001E5B RID: 7771
		[Tooltip("If true, this character can spot enemies behind itself.")]
		public bool fullVision;

		// Token: 0x04001E5C RID: 7772
		[Tooltip("The minimum distance this character will try to maintain from its enemy, in meters, backing up if closer than this range.")]
		public float minDistanceFromEnemy;

		// Token: 0x04001E5D RID: 7773
		public float enemyAttentionDuration = 5f;

		// Token: 0x04001E5E RID: 7774
		public BaseAI.NavigationType navigationType;

		// Token: 0x04001E5F RID: 7775
		public MapNodeGroup.GraphType desiredSpawnNodeGraphType;

		// Token: 0x04001E60 RID: 7776
		[Tooltip("The state machine to run while the body exists.")]
		public EntityStateMachine stateMachine;

		// Token: 0x04001E61 RID: 7777
		public SerializableEntityStateType scanState;

		// Token: 0x04001E62 RID: 7778
		public bool isHealer;

		// Token: 0x04001E63 RID: 7779
		public float enemyAttention;

		// Token: 0x04001E64 RID: 7780
		public float aimVectorDampTime = 0.2f;

		// Token: 0x04001E65 RID: 7781
		public float aimVectorMaxSpeed = 6f;

		// Token: 0x04001E66 RID: 7782
		[HideInInspector]
		public Vector3 desiredAimDirection = Vector3.forward;

		// Token: 0x04001E67 RID: 7783
		private Vector3 aimVelocity = Vector3.zero;

		// Token: 0x04001E68 RID: 7784
		private float targetRefreshTimer;

		// Token: 0x04001E69 RID: 7785
		private const float targetRefreshDuration = 0.5f;

		// Token: 0x04001E6A RID: 7786
		public PathFollower pathFollower = new PathFollower();

		// Token: 0x04001E6B RID: 7787
		public LocalNavigator localNavigator = new LocalNavigator();

		// Token: 0x04001E6C RID: 7788
		protected PathTask pendingPath;

		// Token: 0x04001E6D RID: 7789
		public NavMeshPath navMeshPath;

		// Token: 0x04001E6E RID: 7790
		public bool drawAIPath;

		// Token: 0x04001E6F RID: 7791
		public string selectedSkilldriverName;

		// Token: 0x04001E70 RID: 7792
		private const float maxVisionDistance = float.PositiveInfinity;

		// Token: 0x04001E71 RID: 7793
		public HurtBox debugEnemyHurtBox;

		// Token: 0x04001E74 RID: 7796
		private NodeGraph currentBodyNodegraph;

		// Token: 0x04001E75 RID: 7797
		private Vector3? lastRequestedPathStart;

		// Token: 0x04001E76 RID: 7798
		private Vector3? lastRequestedPathEnd;

		// Token: 0x04001E77 RID: 7799
		public BaseAI.Target currentEnemy;

		// Token: 0x04001E78 RID: 7800
		public BaseAI.Target leader;

		// Token: 0x04001E79 RID: 7801
		private BaseAI.Target buddy;

		// Token: 0x04001E7A RID: 7802
		public BaseAI.Target customTarget;

		// Token: 0x04001E7B RID: 7803
		private BullseyeSearch enemySearch = new BullseyeSearch();

		// Token: 0x04001E7C RID: 7804
		private BullseyeSearch buddySearch = new BullseyeSearch();

		// Token: 0x04001E7D RID: 7805
		private float skillDriverUpdateTimer;

		// Token: 0x04001E7E RID: 7806
		private const float skillDriverMinUpdateInterval = 0.16666667f;

		// Token: 0x04001E7F RID: 7807
		private const float skillDriverMaxUpdateInterval = 0.2f;

		// Token: 0x04001E80 RID: 7808
		public BaseAI.SkillDriverEvaluation skillDriverEvaluation;

		// Token: 0x0200056E RID: 1390
		public enum NavigationType
		{
			// Token: 0x04001E82 RID: 7810
			Nodegraph,
			// Token: 0x04001E83 RID: 7811
			NavMesh
		}

		// Token: 0x0200056F RID: 1391
		[Serializable]
		public class Target
		{
			// Token: 0x06002139 RID: 8505 RVA: 0x0008FD36 File Offset: 0x0008DF36
			public Target([NotNull] BaseAI owner)
			{
				this.owner = owner;
			}

			// Token: 0x17000382 RID: 898
			// (get) Token: 0x0600213A RID: 8506 RVA: 0x0008FD4C File Offset: 0x0008DF4C
			// (set) Token: 0x0600213B RID: 8507 RVA: 0x0008FD54 File Offset: 0x0008DF54
			public GameObject gameObject
			{
				get
				{
					return this._gameObject;
				}
				set
				{
					if (value == this._gameObject)
					{
						return;
					}
					this._gameObject = value;
					GameObject gameObject = this.gameObject;
					this.characterBody = ((gameObject != null) ? gameObject.GetComponent<CharacterBody>() : null);
					CharacterBody characterBody = this.characterBody;
					this.healthComponent = ((characterBody != null) ? characterBody.healthComponent : null);
					CharacterBody characterBody2 = this.characterBody;
					this.hurtBoxGroup = ((characterBody2 != null) ? characterBody2.hurtBoxGroup : null);
					this.bullseyeCount = (this.hurtBoxGroup ? this.hurtBoxGroup.bullseyeCount : 0);
					this.mainHurtBox = (this.hurtBoxGroup ? this.hurtBoxGroup.mainHurtBox : null);
					this.bestHurtBox = this.mainHurtBox;
					this.hasLoS = false;
					this.unset = !this._gameObject;
				}
			}

			// Token: 0x0600213C RID: 8508 RVA: 0x0008FE24 File Offset: 0x0008E024
			public void Update()
			{
				if (!this.gameObject)
				{
					return;
				}
				this.hasLoS = (this.bestHurtBox && this.owner.HasLOS(this.bestHurtBox.transform.position));
				if (this.bullseyeCount > 1 && !this.hasLoS)
				{
					this.bestHurtBox = this.GetBestHurtBox(out this.hasLoS);
				}
			}

			// Token: 0x0600213D RID: 8509 RVA: 0x0008FE94 File Offset: 0x0008E094
			public bool GetBullseyePosition(out Vector3 position)
			{
				if (!this.bestHurtBox)
				{
					position = (this.gameObject ? this.gameObject.transform.position : Vector3.zero);
					return false;
				}
				position = this.bestHurtBox.transform.position;
				return true;
			}

			// Token: 0x0600213E RID: 8510 RVA: 0x0008FEF4 File Offset: 0x0008E0F4
			private HurtBox GetBestHurtBox(out bool hadLoS)
			{
				if (this.owner.bodyInputBank)
				{
					Vector3 aimOrigin = this.owner.bodyInputBank.aimOrigin;
					HurtBox hurtBox = null;
					float num = float.PositiveInfinity;
					foreach (HurtBox hurtBox2 in this.hurtBoxGroup.hurtBoxes)
					{
						if (hurtBox2.isBullseye)
						{
							Vector3 position = hurtBox2.transform.position;
							if (BaseAI.CheckLoS(aimOrigin, hurtBox2.transform.position))
							{
								float sqrMagnitude = (position - aimOrigin).sqrMagnitude;
								if (sqrMagnitude < num)
								{
									num = sqrMagnitude;
									hurtBox = hurtBox2;
								}
							}
						}
					}
					if (hurtBox)
					{
						hadLoS = true;
						return hurtBox;
					}
				}
				hadLoS = false;
				return this.mainHurtBox;
			}

			// Token: 0x17000383 RID: 899
			// (get) Token: 0x0600213F RID: 8511 RVA: 0x0008FFB2 File Offset: 0x0008E1B2
			// (set) Token: 0x06002140 RID: 8512 RVA: 0x0008FFBA File Offset: 0x0008E1BA
			public CharacterBody characterBody { get; private set; }

			// Token: 0x17000384 RID: 900
			// (get) Token: 0x06002141 RID: 8513 RVA: 0x0008FFC3 File Offset: 0x0008E1C3
			// (set) Token: 0x06002142 RID: 8514 RVA: 0x0008FFCB File Offset: 0x0008E1CB
			public HealthComponent healthComponent { get; private set; }

			// Token: 0x06002143 RID: 8515 RVA: 0x0008FFD4 File Offset: 0x0008E1D4
			public void Reset()
			{
				if (this.unset)
				{
					return;
				}
				this._gameObject = null;
				this.characterBody = null;
				this.healthComponent = null;
				this.hurtBoxGroup = null;
				this.bullseyeCount = 0;
				this.mainHurtBox = null;
				this.bestHurtBox = this.mainHurtBox;
				this.hasLoS = false;
				this.unset = true;
			}

			// Token: 0x04001E84 RID: 7812
			private readonly BaseAI owner;

			// Token: 0x04001E85 RID: 7813
			private bool unset = true;

			// Token: 0x04001E86 RID: 7814
			private GameObject _gameObject;

			// Token: 0x04001E89 RID: 7817
			public HurtBox bestHurtBox;

			// Token: 0x04001E8A RID: 7818
			private HurtBoxGroup hurtBoxGroup;

			// Token: 0x04001E8B RID: 7819
			private HurtBox mainHurtBox;

			// Token: 0x04001E8C RID: 7820
			private int bullseyeCount;

			// Token: 0x04001E8D RID: 7821
			public bool hasLoS;
		}

		// Token: 0x02000570 RID: 1392
		public struct SkillDriverEvaluation
		{
			// Token: 0x04001E8E RID: 7822
			public AISkillDriver dominantSkillDriver;

			// Token: 0x04001E8F RID: 7823
			public BaseAI.Target target;

			// Token: 0x04001E90 RID: 7824
			public float separationSqrMagnitude;
		}
	}
}
