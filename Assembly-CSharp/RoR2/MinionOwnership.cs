using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000285 RID: 645
	[DisallowMultipleComponent]
	public class MinionOwnership : NetworkBehaviour
	{
		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x06000E3E RID: 3646 RVA: 0x0003F977 File Offset: 0x0003DB77
		// (set) Token: 0x06000E3F RID: 3647 RVA: 0x0003F97F File Offset: 0x0003DB7F
		public CharacterMaster ownerMaster { get; private set; }

		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x06000E40 RID: 3648 RVA: 0x0003F988 File Offset: 0x0003DB88
		// (set) Token: 0x06000E41 RID: 3649 RVA: 0x0003F990 File Offset: 0x0003DB90
		public MinionOwnership.MinionGroup group { get; private set; }

		// Token: 0x06000E42 RID: 3650 RVA: 0x0003F999 File Offset: 0x0003DB99
		[Server]
		public void SetOwner(CharacterMaster newOwnerMaster)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.MinionOwnership::SetOwner(RoR2.CharacterMaster)' called on client");
				return;
			}
			this.NetworkownerMasterId = (newOwnerMaster ? newOwnerMaster.netId : NetworkInstanceId.Invalid);
			MinionOwnership.MinionGroup.SetMinionOwner(this, this.ownerMasterId);
		}

		// Token: 0x06000E43 RID: 3651 RVA: 0x0003F9D7 File Offset: 0x0003DBD7
		private void OnSyncOwnerMasterId(NetworkInstanceId newOwnerMasterId)
		{
			MinionOwnership.MinionGroup.SetMinionOwner(this, this.ownerMasterId);
		}

		// Token: 0x06000E44 RID: 3652 RVA: 0x0003F9E5 File Offset: 0x0003DBE5
		public override void OnStartClient()
		{
			base.OnStartClient();
			if (!NetworkServer.active)
			{
				MinionOwnership.MinionGroup.SetMinionOwner(this, this.ownerMasterId);
			}
		}

		// Token: 0x06000E45 RID: 3653 RVA: 0x0003FA00 File Offset: 0x0003DC00
		private void HandleGroupDiscovery(MinionOwnership.MinionGroup newGroup)
		{
			this.group = newGroup;
			Action<MinionOwnership> action = MinionOwnership.onMinionGroupChangedGlobal;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		// Token: 0x06000E46 RID: 3654 RVA: 0x0003FA1C File Offset: 0x0003DC1C
		private void HandleOwnerDiscovery(CharacterMaster newOwner)
		{
			if (this.ownerMaster != null)
			{
				Action<CharacterMaster> action = this.onOwnerLost;
				if (action != null)
				{
					action(this.ownerMaster);
				}
			}
			this.ownerMaster = newOwner;
			if (this.ownerMaster != null)
			{
				Action<CharacterMaster> action2 = this.onOwnerDiscovered;
				if (action2 != null)
				{
					action2(this.ownerMaster);
				}
			}
			Action<MinionOwnership> action3 = MinionOwnership.onMinionOwnerChangedGlobal;
			if (action3 == null)
			{
				return;
			}
			action3(this);
		}

		// Token: 0x06000E47 RID: 3655 RVA: 0x0003FA7E File Offset: 0x0003DC7E
		private void OnDestroy()
		{
			MinionOwnership.MinionGroup.SetMinionOwner(this, NetworkInstanceId.Invalid);
		}

		// Token: 0x14000020 RID: 32
		// (add) Token: 0x06000E48 RID: 3656 RVA: 0x0003FA8C File Offset: 0x0003DC8C
		// (remove) Token: 0x06000E49 RID: 3657 RVA: 0x0003FAC4 File Offset: 0x0003DCC4
		public event Action<CharacterMaster> onOwnerDiscovered;

		// Token: 0x14000021 RID: 33
		// (add) Token: 0x06000E4A RID: 3658 RVA: 0x0003FAFC File Offset: 0x0003DCFC
		// (remove) Token: 0x06000E4B RID: 3659 RVA: 0x0003FB34 File Offset: 0x0003DD34
		public event Action<CharacterMaster> onOwnerLost;

		// Token: 0x14000022 RID: 34
		// (add) Token: 0x06000E4C RID: 3660 RVA: 0x0003FB6C File Offset: 0x0003DD6C
		// (remove) Token: 0x06000E4D RID: 3661 RVA: 0x0003FBA0 File Offset: 0x0003DDA0
		public static event Action<MinionOwnership> onMinionGroupChangedGlobal;

		// Token: 0x14000023 RID: 35
		// (add) Token: 0x06000E4E RID: 3662 RVA: 0x0003FBD4 File Offset: 0x0003DDD4
		// (remove) Token: 0x06000E4F RID: 3663 RVA: 0x0003FC08 File Offset: 0x0003DE08
		public static event Action<MinionOwnership> onMinionOwnerChangedGlobal;

		// Token: 0x06000E50 RID: 3664 RVA: 0x0003FC3C File Offset: 0x0003DE3C
		[AssetCheck(typeof(CharacterMaster))]
		private static void AddMinionOwnershipComponent(AssetCheckArgs args)
		{
			CharacterMaster characterMaster = args.asset as CharacterMaster;
			if (!characterMaster.GetComponent<MinionOwnership>())
			{
				characterMaster.gameObject.AddComponent<MinionOwnership>();
				args.UpdatePrefab();
			}
		}

		// Token: 0x06000E51 RID: 3665 RVA: 0x0003FC74 File Offset: 0x0003DE74
		private void OnValidate()
		{
			if (base.GetComponents<MinionOwnership>().Length > 1)
			{
				Debug.LogError("Only one MinionOwnership is allowed per object!", this);
			}
		}

		// Token: 0x06000E53 RID: 3667 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x06000E54 RID: 3668 RVA: 0x0003FCA0 File Offset: 0x0003DEA0
		// (set) Token: 0x06000E55 RID: 3669 RVA: 0x0003FCB3 File Offset: 0x0003DEB3
		public NetworkInstanceId NetworkownerMasterId
		{
			get
			{
				return this.ownerMasterId;
			}
			[param: In]
			set
			{
				uint dirtyBit = 1U;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.OnSyncOwnerMasterId(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<NetworkInstanceId>(value, ref this.ownerMasterId, dirtyBit);
			}
		}

		// Token: 0x06000E56 RID: 3670 RVA: 0x0003FCF4 File Offset: 0x0003DEF4
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.ownerMasterId);
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
				writer.Write(this.ownerMasterId);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000E57 RID: 3671 RVA: 0x0003FD60 File Offset: 0x0003DF60
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.ownerMasterId = reader.ReadNetworkId();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.OnSyncOwnerMasterId(reader.ReadNetworkId());
			}
		}

		// Token: 0x04000E3E RID: 3646
		[SyncVar(hook = "OnSyncOwnerMasterId")]
		private NetworkInstanceId ownerMasterId = NetworkInstanceId.Invalid;

		// Token: 0x02000286 RID: 646
		public class MinionGroup : IDisposable
		{
			// Token: 0x06000E58 RID: 3672 RVA: 0x0003FDA4 File Offset: 0x0003DFA4
			public static void SetMinionOwner(MinionOwnership minion, NetworkInstanceId ownerId)
			{
				if (minion.group != null)
				{
					if (minion.group.ownerId == ownerId)
					{
						return;
					}
					MinionOwnership.MinionGroup.RemoveMinion(minion.group.ownerId, minion);
				}
				if (ownerId != NetworkInstanceId.Invalid)
				{
					MinionOwnership.MinionGroup.AddMinion(ownerId, minion);
				}
			}

			// Token: 0x06000E59 RID: 3673 RVA: 0x0003FDF4 File Offset: 0x0003DFF4
			private static void AddMinion(NetworkInstanceId ownerId, MinionOwnership minion)
			{
				MinionOwnership.MinionGroup minionGroup = null;
				for (int i = 0; i < MinionOwnership.MinionGroup.instancesList.Count; i++)
				{
					MinionOwnership.MinionGroup minionGroup2 = MinionOwnership.MinionGroup.instancesList[i];
					if (MinionOwnership.MinionGroup.instancesList[i].ownerId == ownerId)
					{
						minionGroup = minionGroup2;
						break;
					}
				}
				if (minionGroup == null)
				{
					minionGroup = new MinionOwnership.MinionGroup(ownerId);
				}
				minionGroup.AddMember(minion);
				minionGroup.AttemptToResolveOwner();
			}

			// Token: 0x06000E5A RID: 3674 RVA: 0x0003FE58 File Offset: 0x0003E058
			private static void RemoveMinion(NetworkInstanceId ownerId, MinionOwnership minion)
			{
				MinionOwnership.MinionGroup minionGroup = null;
				for (int i = 0; i < MinionOwnership.MinionGroup.instancesList.Count; i++)
				{
					MinionOwnership.MinionGroup minionGroup2 = MinionOwnership.MinionGroup.instancesList[i];
					if (MinionOwnership.MinionGroup.instancesList[i].ownerId == ownerId)
					{
						minionGroup = minionGroup2;
						break;
					}
				}
				if (minionGroup == null)
				{
					throw new InvalidOperationException(string.Format("{0}.{1} Could not find group to which {2} belongs", "MinionGroup", "RemoveMinion", minion));
				}
				minionGroup.RemoveMember(minion);
				if (minionGroup.refCount == 0 && !minionGroup.resolvedOwnerGameObject)
				{
					minionGroup.Dispose();
				}
			}

			// Token: 0x06000E5B RID: 3675 RVA: 0x0003FEE4 File Offset: 0x0003E0E4
			private MinionGroup(NetworkInstanceId ownerId)
			{
				this.ownerId = ownerId;
				this.members = new MinionOwnership[4];
				this._memberCount = 0;
				MinionOwnership.MinionGroup.instancesList.Add(this);
			}

			// Token: 0x06000E5C RID: 3676 RVA: 0x0003FF14 File Offset: 0x0003E114
			public void Dispose()
			{
				for (int i = this._memberCount - 1; i >= 0; i--)
				{
					this.RemoveMemberAt(i);
				}
				MinionOwnership.MinionGroup.instancesList.Remove(this);
			}

			// Token: 0x170001CB RID: 459
			// (get) Token: 0x06000E5D RID: 3677 RVA: 0x0003FF47 File Offset: 0x0003E147
			public int memberCount
			{
				get
				{
					return this._memberCount;
				}
			}

			// Token: 0x170001CC RID: 460
			// (get) Token: 0x06000E5E RID: 3678 RVA: 0x0003FF4F File Offset: 0x0003E14F
			public bool isMinion
			{
				get
				{
					return this.ownerId != NetworkInstanceId.Invalid;
				}
			}

			// Token: 0x06000E5F RID: 3679 RVA: 0x0003FF64 File Offset: 0x0003E164
			private void AttemptToResolveOwner()
			{
				if (this.resolved)
				{
					return;
				}
				this.resolvedOwnerGameObject = Util.FindNetworkObject(this.ownerId);
				if (!this.resolvedOwnerGameObject)
				{
					return;
				}
				this.resolved = true;
				this.resolvedOwnerMaster = this.resolvedOwnerGameObject.GetComponent<CharacterMaster>();
				this.resolvedOwnerGameObject.AddComponent<MinionOwnership.MinionGroup.MinionGroupDestroyer>().group = this;
				this.refCount++;
				for (int i = 0; i < this._memberCount; i++)
				{
					this.members[i].HandleOwnerDiscovery(this.resolvedOwnerMaster);
				}
			}

			// Token: 0x06000E60 RID: 3680 RVA: 0x0003FFF4 File Offset: 0x0003E1F4
			public void AddMember(MinionOwnership minion)
			{
				HGArrayUtilities.ArrayAppend<MinionOwnership>(ref this.members, ref this._memberCount, ref minion);
				this.refCount++;
				minion.HandleGroupDiscovery(this);
				if (this.resolvedOwnerMaster)
				{
					minion.HandleOwnerDiscovery(this.resolvedOwnerMaster);
				}
			}

			// Token: 0x06000E61 RID: 3681 RVA: 0x00040042 File Offset: 0x0003E242
			public void RemoveMember(MinionOwnership minion)
			{
				this.RemoveMemberAt(Array.IndexOf<MinionOwnership>(this.members, minion));
				this.refCount--;
			}

			// Token: 0x06000E62 RID: 3682 RVA: 0x00040064 File Offset: 0x0003E264
			private void RemoveMemberAt(int i)
			{
				MinionOwnership minionOwnership = this.members[i];
				HGArrayUtilities.ArrayRemoveAt<MinionOwnership>(ref this.members, ref this._memberCount, i, 1);
				minionOwnership.HandleOwnerDiscovery(null);
				minionOwnership.HandleGroupDiscovery(null);
			}

			// Token: 0x06000E63 RID: 3683 RVA: 0x00040090 File Offset: 0x0003E290
			[ConCommand(commandName = "minion_dump", flags = ConVarFlags.None, helpText = "Prints debug information about all active minion groups.")]
			private static void CCMinionPrint(ConCommandArgs args)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < MinionOwnership.MinionGroup.instancesList.Count; i++)
				{
					MinionOwnership.MinionGroup minionGroup = MinionOwnership.MinionGroup.instancesList[i];
					stringBuilder.Append("group [").Append(i).Append("] size=").Append(minionGroup._memberCount).Append(" id=").Append(minionGroup.ownerId).Append(" resolvedOwnerGameObject=").Append(minionGroup.resolvedOwnerGameObject).AppendLine();
					for (int j = 0; j < minionGroup._memberCount; j++)
					{
						stringBuilder.Append("  ").Append("[").Append(j).Append("] member.name=").Append(minionGroup.members[j].name).AppendLine();
					}
				}
				Debug.Log(stringBuilder.ToString());
			}

			// Token: 0x04000E45 RID: 3653
			private static readonly List<MinionOwnership.MinionGroup> instancesList = new List<MinionOwnership.MinionGroup>();

			// Token: 0x04000E46 RID: 3654
			public readonly NetworkInstanceId ownerId;

			// Token: 0x04000E47 RID: 3655
			private MinionOwnership[] members;

			// Token: 0x04000E48 RID: 3656
			private int _memberCount;

			// Token: 0x04000E49 RID: 3657
			private int refCount;

			// Token: 0x04000E4A RID: 3658
			private bool resolved;

			// Token: 0x04000E4B RID: 3659
			private GameObject resolvedOwnerGameObject;

			// Token: 0x04000E4C RID: 3660
			private CharacterMaster resolvedOwnerMaster;

			// Token: 0x02000287 RID: 647
			private class MinionGroupDestroyer : MonoBehaviour
			{
				// Token: 0x06000E65 RID: 3685 RVA: 0x0004018F File Offset: 0x0003E38F
				private void OnDestroy()
				{
					this.group.Dispose();
					this.group.refCount--;
				}

				// Token: 0x04000E4D RID: 3661
				public MinionOwnership.MinionGroup group;
			}
		}
	}
}
