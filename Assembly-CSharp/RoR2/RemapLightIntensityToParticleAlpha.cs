using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003F8 RID: 1016
	public class RemapLightIntensityToParticleAlpha : MonoBehaviour
	{
		// Token: 0x0600188D RID: 6285 RVA: 0x00069D08 File Offset: 0x00067F08
		private void LateUpdate()
		{
			ParticleSystem.MainModule main = this.particleSystem.main;
			ParticleSystem.MinMaxGradient startColor = main.startColor;
			Color color = startColor.color;
			color.a = Util.Remap(this.light.intensity, this.lowerLightIntensity, this.upperLightIntensity, this.lowerParticleAlpha, this.upperParticleAlpha);
			startColor.color = color;
			main.startColor = startColor;
		}

		// Token: 0x04001710 RID: 5904
		public Light light;

		// Token: 0x04001711 RID: 5905
		public ParticleSystem particleSystem;

		// Token: 0x04001712 RID: 5906
		public float lowerLightIntensity;

		// Token: 0x04001713 RID: 5907
		public float upperLightIntensity = 1f;

		// Token: 0x04001714 RID: 5908
		public float lowerParticleAlpha;

		// Token: 0x04001715 RID: 5909
		public float upperParticleAlpha = 1f;
	}
}
