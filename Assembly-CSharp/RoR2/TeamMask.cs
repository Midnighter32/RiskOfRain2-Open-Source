using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020004C0 RID: 1216
	[Serializable]
	public struct TeamMask
	{
		// Token: 0x06001B5A RID: 7002 RVA: 0x0007FE9F File Offset: 0x0007E09F
		public bool HasTeam(TeamIndex teamIndex)
		{
			return teamIndex >= TeamIndex.Neutral && teamIndex < TeamIndex.Count && ((ulong)this.a & 1UL << (int)teamIndex) > 0UL;
		}

		// Token: 0x06001B5B RID: 7003 RVA: 0x0007FEBE File Offset: 0x0007E0BE
		public void AddTeam(TeamIndex teamIndex)
		{
			if (teamIndex < TeamIndex.Neutral || teamIndex >= TeamIndex.Count)
			{
				return;
			}
			this.a |= (byte)(1 << (int)teamIndex);
		}

		// Token: 0x06001B5C RID: 7004 RVA: 0x0007FEDE File Offset: 0x0007E0DE
		public void RemoveTeam(TeamIndex teamIndex)
		{
			if (teamIndex < TeamIndex.Neutral || teamIndex >= TeamIndex.Count)
			{
				return;
			}
			this.a &= (byte)(~(byte)(1 << (int)teamIndex));
		}

		// Token: 0x06001B5D RID: 7005 RVA: 0x0007FF00 File Offset: 0x0007E100
		static TeamMask()
		{
			for (TeamIndex teamIndex = TeamIndex.Neutral; teamIndex < TeamIndex.Count; teamIndex += 1)
			{
				TeamMask.all.AddTeam(teamIndex);
			}
			TeamMask.allButNeutral = TeamMask.all;
			TeamMask.allButNeutral.RemoveTeam(TeamIndex.Neutral);
		}

		// Token: 0x04001DFB RID: 7675
		[SerializeField]
		public byte a;

		// Token: 0x04001DFC RID: 7676
		public static readonly TeamMask none;

		// Token: 0x04001DFD RID: 7677
		public static readonly TeamMask allButNeutral;

		// Token: 0x04001DFE RID: 7678
		public static readonly TeamMask all = default(TeamMask);
	}
}
