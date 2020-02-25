using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020000AF RID: 175
	public struct ServerAchievementIndex : IEquatable<ServerAchievementIndex>
	{
		// Token: 0x0600034E RID: 846 RVA: 0x0000D673 File Offset: 0x0000B873
		public bool Equals(ServerAchievementIndex other)
		{
			return this.intValue == other.intValue;
		}

		// Token: 0x0600034F RID: 847 RVA: 0x0000D683 File Offset: 0x0000B883
		public override bool Equals(object obj)
		{
			return obj != null && obj is ServerAchievementIndex && this.Equals((ServerAchievementIndex)obj);
		}

		// Token: 0x06000350 RID: 848 RVA: 0x0000D6A0 File Offset: 0x0000B8A0
		public override int GetHashCode()
		{
			return this.intValue.GetHashCode();
		}

		// Token: 0x06000351 RID: 849 RVA: 0x0000D6AD File Offset: 0x0000B8AD
		public static ServerAchievementIndex operator ++(ServerAchievementIndex achievementIndex)
		{
			achievementIndex.intValue++;
			return achievementIndex;
		}

		// Token: 0x06000352 RID: 850 RVA: 0x0000D673 File Offset: 0x0000B873
		public static bool operator ==(ServerAchievementIndex a, ServerAchievementIndex b)
		{
			return a.intValue == b.intValue;
		}

		// Token: 0x06000353 RID: 851 RVA: 0x0000D6BC File Offset: 0x0000B8BC
		public static bool operator !=(ServerAchievementIndex a, ServerAchievementIndex b)
		{
			return a.intValue != b.intValue;
		}

		// Token: 0x04000305 RID: 773
		[SerializeField]
		public int intValue;
	}
}
