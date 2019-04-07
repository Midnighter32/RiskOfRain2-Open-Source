using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002CE RID: 718
	public class DetachParticleOnDestroyAndEndEmission : MonoBehaviour
	{
		// Token: 0x06000E75 RID: 3701 RVA: 0x0004741C File Offset: 0x0004561C
		private void OnDisable()
		{
			if (this.particleSystem)
			{
				this.particleSystem.emission.enabled = false;
				this.particleSystem.Stop();
				this.particleSystem.transform.SetParent(null);
			}
		}

		// Token: 0x0400127D RID: 4733
		public ParticleSystem particleSystem;
	}
}
