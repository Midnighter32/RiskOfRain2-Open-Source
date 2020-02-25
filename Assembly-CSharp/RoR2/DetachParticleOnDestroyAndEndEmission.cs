using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001DE RID: 478
	public class DetachParticleOnDestroyAndEndEmission : MonoBehaviour
	{
		// Token: 0x06000A1A RID: 2586 RVA: 0x0002C204 File Offset: 0x0002A404
		private void OnDisable()
		{
			if (this.particleSystem)
			{
				this.particleSystem.emission.enabled = false;
				this.particleSystem.main.stopAction = ParticleSystemStopAction.Destroy;
				this.particleSystem.Stop();
				this.particleSystem.transform.SetParent(null);
			}
		}

		// Token: 0x04000A6C RID: 2668
		public ParticleSystem particleSystem;
	}
}
