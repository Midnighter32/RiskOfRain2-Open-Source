using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003D0 RID: 976
	public struct MemoizedGetComponent<TComponent> where TComponent : Component
	{
		// Token: 0x060017BD RID: 6077 RVA: 0x00067164 File Offset: 0x00065364
		public TComponent Get(GameObject gameObject)
		{
			if (this.cachedGameObject != gameObject)
			{
				this.cachedGameObject = gameObject;
				this.cachedValue = (gameObject ? gameObject.GetComponent<TComponent>() : default(TComponent));
			}
			return this.cachedValue;
		}

		// Token: 0x04001660 RID: 5728
		private GameObject cachedGameObject;

		// Token: 0x04001661 RID: 5729
		private TComponent cachedValue;
	}
}
