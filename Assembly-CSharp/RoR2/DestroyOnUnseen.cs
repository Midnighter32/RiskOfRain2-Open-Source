using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001DD RID: 477
	public class DestroyOnUnseen : MonoBehaviour
	{
		// Token: 0x06000A17 RID: 2583 RVA: 0x0002C1C5 File Offset: 0x0002A3C5
		private void Start()
		{
			this.rend = base.GetComponentInChildren<Renderer>();
		}

		// Token: 0x06000A18 RID: 2584 RVA: 0x0002C1D3 File Offset: 0x0002A3D3
		private void Update()
		{
			if (this.cull && this.rend && !this.rend.isVisible)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x04000A6A RID: 2666
		public bool cull;

		// Token: 0x04000A6B RID: 2667
		private Renderer rend;
	}
}
