using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using RoR2.Networking;
using Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000374 RID: 884
	public class VoteController : NetworkBehaviour
	{
		// Token: 0x0600157E RID: 5502 RVA: 0x0005BB77 File Offset: 0x00059D77
		[Server]
		private void StartTimer()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.VoteController::StartTimer()' called on client");
				return;
			}
			if (this.timerIsActive)
			{
				return;
			}
			this.NetworktimerIsActive = true;
			this.Networktimer = this.timeoutDuration;
		}

		// Token: 0x0600157F RID: 5503 RVA: 0x0005BBAA File Offset: 0x00059DAA
		[Server]
		private void StopTimer()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.VoteController::StopTimer()' called on client");
				return;
			}
			this.NetworktimerIsActive = false;
			this.Networktimer = this.timeoutDuration;
		}

		// Token: 0x06001580 RID: 5504 RVA: 0x0005BBD4 File Offset: 0x00059DD4
		[Server]
		private void InitializeVoters()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.VoteController::InitializeVoters()' called on client");
				return;
			}
			this.StopTimer();
			this.votes.Clear();
			IEnumerable<NetworkUser> source = NetworkUser.readOnlyInstancesList;
			if (this.onlyAllowParticipatingPlayers)
			{
				source = from v in source
				where v.isParticipating
				select v;
			}
			foreach (GameObject networkUserObject in from v in source
			select v.gameObject)
			{
				this.votes.Add(new VoteController.UserVote
				{
					networkUserObject = networkUserObject,
					voteChoiceIndex = -1
				});
			}
		}

		// Token: 0x06001581 RID: 5505 RVA: 0x0005BCB8 File Offset: 0x00059EB8
		[Server]
		private void AddUserToVoters(NetworkUser networkUser)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.VoteController::AddUserToVoters(RoR2.NetworkUser)' called on client");
				return;
			}
			if (this.onlyAllowParticipatingPlayers && !networkUser.isParticipating)
			{
				return;
			}
			if (this.votes.Any((VoteController.UserVote v) => v.networkUserObject == networkUser.gameObject))
			{
				return;
			}
			this.votes.Add(new VoteController.UserVote
			{
				networkUserObject = networkUser.gameObject,
				voteChoiceIndex = -1
			});
		}

		// Token: 0x06001582 RID: 5506 RVA: 0x0005BD48 File Offset: 0x00059F48
		private void Awake()
		{
			if (NetworkServer.active)
			{
				if (this.timerStartCondition == VoteController.TimerStartCondition.Immediate)
				{
					this.StartTimer();
				}
				if (this.addNewPlayers)
				{
					NetworkUser.OnPostNetworkUserStart += this.AddUserToVoters;
				}
				GameNetworkManager.onServerConnectGlobal += this.OnServerConnectGlobal;
				GameNetworkManager.onServerDisconnectGlobal += this.OnServerDisconnectGlobal;
			}
			this.votes.InitializeBehaviour(this, VoteController.kListvotes);
		}

		// Token: 0x06001583 RID: 5507 RVA: 0x0005BDB6 File Offset: 0x00059FB6
		private void OnServerConnectGlobal(NetworkConnection conn)
		{
			if (this.resetOnConnectionsChanged)
			{
				this.InitializeVoters();
			}
		}

		// Token: 0x06001584 RID: 5508 RVA: 0x0005BDB6 File Offset: 0x00059FB6
		private void OnServerDisconnectGlobal(NetworkConnection conn)
		{
			if (this.resetOnConnectionsChanged)
			{
				this.InitializeVoters();
			}
		}

		// Token: 0x06001585 RID: 5509 RVA: 0x0005BDC6 File Offset: 0x00059FC6
		private void OnDestroy()
		{
			NetworkUser.OnPostNetworkUserStart -= this.AddUserToVoters;
			GameNetworkManager.onServerConnectGlobal -= this.OnServerConnectGlobal;
			GameNetworkManager.onServerDisconnectGlobal -= this.OnServerDisconnectGlobal;
		}

		// Token: 0x06001586 RID: 5510 RVA: 0x0005BDFB File Offset: 0x00059FFB
		public override void OnStartServer()
		{
			base.OnStartServer();
			this.InitializeVoters();
		}

		// Token: 0x06001587 RID: 5511 RVA: 0x0005BE0C File Offset: 0x0005A00C
		[Server]
		public void ReceiveUserVote(NetworkUser networkUser, int voteChoiceIndex)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.VoteController::ReceiveUserVote(RoR2.NetworkUser,System.Int32)' called on client");
				return;
			}
			if (this.resetOnConnectionsChanged)
			{
				int connectingClientCount = GameNetworkManager.singleton.GetConnectingClientCount();
				if (connectingClientCount > 0)
				{
					Debug.LogFormat("Vote from user \"{0}\" rejected: {1} clients are currently still in the process of connecting.", new object[]
					{
						networkUser.userName,
						connectingClientCount
					});
					return;
				}
			}
			if (voteChoiceIndex < 0 && !this.canRevokeVote)
			{
				return;
			}
			if (voteChoiceIndex >= this.choices.Length)
			{
				return;
			}
			GameObject gameObject = networkUser.gameObject;
			for (int i = 0; i < (int)this.votes.Count; i++)
			{
				if (gameObject == this.votes[i].networkUserObject)
				{
					if (this.votes[i].receivedVote && !this.canChangeVote)
					{
						return;
					}
					this.votes[i] = new VoteController.UserVote
					{
						networkUserObject = gameObject,
						voteChoiceIndex = voteChoiceIndex
					};
				}
			}
		}

		// Token: 0x06001588 RID: 5512 RVA: 0x0005BEFB File Offset: 0x0005A0FB
		private void Update()
		{
			if (NetworkServer.active)
			{
				this.ServerUpdate();
			}
		}

		// Token: 0x06001589 RID: 5513 RVA: 0x0005BF0C File Offset: 0x0005A10C
		[Server]
		private void ServerUpdate()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.VoteController::ServerUpdate()' called on client");
				return;
			}
			if (this.timerIsActive)
			{
				this.Networktimer = this.timer - Time.deltaTime;
				if (this.timer < 0f)
				{
					this.Networktimer = 0f;
				}
			}
			int num = 0;
			for (int i = (int)(this.votes.Count - 1); i >= 0; i--)
			{
				if (!this.votes[i].networkUserObject)
				{
					this.votes.RemoveAt(i);
				}
				else if (this.votes[i].receivedVote)
				{
					num++;
				}
			}
			bool flag = num > 0;
			bool flag2 = num == (int)this.votes.Count;
			if (flag)
			{
				if (this.timerStartCondition == VoteController.TimerStartCondition.OnAnyVoteReceived || this.timerStartCondition == VoteController.TimerStartCondition.WhileAnyVoteReceived)
				{
					this.StartTimer();
				}
			}
			else if (this.timerStartCondition == VoteController.TimerStartCondition.WhileAnyVoteReceived)
			{
				this.StopTimer();
			}
			if (flag2)
			{
				if (this.timerStartCondition == VoteController.TimerStartCondition.WhileAllVotesReceived)
				{
					this.StartTimer();
				}
				else if (RoR2Application.isInSinglePlayer)
				{
					this.Networktimer = 0f;
				}
				else
				{
					this.Networktimer = Mathf.Min(this.timer, this.minimumTimeBeforeProcessing);
				}
			}
			else if (this.timerStartCondition == VoteController.TimerStartCondition.WhileAllVotesReceived)
			{
				this.StopTimer();
			}
			if ((flag2 && !this.mustTimeOut) || (this.timerIsActive && this.timer <= 0f))
			{
				this.FinishVote();
			}
		}

		// Token: 0x0600158A RID: 5514 RVA: 0x0005C07C File Offset: 0x0005A27C
		[Server]
		private void FinishVote()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.VoteController::FinishVote()' called on client");
				return;
			}
			IGrouping<int, VoteController.UserVote> grouping = (from v in this.votes
			where v.receivedVote
			group v by v.voteChoiceIndex into v
			orderby v.Count<VoteController.UserVote>() descending
			select v).FirstOrDefault<IGrouping<int, VoteController.UserVote>>();
			int num = (grouping == null) ? this.defaultChoiceIndex : grouping.Key;
			if (num >= this.choices.Length)
			{
				num = this.defaultChoiceIndex;
			}
			if (num < this.choices.Length)
			{
				this.choices[num].Invoke();
			}
			base.enabled = false;
			this.NetworktimerIsActive = false;
			this.Networktimer = 0f;
			if (this.destroyGameObjectOnComplete)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x0600158B RID: 5515 RVA: 0x0005C17E File Offset: 0x0005A37E
		public int GetVoteCount()
		{
			return (int)this.votes.Count;
		}

		// Token: 0x0600158C RID: 5516 RVA: 0x0005C18B File Offset: 0x0005A38B
		public VoteController.UserVote GetVote(int i)
		{
			return this.votes[i];
		}

		// Token: 0x0600158E RID: 5518 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x1700028E RID: 654
		// (get) Token: 0x0600158F RID: 5519 RVA: 0x0005C1D0 File Offset: 0x0005A3D0
		// (set) Token: 0x06001590 RID: 5520 RVA: 0x0005C1E3 File Offset: 0x0005A3E3
		public bool NetworktimerIsActive
		{
			get
			{
				return this.timerIsActive;
			}
			[param: In]
			set
			{
				base.SetSyncVar<bool>(value, ref this.timerIsActive, 2U);
			}
		}

		// Token: 0x1700028F RID: 655
		// (get) Token: 0x06001591 RID: 5521 RVA: 0x0005C1F8 File Offset: 0x0005A3F8
		// (set) Token: 0x06001592 RID: 5522 RVA: 0x0005C20B File Offset: 0x0005A40B
		public float Networktimer
		{
			get
			{
				return this.timer;
			}
			[param: In]
			set
			{
				base.SetSyncVar<float>(value, ref this.timer, 4U);
			}
		}

		// Token: 0x06001593 RID: 5523 RVA: 0x0005C21F File Offset: 0x0005A41F
		protected static void InvokeSyncListvotes(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("SyncList votes called on server.");
				return;
			}
			((VoteController)obj).votes.HandleMsg(reader);
		}

		// Token: 0x06001594 RID: 5524 RVA: 0x0005C248 File Offset: 0x0005A448
		static VoteController()
		{
			NetworkBehaviour.RegisterSyncListDelegate(typeof(VoteController), VoteController.kListvotes, new NetworkBehaviour.CmdDelegate(VoteController.InvokeSyncListvotes));
			NetworkCRC.RegisterBehaviour("VoteController", 0);
		}

		// Token: 0x06001595 RID: 5525 RVA: 0x0005C284 File Offset: 0x0005A484
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				GeneratedNetworkCode._WriteStructSyncListUserVote_VoteController(writer, this.votes);
				writer.Write(this.timerIsActive);
				writer.Write(this.timer);
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
				GeneratedNetworkCode._WriteStructSyncListUserVote_VoteController(writer, this.votes);
			}
			if ((base.syncVarDirtyBits & 2U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.timerIsActive);
			}
			if ((base.syncVarDirtyBits & 4U) != 0U)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.timer);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001596 RID: 5526 RVA: 0x0005C370 File Offset: 0x0005A570
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				GeneratedNetworkCode._ReadStructSyncListUserVote_VoteController(reader, this.votes);
				this.timerIsActive = reader.ReadBoolean();
				this.timer = reader.ReadSingle();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				GeneratedNetworkCode._ReadStructSyncListUserVote_VoteController(reader, this.votes);
			}
			if ((num & 2) != 0)
			{
				this.timerIsActive = reader.ReadBoolean();
			}
			if ((num & 4) != 0)
			{
				this.timer = reader.ReadSingle();
			}
		}

		// Token: 0x04001414 RID: 5140
		[Tooltip("Whether or not users must be participating in the run to be allowed to vote.")]
		public bool onlyAllowParticipatingPlayers = true;

		// Token: 0x04001415 RID: 5141
		[Tooltip("Whether or not to add new players to the voting pool when they connect.")]
		public bool addNewPlayers;

		// Token: 0x04001416 RID: 5142
		[Tooltip("Whether or not users are allowed to change their choice after submitting it.")]
		public bool canChangeVote;

		// Token: 0x04001417 RID: 5143
		[Tooltip("Whether or not users are allowed to revoke their vote entirely after submitting it.")]
		public bool canRevokeVote;

		// Token: 0x04001418 RID: 5144
		[Tooltip("If set, the vote cannot be completed early by all users submitting, and the timeout must occur.")]
		public bool mustTimeOut;

		// Token: 0x04001419 RID: 5145
		[Tooltip("Whether or not this vote must reset and be unvotable while someone is connecting or disconnecting.")]
		public bool resetOnConnectionsChanged;

		// Token: 0x0400141A RID: 5146
		[Tooltip("How long it takes for the vote to forcibly complete once the timer begins.")]
		public float timeoutDuration = 15f;

		// Token: 0x0400141B RID: 5147
		[Tooltip("How long it takes for action to be taken after the vote is complete.")]
		public float minimumTimeBeforeProcessing = 3f;

		// Token: 0x0400141C RID: 5148
		[Tooltip("What causes the timer to start counting down.")]
		public VoteController.TimerStartCondition timerStartCondition;

		// Token: 0x0400141D RID: 5149
		[Tooltip("An array of functions to be called based on the user vote.")]
		public UnityEvent[] choices;

		// Token: 0x0400141E RID: 5150
		[Tooltip("The choice to use when nobody votes or everybody who can vote quits.")]
		public int defaultChoiceIndex;

		// Token: 0x0400141F RID: 5151
		[Tooltip("Whether or not to destroy the attached GameObject when the vote completes.")]
		public bool destroyGameObjectOnComplete = true;

		// Token: 0x04001420 RID: 5152
		private VoteController.SyncListUserVote votes = new VoteController.SyncListUserVote();

		// Token: 0x04001421 RID: 5153
		[SyncVar]
		public bool timerIsActive;

		// Token: 0x04001422 RID: 5154
		[SyncVar]
		public float timer;

		// Token: 0x04001423 RID: 5155
		private static int kListvotes = 458257089;

		// Token: 0x02000375 RID: 885
		public enum TimerStartCondition
		{
			// Token: 0x04001425 RID: 5157
			Immediate,
			// Token: 0x04001426 RID: 5158
			OnAnyVoteReceived,
			// Token: 0x04001427 RID: 5159
			WhileAnyVoteReceived,
			// Token: 0x04001428 RID: 5160
			WhileAllVotesReceived,
			// Token: 0x04001429 RID: 5161
			Never
		}

		// Token: 0x02000376 RID: 886
		[Serializable]
		public struct UserVote
		{
			// Token: 0x17000290 RID: 656
			// (get) Token: 0x06001597 RID: 5527 RVA: 0x0005C3FB File Offset: 0x0005A5FB
			public bool receivedVote
			{
				get
				{
					return this.voteChoiceIndex >= 0;
				}
			}

			// Token: 0x0400142A RID: 5162
			public GameObject networkUserObject;

			// Token: 0x0400142B RID: 5163
			public int voteChoiceIndex;
		}

		// Token: 0x02000377 RID: 887
		public class SyncListUserVote : SyncListStruct<VoteController.UserVote>
		{
			// Token: 0x06001599 RID: 5529 RVA: 0x0005C411 File Offset: 0x0005A611
			public override void SerializeItem(NetworkWriter writer, VoteController.UserVote item)
			{
				writer.Write(item.networkUserObject);
				writer.WritePackedUInt32((uint)item.voteChoiceIndex);
			}

			// Token: 0x0600159A RID: 5530 RVA: 0x0005C42C File Offset: 0x0005A62C
			public override VoteController.UserVote DeserializeItem(NetworkReader reader)
			{
				return new VoteController.UserVote
				{
					networkUserObject = reader.ReadGameObject(),
					voteChoiceIndex = (int)reader.ReadPackedUInt32()
				};
			}
		}
	}
}
