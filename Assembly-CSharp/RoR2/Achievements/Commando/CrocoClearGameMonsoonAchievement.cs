using System;

namespace RoR2.Achievements.Commando
{
	// Token: 0x020006F4 RID: 1780
	[RegisterAchievement("CrocoClearGameMonsoon", "Skins.Croco.Alt1", "BeatArena", null)]
	public class CrocoClearGameMonsoonAchievement : BasePerSurvivorClearGameMonsoonAchievement
	{
		// Token: 0x06002958 RID: 10584 RVA: 0x000ADD9B File Offset: 0x000ABF9B
		protected override int LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("CrocoBody");
		}
	}
}
