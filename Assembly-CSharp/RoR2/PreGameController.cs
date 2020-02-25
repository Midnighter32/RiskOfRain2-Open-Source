using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using RoR2.ConVar;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002CA RID: 714
	[RequireComponent(typeof(NetworkRuleBook))]
	[RequireComponent(typeof(NetworkRuleChoiceMask))]
	public class PreGameController : NetworkBehaviour
	{
		// Token: 0x17000201 RID: 513
		// (get) Token: 0x0600102A RID: 4138 RVA: 0x0004710B File Offset: 0x0004530B
		// (set) Token: 0x0600102B RID: 4139 RVA: 0x00047112 File Offset: 0x00045312
		public static PreGameController instance { get; private set; }

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x0600102C RID: 4140 RVA: 0x0004711A File Offset: 0x0004531A
		public RuleChoiceMask resolvedRuleChoiceMask
		{
			get
			{
				return this.networkRuleChoiceMaskComponent.ruleChoiceMask;
			}
		}

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x0600102D RID: 4141 RVA: 0x00047127 File Offset: 0x00045327
		public RuleBook readOnlyRuleBook
		{
			get
			{
				return this.networkRuleBookComponent.ruleBook;
			}
		}

		// Token: 0x0600102E RID: 4142 RVA: 0x00047134 File Offset: 0x00045334
		private void Awake()
		{
			this.networkRuleChoiceMaskComponent = base.GetComponent<NetworkRuleChoiceMask>();
			this.networkRuleBookComponent = base.GetComponent<NetworkRuleBook>();
			this.ruleBookBuffer = new RuleBook();
			this.serverAvailableChoiceMask = new RuleChoiceMask();
			this.unlockedChoiceMask = new RuleChoiceMask();
			this.choiceMaskBuffer = new RuleChoiceMask();
			if (NetworkServer.active)
			{
				this.NetworkgameModeIndex = GameModeCatalog.FindGameModeIndex(PreGameController.GameModeConVar.instance.GetString());
				this.runSeed = RoR2Application.rng.nextUlong;
			}
			bool isInSinglePlayer = RoR2Application.isInSinglePlayer;
			for (int i = 0; i < this.serverAvailableChoiceMask.length; i++)
			{
				RuleChoiceDef choiceDef = RuleCatalog.GetChoiceDef(i);
				this.serverAvailableChoiceMask[i] = (isInSinglePlayer ? choiceDef.availableInSinglePlayer : choiceDef.availableInMultiPlayer);
			}
			NetworkUser.OnPostNetworkUserStart += this.GenerateRuleVoteController;
		}

		// Token: 0x0600102F RID: 4143 RVA: 0x00047202 File Offset: 0x00045402
		private void OnDestroy()
		{
			NetworkUser.OnPostNetworkUserStart -= this.GenerateRuleVoteController;
		}

		// Token: 0x06001030 RID: 4144 RVA: 0x00047215 File Offset: 0x00045415
		private void GenerateRuleVoteController(NetworkUser networkUser)
		{
			if (NetworkServer.active)
			{
				if (PreGameRuleVoteController.FindForUser(networkUser))
				{
					return;
				}
				PreGameRuleVoteController.CreateForNetworkUserServer(networkUser);
			}
		}

		// Token: 0x06001031 RID: 4145 RVA: 0x00047234 File Offset: 0x00045434
		private void Start()
		{
			if (NetworkServer.active)
			{
				this.ResolveChoiceMask();
				foreach (NetworkUser networkUser in NetworkUser.readOnlyInstancesList)
				{
					Debug.LogFormat("Attempting to generate PreGameVoteController for {0}", new object[]
					{
						networkUser.userName
					});
					this.GenerateRuleVoteController(networkUser);
				}
			}
		}

		// Token: 0x06001032 RID: 4146 RVA: 0x000472A8 File Offset: 0x000454A8
		[Server]
		public void ApplyChoice(int ruleChoiceIndex)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.PreGameController::ApplyChoice(System.Int32)' called on client");
				return;
			}
			if (!this.resolvedRuleChoiceMask[ruleChoiceIndex])
			{
				return;
			}
			RuleChoiceDef choiceDef = RuleCatalog.GetChoiceDef(ruleChoiceIndex);
			if (this.readOnlyRuleBook.GetRuleChoice(choiceDef.ruleDef.globalIndex) == choiceDef)
			{
				return;
			}
			this.ruleBookBuffer.Copy(this.readOnlyRuleBook);
			this.ruleBookBuffer.ApplyChoice(choiceDef);
			this.networkRuleBookComponent.SetRuleBook(this.ruleBookBuffer);
		}

		// Token: 0x06001033 RID: 4147 RVA: 0x00047328 File Offset: 0x00045528
		[Server]
		public void EnforceValidRuleChoices()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.PreGameController::EnforceValidRuleChoices()' called on client");
				return;
			}
			this.ruleBookBuffer.Copy(this.readOnlyRuleBook);
			for (int i = 0; i < RuleCatalog.ruleCount; i++)
			{
				if (!this.resolvedRuleChoiceMask[this.ruleBookBuffer.GetRuleChoice(i)])
				{
					RuleDef ruleDef = RuleCatalog.GetRuleDef(i);
					RuleChoiceDef choiceDef = ruleDef.choices[ruleDef.defaultChoiceIndex];
					int num = 0;
					int j = 0;
					int count = ruleDef.choices.Count;
					while (j < count)
					{
						if (this.resolvedRuleChoiceMask[ruleDef.choices[j]])
						{
							num++;
						}
						j++;
					}
					if (this.resolvedRuleChoiceMask[choiceDef] || num == 0)
					{
						this.ruleBookBuffer.ApplyChoice(choiceDef);
					}
					else
					{
						int k = 0;
						int count2 = ruleDef.choices.Count;
						while (k < count2)
						{
							if (this.resolvedRuleChoiceMask[ruleDef.choices[k]])
							{
								this.ruleBookBuffer.ApplyChoice(ruleDef.choices[k]);
								break;
							}
							k++;
						}
					}
				}
			}
			this.networkRuleBookComponent.SetRuleBook(this.ruleBookBuffer);
		}

		// Token: 0x06001034 RID: 4148 RVA: 0x00047468 File Offset: 0x00045668
		private void TestRuleValues()
		{
			RuleBook ruleBook = new RuleBook();
			ruleBook.Copy(this.networkRuleBookComponent.ruleBook);
			RuleDef ruleDef = RuleCatalog.GetRuleDef(UnityEngine.Random.Range(0, RuleCatalog.ruleCount));
			RuleChoiceDef choiceDef = ruleDef.choices[UnityEngine.Random.Range(0, ruleDef.choices.Count)];
			ruleBook.ApplyChoice(choiceDef);
			this.networkRuleBookComponent.SetRuleBook(ruleBook);
			base.Invoke("TestRuleValues", 0.5f);
		}

		// Token: 0x06001035 RID: 4149 RVA: 0x000474E0 File Offset: 0x000456E0
		private void OnEnable()
		{
			PreGameController.instance = SingletonHelper.Assign<PreGameController>(PreGameController.instance, this);
			if (NetworkServer.active)
			{
				this.RecalculateModifierAvailability();
			}
			NetworkUser.OnNetworkUserUnlockablesUpdated += this.OnNetworkUserUnlockablesUpdatedCallback;
			NetworkUser.OnPostNetworkUserStart += this.OnPostNetworkUserStartCallback;
			if (NetworkClient.active)
			{
				foreach (NetworkUser networkUser in NetworkUser.readOnlyLocalPlayersList)
				{
					networkUser.SendServerUnlockables();
				}
			}
		}

		// Token: 0x06001036 RID: 4150 RVA: 0x00047570 File Offset: 0x00045770
		private void OnDisable()
		{
			PreGameController.instance = SingletonHelper.Unassign<PreGameController>(PreGameController.instance, this);
			NetworkUser.OnNetworkUserUnlockablesUpdated -= this.OnNetworkUserUnlockablesUpdatedCallback;
			NetworkUser.OnPostNetworkUserStart -= this.OnPostNetworkUserStartCallback;
		}

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x06001037 RID: 4151 RVA: 0x000475A4 File Offset: 0x000457A4
		// (set) Token: 0x06001038 RID: 4152 RVA: 0x000475AC File Offset: 0x000457AC
		private PreGameController.PregameState pregameState
		{
			get
			{
				return (PreGameController.PregameState)this.pregameStateInternal;
			}
			set
			{
				this.NetworkpregameStateInternal = (int)value;
			}
		}

		// Token: 0x06001039 RID: 4153 RVA: 0x000475B5 File Offset: 0x000457B5
		public bool IsCharacterSwitchingCurrentlyAllowed()
		{
			return this.pregameState == PreGameController.PregameState.Idle;
		}

		// Token: 0x0600103A RID: 4154 RVA: 0x000475C0 File Offset: 0x000457C0
		private void Update()
		{
			if (this.pregameState == PreGameController.PregameState.Launching)
			{
				if (GameNetworkManager.singleton.unpredictedServerFixedTime - this.launchStartTime >= 0.5f && NetworkServer.active)
				{
					this.StartRun();
					return;
				}
			}
			else
			{
				PreGameController.PregameState pregameState = this.pregameState;
			}
		}

		// Token: 0x0600103B RID: 4155 RVA: 0x000475FA File Offset: 0x000457FA
		[Server]
		public void StartLaunch()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.PreGameController::StartLaunch()' called on client");
				return;
			}
			if (this.pregameState == PreGameController.PregameState.Idle)
			{
				this.pregameState = PreGameController.PregameState.Launching;
				this.NetworklaunchStartTime = GameNetworkManager.singleton.unpredictedServerFixedTime;
			}
		}

		// Token: 0x0600103C RID: 4156 RVA: 0x00047630 File Offset: 0x00045830
		[Server]
		private void StartRun()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.PreGameController::StartRun()' called on client");
				return;
			}
			this.pregameState = PreGameController.PregameState.Launched;
			NetworkSession.instance.BeginRun(PreGameController.GameModeConVar.instance.runPrefabComponent, this.readOnlyRuleBook, this.runSeed);
		}

		// Token: 0x0600103D RID: 4157 RVA: 0x0004766F File Offset: 0x0004586F
		[ConCommand(commandName = "pregame_start_run", flags = ConVarFlags.SenderMustBeServer, helpText = "Begins a run out of pregame.")]
		private static void CCPregameStartRun(ConCommandArgs args)
		{
			if (PreGameController.instance)
			{
				PreGameController.instance.StartRun();
			}
		}

		// Token: 0x0600103E RID: 4158 RVA: 0x00047688 File Offset: 0x00045888
		private static bool AnyUserHasUnlockable([NotNull] UnlockableDef unlockableDef)
		{
			ReadOnlyCollection<NetworkUser> readOnlyInstancesList = NetworkUser.readOnlyInstancesList;
			for (int i = 0; i < readOnlyInstancesList.Count; i++)
			{
				if (readOnlyInstancesList[i].unlockables.Contains(unlockableDef))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600103F RID: 4159 RVA: 0x000476C4 File Offset: 0x000458C4
		[Server]
		private void RecalculateModifierAvailability()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.PreGameController::RecalculateModifierAvailability()' called on client");
				return;
			}
			for (int i = 0; i < RuleCatalog.choiceCount; i++)
			{
				RuleChoiceDef choiceDef = RuleCatalog.GetChoiceDef(i);
				bool flag = string.IsNullOrEmpty(choiceDef.unlockableName);
				if (!flag)
				{
					UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(choiceDef.unlockableName);
					if (unlockableDef != null)
					{
						flag = PreGameController.AnyUserHasUnlockable(unlockableDef);
					}
				}
				this.unlockedChoiceMask[i] = flag;
			}
			this.ResolveChoiceMask();
			Action<PreGameController> action = PreGameController.onServerRecalculatedModifierAvailability;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x06001040 RID: 4160 RVA: 0x00047748 File Offset: 0x00045948
		[Server]
		private void ResolveChoiceMask()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.PreGameController::ResolveChoiceMask()' called on client");
				return;
			}
			RuleChoiceMask ruleChoiceMask = new RuleChoiceMask();
			RuleChoiceMask ruleChoiceMask2 = new RuleChoiceMask();
			Run gameModePrefabComponent = GameModeCatalog.GetGameModePrefabComponent(this.gameModeIndex);
			if (gameModePrefabComponent)
			{
				gameModePrefabComponent.OverrideRuleChoices(ruleChoiceMask, ruleChoiceMask2);
			}
			for (int i = 0; i < this.choiceMaskBuffer.length; i++)
			{
				RuleChoiceDef choiceDef = RuleCatalog.GetChoiceDef(i);
				this.choiceMaskBuffer[i] = (ruleChoiceMask[i] || (!ruleChoiceMask2[i] && this.serverAvailableChoiceMask[i] && this.unlockedChoiceMask[i] && !choiceDef.excludeByDefault));
			}
			this.networkRuleChoiceMaskComponent.SetRuleChoiceMask(this.choiceMaskBuffer);
			this.EnforceValidRuleChoices();
		}

		// Token: 0x06001041 RID: 4161 RVA: 0x00047811 File Offset: 0x00045A11
		private void OnNetworkUserUnlockablesUpdatedCallback(NetworkUser networkUser)
		{
			if (NetworkServer.active)
			{
				this.RecalculateModifierAvailability();
			}
		}

		// Token: 0x1400002C RID: 44
		// (add) Token: 0x06001042 RID: 4162 RVA: 0x00047820 File Offset: 0x00045A20
		// (remove) Token: 0x06001043 RID: 4163 RVA: 0x00047854 File Offset: 0x00045A54
		public static event Action<PreGameController> onServerRecalculatedModifierAvailability;

		// Token: 0x06001044 RID: 4164 RVA: 0x00047887 File Offset: 0x00045A87
		private void OnPostNetworkUserStartCallback(NetworkUser networkUser)
		{
			if (NetworkServer.active)
			{
				networkUser.ServerRequestUnlockables();
			}
		}

		// Token: 0x06001046 RID: 4166 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x17000205 RID: 517
		// (get) Token: 0x06001047 RID: 4167 RVA: 0x000478AC File Offset: 0x00045AAC
		// (set) Token: 0x06001048 RID: 4168 RVA: 0x000478BF File Offset: 0x00045ABF
		public int NetworkgameModeIndex
		{
			get
			{
				return this.gameModeIndex;
			}
			[param: In]
			set
			{
				base.SetSyncVar<int>(value, ref this.gameModeIndex, 1U);
			}
		}

		// Token: 0x17000206 RID: 518
		// (get) Token: 0x06001049 RID: 4169 RVA: 0x000478D4 File Offset: 0x00045AD4
		// (set) Token: 0x0600104A RID: 4170 RVA: 0x000478E7 File Offset: 0x00045AE7
		public int NetworkpregameStateInternal
		{
			get
			{
				return this.pregameStateInternal;
			}
			[param: In]
			set
			{
				base.SetSyncVar<int>(value, ref this.pregameStateInternal, 2U);
			}
		}

		// Token: 0x17000207 RID: 519
		// (get) Token: 0x0600104B RID: 4171 RVA: 0x000478FC File Offset: 0x00045AFC
		// (set) Token: 0x0600104C RID: 4172 RVA: 0x0004790F File Offset: 0x00045B0F
		public float NetworklaunchStartTime
		{
			get
			{
				return this.launchStartTime;
			}
			[param: In]
			set
			{
				base.SetSyncVar<float>(value, ref this.launchStartTime, 4U);
			}
		}

		// Token: 0x0600104D RID: 4173 RVA: 0x00047924 File Offset: 0x00045B24
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.WritePackedUInt32((uint)this.gameModeIndex);
				writer.WritePackedUInt32((uint)this.pregameStateInternal);
				writer.Write(this.launchStartTime);
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
				writer.WritePackedUInt32((uint)this.gameModeIndex);
			}
			if ((base.syncVarDirtyBits & 2U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32((uint)this.pregameStateInternal);
			}
			if ((base.syncVarDirtyBits & 4U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.launchStartTime);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x0600104E RID: 4174 RVA: 0x00047A10 File Offset: 0x00045C10
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.gameModeIndex = (int)reader.ReadPackedUInt32();
				this.pregameStateInternal = (int)reader.ReadPackedUInt32();
				this.launchStartTime = reader.ReadSingle();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.gameModeIndex = (int)reader.ReadPackedUInt32();
			}
			if ((num & 2) != 0)
			{
				this.pregameStateInternal = (int)reader.ReadPackedUInt32();
			}
			if ((num & 4) != 0)
			{
				this.launchStartTime = reader.ReadSingle();
			}
		}

		// Token: 0x04000FA9 RID: 4009
		private NetworkRuleChoiceMask networkRuleChoiceMaskComponent;

		// Token: 0x04000FAA RID: 4010
		private NetworkRuleBook networkRuleBookComponent;

		// Token: 0x04000FAB RID: 4011
		private RuleChoiceMask serverAvailableChoiceMask;

		// Token: 0x04000FAC RID: 4012
		public ulong runSeed;

		// Token: 0x04000FAD RID: 4013
		[SyncVar]
		public int gameModeIndex;

		// Token: 0x04000FAE RID: 4014
		private RuleBook ruleBookBuffer;

		// Token: 0x04000FAF RID: 4015
		[SyncVar]
		private int pregameStateInternal;

		// Token: 0x04000FB0 RID: 4016
		private const float launchTransitionDuration = 0f;

		// Token: 0x04000FB1 RID: 4017
		private GameObject gameModePrefab;

		// Token: 0x04000FB2 RID: 4018
		[SyncVar]
		private float launchStartTime = float.PositiveInfinity;

		// Token: 0x04000FB3 RID: 4019
		private RuleChoiceMask unlockedChoiceMask;

		// Token: 0x04000FB4 RID: 4020
		private RuleChoiceMask choiceMaskBuffer;

		// Token: 0x020002CB RID: 715
		private enum PregameState
		{
			// Token: 0x04000FB7 RID: 4023
			Idle,
			// Token: 0x04000FB8 RID: 4024
			Launching,
			// Token: 0x04000FB9 RID: 4025
			Launched
		}

		// Token: 0x020002CC RID: 716
		private class GameModeConVar : BaseConVar
		{
			// Token: 0x0600104F RID: 4175 RVA: 0x0000972B File Offset: 0x0000792B
			public GameModeConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x06001050 RID: 4176 RVA: 0x00047A9B File Offset: 0x00045C9B
			static GameModeConVar()
			{
				GameModeCatalog.availability.CallWhenAvailable(delegate
				{
					PreGameController.GameModeConVar.instance.runPrefabComponent = GameModeCatalog.FindGameModePrefabComponent(PreGameController.GameModeConVar.instance.GetString());
				});
			}

			// Token: 0x06001051 RID: 4177 RVA: 0x00047AD4 File Offset: 0x00045CD4
			public override void SetString(string newValue)
			{
				GameModeCatalog.availability.CallWhenAvailable(delegate
				{
					Run exists = GameModeCatalog.FindGameModePrefabComponent(newValue);
					if (!exists)
					{
						Debug.LogFormat("GameMode \"{0}\" does not exist.", new object[]
						{
							newValue
						});
						return;
					}
					this.runPrefabComponent = exists;
				});
			}

			// Token: 0x06001052 RID: 4178 RVA: 0x00047B0B File Offset: 0x00045D0B
			public override string GetString()
			{
				if (!this.runPrefabComponent)
				{
					return "ClassicRun";
				}
				return this.runPrefabComponent.gameObject.name;
			}

			// Token: 0x04000FBA RID: 4026
			public static readonly PreGameController.GameModeConVar instance = new PreGameController.GameModeConVar("gamemode", ConVarFlags.None, "ClassicRun", "Sets the specified game mode as the one to use in the next run.");

			// Token: 0x04000FBB RID: 4027
			public Run runPrefabComponent;
		}
	}
}
