using System;

namespace RoR2
{
	// Token: 0x020001E6 RID: 486
	[Serializable]
	public class DirectorCard
	{
		// Token: 0x17000149 RID: 329
		// (get) Token: 0x06000A2E RID: 2606 RVA: 0x0002C801 File Offset: 0x0002AA01
		public int cost
		{
			get
			{
				return this.spawnCard.directorCreditCost;
			}
		}

		// Token: 0x06000A2F RID: 2607 RVA: 0x0002C810 File Offset: 0x0002AA10
		public bool CardIsValid()
		{
			bool flag = string.IsNullOrEmpty(this.requiredUnlockable) || Run.instance.IsUnlockableUnlocked(this.requiredUnlockable);
			bool flag2 = !string.IsNullOrEmpty(this.forbiddenUnlockable) && Run.instance.DoesEveryoneHaveThisUnlockableUnlocked(this.forbiddenUnlockable);
			return Run.instance && Run.instance.stageClearCount >= this.minimumStageCompletions && flag && !flag2;
		}

		// Token: 0x04000A89 RID: 2697
		public SpawnCard spawnCard;

		// Token: 0x04000A8A RID: 2698
		public int selectionWeight;

		// Token: 0x04000A8B RID: 2699
		public DirectorCore.MonsterSpawnDistance spawnDistance;

		// Token: 0x04000A8C RID: 2700
		public bool allowAmbushSpawn = true;

		// Token: 0x04000A8D RID: 2701
		public bool preventOverhead;

		// Token: 0x04000A8E RID: 2702
		public int minimumStageCompletions;

		// Token: 0x04000A8F RID: 2703
		public string requiredUnlockable;

		// Token: 0x04000A90 RID: 2704
		public string forbiddenUnlockable;
	}
}
