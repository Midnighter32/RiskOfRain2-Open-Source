using System;

namespace RoR2.Social
{
	// Token: 0x0200050A RID: 1290
	public struct SocialUserId
	{
		// Token: 0x06001D2D RID: 7469 RVA: 0x00087E6A File Offset: 0x0008606A
		public SocialUserId(CSteamID steamId)
		{
			this.steamId = steamId;
		}

		// Token: 0x04001F4E RID: 8014
		public readonly CSteamID steamId;
	}
}
