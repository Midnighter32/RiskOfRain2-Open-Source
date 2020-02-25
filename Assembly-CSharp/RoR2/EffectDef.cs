using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000124 RID: 292
	public class EffectDef
	{
		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x06000538 RID: 1336 RVA: 0x000151C9 File Offset: 0x000133C9
		// (set) Token: 0x06000539 RID: 1337 RVA: 0x000151D1 File Offset: 0x000133D1
		public EffectIndex index { get; set; } = EffectIndex.Invalid;

		// Token: 0x04000572 RID: 1394
		public GameObject prefab;

		// Token: 0x04000573 RID: 1395
		public EffectComponent prefabEffectComponent;

		// Token: 0x04000574 RID: 1396
		public VFXAttributes prefabVfxAttributes;

		// Token: 0x04000575 RID: 1397
		public string prefabName;

		// Token: 0x04000576 RID: 1398
		public string spawnSoundEventName;

		// Token: 0x04000577 RID: 1399
		public Func<EffectData, bool> cullMethod;
	}
}
