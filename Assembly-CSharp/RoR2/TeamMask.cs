using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000456 RID: 1110
	[Serializable]
	public struct TeamMask
	{
		// Token: 0x06001AEC RID: 6892 RVA: 0x000724DB File Offset: 0x000706DB
		public bool HasTeam(TeamIndex teamIndex)
		{
			return teamIndex >= TeamIndex.Neutral && teamIndex < TeamIndex.Count && ((ulong)this.a & 1UL << (int)teamIndex) > 0UL;
		}

		// Token: 0x06001AED RID: 6893 RVA: 0x000724FA File Offset: 0x000706FA
		public void AddTeam(TeamIndex teamIndex)
		{
			if (teamIndex < TeamIndex.Neutral || teamIndex >= TeamIndex.Count)
			{
				return;
			}
			this.a |= (byte)(1 << (int)teamIndex);
		}

		// Token: 0x06001AEE RID: 6894 RVA: 0x0007251A File Offset: 0x0007071A
		public void RemoveTeam(TeamIndex teamIndex)
		{
			if (teamIndex < TeamIndex.Neutral || teamIndex >= TeamIndex.Count)
			{
				return;
			}
			this.a &= (byte)(~(byte)(1 << (int)teamIndex));
		}

		// Token: 0x06001AEF RID: 6895 RVA: 0x0007253C File Offset: 0x0007073C
		static TeamMask()
		{
			for (TeamIndex teamIndex = TeamIndex.Neutral; teamIndex < TeamIndex.Count; teamIndex += 1)
			{
				TeamMask.all.AddTeam(teamIndex);
			}
			TeamMask.allButNeutral = TeamMask.all;
			TeamMask.allButNeutral.RemoveTeam(TeamIndex.Neutral);
		}

		// Token: 0x06001AF0 RID: 6896 RVA: 0x00072584 File Offset: 0x00070784
		public static TeamMask AllExcept(TeamIndex teamIndexToExclude)
		{
			TeamMask result = TeamMask.all;
			result.RemoveTeam(teamIndexToExclude);
			return result;
		}

		// Token: 0x04001876 RID: 6262
		[SerializeField]
		public byte a;

		// Token: 0x04001877 RID: 6263
		public static readonly TeamMask none;

		// Token: 0x04001878 RID: 6264
		public static readonly TeamMask allButNeutral;

		// Token: 0x04001879 RID: 6265
		public static readonly TeamMask all = default(TeamMask);
	}
}
