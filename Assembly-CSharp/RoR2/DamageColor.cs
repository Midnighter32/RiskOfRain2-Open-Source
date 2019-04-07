using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000233 RID: 563
	public static class DamageColor
	{
		// Token: 0x06000AB8 RID: 2744 RVA: 0x00034F9C File Offset: 0x0003319C
		static DamageColor()
		{
			DamageColor.colors[0] = Color.white;
			DamageColor.colors[1] = new Color(0.3647059f, 0.8156863f, 0.14901961f);
			DamageColor.colors[2] = new Color(0.79607844f, 0.1882353f, 0.1882353f);
			DamageColor.colors[3] = new Color(0.827451f, 0.7490196f, 0.3137255f);
			DamageColor.colors[4] = new Color(0.68235296f, 0.9490196f, 0.29803923f);
			DamageColor.colors[5] = new Color(0.9372549f, 0.5176471f, 0.20392157f);
		}

		// Token: 0x06000AB9 RID: 2745 RVA: 0x0003505F File Offset: 0x0003325F
		public static Color FindColor(DamageColorIndex colorIndex)
		{
			if (colorIndex < DamageColorIndex.Default || colorIndex >= DamageColorIndex.Count)
			{
				return Color.white;
			}
			return DamageColor.colors[(int)colorIndex];
		}

		// Token: 0x04000E6C RID: 3692
		private static Color[] colors = new Color[7];
	}
}
