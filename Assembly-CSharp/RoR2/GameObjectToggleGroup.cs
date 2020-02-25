using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200031D RID: 797
	[Serializable]
	public struct GameObjectToggleGroup
	{
		// Token: 0x04001199 RID: 4505
		public GameObject[] objects;

		// Token: 0x0400119A RID: 4506
		public int minEnabled;

		// Token: 0x0400119B RID: 4507
		public int maxEnabled;
	}
}
