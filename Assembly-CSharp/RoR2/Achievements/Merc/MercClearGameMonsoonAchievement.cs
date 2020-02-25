using System;

namespace RoR2.Achievements.Merc
{
	// Token: 0x020006D9 RID: 1753
	[RegisterAchievement("MercClearGameMonsoon", "Skins.Merc.Alt1", "CompleteUnknownEnding", null)]
	public class MercClearGameMonsoonAchievement : BasePerSurvivorClearGameMonsoonAchievement
	{
		// Token: 0x060028B9 RID: 10425 RVA: 0x000ACDE8 File Offset: 0x000AAFE8
		protected override int LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("MercBody");
		}
	}
}
