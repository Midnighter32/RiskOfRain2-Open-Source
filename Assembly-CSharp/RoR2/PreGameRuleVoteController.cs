using System;
using System.Collections.Generic;
using System.Linq;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000393 RID: 915
	public class PreGameRuleVoteController : NetworkBehaviour
	{
		// Token: 0x06001344 RID: 4932 RVA: 0x0005E26C File Offset: 0x0005C46C
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

		// Token: 0x1400001C RID: 28
		// (add) Token: 0x06001345 RID: 4933 RVA: 0x0005E2E8 File Offset: 0x0005C4E8
		// (remove) Token: 0x06001346 RID: 4934 RVA: 0x0005E31C File Offset: 0x0005C51C
		public static event Action onVotesUpdated;

		// Token: 0x06001347 RID: 4935 RVA: 0x0005E34F File Offset: 0x0005C54F
		private void Start()
		{
			if (RoR2Application.isInSinglePlayer)
			{
				this.SetVotesFromRuleBookForSinglePlayer();
			}
			if (NetworkServer.active)
			{
				PreGameRuleVoteController.UpdateGameVotes();
			}
		}

		// Token: 0x06001348 RID: 4936 RVA: 0x0005E36C File Offset: 0x0005C56C
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

		// Token: 0x06001349 RID: 4937 RVA: 0x0005E3C0 File Offset: 0x0005C5C0
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

		// Token: 0x0600134A RID: 4938 RVA: 0x0005E464 File Offset: 0x0005C664
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
			NetworkIdentity networkIdentity = component.networkUserNetworkIdentity;
			if (!networkIdentity)
			{
				Debug.Log("PreGameRuleVoteController.ServerHandleClientVoteUpdate() failed: No NetworkIdentity");
				return;
			}
			NetworkUser component2 = networkIdentity.GetComponent<NetworkUser>();
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

		// Token: 0x0600134B RID: 4939 RVA: 0x0005E590 File Offset: 0x0005C790
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
				base.SetDirtyBit(2u);
			}
			PreGameRuleVoteController.shouldUpdateGameVotes = true;
		}

		// Token: 0x0600134C RID: 4940 RVA: 0x0005E600 File Offset: 0x0005C800
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

		// Token: 0x0600134D RID: 4941 RVA: 0x0005E7CC File Offset: 0x0005C9CC
		private void Awake()
		{
			PreGameRuleVoteController.instancesList.Add(this);
		}

		// Token: 0x0600134E RID: 4942 RVA: 0x0005E7DC File Offset: 0x0005C9DC
		private void OnDestroy()
		{
			int i = 0;
			int ruleCount = RuleCatalog.ruleCount;
			while (i < ruleCount)
			{
				this.SetVote(i, -1);
				i++;
			}
			PreGameRuleVoteController.instancesList.Remove(this);
		}

		// Token: 0x0600134F RID: 4943 RVA: 0x0005E810 File Offset: 0x0005CA10
		public override bool OnSerialize(NetworkWriter writer, bool initialState)
		{
			uint num = base.syncVarDirtyBits;
			if (initialState)
			{
				num = 3u;
			}
			writer.Write((byte)num);
			bool flag = (num & 1u) > 0u;
			bool flag2 = (num & 2u) > 0u;
			if (flag)
			{
				writer.Write(this.networkUserNetworkIdentity);
			}
			if (flag2)
			{
				this.WriteVotes(writer);
			}
			return !initialState && num > 0u;
		}

		// Token: 0x06001350 RID: 4944 RVA: 0x0005E860 File Offset: 0x0005CA60
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			byte b = reader.ReadByte();
			bool flag = (b & 1) > 0;
			bool flag2 = (b & 2) > 0;
			if (flag)
			{
				this.networkUserNetworkIdentity = reader.ReadNetworkIdentity();
			}
			if (flag2)
			{
				this.ReadVotes(reader);
			}
		}

		// Token: 0x06001351 RID: 4945 RVA: 0x0005E897 File Offset: 0x0005CA97
		private RuleChoiceDef GetDefaultChoice(RuleDef ruleDef)
		{
			return ruleDef.choices[PreGameController.instance.readOnlyRuleBook.GetRuleChoiceIndex(ruleDef)];
		}

		// Token: 0x06001352 RID: 4946 RVA: 0x0005E8B4 File Offset: 0x0005CAB4
		private void SetVotesFromRuleBookForSinglePlayer()
		{
			for (int i = 0; i < this.votes.Length; i++)
			{
				RuleDef ruleDef = RuleCatalog.GetRuleDef(i);
				this.votes[i].choiceValue = this.GetDefaultChoice(ruleDef).localIndex;
			}
			base.SetDirtyBit(2u);
		}

		// Token: 0x06001353 RID: 4947 RVA: 0x0005E900 File Offset: 0x0005CB00
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

		// Token: 0x06001354 RID: 4948 RVA: 0x0005E980 File Offset: 0x0005CB80
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
				base.SetDirtyBit(2u);
			}
		}

		// Token: 0x06001355 RID: 4949 RVA: 0x0005EA13 File Offset: 0x0005CC13
		public bool IsChoiceVoted(RuleChoiceDef ruleChoiceDef)
		{
			return this.votes[ruleChoiceDef.ruleDef.globalIndex].choiceValue == ruleChoiceDef.localIndex;
		}

		// Token: 0x06001356 RID: 4950 RVA: 0x0005EA38 File Offset: 0x0005CC38
		static PreGameRuleVoteController()
		{
			PreGameController.onServerRecalculatedModifierAvailability += delegate(PreGameController controller)
			{
				PreGameRuleVoteController.UpdateGameVotes();
			};
		}

		// Token: 0x06001358 RID: 4952 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x040016F2 RID: 5874
		private static readonly List<PreGameRuleVoteController> instancesList = new List<PreGameRuleVoteController>();

		// Token: 0x040016F4 RID: 5876
		private const byte networkUserIdentityDirtyBit = 1;

		// Token: 0x040016F5 RID: 5877
		private const byte votesDirtyBit = 2;

		// Token: 0x040016F6 RID: 5878
		private const byte allDirtyBits = 3;

		// Token: 0x040016F7 RID: 5879
		private PreGameRuleVoteController.Vote[] votes = new PreGameRuleVoteController.Vote[RuleCatalog.ruleCount];

		// Token: 0x040016F8 RID: 5880
		public static int[] votesForEachChoice = new int[RuleCatalog.choiceCount];

		// Token: 0x040016F9 RID: 5881
		private bool clientShouldTransmit;

		// Token: 0x040016FA RID: 5882
		public NetworkIdentity networkUserNetworkIdentity;

		// Token: 0x040016FB RID: 5883
		private static bool shouldUpdateGameVotes;

		// Token: 0x040016FC RID: 5884
		private readonly RuleMask ruleMaskBuffer = new RuleMask();

		// Token: 0x02000394 RID: 916
		[Serializable]
		private struct Vote
		{
			// Token: 0x170001B4 RID: 436
			// (get) Token: 0x06001359 RID: 4953 RVA: 0x0005EA8B File Offset: 0x0005CC8B
			public bool hasVoted
			{
				get
				{
					return this.internalValue > 0;
				}
			}

			// Token: 0x170001B5 RID: 437
			// (get) Token: 0x0600135A RID: 4954 RVA: 0x0005EA96 File Offset: 0x0005CC96
			// (set) Token: 0x0600135B RID: 4955 RVA: 0x0005EAA0 File Offset: 0x0005CCA0
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

			// Token: 0x0600135C RID: 4956 RVA: 0x0005EAAC File Offset: 0x0005CCAC
			public static void Serialize(NetworkWriter writer, PreGameRuleVoteController.Vote vote)
			{
				writer.Write(vote.internalValue);
			}

			// Token: 0x0600135D RID: 4957 RVA: 0x0005EABC File Offset: 0x0005CCBC
			public static PreGameRuleVoteController.Vote Deserialize(NetworkReader reader)
			{
				return new PreGameRuleVoteController.Vote
				{
					internalValue = reader.ReadByte()
				};
			}

			// Token: 0x040016FD RID: 5885
			[SerializeField]
			private byte internalValue;
		}
	}
}
