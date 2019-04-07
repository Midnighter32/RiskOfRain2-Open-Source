using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001FE RID: 510
	[Serializable]
	public struct ArtifactMask
	{
		// Token: 0x060009F9 RID: 2553 RVA: 0x00031AC8 File Offset: 0x0002FCC8
		public bool HasArtifact(ArtifactIndex artifactIndex)
		{
			return artifactIndex >= ArtifactIndex.Command && artifactIndex < ArtifactIndex.Count && ((int)this.a & 1 << (int)artifactIndex) != 0;
		}

		// Token: 0x060009FA RID: 2554 RVA: 0x00031AE4 File Offset: 0x0002FCE4
		public void AddArtifact(ArtifactIndex artifactIndex)
		{
			if (artifactIndex < ArtifactIndex.Command || artifactIndex >= ArtifactIndex.Count)
			{
				return;
			}
			this.a |= (ushort)(1 << (int)artifactIndex);
		}

		// Token: 0x060009FB RID: 2555 RVA: 0x00031B04 File Offset: 0x0002FD04
		public void ToggleArtifact(ArtifactIndex artifactIndex)
		{
			if (artifactIndex < ArtifactIndex.Command || artifactIndex >= ArtifactIndex.Count)
			{
				return;
			}
			this.a ^= (ushort)(1 << (int)artifactIndex);
		}

		// Token: 0x060009FC RID: 2556 RVA: 0x00031B24 File Offset: 0x0002FD24
		public void RemoveArtifact(ArtifactIndex artifactIndex)
		{
			if (artifactIndex < ArtifactIndex.Command || artifactIndex >= ArtifactIndex.Count)
			{
				return;
			}
			this.a &= (ushort)(~(ushort)(1 << (int)artifactIndex));
		}

		// Token: 0x060009FD RID: 2557 RVA: 0x00031B48 File Offset: 0x0002FD48
		public static ArtifactMask operator &(ArtifactMask mask1, ArtifactMask mask2)
		{
			return new ArtifactMask
			{
				a = (mask1.a & mask2.a)
			};
		}

		// Token: 0x060009FE RID: 2558 RVA: 0x00031B74 File Offset: 0x0002FD74
		static ArtifactMask()
		{
			for (ArtifactIndex artifactIndex = ArtifactIndex.Command; artifactIndex < ArtifactIndex.Count; artifactIndex++)
			{
				ArtifactMask.all.AddArtifact(artifactIndex);
			}
		}

		// Token: 0x04000D3D RID: 3389
		[SerializeField]
		public ushort a;

		// Token: 0x04000D3E RID: 3390
		public static readonly ArtifactMask none;

		// Token: 0x04000D3F RID: 3391
		public static readonly ArtifactMask all = default(ArtifactMask);
	}
}
