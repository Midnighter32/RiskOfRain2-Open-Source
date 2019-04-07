using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000475 RID: 1141
	public class RemapLightIntensityToParticleAlpha : MonoBehaviour
	{
		// Token: 0x0600196B RID: 6507 RVA: 0x0007996C File Offset: 0x00077B6C
		private void LateUpdate()
		{
			ParticleSystem.MainModule main = this.particleSystem.main;
			ParticleSystem.MinMaxGradient startColor = main.startColor;
			Color color = startColor.color;
			color.a = Util.Remap(this.light.intensity, this.lowerLightIntensity, this.upperLightIntensity, this.lowerParticleAlpha, this.upperParticleAlpha);
			startColor.color = color;
			main.startColor = startColor;
		}

		// Token: 0x04001CE7 RID: 7399
		public Light light;

		// Token: 0x04001CE8 RID: 7400
		public ParticleSystem particleSystem;

		// Token: 0x04001CE9 RID: 7401
		public float lowerLightIntensity;

		// Token: 0x04001CEA RID: 7402
		public float upperLightIntensity = 1f;

		// Token: 0x04001CEB RID: 7403
		public float lowerParticleAlpha;

		// Token: 0x04001CEC RID: 7404
		public float upperParticleAlpha = 1f;
	}
}
