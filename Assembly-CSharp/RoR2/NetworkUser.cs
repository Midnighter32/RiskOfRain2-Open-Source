using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using Rewired;
using RoR2.Stats;
using Unity;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace RoR2
{
	// Token: 0x02000371 RID: 881
	public class NetworkUser : NetworkBehaviour
	{
		// Token: 0x06001231 RID: 4657 RVA: 0x000598C8 File Offset: 0x00057AC8
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
		}

		// Token: 0x06001232 RID: 4658 RVA: 0x000598EE File Offset: 0x00057AEE
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

		// Token: 0x06001233 RID: 4659 RVA: 0x0005990B File Offset: 0x00057B0B
		private void OnDisable()
		{
			NetworkUser.NetworkUserGenericDelegate networkUserGenericDelegate = NetworkUser.onNetworkUserLost;
			if (networkUserGenericDelegate != null)
			{
				networkUserGenericDelegate(this);
			}
			NetworkUser.instancesList.Remove(this);
		}

		// Token: 0x06001234 RID: 4660 RVA: 0x0005992A File Offset: 0x00057B2A
		private void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}

		// Token: 0x06001235 RID: 4661 RVA: 0x00059938 File Offset: 0x00057B38
		private void Start()
		{
			if (base.isLocalPlayer)
			{
				LocalUser localUser = LocalUserManager.FindLocalUser((int)base.playerControllerId);
				if (localUser != null)
				{
					localUser.LinkNetworkUser(this);
				}
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
			NetworkUser.NetworkUserGenericDelegate onPostNetworkUserStart = NetworkUser.OnPostNetworkUserStart;
			if (onPostNetworkUserStart == null)
			{
				return;
			}
			onPostNetworkUserStart(this);
		}

		// Token: 0x06001236 RID: 4662 RVA: 0x000599C7 File Offset: 0x00057BC7
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

		// Token: 0x06001237 RID: 4663 RVA: 0x000599F6 File Offset: 0x00057BF6
		public override void OnStartLocalPlayer()
		{
			base.OnStartLocalPlayer();
			NetworkUser.localPlayers.Add(this);
		}

		// Token: 0x06001238 RID: 4664 RVA: 0x00059A09 File Offset: 0x00057C09
		public override void OnStartClient()
		{
			this.UpdateUserName();
		}

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x06001239 RID: 4665 RVA: 0x00059A11 File Offset: 0x00057C11
		// (set) Token: 0x0600123A RID: 4666 RVA: 0x00059A19 File Offset: 0x00057C19
		public NetworkUserId id
		{
			get
			{
				return this._id;
			}
			set
			{
				this.Network_id = value;
				this.UpdateUserName();
			}
		}

		// Token: 0x0600123B RID: 4667 RVA: 0x00059A28 File Offset: 0x00057C28
		private void OnSyncId(NetworkUserId newId)
		{
			this.id = newId;
		}

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x0600123C RID: 4668 RVA: 0x00059A31 File Offset: 0x00057C31
		public bool authed
		{
			get
			{
				return this.id.value > 0UL;
			}
		}

		// Token: 0x0600123D RID: 4669 RVA: 0x00059A42 File Offset: 0x00057C42
		private void OnSyncMasterObjectId(NetworkInstanceId newValue)
		{
			this._masterObject = null;
			this.Network_masterObjectId = newValue;
		}

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x0600123E RID: 4670 RVA: 0x00059A52 File Offset: 0x00057C52
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

		// Token: 0x0600123F RID: 4671 RVA: 0x00059A68 File Offset: 0x00057C68
		public NetworkPlayerName GetNetworkPlayerName()
		{
			return new NetworkPlayerName
			{
				nameOverride = null,
				steamId = new CSteamID(this.id.value)
			};
		}

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x06001240 RID: 4672 RVA: 0x00059A9D File Offset: 0x00057C9D
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

		// Token: 0x06001241 RID: 4673 RVA: 0x00059ABE File Offset: 0x00057CBE
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

		// Token: 0x06001242 RID: 4674 RVA: 0x00059AEE File Offset: 0x00057CEE
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

		// Token: 0x06001243 RID: 4675 RVA: 0x00059B25 File Offset: 0x00057D25
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

		// Token: 0x06001244 RID: 4676 RVA: 0x00059B58 File Offset: 0x00057D58
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

		// Token: 0x06001245 RID: 4677 RVA: 0x00059BC0 File Offset: 0x00057DC0
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

		// Token: 0x06001246 RID: 4678 RVA: 0x00059BF6 File Offset: 0x00057DF6
		[Command]
		private void CmdSetNetLunarCoins(uint newNetLunarCoins)
		{
			this.NetworknetLunarCoins = newNetLunarCoins;
		}

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x06001247 RID: 4679 RVA: 0x00059BFF File Offset: 0x00057DFF
		public CharacterMaster master
		{
			get
			{
				return this.cachedMaster.Get(this.masterObject);
			}
		}

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x06001248 RID: 4680 RVA: 0x00059C12 File Offset: 0x00057E12
		public PlayerCharacterMasterController masterController
		{
			get
			{
				return this.cachedPlayerCharacterMasterController.Get(this.masterObject);
			}
		}

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x06001249 RID: 4681 RVA: 0x00059C25 File Offset: 0x00057E25
		public PlayerStatsComponent masterPlayerStatsComponent
		{
			get
			{
				return this.cachedPlayerStatsComponent.Get(this.masterObject);
			}
		}

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x0600124A RID: 4682 RVA: 0x00059C38 File Offset: 0x00057E38
		// (set) Token: 0x0600124B RID: 4683 RVA: 0x00059C60 File Offset: 0x00057E60
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

		// Token: 0x0600124C RID: 4684 RVA: 0x00059CBC File Offset: 0x00057EBC
		public CharacterBody GetCurrentBody()
		{
			CharacterMaster master = this.master;
			if (master)
			{
				return master.GetBody();
			}
			return null;
		}

		// Token: 0x17000199 RID: 409
		// (get) Token: 0x0600124D RID: 4685 RVA: 0x00059CE0 File Offset: 0x00057EE0
		public bool isParticipating
		{
			get
			{
				return this.masterObject;
			}
		}

		// Token: 0x0600124E RID: 4686 RVA: 0x00059CF0 File Offset: 0x00057EF0
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
					}
				}
			}
		}

		// Token: 0x0600124F RID: 4687 RVA: 0x00059D5E File Offset: 0x00057F5E
		private void SetBodyPreference(int newBodyIndexPreference)
		{
			this.NetworkbodyIndexPreference = newBodyIndexPreference;
			if (this.masterObject)
			{
				this.UpdateMasterPreferences();
			}
		}

		// Token: 0x06001250 RID: 4688 RVA: 0x00059D7A File Offset: 0x00057F7A
		[Command]
		public void CmdSetBodyPreference(int newBodyIndexPreference)
		{
			this.SetBodyPreference(newBodyIndexPreference);
		}

		// Token: 0x06001251 RID: 4689 RVA: 0x00059D84 File Offset: 0x00057F84
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
						this.localUser.userProfile.totalRunSeconds += 1u;
						if (this.masterObject)
						{
							CharacterMaster component = this.masterObject.GetComponent<CharacterMaster>();
							if (component && component.alive)
							{
								this.localUser.userProfile.totalAliveSeconds += 1u;
							}
						}
					}
				}
			}
		}

		// Token: 0x06001252 RID: 4690 RVA: 0x00059E40 File Offset: 0x00058040
		public void UpdateUserName()
		{
			this.userName = this.GetNetworkPlayerName().GetResolvedName();
		}

		// Token: 0x06001253 RID: 4691 RVA: 0x00059E61 File Offset: 0x00058061
		[Command]
		public void CmdSendConsoleCommand(string cmd)
		{
			Console.instance.SubmitCmd(this, cmd, false);
		}

		// Token: 0x06001254 RID: 4692 RVA: 0x00059E70 File Offset: 0x00058070
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

		// Token: 0x06001255 RID: 4693 RVA: 0x00059EE8 File Offset: 0x000580E8
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

		// Token: 0x06001256 RID: 4694 RVA: 0x00059F58 File Offset: 0x00058158
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

		// Token: 0x06001257 RID: 4695 RVA: 0x00059F75 File Offset: 0x00058175
		[ClientRpc]
		private void RpcRequestUnlockables()
		{
			if (Util.HasEffectiveAuthority(base.gameObject))
			{
				this.SendServerUnlockables();
			}
		}

		// Token: 0x06001258 RID: 4696 RVA: 0x00059F8C File Offset: 0x0005818C
		[Command]
		public void CmdReportAchievement(string achievementNameToken)
		{
			Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
			{
				baseToken = "ACHIEVEMENT_UNLOCKED_MESSAGE",
				subjectNetworkUser = this,
				paramTokens = new string[]
				{
					achievementNameToken
				}
			});
		}

		// Token: 0x06001259 RID: 4697 RVA: 0x00059FC8 File Offset: 0x000581C8
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

		// Token: 0x0600125A RID: 4698 RVA: 0x0005A000 File Offset: 0x00058200
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

		// Token: 0x0600125B RID: 4699 RVA: 0x0005A070 File Offset: 0x00058270
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

		// Token: 0x14000015 RID: 21
		// (add) Token: 0x0600125C RID: 4700 RVA: 0x0005A0A0 File Offset: 0x000582A0
		// (remove) Token: 0x0600125D RID: 4701 RVA: 0x0005A0D4 File Offset: 0x000582D4
		public static event NetworkUser.NetworkUserGenericDelegate OnPostNetworkUserStart;

		// Token: 0x14000016 RID: 22
		// (add) Token: 0x0600125E RID: 4702 RVA: 0x0005A108 File Offset: 0x00058308
		// (remove) Token: 0x0600125F RID: 4703 RVA: 0x0005A13C File Offset: 0x0005833C
		public static event NetworkUser.NetworkUserGenericDelegate OnNetworkUserUnlockablesUpdated;

		// Token: 0x14000017 RID: 23
		// (add) Token: 0x06001260 RID: 4704 RVA: 0x0005A170 File Offset: 0x00058370
		// (remove) Token: 0x06001261 RID: 4705 RVA: 0x0005A1A4 File Offset: 0x000583A4
		public static event NetworkUser.NetworkUserGenericDelegate onNetworkUserDiscovered;

		// Token: 0x14000018 RID: 24
		// (add) Token: 0x06001262 RID: 4706 RVA: 0x0005A1D8 File Offset: 0x000583D8
		// (remove) Token: 0x06001263 RID: 4707 RVA: 0x0005A20C File Offset: 0x0005840C
		public static event NetworkUser.NetworkUserGenericDelegate onNetworkUserLost;

		// Token: 0x06001265 RID: 4709 RVA: 0x0005A280 File Offset: 0x00058480
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

		// Token: 0x06001266 RID: 4710 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x1700019A RID: 410
		// (get) Token: 0x06001267 RID: 4711 RVA: 0x0005A474 File Offset: 0x00058674
		// (set) Token: 0x06001268 RID: 4712 RVA: 0x0005A487 File Offset: 0x00058687
		public NetworkUserId Network_id
		{
			get
			{
				return this._id;
			}
			set
			{
				uint dirtyBit = 1u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncId(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<NetworkUserId>(value, ref this._id, dirtyBit);
			}
		}

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x06001269 RID: 4713 RVA: 0x0005A4C8 File Offset: 0x000586C8
		// (set) Token: 0x0600126A RID: 4714 RVA: 0x0005A4DB File Offset: 0x000586DB
		public byte NetworkrewiredPlayerId
		{
			get
			{
				return this.rewiredPlayerId;
			}
			set
			{
				base.SetSyncVar<byte>(value, ref this.rewiredPlayerId, 2u);
			}
		}

		// Token: 0x1700019C RID: 412
		// (get) Token: 0x0600126B RID: 4715 RVA: 0x0005A4F0 File Offset: 0x000586F0
		// (set) Token: 0x0600126C RID: 4716 RVA: 0x0005A503 File Offset: 0x00058703
		public NetworkInstanceId Network_masterObjectId
		{
			get
			{
				return this._masterObjectId;
			}
			set
			{
				uint dirtyBit = 4u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncMasterObjectId(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<NetworkInstanceId>(value, ref this._masterObjectId, dirtyBit);
			}
		}

		// Token: 0x1700019D RID: 413
		// (get) Token: 0x0600126D RID: 4717 RVA: 0x0005A544 File Offset: 0x00058744
		// (set) Token: 0x0600126E RID: 4718 RVA: 0x0005A557 File Offset: 0x00058757
		public Color32 NetworkuserColor
		{
			get
			{
				return this.userColor;
			}
			set
			{
				base.SetSyncVar<Color32>(value, ref this.userColor, 8u);
			}
		}

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x0600126F RID: 4719 RVA: 0x0005A56C File Offset: 0x0005876C
		// (set) Token: 0x06001270 RID: 4720 RVA: 0x0005A57F File Offset: 0x0005877F
		public uint NetworknetLunarCoins
		{
			get
			{
				return this.netLunarCoins;
			}
			set
			{
				base.SetSyncVar<uint>(value, ref this.netLunarCoins, 16u);
			}
		}

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x06001271 RID: 4721 RVA: 0x0005A594 File Offset: 0x00058794
		// (set) Token: 0x06001272 RID: 4722 RVA: 0x0005A5A7 File Offset: 0x000587A7
		public int NetworkbodyIndexPreference
		{
			get
			{
				return this.bodyIndexPreference;
			}
			set
			{
				uint dirtyBit = 32u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.SetBodyPreference(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<int>(value, ref this.bodyIndexPreference, dirtyBit);
			}
		}

		// Token: 0x06001273 RID: 4723 RVA: 0x0005A5E6 File Offset: 0x000587E6
		protected static void InvokeCmdCmdSetNetLunarCoins(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdSetNetLunarCoins called on client.");
				return;
			}
			((NetworkUser)obj).CmdSetNetLunarCoins(reader.ReadPackedUInt32());
		}

		// Token: 0x06001274 RID: 4724 RVA: 0x0005A60F File Offset: 0x0005880F
		protected static void InvokeCmdCmdSetBodyPreference(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdSetBodyPreference called on client.");
				return;
			}
			((NetworkUser)obj).CmdSetBodyPreference((int)reader.ReadPackedUInt32());
		}

		// Token: 0x06001275 RID: 4725 RVA: 0x0005A638 File Offset: 0x00058838
		protected static void InvokeCmdCmdSendConsoleCommand(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdSendConsoleCommand called on client.");
				return;
			}
			((NetworkUser)obj).CmdSendConsoleCommand(reader.ReadString());
		}

		// Token: 0x06001276 RID: 4726 RVA: 0x0005A661 File Offset: 0x00058861
		protected static void InvokeCmdCmdSendNewUnlockables(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdSendNewUnlockables called on client.");
				return;
			}
			((NetworkUser)obj).CmdSendNewUnlockables(GeneratedNetworkCode._ReadArrayUnlockableIndex_None(reader));
		}

		// Token: 0x06001277 RID: 4727 RVA: 0x0005A68A File Offset: 0x0005888A
		protected static void InvokeCmdCmdReportAchievement(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdReportAchievement called on client.");
				return;
			}
			((NetworkUser)obj).CmdReportAchievement(reader.ReadString());
		}

		// Token: 0x06001278 RID: 4728 RVA: 0x0005A6B3 File Offset: 0x000588B3
		protected static void InvokeCmdCmdReportUnlock(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdReportUnlock called on client.");
				return;
			}
			((NetworkUser)obj).CmdReportUnlock(GeneratedNetworkCode._ReadUnlockableIndex_None(reader));
		}

		// Token: 0x06001279 RID: 4729 RVA: 0x0005A6DC File Offset: 0x000588DC
		protected static void InvokeCmdCmdSubmitVote(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdSubmitVote called on client.");
				return;
			}
			((NetworkUser)obj).CmdSubmitVote(reader.ReadGameObject(), (int)reader.ReadPackedUInt32());
		}

		// Token: 0x0600127A RID: 4730 RVA: 0x0005A70C File Offset: 0x0005890C
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

		// Token: 0x0600127B RID: 4731 RVA: 0x0005A798 File Offset: 0x00058998
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

		// Token: 0x0600127C RID: 4732 RVA: 0x0005A824 File Offset: 0x00058A24
		public void CallCmdSendConsoleCommand(string cmd)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("Command function CmdSendConsoleCommand called on server.");
				return;
			}
			if (base.isServer)
			{
				this.CmdSendConsoleCommand(cmd);
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.Write(0);
			networkWriter.Write((short)((ushort)5));
			networkWriter.WritePackedUInt32((uint)NetworkUser.kCmdCmdSendConsoleCommand);
			networkWriter.Write(base.GetComponent<NetworkIdentity>().netId);
			networkWriter.Write(cmd);
			base.SendCommandInternal(networkWriter, 0, "CmdSendConsoleCommand");
		}

		// Token: 0x0600127D RID: 4733 RVA: 0x0005A8B0 File Offset: 0x00058AB0
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

		// Token: 0x0600127E RID: 4734 RVA: 0x0005A93C File Offset: 0x00058B3C
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

		// Token: 0x0600127F RID: 4735 RVA: 0x0005A9C8 File Offset: 0x00058BC8
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

		// Token: 0x06001280 RID: 4736 RVA: 0x0005AA54 File Offset: 0x00058C54
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

		// Token: 0x06001281 RID: 4737 RVA: 0x0005AAEC File Offset: 0x00058CEC
		protected static void InvokeRpcRpcDeductLunarCoins(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcDeductLunarCoins called on server.");
				return;
			}
			((NetworkUser)obj).RpcDeductLunarCoins(reader.ReadPackedUInt32());
		}

		// Token: 0x06001282 RID: 4738 RVA: 0x0005AB15 File Offset: 0x00058D15
		protected static void InvokeRpcRpcAwardLunarCoins(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcAwardLunarCoins called on server.");
				return;
			}
			((NetworkUser)obj).RpcAwardLunarCoins(reader.ReadPackedUInt32());
		}

		// Token: 0x06001283 RID: 4739 RVA: 0x0005AB3E File Offset: 0x00058D3E
		protected static void InvokeRpcRpcRequestUnlockables(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcRequestUnlockables called on server.");
				return;
			}
			((NetworkUser)obj).RpcRequestUnlockables();
		}

		// Token: 0x06001284 RID: 4740 RVA: 0x0005AB64 File Offset: 0x00058D64
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

		// Token: 0x06001285 RID: 4741 RVA: 0x0005ABD8 File Offset: 0x00058DD8
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

		// Token: 0x06001286 RID: 4742 RVA: 0x0005AC4C File Offset: 0x00058E4C
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

		// Token: 0x06001287 RID: 4743 RVA: 0x0005ACB8 File Offset: 0x00058EB8
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
			if ((base.syncVarDirtyBits & 1u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				GeneratedNetworkCode._WriteNetworkUserId_None(writer, this._id);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32((uint)this.rewiredPlayerId);
			}
			if ((base.syncVarDirtyBits & 4u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this._masterObjectId);
			}
			if ((base.syncVarDirtyBits & 8u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.userColor);
			}
			if ((base.syncVarDirtyBits & 16u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.WritePackedUInt32(this.netLunarCoins);
			}
			if ((base.syncVarDirtyBits & 32u) != 0u)
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

		// Token: 0x06001288 RID: 4744 RVA: 0x0005AE60 File Offset: 0x00059060
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

		// Token: 0x04001617 RID: 5655
		private static readonly List<NetworkUser> instancesList = new List<NetworkUser>();

		// Token: 0x04001618 RID: 5656
		public static readonly ReadOnlyCollection<NetworkUser> readOnlyInstancesList = new ReadOnlyCollection<NetworkUser>(NetworkUser.instancesList);

		// Token: 0x04001619 RID: 5657
		private static readonly List<NetworkUser> localPlayers = new List<NetworkUser>();

		// Token: 0x0400161A RID: 5658
		public static readonly ReadOnlyCollection<NetworkUser> readOnlyLocalPlayersList = new ReadOnlyCollection<NetworkUser>(NetworkUser.localPlayers);

		// Token: 0x0400161B RID: 5659
		[SyncVar(hook = "OnSyncId")]
		private NetworkUserId _id;

		// Token: 0x0400161C RID: 5660
		[SyncVar]
		public byte rewiredPlayerId;

		// Token: 0x0400161D RID: 5661
		[SyncVar(hook = "OnSyncMasterObjectId")]
		private NetworkInstanceId _masterObjectId;

		// Token: 0x0400161E RID: 5662
		[CanBeNull]
		public LocalUser localUser;

		// Token: 0x0400161F RID: 5663
		public CameraRigController cameraRigController;

		// Token: 0x04001620 RID: 5664
		public string userName = "";

		// Token: 0x04001621 RID: 5665
		[SyncVar]
		public Color32 userColor = Color.red;

		// Token: 0x04001622 RID: 5666
		[SyncVar]
		private uint netLunarCoins;

		// Token: 0x04001623 RID: 5667
		private MemoizedGetComponent<CharacterMaster> cachedMaster;

		// Token: 0x04001624 RID: 5668
		private MemoizedGetComponent<PlayerCharacterMasterController> cachedPlayerCharacterMasterController;

		// Token: 0x04001625 RID: 5669
		private MemoizedGetComponent<PlayerStatsComponent> cachedPlayerStatsComponent;

		// Token: 0x04001626 RID: 5670
		private GameObject _masterObject;

		// Token: 0x04001627 RID: 5671
		[SyncVar(hook = "SetBodyPreference")]
		[NonSerialized]
		public int bodyIndexPreference = -1;

		// Token: 0x04001628 RID: 5672
		private float secondAccumulator;

		// Token: 0x04001629 RID: 5673
		[NonSerialized]
		public List<UnlockableDef> unlockables = new List<UnlockableDef>();

		// Token: 0x0400162A RID: 5674
		public List<string> debugUnlockablesList = new List<string>();

		// Token: 0x0400162F RID: 5679
		private static int kRpcRpcDeductLunarCoins;

		// Token: 0x04001630 RID: 5680
		private static int kRpcRpcAwardLunarCoins;

		// Token: 0x04001631 RID: 5681
		private static int kCmdCmdSetNetLunarCoins = -934763456;

		// Token: 0x04001632 RID: 5682
		private static int kCmdCmdSetBodyPreference;

		// Token: 0x04001633 RID: 5683
		private static int kCmdCmdSendConsoleCommand;

		// Token: 0x04001634 RID: 5684
		private static int kCmdCmdSendNewUnlockables;

		// Token: 0x04001635 RID: 5685
		private static int kRpcRpcRequestUnlockables;

		// Token: 0x04001636 RID: 5686
		private static int kCmdCmdReportAchievement;

		// Token: 0x04001637 RID: 5687
		private static int kCmdCmdReportUnlock;

		// Token: 0x04001638 RID: 5688
		private static int kCmdCmdSubmitVote;

		// Token: 0x02000372 RID: 882
		// (Invoke) Token: 0x0600128A RID: 4746
		public delegate void NetworkUserGenericDelegate(NetworkUser networkUser);
	}
}
