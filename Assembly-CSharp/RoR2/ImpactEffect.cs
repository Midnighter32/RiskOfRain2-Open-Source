using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000259 RID: 601
	[RequireComponent(typeof(EffectComponent))]
	internal class ImpactEffect : MonoBehaviour
	{
		// Token: 0x06000D26 RID: 3366 RVA: 0x0003B2BC File Offset: 0x000394BC
		private void Start()
		{
			EffectComponent component = base.GetComponent<EffectComponent>();
			Color color = (component.effectData != null) ? component.effectData.color : Color.white;
			for (int i = 0; i < this.particleSystems.Length; i++)
			{
				this.particleSystems[i].main.startColor = color;
				this.particleSystems[i].Play();
			}
		}

		// Token: 0x04000D55 RID: 3413
		public ParticleSystem[] particleSystems;
	}
}
