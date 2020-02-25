using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using EntityStates;
using EntityStates.Missions.Arena.NullWard;
using RoR2.CharacterAI;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200014E RID: 334
	[RequireComponent(typeof(EntityStateMachine))]
	public class ArenaMissionController : NetworkBehaviour
	{
		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x060005EB RID: 1515 RVA: 0x00018828 File Offset: 0x00016A28
		// (set) Token: 0x060005EC RID: 1516 RVA: 0x00018830 File Offset: 0x00016A30
		public int currentRound { get; private set; }

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x060005ED RID: 1517 RVA: 0x00018839 File Offset: 0x00016A39
		// (set) Token: 0x060005EE RID: 1518 RVA: 0x00018841 File Offset: 0x00016A41
		public int clearedRounds
		{
			get
			{
				return this._clearedRounds;
			}
			private set
			{
				this.Network_clearedRounds = value;
			}
		}

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x060005EF RID: 1519 RVA: 0x0001884A File Offset: 0x00016A4A
		// (set) Token: 0x060005F0 RID: 1520 RVA: 0x00018851 File Offset: 0x00016A51
		public static ArenaMissionController instance { get; private set; }

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x060005F1 RID: 1521 RVA: 0x00018859 File Offset: 0x00016A59
		private float maxCredits
		{
			get
			{
				return (this.baseMonsterCredit + this.creditMultiplierPerRound * (float)(this.totalRoundsMax - 1)) * this.cachedDifficultyCoefficient;
			}
		}

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x060005F2 RID: 1522 RVA: 0x00018879 File Offset: 0x00016A79
		private float creditsThisRound
		{
			get
			{
				return (this.baseMonsterCredit + this.creditMultiplierPerRound * (float)(this.currentRound - 1)) * this.cachedDifficultyCoefficient;
			}
		}

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x060005F3 RID: 1523 RVA: 0x0001889C File Offset: 0x00016A9C
		// (remove) Token: 0x060005F4 RID: 1524 RVA: 0x000188D0 File Offset: 0x00016AD0
		public static event Action onBeatArena;

		// Token: 0x060005F5 RID: 1525 RVA: 0x00018903 File Offset: 0x00016B03
		private void Awake()
		{
			this.mainStateMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Main");
			this.syncActiveMonsterBodies.InitializeBehaviour(this, ArenaMissionController.kListsyncActiveMonsterBodies);
			this.syncActivePickups.InitializeBehaviour(this, ArenaMissionController.kListsyncActivePickups);
		}

		// Token: 0x060005F6 RID: 1526 RVA: 0x0001893D File Offset: 0x00016B3D
		private void OnEnable()
		{
			ArenaMissionController.instance = SingletonHelper.Assign<ArenaMissionController>(ArenaMissionController.instance, this);
			Action action = ArenaMissionController.onInstanceChangedGlobal;
			if (action != null)
			{
				action();
			}
			SceneDirector.onPreGeneratePlayerSpawnPointsServer += this.OnPreGeneratePlayerSpawnPointsServer;
		}

		// Token: 0x060005F7 RID: 1527 RVA: 0x00018970 File Offset: 0x00016B70
		private void OnDisable()
		{
			SceneDirector.onPreGeneratePlayerSpawnPointsServer -= this.OnPreGeneratePlayerSpawnPointsServer;
			ArenaMissionController.instance = SingletonHelper.Unassign<ArenaMissionController>(ArenaMissionController.instance, this);
			Action action = ArenaMissionController.onInstanceChangedGlobal;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x060005F8 RID: 1528 RVA: 0x000189A2 File Offset: 0x00016BA2
		private void OnPreGeneratePlayerSpawnPointsServer(SceneDirector sceneDirector, ref Action generationMethod)
		{
			generationMethod = new Action(this.GeneratePlayerSpawnPointsServer);
		}

		// Token: 0x060005F9 RID: 1529 RVA: 0x000189B4 File Offset: 0x00016BB4
		private void GeneratePlayerSpawnPointsServer()
		{
			if (this.nullWards.Length == 0)
			{
				return;
			}
			Vector3 position = this.nullWards[0].transform.position;
			NodeGraph groundNodes = SceneInfo.instance.groundNodes;
			NodeGraphSpider nodeGraphSpider = new NodeGraphSpider(SceneInfo.instance.groundNodes, HullMask.Human);
			nodeGraphSpider.AddNodeForNextStep(groundNodes.FindClosestNode(position, HullClassification.Human));
			for (int i = 0; i < 4; i++)
			{
				nodeGraphSpider.PerformStep();
				if (nodeGraphSpider.collectedSteps.Count > 16)
				{
					break;
				}
			}
			for (int j = 0; j < nodeGraphSpider.collectedSteps.Count; j++)
			{
				NodeGraphSpider.StepInfo stepInfo = nodeGraphSpider.collectedSteps[j];
				SpawnPoint.AddSpawnPoint(groundNodes, stepInfo.node, this.rng);
			}
		}

		// Token: 0x060005FA RID: 1530 RVA: 0x00018A68 File Offset: 0x00016C68
		private static bool IsPickupAllowedForMonsters(PickupIndex pickupIndex)
		{
			PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
			if (pickupDef == null)
			{
				return false;
			}
			ItemDef itemDef = ItemCatalog.GetItemDef(pickupDef.itemIndex);
			if (itemDef == null)
			{
				return false;
			}
			for (int i = 0; i < ArenaMissionController.forbiddenTags.Length; i++)
			{
				if (itemDef.ContainsTag(ArenaMissionController.forbiddenTags[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060005FB RID: 1531 RVA: 0x00018AB8 File Offset: 0x00016CB8
		[Server]
		public override void OnStartServer()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ArenaMissionController::OnStartServer()' called on client");
				return;
			}
			base.OnStartServer();
			this.cachedDifficultyCoefficient = Run.instance.difficultyCoefficient;
			this.rng = new Xoroshiro128Plus((ulong)Run.instance.stageRng.nextUint);
			this.InitCombatDirectors();
			Util.ShuffleArray<GameObject>(this.nullWards, this.rng);
			this.ReadyNextNullWard();
			this.availableMonsterCards = Util.CreateReasonableDirectorCardSpawnList(this.baseMonsterCredit * this.cachedDifficultyCoefficient, this.maximumNumberToSpawnBeforeSkipping, this.minimumNumberToSpawnPerMonsterType);
			this.availableTier1DropList = Run.instance.availableTier1DropList.Where(new Func<PickupIndex, bool>(ArenaMissionController.IsPickupAllowedForMonsters)).ToList<PickupIndex>();
			this.availableTier2DropList = Run.instance.availableTier2DropList.Where(new Func<PickupIndex, bool>(ArenaMissionController.IsPickupAllowedForMonsters)).ToList<PickupIndex>();
			this.availableTier3DropList = Run.instance.availableTier3DropList.Where(new Func<PickupIndex, bool>(ArenaMissionController.IsPickupAllowedForMonsters)).ToList<PickupIndex>();
			if (this.availableMonsterCards.Count == 0)
			{
				Debug.Log("No reasonable monsters could be found.");
				return;
			}
		}

		// Token: 0x060005FC RID: 1532 RVA: 0x00018BD6 File Offset: 0x00016DD6
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.FixedUpdateServer();
			}
		}

		// Token: 0x060005FD RID: 1533 RVA: 0x00018BE8 File Offset: 0x00016DE8
		[Server]
		private void FixedUpdateServer()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ArenaMissionController::FixedUpdateServer()' called on client");
				return;
			}
			this.degenTimer += Time.fixedDeltaTime;
			if (this.degenTimer > 1f / this.degenTickFrequency && this.currentRound != this.totalRoundsMax)
			{
				this.degenTimer -= 1f / this.degenTickFrequency;
				foreach (TeamComponent teamComponent in TeamComponent.GetTeamMembers(TeamIndex.Player))
				{
					if (!teamComponent.body.HasBuff(BuffIndex.NullSafeZone))
					{
						float damage = this.percentDegenPerSecond / 100f / this.degenTickFrequency * teamComponent.body.healthComponent.combinedHealth;
						teamComponent.body.healthComponent.TakeDamage(new DamageInfo
						{
							damage = damage,
							position = teamComponent.body.corePosition,
							damageType = DamageType.Silent
						});
					}
				}
			}
		}

		// Token: 0x060005FE RID: 1534 RVA: 0x00018D04 File Offset: 0x00016F04
		[Server]
		private void ReadyNextNullWard()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ArenaMissionController::ReadyNextNullWard()' called on client");
				return;
			}
			if (this.currentRound > this.nullWards.Length)
			{
				Debug.LogError("Out of null wards! Aborting.");
				return;
			}
			EntityStateMachine component = this.nullWards[this.currentRound].GetComponent<EntityStateMachine>();
			component.initialStateType = new SerializableEntityStateType(typeof(WardOnAndReady));
			component.SetNextState(new WardOnAndReady());
		}

		// Token: 0x060005FF RID: 1535 RVA: 0x00018D74 File Offset: 0x00016F74
		[Server]
		private void InitCombatDirectors()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ArenaMissionController::InitCombatDirectors()' called on client");
				return;
			}
			for (int i = 0; i < this.combatDirectors.Length; i++)
			{
				CombatDirector combatDirector = this.combatDirectors[i];
				combatDirector.maximumNumberToSpawnBeforeSkipping = this.maximumNumberToSpawnBeforeSkipping;
				combatDirector.onSpawnedServer.AddListener(new UnityAction<GameObject>(this.ModifySpawnedMasters));
				combatDirector.spawnDistanceMultiplier = this.spawnDistanceMultiplier;
				combatDirector.eliteBias = this.eliteBias;
			}
		}

		// Token: 0x06000600 RID: 1536 RVA: 0x00018DEC File Offset: 0x00016FEC
		[Server]
		public void BeginRound()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ArenaMissionController::BeginRound()' called on client");
				return;
			}
			int currentRound = this.currentRound;
			this.currentRound = currentRound + 1;
			switch (this.currentRound)
			{
			case 1:
				this.AddMonsterType();
				break;
			case 2:
				this.AddItemType(ItemTier.Tier1);
				break;
			case 3:
				this.AddMonsterType();
				break;
			case 4:
				this.AddItemType(ItemTier.Tier1);
				break;
			case 5:
				this.AddMonsterType();
				break;
			case 6:
				this.AddItemType(ItemTier.Tier2);
				break;
			case 7:
				this.AddMonsterType();
				break;
			case 8:
				this.AddItemType(ItemTier.Tier2);
				break;
			case 9:
				this.AddItemType(ItemTier.Tier3);
				break;
			}
			UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/Encounters/ArenaRoundEncounter"));
			for (int i = 0; i < this.activeMonsterCards.Count; i++)
			{
				DirectorCard directorCard = this.activeMonsterCards[i];
				float num = this.creditsThisRound / (float)this.activeMonsterCards.Count;
				float creditMultiplier = this.creditMultiplierPerRound * (float)this.currentRound / (float)this.activeMonsterCards.Count;
				if (i > this.combatDirectors.Length)
				{
					Debug.LogError("Trying to activate more combat directors than available. Aborting.");
					return;
				}
				CombatDirector combatDirector = this.combatDirectors[i];
				combatDirector.monsterCredit += num;
				combatDirector.creditMultiplier = creditMultiplier;
				combatDirector.currentSpawnTarget = this.monsterSpawnPosition;
				combatDirector.OverrideCurrentMonsterCard(directorCard);
				combatDirector.monsterSpawnTimer = 0f;
				combatDirector.enabled = true;
				Debug.LogFormat("Enabling director {0} with {1} credits to spawn {2}", new object[]
				{
					i,
					num,
					directorCard.spawnCard.name
				});
			}
		}

		// Token: 0x06000601 RID: 1537 RVA: 0x00018F94 File Offset: 0x00017194
		[Server]
		public void ModifySpawnedMasters(GameObject targetGameObject)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ArenaMissionController::ModifySpawnedMasters(UnityEngine.GameObject)' called on client");
				return;
			}
			ArenaMissionController.<>c__DisplayClass62_0 CS$<>8__locals1 = new ArenaMissionController.<>c__DisplayClass62_0();
			CharacterMaster component = targetGameObject.GetComponent<CharacterMaster>();
			CS$<>8__locals1.ai = component.GetComponent<BaseAI>();
			if (CS$<>8__locals1.ai)
			{
				CS$<>8__locals1.ai.onBodyDiscovered += CS$<>8__locals1.<ModifySpawnedMasters>g__OnBodyDiscovered|0;
			}
			CharacterBody body = component.GetBody();
			if (body)
			{
				foreach (EntityStateMachine entityStateMachine in body.GetComponents<EntityStateMachine>())
				{
					entityStateMachine.initialStateType = entityStateMachine.mainStateType;
				}
			}
			for (int j = 0; j < this.syncActivePickups.Count; j++)
			{
				int count = 0;
				ItemIndex itemIndex = PickupCatalog.GetPickupDef(new PickupIndex(this.syncActivePickups[j])).itemIndex;
				switch (ItemCatalog.GetItemDef(itemIndex).tier)
				{
				case ItemTier.Tier1:
					count = this.stackCountPerTier1;
					break;
				case ItemTier.Tier2:
					count = this.stackCountPerTier2;
					break;
				case ItemTier.Tier3:
					count = this.stackCountPerTier3;
					break;
				}
				component.inventory.GiveItem(itemIndex, count);
			}
		}

		// Token: 0x06000602 RID: 1538 RVA: 0x000190B8 File Offset: 0x000172B8
		[Server]
		public void EndRound()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ArenaMissionController::EndRound()' called on client");
				return;
			}
			int clearedRounds = this.clearedRounds;
			this.clearedRounds = clearedRounds + 1;
			if (this.currentRound < this.totalRoundsMax)
			{
				this.ReadyNextNullWard();
			}
			else
			{
				Action action = ArenaMissionController.onBeatArena;
				if (action != null)
				{
					action();
				}
				this.mainStateMachine.SetNextState(new ArenaMissionController.MissionCompleted());
				Chat.SendBroadcastChat(new Chat.SimpleChatMessage
				{
					baseToken = "ARENA_END"
				});
			}
			for (int i = 0; i < this.combatDirectors.Length; i++)
			{
				CombatDirector combatDirector = this.combatDirectors[i];
				combatDirector.enabled = false;
				combatDirector.monsterCredit = 0f;
			}
			ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Monster);
			for (int j = teamMembers.Count - 1; j >= 0; j--)
			{
				teamMembers[j].body.healthComponent.Suicide(base.gameObject, base.gameObject, DamageType.VoidDeath);
			}
			int participatingPlayerCount = Run.instance.participatingPlayerCount;
			if (participatingPlayerCount != 0 && this.rewardSpawnPosition)
			{
				List<PickupIndex> list = Run.instance.availableTier1DropList;
				if (this.currentRound > 4)
				{
					list = Run.instance.availableTier2DropList;
				}
				if (this.currentRound == this.totalRoundsMax)
				{
					list = Run.instance.availableTier3DropList;
				}
				ItemIndex itemIndex = this.rng.NextElementUniform<PickupIndex>(list).itemIndex;
				int num = participatingPlayerCount;
				float angle = 360f / (float)num;
				Vector3 vector = Quaternion.AngleAxis((float)UnityEngine.Random.Range(0, 360), Vector3.up) * (Vector3.up * 40f + Vector3.forward * 5f);
				Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
				int k = 0;
				while (k < num)
				{
					PickupDropletController.CreatePickupDroplet(new PickupIndex(itemIndex), this.rewardSpawnPosition.transform.position, vector);
					k++;
					vector = rotation * vector;
				}
			}
		}

		// Token: 0x06000603 RID: 1539 RVA: 0x000192B0 File Offset: 0x000174B0
		[Server]
		private void AddMonsterType()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ArenaMissionController::AddMonsterType()' called on client");
				return;
			}
			if (this.availableMonsterCards.Count == 0)
			{
				Debug.Log("Out of monster types! Aborting.");
				return;
			}
			int num = this.availableMonsterCards.EvaluteToChoiceIndex(this.rng.nextNormalizedFloat);
			DirectorCard value = this.availableMonsterCards.choices[num].value;
			this.activeMonsterCards.Add(value);
			SyncList<int> syncList = this.syncActiveMonsterBodies;
			CharacterMaster component = value.spawnCard.prefab.GetComponent<CharacterMaster>();
			int? num2;
			if (component == null)
			{
				num2 = null;
			}
			else
			{
				CharacterBody component2 = component.bodyPrefab.GetComponent<CharacterBody>();
				num2 = ((component2 != null) ? new int?(component2.bodyIndex) : null);
			}
			syncList.Add(num2 ?? -1);
			this.availableMonsterCards.RemoveChoice(num);
			CharacterBody component3 = value.spawnCard.prefab.GetComponent<CharacterMaster>().bodyPrefab.GetComponent<CharacterBody>();
			Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
			{
				baseToken = "ARENA_ADD_MONSTER",
				paramTokens = new string[]
				{
					component3.baseNameToken
				}
			});
		}

		// Token: 0x06000604 RID: 1540 RVA: 0x000193E0 File Offset: 0x000175E0
		[Server]
		private void AddItemType(ItemTier itemTier)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.ArenaMissionController::AddItemType(RoR2.ItemTier)' called on client");
				return;
			}
			List<PickupIndex> list;
			switch (itemTier)
			{
			case ItemTier.Tier1:
				list = this.availableTier1DropList;
				break;
			case ItemTier.Tier2:
				list = this.availableTier2DropList;
				break;
			case ItemTier.Tier3:
				list = this.availableTier3DropList;
				break;
			default:
				return;
			}
			if (list.Count == 0)
			{
				Debug.LogErrorFormat("No items remaining in arena for tier {0}. Aborting.", new object[]
				{
					itemTier
				});
				return;
			}
			PickupIndex pickupIndex = this.rng.NextElementUniform<PickupIndex>(list);
			list.Remove(pickupIndex);
			this.syncActivePickups.Add(pickupIndex.value);
			for (int i = 0; i < this.pickupDisplays.Length; i++)
			{
				PickupDisplay pickupDisplay = this.pickupDisplays[i];
				if (!pickupDisplay.enabled)
				{
					pickupDisplay.enabled = true;
					pickupDisplay.SetPickupIndex(pickupIndex, false);
					break;
				}
			}
			PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
			Chat.SendBroadcastChat(new Chat.PlayerPickupChatMessage
			{
				baseToken = "ARENA_ADD_ITEM",
				pickupToken = pickupDef.nameToken,
				pickupColor = pickupDef.baseColor
			});
		}

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x06000605 RID: 1541 RVA: 0x000194E8 File Offset: 0x000176E8
		// (remove) Token: 0x06000606 RID: 1542 RVA: 0x0001951C File Offset: 0x0001771C
		public static event Action onInstanceChangedGlobal;

		// Token: 0x06000608 RID: 1544 RVA: 0x00019578 File Offset: 0x00017778
		static ArenaMissionController()
		{
			NetworkBehaviour.RegisterSyncListDelegate(typeof(ArenaMissionController), ArenaMissionController.kListsyncActiveMonsterBodies, new NetworkBehaviour.CmdDelegate(ArenaMissionController.InvokeSyncListsyncActiveMonsterBodies));
			ArenaMissionController.kListsyncActivePickups = 1759005299;
			NetworkBehaviour.RegisterSyncListDelegate(typeof(ArenaMissionController), ArenaMissionController.kListsyncActivePickups, new NetworkBehaviour.CmdDelegate(ArenaMissionController.InvokeSyncListsyncActivePickups));
			NetworkCRC.RegisterBehaviour("ArenaMissionController", 0);
		}

		// Token: 0x06000609 RID: 1545 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x0600060A RID: 1546 RVA: 0x00019600 File Offset: 0x00017800
		// (set) Token: 0x0600060B RID: 1547 RVA: 0x00019613 File Offset: 0x00017813
		public int Network_clearedRounds
		{
			get
			{
				return this._clearedRounds;
			}
			[param: In]
			set
			{
				base.SetSyncVar<int>(value, ref this._clearedRounds, 4U);
			}
		}

		// Token: 0x0600060C RID: 1548 RVA: 0x00019627 File Offset: 0x00017827
		protected static void InvokeSyncListsyncActiveMonsterBodies(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("SyncList syncActiveMonsterBodies called on server.");
				return;
			}
			((ArenaMissionController)obj).syncActiveMonsterBodies.HandleMsg(reader);
		}

		// Token: 0x0600060D RID: 1549 RVA: 0x00019650 File Offset: 0x00017850
		protected static void InvokeSyncListsyncActivePickups(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("SyncList syncActivePickups called on server.");
				return;
			}
			((ArenaMissionController)obj).syncActivePickups.HandleMsg(reader);
		}

		// Token: 0x0600060E RID: 1550 RVA: 0x0001967C File Offset: 0x0001787C
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				SyncListInt.WriteInstance(writer, this.syncActiveMonsterBodies);
				SyncListInt.WriteInstance(writer, this.syncActivePickups);
				writer.WritePackedUInt32((uint)this._clearedRounds);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				SyncListInt.WriteInstance(writer, this.syncActiveMonsterBodies);
			}
			if ((base.syncVarDirtyBits & 2U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				SyncListInt.WriteInstance(writer, this.syncActivePickups);
			}
			if ((base.syncVarDirtyBits & 4U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32((uint)this._clearedRounds);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x0600060F RID: 1551 RVA: 0x00019768 File Offset: 0x00017968
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				SyncListInt.ReadReference(reader, this.syncActiveMonsterBodies);
				SyncListInt.ReadReference(reader, this.syncActivePickups);
				this._clearedRounds = (int)reader.ReadPackedUInt32();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				SyncListInt.ReadReference(reader, this.syncActiveMonsterBodies);
			}
			if ((num & 2) != 0)
			{
				SyncListInt.ReadReference(reader, this.syncActivePickups);
			}
			if ((num & 4) != 0)
			{
				this._clearedRounds = (int)reader.ReadPackedUInt32();
			}
		}

		// Token: 0x0400066F RID: 1647
		[Header("Behavior Values")]
		public float baseMonsterCredit;

		// Token: 0x04000670 RID: 1648
		public float creditMultiplierPerRound;

		// Token: 0x04000671 RID: 1649
		public int minimumNumberToSpawnPerMonsterType;

		// Token: 0x04000672 RID: 1650
		public int totalRoundsMax;

		// Token: 0x04000673 RID: 1651
		public int stackCountPerTier1;

		// Token: 0x04000674 RID: 1652
		public int stackCountPerTier2;

		// Token: 0x04000675 RID: 1653
		public int stackCountPerTier3;

		// Token: 0x04000676 RID: 1654
		public int maximumNumberToSpawnBeforeSkipping;

		// Token: 0x04000677 RID: 1655
		public float spawnDistanceMultiplier;

		// Token: 0x04000678 RID: 1656
		public float eliteBias;

		// Token: 0x04000679 RID: 1657
		public float degenTickFrequency;

		// Token: 0x0400067A RID: 1658
		public float percentDegenPerSecond;

		// Token: 0x0400067B RID: 1659
		[Header("Cached Components")]
		public GameObject[] nullWards;

		// Token: 0x0400067C RID: 1660
		public GameObject monsterSpawnPosition;

		// Token: 0x0400067D RID: 1661
		public GameObject rewardSpawnPosition;

		// Token: 0x0400067E RID: 1662
		public CombatDirector[] combatDirectors;

		// Token: 0x0400067F RID: 1663
		public PickupDisplay[] pickupDisplays;

		// Token: 0x04000680 RID: 1664
		public GameObject clearedEffect;

		// Token: 0x04000681 RID: 1665
		public GameObject killEffectPrefab;

		// Token: 0x04000684 RID: 1668
		private EntityStateMachine mainStateMachine;

		// Token: 0x04000685 RID: 1669
		private Xoroshiro128Plus rng;

		// Token: 0x04000686 RID: 1670
		private List<DirectorCard> activeMonsterCards = new List<DirectorCard>();

		// Token: 0x04000687 RID: 1671
		public readonly SyncListInt syncActiveMonsterBodies = new SyncListInt();

		// Token: 0x04000688 RID: 1672
		public readonly SyncListInt syncActivePickups = new SyncListInt();

		// Token: 0x0400068A RID: 1674
		private WeightedSelection<DirectorCard> availableMonsterCards;

		// Token: 0x0400068B RID: 1675
		private List<PickupIndex> availableTier1DropList;

		// Token: 0x0400068C RID: 1676
		private List<PickupIndex> availableTier2DropList;

		// Token: 0x0400068D RID: 1677
		private List<PickupIndex> availableTier3DropList;

		// Token: 0x0400068E RID: 1678
		private float cachedDifficultyCoefficient;

		// Token: 0x0400068F RID: 1679
		[SyncVar]
		private int _clearedRounds;

		// Token: 0x04000690 RID: 1680
		private static readonly ItemTag[] forbiddenTags = new ItemTag[]
		{
			ItemTag.AIBlacklist,
			ItemTag.EquipmentRelated,
			ItemTag.SprintRelated,
			ItemTag.OnKillEffect
		};

		// Token: 0x04000691 RID: 1681
		private float degenTimer;

		// Token: 0x04000693 RID: 1683
		private static int kListsyncActiveMonsterBodies = 1496902198;

		// Token: 0x04000694 RID: 1684
		private static int kListsyncActivePickups;

		// Token: 0x0200014F RID: 335
		public class ArenaMissionBaseState : EntityState
		{
			// Token: 0x170000B9 RID: 185
			// (get) Token: 0x06000610 RID: 1552 RVA: 0x000197F3 File Offset: 0x000179F3
			protected ArenaMissionController arenaMissionController
			{
				get
				{
					return ArenaMissionController.instance;
				}
			}
		}

		// Token: 0x02000150 RID: 336
		public class MissionCompleted : ArenaMissionController.ArenaMissionBaseState
		{
			// Token: 0x06000612 RID: 1554 RVA: 0x00019802 File Offset: 0x00017A02
			public override void OnEnter()
			{
				base.OnEnter();
				base.arenaMissionController.clearedEffect.SetActive(true);
			}
		}
	}
}
