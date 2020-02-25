using System;

namespace RoR2.Achievements.Mage
{
	// Token: 0x020006DF RID: 1759
	[RegisterAchievement("MageClearGameMonsoon", "Skins.Mage.Alt1", "FreeMage", null)]
	public class MageClearGameMonsoonAchievement : BasePerSurvivorClearGameMonsoonAchievement
	{
		// Token: 0x060028E0 RID: 10464 RVA: 0x000AD129 File Offset: 0x000AB329
		protected override int LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("MageBody");
		}
	}
}
