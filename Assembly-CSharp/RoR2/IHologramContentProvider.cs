using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200032D RID: 813
	internal interface IHologramContentProvider
	{
		// Token: 0x060010BA RID: 4282
		bool ShouldDisplayHologram(GameObject viewer);

		// Token: 0x060010BB RID: 4283
		GameObject GetHologramContentPrefab();

		// Token: 0x060010BC RID: 4284
		void UpdateHologramContent(GameObject hologramContentObject);
	}
}
