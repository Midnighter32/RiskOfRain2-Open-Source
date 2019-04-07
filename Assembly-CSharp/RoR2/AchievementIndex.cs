using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001ED RID: 493
	public struct AchievementIndex
	{
		// Token: 0x0600098D RID: 2445 RVA: 0x00030914 File Offset: 0x0002EB14
		public static AchievementIndex operator ++(AchievementIndex achievementIndex)
		{
			achievementIndex.intValue++;
			return achievementIndex;
		}

		// Token: 0x04000CF2 RID: 3314
		[SerializeField]
		public int intValue;
	}
}
