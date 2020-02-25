using System;
using System.Collections.Generic;
using System.Linq;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002CF RID: 719
	public class PreGameRuleVoteController : NetworkBehaviour
	{
		// Token: 0x06001058 RID: 4184 RVA: 0x00047BA0 File Offset: 0x00045DA0
		public static PreGameRuleVoteController FindForUser(NetworkUser networkUser)
		{
			GameObject gameObject = networkUser.gameObject;
			foreach (PreGameRuleVoteController preGameRuleVoteController in PreGameRuleVoteController.instancesList)
			{
				if (preGameRuleVoteController.networkUserNetworkIdentity && preGameRuleVoteController.networkUserNetworkIdentity.gameObject == gameObject)
				{
					return preGameRuleVoteController;
				}
			}
			return null;
		}

		// Token: 0x06001059 RID: 4185 RVA: 0x00047C1C File Offset: 0x00045E1C
		public static void CreateForNetworkUserServer(NetworkUser networkUser)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/PreGameRuleVoteController"));
			PreGameRuleVoteController component = gameObject.GetComponent<PreGameRuleVoteController>();
			component.networkUserNetworkIdentity = networkUser.GetComponent<NetworkIdentity>();
			component.networkUser = networkUser;
			component.localUser = networkUser.localUser;
			NetworkServer.Spawn(gameObject);
		}

		// Token: 0x1400002D RID: 45
		// (add) Token: 0x0600105A RID: 4186 RVA: 0x00047C58 File Offset: 0x00045E58
		// (remove) Token: 0x0600105B RID: 4187 RVA: 0x00047C8C File Offset: 0x00045E8C
		public static event Action onVotesUpdated;

		// Token: 0x0600105C RID: 4188 RVA: 0x00047CBF File Offset: 0x00045EBF
		private static PreGameRuleVoteController.Vote[] CreateBallot()
		{
			return new PreGameRuleVoteController.Vote[RuleCatalog.ruleCount];
		}

		// Token: 0x0600105D RID: 4189 RVA: 0x00047CCB File Offset: 0x00045ECB
		[SystemInitializer(new Type[]
		{
			typeof(RuleCatalog)
		})]
		private static void Init()
		{
			PreGameRuleVoteController.votesForEachChoice = new int[RuleCatalog.choiceCount];
		}

		// Token: 0x0600105E RID: 4190 RVA: 0x00047CDC File Offset: 0x00045EDC
		private void Start()
		{
			if (this.localUser != null)
			{
				PreGameRuleVoteController.LocalUserBallotPersistenceManager.ApplyPersistentBallotIfPresent(this.localUser, this.votes);
			}
			if (NetworkServer.active)
			{
				PreGameRuleVoteController.UpdateGameVotes();
			}
		}

		// Token: 0x0600105F RID: 4191 RVA: 0x00047D04 File Offset: 0x00045F04
		private void Update()
		{
			if (NetworkServer.active && !this.networkUserNetworkIdentity)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			if (this.clientShouldTransmit)
			{
				this.clientShouldTransmit = false;
				this.ClientTransmitVotesToServer();
			}
			if (PreGameRuleVoteController.shouldUpdateGameVotes)
			{
				PreGameRuleVoteController.shouldUpdateGameVotes = false;
				PreGameRuleVoteController.UpdateGameVotes();
			}
		}

		// Token: 0x06001060 RID: 4192 RVA: 0x00047D58 File Offset: 0x00045F58
		[Client]
		private void ClientTransmitVotesToServer()
		{
			if (!NetworkClient.active)
			{
				Debug.LogWarning("[Client] function 'System.Void RoR2.PreGameRuleVoteController::ClientTransmitVotesToServer()' called on server");
				return;
			}
			Debug.Log("PreGameRuleVoteController.ClientTransmitVotesToServer()");
			if (!this.networkUserNetworkIdentity)
			{
				Debug.Log("Can't transmit votes: No network user object.");
				return;
			}
			NetworkUser component = this.networkUserNetworkIdentity.GetComponent<NetworkUser>();
			if (!component)
			{
				Debug.Log("Can't transmit votes: No network user component.");
				return;
			}
			NetworkWriter networkWriter = new NetworkWriter();
			networkWriter.StartMessage(70);
			networkWriter.Write(base.gameObject);
			this.WriteVotes(networkWriter);
			networkWriter.FinishMessage();
			component.connectionToServer.SendWriter(networkWriter, QosChannelIndex.defaultReliable.intVal);
		}

		// Token: 0x06001061 RID: 4193 RVA: 0x00047DFC File Offset: 0x00045FFC
		[NetworkMessageHandler(msgType = 70, client = false, server = true)]
		public static void ServerHandleClientVoteUpdate(NetworkMessage netMsg)
		{
			string format = "Received vote from {0}";
			object[] array = new object[1];
			int num = 0;
			NetworkUser networkUser = NetworkUser.readOnlyInstancesList.FirstOrDefault((NetworkUser v) => v.connectionToClient == netMsg.conn);
			array[num] = ((networkUser != null) ? networkUser.userName : null);
			Debug.LogFormat(format, array);
			GameObject gameObject = netMsg.reader.ReadGameObject();
			if (!gameObject)
			{
				Debug.Log("PreGameRuleVoteController.ServerHandleClientVoteUpdate() failed: preGameRuleVoteControllerObject=null");
				return;
			}
			PreGameRuleVoteController component = gameObject.GetComponent<PreGameRuleVoteController>();
			if (!component)
			{
				Debug.Log("PreGameRuleVoteController.ServerHandleClientVoteUpdate() failed: preGameRuleVoteController=null");
				return;
			}
			NetworkIdentity networkUserNetworkIdentity = component.networkUserNetworkIdentity;
			if (!networkUserNetworkIdentity)
			{
				Debug.Log("PreGameRuleVoteController.ServerHandleClientVoteUpdate() failed: No NetworkIdentity");
				return;
			}
			NetworkUser component2 = networkUserNetworkIdentity.GetComponent<NetworkUser>();
			if (!component2)
			{
				Debug.Log("PreGameRuleVoteController.ServerHandleClientVoteUpdate() failed: No NetworkUser");
				return;
			}
			if (component2.connectionToClient != netMsg.conn)
			{
				Debug.LogFormat("PreGameRuleVoteController.ServerHandleClientVoteUpdate() failed: {0}!={1}", new object[]
				{
					component.connectionToClient,
					netMsg.conn
				});
				return;
			}
			Debug.LogFormat("Accepting vote from {0}", new object[]
			{
				component2.userName
			});
			component.ReadVotes(netMsg.reader);
		}

		// Token: 0x06001062 RID: 4194 RVA: 0x00047F28 File Offset: 0x00046128
		public void SetVote(int ruleIndex, int choiceValue)
		{
			PreGameRuleVoteController.Vote vote = this.votes[ruleIndex];
			if (vote.choiceValue == choiceValue)
			{
				return;
			}
			this.votes[ruleIndex].choiceValue = choiceValue;
			if (!NetworkServer.active && this.networkUserNetworkIdentity && this.networkUserNetworkIdentity.isLocalPlayer)
			{
				this.clientShouldTransmit = true;
			}
			else
			{
				base.SetDirtyBit(2U);
			}
			PreGameRuleVoteController.shouldUpdateGameVotes = true;
		}

		// Token: 0x17000208 RID: 520
		// (get) Token: 0x06001063 RID: 4195 RVA: 0x00047F96 File Offset: 0x00046196
		// (set) Token: 0x06001064 RID: 4196 RVA: 0x00047F9E File Offset: 0x0004619E
		public NetworkIdentity networkUserNetworkIdentity { get; private set; }

		// Token: 0x06001065 RID: 4197 RVA: 0x00047FA8 File Offset: 0x000461A8
		private static void UpdateGameVotes()
		{
			int i = 0;
			int choiceCount = RuleCatalog.choiceCount;
			while (i < choiceCount)
			{
				PreGameRuleVoteController.votesForEachChoice[i] = 0;
				i++;
			}
			int j = 0;
			int ruleCount = RuleCatalog.ruleCount;
			while (j < ruleCount)
			{
				RuleDef ruleDef = RuleCatalog.GetRuleDef(j);
				int count = ruleDef.choices.Count;
				foreach (PreGameRuleVoteController preGameRuleVoteController in PreGameRuleVoteController.instancesList)
				{
					PreGameRuleVoteController.Vote vote = preGameRuleVoteController.votes[j];
					if (vote.hasVoted && vote.choiceValue < count)
					{
						RuleChoiceDef ruleChoiceDef = ruleDef.choices[vote.choiceValue];
						PreGameRuleVoteController.votesForEachChoice[ruleChoiceDef.globalIndex]++;
					}
				}
				j++;
			}
			if (NetworkServer.active)
			{
				int k = 0;
				int ruleCount2 = RuleCatalog.ruleCount;
				while (k < ruleCount2)
				{
					RuleDef ruleDef2 = RuleCatalog.GetRuleDef(k);
					int count2 = ruleDef2.choices.Count;
					PreGameController.instance.readOnlyRuleBook.GetRuleChoiceIndex(ruleDef2);
					int ruleChoiceIndex = -1;
					int num = 0;
					bool flag = false;
					for (int l = 0; l < count2; l++)
					{
						RuleChoiceDef ruleChoiceDef2 = ruleDef2.choices[l];
						int num2 = PreGameRuleVoteController.votesForEachChoice[ruleChoiceDef2.globalIndex];
						if (num2 == num)
						{
							flag = true;
						}
						else if (num2 > num)
						{
							ruleChoiceIndex = ruleChoiceDef2.globalIndex;
							num = num2;
							flag = false;
						}
					}
					if (num == 0)
					{
						ruleChoiceIndex = ruleDef2.choices[ruleDef2.defaultChoiceIndex].globalIndex;
					}
					if (!flag || num == 0)
					{
						PreGameController.instance.ApplyChoice(ruleChoiceIndex);
					}
					k++;
				}
			}
			Action action = PreGameRuleVoteController.onVotesUpdated;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x06001066 RID: 4198 RVA: 0x00048174 File Offset: 0x00046374
		private void Awake()
		{
			PreGameRuleVoteController.instancesList.Add(this);
		}

		// Token: 0x06001067 RID: 4199 RVA: 0x00048181 File Offset: 0x00046381
		private void OnDestroy()
		{
			PreGameRuleVoteController.shouldUpdateGameVotes = true;
			PreGameRuleVoteController.instancesList.Remove(this);
		}

		// Token: 0x06001068 RID: 4200 RVA: 0x00048198 File Offset: 0x00046398
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint num = base.syncVarDirtyBits;
			if (initialState)
			{
				num = 3U;
			}
			writer.Write((byte)num);
			bool flag = (num & 1U) > 0U;
			bool flag2 = (num & 2U) > 0U;
			if (flag)
			{
				writer.Write(this.networkUserNetworkIdentity);
			}
			if (flag2)
			{
				this.WriteVotes(writer);
			}
			return !initialState && num > 0U;
		}

		// Token: 0x06001069 RID: 4201 RVA: 0x000481E8 File Offset: 0x000463E8
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			byte b = reader.ReadByte();
			bool flag = (b & 1) > 0;
			bool flag2 = (b & 2) > 0;
			if (flag)
			{
				this.networkUserNetworkIdentity = reader.ReadNetworkIdentity();
				this.networkUser = (this.networkUserNetworkIdentity ? this.networkUserNetworkIdentity.GetComponent<NetworkUser>() : null);
				this.localUser = (this.networkUser ? this.networkUser.localUser : null);
			}
			if (flag2)
			{
				this.ReadVotes(reader);
			}
		}

		// Token: 0x0600106A RID: 4202 RVA: 0x00048261 File Offset: 0x00046461
		private RuleChoiceDef GetDefaultChoice(RuleDef ruleDef)
		{
			return ruleDef.choices[PreGameController.instance.readOnlyRuleBook.GetRuleChoiceIndex(ruleDef)];
		}

		// Token: 0x0600106B RID: 4203 RVA: 0x00048280 File Offset: 0x00046480
		private void SetVotesFromRuleBookForSinglePlayer()
		{
			for (int i = 0; i < this.votes.Length; i++)
			{
				RuleDef ruleDef = RuleCatalog.GetRuleDef(i);
				this.votes[i].choiceValue = this.GetDefaultChoice(ruleDef).localIndex;
			}
			base.SetDirtyBit(2U);
		}

		// Token: 0x0600106C RID: 4204 RVA: 0x000482CC File Offset: 0x000464CC
		private void WriteVotes(NetworkWriter writer)
		{
			int i = 0;
			int ruleCount = RuleCatalog.ruleCount;
			while (i < ruleCount)
			{
				this.ruleMaskBuffer[i] = this.votes[i].hasVoted;
				i++;
			}
			writer.Write(this.ruleMaskBuffer);
			int j = 0;
			int ruleCount2 = RuleCatalog.ruleCount;
			while (j < ruleCount2)
			{
				if (this.votes[j].hasVoted)
				{
					PreGameRuleVoteController.Vote.Serialize(writer, this.votes[j]);
				}
				j++;
			}
		}

		// Token: 0x0600106D RID: 4205 RVA: 0x0004834C File Offset: 0x0004654C
		private void ReadVotes(NetworkReader reader)
		{
			reader.ReadRuleMask(this.ruleMaskBuffer);
			bool flag = !this.networkUserNetworkIdentity || !this.networkUserNetworkIdentity.isLocalPlayer;
			int i = 0;
			int ruleCount = RuleCatalog.ruleCount;
			while (i < ruleCount)
			{
				PreGameRuleVoteController.Vote vote;
				if (this.ruleMaskBuffer[i])
				{
					vote = PreGameRuleVoteController.Vote.Deserialize(reader);
				}
				else
				{
					vote = default(PreGameRuleVoteController.Vote);
				}
				if (flag)
				{
					this.votes[i] = vote;
				}
				i++;
			}
			PreGameRuleVoteController.shouldUpdateGameVotes = (PreGameRuleVoteController.shouldUpdateGameVotes || flag);
			if (NetworkServer.active)
			{
				base.SetDirtyBit(2U);
			}
		}

		// Token: 0x0600106E RID: 4206 RVA: 0x000483DF File Offset: 0x000465DF
		public bool IsChoiceVoted(RuleChoiceDef ruleChoiceDef)
		{
			return this.votes[ruleChoiceDef.ruleDef.globalIndex].choiceValue == ruleChoiceDef.localIndex;
		}

		// Token: 0x0600106F RID: 4207 RVA: 0x00048404 File Offset: 0x00046604
		static PreGameRuleVoteController()
		{
			PreGameController.onServerRecalculatedModifierAvailability += delegate(PreGameController controller)
			{
				PreGameRuleVoteController.UpdateGameVotes();
			};
		}

		// Token: 0x06001071 RID: 4209 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x04000FBF RID: 4031
		private static readonly List<PreGameRuleVoteController> instancesList = new List<PreGameRuleVoteController>();

		// Token: 0x04000FC1 RID: 4033
		private const byte networkUserIdentityDirtyBit = 1;

		// Token: 0x04000FC2 RID: 4034
		private const byte votesDirtyBit = 2;

		// Token: 0x04000FC3 RID: 4035
		private const byte allDirtyBits = 3;

		// Token: 0x04000FC4 RID: 4036
		private PreGameRuleVoteController.Vote[] votes = PreGameRuleVoteController.CreateBallot();

		// Token: 0x04000FC5 RID: 4037
		public static int[] votesForEachChoice;

		// Token: 0x04000FC6 RID: 4038
		private bool clientShouldTransmit;

		// Token: 0x04000FC8 RID: 4040
		private NetworkUser networkUser;

		// Token: 0x04000FC9 RID: 4041
		private LocalUser localUser;

		// Token: 0x04000FCA RID: 4042
		private static bool shouldUpdateGameVotes;

		// Token: 0x04000FCB RID: 4043
		private readonly RuleMask ruleMaskBuffer = new RuleMask();

		// Token: 0x020002D0 RID: 720
		private static class LocalUserBallotPersistenceManager
		{
			// Token: 0x06001072 RID: 4210 RVA: 0x00048443 File Offset: 0x00046643
			static LocalUserBallotPersistenceManager()
			{
				LocalUserManager.onUserSignIn += PreGameRuleVoteController.LocalUserBallotPersistenceManager.OnLocalUserSignIn;
				LocalUserManager.onUserSignOut += PreGameRuleVoteController.LocalUserBallotPersistenceManager.OnLocalUserSignOut;
				PreGameRuleVoteController.onVotesUpdated += PreGameRuleVoteController.LocalUserBallotPersistenceManager.OnVotesUpdated;
			}

			// Token: 0x06001073 RID: 4211 RVA: 0x00048482 File Offset: 0x00046682
			private static void OnLocalUserSignIn(LocalUser localUser)
			{
				PreGameRuleVoteController.LocalUserBallotPersistenceManager.votesCache.Add(localUser, null);
			}

			// Token: 0x06001074 RID: 4212 RVA: 0x00048490 File Offset: 0x00046690
			private static void OnLocalUserSignOut(LocalUser localUser)
			{
				PreGameRuleVoteController.LocalUserBallotPersistenceManager.votesCache.Remove(localUser);
			}

			// Token: 0x06001075 RID: 4213 RVA: 0x000484A0 File Offset: 0x000466A0
			private static void OnVotesUpdated()
			{
				foreach (PreGameRuleVoteController preGameRuleVoteController in PreGameRuleVoteController.instancesList)
				{
					if (preGameRuleVoteController.localUser != null)
					{
						PreGameRuleVoteController.LocalUserBallotPersistenceManager.votesCache[preGameRuleVoteController.localUser] = preGameRuleVoteController.votes;
					}
				}
			}

			// Token: 0x06001076 RID: 4214 RVA: 0x0004850C File Offset: 0x0004670C
			public static void ApplyPersistentBallotIfPresent(LocalUser localUser, PreGameRuleVoteController.Vote[] dest)
			{
				PreGameRuleVoteController.Vote[] array;
				if (PreGameRuleVoteController.LocalUserBallotPersistenceManager.votesCache.TryGetValue(localUser, out array) && array != null)
				{
					Debug.LogFormat("Applying persistent ballot of votes for LocalUser {0}.", new object[]
					{
						localUser.userProfile.name
					});
					Array.Copy(array, dest, array.Length);
				}
			}

			// Token: 0x04000FCC RID: 4044
			private static readonly Dictionary<LocalUser, PreGameRuleVoteController.Vote[]> votesCache = new Dictionary<LocalUser, PreGameRuleVoteController.Vote[]>();
		}

		// Token: 0x020002D1 RID: 721
		[Serializable]
		private struct Vote
		{
			// Token: 0x17000209 RID: 521
			// (get) Token: 0x06001077 RID: 4215 RVA: 0x00048553 File Offset: 0x00046753
			public bool hasVoted
			{
				get
				{
					return this.internalValue > 0;
				}
			}

			// Token: 0x1700020A RID: 522
			// (get) Token: 0x06001078 RID: 4216 RVA: 0x0004855E File Offset: 0x0004675E
			// (set) Token: 0x06001079 RID: 4217 RVA: 0x00048568 File Offset: 0x00046768
			public int choiceValue
			{
				get
				{
					return (int)(this.internalValue - 1);
				}
				set
				{
					this.internalValue = (byte)(value + 1);
				}
			}

			// Token: 0x0600107A RID: 4218 RVA: 0x00048574 File Offset: 0x00046774
			public static void Serialize(NetworkWriter writer, PreGameRuleVoteController.Vote vote)
			{
				writer.Write(vote.internalValue);
			}

			// Token: 0x0600107B RID: 4219 RVA: 0x00048584 File Offset: 0x00046784
			public static PreGameRuleVoteController.Vote Deserialize(NetworkReader reader)
			{
				return new PreGameRuleVoteController.Vote
				{
					internalValue = reader.ReadByte()
				};
			}

			// Token: 0x04000FCD RID: 4045
			[SerializeField]
			private byte internalValue;
		}
	}
}
