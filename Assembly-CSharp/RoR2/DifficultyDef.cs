using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000121 RID: 289
	public class DifficultyDef
	{
		// Token: 0x06000534 RID: 1332 RVA: 0x000150BF File Offset: 0x000132BF
		public DifficultyDef(float scalingValue, string nameToken, string iconPath, string descriptionToken, Color color)
		{
			this.scalingValue = scalingValue;
			this.descriptionToken = descriptionToken;
			this.nameToken = nameToken;
			this.iconPath = iconPath;
			this.color = color;
		}

		// Token: 0x06000535 RID: 1333 RVA: 0x000150EC File Offset: 0x000132EC
		public Sprite GetIconSprite()
		{
			if (!this.foundIconSprite)
			{
				this.iconSprite = Resources.Load<Sprite>(this.iconPath);
				this.foundIconSprite = this.iconSprite;
			}
			return this.iconSprite;
		}

		// Token: 0x04000567 RID: 1383
		public readonly float scalingValue;

		// Token: 0x04000568 RID: 1384
		public readonly string descriptionToken;

		// Token: 0x04000569 RID: 1385
		public readonly string nameToken;

		// Token: 0x0400056A RID: 1386
		public readonly string iconPath;

		// Token: 0x0400056B RID: 1387
		public readonly Color color;

		// Token: 0x0400056C RID: 1388
		private Sprite iconSprite;

		// Token: 0x0400056D RID: 1389
		private bool foundIconSprite;
	}
}
