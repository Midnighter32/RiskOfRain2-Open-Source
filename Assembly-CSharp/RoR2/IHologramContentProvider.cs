using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200024F RID: 591
	internal interface IHologramContentProvider
	{
		// Token: 0x06000D06 RID: 3334
		bool ShouldDisplayHologram(GameObject viewer);

		// Token: 0x06000D07 RID: 3335
		GameObject GetHologramContentPrefab();

		// Token: 0x06000D08 RID: 3336
		void UpdateHologramContent(GameObject hologramContentObject);
	}
}
