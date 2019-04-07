using System;
using Facepunch.Steamworks;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200045D RID: 1117
	public struct NetworkPlayerName
	{
		// Token: 0x060018F6 RID: 6390 RVA: 0x00077A7D File Offset: 0x00075C7D
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

		// Token: 0x060018F7 RID: 6391 RVA: 0x00077AB8 File Offset: 0x00075CB8
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

		// Token: 0x060018F8 RID: 6392 RVA: 0x00077AF8 File Offset: 0x00075CF8
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

		// Token: 0x04001C68 RID: 7272
		public CSteamID steamId;

		// Token: 0x04001C69 RID: 7273
		public string nameOverride;
	}
}
