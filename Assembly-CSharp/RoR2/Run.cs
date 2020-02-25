using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using RoR2.ConVar;
using RoR2.Networking;
using Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000305 RID: 773
	[DisallowMultipleComponent]
	[RequireComponent(typeof(NetworkRuleBook))]
	public class Run : NetworkBehaviour
	{
		// Token: 0x1700021E RID: 542
		// (get) Token: 0x060011A4 RID: 4516 RVA: 0x0004CF91 File Offset: 0x0004B191
		// (set) Token: 0x060011A5 RID: 4517 RVA: 0x0004CF98 File Offset: 0x0004B198
		public static Run instance { get; private set; }

		// Token: 0x060011A6 RID: 4518 RVA: 0x0004CFA0 File Offset: 0x0004B1A0
		private void OnEnable()
		{
			Run.instance = SingletonHelper.Assign<Run>(Run.instance, this);
		}

		// Token: 0x060011A7 RID: 4519 RVA: 0x0004CFB2 File Offset: 0x0004B1B2
		private void OnDisable()
		{
			Run.instance = SingletonHelper.Unassign<Run>(Run.instance, this);
		}

		// Token: 0x060011A8 RID: 4520 RVA: 0x0004CFC4 File Offset: 0x0004B1C4
		protected void Awake()
		{
			this.networkRuleBookComponent = base.GetComponent<NetworkRuleBook>();
		}

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x060011A9 RID: 4521 RVA: 0x0004CFD2 File Offset: 0x0004B1D2
		public RuleBook ruleBook
		{
			get
			{
				return this.networkRuleBookComponent.ruleBook;
			}
		}

		// Token: 0x060011AA RID: 4522 RVA: 0x0004CFE0 File Offset: 0x0004B1E0
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

		// Token: 0x060011AB RID: 4523 RVA: 0x0004D040 File Offset: 0x0004B240
		[Server]
		private void SetRunStopwatchPaused(bool isPaused)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Run::SetRunStopwatchPaused(System.Boolean)' called on client");
				return;
			}
			if (isPaused != this.runStopwatch.isPaused)
			{
				Run.RunStopwatch networkrunStopwatch = this.runStopwatch;
				networkrunStopwatch.isPaused = isPaused;
				float num = this.GetRunStopwatch();
				if (isPaused)
				{
					networkrunStopwatch.offsetFromFixedTime = num;
				}
				else
				{
					networkrunStopwatch.offsetFromFixedTime = num - this.fixedTime;
				}
				this.NetworkrunStopwatch = networkrunStopwatch;
			}
		}

		// Token: 0x060011AC RID: 4524 RVA: 0x0004D0A9 File Offset: 0x0004B2A9
		public float GetRunStopwatch()
		{
			if (this.runStopwatch.isPaused)
			{
				return this.runStopwatch.offsetFromFixedTime;
			}
			return this.fixedTime + this.runStopwatch.offsetFromFixedTime;
		}

		// Token: 0x060011AD RID: 4525 RVA: 0x0004D0D8 File Offset: 0x0004B2D8
		[Server]
		public void SetRunStopwatch(float t)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Run::SetRunStopwatch(System.Single)' called on client");
				return;
			}
			Run.RunStopwatch runStopwatch = this.runStopwatch;
			if (runStopwatch.isPaused)
			{
				runStopwatch.offsetFromFixedTime = t;
			}
			else
			{
				runStopwatch.offsetFromFixedTime = t - this.fixedTime;
			}
			this.NetworkrunStopwatch = runStopwatch;
		}

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x060011AE RID: 4526 RVA: 0x0004D129 File Offset: 0x0004B329
		public bool isRunStopwatchPaused
		{
			get
			{
				return this.runStopwatch.isPaused;
			}
		}

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x060011AF RID: 4527 RVA: 0x0004D136 File Offset: 0x0004B336
		public virtual int loopClearCount
		{
			get
			{
				return this.stageClearCount / 4;
			}
		}

		// Token: 0x060011B0 RID: 4528 RVA: 0x0004D140 File Offset: 0x0004B340
		private void GenerateStageRNG()
		{
			this.stageRng = new Xoroshiro128Plus(this.stageRngGenerator.nextUlong);
			this.bossRewardRng = new Xoroshiro128Plus(this.stageRng.nextUlong);
			this.treasureRng = new Xoroshiro128Plus(this.stageRng.nextUlong);
			this.spawnRng = new Xoroshiro128Plus(this.stageRng.nextUlong);
		}

		// Token: 0x17000222 RID: 546
		// (get) Token: 0x060011B1 RID: 4529 RVA: 0x0004D1A5 File Offset: 0x0004B3A5
		// (set) Token: 0x060011B2 RID: 4530 RVA: 0x0004D1AD File Offset: 0x0004B3AD
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

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x060011B3 RID: 4531 RVA: 0x0004D1B6 File Offset: 0x0004B3B6
		// (set) Token: 0x060011B4 RID: 4532 RVA: 0x0004D1BE File Offset: 0x0004B3BE
		public int livingPlayerCount { get; private set; }

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x060011B5 RID: 4533 RVA: 0x0004D1C7 File Offset: 0x0004B3C7
		// (set) Token: 0x060011B6 RID: 4534 RVA: 0x0004D1CF File Offset: 0x0004B3CF
		public int participatingPlayerCount { get; private set; }

		// Token: 0x17000225 RID: 549
		// (get) Token: 0x060011B7 RID: 4535 RVA: 0x0004D1D8 File Offset: 0x0004B3D8
		// (set) Token: 0x060011B8 RID: 4536 RVA: 0x0004D1E0 File Offset: 0x0004B3E0
		public float targetMonsterLevel { get; private set; }

		// Token: 0x17000226 RID: 550
		// (get) Token: 0x060011B9 RID: 4537 RVA: 0x0004D1E9 File Offset: 0x0004B3E9
		public float teamlessDamageCoefficient
		{
			get
			{
				return this.difficultyCoefficient;
			}
		}

		// Token: 0x060011BA RID: 4538 RVA: 0x0004D1F4 File Offset: 0x0004B3F4
		protected void FixedUpdate()
		{
			this.NetworkfixedTime = this.fixedTime + Time.fixedDeltaTime;
			Run.FixedTimeStamp.Update();
			if (NetworkServer.active)
			{
				this.SetRunStopwatchPaused(!this.ShouldUpdateRunStopwatch());
			}
			this.livingPlayerCount = PlayerCharacterMasterController.instances.Count((PlayerCharacterMasterController v) => v.master.alive);
			this.participatingPlayerCount = PlayerCharacterMasterController.instances.Count;
			this.OnFixedUpdate();
		}

		// Token: 0x060011BB RID: 4539 RVA: 0x0004D274 File Offset: 0x0004B474
		protected virtual void OnFixedUpdate()
		{
			float num = this.GetRunStopwatch();
			DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(this.selectedDifficulty);
			float num2 = Mathf.Floor(num * 0.016666668f);
			float num3 = (float)this.participatingPlayerCount * 0.3f;
			float num4 = 0.7f + num3;
			float num5 = 0.7f + num3;
			float num6 = Mathf.Pow((float)this.participatingPlayerCount, 0.2f);
			float num7 = 0.046f * difficultyDef.scalingValue * num6;
			float num8 = 0.046f * difficultyDef.scalingValue * num6;
			float num9 = Mathf.Pow(1.15f, (float)this.stageClearCount);
			this.compensatedDifficultyCoefficient = (num5 + num8 * num2) * num9;
			this.difficultyCoefficient = (num4 + num7 * num2) * num9;
			float num10 = (num4 + num7 * (num * 0.016666668f)) * Mathf.Pow(1.15f, (float)this.stageClearCount);
			if (TeamManager.instance)
			{
				this.targetMonsterLevel = Mathf.Min((num10 - num4) / 0.33f + 1f, TeamManager.naturalLevelCap);
				if (NetworkServer.active)
				{
					uint num11 = (uint)Mathf.FloorToInt(this.targetMonsterLevel);
					uint teamLevel = TeamManager.instance.GetTeamLevel(TeamIndex.Monster);
					if (num11 > teamLevel)
					{
						TeamManager.instance.SetTeamLevel(TeamIndex.Monster, num11);
					}
				}
			}
		}

		// Token: 0x060011BC RID: 4540 RVA: 0x0004D3AE File Offset: 0x0004B5AE
		protected void Update()
		{
			this.time = Mathf.Clamp(this.time + Time.deltaTime, this.fixedTime, this.fixedTime + Time.fixedDeltaTime);
			Run.TimeStamp.Update();
		}

		// Token: 0x060011BD RID: 4541 RVA: 0x0004D3E0 File Offset: 0x0004B5E0
		protected virtual bool ShouldUpdateRunStopwatch()
		{
			SceneDef mostRecentSceneDef = SceneCatalog.mostRecentSceneDef;
			return (mostRecentSceneDef.sceneType == SceneType.Stage || !(mostRecentSceneDef.nameToken != "MAP_ARENA_TITLE")) && this.livingPlayerCount > 0;
		}

		// Token: 0x060011BE RID: 4542 RVA: 0x0004D419 File Offset: 0x0004B619
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

		// Token: 0x060011BF RID: 4543 RVA: 0x0004D440 File Offset: 0x0004B640
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

		// Token: 0x060011C0 RID: 4544 RVA: 0x0004D4D0 File Offset: 0x0004B6D0
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

		// Token: 0x060011C1 RID: 4545 RVA: 0x0004D51B File Offset: 0x0004B71B
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

		// Token: 0x060011C2 RID: 4546 RVA: 0x0004D53F File Offset: 0x0004B73F
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

		// Token: 0x060011C3 RID: 4547 RVA: 0x0004D563 File Offset: 0x0004B763
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

		// Token: 0x060011C4 RID: 4548 RVA: 0x0004D587 File Offset: 0x0004B787
		private static void PopulateValidStages()
		{
			Run.validStages = (from sceneDef in SceneCatalog.allSceneDefs
			where sceneDef.sceneType == SceneType.Stage
			select sceneDef).ToArray<SceneDef>();
		}

		// Token: 0x060011C5 RID: 4549 RVA: 0x0004D5BC File Offset: 0x0004B7BC
		public unsafe void PickNextStageScene(SceneDef[] choices)
		{
			if (this.ruleBook.stageOrder == StageOrder.Normal)
			{
				this.nextStageScene = *this.nextStageRng.NextElementUniform<SceneDef>(choices);
				return;
			}
			SceneDef[] array = (from sceneDef in Run.validStages
			where sceneDef != this.nextStageScene
			select sceneDef).ToArray<SceneDef>();
			this.nextStageScene = *this.nextStageRng.NextElementUniform<SceneDef>(array);
		}

		// Token: 0x060011C6 RID: 4550 RVA: 0x0000409B File Offset: 0x0000229B
		protected virtual void OverrideSeed()
		{
		}

		// Token: 0x060011C7 RID: 4551 RVA: 0x0004D61C File Offset: 0x0004B81C
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

		// Token: 0x060011C8 RID: 4552 RVA: 0x0004D788 File Offset: 0x0004B988
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
				SceneDef[] choices = this.startingScenes;
				string @string = Run.cvRunSceneOverride.GetString();
				if (@string != "")
				{
					choices = new SceneDef[]
					{
						SceneCatalog.GetSceneDefFromSceneName(@string)
					};
				}
				this.PickNextStageScene(choices);
				NetworkManager.singleton.ServerChangeScene(this.nextStageScene.ChooseSceneName());
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

		// Token: 0x060011C9 RID: 4553 RVA: 0x0004D898 File Offset: 0x0004BA98
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

		// Token: 0x060011CA RID: 4554 RVA: 0x0004D960 File Offset: 0x0004BB60
		protected virtual void HandlePostRunDestination()
		{
			if (NetworkServer.active)
			{
				NetworkManager.singleton.ServerChangeScene("lobby");
			}
		}

		// Token: 0x060011CB RID: 4555 RVA: 0x0004D978 File Offset: 0x0004BB78
		protected void OnApplicationQuit()
		{
			this.shutdown = true;
		}

		// Token: 0x060011CC RID: 4556 RVA: 0x0004D984 File Offset: 0x0004BB84
		[Server]
		public CharacterMaster GetUserMaster(NetworkUserId networkUserId)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'RoR2.CharacterMaster RoR2.Run::GetUserMaster(RoR2.NetworkUserId)' called on client");
				return null;
			}
			CharacterMaster result;
			this.userMasters.TryGetValue(networkUserId, out result);
			return result;
		}

		// Token: 0x060011CD RID: 4557 RVA: 0x0004D9C2 File Offset: 0x0004BBC2
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

		// Token: 0x060011CE RID: 4558 RVA: 0x0004D9E6 File Offset: 0x0004BBE6
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

		// Token: 0x060011CF RID: 4559 RVA: 0x0004DA11 File Offset: 0x0004BC11
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

		// Token: 0x060011D0 RID: 4560 RVA: 0x0004DA3E File Offset: 0x0004BC3E
		public void OnUserAdded(NetworkUser user)
		{
			if (NetworkServer.active)
			{
				this.SetupUserCharacterMaster(user);
			}
		}

		// Token: 0x060011D1 RID: 4561 RVA: 0x0000409B File Offset: 0x0000229B
		public void OnUserRemoved(NetworkUser user)
		{
		}

		// Token: 0x060011D2 RID: 4562 RVA: 0x0004DA50 File Offset: 0x0004BC50
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
				else if (this.selectedDifficulty == DifficultyIndex.Hard)
				{
					characterMaster.inventory.GiveItem(ItemIndex.MonsoonPlayerHelper, 1);
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

		// Token: 0x14000034 RID: 52
		// (add) Token: 0x060011D3 RID: 4563 RVA: 0x0004DB7C File Offset: 0x0004BD7C
		// (remove) Token: 0x060011D4 RID: 4564 RVA: 0x0004DBB0 File Offset: 0x0004BDB0
		public static event Action<Run, PlayerCharacterMasterController> onPlayerFirstCreatedServer;

		// Token: 0x060011D5 RID: 4565 RVA: 0x0004DBE4 File Offset: 0x0004BDE4
		[Server]
		public virtual void HandlePlayerFirstEntryAnimation(CharacterBody body, Vector3 spawnPosition, Quaternion spawnRotation)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Run::HandlePlayerFirstEntryAnimation(RoR2.CharacterBody,UnityEngine.Vector3,UnityEngine.Quaternion)' called on client");
				return;
			}
			if (body.preferredPodPrefab)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(body.preferredPodPrefab, body.transform.position, spawnRotation);
				gameObject.GetComponent<VehicleSeat>().AssignPassenger(body.gameObject);
				NetworkServer.Spawn(gameObject);
				return;
			}
			body.SetBodyStateToPreferredInitialState();
		}

		// Token: 0x060011D6 RID: 4566 RVA: 0x0000409B File Offset: 0x0000229B
		public virtual void OnServerBossAdded(BossGroup bossGroup, CharacterMaster characterMaster)
		{
		}

		// Token: 0x060011D7 RID: 4567 RVA: 0x0000409B File Offset: 0x0000229B
		public virtual void OnServerBossDefeated(BossGroup bossGroup)
		{
		}

		// Token: 0x060011D8 RID: 4568 RVA: 0x0000409B File Offset: 0x0000229B
		public virtual void OnServerCharacterBodySpawned(CharacterBody characterBody)
		{
		}

		// Token: 0x060011D9 RID: 4569 RVA: 0x0000409B File Offset: 0x0000229B
		public virtual void OnServerTeleporterPlaced(SceneDirector sceneDirector, GameObject teleporter)
		{
		}

		// Token: 0x060011DA RID: 4570 RVA: 0x0000409B File Offset: 0x0000229B
		public virtual void OnPlayerSpawnPointsPlaced(SceneDirector sceneDirector)
		{
		}

		// Token: 0x060011DB RID: 4571 RVA: 0x0004DC48 File Offset: 0x0004BE48
		public virtual GameObject GetTeleportEffectPrefab(GameObject objectToTeleport)
		{
			return Resources.Load<GameObject>("Prefabs/Effects/TeleportOutBoom");
		}

		// Token: 0x060011DC RID: 4572 RVA: 0x0004DC54 File Offset: 0x0004BE54
		public int GetDifficultyScaledCost(int baseCost)
		{
			return (int)((float)baseCost * Mathf.Pow(Run.instance.difficultyCoefficient, 1.25f));
		}

		// Token: 0x060011DD RID: 4573 RVA: 0x0004DC70 File Offset: 0x0004BE70
		public void BuildDropTable()
		{
			this.availableTier1DropList.Clear();
			this.availableTier2DropList.Clear();
			this.availableTier3DropList.Clear();
			this.availableLunarDropList.Clear();
			this.availableEquipmentDropList.Clear();
			this.availableBossDropList.Clear();
			ItemIndex itemIndex = ItemIndex.Syringe;
			ItemIndex itemCount = (ItemIndex)ItemCatalog.itemCount;
			while (itemIndex < itemCount)
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
					case ItemTier.Boss:
						list = this.availableBossDropList;
						break;
					}
					if (list != null)
					{
						list.Add(new PickupIndex(itemIndex));
					}
				}
				itemIndex++;
			}
			EquipmentIndex equipmentIndex = EquipmentIndex.CommandMissile;
			EquipmentIndex equipmentCount = (EquipmentIndex)EquipmentCatalog.equipmentCount;
			while (equipmentIndex < equipmentCount)
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
				equipmentIndex++;
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

		// Token: 0x060011DE RID: 4574 RVA: 0x0004DE3A File Offset: 0x0004C03A
		[ConCommand(commandName = "run_end", flags = ConVarFlags.SenderMustBeServer, helpText = "Ends the current run.")]
		private static void CCRunEnd(ConCommandArgs args)
		{
			if (Run.instance)
			{
				UnityEngine.Object.Destroy(Run.instance.gameObject);
			}
		}

		// Token: 0x060011DF RID: 4575 RVA: 0x0004DE58 File Offset: 0x0004C058
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

		// Token: 0x060011E0 RID: 4576 RVA: 0x0004DEE4 File Offset: 0x0004C0E4
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

		// Token: 0x060011E1 RID: 4577 RVA: 0x0004DF1F File Offset: 0x0004C11F
		[ConCommand(commandName = "run_set_stages_cleared", flags = (ConVarFlags.ExecuteOnServer | ConVarFlags.Cheat), helpText = "Sets the current number of stages cleared in the run.")]
		private static void CCRunSetStagesCleared(ConCommandArgs args)
		{
			if (!Run.instance)
			{
				throw new ConCommandException("No run is currently in progress.");
			}
			Run.instance.NetworkstageClearCount = args.GetArgInt(0);
		}

		// Token: 0x060011E2 RID: 4578 RVA: 0x0004DF4C File Offset: 0x0004C14C
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			Stage.onServerStageComplete += delegate(Stage stage)
			{
				if (Run.instance && SceneCatalog.GetSceneDefForCurrentScene().sceneType == SceneType.Stage)
				{
					Run instance = Run.instance;
					instance.NetworkstageClearCount = instance.stageClearCount + 1;
				}
			};
			HGXml.Register<Run.TimeStamp>(new HGXml.Serializer<Run.TimeStamp>(Run.TimeStamp.ToXml), new HGXml.Deserializer<Run.TimeStamp>(Run.TimeStamp.FromXml));
			HGXml.Register<Run.FixedTimeStamp>(new HGXml.Serializer<Run.FixedTimeStamp>(Run.FixedTimeStamp.ToXml), new HGXml.Deserializer<Run.FixedTimeStamp>(Run.FixedTimeStamp.FromXml));
		}

		// Token: 0x060011E3 RID: 4579 RVA: 0x0004DFB7 File Offset: 0x0004C1B7
		public virtual void AdvanceStage(SceneDef nextScene)
		{
			if (Stage.instance)
			{
				Stage.instance.CompleteServer();
			}
			this.GenerateStageRNG();
			NetworkManager.singleton.ServerChangeScene(nextScene.ChooseSceneName());
		}

		// Token: 0x17000227 RID: 551
		// (get) Token: 0x060011E4 RID: 4580 RVA: 0x0004DFE5 File Offset: 0x0004C1E5
		// (set) Token: 0x060011E5 RID: 4581 RVA: 0x0004DFED File Offset: 0x0004C1ED
		public bool isGameOverServer { get; private set; }

		// Token: 0x14000035 RID: 53
		// (add) Token: 0x060011E6 RID: 4582 RVA: 0x0004DFF8 File Offset: 0x0004C1F8
		// (remove) Token: 0x060011E7 RID: 4583 RVA: 0x0004E02C File Offset: 0x0004C22C
		public static event Action<Run, GameResultType> OnServerGameOver;

		// Token: 0x060011E8 RID: 4584 RVA: 0x0004E060 File Offset: 0x0004C260
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
			if (gameResultType == GameResultType.Unknown)
			{
				for (int i = 0; i < NetworkUser.readOnlyInstancesList.Count; i++)
				{
					NetworkUser networkUser = NetworkUser.readOnlyInstancesList[i];
					if (networkUser && networkUser.isParticipating)
					{
						networkUser.AwardLunarCoins(5U);
					}
				}
			}
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

		// Token: 0x060011E9 RID: 4585 RVA: 0x0004E112 File Offset: 0x0004C312
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

		// Token: 0x14000036 RID: 54
		// (add) Token: 0x060011EA RID: 4586 RVA: 0x0004E134 File Offset: 0x0004C334
		// (remove) Token: 0x060011EB RID: 4587 RVA: 0x0004E168 File Offset: 0x0004C368
		public static event Action<Run, RunReport> onClientGameOverGlobal;

		// Token: 0x060011EC RID: 4588 RVA: 0x0000409B File Offset: 0x0000229B
		public virtual void OverrideRuleChoices(RuleChoiceMask mustInclude, RuleChoiceMask mustExclude)
		{
		}

		// Token: 0x060011ED RID: 4589 RVA: 0x0004E19C File Offset: 0x0004C39C
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

		// Token: 0x060011EE RID: 4590 RVA: 0x0004E224 File Offset: 0x0004C424
		protected void ForceChoice(RuleChoiceMask mustInclude, RuleChoiceMask mustExclude, string choiceDefGlobalName)
		{
			this.ForceChoice(mustInclude, mustExclude, RuleCatalog.FindChoiceDef(choiceDefGlobalName));
		}

		// Token: 0x14000037 RID: 55
		// (add) Token: 0x060011EF RID: 4591 RVA: 0x0004E234 File Offset: 0x0004C434
		// (remove) Token: 0x060011F0 RID: 4592 RVA: 0x0004E268 File Offset: 0x0004C468
		public static event Action<Run> onRunStartGlobal;

		// Token: 0x14000038 RID: 56
		// (add) Token: 0x060011F1 RID: 4593 RVA: 0x0004E29C File Offset: 0x0004C49C
		// (remove) Token: 0x060011F2 RID: 4594 RVA: 0x0004E2D0 File Offset: 0x0004C4D0
		public static event Action<Run> onRunDestroyGlobal;

		// Token: 0x060011F3 RID: 4595 RVA: 0x0004E303 File Offset: 0x0004C503
		[Server]
		public void SetEventFlag(string name)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Run::SetEventFlag(System.String)' called on client");
				return;
			}
			this.eventFlags.Add(name);
		}

		// Token: 0x060011F4 RID: 4596 RVA: 0x0004E327 File Offset: 0x0004C527
		[Server]
		public bool GetEventFlag(string name)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Boolean RoR2.Run::GetEventFlag(System.String)' called on client");
				return false;
			}
			return this.eventFlags.Contains(name);
		}

		// Token: 0x060011F5 RID: 4597 RVA: 0x0004E34B File Offset: 0x0004C54B
		[Server]
		public void ResetEventFlag(string name)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.Run::ResetEventFlag(System.String)' called on client");
				return;
			}
			this.eventFlags.Remove(name);
		}

		// Token: 0x060011F6 RID: 4598 RVA: 0x0004E36F File Offset: 0x0004C56F
		public virtual bool ShouldAllowNonChampionBossSpawn()
		{
			return this.stageClearCount > 0;
		}

		// Token: 0x060011FA RID: 4602 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x17000228 RID: 552
		// (get) Token: 0x060011FB RID: 4603 RVA: 0x0004E48C File Offset: 0x0004C68C
		// (set) Token: 0x060011FC RID: 4604 RVA: 0x0004E49F File Offset: 0x0004C69F
		public ItemMask NetworkavailableItems
		{
			get
			{
				return this.availableItems;
			}
			[param: In]
			set
			{
				base.SetSyncVar<ItemMask>(value, ref this.availableItems, 1U);
			}
		}

		// Token: 0x17000229 RID: 553
		// (get) Token: 0x060011FD RID: 4605 RVA: 0x0004E4B4 File Offset: 0x0004C6B4
		// (set) Token: 0x060011FE RID: 4606 RVA: 0x0004E4C7 File Offset: 0x0004C6C7
		public EquipmentMask NetworkavailableEquipment
		{
			get
			{
				return this.availableEquipment;
			}
			[param: In]
			set
			{
				base.SetSyncVar<EquipmentMask>(value, ref this.availableEquipment, 2U);
			}
		}

		// Token: 0x1700022A RID: 554
		// (get) Token: 0x060011FF RID: 4607 RVA: 0x0004E4DC File Offset: 0x0004C6DC
		// (set) Token: 0x06001200 RID: 4608 RVA: 0x0004E4EF File Offset: 0x0004C6EF
		public ArtifactMask NetworkenabledArtifacts
		{
			get
			{
				return this.enabledArtifacts;
			}
			[param: In]
			set
			{
				base.SetSyncVar<ArtifactMask>(value, ref this.enabledArtifacts, 4U);
			}
		}

		// Token: 0x1700022B RID: 555
		// (get) Token: 0x06001201 RID: 4609 RVA: 0x0004E504 File Offset: 0x0004C704
		// (set) Token: 0x06001202 RID: 4610 RVA: 0x0004E517 File Offset: 0x0004C717
		public float NetworkfixedTime
		{
			get
			{
				return this.fixedTime;
			}
			[param: In]
			set
			{
				base.SetSyncVar<float>(value, ref this.fixedTime, 8U);
			}
		}

		// Token: 0x1700022C RID: 556
		// (get) Token: 0x06001203 RID: 4611 RVA: 0x0004E52C File Offset: 0x0004C72C
		// (set) Token: 0x06001204 RID: 4612 RVA: 0x0004E53F File Offset: 0x0004C73F
		public Run.RunStopwatch NetworkrunStopwatch
		{
			get
			{
				return this.runStopwatch;
			}
			[param: In]
			set
			{
				base.SetSyncVar<Run.RunStopwatch>(value, ref this.runStopwatch, 16U);
			}
		}

		// Token: 0x1700022D RID: 557
		// (get) Token: 0x06001205 RID: 4613 RVA: 0x0004E554 File Offset: 0x0004C754
		// (set) Token: 0x06001206 RID: 4614 RVA: 0x0004E567 File Offset: 0x0004C767
		public int NetworkstageClearCount
		{
			get
			{
				return this.stageClearCount;
			}
			[param: In]
			set
			{
				base.SetSyncVar<int>(value, ref this.stageClearCount, 32U);
			}
		}

		// Token: 0x1700022E RID: 558
		// (get) Token: 0x06001207 RID: 4615 RVA: 0x0004E57C File Offset: 0x0004C77C
		// (set) Token: 0x06001208 RID: 4616 RVA: 0x0004E58F File Offset: 0x0004C78F
		public int NetworkselectedDifficultyInternal
		{
			get
			{
				return this.selectedDifficultyInternal;
			}
			[param: In]
			set
			{
				base.SetSyncVar<int>(value, ref this.selectedDifficultyInternal, 64U);
			}
		}

		// Token: 0x06001209 RID: 4617 RVA: 0x0004E5A4 File Offset: 0x0004C7A4
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				GeneratedNetworkCode._WriteItemMask_None(writer, this.availableItems);
				GeneratedNetworkCode._WriteEquipmentMask_None(writer, this.availableEquipment);
				GeneratedNetworkCode._WriteArtifactMask_None(writer, this.enabledArtifacts);
				writer.Write(this.fixedTime);
				GeneratedNetworkCode._WriteRunStopwatch_Run(writer, this.runStopwatch);
				writer.WritePackedUInt32((uint)this.stageClearCount);
				writer.WritePackedUInt32((uint)this.selectedDifficultyInternal);
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
				GeneratedNetworkCode._WriteItemMask_None(writer, this.availableItems);
			}
			if ((base.syncVarDirtyBits & 2U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				GeneratedNetworkCode._WriteEquipmentMask_None(writer, this.availableEquipment);
			}
			if ((base.syncVarDirtyBits & 4U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				GeneratedNetworkCode._WriteArtifactMask_None(writer, this.enabledArtifacts);
			}
			if ((base.syncVarDirtyBits & 8U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.fixedTime);
			}
			if ((base.syncVarDirtyBits & 16U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				GeneratedNetworkCode._WriteRunStopwatch_Run(writer, this.runStopwatch);
			}
			if ((base.syncVarDirtyBits & 32U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32((uint)this.stageClearCount);
			}
			if ((base.syncVarDirtyBits & 64U) != 0U)
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

		// Token: 0x0600120A RID: 4618 RVA: 0x0004E78C File Offset: 0x0004C98C
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.availableItems = GeneratedNetworkCode._ReadItemMask_None(reader);
				this.availableEquipment = GeneratedNetworkCode._ReadEquipmentMask_None(reader);
				this.enabledArtifacts = GeneratedNetworkCode._ReadArtifactMask_None(reader);
				this.fixedTime = reader.ReadSingle();
				this.runStopwatch = GeneratedNetworkCode._ReadRunStopwatch_Run(reader);
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
				this.runStopwatch = GeneratedNetworkCode._ReadRunStopwatch_Run(reader);
			}
			if ((num & 32) != 0)
			{
				this.stageClearCount = (int)reader.ReadPackedUInt32();
			}
			if ((num & 64) != 0)
			{
				this.selectedDifficultyInternal = (int)reader.ReadPackedUInt32();
			}
		}

		// Token: 0x04001104 RID: 4356
		private NetworkRuleBook networkRuleBookComponent;

		// Token: 0x04001105 RID: 4357
		public string nameToken = "";

		// Token: 0x04001106 RID: 4358
		[Tooltip("The pool of scenes to select the first scene of the run from.")]
		public SceneDef[] startingScenes = Array.Empty<SceneDef>();

		// Token: 0x04001107 RID: 4359
		[SyncVar]
		public ItemMask availableItems;

		// Token: 0x04001108 RID: 4360
		[SyncVar]
		public EquipmentMask availableEquipment;

		// Token: 0x04001109 RID: 4361
		[SyncVar]
		public ArtifactMask enabledArtifacts;

		// Token: 0x0400110A RID: 4362
		[SyncVar]
		public float fixedTime;

		// Token: 0x0400110B RID: 4363
		public float time;

		// Token: 0x0400110C RID: 4364
		[SyncVar]
		private Run.RunStopwatch runStopwatch;

		// Token: 0x0400110D RID: 4365
		[SyncVar]
		public int stageClearCount;

		// Token: 0x0400110E RID: 4366
		public SceneDef nextStageScene;

		// Token: 0x0400110F RID: 4367
		public ulong seed;

		// Token: 0x04001110 RID: 4368
		public Xoroshiro128Plus runRNG;

		// Token: 0x04001111 RID: 4369
		public Xoroshiro128Plus nextStageRng;

		// Token: 0x04001112 RID: 4370
		public Xoroshiro128Plus stageRngGenerator;

		// Token: 0x04001113 RID: 4371
		public Xoroshiro128Plus stageRng;

		// Token: 0x04001114 RID: 4372
		public Xoroshiro128Plus bossRewardRng;

		// Token: 0x04001115 RID: 4373
		public Xoroshiro128Plus treasureRng;

		// Token: 0x04001116 RID: 4374
		public Xoroshiro128Plus spawnRng;

		// Token: 0x04001117 RID: 4375
		public float difficultyCoefficient = 1f;

		// Token: 0x04001118 RID: 4376
		public float compensatedDifficultyCoefficient = 1f;

		// Token: 0x04001119 RID: 4377
		[SyncVar]
		private int selectedDifficultyInternal = 1;

		// Token: 0x0400111D RID: 4381
		public int shopPortalCount;

		// Token: 0x0400111E RID: 4382
		private static readonly StringConVar cvRunSceneOverride = new StringConVar("run_scene_override", ConVarFlags.Cheat, "", "Overrides the first scene to enter in a run.");

		// Token: 0x0400111F RID: 4383
		private readonly HashSet<string> unlockablesUnlockedByAnyUser = new HashSet<string>();

		// Token: 0x04001120 RID: 4384
		private readonly HashSet<string> unlockablesUnlockedByAllUsers = new HashSet<string>();

		// Token: 0x04001121 RID: 4385
		private readonly HashSet<string> unlockablesAlreadyFullyObtained = new HashSet<string>();

		// Token: 0x04001122 RID: 4386
		private static SceneDef[] validStages;

		// Token: 0x04001123 RID: 4387
		private bool shutdown;

		// Token: 0x04001124 RID: 4388
		private Dictionary<NetworkUserId, CharacterMaster> userMasters = new Dictionary<NetworkUserId, CharacterMaster>();

		// Token: 0x04001125 RID: 4389
		private bool allowNewParticipants;

		// Token: 0x04001127 RID: 4391
		public readonly List<PickupIndex> availableTier1DropList = new List<PickupIndex>();

		// Token: 0x04001128 RID: 4392
		public readonly List<PickupIndex> availableTier2DropList = new List<PickupIndex>();

		// Token: 0x04001129 RID: 4393
		public readonly List<PickupIndex> availableTier3DropList = new List<PickupIndex>();

		// Token: 0x0400112A RID: 4394
		public readonly List<PickupIndex> availableLunarDropList = new List<PickupIndex>();

		// Token: 0x0400112B RID: 4395
		public readonly List<PickupIndex> availableEquipmentDropList = new List<PickupIndex>();

		// Token: 0x0400112C RID: 4396
		public readonly List<PickupIndex> availableBossDropList = new List<PickupIndex>();

		// Token: 0x0400112D RID: 4397
		public WeightedSelection<List<PickupIndex>> smallChestDropTierSelector = new WeightedSelection<List<PickupIndex>>(8);

		// Token: 0x0400112E RID: 4398
		public WeightedSelection<List<PickupIndex>> mediumChestDropTierSelector = new WeightedSelection<List<PickupIndex>>(8);

		// Token: 0x0400112F RID: 4399
		public WeightedSelection<List<PickupIndex>> largeChestDropTierSelector = new WeightedSelection<List<PickupIndex>>(8);

		// Token: 0x04001135 RID: 4405
		private readonly HashSet<string> eventFlags = new HashSet<string>();

		// Token: 0x02000306 RID: 774
		[Serializable]
		public struct RunStopwatch : IEquatable<Run.RunStopwatch>
		{
			// Token: 0x0600120B RID: 4619 RVA: 0x0004E8AB File Offset: 0x0004CAAB
			public bool Equals(Run.RunStopwatch other)
			{
				return this.offsetFromFixedTime.Equals(other.offsetFromFixedTime) && this.isPaused == other.isPaused;
			}

			// Token: 0x0600120C RID: 4620 RVA: 0x0004E8D0 File Offset: 0x0004CAD0
			public override bool Equals(object obj)
			{
				if (obj == null)
				{
					return false;
				}
				if (obj is Run.RunStopwatch)
				{
					Run.RunStopwatch other = (Run.RunStopwatch)obj;
					return this.Equals(other);
				}
				return false;
			}

			// Token: 0x0600120D RID: 4621 RVA: 0x0004E8FC File Offset: 0x0004CAFC
			public override int GetHashCode()
			{
				return this.offsetFromFixedTime.GetHashCode() * 397 ^ this.isPaused.GetHashCode();
			}

			// Token: 0x04001136 RID: 4406
			public float offsetFromFixedTime;

			// Token: 0x04001137 RID: 4407
			public bool isPaused;
		}

		// Token: 0x02000307 RID: 775
		[Serializable]
		public struct TimeStamp : IEquatable<Run.TimeStamp>, IComparable<Run.TimeStamp>
		{
			// Token: 0x1700022F RID: 559
			// (get) Token: 0x0600120E RID: 4622 RVA: 0x0004E91B File Offset: 0x0004CB1B
			public float timeUntil
			{
				get
				{
					return this.t - Run.TimeStamp.tNow;
				}
			}

			// Token: 0x17000230 RID: 560
			// (get) Token: 0x0600120F RID: 4623 RVA: 0x0004E929 File Offset: 0x0004CB29
			public float timeSince
			{
				get
				{
					return Run.TimeStamp.tNow - this.t;
				}
			}

			// Token: 0x17000231 RID: 561
			// (get) Token: 0x06001210 RID: 4624 RVA: 0x0004E937 File Offset: 0x0004CB37
			public float timeUntilClamped
			{
				get
				{
					return Mathf.Max(this.timeUntil, 0f);
				}
			}

			// Token: 0x17000232 RID: 562
			// (get) Token: 0x06001211 RID: 4625 RVA: 0x0004E949 File Offset: 0x0004CB49
			public float timeSinceClamped
			{
				get
				{
					return Mathf.Max(this.timeSince, 0f);
				}
			}

			// Token: 0x17000233 RID: 563
			// (get) Token: 0x06001212 RID: 4626 RVA: 0x0004E95B File Offset: 0x0004CB5B
			public bool hasPassed
			{
				get
				{
					return this.t <= Run.TimeStamp.tNow;
				}
			}

			// Token: 0x06001213 RID: 4627 RVA: 0x0004E970 File Offset: 0x0004CB70
			public override int GetHashCode()
			{
				return this.t.GetHashCode();
			}

			// Token: 0x17000234 RID: 564
			// (get) Token: 0x06001214 RID: 4628 RVA: 0x0004E98B File Offset: 0x0004CB8B
			public bool isInfinity
			{
				get
				{
					return float.IsInfinity(this.t);
				}
			}

			// Token: 0x17000235 RID: 565
			// (get) Token: 0x06001215 RID: 4629 RVA: 0x0004E998 File Offset: 0x0004CB98
			public bool isPositiveInfinity
			{
				get
				{
					return float.IsPositiveInfinity(this.t);
				}
			}

			// Token: 0x17000236 RID: 566
			// (get) Token: 0x06001216 RID: 4630 RVA: 0x0004E9A5 File Offset: 0x0004CBA5
			public bool isNegativeInfinity
			{
				get
				{
					return float.IsNegativeInfinity(this.t);
				}
			}

			// Token: 0x06001217 RID: 4631 RVA: 0x0004E9B2 File Offset: 0x0004CBB2
			public static void Update()
			{
				Run.TimeStamp.tNow = Run.instance.time;
			}

			// Token: 0x17000237 RID: 567
			// (get) Token: 0x06001218 RID: 4632 RVA: 0x0004E9C3 File Offset: 0x0004CBC3
			public static Run.TimeStamp now
			{
				get
				{
					return new Run.TimeStamp(Run.TimeStamp.tNow);
				}
			}

			// Token: 0x06001219 RID: 4633 RVA: 0x0004E9CF File Offset: 0x0004CBCF
			private TimeStamp(float t)
			{
				this.t = t;
			}

			// Token: 0x0600121A RID: 4634 RVA: 0x0004E9D8 File Offset: 0x0004CBD8
			public bool Equals(Run.TimeStamp other)
			{
				return this.t.Equals(other.t);
			}

			// Token: 0x0600121B RID: 4635 RVA: 0x0004E9F9 File Offset: 0x0004CBF9
			public override bool Equals(object obj)
			{
				return obj is Run.TimeStamp && this.Equals((Run.TimeStamp)obj);
			}

			// Token: 0x0600121C RID: 4636 RVA: 0x0004EA14 File Offset: 0x0004CC14
			public int CompareTo(Run.TimeStamp other)
			{
				return this.t.CompareTo(other.t);
			}

			// Token: 0x0600121D RID: 4637 RVA: 0x0004EA35 File Offset: 0x0004CC35
			public static Run.TimeStamp operator +(Run.TimeStamp a, float b)
			{
				return new Run.TimeStamp(a.t + b);
			}

			// Token: 0x0600121E RID: 4638 RVA: 0x0004EA44 File Offset: 0x0004CC44
			public static Run.TimeStamp operator -(Run.TimeStamp a, float b)
			{
				return new Run.TimeStamp(a.t - b);
			}

			// Token: 0x0600121F RID: 4639 RVA: 0x0004EA53 File Offset: 0x0004CC53
			public static float operator -(Run.TimeStamp a, Run.TimeStamp b)
			{
				return a.t - b.t;
			}

			// Token: 0x06001220 RID: 4640 RVA: 0x0004EA62 File Offset: 0x0004CC62
			public static bool operator <(Run.TimeStamp a, Run.TimeStamp b)
			{
				return a.t < b.t;
			}

			// Token: 0x06001221 RID: 4641 RVA: 0x0004EA72 File Offset: 0x0004CC72
			public static bool operator >(Run.TimeStamp a, Run.TimeStamp b)
			{
				return a.t > b.t;
			}

			// Token: 0x06001222 RID: 4642 RVA: 0x0004EA82 File Offset: 0x0004CC82
			public static bool operator <=(Run.TimeStamp a, Run.TimeStamp b)
			{
				return a.t <= b.t;
			}

			// Token: 0x06001223 RID: 4643 RVA: 0x0004EA95 File Offset: 0x0004CC95
			public static bool operator >=(Run.TimeStamp a, Run.TimeStamp b)
			{
				return a.t >= b.t;
			}

			// Token: 0x06001224 RID: 4644 RVA: 0x0004EAA8 File Offset: 0x0004CCA8
			public static bool operator ==(Run.TimeStamp a, Run.TimeStamp b)
			{
				return a.Equals(b);
			}

			// Token: 0x06001225 RID: 4645 RVA: 0x0004EAB2 File Offset: 0x0004CCB2
			public static bool operator !=(Run.TimeStamp a, Run.TimeStamp b)
			{
				return !a.Equals(b);
			}

			// Token: 0x06001226 RID: 4646 RVA: 0x0004EABF File Offset: 0x0004CCBF
			public static Run.TimeStamp Deserialize(NetworkReader reader)
			{
				return new Run.TimeStamp(reader.ReadSingle());
			}

			// Token: 0x06001227 RID: 4647 RVA: 0x0004EACC File Offset: 0x0004CCCC
			public static void Serialize(NetworkWriter writer, Run.TimeStamp timeStamp)
			{
				writer.Write(timeStamp.t);
			}

			// Token: 0x06001228 RID: 4648 RVA: 0x0004EADB File Offset: 0x0004CCDB
			public static void ToXml(XElement element, Run.TimeStamp src)
			{
				element.Value = TextSerialization.ToStringInvariant(src.t);
			}

			// Token: 0x06001229 RID: 4649 RVA: 0x0004EAF0 File Offset: 0x0004CCF0
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

			// Token: 0x0600122A RID: 4650 RVA: 0x0004EB1B File Offset: 0x0004CD1B
			[RuntimeInitializeOnLoadMethod]
			private static void Init()
			{
				HGXml.Register<Run.TimeStamp>(new HGXml.Serializer<Run.TimeStamp>(Run.TimeStamp.ToXml), new HGXml.Deserializer<Run.TimeStamp>(Run.TimeStamp.FromXml));
			}

			// Token: 0x04001138 RID: 4408
			public readonly float t;

			// Token: 0x04001139 RID: 4409
			private static float tNow;

			// Token: 0x0400113A RID: 4410
			public static readonly Run.TimeStamp zero = new Run.TimeStamp(0f);

			// Token: 0x0400113B RID: 4411
			public static readonly Run.TimeStamp positiveInfinity = new Run.TimeStamp(float.PositiveInfinity);

			// Token: 0x0400113C RID: 4412
			public static readonly Run.TimeStamp negativeInfinity = new Run.TimeStamp(float.NegativeInfinity);
		}

		// Token: 0x02000308 RID: 776
		[Serializable]
		public struct FixedTimeStamp : IEquatable<Run.FixedTimeStamp>, IComparable<Run.FixedTimeStamp>
		{
			// Token: 0x17000238 RID: 568
			// (get) Token: 0x0600122C RID: 4652 RVA: 0x0004EB69 File Offset: 0x0004CD69
			public float timeUntil
			{
				get
				{
					return this.t - Run.FixedTimeStamp.tNow;
				}
			}

			// Token: 0x17000239 RID: 569
			// (get) Token: 0x0600122D RID: 4653 RVA: 0x0004EB77 File Offset: 0x0004CD77
			public float timeSince
			{
				get
				{
					return Run.FixedTimeStamp.tNow - this.t;
				}
			}

			// Token: 0x1700023A RID: 570
			// (get) Token: 0x0600122E RID: 4654 RVA: 0x0004EB85 File Offset: 0x0004CD85
			public float timeUntilClamped
			{
				get
				{
					return Mathf.Max(this.timeUntil, 0f);
				}
			}

			// Token: 0x1700023B RID: 571
			// (get) Token: 0x0600122F RID: 4655 RVA: 0x0004EB97 File Offset: 0x0004CD97
			public float timeSinceClamped
			{
				get
				{
					return Mathf.Max(this.timeSince, 0f);
				}
			}

			// Token: 0x1700023C RID: 572
			// (get) Token: 0x06001230 RID: 4656 RVA: 0x0004EBA9 File Offset: 0x0004CDA9
			public bool hasPassed
			{
				get
				{
					return this.t <= Run.FixedTimeStamp.tNow;
				}
			}

			// Token: 0x06001231 RID: 4657 RVA: 0x0004EBBC File Offset: 0x0004CDBC
			public override int GetHashCode()
			{
				return this.t.GetHashCode();
			}

			// Token: 0x1700023D RID: 573
			// (get) Token: 0x06001232 RID: 4658 RVA: 0x0004EBD7 File Offset: 0x0004CDD7
			public bool isInfinity
			{
				get
				{
					return float.IsInfinity(this.t);
				}
			}

			// Token: 0x1700023E RID: 574
			// (get) Token: 0x06001233 RID: 4659 RVA: 0x0004EBE4 File Offset: 0x0004CDE4
			public bool isPositiveInfinity
			{
				get
				{
					return float.IsPositiveInfinity(this.t);
				}
			}

			// Token: 0x1700023F RID: 575
			// (get) Token: 0x06001234 RID: 4660 RVA: 0x0004EBF1 File Offset: 0x0004CDF1
			public bool isNegativeInfinity
			{
				get
				{
					return float.IsNegativeInfinity(this.t);
				}
			}

			// Token: 0x06001235 RID: 4661 RVA: 0x0004EBFE File Offset: 0x0004CDFE
			public static void Update()
			{
				Run.FixedTimeStamp.tNow = Run.instance.fixedTime;
			}

			// Token: 0x17000240 RID: 576
			// (get) Token: 0x06001236 RID: 4662 RVA: 0x0004EC0F File Offset: 0x0004CE0F
			public static Run.FixedTimeStamp now
			{
				get
				{
					return new Run.FixedTimeStamp(Run.FixedTimeStamp.tNow);
				}
			}

			// Token: 0x06001237 RID: 4663 RVA: 0x0004EC1B File Offset: 0x0004CE1B
			private FixedTimeStamp(float t)
			{
				this.t = t;
			}

			// Token: 0x06001238 RID: 4664 RVA: 0x0004EC24 File Offset: 0x0004CE24
			public bool Equals(Run.FixedTimeStamp other)
			{
				return this.t.Equals(other.t);
			}

			// Token: 0x06001239 RID: 4665 RVA: 0x0004EC45 File Offset: 0x0004CE45
			public override bool Equals(object obj)
			{
				return obj is Run.FixedTimeStamp && this.Equals((Run.FixedTimeStamp)obj);
			}

			// Token: 0x0600123A RID: 4666 RVA: 0x0004EC60 File Offset: 0x0004CE60
			public int CompareTo(Run.FixedTimeStamp other)
			{
				return this.t.CompareTo(other.t);
			}

			// Token: 0x0600123B RID: 4667 RVA: 0x0004EC81 File Offset: 0x0004CE81
			public static Run.FixedTimeStamp operator +(Run.FixedTimeStamp a, float b)
			{
				return new Run.FixedTimeStamp(a.t + b);
			}

			// Token: 0x0600123C RID: 4668 RVA: 0x0004EC90 File Offset: 0x0004CE90
			public static Run.FixedTimeStamp operator -(Run.FixedTimeStamp a, float b)
			{
				return new Run.FixedTimeStamp(a.t - b);
			}

			// Token: 0x0600123D RID: 4669 RVA: 0x0004EC9F File Offset: 0x0004CE9F
			public static float operator -(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return a.t - b.t;
			}

			// Token: 0x0600123E RID: 4670 RVA: 0x0004ECAE File Offset: 0x0004CEAE
			public static bool operator <(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return a.t < b.t;
			}

			// Token: 0x0600123F RID: 4671 RVA: 0x0004ECBE File Offset: 0x0004CEBE
			public static bool operator >(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return a.t > b.t;
			}

			// Token: 0x06001240 RID: 4672 RVA: 0x0004ECCE File Offset: 0x0004CECE
			public static bool operator <=(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return a.t <= b.t;
			}

			// Token: 0x06001241 RID: 4673 RVA: 0x0004ECE1 File Offset: 0x0004CEE1
			public static bool operator >=(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return a.t >= b.t;
			}

			// Token: 0x06001242 RID: 4674 RVA: 0x0004ECF4 File Offset: 0x0004CEF4
			public static bool operator ==(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return a.Equals(b);
			}

			// Token: 0x06001243 RID: 4675 RVA: 0x0004ECFE File Offset: 0x0004CEFE
			public static bool operator !=(Run.FixedTimeStamp a, Run.FixedTimeStamp b)
			{
				return !a.Equals(b);
			}

			// Token: 0x06001244 RID: 4676 RVA: 0x0004ED0B File Offset: 0x0004CF0B
			public static Run.FixedTimeStamp Deserialize(NetworkReader reader)
			{
				return new Run.FixedTimeStamp(reader.ReadSingle());
			}

			// Token: 0x06001245 RID: 4677 RVA: 0x0004ED18 File Offset: 0x0004CF18
			public static void Serialize(NetworkWriter writer, Run.FixedTimeStamp timeStamp)
			{
				writer.Write(timeStamp.t);
			}

			// Token: 0x06001246 RID: 4678 RVA: 0x0004ED27 File Offset: 0x0004CF27
			public static void ToXml(XElement element, Run.FixedTimeStamp src)
			{
				element.Value = TextSerialization.ToStringInvariant(src.t);
			}

			// Token: 0x06001247 RID: 4679 RVA: 0x0004ED3C File Offset: 0x0004CF3C
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

			// Token: 0x0400113D RID: 4413
			public readonly float t;

			// Token: 0x0400113E RID: 4414
			private static float tNow;

			// Token: 0x0400113F RID: 4415
			public static readonly Run.FixedTimeStamp zero = new Run.FixedTimeStamp(0f);

			// Token: 0x04001140 RID: 4416
			public static readonly Run.FixedTimeStamp positiveInfinity = new Run.FixedTimeStamp(float.PositiveInfinity);

			// Token: 0x04001141 RID: 4417
			public static readonly Run.FixedTimeStamp negativeInfinity = new Run.FixedTimeStamp(float.NegativeInfinity);
		}
	}
}
