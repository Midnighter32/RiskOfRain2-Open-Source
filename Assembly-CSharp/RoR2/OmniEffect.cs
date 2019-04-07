using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000376 RID: 886
	public class OmniEffect : MonoBehaviour
	{
		// Token: 0x06001294 RID: 4756 RVA: 0x0005B068 File Offset: 0x00059268
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

		// Token: 0x0400163C RID: 5692
		public OmniEffect.OmniEffectGroup[] omniEffectGroups;

		// Token: 0x0400163D RID: 5693
		private float radius;

		// Token: 0x02000377 RID: 887
		[Serializable]
		public class OmniEffectElement
		{
			// Token: 0x06001296 RID: 4758 RVA: 0x0005B0F4 File Offset: 0x000592F4
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

			// Token: 0x0400163E RID: 5694
			public string name;

			// Token: 0x0400163F RID: 5695
			public ParticleSystem particleSystem;

			// Token: 0x04001640 RID: 5696
			public bool particleSystemEmitParticles;

			// Token: 0x04001641 RID: 5697
			public Material particleSystemOverrideMaterial;

			// Token: 0x04001642 RID: 5698
			public float maximumValidRadius = float.PositiveInfinity;

			// Token: 0x04001643 RID: 5699
			public float bonusEmissionPerBonusRadius;
		}

		// Token: 0x02000378 RID: 888
		[Serializable]
		public class OmniEffectGroup
		{
			// Token: 0x04001644 RID: 5700
			public string name;

			// Token: 0x04001645 RID: 5701
			public OmniEffect.OmniEffectElement[] omniEffectElements;
		}
	}
}
