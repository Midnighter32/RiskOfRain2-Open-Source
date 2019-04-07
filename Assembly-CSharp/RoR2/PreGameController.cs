using System;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using RoR2.ConVar;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200038E RID: 910
	[RequireComponent(typeof(NetworkRuleBook))]
	[RequireComponent(typeof(NetworkRuleChoiceMask))]
	public class PreGameController : NetworkBehaviour
	{
		// Token: 0x170001AD RID: 429
		// (get) Token: 0x06001314 RID: 4884 RVA: 0x0005D733 File Offset: 0x0005B933
		// (set) Token: 0x06001315 RID: 4885 RVA: 0x0005D73A File Offset: 0x0005B93A
		public static PreGameController instance { get; private set; }

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x06001316 RID: 4886 RVA: 0x0005D742 File Offset: 0x0005B942
		public RuleChoiceMask resolvedRuleChoiceMask
		{
			get
			{
				return this.networkRuleChoiceMaskComponent.ruleChoiceMask;
			}
		}

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x06001317 RID: 4887 RVA: 0x0005D74F File Offset: 0x0005B94F
		public RuleBook readOnlyRuleBook
		{
			get
			{
				return this.networkRuleBookComponent.ruleBook;
			}
		}

		// Token: 0x06001318 RID: 4888 RVA: 0x0005D75C File Offset: 0x0005B95C
		private void Awake()
		{
			this.networkRuleChoiceMaskComponent = base.GetComponent<NetworkRuleChoiceMask>();
			this.networkRuleBookComponent = base.GetComponent<NetworkRuleBook>();
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
			if (PreGameController.persistentRuleBook != null && NetworkServer.active)
			{
				this.networkRuleBookComponent.SetRuleBook(PreGameController.persistentRuleBook);
			}
			NetworkUser.OnPostNetworkUserStart += this.GenerateRuleVoteController;
		}

		// Token: 0x06001319 RID: 4889 RVA: 0x0005D81C File Offset: 0x0005BA1C
		private void OnDestroy()
		{
			NetworkUser.OnPostNetworkUserStart -= this.GenerateRuleVoteController;
		}

		// Token: 0x0600131A RID: 4890 RVA: 0x0005D830 File Offset: 0x0005BA30
		private void GenerateRuleVoteController(NetworkUser networkUser)
		{
			if (NetworkServer.active)
			{
				if (PreGameRuleVoteController.FindForUser(networkUser))
				{
					return;
				}
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/PreGameRuleVoteController"), base.transform);
				gameObject.GetComponent<PreGameRuleVoteController>().networkUserNetworkIdentity = networkUser.GetComponent<NetworkIdentity>();
				NetworkServer.Spawn(gameObject);
			}
		}

		// Token: 0x0600131B RID: 4891 RVA: 0x0005D880 File Offset: 0x0005BA80
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

		// Token: 0x0600131C RID: 4892 RVA: 0x0005D8F4 File Offset: 0x0005BAF4
		[Server]
		public void UpdatePersistentRulebook()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.PreGameController::UpdatePersistentRulebook()' called on client");
				return;
			}
			if (PreGameController.persistentRuleBook == null)
			{
				PreGameController.persistentRuleBook = new RuleBook();
			}
			PreGameController.persistentRuleBook.Copy(this.readOnlyRuleBook);
		}

		// Token: 0x0600131D RID: 4893 RVA: 0x0005D92C File Offset: 0x0005BB2C
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
			this.UpdatePersistentRulebook();
		}

		// Token: 0x0600131E RID: 4894 RVA: 0x0005D9B4 File Offset: 0x0005BBB4
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
			this.UpdatePersistentRulebook();
		}

		// Token: 0x0600131F RID: 4895 RVA: 0x0005DAF8 File Offset: 0x0005BCF8
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

		// Token: 0x06001320 RID: 4896 RVA: 0x0005DB70 File Offset: 0x0005BD70
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

		// Token: 0x06001321 RID: 4897 RVA: 0x0005DC00 File Offset: 0x0005BE00
		private void OnDisable()
		{
			PreGameController.instance = SingletonHelper.Unassign<PreGameController>(PreGameController.instance, this);
			NetworkUser.OnNetworkUserUnlockablesUpdated -= this.OnNetworkUserUnlockablesUpdatedCallback;
			NetworkUser.OnPostNetworkUserStart -= this.OnPostNetworkUserStartCallback;
		}

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x06001322 RID: 4898 RVA: 0x0005DC34 File Offset: 0x0005BE34
		// (set) Token: 0x06001323 RID: 4899 RVA: 0x0005DC3C File Offset: 0x0005BE3C
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

		// Token: 0x06001324 RID: 4900 RVA: 0x0005DC45 File Offset: 0x0005BE45
		public bool IsCharacterSwitchingCurrentlyAllowed()
		{
			return this.pregameState == PreGameController.PregameState.Idle;
		}

		// Token: 0x06001325 RID: 4901 RVA: 0x0005DC50 File Offset: 0x0005BE50
		private void Update()
		{
			if (this.pregameState == PreGameController.PregameState.Launching)
			{
				if (GameNetworkManager.singleton.unpredictedServerFixedTime - this.launchStartTime >= 0.5f)
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

		// Token: 0x06001326 RID: 4902 RVA: 0x0005DC83 File Offset: 0x0005BE83
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

		// Token: 0x06001327 RID: 4903 RVA: 0x0005DCBC File Offset: 0x0005BEBC
		[Server]
		private void StartRun()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.PreGameController::StartRun()' called on client");
				return;
			}
			this.pregameState = PreGameController.PregameState.Launched;
			Run run = NetworkSession.instance.BeginRun(PreGameController.GameModeConVar.instance.runPrefabComponent);
			run.SetRuleBook(this.readOnlyRuleBook);
			run.seed = this.runSeed;
		}

		// Token: 0x06001328 RID: 4904 RVA: 0x0005DD10 File Offset: 0x0005BF10
		[ConCommand(commandName = "pregame_start_run", flags = ConVarFlags.SenderMustBeServer, helpText = "Begins a run out of pregame.")]
		private static void CCPregameStartRun(ConCommandArgs args)
		{
			if (PreGameController.instance)
			{
				PreGameController.instance.StartRun();
			}
		}

		// Token: 0x06001329 RID: 4905 RVA: 0x0005DD28 File Offset: 0x0005BF28
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

		// Token: 0x0600132A RID: 4906 RVA: 0x0005DD64 File Offset: 0x0005BF64
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

		// Token: 0x0600132B RID: 4907 RVA: 0x0005DDE8 File Offset: 0x0005BFE8
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

		// Token: 0x0600132C RID: 4908 RVA: 0x0005DEB1 File Offset: 0x0005C0B1
		private void OnNetworkUserUnlockablesUpdatedCallback(NetworkUser networkUser)
		{
			if (NetworkServer.active)
			{
				this.RecalculateModifierAvailability();
			}
		}

		// Token: 0x1400001B RID: 27
		// (add) Token: 0x0600132D RID: 4909 RVA: 0x0005DEC0 File Offset: 0x0005C0C0
		// (remove) Token: 0x0600132E RID: 4910 RVA: 0x0005DEF4 File Offset: 0x0005C0F4
		public static event Action<PreGameController> onServerRecalculatedModifierAvailability;

		// Token: 0x0600132F RID: 4911 RVA: 0x0005DF27 File Offset: 0x0005C127
		private void OnPostNetworkUserStartCallback(NetworkUser networkUser)
		{
			if (NetworkServer.active)
			{
				networkUser.ServerRequestUnlockables();
			}
		}

		// Token: 0x06001332 RID: 4914 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x06001333 RID: 4915 RVA: 0x0005DF78 File Offset: 0x0005C178
		// (set) Token: 0x06001334 RID: 4916 RVA: 0x0005DF8B File Offset: 0x0005C18B
		public int NetworkgameModeIndex
		{
			get
			{
				return this.gameModeIndex;
			}
			set
			{
				base.SetSyncVar<int>(value, ref this.gameModeIndex, 1u);
			}
		}

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x06001335 RID: 4917 RVA: 0x0005DFA0 File Offset: 0x0005C1A0
		// (set) Token: 0x06001336 RID: 4918 RVA: 0x0005DFB3 File Offset: 0x0005C1B3
		public int NetworkpregameStateInternal
		{
			get
			{
				return this.pregameStateInternal;
			}
			set
			{
				base.SetSyncVar<int>(value, ref this.pregameStateInternal, 2u);
			}
		}

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x06001337 RID: 4919 RVA: 0x0005DFC8 File Offset: 0x0005C1C8
		// (set) Token: 0x06001338 RID: 4920 RVA: 0x0005DFDB File Offset: 0x0005C1DB
		public float NetworklaunchStartTime
		{
			get
			{
				return this.launchStartTime;
			}
			set
			{
				base.SetSyncVar<float>(value, ref this.launchStartTime, 4u);
			}
		}

		// Token: 0x06001339 RID: 4921 RVA: 0x0005DFF0 File Offset: 0x0005C1F0
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
			if ((base.syncVarDirtyBits & 1u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32((uint)this.gameModeIndex);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32((uint)this.pregameStateInternal);
			}
			if ((base.syncVarDirtyBits & 4u) != 0u)
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

		// Token: 0x0600133A RID: 4922 RVA: 0x0005E0DC File Offset: 0x0005C2DC
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

		// Token: 0x040016DB RID: 5851
		private NetworkRuleChoiceMask networkRuleChoiceMaskComponent;

		// Token: 0x040016DC RID: 5852
		private NetworkRuleBook networkRuleBookComponent;

		// Token: 0x040016DD RID: 5853
		private readonly RuleChoiceMask serverAvailableChoiceMask = new RuleChoiceMask();

		// Token: 0x040016DE RID: 5854
		public ulong runSeed;

		// Token: 0x040016DF RID: 5855
		[SyncVar]
		public int gameModeIndex;

		// Token: 0x040016E0 RID: 5856
		private readonly RuleBook ruleBookBuffer = new RuleBook();

		// Token: 0x040016E1 RID: 5857
		private static RuleBook persistentRuleBook;

		// Token: 0x040016E2 RID: 5858
		[SyncVar]
		private int pregameStateInternal;

		// Token: 0x040016E3 RID: 5859
		private const float launchTransitionDuration = 0f;

		// Token: 0x040016E4 RID: 5860
		private GameObject gameModePrefab;

		// Token: 0x040016E5 RID: 5861
		[SyncVar]
		private float launchStartTime = float.PositiveInfinity;

		// Token: 0x040016E6 RID: 5862
		private readonly RuleChoiceMask unlockedChoiceMask = new RuleChoiceMask();

		// Token: 0x040016E7 RID: 5863
		private readonly RuleChoiceMask choiceMaskBuffer = new RuleChoiceMask();

		// Token: 0x0200038F RID: 911
		private enum PregameState
		{
			// Token: 0x040016EA RID: 5866
			Idle,
			// Token: 0x040016EB RID: 5867
			Launching,
			// Token: 0x040016EC RID: 5868
			Launched
		}

		// Token: 0x02000390 RID: 912
		private class GameModeConVar : BaseConVar
		{
			// Token: 0x0600133B RID: 4923 RVA: 0x00037E38 File Offset: 0x00036038
			public GameModeConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
			{
			}

			// Token: 0x0600133C RID: 4924 RVA: 0x0005E167 File Offset: 0x0005C367
			static GameModeConVar()
			{
				GameModeCatalog.availability.CallWhenAvailable(delegate
				{
					PreGameController.GameModeConVar.instance.runPrefabComponent = GameModeCatalog.FindGameModePrefabComponent(PreGameController.GameModeConVar.instance.GetString());
				});
			}

			// Token: 0x0600133D RID: 4925 RVA: 0x0005E1A0 File Offset: 0x0005C3A0
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

			// Token: 0x0600133E RID: 4926 RVA: 0x0005E1D7 File Offset: 0x0005C3D7
			public override string GetString()
			{
				if (!this.runPrefabComponent)
				{
					return "ClassicRun";
				}
				return this.runPrefabComponent.gameObject.name;
			}

			// Token: 0x040016ED RID: 5869
			public static readonly PreGameController.GameModeConVar instance = new PreGameController.GameModeConVar("gamemode", ConVarFlags.None, "", "Sets the specified game mode as the one to use in the next run.");

			// Token: 0x040016EE RID: 5870
			public Run runPrefabComponent;
		}
	}
}
