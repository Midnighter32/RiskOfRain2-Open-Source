using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001E0 RID: 480
	public class DirectorSpawnRequest
	{
		// Token: 0x06000A1E RID: 2590 RVA: 0x0002C2AF File Offset: 0x0002A4AF
		public DirectorSpawnRequest(SpawnCard spawnCard, DirectorPlacementRule placementRule, Xoroshiro128Plus rng)
		{
			this.spawnCard = spawnCard;
			this.placementRule = placementRule;
			this.rng = rng;
		}

		// Token: 0x04000A6E RID: 2670
		public SpawnCard spawnCard;

		// Token: 0x04000A6F RID: 2671
		public DirectorPlacementRule placementRule;

		// Token: 0x04000A70 RID: 2672
		public Xoroshiro128Plus rng;

		// Token: 0x04000A71 RID: 2673
		public GameObject summonerBodyObject;

		// Token: 0x04000A72 RID: 2674
		public TeamIndex? teamIndexOverride;

		// Token: 0x04000A73 RID: 2675
		public bool ignoreTeamMemberLimit;
	}
}
