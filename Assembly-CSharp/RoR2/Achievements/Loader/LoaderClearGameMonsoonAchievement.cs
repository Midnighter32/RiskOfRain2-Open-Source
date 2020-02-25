using System;

namespace RoR2.Achievements.Loader
{
	// Token: 0x020006E6 RID: 1766
	[RegisterAchievement("LoaderClearGameMonsoon", "Skins.Loader.Alt1", "DefeatSuperRoboBallBoss", null)]
	public class LoaderClearGameMonsoonAchievement : BasePerSurvivorClearGameMonsoonAchievement
	{
		// Token: 0x06002907 RID: 10503 RVA: 0x000AB9EB File Offset: 0x000A9BEB
		protected override int LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("LoaderBody");
		}
	}
}
