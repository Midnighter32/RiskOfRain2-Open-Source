using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x020005FA RID: 1530
	public class MPEventSystemLocator : MonoBehaviour
	{
		// Token: 0x170003C7 RID: 967
		// (get) Token: 0x0600244F RID: 9295 RVA: 0x0009E9BC File Offset: 0x0009CBBC
		// (set) Token: 0x06002450 RID: 9296 RVA: 0x0009E9C4 File Offset: 0x0009CBC4
		public MPEventSystemProvider eventSystemProvider { get; private set; }

		// Token: 0x06002451 RID: 9297 RVA: 0x0009E9CD File Offset: 0x0009CBCD
		private void Awake()
		{
			this.eventSystemProvider = base.GetComponentInParent<MPEventSystemProvider>();
		}

		// Token: 0x170003C8 RID: 968
		// (get) Token: 0x06002452 RID: 9298 RVA: 0x0009E9DB File Offset: 0x0009CBDB
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
