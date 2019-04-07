using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001EE RID: 494
	public struct ServerAchievementIndex : IEquatable<ServerAchievementIndex>
	{
		// Token: 0x0600098E RID: 2446 RVA: 0x00030923 File Offset: 0x0002EB23
		public bool Equals(ServerAchievementIndex other)
		{
			return this.intValue == other.intValue;
		}

		// Token: 0x0600098F RID: 2447 RVA: 0x00030933 File Offset: 0x0002EB33
		public override bool Equals(object obj)
		{
			return obj != null && obj is ServerAchievementIndex && this.Equals((ServerAchievementIndex)obj);
		}

		// Token: 0x06000990 RID: 2448 RVA: 0x00030950 File Offset: 0x0002EB50
		public override int GetHashCode()
		{
			return this.intValue.GetHashCode();
		}

		// Token: 0x06000991 RID: 2449 RVA: 0x0003095D File Offset: 0x0002EB5D
		public static ServerAchievementIndex operator ++(ServerAchievementIndex achievementIndex)
		{
			achievementIndex.intValue++;
			return achievementIndex;
		}

		// Token: 0x06000992 RID: 2450 RVA: 0x00030923 File Offset: 0x0002EB23
		public static bool operator ==(ServerAchievementIndex a, ServerAchievementIndex b)
		{
			return a.intValue == b.intValue;
		}

		// Token: 0x06000993 RID: 2451 RVA: 0x0003096C File Offset: 0x0002EB6C
		public static bool operator !=(ServerAchievementIndex a, ServerAchievementIndex b)
		{
			return a.intValue != b.intValue;
		}

		// Token: 0x04000CF3 RID: 3315
		[SerializeField]
		public int intValue;
	}
}
