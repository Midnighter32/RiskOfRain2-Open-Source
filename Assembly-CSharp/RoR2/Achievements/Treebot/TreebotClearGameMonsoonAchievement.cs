using System;

namespace RoR2.Achievements.Treebot
{
	// Token: 0x020006D0 RID: 1744
	[RegisterAchievement("TreebotClearGameMonsoon", "Skins.Treebot.Alt1", "RescueTreebot", null)]
	public class TreebotClearGameMonsoonAchievement : BasePerSurvivorClearGameMonsoonAchievement
	{
		// Token: 0x0600288C RID: 10380 RVA: 0x000AC8CA File Offset: 0x000AAACA
		protected override int LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("TreebotBody");
		}
	}
}
