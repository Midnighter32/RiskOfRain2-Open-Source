using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using RoR2.ConVar;
using RoR2.Networking;
using Unity;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace RoR2
{
	// Token: 0x020003BD RID: 957
	[RequireComponent(typeof(NetworkRuleBook))]
	[DisallowMultipleComponent]
	public class Run : NetworkBehaviour
	{
		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x0600144A RID: 5194 RVA: 0x00062B4D File Offset: 0x00060D4D
		// (set) Token: 0x0600144B RID: 5195 RVA: 0x00062B54 File Offset: 0x00060D54
		public static Run instance { get; private set; }

		// Token: 0x0600144C RID: 5196 RVA: 0x00062B5C File Offset: 0x00060D5C
		private void OnEnable()
		{
			Run.instance = SingletonHelper.Assign<Run>(Run.instance, this);
		}

		// Token: 0x0600144D RID: 5197 RVA: 0x00062B6E File Offset: 0x00060D6E
		private void OnDisable()
		{
			Run.instance = SingletonHelper.Unassign<Run>(Run.instance, this);
		}

		// Token: 0x0600144E RID: 5198 RVA: 0x00062B80 File Offset: 0x00060D80
		protected void Awake()
		{
			this.networkRuleBookComponent = base.GetComponent<NetworkRuleBook>();
		}

		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x0600144F RID: 5199 RVA: 0x00062B8E File Offset: 0x00060D8E
		public RuleBook ruleBook
		{
			get
			{
				return this.networkRuleBookComponent.ruleBook;
			}
		}

		// Token: 0x06001450 RID: 5200 RVA: 0x00062B9C File Offset: 0x00060D9C
		[Server]
		public void SetRuleBook(RuleBook newRuleBook)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Run::SetRuleBook(RoR2.RuleBook)' called on client");
				return;
			}
			this.networkRuleBookComponent.SetRuleBook(newRuleBook);
			this.selectedDifficulty = newRuleBook.FindDifficulty();
			this.NetworkenabledArtifacts = newRuleBook.GenerateArtifactMask();
			this.NetworkavailableItems = newRuleBook.GenerateItemMask();
			this.NetworkavailableEquipment = newRuleBook.GenerateEquipmentMask();
		}

		// Token: 0x06001451 RID: 5201 RVA: 0x00062BFC File Offset: 0x00060DFC
		private void GenerateStageRNG()
		{
			this.stageRng = new Xoroshiro128Plus(this.stageRngGenerator.nextUlong);
			this.bossRewardRng = new Xoroshiro128Plus(this.stageRng.nextUlong);
			this.treasureRng = new Xoroshiro128Plus(this.stageRng.nextUlong);
			this.spawnRng = new Xoroshiro128Plus(this.stageRng.nextUlong);
		}

		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x06001452 RID: 5202 RVA: 0x00062C61 File Offset: 0x00060E61
		// (set) Token: 0x06001453 RID: 5203 RVA: 0x00062C69 File Offset: 0x00060E69
		public DifficultyIndex selectedDifficulty
		{
			get
			{
				return (DifficultyIndex)this.selectedDifficultyInternal;
			}
			set
			{
				this.NetworkselectedDifficultyInternal = (int)value;
			}
		}

		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x06001454 RID: 5204 RVA: 0x00062C72 File Offset: 0x00060E72
		// (set) Token: 0x06001455 RID: 5205 RVA: 0x00062C7A File Offset: 0x00060E7A
		public int livingPlayerCount { get; private set; }

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x06001456 RID: 5206 RVA: 0x00062C83 File Offset: 0x00060E83
		// (set) Token: 0x06001457 RID: 5207 RVA: 0x00062C8B File Offset: 0x00060E8B
		public int participatingPlayerCount { get; private set; }

		// Token: 0x170001CB RID: 459
		// (get) Token: 0x06001458 RID: 5208 RVA: 0x00062C94 File Offset: 0x00060E94
		// (set) Token: 0x06001459 RID: 5209 RVA: 0x00062C9C File Offset: 0x00060E9C
		public float targetMonsterLevel { get; private set; }

		// Token: 0x170001CC RID: 460
		// (get) Token: 0x0600145A RID: 5210 RVA: 0x00062CA5 File Offset: 0x00060EA5
		public float teamlessDamageCoefficient
		{
			get
			{
				return this.difficultyCoefficient;
			}
		}

		// Token: 0x0600145B RID: 5211 RVA: 0x00062CB0 File Offset: 0x00060EB0
		protected void FixedUpdate()
		{
			this.NetworkfixedTime = this.fixedTime + Time.fixedDeltaTime;
			Run.FixedTimeStamp.Update();
			this.livingPlayerCount = PlayerCharacterMasterController.instances.Count((PlayerCharacterMasterController v) => v.master.alive);
			this.participatingPlayerCount = PlayerCharacterMasterController.instances.Count;
			this.OnFixedUpdate();
		}

		// Token: 0x0600145C RID: 5212 RVA: 0x00062D1C File Offset: 0x00060F1C
		protected virtual void OnFixedUpdate()
		{
			DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(this.selectedDifficulty);
			float num = Mathf.Floor(this.fixedTime * 0.016666668f);
			float num2 = (float)this.participatingPlayerCount * 0.3f;
			float num3 = 0.7f + num2;
			float num4 = 0.7f + num2;
			float num5 = Mathf.Pow((float)this.participatingPlayerCount, 0.2f);
			float num6 = 0.046f * difficultyDef.scalingValue * num5;
			float num7 = 0.046f * difficultyDef.scalingValue * num5;
			float num8 = Mathf.Pow(1.15f, (float)this.stageClearCount);
			this.compensatedDifficultyCoefficient = (num4 + num7 * num) * num8;
			this.difficultyCoefficient = (num3 + num6 * num) * num8;
			float num9 = (num3 + num6 * (this.fixedTime * 0.016666668f)) * Mathf.Pow(1.15f, (float)this.stageClearCount);
			if (TeamManager.instance)
			{
				this.targetMonsterLevel = Mathf.Min((num9 - num3) / 0.33f + 1f, TeamManager.naturalLevelCap);
				if (NetworkServer.active)
				{
					uint num10 = (uint)Mathf.FloorToInt(this.targetMonsterLevel);
					uint teamLevel = TeamManager.instance.GetTeamLevel(TeamIndex.Monster);
					if (num10 > teamLevel)
					{
						TeamManager.instance.SetTeamLevel(TeamIndex.Monster, num10);
					}
				}
			}
		}

		// Token: 0x0600145D RID: 5213 RVA: 0x00062E55 File Offset: 0x00061055
		protected void Update()
		{
			this.time = Mathf.Clamp(this.time + Time.deltaTime, this.fixedTime, this.fixedTime + Time.fixedDeltaTime);
			Run.TimeStamp.Update();
		}

		// Token: 0x0600145E RID: 5214 RVA: 0x00062E85 File Offset: 0x00061085
		[Server]
		public virtual bool CanUnlockableBeGrantedThisRun(string unlockableName)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Boolean RoR2.Run::CanUnlockableBeGrantedThisRun(System.String)' called on client");
				return false;
			}
			return !this.unlockablesAlreadyFullyObtained.Contains(unlockableName);
		}

		// Token: 0x0600145F RID: 5215 RVA: 0x00062EAC File Offset: 0x000610AC
		[Server]
		public void GrantUnlockToAllParticipatingPlayers(string unlockableName)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Run::GrantUnlockToAllParticipatingPlayers(System.String)' called on client");
				return;
			}
			if (this.unlockablesAlreadyFullyObtained.Contains(unlockableName))
			{
				return;
			}
			this.unlockablesAlreadyFullyObtained.Add(unlockableName);
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(unlockableName);
			if (unlockableDef == null)
			{
				return;
			}
			foreach (NetworkUser networkUser in NetworkUser.readOnlyInstancesList)
			{
				if (networkUser.isParticipating)
				{
					networkUser.ServerHandleUnlock(unlockableDef);
				}
			}
		}

		// Token: 0x06001460 RID: 5216 RVA: 0x00062F3C File Offset: 0x0006113C
		[Server]
		public void GrantUnlockToSinglePlayer(string unlockableName, CharacterBody body)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Run::GrantUnlockToSinglePlayer(System.String,RoR2.CharacterBody)' called on client");
				return;
			}
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(unlockableName);
			if (unlockableDef == null)
			{
				return;
			}
			if (body)
			{
				NetworkUser networkUser = Util.LookUpBodyNetworkUser(body);
				if (networkUser)
				{
					networkUser.ServerHandleUnlock(unlockableDef);
				}
			}
		}

		// Token: 0x06001461 RID: 5217 RVA: 0x00062F87 File Offset: 0x00061187
		[Server]
		public virtual bool IsUnlockableUnlocked(string unlockableName)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Boolean RoR2.Run::IsUnlockableUnlocked(System.String)' called on client");
				return false;
			}
			return this.unlockablesUnlockedByAnyUser.Contains(unlockableName);
		}

		// Token: 0x06001462 RID: 5218 RVA: 0x00062FAB File Offset: 0x000611AB
		[Server]
		public virtual bool DoesEveryoneHaveThisUnlockableUnlocked(string unlockableName)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Boolean RoR2.Run::DoesEveryoneHaveThisUnlockableUnlocked(System.String)' called on client");
				return false;
			}
			return this.unlockablesUnlockedByAllUsers.Contains(unlockableName);
		}

		// Token: 0x06001463 RID: 5219 RVA: 0x00062FCF File Offset: 0x000611CF
		[Server]
		public void ForceUnlockImmediate(string unlockableName)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Run::ForceUnlockImmediate(System.String)' called on client");
				return;
			}
			this.unlockablesUnlockedByAnyUser.Add(unlockableName);
		}

		// Token: 0x06001464 RID: 5220 RVA: 0x00062FF4 File Offset: 0x000611F4
		private static void PopulateValidStages()
		{
			Run.validStages = (from sceneDef in SceneCatalog.allSceneDefs
			where sceneDef.sceneType == SceneType.Stage
			select sceneDef.sceneField).ToArray<SceneField>();
		}

		// Token: 0x06001465 RID: 5221 RVA: 0x00063058 File Offset: 0x00061258
		public void PickNextStageScene(SceneField[] choices)
		{
			if (this.ruleBook.stageOrder == StageOrder.Normal)
			{
				this.nextStageScene = choices[this.nextStageRng.RangeInt(0, choices.Length)];
				return;
			}
			SceneField[] array = (from v in Run.validStages
			where v.SceneName != SceneManager.GetActiveScene().name
			select v).ToArray<SceneField>();
			this.nextStageScene = array[this.nextStageRng.RangeInt(0, array.Length)];
		}

		// Token: 0x06001466 RID: 5222 RVA: 0x00004507 File Offset: 0x00002707
		protected virtual void OverrideSeed()
		{
		}

		// Token: 0x06001467 RID: 5223 RVA: 0x000630D0 File Offset: 0x000612D0
		protected virtual void BuildUnlockAvailability()
		{
			this.unlockablesUnlockedByAnyUser.Clear();
			this.unlockablesUnlockedByAllUsers.Clear();
			this.unlockablesAlreadyFullyObtained.Clear();
			int num = 0;
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (NetworkUser networkUser in NetworkUser.readOnlyInstancesList)
			{
				if (networkUser.isParticipating)
				{
					num++;
					foreach (UnlockableDef unlockableDef in networkUser.unlockables)
					{
						string name = unlockableDef.name;
						this.unlockablesUnlockedByAnyUser.Add(name);
						if (!dictionary.ContainsKey(name))
						{
							dictionary.Add(name, 0);
						}
						Dictionary<string, int> dictionary2 = dictionary;
						string key = name;
						int value = dictionary2[key] + 1;
						dictionary2[key] = value;
					}
				}
			}
			if (num > 0)
			{
				foreach (KeyValuePair<string, int> keyValuePair in dictionary)
				{
					if (keyValuePair.Value == num)
					{
						this.unlockablesUnlockedByAllUsers.Add(keyValuePair.Key);
						this.unlockablesAlreadyFullyObtained.Add(keyValuePair.Key);
					}
				}
			}
		}

		// Token: 0x06001468 RID: 5224 RVA: 0x0006323C File Offset: 0x0006143C
		protected void Start()
		{
			if (NetworkServer.active)
			{
				this.OverrideSeed();
				this.runRNG = new Xoroshiro128Plus(this.seed);
				this.nextStageRng = new Xoroshiro128Plus(this.runRNG.nextUlong);
				this.stageRngGenerator = new Xoroshiro128Plus(this.runRNG.nextUlong);
				this.GenerateStageRNG();
				Run.PopulateValidStages();
			}
			this.allowNewParticipants = true;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			ReadOnlyCollection<NetworkUser> readOnlyInstancesList = NetworkUser.readOnlyInstancesList;
			for (int i = 0; i < readOnlyInstancesList.Count; i++)
			{
				this.OnUserAdded(readOnlyInstancesList[i]);
			}
			this.allowNewParticipants = false;
			if (NetworkServer.active)
			{
				SceneField[] choices = this.startingScenes;
				string @string = Run.cvRunSceneOverride.GetString();
				if (@string != "")
				{
					choices = new SceneField[]
					{
						new SceneField(@string)
					};
				}
				this.PickNextStageScene(choices);
				NetworkManager.singleton.ServerChangeScene(this.nextStageScene);
			}
			this.BuildUnlockAvailability();
			this.BuildDropTable();
			Action<Run> action = Run.onRunStartGlobal;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x06001469 RID: 5225 RVA: 0x0006334C File Offset: 0x0006154C
		protected void OnDestroy()
		{
			Action<Run> action = Run.onRunDestroyGlobal;
			if (action != null)
			{
				action(this);
			}
			ReadOnlyCollection<CharacterBody> readOnlyInstancesList = CharacterBody.readOnlyInstancesList;
			for (int i = readOnlyInstancesList.Count - 1; i >= 0; i--)
			{
				if (readOnlyInstancesList[i])
				{
					UnityEngine.Object.Destroy(readOnlyInstancesList[i].gameObject);
				}
			}
			ReadOnlyCollection<CharacterMaster> readOnlyInstancesList2 = CharacterMaster.readOnlyInstancesList;
			for (int j = readOnlyInstancesList2.Count - 1; j >= 0; j--)
			{
				if (readOnlyInstancesList2[j])
				{
					UnityEngine.Object.Destroy(readOnlyInstancesList2[j].gameObject);
				}
			}
			if (Stage.instance)
			{
				UnityEngine.Object.Destroy(Stage.instance.gameObject);
			}
			Chat.Clear();
			if (!this.shutdown && GameNetworkManager.singleton.isNetworkActive)
			{
				this.HandlePostRunDestination();
			}
		}

		// Token: 0x0600146A RID: 5226 RVA: 0x00063414 File Offset: 0x00061614
		protected virtual void HandlePostRunDestination()
		{
			if (NetworkServer.active)
			{
				NetworkManager.singleton.ServerChangeScene("lobby");
			}
		}

		// Token: 0x0600146B RID: 5227 RVA: 0x0006342C File Offset: 0x0006162C
		protected void OnApplicationQuit()
		{
			this.shutdown = true;
		}

		// Token: 0x0600146C RID: 5228 RVA: 0x00063438 File Offset: 0x00061638
		[Server]
		public CharacterMaster GetUserMaster(NetworkUserId networkUserId)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'RoR2.CharacterMaster RoR2.Run::GetUserMaster(RoR2.NetworkUserId)' called on client");
				return null;
			}
			CharacterMaster result = null;
			this.userMasters.TryGetValue(networkUserId, out result);
			return result;
		}

		// Token: 0x0600146D RID: 5229 RVA: 0x00063478 File Offset: 0x00061678
		[Server]
		public void OnServerSceneChanged(string sceneName)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Run::OnServerSceneChanged(System.String)' called on client");
				return;
			}
			this.BeginStage();
			this.isGameOverServer = false;
		}

		// Token: 0x0600146E RID: 5230 RVA: 0x0006349C File Offset: 0x0006169C
		[Server]
		private void BeginStage()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Run::BeginStage()' called on client");
				return;
			}
			NetworkServer.Spawn(UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Stage")));
		}

		// Token: 0x0600146F RID: 5231 RVA: 0x000634C7 File Offset: 0x000616C7
		[Server]
		private void EndStage()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Run::EndStage()' called on client");
				return;
			}
			if (Stage.instance)
			{
				UnityEngine.Object.Destroy(Stage.instance);
			}
		}

		// Token: 0x06001470 RID: 5232 RVA: 0x000634F4 File Offset: 0x000616F4
		public void OnUserAdded(NetworkUser user)
		{
			if (NetworkServer.active)
			{
				this.SetupUserCharacterMaster(user);
			}
		}

		// Token: 0x06001471 RID: 5233 RVA: 0x00004507 File Offset: 0x00002707
		public void OnUserRemoved(NetworkUser user)
		{
		}

		// Token: 0x06001472 RID: 5234 RVA: 0x00063504 File Offset: 0x00061704
		[Server]
		private void SetupUserCharacterMaster(NetworkUser user)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Run::SetupUserCharacterMaster(RoR2.NetworkUser)' called on client");
				return;
			}
			if (user.masterObject)
			{
				return;
			}
			CharacterMaster characterMaster = this.GetUserMaster(user.id);
			bool flag = !characterMaster && this.allowNewParticipants;
			if (flag)
			{
				characterMaster = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/CharacterMasters/CommandoMaster"), Vector3.zero, Quaternion.identity).GetComponent<CharacterMaster>();
				this.userMasters[user.id] = characterMaster;
				characterMaster.GiveMoney(this.ruleBook.startingMoney);
				if (this.selectedDifficulty == DifficultyIndex.Easy)
				{
					characterMaster.inventory.GiveItem(ItemIndex.DrizzlePlayerHelper, 1);
				}
				NetworkServer.Spawn(characterMaster.gameObject);
			}
			PlayerCharacterMasterController playerCharacterMasterController = null;
			if (characterMaster)
			{
				user.masterObject = characterMaster.gameObject;
				playerCharacterMasterController = characterMaster.GetComponent<PlayerCharacterMasterController>();
				if (playerCharacterMasterController)
				{
					playerCharacterMasterController.networkUserObject = user.gameObject;
				}
				characterMaster.GetComponent<NetworkIdentity>().AssignClientAuthority(user.connectionToClient);
			}
			if (flag && playerCharacterMasterController)
			{
				Action<Run, PlayerCharacterMasterController> action = Run.onPlayerFirstCreatedServer;
				if (action == null)
				{
					return;
				}
				action(this, playerCharacterMasterController);
			}
		}

		// Token: 0x14000022 RID: 34
		// (add) Token: 0x06001473 RID: 5235 RVA: 0x00063618 File Offset: 0x00061818
		// (remove) Token: 0x06001474 RID: 5236 RVA: 0x0006364C File Offset: 0x0006184C
		public static event Action<Run, PlayerCharacterMasterController> onPlayerFirstCreatedServer;

		// Token: 0x06001475 RID: 5237 RVA: 0x00063680 File Offset: 0x00061880
		[Server]
		public virtual void HandlePlayerFirstEntryAnimation(CharacterBody body, Vector3 spawnPosition, Quaternion spawnRotation)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Run::HandlePlayerFirstEntryAnimation(RoR2.CharacterBody,UnityEngine.Vector3,UnityEngine.Quaternion)' called on client");
				return;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"), body.transform.position, spawnRotation);
			gameObject.GetComponent<SurvivorPodController>().NetworkcharacterBodyObject = body.gameObject;
			NetworkServer.Spawn(gameObject);
		}

		// Token: 0x06001476 RID: 5238 RVA: 0x00004507 File Offset: 0x00002707
		public virtual void OnServerBossAdded(BossGroup bossGroup, CharacterMaster characterMaster)
		{
		}

		// Token: 0x06001477 RID: 5239 RVA: 0x00004507 File Offset: 0x00002707
		public virtual void OnServerBossKilled(bool bossGroupDefeated)
		{
		}

		// Token: 0x06001478 RID: 5240 RVA: 0x00004507 File Offset: 0x00002707
		public virtual void OnServerCharacterBodySpawned(CharacterBody characterBody)
		{
		}

		// Token: 0x06001479 RID: 5241 RVA: 0x00004507 File Offset: 0x00002707
		public virtual void OnServerTeleporterPlaced(SceneDirector sceneDirector, GameObject teleporter)
		{
		}

		// Token: 0x0600147A RID: 5242 RVA: 0x00004507 File Offset: 0x00002707
		public virtual void OnPlayerSpawnPointsPlaced(SceneDirector sceneDirector)
		{
		}

		// Token: 0x0600147B RID: 5243 RVA: 0x000636D3 File Offset: 0x000618D3
		public virtual GameObject GetTeleportEffectPrefab(GameObject objectToTeleport)
		{
			return Resources.Load<GameObject>("Prefabs/Effects/TeleportOutBoom");
		}

		// Token: 0x0600147C RID: 5244 RVA: 0x000636DF File Offset: 0x000618DF
		public int GetDifficultyScaledCost(int baseCost)
		{
			return (int)((float)baseCost * Mathf.Pow(Run.instance.difficultyCoefficient, 1.25f));
		}

		// Token: 0x0600147D RID: 5245 RVA: 0x000636FC File Offset: 0x000618FC
		public void BuildDropTable()
		{
			this.availableTier1DropList.Clear();
			this.availableTier2DropList.Clear();
			this.availableTier3DropList.Clear();
			this.availableLunarDropList.Clear();
			this.availableEquipmentDropList.Clear();
			for (ItemIndex itemIndex = ItemIndex.Syringe; itemIndex < ItemIndex.Count; itemIndex++)
			{
				if (this.availableItems.HasItem(itemIndex))
				{
					ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
					List<PickupIndex> list = null;
					switch (itemDef.tier)
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
					case ItemTier.Lunar:
						list = this.availableLunarDropList;
						break;
					}
					if (list != null)
					{
						list.Add(new PickupIndex(itemIndex));
					}
				}
			}
			for (EquipmentIndex equipmentIndex = EquipmentIndex.CommandMissile; equipmentIndex < EquipmentIndex.Count; equipmentIndex++)
			{
				if (this.availableEquipment.HasEquipment(equipmentIndex))
				{
					EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(equipmentIndex);
					if (equipmentDef.canDrop)
					{
						if (!equipmentDef.isLunar)
						{
							this.availableEquipmentDropList.Add(new PickupIndex(equipmentIndex));
						}
						else
						{
							this.availableLunarDropList.Add(new PickupIndex(equipmentIndex));
						}
					}
				}
			}
			this.smallChestDropTierSelector.Clear();
			this.smallChestDropTierSelector.AddChoice(this.availableTier1DropList, 0.8f);
			this.smallChestDropTierSelector.AddChoice(this.availableTier2DropList, 0.2f);
			this.smallChestDropTierSelector.AddChoice(this.availableTier3DropList, 0.01f);
			this.mediumChestDropTierSelector.Clear();
			this.mediumChestDropTierSelector.AddChoice(this.availableTier2DropList, 0.8f);
			this.mediumChestDropTierSelector.AddChoice(this.availableTier3DropList, 0.2f);
			this.largeChestDropTierSelector.Clear();
		}

		// Token: 0x0600147E RID: 5246 RVA: 0x0006389A File Offset: 0x00061A9A
		[ConCommand(commandName = "run_end", flags = ConVarFlags.SenderMustBeServer, helpText = "Ends the current run.")]
		private static void CCRunEnd(ConCommandArgs args)
		{
			if (Run.instance)
			{
				UnityEngine.Object.Destroy(Run.instance.gameObject);
			}
		}

		// Token: 0x0600147F RID: 5247 RVA: 0x000638B8 File Offset: 0x00061AB8
		[ConCommand(commandName = "run_print_unlockables", flags = ConVarFlags.SenderMustBeServer, helpText = "Prints all unlockables available in this run.")]
		private static void CCRunPrintUnlockables(ConCommandArgs args)
		{
			if (!Run.instance)
			{
				throw new ConCommandException("No run is currently in progress.");
			}
			List<string> list = new List<string>();
			foreach (string item in Run.instance.unlockablesUnlockedByAnyUser)
			{
				list.Add(item);
			}
			Debug.Log(string.Join("\n", list.ToArray()));
		}

		// Token: 0x06001480 RID: 5248 RVA: 0x00063944 File Offset: 0x00061B44
		[ConCommand(commandName = "run_print_seed", flags = ConVarFlags.None, helpText = "Prints the seed of the current run.")]
		private static void CCRunPrintSeed(ConCommandArgs args)
		{
			if (!Run.instance)
			{
				throw new ConCommandException("No run is currently in progress.");
			}
			Debug.LogFormat("Current run seed: {0}", new object[]
			{
				Run.instance.seed
			});
		}

		// Token: 0x06001481 RID: 5249 RVA: 0x00063980 File Offset: 0x00061B80
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			Stage.onServerStageComplete += delegate(Stage stage)
			{
				if (Run.instance && SceneInfo.instance && SceneInfo.instance.countsAsStage)
				{
					Run instance = Run.instance;
					instance.NetworkstageClearCount = instance.stageClearCount + 1;
				}
			};
			HGXml.Register<Run.TimeStamp>(new HGXml.Serializer<Run.TimeStamp>(Run.TimeStamp.ToXml), new HGXml.Deserializer<Run.TimeStamp>(Run.TimeStamp.FromXml));
			HGXml.Register<Run.FixedTimeStamp>(new HGXml.Serializer<Run.FixedTimeStamp>(Run.FixedTimeStamp.ToXml), new HGXml.Deserializer<Run.FixedTimeStamp>(Run.FixedTimeStamp.FromXml));
		}

		// Token: 0x06001482 RID: 5250 RVA: 0x000639EB File Offset: 0x00061BEB
		public virtual void AdvanceStage(string nextSceneName)
		{
			if (Stage.instance)
			{
				Stage.instance.CompleteServer();
			}
			this.GenerateStageRNG();
			NetworkManager.singleton.ServerChangeScene(nextSceneName);
		}

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x06001483 RID: 5251 RVA: 0x00063A14 File Offset: 0x00061C14
		// (set) Token: 0x06001484 RID: 5252 RVA: 0x00063A1C File Offset: 0x00061C1C
		public bool isGameOverServer { get; private set; }

		// Token: 0x14000023 RID: 35
		// (add) Token: 0x06001485 RID: 5253 RVA: 0x00063A28 File Offset: 0x00061C28
		// (remove) Token: 0x06001486 RID: 5254 RVA: 0x00063A5C File Offset: 0x00061C5C
		public static event Action<Run, GameResultType> OnServerGameOver;

		// Token: 0x06001487 RID: 5255 RVA: 0x00063A90 File Offset: 0x00061C90
		public void BeginGameOver(GameResultType gameResultType)
		{
			if (this.isGameOverServer)
			{
				return;
			}
			if (Stage.instance && gameResultType != GameResultType.Lost)
			{
				Stage.instance.CompleteServer();
			}
			this.isGameOverServer = true;
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/GameOverController"));
			GameOverController component = gameObject.GetComponent<GameOverController>();
			component.SetRunReport(RunReport.Generate(this, gameResultType));
			Action<Run, GameResultType> onServerGameOver = Run.OnServerGameOver;
			if (onServerGameOver != null)
			{
				onServerGameOver(this, gameResultType);
			}
			NetworkServer.Spawn(gameObject);
			component.CallRpcClientGameOver();
		}

		// Token: 0x06001488 RID: 5256 RVA: 0x00063B06 File Offset: 0x00061D06
		public virtual void OnClientGameOver(RunReport runReport)
		{
			RunReport.Save(runReport, "PreviousRun");
			Action<Run, RunReport> action = Run.onClientGameOverGlobal;
			if (action == null)
			{
				return;
			}
			action(this, runReport);
		}

		// Token: 0x14000024 RID: 36
		// (add) Token: 0x06001489 RID: 5257 RVA: 0x00063B28 File Offset: 0x00061D28
		// (remove) Token: 0x0600148A RID: 5258 RVA: 0x00063B5C File Offset: 0x00061D5C
		public static event Action<Run, RunReport> onClientGameOverGlobal;

		// Token: 0x0600148B RID: 5259 RVA: 0x00004507 File Offset: 0x00002707
		public virtual void OverrideRuleChoices(RuleChoiceMask mustInclude, RuleChoiceMask mustExclude)
		{
		}

		// Token: 0x0600148C RID: 5260 RVA: 0x00063B90 File Offset: 0x00061D90
		protected void ForceChoice(RuleChoiceMask mustInclude, RuleChoiceMask mustExclude, RuleChoiceDef choiceDef)
		{
			foreach (RuleChoiceDef ruleChoiceDef in choiceDef.ruleDef.choices)
			{
				mustInclude[ruleChoiceDef.globalIndex] = false;
				mustExclude[ruleChoiceDef.globalIndex] = true;
			}
			mustInclude[choiceDef.globalIndex] = true;
			mustExclude[choiceDef.globalIndex] = false;
		}

		// Token: 0x0600148D RID: 5261 RVA: 0x00063C18 File Offset: 0x00061E18
		protected void ForceChoice(RuleChoiceMask mustInclude, RuleChoiceMask mustExclude, string choiceDefGlobalName)
		{
			this.ForceChoice(mustInclude, mustExclude, RuleCatalog.FindChoiceDef(choiceDefGlobalName));
		}

		// Token: 0x14000025 RID: 37
		// (add) Token: 0x0600148E RID: 5262 RVA: 0x00063C28 File Offset: 0x00061E28
		// (remove) Token: 0x0600148F RID: 5263 RVA: 0x00063C5C File Offset: 0x00061E5C
		public static event Action<Run> onRunStartGlobal;

		// Token: 0x14000026 RID: 38
		// (add) Token: 0x06001490 RID: 5264 RVA: 0x00063C90 File Offset: 0x00061E90
		// (remove) Token: 0x06001491 RID: 5265 RVA: 0x00063CC4 File Offset: 0x00061EC4
		public static event Action<Run> onRunDestroyGlobal;

		// Token: 0x06001494 RID: 5268 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x170001CE RID: 462
		// (get) Token: 0x06001495 RID: 5269 RVA: 0x00063DFC File Offset: 0x00061FFC
		// (set) Token: 0x06001496 RID: 5270 RVA: 0x00063E0F File Offset: 0x0006200F
		public ItemMask NetworkavailableItems
		{
			get
			{
				return this.availableItems;
			}
			set
			{
				base.SetSyncVar<ItemMask>(value, ref this.availableItems, 1u);
			}
		}

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x06001497 RID: 5271 RVA: 0x00063E24 File Offset: 0x00062024
		// (set) Token: 0x06001498 RID: 5272 RVA: 0x00063E37 File Offset: 0x00062037
		public EquipmentMask NetworkavailableEquipment
		{
			get
			{
				return this.availableEquipment;
			}
			set
			{
				base.SetSyncVar<EquipmentMask>(value, ref this.availableEquipment, 2u);
			}
		}

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x06001499 RID: 5273 RVA: 0x00063E4C File Offset: 0x0006204C
		// (set) Token: 0x0600149A RID: 5274 RVA: 0x00063E5F File Offset: 0x0006205F
		public ArtifactMask NetworkenabledArtifacts
		{
			get
			{
				return this.enabledArtifacts;
			}
			set
			{
				base.SetSyncVar<ArtifactMask>(value, ref this.enabledArtifacts, 4u);
			}
		}

		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x0600149B RID: 5275 RVA: 0x00063E74 File Offset: 0x00062074
		// (set) Token: 0x0600149C RID: 5276 RVA: 0x00063E87 File Offset: 0x00062087
		public float NetworkfixedTime
		{
			get
			{
				return this.fixedTime;
			}
			set
			{
				base.SetSyncVar<float>(value, ref this.fixedTime, 8u);
			}
		}

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x0600149D RID: 5277 RVA: 0x00063E9C File Offset: 0x0006209C
		// (set) Token: 0x0600149E RID: 5278 RVA: 0x00063EAF File Offset: 0x000620AF
		public int NetworkstageClearCount
		{
			get
			{
				return this.stageClearCount;
			}
			set
			{
				base.SetSyncVar<int>(value, ref this.stageClearCount, 16u);
			}
		}

		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x0600149F RID: 5279 RVA: 0x00063EC4 File Offset: 0x000620C4
		// (set) Token: 0x060014A0 RID: 5280 RVA: 0x00063ED7 File Offset: 0x000620D7
		public int NetworkselectedDifficultyInternal
		{
			get
			{
				return this.selectedDifficultyInternal;
			}
			set
			{
				base.SetSyncVar<int>(value, ref this.selectedDifficultyInternal, 32u);
			}
		}

		// Token: 0x060014A1 RID: 5281 RVA: 0x00063EEC File Offset: 0x000620EC
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				GeneratedNetworkCode._WriteItemMask_None(writer, this.availableItems);
				GeneratedNetworkCode._WriteEquipmentMask_None(writer, this.availableEquipment);
				GeneratedNetworkCode._WriteArtifactMask_None(writer, this.enabledArtifacts);
				writer.Write(this.fixedTime);
				writer.WritePackedUInt32((uint)this.stageClearCount);
				writer.WritePackedUInt32((uint)this.selectedDifficultyInternal);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				GeneratedNetworkCode._WriteItemMask_None(writer, this.availableItems);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				GeneratedNetworkCode._WriteEquipmentMask_None(writer, this.availableEquipment);
			}
			if ((base.syncVarDirtyBits & 4u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				GeneratedNetworkCode._WriteArtifactMask_None(writer, this.enabledArtifacts);
			}
			if ((base.syncVarDirtyBits & 8u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.fixedTime);
			}
			if ((base.syncVarDirtyBits & 16u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32((uint)this.stageClearCount);
			}
			if ((base.syncVarDirtyBits & 32u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32((uint)this.selectedDifficultyInternal);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x060014A2 RID: 5282 RVA: 0x00064094 File Offset: 0x00062294
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.availableItems = GeneratedNetworkCode._ReadItemMask_None(reader);
				this.availableEquipment = GeneratedNetworkCode._ReadEquipmentMask_None(reader);
				this.enabledArtifacts = GeneratedNetworkCode._ReadArtifactMask_None(reader);
				this.fixedTime = reader.ReadSingle();
				this.stageClearCount = (int)reader.ReadPackedUInt32();
				this.selectedDifficultyInternal = (int)reader.ReadPackedUInt32();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.availableItems = GeneratedNetworkCode._ReadItemMask_None(reader);
			}
			if ((num & 2) != 0)
			{
				this.availableEquipment = GeneratedNetworkCode._ReadEquipmentMask_None(reader);
			}
			if ((num & 4) != 0)
			{
				this.enabledArtifacts = GeneratedNetworkCode._ReadArtifactMask_None(reader);
			}
			if ((num & 8) != 0)
			{
				this.fixedTime = reader.ReadSingle();
			}
			if ((num & 16) != 0)
			{
				this.stageClearCount = (int)reader.ReadPackedUInt32();
			}
			if ((num & 32) != 0)
			{
				this.selectedDifficultyInternal = (int)reader.ReadPackedUInt32();
			}
		}

		// Token: 0x04001801 RID: 6145
		private NetworkRuleBook networkRuleBookComponent;

		// Token: 0x04001802 RID: 6146
		public string nameToken = "";

		// Token: 0x04001803 RID: 6147
		[Tooltip("The pool of scenes to select the first scene of the run from.")]
		public SceneField[] startingScenes = Array.Empty<SceneField>();

		// Token: 0x04001804 RID: 6148
		[SyncVar]
		public ItemMask availableItems;

		// Token: 0x04001805 RID: 6149
		[SyncVar]
		public EquipmentMask availableEquipment;

		// Token: 0x04001806 RID: 6150
		[SyncVar]
		public ArtifactMask enabledArtifacts;

		// Token: 0x04001807 RID: 6151
		[SyncVar]
		public float fixedTime;

		// Token: 0x04001808 RID: 6152
		public float time;

		// Token: 0x04001809 RID: 6153
		[SyncVar]
		public int stageClearCount;

		// Token: 0x0400180A RID: 6154
		public SceneField nextStageScene;

		// Token: 0x0400180B RID: 6155
		public ulong seed;

		// Token: 0x0400180C RID: 6156
		public Xoroshiro128Plus runRNG;

		// Token: 0x0400180D RID: 6157
		public Xoroshiro128Plus nextStageRng;

		// Token: 0x0400180E RID: 6158
		public Xoroshiro128Plus stageRngGenerator;

		// Token: 0x0400180F RID: 6159
		public Xoroshiro128Plus stageRng;

		// Token: 0x04001810 RID: 6160
		public Xoroshiro128Plus bossRewardRng;

		// Token: 0x04001811 RID: 6161
		public Xoroshiro128Plus treasureRng;

		// Token: 0x04001812 RID: 6162
		public Xoroshiro128Plus spawnRng;

		// Token: 0x04001813 RID: 6163
		public float difficultyCoefficient = 1f;

		// Token: 0x04001814 RID: 6164
		public float compensatedDifficultyCoefficient = 1f;

		// Token: 0x04001815 RID: 6165
		[SyncVar]
		private int selectedDifficultyInternal = 1;

		// Token: 0x04001819 RID: 6169
		public int shopPortalCount;

		// Token: 0x0400181A RID: 6170
		private static readonly StringConVar cvRunSceneOverride = new StringConVar("run_scene_override", ConVarFlags.Cheat, "", "Overrides the first scene to enter in a run.");

		// Token: 0x0400181B RID: 6171
		private readonly HashSet<string> unlockablesUnlockedByAnyUser = new HashSet<string>();

		// Token: 0x0400181C RID: 6172
		private readonly HashSet<string> unlockablesUnlockedByAllUsers = new HashSet<string>();

		// Token: 0x0400181D RID: 6173
		private readonly HashSet<string> unlockablesAlreadyFullyObtained = new HashSet<string>();

		// Token: 0x0400181E RID: 6174
		private static SceneField[] validStages;

		// Token: 0x0400181F RID: 6175
		private bool shutdown;

		// Token: 0x04001820 RID: 6176
		private Dictionary<NetworkUserId, CharacterMaster> userMasters = new Dictionary<NetworkUserId, CharacterMaster>();

		// Token: 0x04001821 RID: 6177
		private bool allowNewParticipants;

		// Token: 0x04001823 RID: 6179
		private static BoolConVar stage1PodConVar = new BoolConVar("stage1_pod", ConVarFlags.Cheat, "1", "Whether or not to use the pod when spawning on the first stage.");

		// Token: 0x04001824 RID: 6180
		public readonly List<PickupIndex> availableTier1DropList = new List<PickupIndex>();

		// Token: 0x04001825 RID: 6181
		public readonly List<PickupIndex> availableTier2DropList = new List<PickupIndex>();

		// Token: 0x04001826 RID: 6182
		public readonly List<PickupIndex> availableTier3DropList = new List<PickupIndex>();

		// Token: 0x04001827 RID: 6183
		public readonly List<PickupIndex> availableLunarDropList = new List<PickupIndex>();

		// Token: 0x04001828 RID: 6184
		public readonly List<PickupIndex> availableEquipmentDropList = new List<PickupIndex>();

		// Token: 0x04001829 RID: 6185
		public WeightedSelection<List<PickupIndex>> smallChestDropTierSelector = new WeightedSelection<List<PickupIndex>>(8);

		// Token: 0x0400182A RID: 6186
		public WeightedSelection<List<PickupIndex>> mediumChestDropTierSelector = new WeightedSelection<List<PickupIndex>>(8);

		// Token: 0x0400182B RID: 6187
		public WeightedSelection<List<PickupIndex>> largeChestDropTierSelector = new WeightedSelection<List<PickupIndex>>(8);

		// Token: 0x020003BE RID: 958
		[Serializable]
		public struct TimeStamp : IEquatable<Run.TimeStamp>, IComparable<Run.TimeStamp>
		{
			// Token: 0x170001D4 RID: 468
			// (get) Token: 0x060014A3 RID: 5283 RVA: 0x0006418E File Offset: 0x0006238E
			public float timeUntil
			{
				get
				{
					return this.t - Run.TimeStamp.tNow;
				}
			}

			// Token: 0x170001D5 RID: 469
			// (get) Token: 0x060014A4 RID: 5284 RVA: 0x0006419C File Offset: 0x0006239C
			public float timeSince
			{
				get
				{
					return Run.TimeStamp.tNow - this.t;
				}
			}

			// Token: 0x170001D6 RID: 470
			// (get) Token: 0x060014A5 RID: 5285 RVA: 0x000641AA File Offset: 0x000623AA
			public float timeUntilClamped
			{
				get
				{
					return Mathf.Max(this.timeUntil, 0f);
				}
			}

			// Token: 0x170001D7 RID: 471
			// (get) Token: 0x060014A6 RID: 5286 RVA: 0x000641BC File Offset: 0x000623BC
			public float timeSinceClamped
			{
				get
				{
					return Mathf.Max(this.timeSince, 0f);
				}
			}

			// Token: 0x170001D8 RID: 472
			// (get) Token: 0x060014A7 RID: 5287 RVA: 0x000641CE File Offset: 0x000623CE
			public bool hasPassed
			{
				get
				{
					return this.t <= Run.TimeStamp.tNow;
				}
			}

			// Token: 0x060014A8 RID: 5288 RVA: 0x000641E0 File Offset: 0x000623E0
			public override int GetHashCode()
			{
				return this.t.GetHashCode();
			}

			// Token: 0x170001D9 RID: 473
			// (get) Token: 0x060014A9 RID: 5289 RVA: 0x000641FB File Offset: 0x000623FB
			public bool isInfinity
			{
				get
				{
					return float.IsInfinity(this.t);
				}
			}

			// Token: 0x170001DA RID: 474
			// (get) Token: 0x060014AA RID: 5290 RVA: 0x00064208 File Offset: 0x00062408
			public bool isPositiveInfinity
			{
				get
				{
					return float.IsPositiveInfinity(this.t);
				}
			}

			// Token: 0x170001DB RID: 475
			// (get) Token: 0x060014AB RID: 5291 RVA: 0x00064215 File Offset: 0x00062415
			public bool isNegativeInfinity
			{
				get
				{
					return float.IsNegativeInfinity(this.t);
				}
			}

			// Token: 0x060014AC RID: 5292 RVA: 0x00064222 File Offset: 0x00062422
			public static void Update()
			{
				Run.TimeStamp.tNow = Run.instance.time;
			}

			// Token: 0x170001DC RID: 476
			// (get) Token: 0x060014AD RID: 5293 RVA: 0x00064233 File Offset: 0x00062433
			public static Run.TimeStamp now
			{
				get
				{
					return new Run.TimeStamp(Run.TimeStamp.tNow);
				}
			}

			// Token: 0x060014AE RID: 5294 RVA: 0x0006423F File Offset: 0x0006243F
			private TimeStamp(float t)
			{
				this.t = t;
			}

			// Token: 0x060014AF RID: 5295 RVA: 0x00064248 File Offset: 0x00062448
			public bool Equals(Run.TimeStamp other)
			{
				return this.t.Equals(other.t);
			}

			// Token: 0x060014B0 RID: 5296 RVA: 0x00064269 File Offset: 0x00062469
			public override bool Equals(object obj)
			{
				return obj is Run.TimeStamp && this.Equals((Run.TimeStamp)obj);
			}

			// Token: 0x060014B1 RID: 5297 RVA: 0x00064284 File Offset: 0x00062484
			public int CompareTo(Run.TimeStamp other)
			{
				return this.t.CompareTo(other.t);
			}

			// Token: 0x060014B2 RID: 5298 RVA: 0x000642A5 File Offset: 0x000624A5
			public static Run.TimeStamp operator +(Run.TimeStamp a, float b)
			{
				return new Run.TimeStamp(a.t + b);
			}

			// Token: 0x060014B3 RID: 5299 RVA: 0x000642B4 File Offset: 0x000624B4
			public static Run.TimeStamp operator -(Run.TimeStamp a, float b)
			{
				return new Run.TimeStamp(a.t - b);
			}

			// Token: 0x060014B4 RID: 5300 RVA: 0x000642C3 File Offset: 0x000624C3
			public static float operator -(Run.TimeStamp a, Run.TimeStamp b)
			{
				return a.t - b.t;
			}

			// Token: 0x060014B5 RID: 5301 RVA: 0x000642D2 File Offset: 0x000624D2
			public static bool operator <(Run.TimeStamp a, Run.TimeStamp b)
			{
				return a.t < b.t;
			}

			// Token: 0x060014B6 RID: 5302 RVA: 0x000642E2 File Offset: 0x000624E2
			public static bool operator >(Run.TimeStamp a, Run.TimeStamp b)
			{
				return a.t > b.t;
			}

			// Token: 0x060014B7 RID: 5303 RVA: 0x000642F2 File Offset: 0x000624F2
			public static bool operator <=(Run.TimeStamp a, Run.TimeStamp b)
			{
				return a.t <= b.t;
			}

			// Token: 0x060014B8 RID: 5304 RVA: 0x00064305 File Offset: 0x00062505
			public static bool operator >=(Run.TimeStamp a, Run.TimeStamp b)
			{
				return a.t >= b.t;
			}

			// Token: 0x060014B9 RID: 5305 RVA: 0x00064318 File Offset: 0x00062518
			public static bool operator ==(Run.TimeStamp a, Run.TimeStamp b)
			{
				return a.Equals(b);
			}

			// Token: 0x060014BA RID: 5306 RVA: 0x00064322 File Offset: 0x00062522
			public static bool operator !=(Run.TimeStamp a, Run.TimeStamp b)
			{
				return !a.Equals(b);
			}

			// Token: 0x060014BB RID: 5307 RVA: 0x0006432F File Offset: 0x0006252F
			public static Run.TimeStamp Deserialize(NetworkReader reader)
			{
				return new Run.TimeStamp(reader.ReadSingle());
			}

			// Token: 0x060014BC RID: 5308 RVA: 0x0006433C File Offset: 0x0006253C
			public static void Serialize(NetworkWriter writer, Run.TimeStamp timeStamp)
			{
				writer.Write(timeStamp.t);
			}

			// Token: 0x060014BD RID: 5309 RVA: 0x0006434B File Offset: 0x0006254B
			public static void ToXml(XElement element, Run.TimeStamp src)
			{
				element.Value = TextSerialization.ToStringInvariant(src.t);
			}

			// Token: 0x060014BE RID: 5310 RVA: 0x00064360 File Offset: 0x00062560
			public static bool FromXml(XElement element, ref Run.TimeStamp dest)
			{
				float num;
				if (TextSerialization.TryParseInvariant(element.Value, out num))
				{
					dest = new Run.TimeStamp(num);
					return true;
				}
				return false;
			}

			// Token: 0x060014BF RID: 5311 RVA: 0x0006438B File Offset: 0x0006258B
			[RuntimeInitializeOnLoadMethod]
			private static void Init()
			{
				HGXml.Register<Run.TimeStamp>(new HGXml.Serializer<Run.TimeStamp>(Run.TimeStamp.ToXml), new HGXml.Deserializer<Run.TimeStamp>(Run.TimeStamp.FromXml));
			}

			// Token: 0x04001831 RID: 6193
			public readonly float t;

			// Token: 0x04001832 RID: 6194
			private static float tNow;

			// Token: 0x04001833 RID: 6195
			public static readonly Run.TimeStamp zero = new Run.TimeStamp(0f);

			// Token: 0x04001834 RID: 6196
			public static readonly Run.TimeStamp positiveInfinity = new Run.TimeStamp(float.PositiveInfinity);

			// Token: 0x04001835 RID: 6197
			public static readonly Run.TimeStamp negativeInfinity = new Run.TimeStamp(float.NegativeInfinity);
		}

		// Token: 0x020003BF RID: 959
		[Serializable]
		public struct FixedTimeStamp : IEquatable<Run.FixedTimeStamp>, IComparable<Run.FixedTimeStamp>
		{
			// Token: 0x170001DD RID: 477
			// (get) Token: 0x060014C1 RID: 5313 RVA: 0x000643D9 File Offset: 0x000625D9
			public float timeUntil
			{
				get
				{
					return this.t - Run.FixedTimeStamp.tNow;
				}
			}

			// Token: 0x170001DE RID: 478
			// (get) Token: 0x060014C2 RID: 5314 RVA: 0x000643E7 File Offset: 0x000625E7
			public float timeSince
			{
				get
				{
					return Run.FixedTimeStamp.tNow - this.t;
				}
			}

			// Token: 0x170001DF RID: 479
			// (get) Token: 0x060014C3 RID: 5315 RVA: 0x000643F5 File Offset: 0x000625F5
			public float timeUntilClamped
			{
				get
				{
					return Mathf.Max(this.timeUntil, 0f);
				}
			}

			// Token: 0x170001E0 RID: 480
			// (get) Token: 0x060014C4 RID: 5316 RVA: 0x00064407 File Offset: 0x00062607
			public float timeSinceClamped
			{
				get
				{
					return Mathf.Max(this.timeSince, 0f);
				}
			}

			// Token: 0x170001E1 RID: 481
			// (get) Token: 0x060014C5 RID: 5317 RVA: 0x00064419 File Offset: 0x00062619
			public bool hasPassed
			{
				get
				{
					return this.t <= Run.FixedTimeStamp.tNow;
				}
			}

			// Token: 0x060014C6 RID: 5318 RVA: 0x0006442C File Offset: 0x0006262C
			public override int GetHashCode()
			{
				return this.t.GetHashCode();
			}

			// Token: 0x170001E2 RID: 482
			// (get) Token: 0x060014C7 RID: 5319 RVA: 0x00064447 File Offset: 0x00062647
			public bool isInfinity
			{
				get
				{
					return float.IsInfinity(this.t);
				}
			}

			// Token: 0x170001E3 RID: 483
			// (get) Token: 0x060014C8 RID: 5320 RVA: 0x00064454 File Offset: 0x00062654
			public bool isPositiveInfinity
			{
				get
				{
					return float.IsPositiveInfinity(this.t);
				}
			}

			// Token: 0x170001E4 RID: 484
			// (get) Token: 0x060014C9 RID: 5321 RVA: 0x00064461 File Offset: 0x00062661
			public bool isNegativeInfinity
			{
				get
				{
					return float.IsNegativeInfinity(this.t);
				}
			}

			// Token: 0x060014CA RID: 5322 RVA: 0x0006446E File Offset: 0x0006266E
			public static void Update()
			{
				Run.FixedTimeStamp.tNow = Run.instance.fixedTime;
			}

			// Token: 0x170001E5 RID: 485
			// (get) Token: 0x060014CB RID: 5323 RVA: 0x0006447F File Offset: 0x0006267F
			public static Run.FixedTimeStamp now
			{
				get
				{
					return new Run.FixedTimeStamp(Run.FixedTimeStamp.tNow);
				}
			}

			// Token: 0x060014CC RID: 5324 RVA: 0x0006448B File Offset: 0x0006268B
			private FixedTimeStamp(float t)
			{
				this.t = t;
			}

			// Token: 0x060014CD RID: 5325 RVA: 0x00064494 File Offset: 0x00062694
			public bool Equals(Run.FixedTimeStamp other)
			{
				return this.t.Equals(other.t);
			}

			// Token: 0x060014CE RID: 5326 RVA: 0x000644B5 File Offset: 0x000626B5
			public override bool Equals(object obj)
			{
				return obj is Run.FixedTimeStamp && this.Equals((Run.FixedTimeStamp)obj);
			}

			// Token: 0x060014CF RID: 5327 RVA: 0x000644D0 File Offset: 0x000626D0
			public int CompareTo(Run.FixedTimeStamp other)
			{
				return this.t.CompareTo(other.t);
			}

			// Token: 0x060014D0 RID: 5328 RVA: 0x000644F1 File Offset: 0x000626F1
			public static Run.FixedTimeStamp operator +(Run.FixedTimeStamp a, float b)
			{
				return new Run.FixedTimeStamp(a.t + b);
			}

			// Token: 0x060014D1 RID: 5329 RVA: 0x00064500 File Offset: 0x00062700
			public static Run.FixedTimeStamp operator -(Run.FixedTimeStamp a, float b)
			{
				return new Run.FixedTimeStamp(a.t - b);
			}

			// Token: 0x060014D2 RID: 5330 RVA: 0x0006450F File Offset: 0x0006270F
			public static float operator -(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return a.t - b.t;
			}

			// Token: 0x060014D3 RID: 5331 RVA: 0x0006451E File Offset: 0x0006271E
			public static bool operator <(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return a.t < b.t;
			}

			// Token: 0x060014D4 RID: 5332 RVA: 0x0006452E File Offset: 0x0006272E
			public static bool operator >(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return a.t > b.t;
			}

			// Token: 0x060014D5 RID: 5333 RVA: 0x0006453E File Offset: 0x0006273E
			public static bool operator <=(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return a.t <= b.t;
			}

			// Token: 0x060014D6 RID: 5334 RVA: 0x00064551 File Offset: 0x00062751
			public static bool operator >=(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return a.t >= b.t;
			}

			// Token: 0x060014D7 RID: 5335 RVA: 0x00064564 File Offset: 0x00062764
			public static bool operator ==(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return a.Equals(b);
			}

			// Token: 0x060014D8 RID: 5336 RVA: 0x0006456E File Offset: 0x0006276E
			public static bool operator !=(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return !a.Equals(b);
			}

			// Token: 0x060014D9 RID: 5337 RVA: 0x0006457B File Offset: 0x0006277B
			public static Run.FixedTimeStamp Deserialize(NetworkReader reader)
			{
				return new Run.FixedTimeStamp(reader.ReadSingle());
			}

			// Token: 0x060014DA RID: 5338 RVA: 0x00064588 File Offset: 0x00062788
			public static void Serialize(NetworkWriter writer, Run.FixedTimeStamp timeStamp)
			{
				writer.Write(timeStamp.t);
			}

			// Token: 0x060014DB RID: 5339 RVA: 0x00064597 File Offset: 0x00062797
			public static void ToXml(XElement element, Run.FixedTimeStamp src)
			{
				element.Value = TextSerialization.ToStringInvariant(src.t);
			}

			// Token: 0x060014DC RID: 5340 RVA: 0x000645AC File Offset: 0x000627AC
			public static bool FromXml(XElement element, ref Run.FixedTimeStamp dest)
			{
				float num;
				if (TextSerialization.TryParseInvariant(element.Value, out num))
				{
					dest = new Run.FixedTimeStamp(num);
					return true;
				}
				return false;
			}

			// Token: 0x04001836 RID: 6198
			public readonly float t;

			// Token: 0x04001837 RID: 6199
			private static float tNow;

			// Token: 0x04001838 RID: 6200
			public static readonly Run.FixedTimeStamp zero = new Run.FixedTimeStamp(0f);

			// Token: 0x04001839 RID: 6201
			public static readonly Run.FixedTimeStamp positiveInfinity = new Run.FixedTimeStamp(float.PositiveInfinity);

			// Token: 0x0400183A RID: 6202
			public static readonly Run.FixedTimeStamp negativeInfinity = new Run.FixedTimeStamp(float.NegativeInfinity);
		}
	}
}
