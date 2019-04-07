using System;
using UnityEngine;

namespace RoR2.GameBehaviors
{
	// Token: 0x0200056E RID: 1390
	[RequireComponent(typeof(EffectComponent))]
	internal class ImpactEffect : MonoBehaviour
	{
		// Token: 0x06001EEB RID: 7915 RVA: 0x00091D90 File Offset: 0x0008FF90
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

		// Token: 0x040021A2 RID: 8610
		public ParticleSystem[] particleSystems;
	}
}
