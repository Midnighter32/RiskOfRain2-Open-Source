using System;
using System.Collections.Generic;
using System.Linq;
using RoR2.Networking;
using Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000418 RID: 1048
	public class VoteController : NetworkBehaviour
	{
		// Token: 0x06001746 RID: 5958 RVA: 0x0006E89F File Offset: 0x0006CA9F
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

		// Token: 0x06001747 RID: 5959 RVA: 0x0006E8D2 File Offset: 0x0006CAD2
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

		// Token: 0x06001748 RID: 5960 RVA: 0x0006E8FC File Offset: 0x0006CAFC
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

		// Token: 0x06001749 RID: 5961 RVA: 0x0006E9E0 File Offset: 0x0006CBE0
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

		// Token: 0x0600174A RID: 5962 RVA: 0x0006EA70 File Offset: 0x0006CC70
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

		// Token: 0x0600174B RID: 5963 RVA: 0x0006EADE File Offset: 0x0006CCDE
		private void OnServerConnectGlobal(NetworkConnection conn)
		{
			if (this.resetOnConnectionsChanged)
			{
				this.InitializeVoters();
			}
		}

		// Token: 0x0600174C RID: 5964 RVA: 0x0006EADE File Offset: 0x0006CCDE
		private void OnServerDisconnectGlobal(NetworkConnection conn)
		{
			if (this.resetOnConnectionsChanged)
			{
				this.InitializeVoters();
			}
		}

		// Token: 0x0600174D RID: 5965 RVA: 0x0006EAEE File Offset: 0x0006CCEE
		private void OnDestroy()
		{
			NetworkUser.OnPostNetworkUserStart -= this.AddUserToVoters;
			GameNetworkManager.onServerConnectGlobal -= this.OnServerConnectGlobal;
			GameNetworkManager.onServerDisconnectGlobal -= this.OnServerDisconnectGlobal;
		}

		// Token: 0x0600174E RID: 5966 RVA: 0x0006EB23 File Offset: 0x0006CD23
		public override void OnStartServer()
		{
			base.OnStartServer();
			this.InitializeVoters();
		}

		// Token: 0x0600174F RID: 5967 RVA: 0x0006EB34 File Offset: 0x0006CD34
		[Server]
		public void ReceiveUserVote(NetworkUser networkUser, int voteChoiceIndex)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.VoteController::ReceiveUserVote(RoR2.NetworkUser,System.Int32)' called on client");
				return;
			}
			if (this.resetOnConnectionsChanged && GameNetworkManager.singleton.GetConnectingClientCount() > 0)
			{
				return;
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
					if (!this.canChangeVote && this.votes[i].receivedVote)
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

		// Token: 0x06001750 RID: 5968 RVA: 0x0006EBFF File Offset: 0x0006CDFF
		private void Update()
		{
			if (NetworkServer.active)
			{
				this.ServerUpdate();
			}
		}

		// Token: 0x06001751 RID: 5969 RVA: 0x0006EC10 File Offset: 0x0006CE10
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

		// Token: 0x06001752 RID: 5970 RVA: 0x0006ED80 File Offset: 0x0006CF80
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

		// Token: 0x06001753 RID: 5971 RVA: 0x0006EE82 File Offset: 0x0006D082
		public int GetVoteCount()
		{
			return (int)this.votes.Count;
		}

		// Token: 0x06001754 RID: 5972 RVA: 0x0006EE8F File Offset: 0x0006D08F
		public VoteController.UserVote GetVote(int i)
		{
			return this.votes[i];
		}

		// Token: 0x06001756 RID: 5974 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x06001757 RID: 5975 RVA: 0x0006EED4 File Offset: 0x0006D0D4
		// (set) Token: 0x06001758 RID: 5976 RVA: 0x0006EEE7 File Offset: 0x0006D0E7
		public bool NetworktimerIsActive
		{
			get
			{
				return this.timerIsActive;
			}
			set
			{
				base.SetSyncVar<bool>(value, ref this.timerIsActive, 2u);
			}
		}

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x06001759 RID: 5977 RVA: 0x0006EEFC File Offset: 0x0006D0FC
		// (set) Token: 0x0600175A RID: 5978 RVA: 0x0006EF0F File Offset: 0x0006D10F
		public float Networktimer
		{
			get
			{
				return this.timer;
			}
			set
			{
				base.SetSyncVar<float>(value, ref this.timer, 4u);
			}
		}

		// Token: 0x0600175B RID: 5979 RVA: 0x0006EF23 File Offset: 0x0006D123
		protected static void InvokeSyncListvotes(NetworkBehaviour obj, NetworkReader reader)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("SyncList votes called on server.");
				return;
			}
			((VoteController)obj).votes.HandleMsg(reader);
		}

		// Token: 0x0600175C RID: 5980 RVA: 0x0006EF4C File Offset: 0x0006D14C
		static VoteController()
		{
			NetworkBehaviour.RegisterSyncListDelegate(typeof(VoteController), VoteController.kListvotes, new NetworkBehaviour.CmdDelegate(VoteController.InvokeSyncListvotes));
			NetworkCRC.RegisterBehaviour("VoteController", 0);
		}

		// Token: 0x0600175D RID: 5981 RVA: 0x0006EF88 File Offset: 0x0006D188
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
			if ((base.syncVarDirtyBits & 1u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				GeneratedNetworkCode._WriteStructSyncListUserVote_VoteController(writer, this.votes);
			}
			if ((base.syncVarDirtyBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.timerIsActive);
			}
			if ((base.syncVarDirtyBits & 4u) != 0u)
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

		// Token: 0x0600175E RID: 5982 RVA: 0x0006F074 File Offset: 0x0006D274
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

		// Token: 0x04001A7E RID: 6782
		[Tooltip("Whether or not users must be participating in the run to be allowed to vote.")]
		public bool onlyAllowParticipatingPlayers = true;

		// Token: 0x04001A7F RID: 6783
		[Tooltip("Whether or not to add new players to the voting pool when they connect.")]
		public bool addNewPlayers;

		// Token: 0x04001A80 RID: 6784
		[Tooltip("Whether or not users are allowed to change their choice after submitting it.")]
		public bool canChangeVote;

		// Token: 0x04001A81 RID: 6785
		[Tooltip("Whether or not users are allowed to revoke their vote entirely after submitting it.")]
		public bool canRevokeVote;

		// Token: 0x04001A82 RID: 6786
		[Tooltip("If set, the vote cannot be completed early by all users submitting, and the timeout must occur.")]
		public bool mustTimeOut;

		// Token: 0x04001A83 RID: 6787
		[Tooltip("Whether or not this vote must reset and be unvotable while someone is connecting or disconnecting.")]
		public bool resetOnConnectionsChanged;

		// Token: 0x04001A84 RID: 6788
		[Tooltip("How long it takes for the vote to forcibly complete once the timer begins.")]
		public float timeoutDuration = 15f;

		// Token: 0x04001A85 RID: 6789
		[Tooltip("How long it takes for action to be taken after the vote is complete.")]
		public float minimumTimeBeforeProcessing = 3f;

		// Token: 0x04001A86 RID: 6790
		[Tooltip("What causes the timer to start counting down.")]
		public VoteController.TimerStartCondition timerStartCondition;

		// Token: 0x04001A87 RID: 6791
		[Tooltip("An array of functions to be called based on the user vote.")]
		public UnityEvent[] choices;

		// Token: 0x04001A88 RID: 6792
		[Tooltip("The choice to use when nobody votes or everybody who can vote quits.")]
		public int defaultChoiceIndex;

		// Token: 0x04001A89 RID: 6793
		[Tooltip("Whether or not to destroy the attached GameObject when the vote completes.")]
		public bool destroyGameObjectOnComplete = true;

		// Token: 0x04001A8A RID: 6794
		private VoteController.SyncListUserVote votes = new VoteController.SyncListUserVote();

		// Token: 0x04001A8B RID: 6795
		[SyncVar]
		public bool timerIsActive;

		// Token: 0x04001A8C RID: 6796
		[SyncVar]
		public float timer;

		// Token: 0x04001A8D RID: 6797
		private static int kListvotes = 458257089;

		// Token: 0x02000419 RID: 1049
		public enum TimerStartCondition
		{
			// Token: 0x04001A8F RID: 6799
			Immediate,
			// Token: 0x04001A90 RID: 6800
			OnAnyVoteReceived,
			// Token: 0x04001A91 RID: 6801
			WhileAnyVoteReceived,
			// Token: 0x04001A92 RID: 6802
			WhileAllVotesReceived,
			// Token: 0x04001A93 RID: 6803
			Never
		}

		// Token: 0x0200041A RID: 1050
		[Serializable]
		public struct UserVote
		{
			// Token: 0x17000225 RID: 549
			// (get) Token: 0x0600175F RID: 5983 RVA: 0x0006F0FF File Offset: 0x0006D2FF
			public bool receivedVote
			{
				get
				{
					return this.voteChoiceIndex >= 0;
				}
			}

			// Token: 0x04001A94 RID: 6804
			public GameObject networkUserObject;

			// Token: 0x04001A95 RID: 6805
			public int voteChoiceIndex;
		}

		// Token: 0x0200041B RID: 1051
		public class SyncListUserVote : SyncListStruct<VoteController.UserVote>
		{
			// Token: 0x06001761 RID: 5985 RVA: 0x0006F115 File Offset: 0x0006D315
			public override void SerializeItem(NetworkWriter writer, VoteController.UserVote item)
			{
				writer.Write(item.networkUserObject);
				writer.WritePackedUInt32((uint)item.voteChoiceIndex);
			}

			// Token: 0x06001762 RID: 5986 RVA: 0x0006F130 File Offset: 0x0006D330
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
