using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000255 RID: 597
	[RequireComponent(typeof(CharacterModel))]
	public class AncientWispFireController : MonoBehaviour
	{
		// Token: 0x06000B20 RID: 2848 RVA: 0x00037417 File Offset: 0x00035617
		private void Awake()
		{
			this.characterModel = base.GetComponent<CharacterModel>();
		}

		// Token: 0x06000B21 RID: 2849 RVA: 0x00037428 File Offset: 0x00035628
		private void Update()
		{
			bool flag = false;
			CharacterBody body = this.characterModel.body;
			if (body)
			{
				flag = body.HasBuff(BuffIndex.EnrageAncientWisp);
			}
			if (this.normalParticles)
			{
				ParticleSystem.EmissionModule emission = this.normalParticles.emission;
				if (emission.enabled == flag)
				{
					emission.enabled = !flag;
					if (!flag)
					{
						this.normalParticles.Play();
					}
				}
			}
			if (this.rageParticles)
			{
				ParticleSystem.EmissionModule emission2 = this.rageParticles.emission;
				if (emission2.enabled != flag)
				{
					emission2.enabled = flag;
					if (flag)
					{
						this.rageParticles.Play();
					}
				}
			}
			if (this.normalLight)
			{
				this.normalLight.enabled = !flag;
			}
			if (this.rageLight)
			{
				this.rageLight.enabled = flag;
			}
		}

		// Token: 0x04000F28 RID: 3880
		public ParticleSystem normalParticles;

		// Token: 0x04000F29 RID: 3881
		public Light normalLight;

		// Token: 0x04000F2A RID: 3882
		public ParticleSystem rageParticles;

		// Token: 0x04000F2B RID: 3883
		public Light rageLight;

		// Token: 0x04000F2C RID: 3884
		private CharacterModel characterModel;
	}
}
