using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000117 RID: 279
	public static class DamageColor
	{
		// Token: 0x0600051C RID: 1308 RVA: 0x000146A8 File Offset: 0x000128A8
		static DamageColor()
		{
			DamageColor.colors[0] = Color.white;
			DamageColor.colors[1] = new Color(0.3647059f, 0.8156863f, 0.14901961f);
			DamageColor.colors[2] = new Color(0.79607844f, 0.1882353f, 0.1882353f);
			DamageColor.colors[3] = new Color(0.827451f, 0.7490196f, 0.3137255f);
			DamageColor.colors[4] = new Color(0.79352224f, 0.96862745f, 0.34901962f);
			DamageColor.colors[5] = new Color(0.9372549f, 0.5176471f, 0.20392157f);
		}

		// Token: 0x0600051D RID: 1309 RVA: 0x0001476B File Offset: 0x0001296B
		public static Color FindColor(DamageColorIndex colorIndex)
		{
			if (colorIndex < DamageColorIndex.Default || colorIndex >= DamageColorIndex.Count)
			{
				return Color.white;
			}
			return DamageColor.colors[(int)colorIndex];
		}

		// Token: 0x04000533 RID: 1331
		private static Color[] colors = new Color[7];
	}
}
