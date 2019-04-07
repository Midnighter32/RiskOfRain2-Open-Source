using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001FF RID: 511
	public class ArtifactDef
	{
		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x060009FF RID: 2559 RVA: 0x00031BA3 File Offset: 0x0002FDA3
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

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000A00 RID: 2560 RVA: 0x00031BBF File Offset: 0x0002FDBF
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

		// Token: 0x04000D40 RID: 3392
		public string nameToken;

		// Token: 0x04000D41 RID: 3393
		public string unlockableName = "";

		// Token: 0x04000D42 RID: 3394
		public string smallIconSelectedPath;

		// Token: 0x04000D43 RID: 3395
		public string smallIconDeselectedPath;

		// Token: 0x04000D44 RID: 3396
		public string descriptionToken;
	}
}
