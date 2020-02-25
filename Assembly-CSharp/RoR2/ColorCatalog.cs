using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020000E9 RID: 233
	public class ColorCatalog
	{
		// Token: 0x06000494 RID: 1172 RVA: 0x00012520 File Offset: 0x00010720
		static ColorCatalog()
		{
			ColorCatalog.indexToColor32[1] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			ColorCatalog.indexToColor32[2] = new Color32(119, byte.MaxValue, 23, byte.MaxValue);
			ColorCatalog.indexToColor32[3] = new Color32(231, 84, 58, byte.MaxValue);
			ColorCatalog.indexToColor32[4] = new Color32(48, 127, byte.MaxValue, byte.MaxValue);
			ColorCatalog.indexToColor32[5] = new Color32(byte.MaxValue, 128, 0, byte.MaxValue);
			ColorCatalog.indexToColor32[6] = new Color32(235, 232, 122, byte.MaxValue);
			ColorCatalog.indexToColor32[7] = new Color32(231, 84, 58, byte.MaxValue);
			ColorCatalog.indexToColor32[8] = new Color32(239, 235, 26, byte.MaxValue);
			ColorCatalog.indexToColor32[9] = new Color32(206, 41, 41, byte.MaxValue);
			ColorCatalog.indexToColor32[10] = new Color32(100, 100, 100, byte.MaxValue);
			ColorCatalog.indexToColor32[11] = Color32.Lerp(new Color32(142, 56, 206, byte.MaxValue), new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue), 0.575f);
			ColorCatalog.indexToColor32[12] = new Color32(198, 173, 250, byte.MaxValue);
			ColorCatalog.indexToColor32[13] = Color.yellow;
			ColorCatalog.indexToColor32[14] = new Color32(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue);
			ColorCatalog.indexToColor32[15] = new Color32(106, 170, 95, byte.MaxValue);
			ColorCatalog.indexToColor32[16] = new Color32(173, 117, 80, byte.MaxValue);
			ColorCatalog.indexToColor32[17] = new Color32(142, 49, 49, byte.MaxValue);
			ColorCatalog.indexToColor32[18] = new Color32(193, 193, 193, byte.MaxValue);
			ColorCatalog.indexToColor32[19] = new Color32(88, 149, 88, byte.MaxValue);
			ColorCatalog.indexToColor32[20] = new Color32(142, 49, 49, byte.MaxValue);
			ColorCatalog.indexToColor32[21] = new Color32(76, 84, 144, byte.MaxValue);
			ColorCatalog.indexToColor32[22] = new Color32(189, 180, 60, byte.MaxValue);
			ColorCatalog.indexToColor32[23] = new Color32(200, 80, 0, byte.MaxValue);
			for (ColorCatalog.ColorIndex colorIndex = ColorCatalog.ColorIndex.None; colorIndex < ColorCatalog.ColorIndex.Count; colorIndex++)
			{
				ColorCatalog.indexToHexString[(int)colorIndex] = Util.RGBToHex(ColorCatalog.indexToColor32[(int)colorIndex]);
			}
		}

		// Token: 0x06000495 RID: 1173 RVA: 0x000128EF File Offset: 0x00010AEF
		public static Color32 GetColor(ColorCatalog.ColorIndex colorIndex)
		{
			if (colorIndex < ColorCatalog.ColorIndex.None || colorIndex >= ColorCatalog.ColorIndex.Count)
			{
				colorIndex = ColorCatalog.ColorIndex.Error;
			}
			return ColorCatalog.indexToColor32[(int)colorIndex];
		}

		// Token: 0x06000496 RID: 1174 RVA: 0x00012909 File Offset: 0x00010B09
		public static string GetColorHexString(ColorCatalog.ColorIndex colorIndex)
		{
			if (colorIndex < ColorCatalog.ColorIndex.None || colorIndex >= ColorCatalog.ColorIndex.Count)
			{
				colorIndex = ColorCatalog.ColorIndex.Error;
			}
			return ColorCatalog.indexToHexString[(int)colorIndex];
		}

		// Token: 0x06000497 RID: 1175 RVA: 0x0001291F File Offset: 0x00010B1F
		public static Color GetMultiplayerColor(int playerSlot)
		{
			if (playerSlot >= 0 && playerSlot < ColorCatalog.multiplayerColors.Length)
			{
				return ColorCatalog.multiplayerColors[playerSlot];
			}
			return Color.black;
		}

		// Token: 0x04000441 RID: 1089
		private static readonly Color32[] indexToColor32 = new Color32[24];

		// Token: 0x04000442 RID: 1090
		private static readonly string[] indexToHexString = new string[24];

		// Token: 0x04000443 RID: 1091
		private static readonly Color[] multiplayerColors = new Color[]
		{
			new Color32(252, 62, 62, byte.MaxValue),
			new Color32(62, 109, 252, byte.MaxValue),
			new Color32(129, 252, 62, byte.MaxValue),
			new Color32(252, 241, 62, byte.MaxValue)
		};

		// Token: 0x020000EA RID: 234
		public enum ColorIndex
		{
			// Token: 0x04000445 RID: 1093
			None,
			// Token: 0x04000446 RID: 1094
			Tier1Item,
			// Token: 0x04000447 RID: 1095
			Tier2Item,
			// Token: 0x04000448 RID: 1096
			Tier3Item,
			// Token: 0x04000449 RID: 1097
			LunarItem,
			// Token: 0x0400044A RID: 1098
			Equipment,
			// Token: 0x0400044B RID: 1099
			Interactable,
			// Token: 0x0400044C RID: 1100
			Teleporter,
			// Token: 0x0400044D RID: 1101
			Money,
			// Token: 0x0400044E RID: 1102
			Blood,
			// Token: 0x0400044F RID: 1103
			Unaffordable,
			// Token: 0x04000450 RID: 1104
			Unlockable,
			// Token: 0x04000451 RID: 1105
			LunarCoin,
			// Token: 0x04000452 RID: 1106
			BossItem,
			// Token: 0x04000453 RID: 1107
			Error,
			// Token: 0x04000454 RID: 1108
			EasyDifficulty,
			// Token: 0x04000455 RID: 1109
			NormalDifficulty,
			// Token: 0x04000456 RID: 1110
			HardDifficulty,
			// Token: 0x04000457 RID: 1111
			Tier1ItemDark,
			// Token: 0x04000458 RID: 1112
			Tier2ItemDark,
			// Token: 0x04000459 RID: 1113
			Tier3ItemDark,
			// Token: 0x0400045A RID: 1114
			LunarItemDark,
			// Token: 0x0400045B RID: 1115
			BossItemDark,
			// Token: 0x0400045C RID: 1116
			WIP,
			// Token: 0x0400045D RID: 1117
			Count
		}
	}
}
