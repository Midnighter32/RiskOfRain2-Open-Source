using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Rewired;
using RoR2.Networking;
using RoR2.Stats;
using Unity;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace RoR2
{
	// Token: 0x020002A2 RID: 674
	[RequireComponent(typeof(NetworkLoadout))]
	public class NetworkUser : NetworkBehaviour
	{
		// Token: 0x06000F12 RID: 3858 RVA: 0x00042994 File Offset: 0x00040B94
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Init()
		{
			UserProfile.onUnlockableGranted += delegate(UserProfile userProfile, string unlockableName)
			{
				if (NetworkClient.active)
				{
					foreach (NetworkUser networkUser in NetworkUser.readOnlyLocalPlayersList)
					{
						if (networkUser.localUser.userProfile == userProfile)
						{
							networkUser.SendServerUnlockables();
						}
					}
				}
			};
			UserProfile.onLoadoutChangedGlobal += delegate(UserProfile userProfile)
			{
				if (NetworkClient.active)
				{
					foreach (NetworkUser networkUser in NetworkUser.readOnlyLocalPlayersList)
					{
						if (networkUser.localUser.userProfile == userProfile)
						{
							networkUser.PullLoadoutFromUserProfile();
						}
					}
				}
			};
		}

		// Token: 0x06000F13 RID: 3859 RVA: 0x000429E9 File Offset: 0x00040BE9
		private void OnEnable()
		{
			NetworkUser.instancesList.Add(this);
			NetworkUser.NetworkUserGenericDelegate networkUserGenericDelegate = NetworkUser.onNetworkUserDiscovered;
			if (networkUserGenericDelegate == null)
			{
				return;
			}
			networkUserGenericDelegate(this);
		}

		// Token: 0x06000F14 RID: 3860 RVA: 0x00042A06 File Offset: 0x00040C06
		private void OnDisable()
		{
			NetworkUser.NetworkUserGenericDelegate networkUserGenericDelegate = NetworkUser.onNetworkUserLost;
			if (networkUserGenericDelegate != null)
			{
				networkUserGenericDelegate(this);
			}
			NetworkUser.instancesList.Remove(this);
		}

		// Token: 0x170001E0 RID: 480
		// (get) Token: 0x06000F15 RID: 3861 RVA: 0x00042A25 File Offset: 0x00040C25
		// (set) Token: 0x06000F16 RID: 3862 RVA: 0x00042A2D File Offset: 0x00040C2D
		public NetworkLoadout networkLoadout { get; private set; }

		// Token: 0x06000F17 RID: 3863 RVA: 0x00042A36 File Offset: 0x00040C36
		private void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			this.networkLoadout = base.GetComponent<NetworkLoadout>();
			this.networkLoadout.onLoadoutUpdated += this.OnLoadoutUpdated;
		}

		// Token: 0x06000F18 RID: 3864 RVA: 0x00042A66 File Offset: 0x00040C66
		private void OnLoadoutUpdated()
		{
			Action<NetworkUser> action = NetworkUser.onLoadoutChangedGlobal;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x06000F19 RID: 3865 RVA: 0x00042A78 File Offset: 0x00040C78
		private void PullLoadoutFromUserProfile()
		{
			LocalUser localUser = this.localUser;
			UserProfile userProfile = (localUser != null) ? localUser.userProfile : null;
			if (userProfile == null)
			{
				return;
			}
			Loadout loadout = new Loadout();
			userProfile.CopyLoadout(loadout);
			this.networkLoadout.SetLoadout(loadout);
		}

		// Token: 0x06000F1A RID: 3866 RVA: 0x00042AB8 File Offset: 0x00040CB8
		private void Start()
		{
			if (base.isLocalPlayer)
			{
				LocalUser localUser = LocalUserManager.FindLocalUser((int)base.playerControllerId);
				if (localUser != null)
				{
					localUser.LinkNetworkUser(this);
				}
				this.PullLoadoutFromUserProfile();
				if (SceneManager.GetActiveScene().name == "lobby")
				{
					this.CallCmdSetBodyPreference(BodyCatalog.FindBodyIndex("CommandoBody"));
				}
			}
			if (Run.instance)
			{
				Run.instance.OnUserAdded(this);
			}
			if (NetworkClient.active)
			{
				this.SyncLunarCoinsToServer();
				this.SendServerUnlockables();
			}
			this.OnLoadoutUpdated();
			NetworkUser.NetworkUserGenericDelegate onPostNetworkUserStart = NetworkUser.OnPostNetworkUserStart;
			if (onPostNetworkUserStart == null)
			{
				return;
			}
			onPostNetworkUserStart(this);
		}

		// Token: 0x06000F1B RID: 3867 RVA: 0x00042B53 File Offset: 0x00040D53
		private void OnDestroy()
		{
			NetworkUser.localPlayers.Remove(this);
			Run instance = Run.instance;
			if (instance != null)
			{
				instance.OnUserRemoved(this);
			}
			LocalUser localUser = this.localUser;
			if (localUser == null)
			{
				return;
			}
			localUser.UnlinkNetworkUser();
		}

		// Token: 0x06000F1C RID: 3868 RVA: 0x00042B82 File Offset: 0x00040D82
		public override void OnStartLocalPlayer()
		{
			base.OnStartLocalPlayer();
			NetworkUser.localPlayers.Add(this);
		}

		// Token: 0x06000F1D RID: 3869 RVA: 0x00042B95 File Offset: 0x00040D95
		public override void OnStartClient()
		{
			this.UpdateUserName();
		}

		// Token: 0x170001E1 RID: 481
		// (get) Token: 0x06000F1E RID: 3870 RVA: 0x00042B9D File Offset: 0x00040D9D
		// (set) Token: 0x06000F1F RID: 3871 RVA: 0x00042BA5 File Offset: 0x00040DA5
		public NetworkUserId id
		{
			get
			{
				return this._id;
			}
			set
			{
				if (this._id.Equals(value))
				{
					return;
				}
				this.Network_id = value;
				this.UpdateUserName();
			}
		}

		// Token: 0x06000F20 RID: 3872 RVA: 0x00042BC3 File Offset: 0x00040DC3
		private void OnSyncId(NetworkUserId newId)
		{
			this.id = newId;
		}

		// Token: 0x170001E2 RID: 482
		// (get) Token: 0x06000F21 RID: 3873 RVA: 0x00042BCC File Offset: 0x00040DCC
		public bool authed
		{
			get
			{
				return this.id.value > 0UL;
			}
		}

		// Token: 0x06000F22 RID: 3874 RVA: 0x00042BDD File Offset: 0x00040DDD
		private void OnSyncMasterObjectId(NetworkInstanceId newValue)
		{
			this._masterObject = null;
			this.Network_masterObjectId = newValue;
		}

		// Token: 0x170001E3 RID: 483
		// (get) Token: 0x06000F23 RID: 3875 RVA: 0x00042BED File Offset: 0x00040DED
		public Player inputPlayer
		{
			get
			{
				LocalUser localUser = this.localUser;
				if (localUser == null)
				{
					return null;
				}
				return localUser.inputPlayer;
			}
		}

		// Token: 0x06000F24 RID: 3876 RVA: 0x00042C00 File Offset: 0x00040E00
		public NetworkPlayerName GetNetworkPlayerName()
		{
			return new NetworkPlayerName
			{
				nameOverride = null,
				steamId = this.id.steamId
			};
		}

		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x06000F25 RID: 3877 RVA: 0x00042C33 File Offset: 0x00040E33
		public uint lunarCoins
		{
			get
			{
				if (this.localUser != null)
				{
					return this.localUser.userProfile.coins;
				}
				return this.netLunarCoins;
			}
		}

		// Token: 0x06000F26 RID: 3878 RVA: 0x00042C54 File Offset: 0x00040E54
		[Server]
		public void DeductLunarCoins(uint count)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.NetworkUser::DeductLunarCoins(System.UInt32)' called on client");
				return;
			}
			this.NetworknetLunarCoins = HGMath.UintSafeSubtact(this.netLunarCoins, count);
			this.CallRpcDeductLunarCoins(count);
		}

		// Token: 0x06000F27 RID: 3879 RVA: 0x00042C84 File Offset: 0x00040E84
		[ClientRpc]
		private void RpcDeductLunarCoins(uint count)
		{
			if (this.localUser == null)
			{
				return;
			}
			this.localUser.userProfile.coins = HGMath.UintSafeSubtact(this.localUser.userProfile.coins, count);
			this.SyncLunarCoinsToServer();
		}

		// Token: 0x06000F28 RID: 3880 RVA: 0x00042CBB File Offset: 0x00040EBB
		[Server]
		public void AwardLunarCoins(uint count)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.NetworkUser::AwardLunarCoins(System.UInt32)' called on client");
				return;
			}
			this.NetworknetLunarCoins = HGMath.UintSafeAdd(this.netLunarCoins, count);
			this.CallRpcAwardLunarCoins(count);
		}

		// Token: 0x06000F29 RID: 3881 RVA: 0x00042CEC File Offset: 0x00040EEC
		[ClientRpc]
		private void RpcAwardLunarCoins(uint count)
		{
			if (this.localUser == null)
			{
				return;
			}
			this.localUser.userProfile.coins = HGMath.UintSafeAdd(this.localUser.userProfile.coins, count);
			this.localUser.userProfile.totalCollectedCoins = HGMath.UintSafeAdd(this.localUser.userProfile.totalCollectedCoins, count);
			this.SyncLunarCoinsToServer();
		}

		// Token: 0x06000F2A RID: 3882 RVA: 0x00042D54 File Offset: 0x00040F54
		[Client]
		private void SyncLunarCoinsToServer()
		{
			if (!NetworkClient.active)
			{
				Debug.LogWarning("[Client] function 'System.Void RoR2.NetworkUser::SyncLunarCoinsToServer()' called on server");
				return;
			}
			if (this.localUser == null)
			{
				return;
			}
			this.CallCmdSetNetLunarCoins(this.localUser.userProfile.coins);
		}

		// Token: 0x06000F2B RID: 3883 RVA: 0x00042D8A File Offset: 0x00040F8A
		[Command]
		private void CmdSetNetLunarCoins(uint newNetLunarCoins)
		{
			this.NetworknetLunarCoins = newNetLunarCoins;
		}

		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x06000F2C RID: 3884 RVA: 0x00042D93 File Offset: 0x00040F93
		public CharacterMaster master
		{
			get
			{
				return this.cachedMaster.Get(this.masterObject);
			}
		}

		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x06000F2D RID: 3885 RVA: 0x00042DA6 File Offset: 0x00040FA6
		public PlayerCharacterMasterController masterController
		{
			get
			{
				return this.cachedPlayerCharacterMasterController.Get(this.masterObject);
			}
		}

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x06000F2E RID: 3886 RVA: 0x00042DB9 File Offset: 0x00040FB9
		public PlayerStatsComponent masterPlayerStatsComponent
		{
			get
			{
				return this.cachedPlayerStatsComponent.Get(this.masterObject);
			}
		}

		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x06000F2F RID: 3887 RVA: 0x00042DCC File Offset: 0x00040FCC
		// (set) Token: 0x06000F30 RID: 3888 RVA: 0x00042DF4 File Offset: 0x00040FF4
		public GameObject masterObject
		{
			get
			{
				if (!this._masterObject)
				{
					this._masterObject = Util.FindNetworkObject(this._masterObjectId);
				}
				return this._masterObject;
			}
			set
			{
				if (value)
				{
					this.Network_masterObjectId = value.GetComponent<NetworkIdentity>().netId;
					this._masterObject = value;
				}
				else
				{
					this.Network_masterObjectId = NetworkInstanceId.Invalid;
					this._masterObject = null;
				}
				if (this._masterObject && NetworkServer.active)
				{
					this.UpdateMasterPreferences();
				}
			}
		}

		// Token: 0x06000F31 RID: 3889 RVA: 0x00042E50 File Offset: 0x00041050
		public CharacterBody GetCurrentBody()
		{
			CharacterMaster master = this.master;
			if (master)
			{
				return master.GetBody();
			}
			return null;
		}

		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x06000F32 RID: 3890 RVA: 0x00042E74 File Offset: 0x00041074
		public bool isParticipating
		{
			get
			{
				return this.masterObject;
			}
		}

		// Token: 0x06000F33 RID: 3891 RVA: 0x00042E84 File Offset: 0x00041084
		private void UpdateMasterPreferences()
		{
			if (this.masterObject)
			{
				CharacterMaster master = this.master;
				if (master)
				{
					if (this.bodyIndexPreference == -1)
					{
						this.NetworkbodyIndexPreference = BodyCatalog.FindBodyIndex(master.bodyPrefab);
						if (this.bodyIndexPreference == -1)
						{
							this.NetworkbodyIndexPreference = BodyCatalog.FindBodyIndex("CommandoBody");
							return;
						}
					}
					else
					{
						master.bodyPrefab = BodyCatalog.GetBodyPrefab(this.bodyIndexPreference);
						Loadout loadout = new Loadout();
						this.networkLoadout.CopyLoadout(loadout);
						master.SetLoadoutServer(loadout);
					}
				}
			}
		}

		// Token: 0x06000F34 RID: 3892 RVA: 0x00042F0B File Offset: 0x0004110B
		private void SetBodyPreference(int newBodyIndexPreference)
		{
			this.NetworkbodyIndexPreference = newBodyIndexPreference;
			if (this.masterObject)
			{
				this.UpdateMasterPreferences();
			}
		}

		// Token: 0x06000F35 RID: 3893 RVA: 0x00042F27 File Offset: 0x00041127
		[Command]
		public void CmdSetBodyPreference(int newBodyIndexPreference)
		{
			this.SetBodyPreference(newBodyIndexPreference);
		}

		// Token: 0x06000F36 RID: 3894 RVA: 0x00042F30 File Offset: 0x00041130
		private void Update()
		{
			if (this.localUser != null)
			{
				if (Time.timeScale != 0f)
				{
					this.secondAccumulator += Time.unscaledDeltaTime;
				}
				if (this.secondAccumulator >= 1f)
				{
					this.secondAccumulator -= 1f;
					if (Run.instance)
					{
						this.localUser.userProfile.totalRunSeconds += 1U;
						if (this.masterObject)
						{
							CharacterMaster component = this.masterObject.GetComponent<CharacterMaster>();
							if (component && component.alive)
							{
								this.localUser.userProfile.totalAliveSeconds += 1U;
							}
						}
					}
				}
			}
		}

		// Token: 0x06000F37 RID: 3895 RVA: 0x00042FEC File Offset: 0x000411EC
		public void UpdateUserName()
		{
			this.userName = this.GetNetworkPlayerName().GetResolvedName();
		}

		// Token: 0x06000F38 RID: 3896 RVA: 0x0004300D File Offset: 0x0004120D
		[Command]
		public void CmdSendConsoleCommand(string commandName, string[] args)
		{
			Console.instance.RunClientCmd(this, commandName, args);
		}

		// Token: 0x06000F39 RID: 3897 RVA: 0x0004301C File Offset: 0x0004121C
		[Client]
		public void SendServerUnlockables()
		{
			if (!NetworkClient.active)
			{
				Debug.LogWarning("[Client] function 'System.Void RoR2.NetworkUser::SendServerUnlockables()' called on server");
				return;
			}
			if (this.localUser != null)
			{
				int unlockableCount = this.localUser.userProfile.statSheet.GetUnlockableCount();
				UnlockableIndex[] array = new UnlockableIndex[unlockableCount];
				for (int i = 0; i < unlockableCount; i++)
				{
					array[i] = this.localUser.userProfile.statSheet.GetUnlockableIndex(i);
				}
				this.CallCmdSendNewUnlockables(array);
			}
		}

		// Token: 0x06000F3A RID: 3898 RVA: 0x00043094 File Offset: 0x00041294
		[Command]
		private void CmdSendNewUnlockables(UnlockableIndex[] newUnlockableIndices)
		{
			this.unlockables.Clear();
			this.debugUnlockablesList.Clear();
			int i = 0;
			int num = newUnlockableIndices.Length;
			while (i < num)
			{
				UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(newUnlockableIndices[i]);
				if (unlockableDef != null)
				{
					this.unlockables.Add(unlockableDef);
					this.debugUnlockablesList.Add(unlockableDef.name);
				}
				i++;
			}
			NetworkUser.NetworkUserGenericDelegate onNetworkUserUnlockablesUpdated = NetworkUser.OnNetworkUserUnlockablesUpdated;
			if (onNetworkUserUnlockablesUpdated == null)
			{
				return;
			}
			onNetworkUserUnlockablesUpdated(this);
		}

		// Token: 0x06000F3B RID: 3899 RVA: 0x00043104 File Offset: 0x00041304
		[Server]
		public void ServerRequestUnlockables()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.NetworkUser::ServerRequestUnlockables()' called on client");
				return;
			}
			this.CallRpcRequestUnlockables();
		}

		// Token: 0x06000F3C RID: 3900 RVA: 0x00043121 File Offset: 0x00041321
		[ClientRpc]
		private void RpcRequestUnlockables()
		{
			if (Util.HasEffectiveAuthority(base.gameObject))
			{
				this.SendServerUnlockables();
			}
		}

		// Token: 0x06000F3D RID: 3901 RVA: 0x00043138 File Offset: 0x00041338
		[Command]
		public void CmdReportAchievement(string achievementNameToken)
		{
			Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
			{
				baseToken = "ACHIEVEMENT_UNLOCKED_MESSAGE",
				subjectAsNetworkUser = this,
				paramTokens = new string[]
				{
					achievementNameToken
				}
			});
		}

		// Token: 0x06000F3E RID: 3902 RVA: 0x00043174 File Offset: 0x00041374
		[Command]
		public void CmdReportUnlock(UnlockableIndex unlockIndex)
		{
			Debug.LogFormat("NetworkUser.CmdReportUnlock({0})", new object[]
			{
				unlockIndex
			});
			UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(unlockIndex);
			if (unlockableDef != null)
			{
				this.ServerHandleUnlock(unlockableDef);
			}
		}

		// Token: 0x06000F3F RID: 3903 RVA: 0x000431AC File Offset: 0x000413AC
		[Server]
		public void ServerHandleUnlock([NotNull] UnlockableDef unlockableDef)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.NetworkUser::ServerHandleUnlock(RoR2.UnlockableDef)' called on client");
				return;
			}
			Debug.LogFormat("NetworkUser.ServerHandleUnlock({0})", new object[]
			{
				unlockableDef.name
			});
			if (this.masterObject)
			{
				PlayerStatsComponent component = this.masterObject.GetComponent<PlayerStatsComponent>();
				if (component)
				{
					component.currentStats.AddUnlockable(unlockableDef);
					component.ForceNextTransmit();
				}
			}
		}

		// Token: 0x06000F40 RID: 3904 RVA: 0x0004321C File Offset: 0x0004141C
		[Command]
		public void CmdSubmitVote(GameObject voteControllerGameObject, int choiceIndex)
		{
			if (!voteControllerGameObject)
			{
				return;
			}
			VoteController component = voteControllerGameObject.GetComponent<VoteController>();
			if (!component)
			{
				return;
			}
			component.ReceiveUserVote(this, choiceIndex);
		}

		// Token: 0x14000025 RID: 37
		// (add) Token: 0x06000F41 RID: 3905 RVA: 0x0004324C File Offset: 0x0004144C
		// (remove) Token: 0x06000F42 RID: 3906 RVA: 0x00043280 File Offset: 0x00041480
		public static event Action<NetworkUser> onLoadoutChangedGlobal;

		// Token: 0x14000026 RID: 38
		// (add) Token: 0x06000F43 RID: 3907 RVA: 0x000432B4 File Offset: 0x000414B4
		// (remove) Token: 0x06000F44 RID: 3908 RVA: 0x000432E8 File Offset: 0x000414E8
		public static event NetworkUser.NetworkUserGenericDelegate OnPostNetworkUserStart;

		// Token: 0x14000027 RID: 39
		// (add) Token: 0x06000F45 RID: 3909 RVA: 0x0004331C File Offset: 0x0004151C
		// (remove) Token: 0x06000F46 RID: 3910 RVA: 0x00043350 File Offset: 0x00041550
		public static event NetworkUser.NetworkUserGenericDelegate OnNetworkUserUnlockablesUpdated;

		// Token: 0x14000028 RID: 40
		// (add) Token: 0x06000F47 RID: 3911 RVA: 0x00043384 File Offset: 0x00041584
		// (remove) Token: 0x06000F48 RID: 3912 RVA: 0x000433B8 File Offset: 0x000415B8
		public static event NetworkUser.NetworkUserGenericDelegate onNetworkUserDiscovered;

		// Token: 0x14000029 RID: 41
		// (add) Token: 0x06000F49 RID: 3913 RVA: 0x000433EC File Offset: 0x000415EC
		// (remove) Token: 0x06000F4A RID: 3914 RVA: 0x00043420 File Offset: 0x00041620
		public static event NetworkUser.NetworkUserGenericDelegate onNetworkUserLost;

		// Token: 0x06000F4C RID: 3916 RVA: 0x00043494 File Offset: 0x00041694
		static NetworkUser()
		{
			NetworkBehaviour.RegisterCommandDelegate(typeof(NetworkUser), NetworkUser.kCmdCmdSetNetLunarCoins, new NetworkBehaviour.CmdDelegate(NetworkUser.InvokeCmdCmdSetNetLunarCoins));
			NetworkUser.kCmdCmdSetBodyPreference = 234442470;
			NetworkBehaviour.RegisterCommandDelegate(typeof(NetworkUser), NetworkUser.kCmdCmdSetBodyPreference, new NetworkBehaviour.CmdDelegate(NetworkUser.InvokeCmdCmdSetBodyPreference));
			NetworkUser.kCmdCmdSendConsoleCommand = -1997680971;
			NetworkBehaviour.RegisterCommandDelegate(typeof(NetworkUser), NetworkUser.kCmdCmdSendConsoleCommand, new NetworkBehaviour.CmdDelegate(NetworkUser.InvokeCmdCmdSendConsoleCommand));
			NetworkUser.kCmdCmdSendNewUnlockables = 1855027350;
			NetworkBehaviour.RegisterCommandDelegate(typeof(NetworkUser), NetworkUser.kCmdCmdSendNewUnlockables, new NetworkBehaviour.CmdDelegate(NetworkUser.InvokeCmdCmdSendNewUnlockables));
			NetworkUser.kCmdCmdReportAchievement = -1674656990;
			NetworkBehaviour.RegisterCommandDelegate(typeof(NetworkUser), NetworkUser.kCmdCmdReportAchievement, new NetworkBehaviour.CmdDelegate(NetworkUser.InvokeCmdCmdReportAchievement));
			NetworkUser.kCmdCmdReportUnlock = -1831223439;
			NetworkBehaviour.RegisterCommandDelegate(typeof(NetworkUser), NetworkUser.kCmdCmdReportUnlock, new NetworkBehaviour.CmdDelegate(NetworkUser.InvokeCmdCmdReportUnlock));
			NetworkUser.kCmdCmdSubmitVote = 329593659;
			NetworkBehaviour.RegisterCommandDelegate(typeof(NetworkUser), NetworkUser.kCmdCmdSubmitVote, new NetworkBehaviour.CmdDelegate(NetworkUser.InvokeCmdCmdSubmitVote));
			NetworkUser.kRpcRpcDeductLunarCoins = -1554352898;
			NetworkBehaviour.RegisterRpcDelegate(typeof(NetworkUser), NetworkUser.kRpcRpcDeductLunarCoins, new NetworkBehaviour.CmdDelegate(NetworkUser.InvokeRpcRpcDeductLunarCoins));
			NetworkUser.kRpcRpcAwardLunarCoins = -604060198;
			NetworkBehaviour.RegisterRpcDelegate(typeof(NetworkUser), NetworkUser.kRpcRpcAwardLunarCoins, new NetworkBehaviour.CmdDelegate(NetworkUser.InvokeRpcRpcAwardLunarCoins));
			NetworkUser.kRpcRpcRequestUnlockables = -1809653515;
			NetworkBehaviour.RegisterRpcDelegate(typeof(NetworkUser), NetworkUser.kRpcRpcRequestUnlockables, new NetworkBehaviour.CmdDelegate(NetworkUser.InvokeRpcRpcRequestUnlockables));
			NetworkCRC.RegisterBehaviour("NetworkUser", 0);
		}

		// Token: 0x06000F4D RID: 3917 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x170001EA RID: 490
		// (get) Token: 0x06000F4E RID: 3918 RVA: 0x00043688 File Offset: 0x00041888
		// (set) Token: 0x06000F4F RID: 3919 RVA: 0x0004369B File Offset: 0x0004189B
		public NetworkUserId Network_id
		{
			get
			{
				return this._id;
			}
			[param: In]
			set
			{
				uint dirtyBit = 1U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncId(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<NetworkUserId>(value, ref this._id, dirtyBit);
			}
		}

		// Token: 0x170001EB RID: 491
		// (get) Token: 0x06000F50 RID: 3920 RVA: 0x000436DC File Offset: 0x000418DC
		// (set) Token: 0x06000F51 RID: 3921 RVA: 0x000436EF File Offset: 0x000418EF
		public byte NetworkrewiredPlayerId
		{
			get
			{
				return this.rewiredPlayerId;
			}
			[param: In]
			set
			{
				base.SetSyncVar<byte>(value, ref this.rewiredPlayerId, 2U);
			}
		}

		// Token: 0x170001EC RID: 492
		// (get) Token: 0x06000F52 RID: 3922 RVA: 0x00043704 File Offset: 0x00041904
		// (set) Token: 0x06000F53 RID: 3923 RVA: 0x00043717 File Offset: 0x00041917
		public NetworkInstanceId Network_masterObjectId
		{
			get
			{
				return this._masterObjectId;
			}
			[param: In]
			set
			{
				uint dirtyBit = 4U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncMasterObjectId(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<NetworkInstanceId>(value, ref this._masterObjectId, dirtyBit);
			}
		}

		// Token: 0x170001ED RID: 493
		// (get) Token: 0x06000F54 RID: 3924 RVA: 0x00043758 File Offset: 0x00041958
		// (set) Token: 0x06000F55 RID: 3925 RVA: 0x0004376B File Offset: 0x0004196B
		public Color32 NetworkuserColor
		{
			get
			{
				return this.userColor;
			}
			[param: In]
			set
			{
				base.SetSyncVar<Color32>(value, ref this.userColor, 8U);
			}
		}

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x06000F56 RID: 3926 RVA: 0x00043780 File Offset: 0x00041980
		// (set) Token: 0x06000F57 RID: 3927 RVA: 0x00043793 File Offset: 0x00041993
		public uint NetworknetLunarCoins
		{
			get
			{
				return this.netLunarCoins;
			}
			[param: In]
			set
			{
				base.SetSyncVar<uint>(value, ref this.netLunarCoins, 16U);
			}
		}

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x06000F58 RID: 3928 RVA: 0x000437A8 File Offset: 0x000419A8
		// (set) Token: 0x06000F59 RID: 3929 RVA: 0x000437BB File Offset: 0x000419BB
		public int NetworkbodyIndexPreference
		{
			get
			{
				return this.bodyIndexPreference;
			}
			[param: In]
			set
			{
				uint dirtyBit = 32U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.SetBodyPreference(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<int>(value, ref this.bodyIndexPreference, dirtyBit);
			}
		}

		// Token: 0x06000F5A RID: 3930 RVA: 0x000437FA File Offset: 0x000419FA
		protected static void InvokeCmdCmdSetNetLunarCoins(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdSetNetLunarCoins called on client.");
				return;
			}
			((NetworkUser)obj).CmdSetNetLunarCoins(reader.ReadPackedUInt32());
		}

		// Token: 0x06000F5B RID: 3931 RVA: 0x00043823 File Offset: 0x00041A23
		protected static void InvokeCmdCmdSetBodyPreference(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdSetBodyPreference called on client.");
				return;
			}
			((NetworkUser)obj).CmdSetBodyPreference((int)reader.ReadPackedUInt32());
		}

		// Token: 0x06000F5C RID: 3932 RVA: 0x0004384C File Offset: 0x00041A4C
		protected static void InvokeCmdCmdSendConsoleCommand(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdSendConsoleCommand called on client.");
				return;
			}
			((NetworkUser)obj).CmdSendConsoleCommand(reader.ReadString(), GeneratedNetworkCode._ReadArrayString_None(reader));
		}

		// Token: 0x06000F5D RID: 3933 RVA: 0x0004387B File Offset: 0x00041A7B
		protected static void InvokeCmdCmdSendNewUnlockables(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdSendNewUnlockables called on client.");
				return;
			}
			((NetworkUser)obj).CmdSendNewUnlockables(GeneratedNetworkCode._ReadArrayUnlockableIndex_None(reader));
		}

		// Token: 0x06000F5E RID: 3934 RVA: 0x000438A4 File Offset: 0x00041AA4
		protected static void InvokeCmdCmdReportAchievement(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdReportAchievement called on client.");
				return;
			}
			((NetworkUser)obj).CmdReportAchievement(reader.ReadString());
		}

		// Token: 0x06000F5F RID: 3935 RVA: 0x000438CD File Offset: 0x00041ACD
		protected static void InvokeCmdCmdReportUnlock(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdReportUnlock called on client.");
				return;
			}
			((NetworkUser)obj).CmdReportUnlock(GeneratedNetworkCode._ReadUnlockableIndex_None(reader));
		}

		// Token: 0x06000F60 RID: 3936 RVA: 0x000438F6 File Offset: 0x00041AF6
		protected static void InvokeCmdCmdSubmitVote(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdSubmitVote called on client.");
				return;
			}
			((NetworkUser)obj).CmdSubmitVote(reader.ReadGameObject(), (int)reader.ReadPackedUInt32());
		}

		// Token: 0x06000F61 RID: 3937 RVA: 0x00043928 File Offset: 0x00041B28
		public void CallCmdSetNetLunarCoins(uint newNetLunarCoins)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdSetNetLunarCoins called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdSetNetLunarCoins(newNetLunarCoins);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)NetworkUser.kCmdCmdSetNetLunarCoins);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.WritePackedUInt32(newNetLunarCoins);
			base.SendCommandInternal(networkWriter, 0, "CmdSetNetLunarCoins");
		}

		// Token: 0x06000F62 RID: 3938 RVA: 0x000439B4 File Offset: 0x00041BB4
		public void CallCmdSetBodyPreference(int newBodyIndexPreference)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdSetBodyPreference called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdSetBodyPreference(newBodyIndexPreference);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)NetworkUser.kCmdCmdSetBodyPreference);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.WritePackedUInt32((uint)newBodyIndexPreference);
			base.SendCommandInternal(networkWriter, 0, "CmdSetBodyPreference");
		}

		// Token: 0x06000F63 RID: 3939 RVA: 0x00043A40 File Offset: 0x00041C40
		public void CallCmdSendConsoleCommand(string commandName, string[] args)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdSendConsoleCommand called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdSendConsoleCommand(commandName, args);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)NetworkUser.kCmdCmdSendConsoleCommand);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(commandName);
			GeneratedNetworkCode._WriteArrayString_None(networkWriter, args);
			base.SendCommandInternal(networkWriter, 0, "CmdSendConsoleCommand");
		}

		// Token: 0x06000F64 RID: 3940 RVA: 0x00043AD8 File Offset: 0x00041CD8
		public void CallCmdSendNewUnlockables(UnlockableIndex[] newUnlockableIndices)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdSendNewUnlockables called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdSendNewUnlockables(newUnlockableIndices);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)NetworkUser.kCmdCmdSendNewUnlockables);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			GeneratedNetworkCode._WriteArrayUnlockableIndex_None(networkWriter, newUnlockableIndices);
			base.SendCommandInternal(networkWriter, 0, "CmdSendNewUnlockables");
		}

		// Token: 0x06000F65 RID: 3941 RVA: 0x00043B64 File Offset: 0x00041D64
		public void CallCmdReportAchievement(string achievementNameToken)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdReportAchievement called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdReportAchievement(achievementNameToken);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)NetworkUser.kCmdCmdReportAchievement);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(achievementNameToken);
			base.SendCommandInternal(networkWriter, 0, "CmdReportAchievement");
		}

		// Token: 0x06000F66 RID: 3942 RVA: 0x00043BF0 File Offset: 0x00041DF0
		public void CallCmdReportUnlock(UnlockableIndex unlockIndex)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdReportUnlock called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdReportUnlock(unlockIndex);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)NetworkUser.kCmdCmdReportUnlock);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			GeneratedNetworkCode._WriteUnlockableIndex_None(networkWriter, unlockIndex);
			base.SendCommandInternal(networkWriter, 0, "CmdReportUnlock");
		}

		// Token: 0x06000F67 RID: 3943 RVA: 0x00043C7C File Offset: 0x00041E7C
		public void CallCmdSubmitVote(GameObject voteControllerGameObject, int choiceIndex)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdSubmitVote called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdSubmitVote(voteControllerGameObject, choiceIndex);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)NetworkUser.kCmdCmdSubmitVote);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(voteControllerGameObject);
			networkWriter.WritePackedUInt32((uint)choiceIndex);
			base.SendCommandInternal(networkWriter, 0, "CmdSubmitVote");
		}

		// Token: 0x06000F68 RID: 3944 RVA: 0x00043D14 File Offset: 0x00041F14
		protected static void InvokeRpcRpcDeductLunarCoins(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcDeductLunarCoins called on server.");
				return;
			}
			((NetworkUser)obj).RpcDeductLunarCoins(reader.ReadPackedUInt32());
		}

		// Token: 0x06000F69 RID: 3945 RVA: 0x00043D3D File Offset: 0x00041F3D
		protected static void InvokeRpcRpcAwardLunarCoins(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcAwardLunarCoins called on server.");
				return;
			}
			((NetworkUser)obj).RpcAwardLunarCoins(reader.ReadPackedUInt32());
		}

		// Token: 0x06000F6A RID: 3946 RVA: 0x00043D66 File Offset: 0x00041F66
		protected static void InvokeRpcRpcRequestUnlockables(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcRequestUnlockables called on server.");
				return;
			}
			((NetworkUser)obj).RpcRequestUnlockables();
		}

		// Token: 0x06000F6B RID: 3947 RVA: 0x00043D8C File Offset: 0x00041F8C
		public void CallRpcDeductLunarCoins(uint count)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcDeductLunarCoins called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)NetworkUser.kRpcRpcDeductLunarCoins);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.WritePackedUInt32(count);
			this.SendRPCInternal(networkWriter, 0, "RpcDeductLunarCoins");
		}

		// Token: 0x06000F6C RID: 3948 RVA: 0x00043E00 File Offset: 0x00042000
		public void CallRpcAwardLunarCoins(uint count)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcAwardLunarCoins called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)NetworkUser.kRpcRpcAwardLunarCoins);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.WritePackedUInt32(count);
			this.SendRPCInternal(networkWriter, 0, "RpcAwardLunarCoins");
		}

		// Token: 0x06000F6D RID: 3949 RVA: 0x00043E74 File Offset: 0x00042074
		public void CallRpcRequestUnlockables()
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("RPC Function RpcRequestUnlockables called on client.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)2));
			networkWriter.WritePackedUInt32((uint)NetworkUser.kRpcRpcRequestUnlockables);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			this.SendRPCInternal(networkWriter, 0, "RpcRequestUnlockables");
		}

		// Token: 0x06000F6E RID: 3950 RVA: 0x00043EE0 File Offset: 0x000420E0
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				GeneratedNetworkCode._WriteNetworkUserId_None(writer, this._id);
				writer.WritePackedUInt32((uint)this.rewiredPlayerId);
				writer.Write(this._masterObjectId);
				writer.Write(this.userColor);
				writer.WritePackedUInt32(this.netLunarCoins);
				writer.WritePackedUInt32((uint)this.bodyIndexPreference);
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
				GeneratedNetworkCode._WriteNetworkUserId_None(writer, this._id);
			}
			if ((base.syncVarDirtyBits & 2U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32((uint)this.rewiredPlayerId);
			}
			if ((base.syncVarDirtyBits & 4U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this._masterObjectId);
			}
			if ((base.syncVarDirtyBits & 8U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.userColor);
			}
			if ((base.syncVarDirtyBits & 16U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32(this.netLunarCoins);
			}
			if ((base.syncVarDirtyBits & 32U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32((uint)this.bodyIndexPreference);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000F6F RID: 3951 RVA: 0x00044088 File Offset: 0x00042288
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this._id = GeneratedNetworkCode._ReadNetworkUserId_None(reader);
				this.rewiredPlayerId = (byte)reader.ReadPackedUInt32();
				this._masterObjectId = reader.ReadNetworkId();
				this.userColor = reader.ReadColor32();
				this.netLunarCoins = reader.ReadPackedUInt32();
				this.bodyIndexPreference = (int)reader.ReadPackedUInt32();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.OnSyncId(GeneratedNetworkCode._ReadNetworkUserId_None(reader));
			}
			if ((num & 2) != 0)
			{
				this.rewiredPlayerId = (byte)reader.ReadPackedUInt32();
			}
			if ((num & 4) != 0)
			{
				this.OnSyncMasterObjectId(reader.ReadNetworkId());
			}
			if ((num & 8) != 0)
			{
				this.userColor = reader.ReadColor32();
			}
			if ((num & 16) != 0)
			{
				this.netLunarCoins = reader.ReadPackedUInt32();
			}
			if ((num & 32) != 0)
			{
				this.SetBodyPreference((int)reader.ReadPackedUInt32());
			}
		}

		// Token: 0x04000EC5 RID: 3781
		private static readonly List<NetworkUser> instancesList = new List<NetworkUser>();

		// Token: 0x04000EC6 RID: 3782
		public static readonly ReadOnlyCollection<NetworkUser> readOnlyInstancesList = new ReadOnlyCollection<NetworkUser>(NetworkUser.instancesList);

		// Token: 0x04000EC7 RID: 3783
		private static readonly List<NetworkUser> localPlayers = new List<NetworkUser>();

		// Token: 0x04000EC8 RID: 3784
		public static readonly ReadOnlyCollection<NetworkUser> readOnlyLocalPlayersList = new ReadOnlyCollection<NetworkUser>(NetworkUser.localPlayers);

		// Token: 0x04000ECA RID: 3786
		[SyncVar(hook = "OnSyncId")]
		private NetworkUserId _id;

		// Token: 0x04000ECB RID: 3787
		[SyncVar]
		public byte rewiredPlayerId;

		// Token: 0x04000ECC RID: 3788
		[SyncVar(hook = "OnSyncMasterObjectId")]
		private NetworkInstanceId _masterObjectId;

		// Token: 0x04000ECD RID: 3789
		[CanBeNull]
		public LocalUser localUser;

		// Token: 0x04000ECE RID: 3790
		public CameraRigController cameraRigController;

		// Token: 0x04000ECF RID: 3791
		public string userName = "";

		// Token: 0x04000ED0 RID: 3792
		[SyncVar]
		public Color32 userColor = Color.red;

		// Token: 0x04000ED1 RID: 3793
		[SyncVar]
		private uint netLunarCoins;

		// Token: 0x04000ED2 RID: 3794
		private MemoizedGetComponent<CharacterMaster> cachedMaster;

		// Token: 0x04000ED3 RID: 3795
		private MemoizedGetComponent<PlayerCharacterMasterController> cachedPlayerCharacterMasterController;

		// Token: 0x04000ED4 RID: 3796
		private MemoizedGetComponent<PlayerStatsComponent> cachedPlayerStatsComponent;

		// Token: 0x04000ED5 RID: 3797
		private GameObject _masterObject;

		// Token: 0x04000ED6 RID: 3798
		[SyncVar(hook = "SetBodyPreference")]
		[NonSerialized]
		public int bodyIndexPreference = -1;

		// Token: 0x04000ED7 RID: 3799
		private float secondAccumulator;

		// Token: 0x04000ED8 RID: 3800
		[NonSerialized]
		public List<UnlockableDef> unlockables = new List<UnlockableDef>();

		// Token: 0x04000ED9 RID: 3801
		public List<string> debugUnlockablesList = new List<string>();

		// Token: 0x04000EDF RID: 3807
		private static int kRpcRpcDeductLunarCoins;

		// Token: 0x04000EE0 RID: 3808
		private static int kRpcRpcAwardLunarCoins;

		// Token: 0x04000EE1 RID: 3809
		private static int kCmdCmdSetNetLunarCoins = -934763456;

		// Token: 0x04000EE2 RID: 3810
		private static int kCmdCmdSetBodyPreference;

		// Token: 0x04000EE3 RID: 3811
		private static int kCmdCmdSendConsoleCommand;

		// Token: 0x04000EE4 RID: 3812
		private static int kCmdCmdSendNewUnlockables;

		// Token: 0x04000EE5 RID: 3813
		private static int kRpcRpcRequestUnlockables;

		// Token: 0x04000EE6 RID: 3814
		private static int kCmdCmdReportAchievement;

		// Token: 0x04000EE7 RID: 3815
		private static int kCmdCmdReportUnlock;

		// Token: 0x04000EE8 RID: 3816
		private static int kCmdCmdSubmitVote;

		// Token: 0x020002A3 RID: 675
		// (Invoke) Token: 0x06000F71 RID: 3953
		public delegate void NetworkUserGenericDelegate(NetworkUser networkUser);
	}
}
