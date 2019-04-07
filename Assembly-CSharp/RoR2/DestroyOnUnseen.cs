using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002CD RID: 717
	public class DestroyOnUnseen : MonoBehaviour
	{
		// Token: 0x06000E72 RID: 3698 RVA: 0x000473DC File Offset: 0x000455DC
		private void Start()
		{
			this.rend = base.GetComponentInChildren<Renderer>();
		}

		// Token: 0x06000E73 RID: 3699 RVA: 0x000473EA File Offset: 0x000455EA
		private void Update()
		{
			if (this.cull && this.rend && !this.rend.isVisible)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x0400127B RID: 4731
		public bool cull;

		// Token: 0x0400127C RID: 4732
		private Renderer rend;
	}
}
