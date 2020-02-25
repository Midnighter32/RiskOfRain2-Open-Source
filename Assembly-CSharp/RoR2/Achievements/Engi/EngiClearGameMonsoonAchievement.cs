using System;

namespace RoR2.Achievements.Engi
{
	// Token: 0x020006EC RID: 1772
	public class EngiClearGameMonsoonAchievement : BasePerSurvivorClearGameMonsoonAchievement
	{
		// Token: 0x06002935 RID: 10549 RVA: 0x000AD916 File Offset: 0x000ABB16
		protected override int LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("EngiBody");
		}
	}
}
