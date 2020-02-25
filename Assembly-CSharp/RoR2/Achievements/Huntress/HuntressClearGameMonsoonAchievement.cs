using System;

namespace RoR2.Achievements.Huntress
{
	// Token: 0x020006E8 RID: 1768
	[RegisterAchievement("HuntressClearGameMonsoon", "Skins.Huntress.Alt1", "CompleteThreeStages", null)]
	public class HuntressClearGameMonsoonAchievement : BasePerSurvivorClearGameMonsoonAchievement
	{
		// Token: 0x06002910 RID: 10512 RVA: 0x000AD590 File Offset: 0x000AB790
		protected override int LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("HuntressBody");
		}
	}
}
