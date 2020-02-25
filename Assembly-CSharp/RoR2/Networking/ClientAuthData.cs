using System;
using Unity;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x02000558 RID: 1368
	public class ClientAuthData : MessageBase
	{
		// Token: 0x060020AD RID: 8365 RVA: 0x0008D699 File Offset: 0x0008B899
		public override void Serialize(NetworkWriter writer)
		{
			GeneratedNetworkCode._WriteCSteamID_None(writer, this.steamId);
			writer.WriteBytesFull(this.authTicket);
			writer.Write(this.password);
			writer.Write(this.version);
		}

		// Token: 0x060020AE RID: 8366 RVA: 0x0008D6CB File Offset: 0x0008B8CB
		public override void Deserialize(NetworkReader reader)
		{
			this.steamId = GeneratedNetworkCode._ReadCSteamID_None(reader);
			this.authTicket = reader.ReadBytesAndSize();
			this.password = reader.ReadString();
			this.version = reader.ReadString();
		}

		// Token: 0x04001DE1 RID: 7649
		public CSteamID steamId;

		// Token: 0x04001DE2 RID: 7650
		public byte[] authTicket;

		// Token: 0x04001DE3 RID: 7651
		public string password;

		// Token: 0x04001DE4 RID: 7652
		public string version;
	}
}
