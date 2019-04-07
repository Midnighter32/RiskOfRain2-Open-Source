using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200060C RID: 1548
	public class MPEventSystemProvider : MonoBehaviour
	{
		// Token: 0x17000313 RID: 787
		// (get) Token: 0x060022E4 RID: 8932 RVA: 0x000A4887 File Offset: 0x000A2A87
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

		// Token: 0x040025E6 RID: 9702
		public MPEventSystem eventSystem;

		// Token: 0x040025E7 RID: 9703
		public bool fallBackToMainEventSystem = true;
	}
}
