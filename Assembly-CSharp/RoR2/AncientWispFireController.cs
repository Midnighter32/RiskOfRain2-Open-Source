using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000146 RID: 326
	[RequireComponent(typeof(CharacterModel))]
	public class AncientWispFireController : MonoBehaviour
	{
		// Token: 0x060005C8 RID: 1480 RVA: 0x00017F3E File Offset: 0x0001613E
		private void Awake()
		{
			this.characterModel = base.GetComponent<CharacterModel>();
		}

		// Token: 0x060005C9 RID: 1481 RVA: 0x00017F4C File Offset: 0x0001614C
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

		// Token: 0x04000645 RID: 1605
		public ParticleSystem normalParticles;

		// Token: 0x04000646 RID: 1606
		public Light normalLight;

		// Token: 0x04000647 RID: 1607
		public ParticleSystem rageParticles;

		// Token: 0x04000648 RID: 1608
		public Light rageLight;

		// Token: 0x04000649 RID: 1609
		private CharacterModel characterModel;
	}
}
