using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002D4 RID: 724
	[Serializable]
	public class DirectorCard
	{
		// Token: 0x06000E83 RID: 3715 RVA: 0x000478DC File Offset: 0x00045ADC
		public bool CardIsValid()
		{
			bool flag = string.IsNullOrEmpty(this.requiredUnlockable) || Run.instance.IsUnlockableUnlocked(this.requiredUnlockable);
			bool flag2 = !string.IsNullOrEmpty(this.forbiddenUnlockable) && Run.instance.DoesEveryoneHaveThisUnlockableUnlocked(this.forbiddenUnlockable);
			return Run.instance && Run.instance.stageClearCount >= this.minimumStageCompletions && flag && !flag2;
		}

		// Token: 0x04001293 RID: 4755
		public SpawnCard spawnCard;

		// Token: 0x04001294 RID: 4756
		[Tooltip("Should not be zero! EVER.")]
		public int cost;

		// Token: 0x04001295 RID: 4757
		public int selectionWeight;

		// Token: 0x04001296 RID: 4758
		public DirectorCore.MonsterSpawnDistance spawnDistance;

		// Token: 0x04001297 RID: 4759
		public bool allowAmbushSpawn = true;

		// Token: 0x04001298 RID: 4760
		public bool preventOverhead;

		// Token: 0x04001299 RID: 4761
		public int minimumStageCompletions;

		// Token: 0x0400129A RID: 4762
		public string requiredUnlockable;

		// Token: 0x0400129B RID: 4763
		public string forbiddenUnlockable;
	}
}
