using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000382 RID: 898
	[RequireComponent(typeof(EffectComponent))]
	public class ParticleSystemColorFromEffectData : MonoBehaviour
	{
		// Token: 0x060012B7 RID: 4791 RVA: 0x0005BCD8 File Offset: 0x00059ED8
		private void Start()
		{
			Color color = this.effectComponent.effectData.color;
			for (int i = 0; i < this.particleSystems.Length; i++)
			{
				this.particleSystems[i].main.startColor = color;
				this.particleSystems[i].Clear();
				this.particleSystems[i].Play();
			}
		}

		// Token: 0x04001687 RID: 5767
		public ParticleSystem[] particleSystems;

		// Token: 0x04001688 RID: 5768
		public EffectComponent effectComponent;
	}
}
