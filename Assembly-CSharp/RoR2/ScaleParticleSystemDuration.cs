using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003C3 RID: 963
	public class ScaleParticleSystemDuration : MonoBehaviour
	{
		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x060014ED RID: 5357 RVA: 0x00064A71 File Offset: 0x00062C71
		// (set) Token: 0x060014EC RID: 5356 RVA: 0x00064A59 File Offset: 0x00062C59
		public float newDuration
		{
			get
			{
				return this._newDuration;
			}
			set
			{
				if (this._newDuration != value)
				{
					this._newDuration = value;
					this.UpdateParticleDurations();
				}
			}
		}

		// Token: 0x060014EE RID: 5358 RVA: 0x00064A79 File Offset: 0x00062C79
		private void Start()
		{
			this.UpdateParticleDurations();
		}

		// Token: 0x060014EF RID: 5359 RVA: 0x00064A84 File Offset: 0x00062C84
		private void UpdateParticleDurations()
		{
			float simulationSpeed = this.initialDuration / this._newDuration;
			for (int i = 0; i < this.particleSystems.Length; i++)
			{
				ParticleSystem particleSystem = this.particleSystems[i];
				if (particleSystem)
				{
					particleSystem.main.simulationSpeed = simulationSpeed;
				}
			}
		}

		// Token: 0x04001844 RID: 6212
		public float initialDuration = 1f;

		// Token: 0x04001845 RID: 6213
		private float _newDuration = 1f;

		// Token: 0x04001846 RID: 6214
		public ParticleSystem[] particleSystems;
	}
}
