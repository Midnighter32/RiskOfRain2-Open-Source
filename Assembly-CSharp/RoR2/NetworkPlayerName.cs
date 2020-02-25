using System;
using Facepunch.Steamworks;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003D4 RID: 980
	public struct NetworkPlayerName
	{
		// Token: 0x060017CF RID: 6095 RVA: 0x00067385 File Offset: 0x00065585
		public void Deserialize(NetworkReader reader)
		{
			if (reader.ReadBoolean())
			{
				this.steamId = CSteamID.nil;
				this.nameOverride = reader.ReadString();
				return;
			}
			this.steamId = new CSteamID(reader.ReadUInt64());
			this.nameOverride = null;
		}

		// Token: 0x060017D0 RID: 6096 RVA: 0x000673C0 File Offset: 0x000655C0
		public void Serialize(NetworkWriter writer)
		{
			bool flag = this.nameOverride != null;
			writer.Write(flag);
			if (flag)
			{
				writer.Write(this.nameOverride);
				return;
			}
			writer.Write(this.steamId.value);
		}

		// Token: 0x060017D1 RID: 6097 RVA: 0x00067400 File Offset: 0x00065600
		public string GetResolvedName()
		{
			if (this.nameOverride != null)
			{
				return this.nameOverride;
			}
			Client instance = Client.Instance;
			if (instance != null)
			{
				return instance.Friends.GetName(this.steamId.value);
			}
			return "???";
		}

		// Token: 0x0400166A RID: 5738
		public CSteamID steamId;

		// Token: 0x0400166B RID: 5739
		public string nameOverride;
	}
}
