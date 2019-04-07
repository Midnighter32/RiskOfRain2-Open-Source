using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003CF RID: 975
	[Serializable]
	public struct GameObjectToggleGroup
	{
		// Token: 0x04001876 RID: 6262
		public GameObject[] objects;

		// Token: 0x04001877 RID: 6263
		public int minEnabled;

		// Token: 0x04001878 RID: 6264
		public int maxEnabled;
	}
}
