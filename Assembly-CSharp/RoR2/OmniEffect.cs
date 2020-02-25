using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002AA RID: 682
	public class OmniEffect : MonoBehaviour
	{
		// Token: 0x06000F8F RID: 3983 RVA: 0x00044518 File Offset: 0x00042718
		private void Start()
		{
			this.radius = base.transform.localScale.x;
			foreach (OmniEffect.OmniEffectGroup omniEffectGroup in this.omniEffectGroups)
			{
				float minimumValidRadius = 0f;
				for (int j = 0; j < omniEffectGroup.omniEffectElements.Length; j++)
				{
					OmniEffect.OmniEffectElement omniEffectElement = omniEffectGroup.omniEffectElements[j];
					if (omniEffectElement.maximumValidRadius >= this.radius)
					{
						omniEffectElement.ProcessEffectElement(this.radius, minimumValidRadius);
						break;
					}
					minimumValidRadius = omniEffectElement.maximumValidRadius;
				}
			}
		}

		// Token: 0x04000EF7 RID: 3831
		public OmniEffect.OmniEffectGroup[] omniEffectGroups;

		// Token: 0x04000EF8 RID: 3832
		private float radius;

		// Token: 0x020002AB RID: 683
		[Serializable]
		public class OmniEffectElement
		{
			// Token: 0x06000F91 RID: 3985 RVA: 0x000445A4 File Offset: 0x000427A4
			public void ProcessEffectElement(float radius, float minimumValidRadius)
			{
				if (this.particleSystem)
				{
					if (this.particleSystemOverrideMaterial)
					{
						ParticleSystemRenderer component = this.particleSystem.GetComponent<ParticleSystemRenderer>();
						if (this.particleSystemOverrideMaterial)
						{
							component.material = this.particleSystemOverrideMaterial;
						}
					}
					ParticleSystem.EmissionModule emission = this.particleSystem.emission;
					int num = (int)emission.GetBurst(0).maxCount + (int)((radius - minimumValidRadius) * this.bonusEmissionPerBonusRadius);
					emission.SetBurst(0, new ParticleSystem.Burst(0f, (float)num));
					if (this.particleSystemEmitParticles)
					{
						this.particleSystem.gameObject.SetActive(true);
						return;
					}
					this.particleSystem.gameObject.SetActive(false);
				}
			}

			// Token: 0x04000EF9 RID: 3833
			public string name;

			// Token: 0x04000EFA RID: 3834
			public ParticleSystem particleSystem;

			// Token: 0x04000EFB RID: 3835
			public bool particleSystemEmitParticles;

			// Token: 0x04000EFC RID: 3836
			public Material particleSystemOverrideMaterial;

			// Token: 0x04000EFD RID: 3837
			public float maximumValidRadius = float.PositiveInfinity;

			// Token: 0x04000EFE RID: 3838
			public float bonusEmissionPerBonusRadius;
		}

		// Token: 0x020002AC RID: 684
		[Serializable]
		public class OmniEffectGroup
		{
			// Token: 0x04000EFF RID: 3839
			public string name;

			// Token: 0x04000F00 RID: 3840
			public OmniEffect.OmniEffectElement[] omniEffectElements;
		}
	}
}
