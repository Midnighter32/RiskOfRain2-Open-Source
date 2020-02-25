using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2.ConVar;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x020001AD RID: 429
	public class CombatDirector : MonoBehaviour
	{
		// Token: 0x17000132 RID: 306
		// (get) Token: 0x06000922 RID: 2338 RVA: 0x000276CF File Offset: 0x000258CF
		// (set) Token: 0x06000923 RID: 2339 RVA: 0x000276D7 File Offset: 0x000258D7
		public float monsterSpawnTimer { get; set; }

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x06000924 RID: 2340 RVA: 0x000276E0 File Offset: 0x000258E0
		// (set) Token: 0x06000925 RID: 2341 RVA: 0x000276E8 File Offset: 0x000258E8
		public DirectorCard lastAttemptedMonsterCard { get; set; }

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x06000926 RID: 2342 RVA: 0x000276F1 File Offset: 0x000258F1
		private WeightedSelection<DirectorCard> monsterCards
		{
			get
			{
				return ClassicStageInfo.instance.monsterSelection;
			}
		}

		// Token: 0x06000927 RID: 2343 RVA: 0x00027700 File Offset: 0x00025900
		private void Awake()
		{
			if (NetworkServer.active)
			{
				this.rng = new Xoroshiro128Plus((ulong)Run.instance.stageRng.nextUint);
				this.moneyWaves = new CombatDirector.DirectorMoneyWave[this.moneyWaveIntervals.Length];
				for (int i = 0; i < this.moneyWaveIntervals.Length; i++)
				{
					this.moneyWaves[i] = new CombatDirector.DirectorMoneyWave
					{
						interval = this.rng.RangeFloat(this.moneyWaveIntervals[i].min, this.moneyWaveIntervals[i].max),
						multiplier = this.creditMultiplier
					};
				}
			}
		}

		// Token: 0x06000928 RID: 2344 RVA: 0x000277A4 File Offset: 0x000259A4
		private void OnEnable()
		{
			CombatDirector.instancesList.Add(this);
		}

		// Token: 0x06000929 RID: 2345 RVA: 0x000277B4 File Offset: 0x000259B4
		private void OnDisable()
		{
			CombatDirector.instancesList.Remove(this);
			if (NetworkServer.active && CombatDirector.instancesList.Count > 0)
			{
				float num = 0.4f;
				CombatDirector combatDirector = this.rng.NextElementUniform<CombatDirector>(CombatDirector.instancesList);
				this.monsterCredit *= num;
				combatDirector.monsterCredit += this.monsterCredit;
				Debug.LogFormat("Transfered {0} monster credits from {1} to {2}", new object[]
				{
					this.monsterCredit,
					base.gameObject,
					combatDirector.gameObject
				});
				this.monsterCredit = 0f;
			}
		}

		// Token: 0x0600092A RID: 2346 RVA: 0x00027858 File Offset: 0x00025A58
		private void GenerateAmbush(Vector3 victimPosition)
		{
			NodeGraph groundNodes = SceneInfo.instance.groundNodes;
			NodeGraph.NodeIndex nodeIndex = groundNodes.FindClosestNode(victimPosition, HullClassification.Human);
			NodeGraphSpider nodeGraphSpider = new NodeGraphSpider(groundNodes, HullMask.Human);
			nodeGraphSpider.AddNodeForNextStep(nodeIndex);
			List<NodeGraphSpider.StepInfo> list = new List<NodeGraphSpider.StepInfo>();
			int num = 0;
			List<NodeGraphSpider.StepInfo> collectedSteps = nodeGraphSpider.collectedSteps;
			while (nodeGraphSpider.PerformStep() && num < 8)
			{
				num++;
				for (int i = 0; i < collectedSteps.Count; i++)
				{
					if (CombatDirector.IsAcceptableAmbushSpiderStep(groundNodes, nodeIndex, collectedSteps[i]))
					{
						list.Add(collectedSteps[i]);
					}
				}
				collectedSteps.Clear();
			}
			for (int j = 0; j < list.Count; j++)
			{
				Vector3 position;
				groundNodes.GetNodePosition(list[j].node, out position);
				Resources.Load<SpawnCard>("SpawnCards/scLemurian").DoSpawn(position, Quaternion.identity, null);
			}
		}

		// Token: 0x0600092B RID: 2347 RVA: 0x00027930 File Offset: 0x00025B30
		private static bool IsAcceptableAmbushSpiderStep(NodeGraph nodeGraph, NodeGraph.NodeIndex startNode, NodeGraphSpider.StepInfo stepInfo)
		{
			int num = 0;
			while (stepInfo.previousStep != null)
			{
				if (nodeGraph.TestNodeLineOfSight(startNode, stepInfo.node))
				{
					return false;
				}
				stepInfo = stepInfo.previousStep;
				num++;
				if (num > 2)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600092C RID: 2348 RVA: 0x0002796D File Offset: 0x00025B6D
		public void OverrideCurrentMonsterCard(DirectorCard overrideMonsterCard)
		{
			this.PrepareNewMonsterWave(overrideMonsterCard);
		}

		// Token: 0x0600092D RID: 2349 RVA: 0x00027978 File Offset: 0x00025B78
		public void SetNextSpawnAsBoss()
		{
			WeightedSelection<DirectorCard> weightedSelection = new WeightedSelection<DirectorCard>(8);
			bool flag = !Run.instance.ShouldAllowNonChampionBossSpawn() || this.rng.nextNormalizedFloat > 0.1f;
			int i = 0;
			int count = this.monsterCards.Count;
			while (i < count)
			{
				WeightedSelection<DirectorCard>.ChoiceInfo choice = this.monsterCards.GetChoice(i);
				SpawnCard spawnCard = choice.value.spawnCard;
				bool isChampion = spawnCard.prefab.GetComponent<CharacterMaster>().bodyPrefab.GetComponent<CharacterBody>().isChampion;
				CharacterSpawnCard characterSpawnCard = spawnCard as CharacterSpawnCard;
				bool flag2 = characterSpawnCard != null && characterSpawnCard.forbiddenAsBoss;
				if (isChampion == flag && !flag2 && choice.value.CardIsValid())
				{
					weightedSelection.AddChoice(choice);
				}
				i++;
			}
			if (weightedSelection.Count > 0)
			{
				this.PrepareNewMonsterWave(weightedSelection.Evaluate(this.rng.nextNormalizedFloat));
			}
			this.monsterSpawnTimer = -600f;
		}

		// Token: 0x0600092E RID: 2350 RVA: 0x00027A5C File Offset: 0x00025C5C
		private void PickPlayerAsSpawnTarget()
		{
			ReadOnlyCollection<PlayerCharacterMasterController> instances = PlayerCharacterMasterController.instances;
			List<PlayerCharacterMasterController> list = new List<PlayerCharacterMasterController>();
			foreach (PlayerCharacterMasterController playerCharacterMasterController in instances)
			{
				if (playerCharacterMasterController.master.alive)
				{
					list.Add(playerCharacterMasterController);
				}
			}
			if (list.Count > 0)
			{
				this.currentSpawnTarget = this.rng.NextElementUniform<PlayerCharacterMasterController>(list).master.GetBodyObject();
			}
		}

		// Token: 0x0600092F RID: 2351 RVA: 0x00027AE0 File Offset: 0x00025CE0
		private void Simulate(float deltaTime)
		{
			if (this.targetPlayers)
			{
				this.playerRetargetTimer -= deltaTime;
				if (this.playerRetargetTimer <= 0f)
				{
					this.playerRetargetTimer = this.rng.RangeFloat(1f, 10f);
					this.PickPlayerAsSpawnTarget();
				}
			}
			this.monsterSpawnTimer -= deltaTime;
			if (this.monsterSpawnTimer <= 0f)
			{
				bool flag = false;
				if (TeamComponent.GetTeamMembers(TeamIndex.Monster).Count < 40 || this.ignoreTeamSizeLimit)
				{
					flag = this.AttemptSpawnOnTarget(this.currentSpawnTarget);
				}
				if (flag)
				{
					if (this.shouldSpawnOneWave)
					{
						this.hasStartedWave = true;
					}
					this.monsterSpawnTimer += this.rng.RangeFloat(this.minSeriesSpawnInterval, this.maxSeriesSpawnInterval);
					return;
				}
				this.monsterSpawnTimer += this.rng.RangeFloat(this.minRerollSpawnInterval, this.maxRerollSpawnInterval);
				if (this.resetMonsterCardIfFailed)
				{
					this.currentMonsterCard = null;
				}
				if (this.shouldSpawnOneWave && this.hasStartedWave)
				{
					base.enabled = false;
					return;
				}
			}
		}

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x06000930 RID: 2352 RVA: 0x00027BF8 File Offset: 0x00025DF8
		public static float highestEliteCostMultiplier
		{
			get
			{
				float num = 1f;
				for (int i = 1; i < CombatDirector.eliteTiers.Length; i++)
				{
					if (CombatDirector.eliteTiers[i].isAvailable())
					{
						num = Mathf.Max(num, CombatDirector.eliteTiers[i].costMultiplier);
					}
				}
				return num;
			}
		}

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x06000931 RID: 2353 RVA: 0x00027C44 File Offset: 0x00025E44
		public static float lowestEliteCostMultiplier
		{
			get
			{
				return CombatDirector.eliteTiers[1].costMultiplier;
			}
		}

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x06000932 RID: 2354 RVA: 0x00027C54 File Offset: 0x00025E54
		private int mostExpensiveMonsterCostInDeck
		{
			get
			{
				int num = 0;
				for (int i = 0; i < this.monsterCards.Count; i++)
				{
					DirectorCard value = this.monsterCards.GetChoice(i).value;
					int num2 = value.cost;
					if (!(value.spawnCard as CharacterSpawnCard).noElites)
					{
						num2 = (int)((float)num2 * CombatDirector.highestEliteCostMultiplier);
					}
					num = Mathf.Max(num, num2);
				}
				return num;
			}
		}

		// Token: 0x06000933 RID: 2355 RVA: 0x00027CB8 File Offset: 0x00025EB8
		private unsafe void PrepareNewMonsterWave(DirectorCard monsterCard)
		{
			if (CombatDirector.cvDirectorCombatEnableInternalLogs.value)
			{
				Debug.LogFormat("Preparing monster wave {0}", new object[]
				{
					monsterCard.spawnCard
				});
			}
			this.currentMonsterCard = monsterCard;
			this.currentActiveEliteTier = CombatDirector.eliteTiers[0];
			if (!(this.currentMonsterCard.spawnCard as CharacterSpawnCard).noElites)
			{
				for (int i = 1; i < CombatDirector.eliteTiers.Length; i++)
				{
					CombatDirector.EliteTierDef eliteTierDef = CombatDirector.eliteTiers[i];
					if (!eliteTierDef.isAvailable())
					{
						if (CombatDirector.cvDirectorCombatEnableInternalLogs.value)
						{
							Debug.LogFormat("Elite tier index {0} is unavailable", new object[]
							{
								i
							});
						}
					}
					else
					{
						float num = (float)this.currentMonsterCard.cost * eliteTierDef.costMultiplier * this.eliteBias;
						if (num <= this.monsterCredit)
						{
							this.currentActiveEliteTier = eliteTierDef;
							if (CombatDirector.cvDirectorCombatEnableInternalLogs.value)
							{
								Debug.LogFormat("Found valid elite tier index {0}", new object[]
								{
									i
								});
							}
						}
						else if (CombatDirector.cvDirectorCombatEnableInternalLogs.value)
						{
							Debug.LogFormat("Elite tier index {0} is too expensive ({1}/{2})", new object[]
							{
								i,
								num,
								this.monsterCredit
							});
						}
					}
				}
			}
			else if (CombatDirector.cvDirectorCombatEnableInternalLogs.value)
			{
				Debug.LogFormat("Card {0} cannot be elite. Skipping elite procedure.", new object[]
				{
					this.currentMonsterCard.spawnCard
				});
			}
			this.currentActiveEliteIndex = *this.rng.NextElementUniform<EliteIndex>(this.currentActiveEliteTier.eliteTypes);
			if (CombatDirector.cvDirectorCombatEnableInternalLogs.value)
			{
				Debug.LogFormat("Assigned elite index {0}", new object[]
				{
					this.currentActiveEliteIndex
				});
			}
			this.lastAttemptedMonsterCard = this.currentMonsterCard;
			this.spawnCountInCurrentWave = 0;
		}

		// Token: 0x06000934 RID: 2356 RVA: 0x00027E88 File Offset: 0x00026088
		private bool AttemptSpawnOnTarget(GameObject spawnTarget)
		{
			if (this.currentMonsterCard == null)
			{
				if (CombatDirector.cvDirectorCombatEnableInternalLogs.value)
				{
					Debug.Log("Current monster card is null, pick new one.");
				}
				this.PrepareNewMonsterWave(this.monsterCards.Evaluate(this.rng.nextNormalizedFloat));
			}
			if (!spawnTarget)
			{
				if (CombatDirector.cvDirectorCombatEnableInternalLogs.value)
				{
					Debug.LogFormat("Spawn target {0} is invalid.", new object[]
					{
						spawnTarget
					});
				}
				return false;
			}
			if (this.spawnCountInCurrentWave >= this.maximumNumberToSpawnBeforeSkipping)
			{
				this.spawnCountInCurrentWave = 0;
				if (CombatDirector.cvDirectorCombatEnableInternalLogs.value)
				{
					Debug.LogFormat("Spawn count has hit the max ({0}/{1}). Aborting spawn.", new object[]
					{
						this.spawnCountInCurrentWave,
						this.maximumNumberToSpawnBeforeSkipping
					});
				}
				return false;
			}
			int cost = this.currentMonsterCard.cost;
			int num = this.currentMonsterCard.cost;
			int num2 = this.currentMonsterCard.cost;
			CombatDirector.EliteTierDef eliteTierDef = this.currentActiveEliteTier;
			EliteIndex eliteIndex = this.currentActiveEliteIndex;
			num2 = (int)((float)num * this.currentActiveEliteTier.costMultiplier);
			if ((float)num2 <= this.monsterCredit)
			{
				num = num2;
				eliteTierDef = this.currentActiveEliteTier;
				eliteIndex = this.currentActiveEliteIndex;
			}
			else
			{
				eliteTierDef = CombatDirector.eliteTiers[0];
				eliteIndex = EliteIndex.None;
			}
			if (!this.currentMonsterCard.CardIsValid())
			{
				if (CombatDirector.cvDirectorCombatEnableInternalLogs.value)
				{
					Debug.LogFormat("Spawn card {0} is invalid, aborting spawn.", new object[]
					{
						this.currentMonsterCard.spawnCard
					});
				}
				return false;
			}
			if (this.monsterCredit < (float)num)
			{
				if (CombatDirector.cvDirectorCombatEnableInternalLogs.value)
				{
					Debug.LogFormat("Spawn card {0} is too expensive, aborting spawn.", new object[]
					{
						this.currentMonsterCard.spawnCard
					});
				}
				return false;
			}
			if (this.skipSpawnIfTooCheap && (float)(num2 * this.maximumNumberToSpawnBeforeSkipping) < this.monsterCredit)
			{
				if (CombatDirector.cvDirectorCombatEnableInternalLogs.value)
				{
					Debug.LogFormat("Card {0} seems too cheap ({1}/{2}). Comparing against most expensive possible ({3})", new object[]
					{
						this.currentMonsterCard.spawnCard,
						num * this.maximumNumberToSpawnBeforeSkipping,
						this.monsterCredit,
						this.mostExpensiveMonsterCostInDeck
					});
				}
				if (this.mostExpensiveMonsterCostInDeck > num)
				{
					if (CombatDirector.cvDirectorCombatEnableInternalLogs.value)
					{
						Debug.LogFormat("Spawn card {0} is too cheap, aborting spawn.", new object[]
						{
							this.currentMonsterCard.spawnCard
						});
					}
					return false;
				}
			}
			SpawnCard spawnCard = this.currentMonsterCard.spawnCard;
			DirectorPlacementRule directorPlacementRule = new DirectorPlacementRule
			{
				placementMode = DirectorPlacementRule.PlacementMode.Approximate,
				spawnOnTarget = spawnTarget.transform,
				preventOverhead = this.currentMonsterCard.preventOverhead
			};
			DirectorCore.GetMonsterSpawnDistance(this.currentMonsterCard.spawnDistance, out directorPlacementRule.minDistance, out directorPlacementRule.maxDistance);
			directorPlacementRule.minDistance *= this.spawnDistanceMultiplier;
			directorPlacementRule.maxDistance *= this.spawnDistanceMultiplier;
			DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(spawnCard, directorPlacementRule, this.rng);
			directorSpawnRequest.ignoreTeamMemberLimit = true;
			directorSpawnRequest.teamIndexOverride = new TeamIndex?(TeamIndex.Monster);
			GameObject gameObject = DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
			if (!gameObject)
			{
				Debug.LogFormat("Spawn card {0} failed to spawn. Aborting cost procedures.", new object[]
				{
					spawnCard
				});
				return false;
			}
			this.monsterCredit -= (float)num;
			this.spawnCountInCurrentWave++;
			CharacterMaster component = gameObject.GetComponent<CharacterMaster>();
			GameObject bodyObject = component.GetBodyObject();
			if (this.combatSquad)
			{
				this.combatSquad.AddMember(component);
			}
			float num3 = eliteTierDef.healthBoostCoefficient;
			float damageBoostCoefficient = eliteTierDef.damageBoostCoefficient;
			EliteDef eliteDef = EliteCatalog.GetEliteDef(eliteIndex);
			EquipmentIndex equipmentIndex = (eliteDef != null) ? eliteDef.eliteEquipmentIndex : EquipmentIndex.None;
			if (equipmentIndex != EquipmentIndex.None)
			{
				component.inventory.SetEquipmentIndex(equipmentIndex);
			}
			if (this.combatSquad)
			{
				int livingPlayerCount = Run.instance.livingPlayerCount;
				num3 *= Mathf.Pow((float)livingPlayerCount, 1f);
			}
			component.inventory.GiveItem(ItemIndex.BoostHp, Mathf.RoundToInt((num3 - 1f) * 10f));
			component.inventory.GiveItem(ItemIndex.BoostDamage, Mathf.RoundToInt((damageBoostCoefficient - 1f) * 10f));
			DeathRewards component2 = bodyObject.GetComponent<DeathRewards>();
			if (component2)
			{
				component2.expReward = (uint)((float)num * this.expRewardCoefficient * Run.instance.compensatedDifficultyCoefficient);
				component2.goldReward = (uint)((float)num * this.expRewardCoefficient * 2f * Run.instance.compensatedDifficultyCoefficient);
			}
			if (this.spawnEffectPrefab && NetworkServer.active)
			{
				Vector3 origin = gameObject.transform.position;
				CharacterBody component3 = bodyObject.GetComponent<CharacterBody>();
				if (component3)
				{
					origin = component3.corePosition;
				}
				EffectManager.SpawnEffect(this.spawnEffectPrefab, new EffectData
				{
					origin = origin
				}, true);
			}
			CombatDirector.OnSpawnedServer onSpawnedServer = this.onSpawnedServer;
			if (onSpawnedServer != null)
			{
				onSpawnedServer.Invoke(gameObject);
			}
			return true;
		}

		// Token: 0x06000935 RID: 2357 RVA: 0x00028358 File Offset: 0x00026558
		private void FixedUpdate()
		{
			if (CombatDirector.cvDirectorCombatDisable.value)
			{
				return;
			}
			if (NetworkServer.active && Run.instance)
			{
				float compensatedDifficultyCoefficient = Run.instance.compensatedDifficultyCoefficient;
				for (int i = 0; i < this.moneyWaves.Length; i++)
				{
					float num = this.moneyWaves[i].Update(Time.fixedDeltaTime, compensatedDifficultyCoefficient);
					this.monsterCredit += num;
				}
				this.Simulate(Time.fixedDeltaTime);
			}
		}

		// Token: 0x0400097B RID: 2427
		[Header("Core Director Values")]
		public string customName;

		// Token: 0x0400097C RID: 2428
		public float monsterCredit;

		// Token: 0x0400097D RID: 2429
		public float expRewardCoefficient = 0.2f;

		// Token: 0x0400097E RID: 2430
		public float minSeriesSpawnInterval = 0.1f;

		// Token: 0x0400097F RID: 2431
		public float maxSeriesSpawnInterval = 1f;

		// Token: 0x04000980 RID: 2432
		public float minRerollSpawnInterval = 2.3333333f;

		// Token: 0x04000981 RID: 2433
		public float maxRerollSpawnInterval = 4.3333335f;

		// Token: 0x04000982 RID: 2434
		public RangeFloat[] moneyWaveIntervals;

		// Token: 0x04000983 RID: 2435
		[Tooltip("How much to multiply money wave yield by.")]
		[Header("Optional Behaviors")]
		public float creditMultiplier = 1f;

		// Token: 0x04000984 RID: 2436
		[Tooltip("The coefficient to multiply spawn distances. Used for combat shrines, to keep spawns nearby.")]
		public float spawnDistanceMultiplier = 1f;

		// Token: 0x04000985 RID: 2437
		public bool shouldSpawnOneWave;

		// Token: 0x04000986 RID: 2438
		public bool targetPlayers = true;

		// Token: 0x04000987 RID: 2439
		public bool skipSpawnIfTooCheap = true;

		// Token: 0x04000988 RID: 2440
		public bool resetMonsterCardIfFailed = true;

		// Token: 0x04000989 RID: 2441
		public int maximumNumberToSpawnBeforeSkipping = 6;

		// Token: 0x0400098A RID: 2442
		public float eliteBias = 1f;

		// Token: 0x0400098B RID: 2443
		public CombatDirector.OnSpawnedServer onSpawnedServer;

		// Token: 0x0400098C RID: 2444
		[FormerlySerializedAs("_combatSquad")]
		public CombatSquad combatSquad;

		// Token: 0x0400098D RID: 2445
		[Tooltip("A special effect for when a monster appears will be instantiated at its position. Used for combat shrine.")]
		public GameObject spawnEffectPrefab;

		// Token: 0x0400098E RID: 2446
		public bool ignoreTeamSizeLimit;

		// Token: 0x04000991 RID: 2449
		public static readonly List<CombatDirector> instancesList = new List<CombatDirector>();

		// Token: 0x04000992 RID: 2450
		private bool hasStartedWave;

		// Token: 0x04000993 RID: 2451
		private Xoroshiro128Plus rng;

		// Token: 0x04000994 RID: 2452
		private DirectorCard currentMonsterCard;

		// Token: 0x04000995 RID: 2453
		private CombatDirector.EliteTierDef currentActiveEliteTier;

		// Token: 0x04000996 RID: 2454
		private EliteIndex currentActiveEliteIndex;

		// Token: 0x04000997 RID: 2455
		private int currentMonsterCardCost;

		// Token: 0x04000998 RID: 2456
		public GameObject currentSpawnTarget;

		// Token: 0x04000999 RID: 2457
		private float playerRetargetTimer;

		// Token: 0x0400099A RID: 2458
		private static readonly CombatDirector.EliteTierDef[] eliteTiers = new CombatDirector.EliteTierDef[]
		{
			new CombatDirector.EliteTierDef
			{
				costMultiplier = 1f,
				damageBoostCoefficient = 1f,
				healthBoostCoefficient = 1f,
				eliteTypes = new EliteIndex[]
				{
					EliteIndex.None
				}
			},
			new CombatDirector.EliteTierDef
			{
				costMultiplier = 6f,
				damageBoostCoefficient = 2f,
				healthBoostCoefficient = 4.7f,
				eliteTypes = new EliteIndex[]
				{
					EliteIndex.Fire,
					EliteIndex.Lightning,
					EliteIndex.Ice
				},
				isAvailable = (() => true)
			},
			new CombatDirector.EliteTierDef
			{
				costMultiplier = 36f,
				damageBoostCoefficient = 6f,
				healthBoostCoefficient = 23.5f,
				eliteTypes = new EliteIndex[]
				{
					EliteIndex.Poison,
					EliteIndex.Haunted
				},
				isAvailable = (() => Run.instance.loopClearCount > 0)
			}
		};

		// Token: 0x0400099B RID: 2459
		private int spawnCountInCurrentWave;

		// Token: 0x0400099C RID: 2460
		private static readonly BoolConVar cvDirectorCombatDisable = new BoolConVar("director_combat_disable", ConVarFlags.SenderMustBeServer | ConVarFlags.Cheat, "0", "Disables all combat directors.");

		// Token: 0x0400099D RID: 2461
		private static readonly BoolConVar cvDirectorCombatEnableInternalLogs = new BoolConVar("director_combat_enable_internal_logs", ConVarFlags.None, "0", "Enables all combat directors to print internal logging.");

		// Token: 0x0400099E RID: 2462
		private CombatDirector.DirectorMoneyWave[] moneyWaves;

		// Token: 0x020001AE RID: 430
		[Serializable]
		public class OnSpawnedServer : UnityEvent<GameObject>
		{
		}

		// Token: 0x020001AF RID: 431
		public class EliteTierDef
		{
			// Token: 0x0400099F RID: 2463
			public float costMultiplier;

			// Token: 0x040009A0 RID: 2464
			public float damageBoostCoefficient;

			// Token: 0x040009A1 RID: 2465
			public float healthBoostCoefficient;

			// Token: 0x040009A2 RID: 2466
			public EliteIndex[] eliteTypes;

			// Token: 0x040009A3 RID: 2467
			public Func<bool> isAvailable = () => true;
		}

		// Token: 0x020001B1 RID: 433
		private class DirectorMoneyWave
		{
			// Token: 0x0600093D RID: 2365 RVA: 0x000285D8 File Offset: 0x000267D8
			public float Update(float deltaTime, float difficultyCoefficient)
			{
				this.timer += deltaTime;
				if (this.timer > this.interval)
				{
					float num = 0.5f + (float)Run.instance.participatingPlayerCount * 0.5f;
					this.timer -= this.interval;
					float num2 = 1f;
					float num3 = 0.4f;
					this.accumulatedAward += this.interval * this.multiplier * (num2 + num3 * difficultyCoefficient) * num;
				}
				float num4 = (float)Mathf.FloorToInt(this.accumulatedAward);
				this.accumulatedAward -= num4;
				return num4;
			}

			// Token: 0x040009A6 RID: 2470
			public float interval;

			// Token: 0x040009A7 RID: 2471
			public float timer;

			// Token: 0x040009A8 RID: 2472
			public float multiplier;

			// Token: 0x040009A9 RID: 2473
			private float accumulatedAward;
		}
	}
}
