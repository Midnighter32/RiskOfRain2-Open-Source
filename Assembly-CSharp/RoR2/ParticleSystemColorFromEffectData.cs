using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020002BA RID: 698
	[RequireComponent(typeof(EffectComponent))]
	public class ParticleSystemColorFromEffectData : MonoBehaviour
	{
		// Token: 0x06000FC3 RID: 4035 RVA: 0x0004533C File Offset: 0x0004353C
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

		// Token: 0x04000F44 RID: 3908
		public ParticleSystem[] particleSystems;

		// Token: 0x04000F45 RID: 3909
		public EffectComponent effectComponent;
	}
}
