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
	// Token: 0x0200059C RID: 1436
	[RequireComponent(typeof(CharacterMaster))]
	public class BaseAI : MonoBehaviour
	{
		// Token: 0x0600203C RID: 8252 RVA: 0x000974D4 File Offset: 0x000956D4
		private void Awake()
		{
			this.targetRefreshTimer = 0.5f;
			this.master = base.GetComponent<CharacterMaster>();
			this.stateMachine = base.GetComponent<EntityStateMachine>();
			if (this.stateMachine)
			{
				this.stateMachine.SetNextState(EntityState.Instantiate(this.scanState));
			}
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
			this.skillDrivers = base.GetComponents<AISkillDriver>();
			this.currentEnemy = new BaseAI.Target(this);
			this.leader = new BaseAI.Target(this);
			this.buddy = new BaseAI.Target(this);
		}

		// Token: 0x0600203D RID: 8253 RVA: 0x00097564 File Offset: 0x00095764
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

		// Token: 0x0600203E RID: 8254 RVA: 0x000975B0 File Offset: 0x000957B0
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
						this.enemySearch.viewer = this.body;
						this.enemySearch.teamMaskFilter = TeamMask.allButNeutral;
						this.enemySearch.teamMaskFilter.RemoveTeam(this.master.teamIndex);
						this.enemySearch.sortMode = BullseyeSearch.SortMode.Distance;
						this.enemySearch.minDistanceFilter = 0f;
						this.enemySearch.maxDistanceFilter = float.PositiveInfinity;
						this.enemySearch.searchOrigin = this.bodyInputBank.aimOrigin;
						this.enemySearch.searchDirection = this.bodyInputBank.aimDirection;
						this.enemySearch.maxAngleFilter = (this.fullVision ? 180f : 90f);
						this.enemySearch.filterByLoS = true;
						this.enemySearch.RefreshCandidates();
						HurtBox hurtBox = this.enemySearch.GetResults().FirstOrDefault<HurtBox>();
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
					this.skillDriverEvaluation = this.EvaluateSkillDrivers();
					if (this.skillDriverEvaluation.dominantSkillDriver && this.skillDriverEvaluation.dominantSkillDriver.driverUpdateTimerOverride >= 0f)
					{
						this.skillDriverUpdateTimer = this.skillDriverEvaluation.dominantSkillDriver.driverUpdateTimerOverride;
					}
					else
					{
						this.skillDriverUpdateTimer = UnityEngine.Random.Range(0.16666667f, 0.2f);
					}
				}
			}
			if (this.bodyInputBank)
			{
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
						}
						else
						{
							if (this.bodyInputBank.moveVector != Vector3.zero)
							{
								this.desiredAimDirection = this.bodyInputBank.moveVector;
							}
							this.bodyInputBank.sprint.PushState(this.skillDriverEvaluation.dominantSkillDriver.shouldSprint);
						}
					}
				}
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

		// Token: 0x0600203F RID: 8255 RVA: 0x00097A9C File Offset: 0x00095C9C
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
			base.enabled = true;
			if (this.stateMachine && Util.HasEffectiveAuthority(this.networkIdentity))
			{
				this.stateMachine.enabled = true;
				this.stateMachine.SetNextState(EntityState.Instantiate(this.scanState));
			}
			if (this.bodyInputBank)
			{
				this.desiredAimDirection = this.bodyInputBank.aimDirection;
			}
		}

		// Token: 0x06002040 RID: 8256 RVA: 0x00097B65 File Offset: 0x00095D65
		public virtual void OnBodyDeath()
		{
			this.OnBodyLost();
		}

		// Token: 0x06002041 RID: 8257 RVA: 0x00097B65 File Offset: 0x00095D65
		public virtual void OnBodyDestroyed()
		{
			this.OnBodyLost();
		}

		// Token: 0x06002042 RID: 8258 RVA: 0x00097B70 File Offset: 0x00095D70
		public virtual void OnBodyLost()
		{
			if (this.body)
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
					this.stateMachine.SetState(new Idle());
				}
			}
		}

		// Token: 0x06002043 RID: 8259 RVA: 0x00097BF8 File Offset: 0x00095DF8
		public virtual void OnBodyDamaged(DamageInfo damageInfo)
		{
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

		// Token: 0x06002044 RID: 8260 RVA: 0x00097C75 File Offset: 0x00095E75
		private void UpdateTargets()
		{
			this.currentEnemy.Update();
			this.leader.Update();
		}

		// Token: 0x06002045 RID: 8261 RVA: 0x00097C90 File Offset: 0x00095E90
		public virtual bool HasLOS(Vector3 start, Vector3 end)
		{
			RaycastHit raycastHit;
			return !Physics.Raycast(new Ray
			{
				origin = start,
				direction = end - start
			}, out raycastHit, Vector3.Magnitude(end - start), LayerIndex.world.mask);
		}

		// Token: 0x06002046 RID: 8262 RVA: 0x00097CE4 File Offset: 0x00095EE4
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

		// Token: 0x06002047 RID: 8263 RVA: 0x00097D54 File Offset: 0x00095F54
		private NodeGraph.PathRequest GeneratePathRequest(Vector3 endPos)
		{
			Vector3 position = this.bodyTransform.position;
			if (this.bodyCharacterMotor)
			{
				position.y -= this.bodyCharacterMotor.capsuleHeight * 0.5f;
			}
			return new NodeGraph.PathRequest
			{
				startPos = position,
				endPos = endPos,
				maxJumpHeight = this.body.maxJumpHeight,
				maxSpeed = this.body.moveSpeed,
				hullClassification = this.body.hullClassification,
				path = new Path(this.GetNodeGraph())
			};
		}

		// Token: 0x06002048 RID: 8264 RVA: 0x00097DED File Offset: 0x00095FED
		public NodeGraph GetNodeGraph()
		{
			return SceneInfo.instance.GetNodeGraph(this.nodegraphType);
		}

		// Token: 0x06002049 RID: 8265 RVA: 0x00097E00 File Offset: 0x00096000
		public void RefreshPath(Vector3 startVec, Vector3 endVec)
		{
			NodeGraph.PathRequest pathRequest = this.GeneratePathRequest(endVec);
			NodeGraph nodeGraph = this.GetNodeGraph();
			if (nodeGraph)
			{
				this.pendingPath = nodeGraph.ComputePath(pathRequest);
			}
		}

		// Token: 0x0600204A RID: 8266 RVA: 0x00097E31 File Offset: 0x00096031
		public void DebugDrawPath(Color color, float duration)
		{
			this.pathFollower.DebugDrawPath(color, duration);
		}

		// Token: 0x0600204B RID: 8267 RVA: 0x00097E40 File Offset: 0x00096040
		private static bool CheckLoS(Vector3 start, Vector3 end)
		{
			Vector3 direction = end - start;
			RaycastHit raycastHit;
			return !Physics.Raycast(start, direction, out raycastHit, direction.magnitude, LayerIndex.world.mask, QueryTriggerInteraction.Ignore);
		}

		// Token: 0x0600204C RID: 8268 RVA: 0x00097E7C File Offset: 0x0009607C
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

		// Token: 0x0600204D RID: 8269 RVA: 0x00097F54 File Offset: 0x00096154
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

		// Token: 0x0600204E RID: 8270 RVA: 0x00098060 File Offset: 0x00096260
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
						if (aiskillDriver.skillSlot != SkillSlot.None && aiskillDriver.requireSkillReady)
						{
							GenericSkill skill = this.bodySkillLocator.GetSkill(aiskillDriver.skillSlot);
							if (!skill || !skill.CanExecute())
							{
								goto IL_293;
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
					IL_293:;
				}
			}
			return result;
		}

		// Token: 0x04002292 RID: 8850
		protected CharacterMaster master;

		// Token: 0x04002293 RID: 8851
		protected CharacterBody body;

		// Token: 0x04002294 RID: 8852
		protected Transform bodyTransform;

		// Token: 0x04002295 RID: 8853
		protected CharacterDirection bodyCharacterDirection;

		// Token: 0x04002296 RID: 8854
		protected CharacterMotor bodyCharacterMotor;

		// Token: 0x04002297 RID: 8855
		protected InputBankTest bodyInputBank;

		// Token: 0x04002298 RID: 8856
		protected HealthComponent bodyHealthComponent;

		// Token: 0x04002299 RID: 8857
		protected SkillLocator bodySkillLocator;

		// Token: 0x0400229A RID: 8858
		protected NetworkIdentity networkIdentity;

		// Token: 0x0400229B RID: 8859
		protected AISkillDriver[] skillDrivers;

		// Token: 0x0400229C RID: 8860
		[Tooltip("If true, this character can spot enemies behind itself.")]
		public bool fullVision;

		// Token: 0x0400229D RID: 8861
		[Tooltip("The minimum distance this character will try to maintain from its enemy, in meters, backing up if closer than this range.")]
		public float minDistanceFromEnemy;

		// Token: 0x0400229E RID: 8862
		public float enemyAttentionDuration = 5f;

		// Token: 0x0400229F RID: 8863
		public BaseAI.NavigationType navigationType;

		// Token: 0x040022A0 RID: 8864
		public MapNodeGroup.GraphType nodegraphType;

		// Token: 0x040022A1 RID: 8865
		[Tooltip("The state machine to run while the body exists.")]
		public EntityStateMachine stateMachine;

		// Token: 0x040022A2 RID: 8866
		public SerializableEntityStateType scanState;

		// Token: 0x040022A3 RID: 8867
		public bool isHealer;

		// Token: 0x040022A4 RID: 8868
		public float enemyAttention;

		// Token: 0x040022A5 RID: 8869
		public float aimVectorDampTime = 0.2f;

		// Token: 0x040022A6 RID: 8870
		public float aimVectorMaxSpeed = 6f;

		// Token: 0x040022A7 RID: 8871
		[HideInInspector]
		public Vector3 desiredAimDirection = Vector3.forward;

		// Token: 0x040022A8 RID: 8872
		private Vector3 aimVelocity = Vector3.zero;

		// Token: 0x040022A9 RID: 8873
		private float targetRefreshTimer;

		// Token: 0x040022AA RID: 8874
		private const float targetRefreshDuration = 0.5f;

		// Token: 0x040022AB RID: 8875
		public PathFollower pathFollower = new PathFollower();

		// Token: 0x040022AC RID: 8876
		public LocalNavigator localNavigator = new LocalNavigator();

		// Token: 0x040022AD RID: 8877
		protected PathTask pendingPath;

		// Token: 0x040022AE RID: 8878
		public NavMeshPath navMeshPath;

		// Token: 0x040022AF RID: 8879
		public bool drawAIPath;

		// Token: 0x040022B0 RID: 8880
		public string selectedSkilldriverName;

		// Token: 0x040022B1 RID: 8881
		private const float maxVisionDistance = float.PositiveInfinity;

		// Token: 0x040022B2 RID: 8882
		public HurtBox debugEnemyHurtBox;

		// Token: 0x040022B3 RID: 8883
		private BaseAI.Target currentEnemy;

		// Token: 0x040022B4 RID: 8884
		public BaseAI.Target leader;

		// Token: 0x040022B5 RID: 8885
		private BaseAI.Target buddy;

		// Token: 0x040022B6 RID: 8886
		private BullseyeSearch enemySearch = new BullseyeSearch();

		// Token: 0x040022B7 RID: 8887
		private BullseyeSearch buddySearch = new BullseyeSearch();

		// Token: 0x040022B8 RID: 8888
		private float skillDriverUpdateTimer;

		// Token: 0x040022B9 RID: 8889
		private const float skillDriverMinUpdateInterval = 0.16666667f;

		// Token: 0x040022BA RID: 8890
		private const float skillDriverMaxUpdateInterval = 0.2f;

		// Token: 0x040022BB RID: 8891
		public BaseAI.SkillDriverEvaluation skillDriverEvaluation;

		// Token: 0x0200059D RID: 1437
		public enum NavigationType
		{
			// Token: 0x040022BD RID: 8893
			Nodegraph,
			// Token: 0x040022BE RID: 8894
			NavMesh
		}

		// Token: 0x0200059E RID: 1438
		public class Target
		{
			// Token: 0x06002050 RID: 8272 RVA: 0x0009838A File Offset: 0x0009658A
			public Target([NotNull] BaseAI owner)
			{
				this.owner = owner;
			}

			// Token: 0x170002D5 RID: 725
			// (get) Token: 0x06002051 RID: 8273 RVA: 0x000983A0 File Offset: 0x000965A0
			// (set) Token: 0x06002052 RID: 8274 RVA: 0x000983A8 File Offset: 0x000965A8
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

			// Token: 0x06002053 RID: 8275 RVA: 0x0009847C File Offset: 0x0009667C
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

			// Token: 0x06002054 RID: 8276 RVA: 0x000984EC File Offset: 0x000966EC
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

			// Token: 0x06002055 RID: 8277 RVA: 0x0009854C File Offset: 0x0009674C
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

			// Token: 0x170002D6 RID: 726
			// (get) Token: 0x06002056 RID: 8278 RVA: 0x0009860A File Offset: 0x0009680A
			// (set) Token: 0x06002057 RID: 8279 RVA: 0x00098612 File Offset: 0x00096812
			public CharacterBody characterBody { get; private set; }

			// Token: 0x170002D7 RID: 727
			// (get) Token: 0x06002058 RID: 8280 RVA: 0x0009861B File Offset: 0x0009681B
			// (set) Token: 0x06002059 RID: 8281 RVA: 0x00098623 File Offset: 0x00096823
			public HealthComponent healthComponent { get; private set; }

			// Token: 0x0600205A RID: 8282 RVA: 0x0009862C File Offset: 0x0009682C
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

			// Token: 0x040022BF RID: 8895
			private readonly BaseAI owner;

			// Token: 0x040022C0 RID: 8896
			private bool unset = true;

			// Token: 0x040022C1 RID: 8897
			private GameObject _gameObject;

			// Token: 0x040022C4 RID: 8900
			public HurtBox bestHurtBox;

			// Token: 0x040022C5 RID: 8901
			private HurtBoxGroup hurtBoxGroup;

			// Token: 0x040022C6 RID: 8902
			private HurtBox mainHurtBox;

			// Token: 0x040022C7 RID: 8903
			private int bullseyeCount;

			// Token: 0x040022C8 RID: 8904
			public bool hasLoS;
		}

		// Token: 0x0200059F RID: 1439
		public enum AiSkillTargetType
		{
			// Token: 0x040022CA RID: 8906
			CurrentEnemy,
			// Token: 0x040022CB RID: 8907
			NearestFriendlyInSkillRange,
			// Token: 0x040022CC RID: 8908
			AnyFriendlyInSkillRange
		}

		// Token: 0x020005A0 RID: 1440
		public struct SkillDriverEvaluation
		{
			// Token: 0x040022CD RID: 8909
			public AISkillDriver dominantSkillDriver;

			// Token: 0x040022CE RID: 8910
			public BaseAI.Target target;

			// Token: 0x040022CF RID: 8911
			public float separationSqrMagnitude;
		}
	}
}
