using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020000D0 RID: 208
	public class BuffDef
	{
		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000401 RID: 1025 RVA: 0x0000FE84 File Offset: 0x0000E084
		// (set) Token: 0x06000402 RID: 1026 RVA: 0x0000FE8C File Offset: 0x0000E08C
		public BuffIndex buffIndex { get; set; } = BuffIndex.None;

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x06000403 RID: 1027 RVA: 0x0000FE95 File Offset: 0x0000E095
		public bool isElite
		{
			get
			{
				return this.eliteIndex != EliteIndex.None;
			}
		}

		// Token: 0x040003D4 RID: 980
		public string name;

		// Token: 0x040003D5 RID: 981
		public string iconPath = "Textures/ItemIcons/texNullIcon";

		// Token: 0x040003D6 RID: 982
		public Color buffColor = Color.white;

		// Token: 0x040003D7 RID: 983
		public bool canStack;

		// Token: 0x040003D8 RID: 984
		public EliteIndex eliteIndex = EliteIndex.None;

		// Token: 0x040003D9 RID: 985
		public bool isDebuff;
	}
}
