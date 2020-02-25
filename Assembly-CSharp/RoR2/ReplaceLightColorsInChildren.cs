using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002E2 RID: 738
	public class ReplaceLightColorsInChildren : MonoBehaviour
	{
		// Token: 0x060010EA RID: 4330 RVA: 0x0004A49C File Offset: 0x0004869C
		private void Awake()
		{
			foreach (Light light in base.GetComponentsInChildren<Light>())
			{
				light.color = this.newLightColor;
				light.intensity *= this.intensityMultiplier;
			}
			if (this.newParticleSystemMaterial)
			{
				ParticleSystem[] componentsInChildren2 = base.GetComponentsInChildren<ParticleSystem>();
				for (int i = 0; i < componentsInChildren2.Length; i++)
				{
					ParticleSystemRenderer component = componentsInChildren2[i].GetComponent<ParticleSystemRenderer>();
					if (component)
					{
						component.material = this.newParticleSystemMaterial;
					}
				}
			}
		}

		// Token: 0x060010EB RID: 4331 RVA: 0x0000409B File Offset: 0x0000229B
		private void Update()
		{
		}

		// Token: 0x04001049 RID: 4169
		public Color newLightColor;

		// Token: 0x0400104A RID: 4170
		public float intensityMultiplier;

		// Token: 0x0400104B RID: 4171
		public Material newParticleSystemMaterial;
	}
}
