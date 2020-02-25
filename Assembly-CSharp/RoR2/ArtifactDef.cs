using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020000C0 RID: 192
	public class ArtifactDef
	{
		// Token: 0x17000081 RID: 129
		// (get) Token: 0x060003C4 RID: 964 RVA: 0x0000E86F File Offset: 0x0000CA6F
		public Sprite smallIconSelectedSprite
		{
			get
			{
				if (!string.IsNullOrEmpty(this.smallIconSelectedPath))
				{
					return Resources.Load<Sprite>(this.smallIconSelectedPath);
				}
				return null;
			}
		}

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x060003C5 RID: 965 RVA: 0x0000E88B File Offset: 0x0000CA8B
		public Sprite smallIconDeselectedSprite
		{
			get
			{
				if (!string.IsNullOrEmpty(this.smallIconDeselectedPath))
				{
					return Resources.Load<Sprite>(this.smallIconDeselectedPath);
				}
				return null;
			}
		}

		// Token: 0x04000350 RID: 848
		public string nameToken;

		// Token: 0x04000351 RID: 849
		public string unlockableName = "";

		// Token: 0x04000352 RID: 850
		public string smallIconSelectedPath;

		// Token: 0x04000353 RID: 851
		public string smallIconDeselectedPath;

		// Token: 0x04000354 RID: 852
		public string descriptionToken;
	}
}
