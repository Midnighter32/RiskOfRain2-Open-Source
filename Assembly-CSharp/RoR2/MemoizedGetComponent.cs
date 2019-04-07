using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200045A RID: 1114
	public struct MemoizedGetComponent<TComponent> where TComponent : Component
	{
		// Token: 0x060018EB RID: 6379 RVA: 0x000778F4 File Offset: 0x00075AF4
		public TComponent Get(GameObject gameObject)
		{
			if (this.cachedGameObject != gameObject)
			{
				this.cachedGameObject = gameObject;
				this.cachedValue = (gameObject ? gameObject.GetComponent<TComponent>() : default(TComponent));
			}
			return this.cachedValue;
		}

		// Token: 0x04001C61 RID: 7265
		private GameObject cachedGameObject;

		// Token: 0x04001C62 RID: 7266
		private TComponent cachedValue;
	}
}
