using System;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace RoR2
{
	// Token: 0x020003F5 RID: 1013
	public class TeamFilter : NetworkBehaviour
	{
		// Token: 0x17000200 RID: 512
		// (get) Token: 0x0600163A RID: 5690 RVA: 0x0006A54E File Offset: 0x0006874E
		// (set) Token: 0x0600163B RID: 5691 RVA: 0x0006A557 File Offset: 0x00068757
		public TeamIndex teamIndex
		{
			get
			{
				return (TeamIndex)this.teamIndexInternal;
			}
			set
			{
				this.NetworkteamIndexInternal = (int)value;
			}
		}

		// Token: 0x0600163D RID: 5693 RVA: 0x00004507 File Offset: 0x00002707
		private void UNetVersion()
		{
		}

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x0600163E RID: 5694 RVA: 0x0006A560 File Offset: 0x00068760
		// (set) Token: 0x0600163F RID: 5695 RVA: 0x0006A573 File Offset: 0x00068773
		public int NetworkteamIndexInternal
		{
			get
			{
				return this.teamIndexInternal;
			}
			set
			{
				base.SetSyncVar<int>(value, ref this.teamIndexInternal, 1u);
			}
		}

		// Token: 0x06001640 RID: 5696 RVA: 0x0006A588 File Offset: 0x00068788
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.WritePackedUInt32((uint)this.teamIndexInternal);
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
				writer.WritePackedUInt32((uint)this.teamIndexInternal);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001641 RID: 5697 RVA: 0x0006A5F4 File Offset: 0x000687F4
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.teamIndexInternal = (int)reader.ReadPackedUInt32();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.teamIndexInternal = (int)reader.ReadPackedUInt32();
			}
		}

		// Token: 0x04001994 RID: 6548
		[SyncVar]
		[FormerlySerializedAs("teamIndex")]
		private int teamIndexInternal;
	}
}
