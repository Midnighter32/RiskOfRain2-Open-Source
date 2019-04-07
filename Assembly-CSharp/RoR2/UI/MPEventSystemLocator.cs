using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200060B RID: 1547
	public class MPEventSystemLocator : MonoBehaviour
	{
		// Token: 0x17000311 RID: 785
		// (get) Token: 0x060022DF RID: 8927 RVA: 0x000A484C File Offset: 0x000A2A4C
		// (set) Token: 0x060022E0 RID: 8928 RVA: 0x000A4854 File Offset: 0x000A2A54
		public MPEventSystemProvider eventSystemProvider { get; private set; }

		// Token: 0x060022E1 RID: 8929 RVA: 0x000A485D File Offset: 0x000A2A5D
		private void Awake()
		{
			this.eventSystemProvider = base.GetComponentInParent<MPEventSystemProvider>();
		}

		// Token: 0x17000312 RID: 786
		// (get) Token: 0x060022E2 RID: 8930 RVA: 0x000A486B File Offset: 0x000A2A6B
		public MPEventSystem eventSystem
		{
			get
			{
				if (!this.eventSystemProvider)
				{
					return null;
				}
				return this.eventSystemProvider.resolvedEventSystem;
			}
		}
	}
}
