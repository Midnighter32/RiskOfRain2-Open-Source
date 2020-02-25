using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005FB RID: 1531
	public class MPEventSystemProvider : MonoBehaviour
	{
		// Token: 0x170003C9 RID: 969
		// (get) Token: 0x06002454 RID: 9300 RVA: 0x0009E9F7 File Offset: 0x0009CBF7
		public MPEventSystem resolvedEventSystem
		{
			get
			{
				if (this.eventSystem)
				{
					return this.eventSystem;
				}
				if (this.fallBackToMainEventSystem)
				{
					return MPEventSystemManager.primaryEventSystem;
				}
				return null;
			}
		}

		// Token: 0x0400222A RID: 8746
		public MPEventSystem eventSystem;

		// Token: 0x0400222B RID: 8747
		public bool fallBackToMainEventSystem = true;
	}
}
