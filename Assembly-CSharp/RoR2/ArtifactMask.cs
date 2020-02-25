using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020000BF RID: 191
	[Serializable]
	public struct ArtifactMask
	{
		// Token: 0x060003BE RID: 958 RVA: 0x0000E797 File Offset: 0x0000C997
		public bool HasArtifact(ArtifactIndex artifactIndex)
		{
			return artifactIndex >= ArtifactIndex.Command && artifactIndex < ArtifactIndex.Count && ((int)this.a & 1 << (int)artifactIndex) != 0;
		}

		// Token: 0x060003BF RID: 959 RVA: 0x0000E7B3 File Offset: 0x0000C9B3
		public void AddArtifact(ArtifactIndex artifactIndex)
		{
			if (artifactIndex < ArtifactIndex.Command || artifactIndex >= ArtifactIndex.Count)
			{
				return;
			}
			this.a |= (ushort)(1 << (int)artifactIndex);
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x0000E7D3 File Offset: 0x0000C9D3
		public void ToggleArtifact(ArtifactIndex artifactIndex)
		{
			if (artifactIndex < ArtifactIndex.Command || artifactIndex >= ArtifactIndex.Count)
			{
				return;
			}
			this.a ^= (ushort)(1 << (int)artifactIndex);
		}

		// Token: 0x060003C1 RID: 961 RVA: 0x0000E7F3 File Offset: 0x0000C9F3
		public void RemoveArtifact(ArtifactIndex artifactIndex)
		{
			if (artifactIndex < ArtifactIndex.Command || artifactIndex >= ArtifactIndex.Count)
			{
				return;
			}
			this.a &= (ushort)(~(ushort)(1 << (int)artifactIndex));
		}

		// Token: 0x060003C2 RID: 962 RVA: 0x0000E814 File Offset: 0x0000CA14
		public static ArtifactMask operator &(ArtifactMask mask1, ArtifactMask mask2)
		{
			return new ArtifactMask
			{
				a = (mask1.a & mask2.a)
			};
		}

		// Token: 0x060003C3 RID: 963 RVA: 0x0000E840 File Offset: 0x0000CA40
		static ArtifactMask()
		{
			for (ArtifactIndex artifactIndex = ArtifactIndex.Command; artifactIndex < ArtifactIndex.Count; artifactIndex++)
			{
				ArtifactMask.all.AddArtifact(artifactIndex);
			}
		}

		// Token: 0x0400034D RID: 845
		[SerializeField]
		public ushort a;

		// Token: 0x0400034E RID: 846
		public static readonly ArtifactMask none;

		// Token: 0x0400034F RID: 847
		public static readonly ArtifactMask all = default(ArtifactMask);
	}
}
