using System;
using JetBrains.Annotations;

namespace RoR2
{
	// Token: 0x02000458 RID: 1112
	public static class TeamCatalog
	{
		// Token: 0x06001AF2 RID: 6898 RVA: 0x000725A0 File Offset: 0x000707A0
		static TeamCatalog()
		{
			TeamCatalog.Register(TeamIndex.Neutral, new TeamDef
			{
				nameToken = "TEAM_NEUTRAL_NAME",
				softCharacterLimit = 40
			});
			TeamCatalog.Register(TeamIndex.Player, new TeamDef
			{
				nameToken = "TEAM_PLAYER_NAME",
				softCharacterLimit = 20
			});
			TeamCatalog.Register(TeamIndex.Monster, new TeamDef
			{
				nameToken = "TEAM_MONSTER_NAME",
				softCharacterLimit = 40
			});
		}

		// Token: 0x06001AF3 RID: 6899 RVA: 0x00072612 File Offset: 0x00070812
		private static void Register(TeamIndex teamIndex, TeamDef teamDef)
		{
			TeamCatalog.teamDefs[(int)teamIndex] = teamDef;
		}

		// Token: 0x06001AF4 RID: 6900 RVA: 0x0007261C File Offset: 0x0007081C
		[CanBeNull]
		public static TeamDef GetTeamDef(TeamIndex teamIndex)
		{
			return HGArrayUtilities.GetSafe<TeamDef>(TeamCatalog.teamDefs, (int)teamIndex);
		}

		// Token: 0x0400187C RID: 6268
		private static TeamDef[] teamDefs = new TeamDef[3];
	}
}
