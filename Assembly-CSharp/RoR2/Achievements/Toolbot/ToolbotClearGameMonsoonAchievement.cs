using System;

namespace RoR2.Achievements.Toolbot
{
	// Token: 0x020006D4 RID: 1748
	[RegisterAchievement("ToolbotClearGameMonsoon", "Skins.Toolbot.Alt1", "RepeatFirstTeleporter", null)]
	public class ToolbotClearGameMonsoonAchievement : BasePerSurvivorClearGameMonsoonAchievement
	{
		// Token: 0x060028A2 RID: 10402 RVA: 0x000ACB56 File Offset: 0x000AAD56
		protected override int LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("ToolbotBody");
		}
	}
}
