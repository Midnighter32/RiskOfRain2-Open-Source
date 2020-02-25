using System;

namespace RoR2.Achievements.Commando
{
	// Token: 0x020006F0 RID: 1776
	[RegisterAchievement("CommandoClearGameMonsoon", "Skins.Commando.Alt1", null, null)]
	public class CommandoClearGameMonsoonAchievement : BasePerSurvivorClearGameMonsoonAchievement
	{
		// Token: 0x06002945 RID: 10565 RVA: 0x000ADAE6 File Offset: 0x000ABCE6
		protected override int LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("CommandoBody");
		}
	}
}
