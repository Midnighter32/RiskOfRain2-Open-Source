using System;
using System.Runtime.InteropServices;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200034C RID: 844
	public class TeamFilter : NetworkBehaviour
	{
		// Token: 0x17000265 RID: 613
		// (get) Token: 0x06001431 RID: 5169 RVA: 0x0005642E File Offset: 0x0005462E
		// (set) Token: 0x06001432 RID: 5170 RVA: 0x00056437 File Offset: 0x00054637
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

		// Token: 0x06001434 RID: 5172 RVA: 0x0000409B File Offset: 0x0000229B
		private void UNetVersion()
		{
		}

		// Token: 0x17000266 RID: 614
		// (get) Token: 0x06001435 RID: 5173 RVA: 0x00056440 File Offset: 0x00054640
		// (set) Token: 0x06001436 RID: 5174 RVA: 0x00056453 File Offset: 0x00054653
		public int NetworkteamIndexInternal
		{
			get
			{
				return this.teamIndexInternal;
			}
			[param: In]
			set
			{
				base.SetSyncVar<int>(value, ref this.teamIndexInternal, 1U);
			}
		}

		// Token: 0x06001437 RID: 5175 RVA: 0x00056468 File Offset: 0x00054668
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.WritePackedUInt32((uint)this.teamIndexInternal);
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
				writer.WritePackedUInt32((uint)this.teamIndexInternal);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06001438 RID: 5176 RVA: 0x000564D4 File Offset: 0x000546D4
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

		// Token: 0x040012F3 RID: 4851
		[SyncVar]
		private int teamIndexInternal;
	}
}
