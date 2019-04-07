using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200023B RID: 571
	public class DifficultyDef
	{
		// Token: 0x06000AC9 RID: 2761 RVA: 0x000354E0 File Offset: 0x000336E0
		public DifficultyDef(float scalingValue, string nameToken, string iconPath, string descriptionToken, Color color)
		{
			this.scalingValue = scalingValue;
			this.descriptionToken = descriptionToken;
			this.nameToken = nameToken;
			this.iconPath = iconPath;
			this.color = color;
		}

		// Token: 0x06000ACA RID: 2762 RVA: 0x0003550D File Offset: 0x0003370D
		public Sprite GetIconSprite()
		{
			if (!this.foundIconSprite)
			{
				this.iconSprite = Resources.Load<Sprite>(this.iconPath);
				this.foundIconSprite = this.iconSprite;
			}
			return this.iconSprite;
		}

		// Token: 0x04000E8C RID: 3724
		public readonly float scalingValue;

		// Token: 0x04000E8D RID: 3725
		public readonly string descriptionToken;

		// Token: 0x04000E8E RID: 3726
		public readonly string nameToken;

		// Token: 0x04000E8F RID: 3727
		public readonly string iconPath;

		// Token: 0x04000E90 RID: 3728
		public readonly Color color;

		// Token: 0x04000E91 RID: 3729
		private Sprite iconSprite;

		// Token: 0x04000E92 RID: 3730
		private bool foundIconSprite;
	}
}
