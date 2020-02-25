using System;

namespace RoR2.Social
{
	// Token: 0x020004AF RID: 1199
	public struct SocialUserId
	{
		// Token: 0x06001D0C RID: 7436 RVA: 0x0007C652 File Offset: 0x0007A852
		public SocialUserId(CSteamID steamId)
		{
			this.steamId = steamId;
		}

		// Token: 0x04001A20 RID: 6688
		public readonly CSteamID steamId;
	}
}
