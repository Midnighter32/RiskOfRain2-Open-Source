using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020000AE RID: 174
	public struct AchievementIndex
	{
		// Token: 0x0600034D RID: 845 RVA: 0x0000D664 File Offset: 0x0000B864
		public static AchievementIndex operator ++(AchievementIndex achievementIndex)
		{
			achievementIndex.intValue++;
			return achievementIndex;
		}

		// Token: 0x04000304 RID: 772
		[SerializeField]
		public int intValue;
	}
}
